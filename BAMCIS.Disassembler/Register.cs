using System;
using System.Collections.Generic;

namespace BAMCIS.Disassembler
{
    public class Register
    {
        #region Public Properties

        public byte Code { get; }

        public string Name { get; }

        public bool MustSaveByConvention { get; }

        #endregion

        #region Public Static Properties

        public static readonly Register EAX = new Register(0x00, "EAX", false);
        public static readonly Register ECX = new Register(0x01, "ECX", false);
        public static readonly Register EDX = new Register(0x02, "EDX", false);
        public static readonly Register EBX = new Register(0x03, "EBX", true);
        public static readonly Register ESP = new Register(0x04, "ESP", true);
        public static readonly Register EBP = new Register(0x05, "EBP", true);
        public static readonly Register ESI = new Register(0x06, "ESI", true);
        public static readonly Register EDI = new Register(0x07, "EDI", true);

        private static readonly Dictionary<byte, Register> LookupTable;

        #endregion

        #region Constructors

        static Register()
        {
            LookupTable = new Dictionary<byte, Register>()
            {
                { Register.EAX.Code, Register.EAX },
                { Register.ECX.Code, Register.ECX },
                { Register.EDX.Code, Register.EDX },
                { Register.EBX.Code, Register.EBX },
                { Register.ESP.Code, Register.ESP },
                { Register.EBP.Code, Register.EBP },
                { Register.ESI.Code, Register.ESI },
                { Register.EDI.Code, Register.EDI },
            };
        }

        private Register(byte code, string name, bool mustSaveByConvention)
        {
            this.Code = code;
            this.Name = name;
            this.MustSaveByConvention = mustSaveByConvention;
        }

        #endregion

        #region Public Methods

        public static bool IsValidRegister(byte code)
        {
            return LookupTable.ContainsKey(code);
        }

        public static Register GetRegister(byte code)
        {
            if (IsValidRegister(code))
            {
                return LookupTable[code];
            }
            else
            {
                throw new ArgumentException($"The code {Convert.ToString(code, 2)} is not a valid register.");
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            Register Other = (Register)obj;

            return this.Code.Equals(Other.Code) &&
                this.Name.Equals(Other.Name) &&
                this.MustSaveByConvention == Other.MustSaveByConvention;
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(this.Code, this.Name, this.MustSaveByConvention);
        }

        public static bool operator ==(Register left, Register right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Register left, Register right)
        {
            return !(left == right);
        }

        #endregion
    }
}
