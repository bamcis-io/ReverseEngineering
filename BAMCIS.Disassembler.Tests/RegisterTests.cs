using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BAMCIS.Disassembler.Tests
{
    public class RegisterTests
    {
        [Fact]
        public void TestValidRegister()
        {
            // ARRANGE
            byte code = 0x00;

            // ACT
            Register EAX = Register.GetRegister(code);

            // ASSERT
            Assert.Equal(Register.EAX, EAX);
        }

        [Fact]
        public void TestInvalidRegister()
        {
            // ARRANGE
            byte code = 0xFF;

            // ASSERT
            Assert.Throws<ArgumentException>(() => {
                // ACT
                Register.GetRegister(code);
            });
        }
    }
}
