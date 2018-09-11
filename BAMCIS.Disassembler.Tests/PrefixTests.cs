using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BAMCIS.Disassembler.Tests
{
    public class PrefixTests
    {
        [Fact]
        public void TestValidPrefix()
        {
            // ARRANGE
            byte Prefix = 0xF0;

            // ACT
            IEnumerable<Prefix> Prefixes = Disassembler.Prefix.Parse(Prefix);

            // ASSERT
            Assert.True(Prefixes.Count() == 1);
            Assert.True(Prefixes.First() == Disassembler.Prefix.LOCK);
        }

        [Fact]
        public void TestBadPrefix()
        {
            // ARRANGE
            byte Prefix = 0x10;

            // ACT
            IEnumerable<Prefix> Prefixes = Disassembler.Prefix.Parse(Prefix);

            // ASSERT
            Assert.True(Prefixes.Count() == 1);
            Assert.True(Prefixes.First() == Disassembler.Prefix.NONE);
        }
    }
}
