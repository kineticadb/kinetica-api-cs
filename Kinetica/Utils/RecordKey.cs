using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace kinetica.Utils
{
    /// <summary>
    /// A key based on a given record that serves as either a primary key
    /// or a shard key.  The <see cref="RecordKeyBuilder{T}"/> class creates
    /// these record keys.
    /// </summary>
    internal sealed class RecordKey
    {
        /// <summary>
        /// YYYY-MM-DD
        /// </summary>
        private static readonly Regex DATE_REGEX = new Regex("\\A(\\d{4})-(\\d{2})-(\\d{2})$");

        /// <summary>
        /// YYYY-MM-DD [HH:MM:SS[.mmm]]
        /// </summary>
        private static readonly Regex DATETIME_REGEX = new Regex("\\A(?<year>\\d{4})-(?<month>\\d{2})-(?<day>\\d{2})(?<time>\\s+(?<hour>\\d{1,2}):(?<min>\\d{2}):(?<sec>\\d{2})(?:\\.(?<ms>\\d{1,6}))?)?$");

        /// <summary>
        /// Decimal number with upto 19 digits of precision and 4 digits of scale
        /// </summary>
        private static readonly Regex DECIMAL_REGEX = new Regex("\\A\\s*(?<sign>[+-]?)((?<int>\\d+)(\\.(?<intfrac>\\d{0,4}))?|\\.(?<onlyfrac>\\d{1,4}))\\s*\\z");

        /// <summary>
        /// xxx.xxx.xxx.xxx (where xxx is in the range [0, 255])
        /// </summary>
        private static readonly Regex IPV4_REGEX = new Regex("\\A(?<a>\\d{1,3})\\.(?<b>\\d{1,3})\\.(?<c>\\d{1,3})\\.(?<d>\\d{1,3})$");

        /// <summary>
        /// HH:MM:SS[.mmm]
        /// </summary>
        private static readonly Regex TIME_REGEX = new Regex("\\A(?<hour>\\d{1,2}):(?<minute>\\d{2}):(?<seconds>\\d{2})(\\.(?<milliseconds>\\d{1,3}))?$");

        /// <summary>
        /// The day of the unix epoch (1970-01-01)
        /// </summary>
        private static readonly DateTime EPOCH_DATE = new DateTime(1970, 1, 1);

        /// <summary>
        /// Minimum supported year by Kinetica is 1000
        /// </summary>
        private static readonly int MIN_SUPPORTED_YEAR = 1000;

        /// <summary>
        /// Maximum supported year by Kinetica is 2900
        /// </summary>
        private static readonly int MAX_SUPPORTED_YEAR = 2900;

        /// <summary>
        /// 1900
        /// </summary>
        private static readonly int YEAR_1900 = 1900;

        /// <summary>
        /// The UTC timezone
        /// </summary>
        private static readonly TimeZoneInfo UTC = TimeZoneInfo.Utc;

        private readonly byte[] buffer;
        private readonly int buffer_size;
        private int current_size;
        private int hash_code;
        private bool is_valid;
        private long routingHash;

        /// <summary>
        /// Allocate the buffer for the record key with the given size.
        /// </summary>
        /// <param name="size">The size of the buffer.  Must be greater
        /// than or equal to 1.</param>
        public RecordKey(int size)
        {
            if (size < 1)
                throw new KineticaException("Buffer size must be greater than or equal to 1.  "
                                              + "Size given: " + size);
            buffer_size = size;
            current_size = 0;
            buffer = new byte[size];
            this.is_valid = true;
        }

        /// <summary>
        /// Returns whether the key is valid or not.
        /// </summary>
        /// <returns>A bool indicating whether the key is valid at the moment.</returns>
        public bool isValid()
        {
            return this.is_valid;
        }

        /// <summary>
        /// Returns key's hash code.
        /// </summary>
        /// <returns>The hash code for the key.</returns>
        public int hashCode()
        {
            return this.hash_code;
        }



        /// <summary>
        /// Check whether the buffer is already full.
        /// </summary>
        /// <param name="throw_if_full">Optional value; if true, then
        /// throw if the buffer is full.  Default value is true.</param>
        /// <returns>Indicates if the buffer is full.</returns>
        private bool isBufferFull(bool throw_if_full = true)
        {
            if (this.current_size == this.buffer_size)
            {
                if (throw_if_full)
                    throw new KineticaException("The buffer is already full!");
                return true; // yes, the buffer is full, and we haven't thrown
            }
            return false; // buffer is NOT full
        }  // end isBufferFull

        /// <summary>
        /// Check whether the buffer will overflow if we attempt to add
        /// <paramref name="n"/> more bytes.
        /// </summary>
        /// <param name="n">The number of bytes we're attempting to add to
        /// the buffer.</param>
        /// <param name="throw_if_overflow">Optional boolean flag; if true,
        /// then throw an exception if the buffer has insufficient room in it.
        /// Default value is true.</param>
        /// <returns>Boolean flag indicating if the buffer will overflow upon an
        /// attempt to put <paramref name="n"/> more bytes in it.</returns>
        private bool willBufferOverflow(int n, bool throw_if_overflow = true)
        {
            // Note: We're not checking for a negative value for n here
            if ((this.current_size + n) > this.buffer_size)
            {
                if (throw_if_overflow)
                    throw new KineticaException($"The buffer (of size {buffer_size}) does not have sufficient room in it to put {n} more byte(s) (current size is {this.current_size}).");
                return true; // yes, the buffer WILL overflow, but we haven't thrown
            }
            return false; // buffer will NOT overflow
        }  // end willBufferOverflow


        /// <summary>
        /// Adds a byte to the byte buffer and increments the current size
        /// by one.  Use ONLY this method to add to the buffer; do not add
        /// to buffer in other methods directly.
        /// </summary>
        /// <param name="b">The byte to be added to the buffer.</param>
        private void add(byte b)
        {
            // Add the byte to the buffer and increment the size
            buffer.SetValue(b, current_size++);
        } // end add()



        /// <summary>
        /// Add an integer to the buffer.
        /// </summary>
        /// <param name="value">The integer value to be added.  Can be null.</param>
        public void addInt(int? value)
        {
            // Check if the given number of characters will fit in the buffer
            this.willBufferOverflow(4);  // int is four bytes long

            // Handle nulls
            if (value == null)
            {
                // Add four zero bytes for the null value
                this.add((byte)0);  // 1st 0
                this.add((byte)0);  // 2nd 0
                this.add((byte)0);  // 3rd 0
                this.add((byte)0);  // 4th 0
                return;
            }

            // Put the integer into the array, but first convert to bytes
            byte[] int_bytes = BitConverter.GetBytes((int)value);

            // Add the four bytes
            foreach (byte b in int_bytes)
                this.add(b);
        }  // end addInt


        /// <summary>
        /// Add an 8-bit integer to the buffer.
        /// </summary>
        /// <param name="value">The integer value to be added.  Can be null.</param>
        public void addInt8(int? value)
        {
            // Check if the given number of characters will fit in the buffer
            this.willBufferOverflow(1);  // int8 is one byte long

            // Handle nulls
            if (value == null)
            {
                // Add one zero byte for the null value
                this.add((byte)0);
                return;
            }

            // Put the integer into the array, but first convert to byte
            this.add((byte)value);
        }  // end addInt8


        /// <summary>
        /// Add a short (two bytes) to the buffer.
        /// </summary>
        /// <param name="value">The short value to be added.  Can be null.</param>
        public void addInt16(int? value)
        {
            // Check if the given number of characters will fit in the buffer
            this.willBufferOverflow(2);  // int16 is two bytes long

            // Handle nulls
            if (value == null)
            {
                // Add two zero bytes for the null value
                this.add((byte)0);  // 1st 0
                this.add((byte)0);  // 2nd 0
                return;
            }

            // Put the short into the array, but first convert to bytes
            byte[] short_bytes = BitConverter.GetBytes((short)value);

            // Add the two bytes
            foreach (byte b in short_bytes)
                this.add(b);
        }  // end addInt16



        /// <summary>
        /// Add a long to the buffer.
        /// </summary>
        /// <param name="value">The long value to be added.  Can be null.</param>
        public void addLong(long? value)
        {
            // Check if the given number of characters will fit in the buffer
            this.willBufferOverflow(8);  // int is eight bytes long

            // Handle nulls
            if (value == null)
            {
                // Add four zero bytes for the null value
                this.add((byte)0);  // 1st 0
                this.add((byte)0);  // 2nd 0
                this.add((byte)0);  // 3rd 0
                this.add((byte)0);  // 4th 0
                this.add((byte)0);  // 5th 0
                this.add((byte)0);  // 6th 0
                this.add((byte)0);  // 7th 0
                this.add((byte)0);  // 8th 0
                return;
            }

            // Put the long into the array, but first convert to bytes
            byte[] long_bytes = BitConverter.GetBytes((long)value);

            // Add the eight bytes
            foreach (byte b in long_bytes)
                this.add(b);
        }  // end addLong


        /// <summary>
        /// Add a float to the buffer.
        /// </summary>
        /// <param name="value">The float value to be added.  Can be null.</param>
        public void addFloat(float? value)
        {
            // Check if the given number of characters will fit in the buffer
            this.willBufferOverflow(4);  // int is four bytes long

            // Handle nulls
            if (value == null)
            {
                // Add four zero bytes for the null value
                this.add((byte)0.0f);  // 1st 0
                this.add((byte)0.0f);  // 2nd 0
                this.add((byte)0.0f);  // 3rd 0
                this.add((byte)0.0f);  // 4th 0
                return;
            }

            // Put the integer into the array, but first convert to bytes
            byte[] float_bytes = BitConverter.GetBytes((float)value);

            // Add the four bytes
            foreach (byte b in float_bytes)
                this.add(b);
        }  // end addFloat



        /// <summary>
        /// Add a double to the buffer.
        /// </summary>
        /// <param name="value">The double value to be added.  Can be null.</param>
        public void addDouble(double? value)
        {
            // Check if the given number of characters will fit in the buffer
            this.willBufferOverflow(8);  // int is eight bytes long

            // Handle nulls
            if (value == null)
            {
                // Add four zero bytes for the null value
                this.add((byte)0.0);  // 1st 0
                this.add((byte)0.0);  // 2nd 0
                this.add((byte)0.0);  // 3rd 0
                this.add((byte)0.0);  // 4th 0
                this.add((byte)0.0);  // 5th 0
                this.add((byte)0.0);  // 6th 0
                this.add((byte)0.0);  // 7th 0
                this.add((byte)0.0);  // 8th 0
                return;
            }

            // Put the integer into the array, but first convert to bytes
            byte[] double_bytes = BitConverter.GetBytes((double)value);

            // Add the eight bytes
            foreach (byte b in double_bytes)
                this.add(b);
        }  // end addDouble



        /// <summary>
        /// Add a string to the buffer.  Hash the string value and add it
        /// as a long internally.
        /// </summary>
        /// <param name="value">The string value to be added.  Can be null.</param>
        public void addString(string value)
        {
            // Handle nulls
            if (value == null)
            {
                this.addLong(0L);
                return;
            }

            // Hash the value
            MurMurHash3.LongPair murmur = new MurMurHash3.LongPair();
            System.Text.Encoding encoding = new System.Text.UTF8Encoding();
            byte[] input = encoding.GetBytes(value);
            MurMurHash3.murmurhash3_x64_128(input, 0, (uint)input.Length, 10, out murmur);

            // Add the hashed value to the buffer
            this.addLong(murmur.val1);
        }  // end addString



        /// <summary>
        /// Appends a charN value to the buffer.  If <paramref name="value"/> is longer than
        /// N in length, it gets truncated.  If it is shorter, then the remaining characters
        /// get padded with byte 0.
        /// </summary>
        /// <param name="value">The value to be added to the byte buffer.</param>
        /// <param name="N">The number of characters to be added to the byte buffer.  If N
        /// is greater than what would fit in the buffer, then an exception is thrown.</param>
        public void addCharN(string value, int N)
        {
            // Check if the given number of characters will fit in the buffer
            this.willBufferOverflow(N);
            //// Check if the given number of characters will fit in the buffer
            //if ( ( this.current_size + N ) > buffer_size )
            //    throw new KineticaException( $"The given {N} character(s) will not fit in the buffer (of size {buffer_size}) which has {this.current_size} bytes in it already." );

            // Handle nulls
            if (value == null)
            {
                for (int i = 0; i < N; ++i)
                {
                    this.add((byte)0);
                }
                return;
            }

            // Encode the string into bytes (using the UTF-8 encoding)
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            int byte_count = bytes.GetLength(0);

            // Truncate longer strings to the given length
            if (byte_count > N)
                byte_count = N;

            // Put the characters in the byte buffer in the little endian
            // order (which means it will be right to left)
            // ----------------------------------------------------------
            // First, pad with any zeroes "at the end"
            for (int i = N; i > byte_count; --i)
            {
                this.add((byte)0);
            }

            // Then, put all the characters (in reverse order)
            for (int i = (byte_count - 1); i >= 0; --i)
            {
                this.add(bytes[i]);
            }
        }  // end addCharN()


        /// <summary>
        /// Adds a string to the buffer that has the 'date' property.
        /// Internally, the date is stored as an integer.
        /// </summary>
        /// <param name="value">The date string to be added.  Must have
        /// the YYYY-MM-DD format.</param>
        public void addDate(string value)
        {
            // Check and throw if the buffer is already full
            this.isBufferFull(true);

            // Handle nulls
            if (value == null)
            {
                this.addInt(0);
                return;
            }

            // Check that the given value matches the YYYY-MM-DD pattern
            Match match = DATE_REGEX.Match(value);
            if (!match.Success)
            {
                // No match, so the key is invalid
                this.is_valid = false;
                this.addInt(0);
                return;
            }

            // We'll need to parse the string into year, month, and day
            int year, month, day;
            DateTime date;
            System.Globalization.GregorianCalendar calendar = new System.Globalization.GregorianCalendar();

            // Parse the string value
            try
            {
                year = int.Parse(match.Groups[1].ToString());
                month = int.Parse(match.Groups[2].ToString());
                day = int.Parse(match.Groups[3].ToString());
                date = new DateTime(year, month, day, calendar);
            }
            catch (Exception ex)
            {
                // Upon any error, set this key to be invalid
                this.addInt(0);
                this.is_valid = false;
                return;
            }

            // Kinetica does not support years outside the range [1000, 2900]
            if ((year < MIN_SUPPORTED_YEAR) || (year > MAX_SUPPORTED_YEAR))
            {
                this.addInt(0);
                this.is_valid = false;
                return;
            }

            int fixed_day_of_week = ((int)calendar.GetDayOfWeek(date) + 1);

            // Deduce the integer representing the date
            int date_integer = (((year - YEAR_1900) << 21)
                                 | (month << 17)
                                 | (day << 12)
                                 | (calendar.GetDayOfYear(date) << 3)
                                 | fixed_day_of_week);
            this.addInt(date_integer);
        }  // end addDate()


        /// <summary>
        /// Adds a string to the buffer that has the 'datetime' property.
        /// Internally, the date is stored as a long.
        /// </summary>
        /// <param name="value">The datetime string to be added.  Must have
        /// the YYYY-MM-DD HH:MM:SS.mmm format.</param>
        public void addDateTime(string value)
        {
            // Check and throw if the buffer is already full
            this.isBufferFull(true);

            // Handle nulls
            if (value == null)
            {
                this.addLong(0);
                return;
            }

            // Check that the given value matches the YYYY-MM-DD HH:MM:SS.mmm pattern
            Match match = DATETIME_REGEX.Match(value);
            if (!match.Success)
            {
                // No match, so the key is invalid
                this.is_valid = false;
                this.addLong(0);
                return;
            }

            // We'll need to parse the string into year, month, day, hour,
            // minute, second, and millisecond
            int year, month, day;
            int hour = 0;
            int minute = 0;
            int second = 0;
            int msecond = 0;
            DateTime date;
            System.Globalization.GregorianCalendar calendar = new System.Globalization.GregorianCalendar();

            // Parse the string value
            try
            {
                year = int.Parse(match.Groups["year"].Value);
                month = int.Parse(match.Groups["month"].Value);
                day = int.Parse(match.Groups["day"].Value);

                // Handle the optional time part
                Group time_group = match.Groups["time"];
                if (time_group.Success)
                {
                    hour = int.Parse(match.Groups["hour"].Value);
                    minute = int.Parse(match.Groups["min"].Value);
                    second = int.Parse(match.Groups["sec"].Value);

                    // Handle the further optional milliseconds
                    Group ms_group = match.Groups["ms"];
                    if (ms_group.Success)
                    {
                        msecond = int.Parse(match.Groups["ms"].Value);
                        // Need to have the milliseconds be milliseconds (three digits)
                        switch (ms_group.Value.Length)
                        {
                            case 1:
                                msecond *= 100; break;
                            case 2:
                                msecond *= 10; break;
                            // No need for case 3
                            case 4:
                                msecond /= 10; break;
                            case 5:
                                msecond /= 100; break;
                            case 6:
                                msecond /= 1000; break;
                        }
                    }
                }  // end parsing the time component

                // Now put it all together
                date = new DateTime(year, month, day, hour, minute, second, msecond, calendar);
            }
            catch (Exception ex)
            {
                // Upon any error, set this key to be invalid
                this.addLong(0);
                this.is_valid = false;
                return;
            }

            // Kinetica does not support years outside the range [1000, 2900]
            if ((year < MIN_SUPPORTED_YEAR) || (year > MAX_SUPPORTED_YEAR))
            {
                this.addLong(0);
                this.is_valid = false;
                return;
            }

            int fixed_day_of_week = ((int)calendar.GetDayOfWeek(date) + 1);

            // Deduce the integer representing the date
            long datetime_long = (long)((((long)(year - YEAR_1900)) << 53)
                                          | (((long)month) << 49)
                                          | (((long)day) << 44)
                                          | (((long)hour) << 39)
                                          | (((long)minute) << 33)
                                          | (((long)second) << 27)
                                          | (((long)msecond) << 17)
                                          | (((long)calendar.GetDayOfYear(date)) << 8)
                                          | (((long)fixed_day_of_week) << 5));
            this.addLong(datetime_long);
        }  // end addDateTime()


        /// <summary>
        /// Adds a string to the buffer that has the 'decimal' property.
        /// Internally, the date is stored as a long.
        /// </summary>
        /// <param name="value">The date string to be added.  Must have
        /// upto 19 digits of precision and four digits of scale format.</param>
        public void addDecimal(string value)
        {
            // Check and throw if the buffer is already full
            this.isBufferFull(true);

            // Handle nulls
            if (value == null)
            {
                this.addLong(0L);
                return;
            }

            // Check that the given value matches the decimal regular expression pattern
            Match match = DECIMAL_REGEX.Match(value);
            if (!match.Success)
            {
                // No match, so the key is invalid
                this.is_valid = false;
                this.addLong(0L);
                return;
            }

            // Parse the string value
            long decimal_value;
            try
            {
                // Extract the integral and fractional parts
                Group integral_group = match.Groups["int"];
                Group fraction_with_integral_group = match.Groups["intfrac"];
                Group frac_only_group = match.Groups["onlyfrac"];

                if (integral_group.Success)
                {   // Has an integral part to the decimal
                    decimal_value = long.Parse(integral_group.Value);

                    if (fraction_with_integral_group.Success)
                    {   // Also have a fractional part
                        long fraction = 0;
                        // The fraction could be zero in length (i.e. the string ends with the decimal point)
                        if (fraction_with_integral_group.Value.Length > 0)
                            fraction = long.Parse(fraction_with_integral_group.Value);

                        // We need to shift the integral part to the left appropriately
                        // before adding the fraction
                        long integral_part = decimal_value * (long)Math.Pow(10, fraction_with_integral_group.Value.Length);
                        decimal_value = integral_part + fraction;

                        // Shift it further to the left if the fraction is less than 1000
                        switch (fraction_with_integral_group.Value.Length)
                        {
                            case 1:
                                decimal_value *= 1000; break;
                            case 2:
                                decimal_value *= 100; break;
                            case 3:
                                decimal_value *= 10; break;
                        }
                    }
                }
                else if (frac_only_group.Success)
                {  // Only the fractional part is given
                    decimal_value = long.Parse(frac_only_group.Value);

                    // Adjust the value so that it is always four digits long
                    switch (frac_only_group.Value.Length)
                    {
                        case 1:
                            decimal_value *= 1000; break;
                        case 2:
                            decimal_value *= 100; break;
                        case 3:
                            decimal_value *= 10; break;
                    }
                }
                else
                    throw new KineticaException("No match for decimal!");

                // Now handle the sign
                Group sign_group = match.Groups["sign"];
                if (sign_group.Success)
                {   // Needs action only if negative
                    if (sign_group.Value == "-")
                        decimal_value = (-1) * decimal_value;
                }
            }
            catch (Exception ex)
            {
                // Upon any error, set this key to be invalid
                this.addLong(0L);
                this.is_valid = false;
                return;
            }

            // Deduce the integer representing the date
            this.addLong(decimal_value);
        }  // end addDecimal()


        /// <summary>
        /// Adds a string to the buffer that has the 'ipv4' property.
        /// Internally, the IP is stored as an integer.
        /// </summary>
        /// <param name="value">The IPv4 string to be added.  Must have
        /// the XXX.XXX.XXX.XXX format.</param>
        public void addIPv4(string value)
        {
            // Check and throw if the buffer is already full
            this.isBufferFull(true);

            // Handle nulls
            if (value == null)
            {
                this.addInt(0);
                return;
            }

            // Check that the given value matches the XXX.XXX.XXX.XXX pattern
            Match match = IPV4_REGEX.Match(value);
            if (!match.Success)
            {
                // No match, so the key is invalid
                this.is_valid = false;
                this.addInt(0);
                return;
            }

            // We'll need to parse the string into four integers
            int a, b, c, d;

            // Parse the string value
            try
            {
                a = int.Parse(match.Groups["a"].Value);
                b = int.Parse(match.Groups["b"].Value);
                c = int.Parse(match.Groups["c"].Value);
                d = int.Parse(match.Groups["d"].Value);
            }
            catch (Exception ex)
            {
                // Upon any error, set this key to be invalid
                this.addInt(0);
                this.is_valid = false;
                return;
            }

            // Each byte has to be within the range [0, 255] (the regex does
            // not support negative numbers, so no worries about those)
            if ((a > 255) || (b > 255) || (c > 255) || (d > 255))
            {
                this.addInt(0);
                this.is_valid = false;
                return;
            }

            // Deduce the integer representing the date
            int ipv4_integer = ((a << 24) | (b << 16) | (c << 8) | d);
            this.addInt(ipv4_integer);
        }  // end addIPv4()


        /// <summary>
        /// Adds a string to the buffer that has the 'time' property.
        /// Internally, the time is stored as an integer.
        /// </summary>
        /// <param name="value">The time string to be added.  Must have
        /// the HH:MM:SS[.mmm] format.  Milliseconds can have one to three
        /// digits.  HH can be one or two digits.</param>
        public void addTime(string value)
        {
            // Check and throw if the buffer is already full
            this.isBufferFull(true);

            // Handle nulls
            if (value == null)
            {
                this.addInt(0);
                return;
            }

            // Check that the given value matches the HH:MM:SS[.mmm] pattern
            Match match = TIME_REGEX.Match(value);
            if (!match.Success)
            {
                // No match, so the key is invalid
                this.is_valid = false;
                this.addInt(0);
                return;
            }

            // We'll need to parse the string into four integers
            uint hour, minute, second, milliseconds;

            // Parse the string value
            try
            {
                hour = uint.Parse(match.Groups["hour"].Value);
                minute = uint.Parse(match.Groups["minute"].Value);
                second = uint.Parse(match.Groups["seconds"].Value);
                Group msec_group = match.Groups["milliseconds"];

                // Milliseconds are optional
                milliseconds = 0;
                if (msec_group.Success)
                {
                    milliseconds = uint.Parse(msec_group.Value);

                    // Handle single and double digits for milliseconds
                    switch (msec_group.Value.Length)
                    {
                        case 1:
                            milliseconds *= 100; break;
                        case 2:
                            milliseconds *= 10; break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Upon any error, set this key to be invalid
                this.addInt(0);
                this.is_valid = false;
                return;
            }

            // Validate the hour, minute, second values
            if ((hour > 23) || (minute > 59) || (second > 59))
            {
                this.addInt(0);
                this.is_valid = false;
                return;
            }

            // Deduce the integer representing the time
            int time_integer = (int)((hour << 26) | (minute << 20) | (second << 14) | (milliseconds << 4));
            this.addInt(time_integer);
        }  // end addTime()


        /// <summary>
        /// Adds a long to the buffer that has the 'timestamp' property.
        /// </summary>
        /// <param name="value">The timestamp to be added (in milliseconds).</param>
        public void addTimeStamp(long? value)
        {
            // Handle nulls
            if (value == null)
            {
                this.addLong(0);
                return;
            }

            // Encode the timestamp the way the database server does it
            DateTime time = EPOCH_DATE.AddMilliseconds((double)value);
            long fixed_day_of_week = ((long)time.DayOfWeek + 1);

            long timestamp = (long)((((long)(time.Year - YEAR_1900)) << 53)
                                      | (((long)(time.Month)) << 49)
                                      | (((long)time.Day) << 44)
                                      | (((long)time.Hour) << 39)
                                      | (((long)time.Minute) << 33)
                                      | (((long)time.Second) << 27)
                                      | (((long)time.Millisecond) << 17)
                                      | (((long)time.DayOfYear) << 8)
                                      | (fixed_day_of_week << 5));
            this.addLong(timestamp);
        }  // end addTimeStamp()



        /// <summary>
        /// Compute the hash of the key in the buffer.  Use the Murmurhash3
        /// algorithm to compute the hash.  If not all of the values have been
        /// added to the key (i.e. if the buffer is not full), then throw an
        /// exception.
        /// </summary>
        public void computHashes()
        {
            // Check all the values for the key have been added
            if (this.current_size != this.buffer_size)
                throw new KineticaException("The RecordKey buffer is not full; check that all the relevant values have been added.");

            // Hash the value
            MurMurHash3.LongPair murmur = new MurMurHash3.LongPair();
            MurMurHash3.murmurhash3_x64_128(this.buffer, 0, (uint)this.buffer_size, 10, out murmur);

            // Save the hash value
            this.routingHash = murmur.val1;
            this.hash_code = (int)(this.routingHash ^ ((this.routingHash >> 32) & 0x0000ffffL));
        }  // end computHashes



        /// <summary>
        /// Given a routing table consisting of worker rank indices, choose a
        /// worker rank based on the hash of the record key.
        /// </summary>
        /// <param name="routingTable">A list of integers which represent worker ranks.</param>
        /// <returns>The appropriate entry from<paramref name="routingTable"/>.</returns>
        public int route(IList<int> routingTable)
        {
            // Return 1 less than the value of the nth element of routingTable where
            //    n == (record key hash) % (number of elements in routingTable)
            // (because the 1st worker rank is the 0th element in the worker list)
            return (routingTable[Math.Abs((int)(this.routingHash % routingTable.Count))] - 1);
        }  // end route

    }  // end class RecordKey

}   // end namespace kinetica.Utils
