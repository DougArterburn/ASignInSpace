using System.Collections;
using System.Text;

namespace ASignInSpace
{
    internal static class BitArrayHelper
    {


        public const char One = '1';
        public const char Zero = '0';

        static public BitArray CreateBitArray(string s, bool reverse = false)
        {
            BitArray ba = new BitArray(s.Length);
            ba.SetAll(false);

            if (reverse)
            {
                for (int i = s.Length-1, j =0; i >= 0; i--, j++)
                {
                    if (s[j] == '0') ba[i] = false;
                    else ba[i] = true;
                }
            }
            else
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '0') ba[i] = false;
                    else ba[i] = true;
                }
            }
            return ba;
        }

        static public bool Compare(BitArray a1, BitArray a2)
        {
            if (a2 == null) return false;

            string s1 = ConvertToString(a1);
            string s2 = ConvertToString(a2);

            if (a1.Length < a2.Length)
            {
                return s2.Contains(s1);
            }
            else if (a1.Length > a2.Length)
            {
                return s1.Contains(s2);
            }

            else
            {
                return s1 == s2;
            }
        }

        static public string ConvertToString(BitArray a1)
        {
            StringBuilder s = new StringBuilder(a1.Length);
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i]) s.Append('1');
                else s.Append("0");
            }
            return s.ToString();
        }

        static public BitArray CreateBitArray(byte[] data)
        {
            BitArray ba = new BitArray(data.Length * 8);    // definitly MSB-first

            for (int iByte = 0; iByte < data.Length; iByte++)
            {
                byte aByte = data[iByte];
                for (int iBit = 0; iBit < 8; iBit++)
                {
                    bool aBit = (((aByte << iBit) & 0x80) == 0x80);   // MSB-first
                    ba[iByte * 8 + iBit] = aBit;
                }
            }
            return ba;
        }

        static public byte[] CreateByteArray(BitArray ba)
        {
            byte[] data = new byte[ba.Length / 8];
            int iByte = 0, iBit = 0;
            while (iByte < data.Length)
            {
                byte aByte = 0;
                for (int j = 0; j < 8; j++)
                {
                    aByte <<= 1;
                    if (ba[iBit])
                    {
                        aByte += 1;
                    }
                    iBit++;
                }
                data[iByte] = aByte;
                iByte++;
            }
            return data;
        }

        static public byte[] CreateByteArray(string s)
        {
            byte[] data = new byte[s.Length / 8];
            int iByte = 0, iBit = 0;
            while (iByte < data.Length)
            {
                byte aByte = 0;
                for (int j = 0; j < 8; j++)
                {
                    aByte <<= 1;
                    if (s[iBit] == One)
                    {
                        aByte += 1;
                    }
                    iBit++;
                }
                data[iByte] = aByte;
                iByte++;
            }
            return data;
        }

        static public BitArray ShiftLeft(BitArray ba, int count, bool fill = false)
        {
            BitArray newArray = new BitArray(ba);
            int len = ba.Length;
            for (int i = 0; i < len - count; i++)
            {
                newArray[i] = ba[i + count];
            }

            for (int i = 0, j = len - count; i < count; i++, j++)
            {
                newArray[j] = fill;
            }

            return newArray;
        }
        static public BitArray RotateLeft(BitArray ba, int count, bool fill = false)
        {
            BitArray holdBits = new BitArray(count);
            BitArray newArray = new BitArray(ba);
            int len = ba.Length;
            for (int i = 0; i < count; i++)
            {
                holdBits[i] = ba[i];
            }

            newArray = ShiftLeft(newArray, count, fill);

            for (int i = 0, j = len - count; i < holdBits.Length; i++, j++)
            {
                newArray[j] = holdBits[i];
            }

            return newArray;
        }

        static public BitArray ShiftRight(BitArray ba, int count, bool fill = false)
        {
            BitArray newArray = new BitArray(ba);
            int len = ba.Length;
            for (int i = len - 1; i > count-1; i--)
            {
                newArray[i] = ba[i - count];
            }

            for (int i = 0; i < count; i++)
            {
                newArray[i] = fill;
            }

            return newArray;
        }
        static public BitArray RotateRight(BitArray ba, int count, bool fill = false)
        {
            BitArray holdBits = new BitArray(count);
            BitArray newArray = new BitArray(ba);
            int len = ba.Length;
            for (int i = 0, j=len-count; i < count; i++,j++)
            {
                holdBits[i] = ba[j];
            }

            newArray = ShiftRight(newArray, count, fill);

            for (int i = 0; i < holdBits.Length; i++)
            {
                newArray[i] = holdBits[i];
            }

            return newArray;
        }


    }



}
