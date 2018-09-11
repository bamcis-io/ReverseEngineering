using System;
using System.Collections.Generic;
using System.Text;

namespace BAMCIS.Disassembler
{
    public enum OperandEncoding
    {
        /// <summary>
        /// IMMEDIATE
        /// </summary>
        I,

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
        /// OFFSET
        /// </summary>
        D,

        /// <summary>
        /// No Operands
        /// </summary>
        ZO,

        /// <summary>
        /// Opcode + REG 
        /// </summary>
        O
    }
}
