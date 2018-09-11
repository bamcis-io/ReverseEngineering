using System;
using System.Collections.Generic;
using System.Text;

namespace BAMCIS.Disassembler
{
    public class OpCode
    {
        #region Private Fields

        private static readonly IEnumerable<OpCode> InstructionList;

        #endregion

        #region Public Properties

        public byte[] Code { get; }

        public string Name { get; }

        public string Description { get; }

        public OperandEncoding OpEn { get; }

        public bool RequiresModRM { get; }

        public bool RequiresSIB { get; }

        public bool HasDisplacement { get; }

        public bool HasImmediate { get; }

        #endregion

        #region Constructors

        static OpCode()
        {
            InstructionList = new List<OpCode>()
            {
                #region ADD
                                            
                new OpCode(new byte[] { 0x00 }, "ADD", OperandEncoding.MR, "Add r8 to r/m8.", true, false, false, false),
                new OpCode(new byte[] { 0x01 }, "ADD", OperandEncoding.MR, "Add r32 to r/m32.", true, false, false, false),
                new OpCode(new byte[] { 0x02 }, "ADD", OperandEncoding.MR, "Add r/m8 to r8.", true, false, false, false),
                new OpCode(new byte[] { 0x03 }, "ADD", OperandEncoding.RM, "Add r/m32 to r32.", true, false, false, false),
                new OpCode(new byte[] { 0x04 }, "ADD", OperandEncoding.I, "Add imm8 to AL.", true, false, false, true),
                new OpCode(new byte[] { 0x05 }, "ADD", OperandEncoding.I, "Add imm32 to EAX.", true, false, false, true),
                new OpCode(new byte[] { 0x80 }, "ADD", OperandEncoding.MI, "Add imm8 to r/m8.", true, false, false, true),
                new OpCode(new byte[] { 0x81 }, "ADD", OperandEncoding.MI, "Add imm32 to r/m32.", true, false, false, true),
                new OpCode(new byte[] { 0x83 }, "ADD", OperandEncoding.MI, "Add sign-extended imm8 to r/m32.", true, false, false, true),   
                #endregion

                #region SUB

                new OpCode(new byte[] { 0x2C }, "SUB", OperandEncoding.I, "Subtract imm8 from AL.", true, false, false, true),
                new OpCode(new byte[] { 0x2D }, "SUB", OperandEncoding.I, "Subtract imm32 from EAX.", true, false, false, true),

                #endregion
            };
        }

        private OpCode(byte[] code, string name, OperandEncoding opEn, string description, bool requiresModRM, bool requiresSIB, bool hasDisplacement, bool hasImmediate)
        {
            this.Code = code;
            this.Name = name;
            this.OpEn = opEn;
            this.Description = description;
            this.RequiresModRM = requiresModRM;
            this.RequiresSIB = requiresSIB;
            this.HasDisplacement = hasDisplacement;
            this.HasImmediate = hasImmediate
        }

        #endregion

        #region Public Methods

        public static OpCode Parse(byte[] data)
        {
            return new OpCode(data);
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}
