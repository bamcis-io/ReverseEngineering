using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BAMCIS.Disassembler.Core;

namespace BAMCIS.Disassembler.Tests
{
    public class PrefixTests
    {
        [Fact]
        public void TestValidPrefix()
        {
            // ARRANGE
            byte PrefixByte = 0xF0;

            // ACT
            IEnumerable<Prefix> Prefixes = Prefix.Parse(PrefixByte);

            // ASSERT
            Assert.True(Prefixes.Count() == 1);
            Assert.True(Prefixes.First() == Prefix.LOCK);
        }

        [Fact]
        public void TestBadPrefix()
        {
            // ARRANGE
            byte PrefixByte = 0x10;

            // ACT
            IEnumerable<Prefix> Prefixes = Prefix.Parse(PrefixByte);

            // ASSERT
            Assert.True(Prefixes.Count() == 1);
            Assert.True(Prefixes.First() == Prefix.NONE);
        }
    }
}
