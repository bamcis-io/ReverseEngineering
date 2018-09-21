using System;
using System.Collections.Generic;
using System.Linq;

namespace BAMCIS.Disassembler
{
    public class OpCode
    {
        #region Private Fields

        private static readonly IEnumerable<OpCode> InstructionList;

        private static readonly Dictionary<byte[], List<OpCode>> LookupTable;

        private static readonly Dictionary<byte[], Dictionary<int, OpCode>> ExtensionLookupTable;

        #endregion

        #region Public Properties

        public byte[] Code { get; }

        public int Extension { get; }

        public string Name { get; }

        public string Description { get; }

        public OperandEncoding OpEn { get; }

        public int OperandSize { get; }

        public bool SignExtendedImmediate { get; }

        public int SignExtensionSize { get; }

        #endregion

        #region Constructors

        static OpCode()
        {
            InstructionList = new List<OpCode>()
            {
                #region ADD
                                            
                new OpCode(new byte[] { 0x00 }, Constants.ADD, OperandEncoding.MR, "Add r8 to r/m8.", 8),
                new OpCode(new byte[] { 0x01 }, Constants.ADD, OperandEncoding.MR, "Add r32 to r/m32.", 32),
                new OpCode(new byte[] { 0x02 }, Constants.ADD, OperandEncoding.MR, "Add r/m8 to r8.", 8),
                new OpCode(new byte[] { 0x03 }, Constants.ADD, OperandEncoding.RM, "Add r/m32 to r32.", 32),
                new OpCode(new byte[] { 0x04 }, Constants.ADD, OperandEncoding.I, "Add imm8 to AL.", 8),
                new OpCode(new byte[] { 0x05 }, Constants.ADD, OperandEncoding.I, "Add imm32 to EAX.", 32),
                new OpCode(new byte[] { 0x80 }, Constants.ADD, OperandEncoding.MI, "Add imm8 to r/m8.", 8, 0),
                new OpCode(new byte[] { 0x81 }, Constants.ADD, OperandEncoding.MI, "Add imm32 to r/m32.", 32, 0),
                new OpCode(new byte[] { 0x83 }, Constants.ADD, OperandEncoding.MI, "Add sign-extended imm8 to r/m32.", 32, 0),   
                #endregion

                #region SUB

                new OpCode(new byte[] { 0x2C }, Constants.SUB, OperandEncoding.I, "Subtract imm8 from AL.", 8),
                new OpCode(new byte[] { 0x2D }, Constants.SUB, OperandEncoding.I, "Subtract imm32 from EAX.", 32),
                new OpCode(new byte[] { 0x80 }, Constants.SUB, OperandEncoding.MI, "Subtract imm8 from r/m8.", 8, 5),
                new OpCode(new byte[] { 0x81 }, Constants.SUB, OperandEncoding.MI, "SUB r/m32, imm32.", 32, 5),
                new OpCode(new byte[] { 0x83 }, Constants.SUB, OperandEncoding.MI, "Subtract sign-extended imm8 from r/m32.", 32, 5),
                new OpCode(new byte[] { 0x28 }, Constants.SUB, OperandEncoding.MR, "Subtract r8 from r/m8.", 8),
                new OpCode(new byte[] { 0x29 }, Constants.SUB, OperandEncoding.MR, "Subtract r32 from r/m32.", 32),
                new OpCode(new byte[] { 0x2A }, Constants.SUB, OperandEncoding.RM, "Subtract r/m8 from r8.", 8),
                new OpCode(new byte[] { 0x2B }, Constants.SUB, OperandEncoding.RM, "Subtract r/m32 from r32.", 32),

                #endregion

                #region AND

                new OpCode(new byte[] { 0x24 }, Constants.AND, OperandEncoding.I, "AL AND imm8.", 8),
                new OpCode(new byte[] { 0x25 }, Constants.AND, OperandEncoding.I, "EAX AND imm32.", 32),
                new OpCode(new byte[] { 0x80 }, Constants.AND, OperandEncoding.MI, "r/m8 AND imm8.", 8, 4),
                new OpCode(new byte[] { 0x81 }, Constants.AND, OperandEncoding.MI, "r/m32 AND imm32.", 32, 4),
                new OpCode(new byte[] { 0x83 }, Constants.AND, OperandEncoding.MI, "r/m32 AND imm8 (sign-extended).", 8, 4),
                new OpCode(new byte[] { 0x20 }, Constants.AND, OperandEncoding.MR, "r/m8 AND r8.", 8),
                new OpCode(new byte[] { 0x21 }, Constants.AND, OperandEncoding.MR, "r/m32 AND r32.", 32),
                new OpCode(new byte[] { 0x22 }, Constants.AND, OperandEncoding.RM, "r8 AND r/m8.", 8),
                new OpCode(new byte[] { 0x23 }, Constants.AND, OperandEncoding.RM, "r32 AND r/m32.", 32),

                #endregion

                #region CALL

                new OpCode(new byte[] { 0xE8 }, Constants.CALL, OperandEncoding.D, "Call near, relative, displacement relative to next instruction.", 32),
                new OpCode(new byte[] { 0xFF }, Constants.CALL, OperandEncoding.M, "Call near, absolute indirect, address given in r/m32.", 32, 2),
                new OpCode(new byte[] { 0x9A }, Constants.CALL, OperandEncoding.D, "Call far, absolute, address given in operand.", 32),
                new OpCode(new byte[] { 0xFF }, Constants.CALL, OperandEncoding.M, "Call far, absolute indirect address given in m16:16.", 16, 3),

                #endregion

                #region CLFLUSH

                new OpCode(new byte[] { 0x0F, 0xAE }, Constants.CLFLUSH, OperandEncoding.M, "Flushes cache line containing m8.", 8, 7),

                #endregion

                #region CMP

                new OpCode(new byte[] { 0x3C }, Constants.CMP, OperandEncoding.I, "Compare imm8 with AL.", 8),
                new OpCode(new byte[] { 0x3D }, Constants.CMP, OperandEncoding.I, "Compare imm32 with EAX.", 32),
                new OpCode(new byte[] { 0x80 }, Constants.CMP, OperandEncoding.MI, "Compare imm8 with r/m8.", 8, 7),
                new OpCode(new byte[] { 0x81 }, Constants.CMP, OperandEncoding.MI, "Compare imm32 with r/m32.", 32, 7),
                new OpCode(new byte[] { 0x83 }, Constants.CMP, OperandEncoding.MI, "Compare imm8 with r/m32.", 32, 7),
                new OpCode(new byte[] { 0x38 }, Constants.CMP, OperandEncoding.MR, "Compare r8 with r/m8.", 8),
                new OpCode(new byte[] { 0x39 }, Constants.CMP, OperandEncoding.MR, "Compare r32 with r/m32.", 32),
                new OpCode(new byte[] { 0x3A }, Constants.CMP, OperandEncoding.RM, "Compare r/m8 with r8.", 8),
                new OpCode(new byte[] { 0x3B }, Constants.CMP, OperandEncoding.RM, "Compare r/m32 with r32.", 32),

                #endregion

                #region DEC

                new OpCode(new byte[] { 0xFE }, Constants.DEC, OperandEncoding.M, "Decrement r/m8 by 1.", 8, 1),
                new OpCode(new byte[] { 0xFF }, Constants.DEC, OperandEncoding.M, "Decrement r/m32 by 1.", 32, 1),
                new OpCode(new byte[] { 0x48 }, Constants.DEC, OperandEncoding.O, "Decrement r32 by 1.", 32),

                #endregion

                #region IDIV

                new OpCode(new byte[] { 0xF6 }, Constants.IDIV, OperandEncoding.M, "Signed divide AX by r/m8, with result stored in: AL ← Quotient, AH ← Remainder.", 8, 7),
                new OpCode(new byte[] { 0xF7 }, Constants.IDIV, OperandEncoding.M, "Signed divide EDX:EAX by r/m32, with result stored in EAX ← Quotient, EDX ← Remainder.", 32, 7),

                #endregion

                #region IMUL

                new OpCode(new byte[] { 0xF6 }, Constants.IMUL, OperandEncoding.M, "AX← AL ∗ r/m byte.", 8, 5),
                new OpCode(new byte[] { 0xF7 }, Constants.IMUL, OperandEncoding.M, "EDX:EAX ← EAX ∗ r/m32.", 32, 5),
                new OpCode(new byte[] { 0x0F, 0xAF }, Constants.IMUL, OperandEncoding.RM, "doubleword register ← doubleword register ∗r/m32.", 32),
                new OpCode(new byte[] { 0x6B }, Constants.IMUL, OperandEncoding.RMI, "doubleword register ← r/m32 ∗ signextended immediate byte.", 32),
                new OpCode(new byte[] { 0x69 }, Constants.IMUL, OperandEncoding.RMI, "doubleword register ← r/m32 ∗ immediate doubleword.", 32),

                #endregion

                #region INC

                new OpCode(new byte[] { 0xFE }, Constants.INC, OperandEncoding.M, "Increment r/m byte by 1.", 8, 0),
                new OpCode(new byte[] { 0xFF }, Constants.INC, OperandEncoding.M, "Increment r/m doubleword by 1.", 32, 0),
                new OpCode(new byte[] { 0x40 }, Constants.INC, OperandEncoding.O, "Increment word register by 1.", 32),

                #endregion

                #region JMP

                new Bit64SignExtendedOpCode(new byte[] { 0xEB }, Constants.JMP, OperandEncoding.D, "Jump short, RIP = RIP + 8-bit displacement sign extended to 64-bits.", 8),
                new Bit64SignExtendedOpCode(new byte[] { 0xE9 }, Constants.JMP, OperandEncoding.D, "Jump near, relative, RIP = RIP + 32-bit displacement sign extended to 64-bits.", 32),
                new OpCode(new byte[] { 0xFF }, Constants.JMP, OperandEncoding.M, "Jump near, absolute indirect, address given in r/m32. Not supported in 64-bit mode.", 32, 4),
                new OpCode(new byte[] { 0xEA }, Constants.JMP, OperandEncoding.D, "Jump far, absolute, address given in operand.", 32),
                new OpCode(new byte[] { 0xFF }, Constants.JMP, OperandEncoding.D, "Jump far, absolute indirect, address given in m16:32.", 32, 5),

                #endregion

                #region JZ / JNZ

                new OpCode(new byte[] { 0x74 }, Constants.JZ, OperandEncoding.D, "Jump short if zero (ZF = 1).", 8),
                new OpCode(new byte[] { 0x0F, 0x84 }, Constants.JZ, OperandEncoding.D, "Jump near if 0 (ZF=1).", 32),
                new OpCode(new byte[] { 0x75 }, Constants.JNZ, OperandEncoding.D, "Jump short if not zero (ZF=0).", 8),
                new OpCode(new byte[] { 0x0F, 0x85 }, Constants.JNZ, OperandEncoding.D, "Jump near if not zero(ZF = 0).", 32),

                #endregion

                #region LEA

                new OpCode(new byte[] { 0x8D }, Constants.LEA, OperandEncoding.RM, "Store effective address for m in register r32.", 32),

                #endregion

                #region MOV

                new OpCode(new byte[] { 0x88 }, Constants.MOV, OperandEncoding.MR, "Move r8 to r/m8.", 8),
                new OpCode(new byte[] { 0x89 }, Constants.MOV, OperandEncoding.MR, "Move r32 to r/m32.", 32),
                new OpCode(new byte[] { 0x8A }, Constants.MOV, OperandEncoding.RM, "Move r/m8 to r8.", 8),
                new OpCode(new byte[] { 0x8B }, Constants.MOV, OperandEncoding.RM, "Move r/m32 to r32.", 32),
                new OpCode(new byte[] { 0x8C }, Constants.MOV, OperandEncoding.MR, "Move segment register to r/m16.", 16),
                new OpCode(new byte[] { 0x8E }, Constants.MOV, OperandEncoding.RM, "Move r/m16 to segment register.", 16),
                new OpCode(new byte[] { 0xA0 }, Constants.MOV, OperandEncoding.FD, "Move byte at (seg:offset) to AL.", 8),
                new OpCode(new byte[] { 0xA1 }, Constants.MOV, OperandEncoding.FD, "Move doubleword at (seg:offset) to EAX.", 32),
                new OpCode(new byte[] { 0xA2 }, Constants.MOV, OperandEncoding.TD, "Move AL to (seg:offset).", 8),
                new OpCode(new byte[] { 0xA3 }, Constants.MOV, OperandEncoding.TD, "Move EAX to (seg:offset).", 32),
                new OpCode(new byte[] { 0xB0 }, Constants.MOV, OperandEncoding.OI, "Move imm8 to r8.", 8),
                new OpCode(new byte[] { 0xB8 }, Constants.MOV, OperandEncoding.OI, "Move imm32 to r32.", 32),
                new OpCode(new byte[] { 0xC6 }, Constants.MOV, OperandEncoding.MI, "Move imm8 to r/m8.", 8, 0),
                new OpCode(new byte[] { 0xC7 }, Constants.MOV, OperandEncoding.MI, "Move imm32 to r/m32.", 32, 0),

                #endregion

                #region MOVSD

                new OpCode(new byte[] { 0xA5 }, Constants.MOVSD, OperandEncoding.ZO, "For legacy mode, move dword from address DS:(E)SI to ES:(E)DI. For 64-bit mode move dword from address (R|E)SI to (R|E)DI.", 32),

                #endregion

                #region MUL

                new OpCode(new byte[] { 0xF6 }, Constants.MUL, OperandEncoding.M, "Unsigned multiply (AX ← AL ∗ r/m8).", 8, 4),
                new OpCode(new byte[] { 0xF7 }, Constants.MUL, OperandEncoding.M, "Unsigned multiply (EDX:EAX ← EAX ∗ r/m32).", 32, 4),

                #endregion

                #region NEG

                new OpCode(new byte[] { 0xF6 }, Constants.NEG, OperandEncoding.M, "Two's complement negate r/m8.", 8, 3),
                new OpCode(new byte[] { 0xF7 }, Constants.NEG, OperandEncoding.M, "Two's complement negate r/m32.", 32, 3),

                #endregion

                #region NOP

                new OpCode(new byte[] { 0x90 }, Constants.NOP, OperandEncoding.ZO, "One byte no-operation instruction.", 8),
                new OpCode(new byte[] { 0x0F, 0x1F }, Constants.NOP, OperandEncoding.M, "Multi-byte no-operation instruction.", 32, 0),

                #endregion

                #region NOT

                new OpCode(new byte[] { 0xF6 }, Constants.NOT, OperandEncoding.M, "Reverse each bit of r/m8.", 8, 2),
                new OpCode(new byte[] { 0xF7 }, Constants.NOT, OperandEncoding.M, "Reverse each bit of r/m32.", 32, 2),

                #endregion

                #region OR

                new OpCode(new byte[] { 0x0C }, Constants.OR, OperandEncoding.I, "AL OR imm8.", 8),
                new OpCode(new byte[] { 0x0D }, Constants.OR, OperandEncoding.I, "AL OR imm32.", 32),
                new OpCode(new byte[] { 0x80 }, Constants.OR, OperandEncoding.MI, "r/m8 OR imm8.", 8, 1),
                new OpCode(new byte[] { 0x81 }, Constants.OR, OperandEncoding.MI, "r/m32 OR imm32.", 32, 1),
                new OpCode(new byte[] { 0x83 }, Constants.OR, OperandEncoding.MI, "r/m32 OR imm8 (sign-extended).", 8, 1),
                new OpCode(new byte[] { 0x08 }, Constants.OR, OperandEncoding.MR, "r/m8 OR r8.", 8),
                new OpCode(new byte[] { 0x09 }, Constants.OR, OperandEncoding.MR, "r/m32 OR r32.", 32),
                new OpCode(new byte[] { 0x0A }, Constants.OR, OperandEncoding.RM, "r8 OR r/m8.", 8),
                new OpCode(new byte[] { 0x0B }, Constants.OR, OperandEncoding.RM, "r32 OR r/m32.", 32),

                #endregion

                #region POP

                new OpCode(new byte[] { 0x8F }, Constants.POP, OperandEncoding.M, "Pop top of stack into m32; increment stack pointer.", 32, 0),
                new OpCode(new byte[] { 0x58 }, Constants.POP, OperandEncoding.O, "Pop top of stack into r32; increment stack pointer.", 32),
                new OpCode(new byte[] { 0x1F }, Constants.POP, OperandEncoding.ZO, "Pop top of stack into DS; increment stack pointer.", 32),
                new OpCode(new byte[] { 0x07 }, Constants.POP, OperandEncoding.ZO, "Pop top of stack into ES; increment stack pointer.", 32),
                new OpCode(new byte[] { 0x17 }, Constants.POP, OperandEncoding.ZO, "Pop top of stack into SS; increment stack pointer.", 32),
                new OpCode(new byte[] { 0x0F, 0xA1 }, Constants.POP, OperandEncoding.ZO, "Pop top of stack into FS; increment stack pointer by 32 bits.", 32),
                new OpCode(new byte[] { 0x0F, 0xA9 }, Constants.POP, OperandEncoding.ZO, "Pop top of stack into GS; increment stack pointer by 32 bits.", 32),

                #endregion

                #region PUSH

                new OpCode(new byte[] { 0xFF }, Constants.PUSH, OperandEncoding.M, "Push r/m32.", 32, 6),
                new OpCode(new byte[] { 0x50 }, Constants.PUSH, OperandEncoding.O, "Push r32.", 32),
                new OpCode(new byte[] { 0x6A }, Constants.PUSH, OperandEncoding.I, "Push imm8.", 8),
                new OpCode(new byte[] { 0x68 }, Constants.PUSH, OperandEncoding.I, "Push imm32.", 32),
                new OpCode(new byte[] { 0x0E }, Constants.PUSH, OperandEncoding.ZO, "Push CS.", 32),
                new OpCode(new byte[] { 0x16 }, Constants.PUSH, OperandEncoding.ZO, "Push SS.", 32),
                new OpCode(new byte[] { 0x1E }, Constants.PUSH, OperandEncoding.ZO, "Push DS.", 32),
                new OpCode(new byte[] { 0x06 }, Constants.PUSH, OperandEncoding.ZO, "Push ES.", 32),
                new OpCode(new byte[] { 0x0F, 0xA0 }, Constants.PUSH, OperandEncoding.ZO, "Push FS.", 32),
                new OpCode(new byte[] { 0x0F, 0xA8 }, Constants.PUSH, OperandEncoding.ZO, "Push GS.", 32),

                #endregion

                #region RET

                new OpCode(new byte[] { 0xC3 }, Constants.RET, OperandEncoding.ZO, "Near return to calling procedure.", 32),
                new OpCode(new byte[] { 0xCB }, Constants.RET, OperandEncoding.ZO, "Far return to calling procedure.", 32),
                new OpCode(new byte[] { 0xC2 }, Constants.RET, OperandEncoding.I, "Near return to calling procedure and pop imm16 bytes from stack.", 16),
                new OpCode(new byte[] { 0xCA }, Constants.RET, OperandEncoding.I, "Far return to calling procedure and pop imm16 bytes from stack.", 16),

                #endregion

                #region SAL / SAR

                new OpCode(new byte[] { 0xD0 }, Constants.SAL, OperandEncoding.M1, "Multiply r/m8 by 2, once.", 8, 4),
                new OpCode(new byte[] { 0xD2 }, Constants.SAL, OperandEncoding.MC, "Multiply r/m8 by 2, CL times.", 8, 4),
                new OpCode(new byte[] { 0xC0 }, Constants.SAL, OperandEncoding.MI, "Multiply r/m8 by 2, imm8 times.", 8, 4),

                new OpCode(new byte[] { 0xD1 }, Constants.SAL, OperandEncoding.M1, "Multiply r/m32 by 2, once.", 32, 4),
                new OpCode(new byte[] { 0xD3 }, Constants.SAL, OperandEncoding.MC, "Multiply r/m32 by 2, CL times.", 32, 4),
                new OpCode(new byte[] { 0xC1 }, Constants.SAL, OperandEncoding.MI, "Multiply r/m32 by 2, imm8 times.", 32, 4),

                new OpCode(new byte[] { 0xD0 }, Constants.SAR, OperandEncoding.M1, "Signed divide* r/m8 by 2, once.", 8, 7),
                new OpCode(new byte[] { 0xD2 }, Constants.SAR, OperandEncoding.MC, "Signed divide* r/m8 by 2, CL times.", 8, 7),
                new OpCode(new byte[] { 0xC0 }, Constants.SAR, OperandEncoding.MI, "Signed divide* r/m8 by 2, imm8 time.", 8, 7),

                new OpCode(new byte[] { 0xD1 }, Constants.SAR, OperandEncoding.M1, "Signed divide* r/m32 by 2, once.", 32, 7),
                new OpCode(new byte[] { 0xD3 }, Constants.SAR, OperandEncoding.MC, "Signed divide* r/m32 by 2, CL times.", 32, 7),
                new OpCode(new byte[] { 0xC1 }, Constants.SAR, OperandEncoding.MI, "Signed divide* r/m32 by 2, imm8 time.", 32, 7),

                new OpCode(new byte[] { 0xD0 }, Constants.SHL, OperandEncoding.M1, "Multiply r/m8 by 2, once.", 8, 4),
                new OpCode(new byte[] { 0xD2 }, Constants.SHL, OperandEncoding.MC, "Multiply r/m8 by 2, CL times.", 8, 4),
                new OpCode(new byte[] { 0xC0 }, Constants.SHL, OperandEncoding.MI, "Multiply r/m8 by 2, imm8 times.", 8, 4),

                new OpCode(new byte[] { 0xD1 }, Constants.SHL, OperandEncoding.M1, "Multiply r/m32 by 2, once.", 32, 4),
                new OpCode(new byte[] { 0xD3 }, Constants.SHL, OperandEncoding.MC, "Multiply r/m32 by 2, CL times.", 32, 4),
                new OpCode(new byte[] { 0xC1 }, Constants.SHL, OperandEncoding.MI, "Multiply r/m32 by 2, imm8 times.", 32, 4),

                new OpCode(new byte[] { 0xD0 }, Constants.SHR, OperandEncoding.M1, "Unsigned divide* r/m8 by 2, once.", 8, 5),
                new OpCode(new byte[] { 0xD2 }, Constants.SHR, OperandEncoding.MC, "Unsigned divide* r/m8 by 2, CL times.", 8, 5),
                new OpCode(new byte[] { 0xC0 }, Constants.SHR, OperandEncoding.MI, "Unsigned divide* r/m8 by 2, imm8 time.", 8, 5),

                new OpCode(new byte[] { 0xD1 }, Constants.SHR, OperandEncoding.M1, "Unsigned divide* r/m32 by 2, once.", 32, 5),
                new OpCode(new byte[] { 0xD3 }, Constants.SHR, OperandEncoding.MC, "Unsigned divide* r/m32 by 2, CL times.", 32, 5),
                new OpCode(new byte[] { 0xC1 }, Constants.SHR, OperandEncoding.MI, "Unsigned divide* r/m32 by 2, imm8 time.", 32, 5),

                #endregion

                #region SBB

                new OpCode(new byte[] { 0x1C }, Constants.SBB, OperandEncoding.I, "Subtract with borrow imm8 from AL.", 8),
                new OpCode(new byte[] { 0x1D }, Constants.SBB, OperandEncoding.I, "Subtract with borrow imm32 from EAX.", 32),
                new OpCode(new byte[] { 0x80 }, Constants.SBB, OperandEncoding.MI, "Subtract with borrow imm8 from r/m8.", 8, 3),
                new OpCode(new byte[] { 0x81 }, Constants.SBB, OperandEncoding.MI, "Subtract with borrow imm32 from r/m32.", 32, 3),
                new OpCode(new byte[] { 0x83 }, Constants.SBB, OperandEncoding.MI, "Subtract with borrow sign-extended imm8 from r/m32.", 32, 3),
                new OpCode(new byte[] { 0x18 }, Constants.SBB, OperandEncoding.MR, "Subtract with borrow r8 from r/m8.", 8),
                new OpCode(new byte[] { 0x19 }, Constants.SBB, OperandEncoding.MR, "Subtract with borrow r32 from r/m32.", 32),
                new OpCode(new byte[] { 0x1A }, Constants.SBB, OperandEncoding.RM, "Subtract with borrow r/m8 from r8.", 8),
                new OpCode(new byte[] { 0x1B }, Constants.SBB, OperandEncoding.RM, "Subtract with borrow r/m32 from r32.", 32),

                #endregion

                #region TEST

                new OpCode(new byte[] { 0xA8 }, Constants.TEST, OperandEncoding.I, "AND imm8 with AL; set SF, ZF, PF according to result.", 8),
                new OpCode(new byte[] { 0xA9 }, Constants.TEST, OperandEncoding.I, "AND imm32 with EAX; set SF, ZF, PF according to result.", 32),
                new OpCode(new byte[] { 0xF6 }, Constants.TEST, OperandEncoding.MI, "AND imm8 with r/m8; set SF, ZF, PF according to result.", 8, 0),
                new OpCode(new byte[] { 0xF7 }, Constants.TEST, OperandEncoding.MI, "AND imm32 with r/m32; set SF, ZF, PF according to result.", 32, 0),
                new OpCode(new byte[] { 0x84 }, Constants.TEST, OperandEncoding.MR, "AND r8 with r/m8; set SF, ZF, PF according to result.", 8, 0),
                new OpCode(new byte[] { 0x85 }, Constants.TEST, OperandEncoding.MR, "AND r32 with r/m32; set SF, ZF, PF according to result.", 32, 0),

                #endregion

                #region XOR

                new OpCode(new byte[] { 0x34 }, Constants.XOR, OperandEncoding.I, "AL XOR imm8.", 8),
                new OpCode(new byte[] { 0x35 }, Constants.XOR, OperandEncoding.I, "EAX XOR imm32.", 32),
                new OpCode(new byte[] { 0x80 }, Constants.XOR, OperandEncoding.MI, "r/m8 XOR imm8.", 8, 6),
                new OpCode(new byte[] { 0x81 }, Constants.XOR, OperandEncoding.MI, "r/m32 XOR imm32.", 32, 6),
                new OpCode(new byte[] { 0x83 }, Constants.XOR, OperandEncoding.MI, "r/m32 XOR imm8 (sign-extended).", 8, 6),
                new OpCode(new byte[] { 0x30 }, Constants.XOR, OperandEncoding.MR, "r/m8 XOR r8.", 8),
                new OpCode(new byte[] { 0x31 }, Constants.XOR, OperandEncoding.MR, "r/m32 XOR r32.", 32),
                new OpCode(new byte[] { 0x32 }, Constants.XOR, OperandEncoding.RM, "r8 XOR r/m8.", 8),
                new OpCode(new byte[] { 0x33 }, Constants.XOR, OperandEncoding.RM, "r32 XOR r/m32.", 32),

                #endregion
            };

            LookupTable = new Dictionary<byte[], List<OpCode>>(new ByteArrayComparer());
            ExtensionLookupTable = new Dictionary<byte[], Dictionary<int, OpCode>>(new ByteArrayComparer());

            foreach (OpCode Item in InstructionList)
            {
                if (LookupTable.ContainsKey(Item.Code))
                {
                    LookupTable[Item.Code].Add(Item);
                }
                else
                {
                    LookupTable.Add(Item.Code, new List<OpCode>() { Item });                   
                }

                // Anything that requires an opcode extension, add to the
                // other lookup table
                if (Item.Extension != -1)
                {
                    if (!ExtensionLookupTable.ContainsKey(Item.Code))
                    {
                        ExtensionLookupTable.Add(Item.Code, new Dictionary<int, OpCode>());
                    }

                    // Make sure in cases like SAL/SHL that we don't add instructions that
                    // have the same opcode and same extension
                    if (!ExtensionLookupTable[Item.Code].ContainsKey(Item.Extension))
                    {
                        ExtensionLookupTable[Item.Code].Add(Item.Extension, Item);
                    }
                }
            }
        }

        protected OpCode(byte[] code, string name, OperandEncoding opEn, string description, int operandSize, int extension = -1, bool signExtended = false, int signExtensionSize = 64)
        {
            this.Code = code;
            this.Name = name;
            this.OpEn = opEn;
            this.Description = description;
            this.OperandSize = operandSize;
            this.Extension = extension;
            this.SignExtendedImmediate = signExtended;
            this.SignExtensionSize = signExtensionSize;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the provided OpCode requires an extension to
        /// determine the actual instruction. If true, call Parse() with the 
        /// MODR/M byte parameter
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public static bool RequiresOpCodeExtension(byte[] opcode)
        {
            return ExtensionLookupTable.ContainsKey(opcode);
        }

        /// <summary>
        /// Specifies if this op code requires the ModRM byte
        /// </summary>
        /// <returns></returns>
        public bool RequiresModRM()
        {
            return this.OpEn.RequiresModRM();
        }

        /// <summary>
        /// Parses the opcode byte(s) to an OpCode object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="modrm"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, out OpCode opCode, out Register offsetRegister, byte modrm = 0x00)
        {
            offsetRegister = null;

            if (!LookupTable.ContainsKey(data))
            {
                byte Reg = (byte)(0x07 & data[data.Length - 1]); // This will save the last 3 bits

                if (Register.IsValidRegister(Reg))
                {
                    offsetRegister = Register.GetRegister(Reg);
                    // Drop the last 3 bits
                    data[data.Length - 1] = (byte)((data[data.Length - 1] >> 3) << 3);
                }
            }

            if (LookupTable.ContainsKey(data))
            {
                if (LookupTable[data].Count == 1)
                {
                    opCode = LookupTable[data].First();

                    // This means the encoded register is EAX
                    if ((opCode.OpEn == OperandEncoding.O ||
                        opCode.OpEn == OperandEncoding.OI) && offsetRegister == null)
                    {
                        offsetRegister = Register.EAX;
                    }

                    return true;
                }
                else
                {
                    if (modrm != 0x00)
                    {
                        int Reg = (modrm >> 3) & 0x07;

                        opCode = ExtensionLookupTable[data][Reg];

                        // This means the encoded register is EAX
                        if ((opCode.OpEn == OperandEncoding.O ||
                            opCode.OpEn == OperandEncoding.OI) && offsetRegister == null)
                        {
                            offsetRegister = Register.EAX;
                        }

                        return true;
                    }
                    else
                    {
                        opCode = null;
                        return false;
                    }
                }
            }
            else
            {
                opCode = null;
                return false;
            }
        }

        /// <summary>
        /// Returns the name of the instruction the OpCode represents
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
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

            OpCode Other = (OpCode)obj;

            return this.Name.Equals(Other.Name) &&
                this.Code.Equals(Other.Code) &&
                this.Extension.Equals(Other.Extension) &&
                this.Description.Equals(Other.Description) &&
                this.OpEn.Equals(Other.OpEn);
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(this.Code, this.Name, this.Description, this.Extension, this.OpEn);
        }

        public static bool operator ==(OpCode left, OpCode right)
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

        public static bool operator !=(OpCode left, OpCode right)
        {
            return !(left == right);
        }

        #endregion
    }
}
