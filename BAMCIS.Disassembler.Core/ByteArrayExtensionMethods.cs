using System;

namespace BAMCIS.Disassembler.Core
{
    public static class ByteArrayExtensionMethods
    {
        /// <summary>
        /// Signs extends the provided byte array to the given length
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <param name="isLittleEndian">Specifies that the input array is little endian if true</param>
        /// <returns>A big endian byte array</returns>
        public static byte[] SignExtend(this byte[] bytes, bool isLittleEndian = false, int length = 4)
        {
            if (length < 1 || length < bytes.Length)
            {
                throw new ArgumentOutOfRangeException("length", "The hex length must be greater than 0 and at least the length of the input bytes.");
            }

            byte[] Temp = new byte[bytes.Length];
            bytes.CopyTo(Temp, 0);

            if (isLittleEndian)
            {
                Array.Reverse(Temp);
            }

            if (length > bytes.Length)
            {
                byte[] Result = new byte[length];

                Array.Copy(Temp, 0, Result, Result.Length - Temp.Length, Temp.Length);

                byte Padding = 0x00;

                if ((Temp[0] & 0x80) == 0x80) // Value is negative
                {
                    Padding = 0xFF;
                }

                for (int i = 0; i < Result.Length - bytes.Length; i++)
                {
                    Result[i] = Padding;
                }

                return Result;
            }
            else
            {              
                return Temp;
            }
        }

        /// <summary>
        /// Provides a literal interpretation of the byte array into hex. This can be padded with leading
        /// 0x00 bytes by supplying a length that is longer than the current input
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isLittleEndian">Indicates that the source data is in little endian form</param>
        /// <param name="length">The length in bytes of the resulting string representation. Specifying
        /// an amount over the input will cause the string to be padded with '00' bytes.</param>
        /// <returns>The string representation of the byte array in big endian form</returns>
        public static string ToHexString(this byte[] bytes, bool isLittleEndian = false, int length = -1)
        {
            byte[] Temp = new byte[bytes.Length];
            bytes.CopyTo(Temp, 0);

            if (isLittleEndian)
            {
                Array.Reverse(Temp);
            }

            string Bytes = BitConverter.ToString(Temp).Replace("-", "");

            if (length > Temp.Length)
            {
                // Times 2 since there are 2 chars for each byte
                Bytes = Bytes.PadLeft(length * 2, '0');
            }
            else if (length > 0)
            {
                Bytes = Bytes.Tail(length * 2);
            }

            return $"0x{Bytes}";
        }

        /// <summary>
        /// Converts the byte array to a sign extended string representation
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <param name="isLittleEndian">Indicates that the source data is in little endian form</param>
        /// <returns></returns>
        public static string ToHexStringSignExtended(this byte[] bytes, bool isLittleEndian = false, int length = 4)
        {
            byte[] Temp = bytes.SignExtend(isLittleEndian, length);

            return Temp.ToHexString();
        }
    }
}
