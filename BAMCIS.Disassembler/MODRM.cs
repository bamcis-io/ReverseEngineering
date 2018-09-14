using System;
using System.Collections.Generic;
using System.Text;

namespace BAMCIS.Disassembler
{
    public class ModRM
    {
        #region Public Properties

        public MOD MOD { get; }

        public byte REG { get; }

        public byte RM { get; }

        #endregion

        #region Constructors

        public ModRM(byte modrm)
        {
            byte mod = (byte)(modrm >> 6);

            if (Enum.IsDefined(typeof(MOD), mod))
            {
                this.MOD = (MOD)mod;
            }
            else
            {
                throw new ArgumentException($"The bits for MOD {Convert.ToString(mod, 2)} are not valid.");
            }

            this.REG = (byte)((modrm >> 3) & 0x07); // Move middle 3 to LSB, then drop top five
            this.RM = (byte)(modrm & 0x07); // Drop top five bits
        }

        #endregion

        #region Public Methods

        public bool SIBByteFollows()
        {
            return (this.MOD != MOD.RM_DIRECT) && this.RM == 0x04;
        }

        public bool InstructionHasDisplacement()
        {
            return this.MOD == MOD.RM_BYTE || this.MOD == MOD.RM_DWORD || (this.MOD == MOD.RM && this.RM == 0x05);
        }

        #endregion
    }
}
