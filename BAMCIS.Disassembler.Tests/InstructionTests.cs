using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BAMCIS.Disassembler.Tests
{
    public class InstructionTests
    {
        [Fact]
        public void TestBothDirectRegisterAccess()
        {
            // ARRANGE
            // mov esi, ebx
            byte[] Instructions = new byte[] { 0x8B, 0xF3 };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tmov\t\tesi, ebx", Ins.ToString());

        }

        [Fact]
        public void TestIndirectMemoryAccess()
        {
            // ARRANGE
            // mov [esi], ebx
            byte[] Instructions = new byte[] { 0x89, 0x1E };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tmov\t\t[esi], ebx", Ins.ToString());

        }

        [Fact]
        public void TestAddWithImmediate()
        {
            // ARRANGE
            // add edi, 0x11223344
            byte[] Instructions = new byte[] { 0x81, 0xC7, 0x44, 0x33, 0x22, 0x11 };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.ADD, Ins.OpCode.Name);
            Assert.Equal(new byte[]{ 0x44, 0x33, 0x22, 0x11}, Ins.Immediate);
            Assert.Equal("0x00000000\t\tadd\t\tedi, 0x11223344", Ins.ToString());

        }

        [Fact]
        public void TestAddWithDisplacementAndImmediate()
        {
            // ARRANGE
            // add [ edi + aabbccdd ], 0x11223344
            byte[] Instructions = new byte[] { 0x81, 0x87, 0xDD, 0xCC, 0xBB, 0xAA, 0x44, 0x33, 0x22, 0x11 };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.ADD, Ins.OpCode.Name);
            Assert.Equal(new byte[] { 0x44, 0x33, 0x22, 0x11 }, Ins.Immediate);
            Assert.Equal(new byte[] { 0xDD, 0xCC, 0xBB, 0xAA }, Ins.Displacement);
            Assert.Equal("0x00000000\t\tadd\t\t[edi+0xAABBCCDD], 0x11223344", Ins.ToString());
        }

        [Fact]
        public void Test2MoveInstructions()
        {
            // ARRANGE
            byte[] Instructions = new byte[] {
                0x8B, 0xF3, // mov esi, ebx
                0x89, 0x1E // mov [esi], ebx
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();
            
            foreach (Instruction In in Parsed)
            {
                Buffer.AppendLine(In.ToString());
            }

            // ASSERT
            Assert.Equal(2, Parsed.Count());

            Assert.Equal("0x00000000\t\tmov\t\tesi, ebx\r\n0x00000002\t\tmov\t\t[esi], ebx\r\n", Buffer.ToString());
        }

        [Fact]
        public void TestClflush1()
        {
            // ARRANGE
            // clflush [eax]
            byte[] Instructions = new byte[] {
                   0x0F, 0xAE, 0x38,
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.CLFLUSH, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tclflush\t\t[eax]", Ins.ToString());
        }

        [Fact]
        public void TestClflush2()
        {
            // ARRANGE
            // clflush [eax+0x8]
            byte[] Instructions = new byte[] {
                   0x0F, 0xAE, 0x78, 0x08
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.CLFLUSH, Ins.OpCode.Name);
            Assert.Equal(Register.EAX.Code, Ins.ModRM.RM);
            Assert.Equal(new byte[] { 0x08, 0, 0, 0 }, Ins.Displacement);
            Assert.Equal("0x00000000\t\tclflush\t\t[eax+0x08]", Ins.ToString());
        }

        [Fact]
        public void TestClflush3()
        {
            // ARRANGE
            // clflush [eax+0x8]
            byte[] Instructions = new byte[] {
                   0x0F, 0xAE, 0xB8, 0x08, 0x00, 0x00, 0x00
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.CLFLUSH, Ins.OpCode.Name);
            Assert.Equal(Register.EAX.Code, Ins.ModRM.RM);
            Assert.Equal(new byte[] { 0x08, 0, 0, 0 }, Ins.Displacement);
            Assert.Equal("0x00000000\t\tclflush\t\t[eax+0x00000008]", Ins.ToString());

        }

        [Fact]
        public void TestClflush4()
        {
            // ARRANGE
            // clflush [esi+edi*4+0x8]
            byte[] Instructions = new byte[] {
                   0x0F, 0xAE, 0xBC, 0xBE, 0x08, 0x00, 0x00, 0x00
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.CLFLUSH, Ins.OpCode.Name);
            Assert.Equal(Scale.TIMES_FOUR, Ins.SIB.Scale);
            Assert.Equal(Register.ESI.Code, Ins.SIB.Base);
            Assert.Equal(Register.EDI.Code, Ins.SIB.Index);
            Assert.Equal(new byte[] { 0x08, 0, 0, 0 }, Ins.Displacement);
            Assert.Equal("0x00000000\t\tclflush\t\t[edi*4+esi+0x00000008]", Ins.ToString());
        }

        [Fact]
        public void TestClflush5()
        {
            // ARRANGE
            // clflush [esi+edi*4+0x8]
            byte[] Instructions = new byte[] {
                0x0F, 0xAE, 0x7C, 0xBE, 0x08
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.CLFLUSH, Ins.OpCode.Name);
            Assert.Equal(Scale.TIMES_FOUR, Ins.SIB.Scale);
            Assert.Equal(Register.ESI.Code, Ins.SIB.Base);
            Assert.Equal(Register.EDI.Code, Ins.SIB.Index);
            Assert.Equal(new byte[] { 0x08, 0, 0, 0 }, Ins.Displacement);
            Assert.Equal("0x00000000\t\tclflush\t\t[edi*4+esi+0x08]", Ins.ToString());
        }

        [Fact]
        public void TestClflush6()
        {
            // ARRANGE
            // clflush [0x11223344]
            byte[] Instructions = new byte[] {
                   0x0F, 0xAE, 0x3D, 0x44, 0x33, 0x22, 0x11
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.CLFLUSH, Ins.OpCode.Name);
            Assert.Equal(new byte[] { 0x44, 0x33, 0x22, 0x11 }, Ins.Displacement);
            Assert.Equal("0x00000000\t\tclflush\t\t[0x11223344]", Ins.ToString());
        }

        [Fact]
        public void MultipleInstructionTest()
        {
            // ARRANGE
            // push ebp
            // call offset_00000008
            // nop
            // nop
            // nop
            byte[] Instructions = new byte[] {
                0x55,
                0xE8, 0x02, 0x00, 0x00, 0x00,
                0x90,
                0x90,
                0x90
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(5, Parsed.Count());
            Assert.True(!String.IsNullOrEmpty(Instruc));
        }

        [Fact]
        public void TestSIB1()
        {
            // ARRANGE
            // mov [ esi*8 + edi ], ebx
            byte[] Instructions = new byte[] {
                0x89, 0x1C, 0xF7
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tmov\t\t[esi*8+edi], ebx\r\n", Instruc);
        }

        [Fact]
        public void TestSIB2()
        {
            // ARRANGE
            // mov [ esi*4 + edi + 0xaabbccdd ], 0x11223344
            byte[] Instructions = new byte[] {
                0xC7, 0x84, 0xB7, 0xdd, 0xcc, 0xbb, 0xaa, 0x44, 0x33, 0x22, 0x11
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tmov\t\t[esi*4+edi+0xAABBCCDD], 0x11223344\r\n", Instruc);
        }

        [Fact]
        public void TestSIB3()
        {
            // ARRANGE
            // lea ecx, [ebp*2+ebx]
            byte[] Instructions = new byte[] {
                0x8D, 0x0C, 0x6B
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.LEA, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tlea\t\tecx, [ebp*2+ebx]\r\n", Instruc);
        }

        [Fact]
        public void TestSIB4()
        {
            // ARRANGE
            // mov [ esi*4 ], 0x11223344
            byte[] Instructions = new byte[] {
                0xC7, 0x04, 0xB5, 0x00, 0x00, 0x00, 0x00, 0x44, 0x33, 0x22, 0x11
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tmov\t\t[esi*4], 0x11223344\r\n", Instruc);
        }

        [Fact]
        public void TestSIB5()
        {
            // ARRANGE
            // mov [ esp ], ecx
            byte[] Instructions = new byte[] {
                0x89, 0x0C, 0xE4
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);
            Assert.Equal("0x00000000\t\tmov\t\t[esp], ecx\r\n", Instruc);
        }

        [Fact]
        public void TestBranch1()
        {
            // ARRANGE
            // mov [ esp ], ecx
            byte[] Instructions = new byte[] {
                0xC3, // retn
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0xE9, 0xF3, 0xFF, 0xFF, 0xFF // jmp offset_00000000
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            Buffer.Length = Buffer.Length - 2; // Remove last \r\n

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(9, Parsed.Count());
            Instruction Ins = Parsed.Last();
            Assert.Equal(Constants.JMP, Ins.OpCode.Name);
            string LastLine = Instruc.Split("\r\n").Last();
            Assert.Equal("0x00000008\t\tjmp\t\toffset_00000000", LastLine);
        }

        [Fact]
        public void TestBranch2()
        {
            // ARRANGE
            // mov [ esp ], ecx
            byte[] Instructions = new byte[] {
                0xC3, // retn
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0x90, // nop
                0xEB, 0xF6 // jmp offset_00000000
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            StringBuilder Buffer = new StringBuilder();

            foreach (Instruction Inst in Parsed)
            {
                Buffer.AppendLine(Inst.ToString());
            }

            Buffer.Length = Buffer.Length - 2; // Remove last \r\n

            string Instruc = Buffer.ToString();

            // ASSERT
            Assert.Equal(9, Parsed.Count());
            Instruction Ins = Parsed.Last();
            Assert.Equal(Constants.JMP, Ins.OpCode.Name);
            string LastLine = Instruc.Split("\r\n").Last();
            Assert.Equal("0x00000008\t\tjmp\t\toffset_00000000", LastLine);
        }

        [Fact]
        public void TestSAL1()
        {
            // ARRANGE
            // shl ecx, 1
            byte[] Instructions = new byte[] {
                0xD1, 0xE1
            };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.Last();
            Assert.Equal(Constants.SAL, Ins.OpCode.Name);
            
            Assert.Equal("0x00000000\t\tsal\t\tecx, 1", Ins.ToString());
        }
    }
}
