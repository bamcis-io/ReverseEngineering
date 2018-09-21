namespace BAMCIS.Disassembler.Core
{
    public class Bit32SignExtendedOpCode : OpCode
    {
        #region Constructors

        internal Bit32SignExtendedOpCode(byte[] code, string name, OperandEncoding opEn, string description, int operandSize, int extension = -1) : base(code, name, opEn, description, operandSize, extension, true, 32)
        {
            
        }

        #endregion
    }
}
