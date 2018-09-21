using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BAMCIS.Disassembler.Core;

namespace BAMCIS.Disassembler.Tests
{
    public class OpCodeTests
    {
        [Fact]
        public void TestAddNoModRM()
        {
            // ARRANGE
            byte[] Bytes = new byte[] { 0x03 };

            // ACT
            bool Result = OpCode.TryParse(Bytes, out OpCode Op, out Register OffsetRegister);

            // ASSERT
            Assert.NotNull(Op);
            Assert.Equal(Bytes, Op.Code);
            Assert.Equal(Constants.ADD, Op.Name);
            Assert.True(Op.RequiresModRM());
        }

        [Fact]
        public void TestAddWithModRM()
        {
            // ARRANGE
            byte[] Bytes = new byte[] { 0x83 };

            // ACT
            bool Result = OpCode.TryParse(Bytes, out OpCode Op, out Register OffsetRegister, 0xC7);

            // ASSERT
            Assert.NotNull(Op);
            Assert.Equal(Bytes, Op.Code);
            Assert.Equal(Constants.ADD, Op.Name);
            Assert.Equal(0, Op.Extension);
            Assert.True(Op.RequiresModRM());
        }

        [Fact]
        public void TestPop()
        {
            // ARRANGE
            byte[] Bytes = new byte[] { 0x59 };

            // ACT
            bool Result = OpCode.TryParse(Bytes, out OpCode Op, out Register OffsetRegister);

            // ASSERT
            Assert.NotNull(Op);
            Assert.Equal(Bytes, Op.Code);
            Assert.Equal(Constants.POP, Op.Name);
            Assert.False(Op.RequiresModRM());
        }
    }
}
