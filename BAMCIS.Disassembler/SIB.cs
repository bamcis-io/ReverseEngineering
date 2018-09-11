using System;

namespace BAMCIS.Disassembler
{
    /// <summary>
    /// The SIB byte is used when scaling needs to occur or often when we are adding two registers together to calculate an address.
    /// There are other circumstances when the SIB is required, for example, when accessing[ESP] directly.
    /// </summary>
    public class SIB
    {
        #region Public Properties

        public Scale Scale { get; }

        public byte Index { get; }

        public byte Base { get; }

        #endregion

        #region Constructors

        public SIB(byte sib)
        {
            byte scale = (byte)(sib >> 6);

            if (Enum.IsDefined(typeof(Scale), scale))
            {
                this.Scale = (Scale)scale;
            }
            else
            {
                throw new ArgumentException($"The bits for Scale {Convert.ToString(scale, 2)} are not valid.");
            }

            this.Index = (byte)((sib >> 3) & 0x07); // Move middle 3 to LSB, then drop top five
            this.Base = (byte)(sib & 0x07); // Drop top five bits
        }

        #endregion
    }
}
