namespace BAMCIS.Disassembler
{
    public static class OperandEncodingExtensionMethods
    {
        public static bool RequiresModRM(this OperandEncoding encoding)
        {
            return (encoding == OperandEncoding.M ||
                encoding == OperandEncoding.MI ||
                encoding == OperandEncoding.MR ||
                encoding == OperandEncoding.RM ||
                encoding == OperandEncoding.RMI ||
                encoding == OperandEncoding.M1 ||
                encoding == OperandEncoding.MC);
        }

        public static bool UsesRegisterValueWithOpCode(this OperandEncoding encoding)
        {
            return encoding == OperandEncoding.O ||
                encoding == OperandEncoding.OI;
        }

        public static bool InstructionHasImmediate(this OperandEncoding encoding)
        {
            return encoding == OperandEncoding.I ||
                encoding == OperandEncoding.MI ||
                encoding == OperandEncoding.OI ||
                encoding == OperandEncoding.RMI ||
                encoding == OperandEncoding.D;
        }
    }
}
