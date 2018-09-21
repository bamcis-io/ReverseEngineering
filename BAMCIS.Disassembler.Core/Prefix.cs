using System.Collections.Generic;

namespace BAMCIS.Disassembler.Core
{
    public class Prefix
    {
        #region Public Properties

        public string Name { get; }

        public byte Code { get; }

        #endregion

        #region Public Static Properties

        public static readonly Prefix NONE = new Prefix("NONE", 0x00);

        // Group 1

        public static readonly Prefix LOCK = new Prefix("LOCK", 0xF0);

        public static readonly Prefix REPNE = new Prefix("REPNE", 0xF2);
        public static readonly Prefix REPNZ = new Prefix("REPNZ", 0xF2);

        public static readonly Prefix REP = new Prefix("REP", 0xF3);
        public static readonly Prefix REPE = new Prefix("REPE", 0xF3);
        public static readonly Prefix REPZ = new Prefix("REPZ", 0xF3);

        public static readonly Prefix BND = new Prefix("BND", 0xF2);

        // Group 2

        public static readonly Prefix CS = new Prefix("CC", 0x2E);
        public static readonly Prefix SS = new Prefix("SS", 0x36);
        public static readonly Prefix DS = new Prefix("DS", 0x3E);
        public static readonly Prefix ES = new Prefix("ES", 0x26);
        public static readonly Prefix FS = new Prefix("FS", 0x64);
        public static readonly Prefix GS = new Prefix("GS", 0x65);

        public static readonly Prefix BRANCH_NOT_TAKEN = new Prefix("BRANCH_NOT_TAKEN", 0x2E);
        public static readonly Prefix BRANCH_TAKEN = new Prefix("BRANCH_TAKEN", 0x3E);

        // Group 3

        public static readonly Prefix OPERAND_SIZE_OVERRIDE = new Prefix("OPERAND_SIZE_OVERRIDE", 0x66);

        // Group 4

        public static readonly Prefix ADDRESS_SIZE_OVERRIDE = new Prefix("ADDRESS_SIZE_OVERRIDE", 0x67);

        // Lookups

        private static Dictionary<byte, IEnumerable<Prefix>> LookupTable;

        #endregion

        #region Constructors

        static Prefix()
        {
            LookupTable = new Dictionary<byte, IEnumerable<Prefix>>()
            {
                { 0x00, new Prefix[] { NONE } },
                { 0xF0, new Prefix[] { LOCK } },
                { 0xF2, new Prefix[] { REPNE, REPNZ, BND } },
                { 0xF3, new Prefix[] { REP, REPE, REPZ } },
                { 0x2E, new Prefix[] { CS, BRANCH_NOT_TAKEN } },
                { 0x36, new Prefix[] { SS } },
                { 0x3E, new Prefix[] { DS, BRANCH_TAKEN } },
                { 0x26, new Prefix[] { ES } },
                { 0x64, new Prefix[] { FS } },
                { 0x65, new Prefix[] { GS } },
                { 0x66, new Prefix[] { OPERAND_SIZE_OVERRIDE } },
                { 0x67, new Prefix[] { ADDRESS_SIZE_OVERRIDE } }
            };
        }

        private Prefix(string prefix, byte code)
        {
            this.Name = prefix;
            this.Code = code;
        }

        #endregion

        #region Public Methods

        public static IEnumerable<Prefix> Parse(byte code)
        {
            if (LookupTable.ContainsKey(code))
            {
                return LookupTable[code];
            }
            else
            {
                return new Prefix[] { NONE };
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

            Prefix Other = (Prefix)obj;

            return this.Name.Equals(Other.Name) &&
                this.Code.Equals(Other.Code);
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(this.Code, this.Name);
        }

        public static bool operator ==(Prefix left, Prefix right)
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

        public static bool operator !=(Prefix left, Prefix right)
        {
            return !(left == right);
        }

        #endregion
    }
}
