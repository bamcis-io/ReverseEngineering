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
    }
}
