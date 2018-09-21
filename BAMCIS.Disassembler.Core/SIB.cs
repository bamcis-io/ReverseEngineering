using System;
using System.Linq;
using System.Text;

namespace BAMCIS.Disassembler.Core
{
    /// <summary>
    /// The SIB byte is used when scaling needs to occur or often when we are adding two registers together to calculate an address.
    /// There are other circumstances when the SIB is required, for example, when accessing[ESP] directly.
    /// 
    /// SCALE (2 Bits) / INDEX (3 Bits) / BASE (3 Bits)
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

        #region Public Methods

        public string ToString(ModRM modrm, byte[] displacement)
        {
            StringBuilder Buffer = new StringBuilder("[");
            
            if (this.Index == 0x04) // The index is ESP
            {
                Buffer.Append("esp");
            }
            else
            {
                Buffer.Append(Register.GetRegister(this.Index).Name.ToLower());

                if (this.Scale != Scale.TIMES_ONE)
                {
                    Buffer.Append("*");
                    Buffer.Append(this.Scale.GetScaleFactor().ToString());
                }

                if (this.Base == Register.EBP.Code) // If the Base is EBP, there is no base in the instruction
                {
                    // The display now depends on the ModRM byte
                    switch (modrm.MOD)
                    {
                        case MOD.RM:
                            {
                                if (displacement != null && displacement.Any(x => x != 0x00))
                                {
                                    Buffer.Append("+");
                                    Buffer.Append(displacement.ToHexString(true));
                                }
                                break;
                            }
                        case MOD.RM_BYTE:
                            {
                                if (displacement != null && displacement.Any(x => x != 0x00))
                                {
                                    Buffer.Append("+");
                                    Buffer.Append(displacement.ToHexString(true, 1));
                                    Buffer.Append("+ebp");
                                }
                                break;
                            }
                        case MOD.RM_DWORD:
                            {
                                if (displacement != null && displacement.Any(x => x != 0x00))
                                {
                                    Buffer.Append("+");
                                    Buffer.Append(displacement.ToHexString(true));
                                    Buffer.Append("+ebp");
                                }
                                break;
                            }
                        default:
                            {
                                throw new ArgumentException("The ModRM MOD bits were set to direct register access, which is not compatible with a SIB base of 0x05.");
                            }

                    }
                }
                else
                {
                    Buffer.Append("+");
                    Buffer.Append(Register.GetRegister(this.Base).Name.ToLower());

                    if (displacement != null && displacement.Any(x => x != 0x00))
                    {
                        if (modrm.MOD == MOD.RM_BYTE)
                        {
                            Buffer.Append("+");
                            Buffer.Append(displacement.ToHexString(true, 1));
                        }
                        else
                        {
                            Buffer.Append("+");
                            Buffer.Append(displacement.ToHexString(true));
                        }
                    }
                }
            }

            Buffer.Append("]");

            return Buffer.ToString();
        }

        #endregion
    }
}
