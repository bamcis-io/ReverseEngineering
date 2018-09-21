namespace BAMCIS.Disassembler.Core
{
    public enum OperandEncoding
    {
        /// <summary>
        /// REG/MEM
        /// </summary>
        M,

        /// <summary>
        /// REG/MEM, IMMEDIATE
        /// </summary>
        MI,

        /// <summary>
        /// REG/MEM, REG
        /// </summary>
        MR,

        /// <summary>
        /// REG, REG/MEM
        /// </summary>
        RM,

        /// <summary>
        /// REG, REG/MEM, IMMEDIATE
        /// </summary>
        RMI,

        /// <summary>
        /// REG/MEM, 1
        /// </summary>
        M1,

        /// <summary>
        /// REG/MEM, CL
        /// </summary>
        MC,

        /// <summary>
        /// IMMEDIATE
        /// </summary>
        I,

        /// <summary>
        /// OFFSET, relative displacement
        /// </summary>
        D,

        /// <summary>
        /// AL/AX/EAX/RAX, Moffs
        /// </summary>
        FD,

        /// <summary>
        /// Moffs, AL/AX/EAX/RAX
        /// </summary>
        TD,

        /// <summary>
        /// No Operands
        /// </summary>
        ZO,

        /// <summary>
        /// No Operands
        /// </summary>
        NP,

        /// <summary>
        /// Opcode + REG 
        /// </summary>
        O,

        /// <summary>
        /// Opcode + REG + IMMEDIATE
        /// </summary>
        OI
    }
}
