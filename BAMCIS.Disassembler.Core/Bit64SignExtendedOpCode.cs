namespace BAMCIS.Disassembler.Core
{
    public class Bit64SignExtendedOpCode : OpCode
    {
        #region Constructors

        internal Bit64SignExtendedOpCode(byte[] code, string name, OperandEncoding opEn, string description, int operandSize, int extension = -1) : base(code, name, opEn, description, operandSize, extension, true, 64)
        {
            
        }

        #endregion
    }
}
