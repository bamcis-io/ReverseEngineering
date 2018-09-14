using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BAMCIS.Disassembler.Tests
{
    public class MODRMTests
    {
        [Fact]
        public void TestMODRM1()
        {
            // ARRANGE
            byte MODRM = 0xFF;

            // ACT
            ModRM modrm = new ModRM(MODRM);

            // ASSERT
            Assert.Equal(MOD.RM_DIRECT, modrm.MOD);
            Assert.Equal(0x07, modrm.REG);
            Assert.Equal(0x07, modrm.RM);
        }

        [Fact]
        public void TestMODRM2()
        {
            // ARRANGE
            byte MODRM = 0xFE;

            // ACT
            ModRM modrm = new ModRM(MODRM);

            // ASSERT
            Assert.Equal(MOD.RM_DIRECT, modrm.MOD);
            Assert.Equal(0x07, modrm.REG);
            Assert.Equal(0x06, modrm.RM);
        }

        [Fact]
        public void TestMODRM3()
        {
            // ARRANGE
            byte MODRM = 0x0F;

            // ACT
            ModRM modrm = new ModRM(MODRM);

            // ASSERT
            Assert.Equal(MOD.RM, modrm.MOD);
            Assert.Equal(0x01, modrm.REG);
            Assert.Equal(0x07, modrm.RM);
        }
    }
}
