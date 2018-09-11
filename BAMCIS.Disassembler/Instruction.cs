using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BAMCIS.Disassembler
{
    public class Instruction
    {
        #region Public Properties

        public Prefix Prefix { get; }

        public OpCode OpCode { get; }

        public MODRM MODRM { get; }

        public SIB SIB { get; }

        public int Displacement { get; }

        public int Immediate { get; }

        #endregion

        #region Constructors

        public Instruction(MemoryStream stream)
        {
            if (stream.Position < stream.Length - 1)
            {
                byte FirstByte = (byte)stream.ReadByte();

                IEnumerable<Prefix> Prefixes = Prefix.Parse(FirstByte);
                Prefix Pre = Prefixes.First();
                
                if (Pre != Prefix.NONE)
                {
                    // Find the right prefix
                }
                else
                {
                    // Move back to the original byte
                    stream.Position = stream.Position - 1;
                }

                this.Prefix = Pre;
                byte[] OpCodeBytes;

                long BytesLeft = stream.Length - stream.Position;
                int BytesToRead = 3;

                switch (BytesLeft)
                {
                    case 1:
                        {
                            OpCodeBytes = new byte[1];
                            BytesToRead = 1;
                            break;
                        }
                    case 2:
                        {
                            OpCodeBytes = new byte[2];
                            BytesToRead = 2;
                            break;
                        }
                    default:
                        {
                            OpCodeBytes = new byte[3];
                            break;
                        }
                }

                stream.Read(OpCodeBytes, 0, BytesToRead);

                this.OpCode = OpCode.Parse(OpCodeBytes);

                // If we took too many bytes, move the stream position back
                stream.Position = stream.Position - (OpCodeBytes.Length - this.OpCode.Code.Length);

                if (this.OpCode.RequiresModRM)
                {
                    if (stream.Position < stream.Length - 1)
                    {
                        this.MODRM = new MODRM((byte)stream.ReadByte());
                    }
                }
                else
                {
                    this.MODRM = null;
                }


                if (this.OpCode.RequiresSIB)
                {
                    if (stream.Position < stream.Length - 1)
                    {
                        this.SIB = new SIB((byte)stream.ReadByte());
                    }
                }
                else
                {
                    this.SIB = null;
                }

                if (this.OpCode.HasDisplacement)
                {

                }

                if (this.OpCode.HasImmediate)
                {

                }
            }
        }

        #endregion

        #region Public Methods

        public IEnumerable<Instruction> LinearSweep(byte[] data)
        {
            using (MemoryStream MStream = new MemoryStream())
            {
                MStream.Write(data, 0, data.Length);
                MStream.Position = 0;

                while (MStream.Position < MStream.Length - 1)
                {
                    yield return new Instruction(MStream);
                }
            }
        }

        #endregion
    }
}
