using System.Collections.Generic;


namespace kinetica.Utils
{

    /// <summary>
    /// Builds or creates <see cref="RecordKey"/> objects based on a given record.
    /// </summary>
    /// <typeparam name="T">The type of record to build keys off of.</typeparam>
    internal sealed class RecordKeyBuilder<T>
    {
        /// <summary>
        /// Enumerate all the data types supported by Kinetica.
        /// </summary>
        private enum ColumnType
        {
            CHAR1,
            CHAR2,
            CHAR4,
            CHAR8,
            CHAR16,
            CHAR32,
            CHAR64,
            CHAR128,
            CHAR256,
            DATE,
            DATETIME,
            DECIMAL,
            DOUBLE,
            FLOAT,
            INT,
            INT8,
            INT16,
            IPV4,
            LONG,
            STRING,
            TIME,
            TIMESTAMP
        }  // end enum ColumnType


        // Class members
        private KineticaType ktype;
        private IList<int> routing_column_indices;
        private IList<ColumnType> column_types;
        private int buffer_size;

        public RecordKeyBuilder(bool is_primary_key, KineticaType ktype)
        {
            this.ktype = ktype;

            this.buffer_size = 0;
            routing_column_indices = new List<int>();
            column_types = new List<ColumnType>();

            // We need to check if the type has all of the following: x, y, timestamp, track ID
            // (this will tell us if it's a track type table, and if so, the track ID
            // column would be a routing column)
            bool has_timestamp = false;
            bool has_x = false;
            bool has_y = false;
            int track_id_column_idx = -1;  // not found yet

            // Add indices of any primary or shard key (based on is_primary_key)
            // to the list of routing columns
            IList<KineticaType.Column> columns = ktype.getColumns();
            for (int i = 0; i < columns.Count; ++i)
            {
                // Get the column
                KineticaType.Column column = columns[i];

                // Check if it is one of: x, y, timestamp, track ID
                switch (column.getName())
                {
                    case "TRACKID":
                        track_id_column_idx = i;
                        break;

                    case "TIMESTAMP":
                        has_timestamp = true;
                        break;

                    case "x":
                        has_x = true;
                        break;

                    case "y":
                        has_y = true;
                        break;
                }  // end switch on column name

                // Check if this column has been declared as a primary/shard key
                // And if so, and if appropriate, add it to the routing key column list
                if (is_primary_key && column.getProperties().Contains(ColumnProperty.PRIMARY_KEY))
                {
                    routing_column_indices.Add(i);
                }
                else if (!is_primary_key && column.getProperties().Contains(ColumnProperty.SHARD_KEY))
                {
                    routing_column_indices.Add(i);
                }
            }  // end for loop

            // Check if this is a track-type table; if so, add the track ID column's index to the list
            if (!is_primary_key
                && has_timestamp && has_x && has_y && (track_id_column_idx != -1))
            {
                if (routing_column_indices.Count == 0)
                {
                    routing_column_indices.Add(track_id_column_idx);
                }
                else if ((routing_column_indices.Count != 1)
                          || (routing_column_indices[0] != track_id_column_idx))
                {
                    // Track type tables can't have any other routing key
                    throw new KineticaException("Cannot have a shard key other than 'TRACKID' for track tables.");
                }
            }  // end if a track type table


            // For each index of routing columns, save the column type, and increase
            // the buffer size appropriately
            foreach (int i in routing_column_indices)
            {
                // Get the column information
                KineticaType.Column column = columns[i];

                switch (column.getType())
                {
                    // Float and double are the simplest
                    case KineticaType.Column.ColumnType.FLOAT:
                        {
                            column_types.Add(ColumnType.FLOAT);
                            this.buffer_size += 4;
                            break;
                        }
                    case KineticaType.Column.ColumnType.DOUBLE:
                        {
                            column_types.Add(ColumnType.DOUBLE);
                            this.buffer_size += 8;
                            break;
                        }

                    case KineticaType.Column.ColumnType.INT:
                        {
                            // Integer has byte, short and int
                            if (column.getProperties().Contains(ColumnProperty.INT8))
                            {   // byte
                                column_types.Add(ColumnType.INT8);
                                this.buffer_size += 1;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.INT16))
                            {   // short
                                column_types.Add(ColumnType.INT16);
                                this.buffer_size += 2;
                            }
                            else // regular 4-byte integer
                            {
                                column_types.Add(ColumnType.INT);
                                this.buffer_size += 4;
                            }
                            break;
                        }  // end case integer

                    case KineticaType.Column.ColumnType.LONG:
                        {
                            // Long has the regular long and timestamp
                            if (column.getProperties().Contains(ColumnProperty.TIMESTAMP))
                            {   // it's a timestamp
                                column_types.Add(ColumnType.TIMESTAMP);
                            }
                            else // regular long
                            {
                                column_types.Add(ColumnType.LONG);
                            }
                            this.buffer_size += 8;
                            break;
                        }  // end case long

                    case KineticaType.Column.ColumnType.STRING:
                        {
                            if (column.getProperties().Contains(ColumnProperty.CHAR1))
                            {
                                column_types.Add(ColumnType.CHAR1);
                                this.buffer_size += 1;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR2))
                            {
                                column_types.Add(ColumnType.CHAR2);
                                this.buffer_size += 2;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR4))
                            {
                                column_types.Add(ColumnType.CHAR4);
                                this.buffer_size += 4;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR8))
                            {
                                column_types.Add(ColumnType.CHAR8);
                                this.buffer_size += 8;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR16))
                            {
                                column_types.Add(ColumnType.CHAR16);
                                this.buffer_size += 16;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR32))
                            {
                                column_types.Add(ColumnType.CHAR32);
                                this.buffer_size += 32;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR64))
                            {
                                column_types.Add(ColumnType.CHAR64);
                                this.buffer_size += 64;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR128))
                            {
                                column_types.Add(ColumnType.CHAR128);
                                this.buffer_size += 128;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.CHAR256))
                            {
                                column_types.Add(ColumnType.CHAR256);
                                this.buffer_size += 256;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.DATE))
                            {
                                column_types.Add(ColumnType.DATE);
                                this.buffer_size += 4;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.DATETIME))
                            {
                                column_types.Add(ColumnType.DATETIME);
                                this.buffer_size += 8;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.DECIMAL))
                            {
                                column_types.Add(ColumnType.DECIMAL);
                                this.buffer_size += 8;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.IPV4))
                            {
                                column_types.Add(ColumnType.IPV4);
                                this.buffer_size += 4;
                            }
                            else if (column.getProperties().Contains(ColumnProperty.TIME))
                            {
                                column_types.Add(ColumnType.TIME);
                                this.buffer_size += 4;
                            }
                            else // regular string
                            {
                                column_types.Add(ColumnType.STRING);
                                this.buffer_size += 8;
                            }
                            break;
                        }  // end case string

                    // Other types are not allowed for routing columns
                    case KineticaType.Column.ColumnType.BYTES:
                    case KineticaType.Column.ColumnType.DEFAULT:
                        throw new KineticaException($"Cannot use column '{column.getName()}' as a key.");
                }  // end switch on the column's primitive data type
            }  // end foreach
        }  // end constructor RecordKeyBuilder


        /// <summary>
        /// Build a RecordKey object based on a record.
        /// </summary>
        /// <param name="record">The object based on which the key is to
        /// be built.</param>
        /// <returns>The record key that helps is routing this record
        /// correctly.</returns>
        public RecordKey build(T record)
        {
            // Can't build a key if the buffer size is zero!
            if (this.buffer_size == 0)
                return null;

            // Create the empty key
            RecordKey key = new RecordKey(this.buffer_size);

            // Add each routing column's value to the key
            for (int i = 0; i < this.routing_column_indices.Count; ++i)
            {
                // Get the column (with type and name)
                KineticaType.Column column = this.ktype.getColumns()[this.routing_column_indices[i]];

                // Get the value out of the record using the column's name and reflection
                var value = record.GetType().GetProperty(column.getName()).GetValue(record, null);

                switch (this.column_types[i])
                {
                    case ColumnType.CHAR1:
                        key.addCharN((string)value, 1);
                        break;

                    case ColumnType.CHAR2:
                        key.addCharN((string)value, 2);
                        break;

                    case ColumnType.CHAR4:
                        key.addCharN((string)value, 4);
                        break;

                    case ColumnType.CHAR8:
                        key.addCharN((string)value, 8);
                        break;

                    case ColumnType.CHAR16:
                        key.addCharN((string)value, 16);
                        break;

                    case ColumnType.CHAR32:
                        key.addCharN((string)value, 32);
                        break;

                    case ColumnType.CHAR64:
                        key.addCharN((string)value, 64);
                        break;

                    case ColumnType.CHAR128:
                        key.addCharN((string)value, 128);
                        break;

                    case ColumnType.CHAR256:
                        key.addCharN((string)value, 256);
                        break;

                    case ColumnType.DATE:
                        key.addDate((string)value);
                        break;

                    case ColumnType.DATETIME:
                        key.addDateTime((string)value);
                        break;

                    case ColumnType.DECIMAL:
                        key.addDecimal((string)value);
                        break;

                    case ColumnType.DOUBLE:
                        key.addDouble((double?)value);
                        break;

                    case ColumnType.FLOAT:
                        key.addFloat((float?)value);
                        break;

                    case ColumnType.INT:
                        key.addInt((int?)value);
                        break;

                    case ColumnType.INT8:
                        key.addInt8((int?)value);
                        break;

                    case ColumnType.INT16:
                        key.addInt16((int?)value);
                        break;

                    case ColumnType.IPV4:
                        key.addIPv4((string)value);
                        break;

                    case ColumnType.LONG:
                        key.addLong((long?)value);
                        break;

                    case ColumnType.STRING:
                        key.addString((string)value);
                        break;

                    case ColumnType.TIME:
                        key.addTime((string)value);
                        break;

                    case ColumnType.TIMESTAMP:
                        key.addTimeStamp((long?)value);
                        break;
                }  // end switch
            }  // end for loop

            // Compute the hash for the key and return it
            key.computHashes();
            return key;
        }  // end build()



        /// <summary>
        /// Build an expression to be passed to getRecords in the option.  The
        /// expression is based on the record provided, but only values of shard
        /// columns will be used, the rest will be ignored.
        /// </summary>
        /// <param name="record">The object based on which the expression is to
        /// be built.</param>
        /// <returns>The record key that helps is routing this record
        /// correctly.</returns>
        public string buildExpression(T record)
        {
            // Can't build a key if the buffer size is zero!
            if (this.buffer_size == 0)
                return null;

            // Create the empty expression
            System.Text.StringBuilder expression = new System.Text.StringBuilder( "(" );

            // Add each routing column's value to the key
            for (int i = 0; i < this.routing_column_indices.Count; ++i)
            {
                if ( i > 0 )  // need a conjunction
                    expression.Append( " and " );

                // Get the column (with type and name)
                KineticaType.Column column = this.ktype.getColumns()[this.routing_column_indices[i]];
                string column_name = column.getName();

                // Get the value out of the record using the column's name and reflection
                var value = record.GetType().GetProperty( column_name ).GetValue( record, null );

                // Handle null values
                if ( value == null )
                {
                    expression.Append( "is_null(" );
                    expression.Append( column_name );
                    expression.Append( ")" );
                    continue; // nothing more to do for this column
                }

                // Add this column to the expression
                expression.Append( "(" );
                expression.Append( column_name );
                expression.Append( " = " );

                // Add the value to the expression
                switch ( this.column_types[i] )
                {
                    // Need to quote string values
                    case ColumnType.CHAR1:
                    case ColumnType.CHAR2:
                    case ColumnType.CHAR4:
                    case ColumnType.CHAR8:
                    case ColumnType.CHAR16:
                    case ColumnType.CHAR32:
                    case ColumnType.CHAR64:
                    case ColumnType.CHAR128:
                    case ColumnType.CHAR256:
                    case ColumnType.DATE:
                    case ColumnType.DATETIME:
                    case ColumnType.DECIMAL:
                    case ColumnType.IPV4:
                    case ColumnType.STRING:
                    case ColumnType.TIME:
                        expression.Append( "\"" );
                        expression.Append( value );
                        expression.Append( "\"" );
                        break;

                    case ColumnType.DOUBLE:
                    case ColumnType.FLOAT:
                    case ColumnType.INT:
                    case ColumnType.INT8:
                    case ColumnType.INT16:
                    case ColumnType.LONG:
                        expression.Append( value );
                        break;
                }  // end switch

                // Closing parenthesis for the column
                expression.Append( ")" );
            }  // end for loop

            // Final closing parenthesis
            expression.Append( ")" );

            return expression.ToString();
        }  // end buildExpression()



        /// <summary>
        /// Returns whether this builder builds any routing keys. That is,
        /// if there are any routing columns in the relevant record type.
        /// </summary>
        /// <returns>Bool value indicating if this builder builds keys.</returns>
        public bool hasKey()
        {
            // Does it have any routing columns?
            return !(this.routing_column_indices.Count == 0);
        }


        /// <summary>
        /// Returns if <paramref name="other"/> is equivalent to this builder.
        /// </summary>
        /// <param name="other">The other key builder.</param>
        /// <returns>Whether the other builder is the same as this one.</returns>
        public bool hasSameKey(RecordKeyBuilder<T> other)
        {
            return this.column_types.Equals(other.column_types);
        }

    }  // end class RecordKeyBuilder

}   // end namespace kinetica.Utils
