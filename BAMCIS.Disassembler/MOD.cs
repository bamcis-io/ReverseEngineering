namespace BAMCIS.Disassembler
{
    public enum MOD : byte
    {
        /// <summary>
        /// The r/m32 operand’s memory address is located in the reg register
        /// </summary>
        RM          = 0x00,

        /// <summary>
        /// The r/m32 operand’s memory address is located in the reg register + a 1-byte displacement.
        /// </summary>
        RM_BYTE     = 0x01,

        /// <summary>
        /// The r/m32 operand’s memory address is located in the reg register + a 4-byte displacement.
        /// </summary>
        RM_DWORD    = 0x02,

        /// <summary>
        /// The r/m32 operand is a direct register access.
        /// </summary>
        RM_DIRECT   = 0x03
    }
}
