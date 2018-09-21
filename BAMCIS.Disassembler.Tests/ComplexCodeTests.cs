using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.IO;

namespace BAMCIS.Disassembler.Tests
{
    public class ComplexCodeTests
    {
        [Fact]
        public void TestSignExtension()
        {
            // ARRANGE
            byte Temp = 0xF1;

            // ACT
            long Temp2 = (long)Temp;

            // ASSERT

            Assert.Equal(241, Temp2);
        }

        [Fact]
        public void TestSignExtension2()
        {
            // ARRANGE
            uint Temp = 0xFFFFFFF1;

            // ACT
            long Temp2 = (int)Temp;

            // ASSERT

            Assert.Equal(-15, Temp2);
        }

        [Fact]
        public void TestExample1O()
        {
            // ARRANGE
            using (FileStream FS = File.OpenRead("CodeFiles\\example1.o"))
            {
                byte[] Content = new byte[FS.Length];
                FS.Read(Content, 0, Content.Length);

                // ACT
                IEnumerable<Instruction> Instructions = Instruction.LinearSweep(Content);

                StringBuilder Buffer = new StringBuilder();

                foreach (Instruction Ins in Instructions)
                {
                    Buffer.AppendLine(Ins.ToString());
                }

                Buffer.Length += -2;

                string Commands = Buffer.ToString();

                // ASSERT
                Assert.True(!String.IsNullOrEmpty(Commands));
            }
        }

        [Fact]
        public void TestExample2O()
        {
            // ARRANGE
            using (FileStream FS = File.OpenRead("CodeFiles\\example2.o"))
            {
                byte[] Content = new byte[FS.Length];
                FS.Read(Content, 0, Content.Length);

                // ACT
                IEnumerable<Instruction> Instructions = Instruction.LinearSweep(Content);

                StringBuilder Buffer = new StringBuilder();

                foreach (Instruction Ins in Instructions)
                {
                    Buffer.AppendLine(Ins.ToString());
                }

                Buffer.Length += -2;

                string Commands = Buffer.ToString();

                // ASSERT
                Assert.True(!String.IsNullOrEmpty(Commands));
            }
        }

        [Fact]
        public void TestMoreExamplesO()
        {
            // ARRANGE
            using (FileStream FS = File.OpenRead("CodeFiles\\more_examples.o"))
            {
                byte[] Content = new byte[FS.Length];
                FS.Read(Content, 0, Content.Length);

                // ACT
                IEnumerable<Instruction> Instructions = Instruction.LinearSweep(Content);

                StringBuilder Buffer = new StringBuilder();

                foreach (Instruction Ins in Instructions)
                {
                    Buffer.AppendLine(Ins.ToString());
                }

                Buffer.Length += -2;

                string Commands = Buffer.ToString();

                // ASSERT
                Assert.True(!String.IsNullOrEmpty(Commands));
            }
        }

        [Fact]
        public void TestEx2O()
        {
            // ARRANGE
            using (FileStream FS = File.OpenRead("CodeFiles\\ex2.o"))
            {
                byte[] Content = new byte[FS.Length];
                FS.Read(Content, 0, Content.Length);

                // ACT
                IEnumerable<Instruction> Instructions = Instruction.LinearSweep(Content);

                StringBuilder Buffer = new StringBuilder();

                foreach (Instruction Ins in Instructions)
                {
                    Buffer.AppendLine(Ins.ToString());
                }

                Buffer.Length += -2;

                string Commands = Buffer.ToString();

                // ASSERT
                Assert.True(!String.IsNullOrEmpty(Commands));
            }
        }
    }
}
