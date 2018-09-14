namespace BAMCIS.Disassembler
{
    public enum Scale : byte
    {
        /// <summary>
        /// The index register is multiplied by 1. The base register followed by a displacement. 
        /// </summary>
        TIMES_ONE       = 0x00,

        /// <summary>
        /// The index register is multiplied by 2. The base register followed by a displacement. 
        /// </summary>
        TIMES_TWO       = 0x01,

        /// <summary>
        /// The index register is multiplied by 4. The base register followed by a displacement. 
        /// </summary>
        TIMES_FOUR      = 0x02,

        /// <summary>
        /// The index register is multiplied by 8. The base register followed by a displacement. 
        /// </summary>
        TIMES_EIGHT     = 0x03
    }
}
