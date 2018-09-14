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
        public void Test1()
        {
            // ARRANGE
            // move esi, ebx
            byte[] Instructions = new byte[] { 0x8B, 0xF3 };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);

        }

        [Fact]
        public void Test2()
        {
            // ARRANGE
            // move [ esi ], ebx
            byte[] Instructions = new byte[] { 0x89, 0x1E };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.MOV, Ins.OpCode.Name);

        }

        [Fact]
        public void Test3()
        {
            // ARRANGE
            // move [ esi ], ebx
            byte[] Instructions = new byte[] { 0x81, 0xC7, 0x44, 0x33, 0x22, 0x11 };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.ADD, Ins.OpCode.Name);
            Assert.Equal(new byte[]{ 0x44, 0x33, 0x22, 0x11}, Ins.Immediate);

        }

        [Fact]
        public void Test4()
        {
            // ARRANGE
            // move [ esi ], ebx
            byte[] Instructions = new byte[] { 0x81, 0x87, 0xDD, 0xCC, 0xBB, 0xAA, 0x44, 0x33, 0x22, 0x11 };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.ADD, Ins.OpCode.Name);
            Assert.Equal(new byte[] { 0x44, 0x33, 0x22, 0x11 }, Ins.Immediate);
            Assert.Equal(new byte[] { 0xDD, 0xCC, 0xBB, 0xAA }, Ins.Displacement);

        }

        [Fact]
        public void Test5()
        {
            // ARRANGE
            byte[] Instructions = new byte[] { 0x8B, 0xF3, 0x89, 0x1E };

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(2, Parsed.Count());
        }

        [Fact]
        public void Test6()
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
        }

        [Fact]
        public void Test7()
        {
            // ARRANGE
            // clflush [eax]
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
        }

        [Fact]
        public void Test8()
        {
            // ARRANGE
            // clflush [eax]
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
        }

        [Fact]
        public void Test9()
        {
            // ARRANGE
            // clflush [eax]
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
        }

        [Fact]
        public void Test10()
        {
            // ARRANGE
            // clflush [0x11223344]
            byte[] Instructions = new byte[] {
                   0x0F, 0xAE, 0x3D, 0x44, 0x33, 0x22, 0x11
            };

            // 0011 1101

            // ACT
            IEnumerable<Instruction> Parsed = Instruction.LinearSweep(Instructions).ToList();

            // ASSERT
            Assert.Equal(1, Parsed.Count());
            Instruction Ins = Parsed.First();
            Assert.Equal(Constants.CLFLUSH, Ins.OpCode.Name);
            Assert.Equal(new byte[] { 0x44, 0x33, 0x22, 0x11 }, Ins.Displacement);
        }
    }
}
