/*
This code is public domain.

The MurmurHash3 algorithm was created by Austin Appleby and put into the public domain.  See http://code.google.com/p/smhasher/
*/

using System;


namespace kinetica
{

    /// <summary>
    /// Usage:
    /// <code>
    ///    MurMurHash3.LongPair murmur = new MurMurHash3.LongPair();
    ///    System.Text.Encoding encoding = new System.Text.UTF8Encoding();
    ///    byte[] input = encoding.GetBytes( value );
    ///    MurMurHash3.murmurhash3_x64_128( input, 0, (uint)input.Length, 10, out murmur );
    ///    // Use the values as murmur.val1 and murmur.val2
    /// </code>
    /// </summary>
    public static class MurMurHash3
    {
        /** 128 bits of state */
        public class LongPair
        {
            public long val1 { get; set; }
            public long val2 { get; set; }
        }


        private static ulong fmix64( UInt64 k )
        {
            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;
            return k;
        }  // end fmix64


        /// <summary>
        /// Rotate the given number by the given number of bits.
        /// </summary>
        /// <param name="original">Rotate this number.</param>
        /// <param name="bits">Rotate by this many bits.</param>
        /// <returns>The rotated number.</returns>
        private static ulong RotateLeft( this ulong original, int bits )
        {
            return ( original << bits ) | ( original >> ( 64 - bits ) );
        }




        /// Returns the MurmurHash3_x64_128 hash, placing the result in <paramref name="output" />
        public static void murmurhash3_x64_128( byte[] key, uint offset, uint len, int seed, out LongPair output )
        {
            // The original algorithm does have a 32 bit unsigned seed.
            // We have to mask to match the behavior of the unsigned types and prevent sign extension.
            ulong h1 = (ulong) (seed & 0x00000000FFFFFFFFL);
            ulong h2 = (ulong) (seed & 0x00000000FFFFFFFFL);

            const ulong c1 = 0x87c37b91114253d5L;
            const ulong c2 = 0x4cf5ad432745937fL;

            uint roundedEnd = offset + (len & 0xFFFFFFF0);  // round down to 16 byte block

            // Process the body in 16 byte blocks
            for ( int i = (int)offset; i < roundedEnd; i += 16 )
            {
                ulong key1 = BitConverter.ToUInt64( key, i );
                ulong key2 = BitConverter.ToUInt64( key, (i + 8) );

                key1 *= c1; key1 = key1.RotateLeft( 31 ); key1 *= c2; h1 ^= key1;
                h1 = h1.RotateLeft( 27 ); h1 += h2; h1 = h1 * 5 + 0x52dce729;
                key2 *= c2; key2 = key2.RotateLeft( 33 ); key2 *= c1; h2 ^= key2;
                h2 = h2.RotateLeft( 31 ); h2 += h1; h2 = h2 * 5 + 0x38495ab5;
            }

            // Process the remaining bytes, if any
            ulong k1 = 0;
            ulong k2 = 0;

            switch ( len & 15 )
            {
                case 15:
                    k2 = ( ulong ) ( key[roundedEnd + 14] & 0xffL ) << 48;
                    goto case 14; // fall through
                case 14:
                    k2 |= ( ulong ) ( key[roundedEnd + 13] & 0xffL ) << 40;
                    goto case 13; // fall through
                case 13:
                    k2 |= ( ulong ) ( key[roundedEnd + 12] & 0xffL ) << 32;
                    goto case 12; // fall through
                case 12:
                    k2 |= ( ulong ) ( key[roundedEnd + 11] & 0xffL ) << 24;
                    goto case 11; // fall through
                case 11:
                    k2 |= ( ulong ) ( key[roundedEnd + 10] & 0xffL ) << 16;
                    goto case 10; // fall through
                case 10:
                    k2 |= ( ulong ) ( key[roundedEnd + 9] & 0xffL ) << 8;
                    goto case 9; // fall through
                case 9:
                    k2 |= ( ulong ) ( key[roundedEnd + 8] & 0xffL );
                    k2 *= c2; k2 = k2.RotateLeft( 33 ); k2 *= c1; h2 ^= k2;
                    //break;
                    goto case 8; // fall through

                case 8:
                    k1 = ( ( ulong ) key[roundedEnd + 7] ) << 56;
                    goto case 7; // fall through
                case 7:
                    k1 |= ( ulong ) ( key[roundedEnd + 6] & 0xffL ) << 48;
                    goto case 6; // fall through
                case 6:
                    k1 |= ( ulong ) ( key[roundedEnd + 5] & 0xffL ) << 40;
                    goto case 5; // fall through
                case 5:
                    k1 |= ( ulong ) ( key[roundedEnd + 4] & 0xffL ) << 32;
                    goto case 4; // fall through
                case 4:
                    k1 |= ( ulong ) ( key[roundedEnd + 3] & 0xffL ) << 24;
                    goto case 3; // fall through
                case 3:
                    k1 |= ( ulong ) ( key[roundedEnd + 2] & 0xffL ) << 16;
                    goto case 2; // fall through
                case 2:
                    k1 |= ( ulong ) ( key[roundedEnd + 1] & 0xffL ) << 8;
                    goto case 1; // fall through
                case 1:
                    k1 |= ( ulong ) ( key[roundedEnd] & 0xffL );
                    k1 *= c1; k1 = k1.RotateLeft( 31 ); k1 *= c2; h1 ^= k1;
                    break;
            }

            //----------
            // Finalize the values

            h1 ^= len;
            h2 ^= len;

            h1 += h2;
            h2 += h1;

            h1 = fmix64( h1 );
            h2 = fmix64( h2 );

            h1 += h2;
            h2 += h1;

            // Save the values in the output
            output = new LongPair();
            output.val1 = (long) h1;
            output.val2 = (long) h2;
        }  // end murmurhash3_x64_128

    }  // end class MurMurHash3

}  // end namespace kinetica
