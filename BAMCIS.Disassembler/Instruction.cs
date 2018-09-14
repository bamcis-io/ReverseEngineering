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

        public IEnumerable<Prefix> Prefixes { get; }

        public OpCode OpCode { get; }

        public ModRM ModRM { get; }

        public SIB SIB { get; }

        public byte[] Displacement { get; }

        public byte[] Immediate { get; }

        #endregion

        #region Constructors

        private Instruction(MemoryStream stream)
        {
            if (stream.Position < stream.Length - 1)
            {
                Prefix Pre;
                List<Prefix> TempPres = new List<Prefix>();

                int LastByteRead;

                // Deal with optional, variable amount of prefixes
                do
                {
                    LastByteRead = stream.ReadByte();
                    IEnumerable<Prefix> Prefixes = Prefix.Parse((byte)LastByteRead);
                    Pre = Prefixes.First();
                    TempPres.Add(Pre);
                } while (Pre != Prefix.NONE && LastByteRead != -1);

                // If there are not prefixes, we'll just have 1 prefix in there, NONE
                this.Prefixes = TempPres;

                // If there were any prefixes, then we need to know what that last one was
                // Otherwise the last byte read can stay in place
                if (TempPres.Count > 1)
                {
                    stream.Position += -1;
                    LastByteRead = stream.ReadByte();

                    // If the last prefix wasn't a mandatory prefix, read the following byte
                    // Otherwise leave the last byte read as that prefix
                    if (LastByteRead != 0x66 &&
                        LastByteRead != 0xF2 &&
                        LastByteRead != 0xF3)
                    {
                        LastByteRead = stream.ReadByte();
                    }
                }

                // The position in the stream is directly after the last byte read

                int MaxOpCodeLength = 1;

                // The last byte read will either be the last prefix in the list
                // Or it will be a noop or the op code
                switch (LastByteRead)
                {
                    case 0x0F:
                        {
                            // Escape OpCode, the following 1 or 2 bytes will be 
                            // the actual opcode, do nothing
                            MaxOpCodeLength = 2;
                            // Move the stream back since this byte is part of the opcode signature
                            stream.Position += -1;
                            break;
                        }
                    case 0x66:
                    case 0xF2:
                    case 0xF3:
                        {
                            // Mandatory Prefix, next byte should be an escape opcode (where
                            // the stream position currently points),
                            // so move to the byte after that
                            MaxOpCodeLength = 2;
                            break;
                        }
                    default:
                        {
                            // One byte opcode, move stream back 1 so that it will be read
                            stream.Position += -1;
                            break;
                        }
                }

                // Make sure we don't try to read off the end of the stream                
                if (MaxOpCodeLength > (stream.Length - stream.Position))
                {
                    MaxOpCodeLength = (int)(stream.Length - stream.Position);
                }

                // Allocate a buffer to hold the bytes being read
                byte[] OpCodeBytes = new byte[MaxOpCodeLength];

                // Read the bytes
                stream.Read(OpCodeBytes, 0, MaxOpCodeLength);

                bool ValidOpCode = false;
                OpCode Op;
                Register OffsetRegister;

                // Try to see if 2 byte arrangement is a valid opcode
                if (OpCode.RequiresOpCodeExtension(OpCodeBytes))
                {
                    byte ModRM = (byte)stream.ReadByte();
                    stream.Position += -1;
                    ValidOpCode = OpCode.TryParse(OpCodeBytes, out Op, out OffsetRegister, ModRM);
                }
                else
                {
                    ValidOpCode = OpCode.TryParse(OpCodeBytes, out Op, out OffsetRegister);
                }

                // If the 2 bytes weren't a valid opcode, try with just 1 byte
                if (!ValidOpCode && OpCodeBytes.Length == 2)
                {
                    byte ModRM = OpCodeBytes[1];
                    OpCodeBytes = OpCodeBytes.Take(1).ToArray();
                    stream.Position += -1; // Move the stream back 1 position

                    if (OpCode.RequiresOpCodeExtension(OpCodeBytes))
                    {
                        ValidOpCode = OpCode.TryParse(OpCodeBytes, out Op, out OffsetRegister, ModRM);
                    }
                    else
                    {
                        ValidOpCode = OpCode.TryParse(OpCodeBytes, out Op, out OffsetRegister);
                    }
                }

                // Right now the stream position is on the byte right after the op code bytes

                if (ValidOpCode)
                {
                    this.OpCode = Op;

                    if (this.OpCode.RequiresModRM())
                    {
                        int ModRMByte = stream.ReadByte();

                        if (ModRMByte == -1)
                        {
                            return;
                        }
                        this.ModRM = new ModRM((byte)ModRMByte);

                        // If a SIB byte is used, it must follow a ModRM byte
                        if (this.ModRM.SIBByteFollows())
                        {
                            int SIBByte = stream.ReadByte();

                            if (SIBByte == -1)
                            {
                                return;
                            }

                            this.SIB = new SIB((byte)SIBByte);
                        }
                        else
                        {
                            this.SIB = null;
                        }

                        if (this.ModRM.InstructionHasDisplacement())
                        {
                            if (this.ModRM.MOD == MOD.RM_BYTE)
                            {
                                int Displacement = stream.ReadByte();

                                if (Displacement == -1)
                                {
                                    return;
                                }

                                this.Displacement = new byte[4] { (byte)Displacement, 0, 0, 0 };
                            }
                            else
                            {
                                byte[] Disp = new byte[4];
                                int BytesRead = stream.Read(Disp, 0, 4);

                                if (BytesRead == 0)
                                {
                                    return;
                                }

                                this.Displacement = Disp;
                            }
                        }

                        if (this.OpCode.OpEn.InstructionHasImmediate())
                        {
                            byte[] Imm = new byte[4];
                            int BytesRead = stream.Read(Imm, 0, 4);

                            if (BytesRead == 0)
                            {
                                return;
                            }

                            this.Immediate = Imm;
                        }
                    }
                    else
                    {
                        this.ModRM = null;
                        this.SIB = null;
                        this.Displacement = null;
                        this.Immediate = null;
                    }
                }
                // Otherwise, skip this byte and move on
            }
        }

        #endregion

        #region Public Methods

        public static IEnumerable<Instruction> LinearSweep(byte[] data)
        {
            using (MemoryStream MStream = new MemoryStream())
            {
                MStream.Write(data, 0, data.Length);
                MStream.Position = 0;

                while (MStream.Position < MStream.Length)
                {
                    yield return new Instruction(MStream);
                }
            }
        }

        #endregion
    }
}
