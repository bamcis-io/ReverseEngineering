using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BAMCIS.Disassembler
{
    public class Instruction
    {
        #region Private Fields

        private byte[] _Bytes;

        #endregion

        #region Public Properties

        public Int64 Address { get; }

        public byte[] Bytes {
            get
            {
                return this._Bytes;
            }
        }

        public IEnumerable<Prefix> Prefixes { get; }

        public OpCode OpCode { get; }

        public ModRM ModRM { get; }

        public SIB SIB { get; }

        public Register EncodedRegister { get; }

        public byte[] Displacement { get; }

        public byte[] Immediate { get; }

        #endregion

        #region Constructors

        private Instruction(MemoryStream stream)
        {
            this.Address = stream.Position;

            if (stream.Position < stream.Length)
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
                Register OffsetRegister = null;

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
                    this.EncodedRegister = OffsetRegister;

                    this.OpCode = Op;

                    if (this.OpCode.RequiresModRM())
                    {
                        int ModRMByte = stream.ReadByte();

                        if (ModRMByte == -1)
                        {
                            this.OpCode = null;
                            this.Finalize(stream);
                            return;
                        }
                        this.ModRM = new ModRM((byte)ModRMByte);

                        // If a SIB byte is used, it must follow a ModRM byte
                        if (this.ModRM.SIBByteFollows())
                        {
                            int SIBByte = stream.ReadByte();

                            if (SIBByte == -1)
                            {
                                this.OpCode = null;
                                this.Finalize(stream);
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
                                    this.OpCode = null;
                                    this.Finalize(stream);
                                    return;
                                }

                                this.Displacement = new byte[4] { (byte)Displacement, 0, 0, 0 };
                            }
                            else
                            {
                                byte[] Disp = new byte[4];
                                int BytesRead = stream.Read(Disp, 0, 4);

                                if (BytesRead == -1)
                                {
                                    this.OpCode = null;
                                    this.Finalize(stream);
                                    return;
                                }

                                if (BytesRead == 0)
                                {
                                    this.Finalize(stream);
                                    return;
                                }

                                this.Displacement = Disp;
                            }                          
                        }
                        else if (this.SIB != null && this.SIB.Base == Register.EBP.Code)
                        {
                            byte[] Disp = new byte[4];
                            int BytesRead = stream.Read(Disp, 0, 4);

                            if (BytesRead == -1)
                            {
                                this.OpCode = null;
                                this.Finalize(stream);
                                return;
                            }

                            if (BytesRead == 0)
                            {
                                this.Finalize(stream);
                                return;
                            }

                            this.Displacement = Disp;
                        }
                    }
                    else
                    {
                        this.ModRM = null;
                        this.SIB = null;
                        this.Displacement = null;
                    }

                    if (this.OpCode.OpEn.InstructionHasImmediate())
                    {
                        if (this.OpCode.SignExtendedImmediate)
                        {
                            byte[] Imm = new byte[this.OpCode.OperandSize / 8];
                            int BytesRead = stream.Read(Imm, 0, Imm.Length);

                            byte[] Imm2 = new byte[this.OpCode.SignExtensionSize / 8];

                            if (BytesRead < Imm2.Length)
                            {
                                Array.Copy(Imm, Imm2, Imm.Length);
                                byte Padding = 0x00;

                                if ((Imm[Imm.Length - 1] & 0x80) == 0x80)
                                {
                                    Padding = 0xFF;
                                }

                                for (int i = Imm.Length; i < Imm2.Length; i++)
                                {
                                    Imm2[i] = Padding;
                                }

                                this.Immediate = Imm2;
                            }
                            else
                            {
                                this.Immediate = Imm;
                            }
                        }
                        else
                        {
                            byte[] Imm = new byte[this.OpCode.OperandSize / 8];
                            int BytesRead = stream.Read(Imm, 0, Imm.Length);

                            if (BytesRead == -1)
                            {
                                this.OpCode = null;
                                this.Finalize(stream);
                                return;
                            }

                            if (BytesRead == 0)
                            {
                                this.Finalize(stream);
                                return;
                            }

                            this.Immediate = Imm;
                        }
                    }
                }
                // Otherwise, skip this byte and move on
            }

            this.Finalize(stream);
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

        public override string ToString()
        {
            StringBuilder Buffer = new StringBuilder($"0x{this.Address.ToString("X8")}\t\t");

            if (this.OpCode == null)
            {
                Buffer.Append($" db {this.Bytes[0].ToString("X2")} INVALID OPCODE");
            }
            else
            {
                Buffer.Append($"{this.OpCode.Name.ToLower()}\t\t");

                switch (this.OpCode.OpEn)
                {
                    case OperandEncoding.D:
                        {
                            long Value = 0;

                            
                            if (
                                this.OpCode.Code.SequenceEqual(new byte[] { 0xE8 }) || // NEAR CALL where displacement is relative to next instruction
                                this.OpCode.Code.SequenceEqual(new byte[] { 0x74 }) || // JZ
                                this.OpCode.Code.SequenceEqual(new byte[] { 0x75 }) || // JNZ
                                this.OpCode.Code.SequenceEqual(new byte[] { 0x0F, 0x84 }) || // JZ
                                this.OpCode.Code.SequenceEqual(new byte[] { 0x0F, 0x85 }) || // JNZ
                                this.OpCode.Code.SequenceEqual(new byte[] { 0xEB }) || // JMP
                                this.OpCode.Code.SequenceEqual(new byte[] { 0xE9 }) // JMP
                                )
                            {
                                if (this.Immediate.Length == 1)
                                {
                                    byte[] Bytes = new byte[2] { this.Immediate[0], 0x00 };
                                    Value = unchecked(BitConverter.ToInt16(Bytes, 0) + this.Address + this.Bytes.Length);
                                }
                                else if (this.Immediate.Length == 2)
                                {
                                    Value = unchecked(BitConverter.ToInt16(this.Immediate, 0) + this.Address + this.Bytes.Length);
                                }
                                else if (this.Immediate.Length == 4)
                                {
                                    Value = unchecked(BitConverter.ToInt32(this.Immediate, 0) + this.Address + this.Bytes.Length);
                                }
                                else
                                {
                                    Value = unchecked(BitConverter.ToInt64(this.Immediate, 0) + this.Address + this.Bytes.Length);
                                }
                                                              
                                Buffer.Append("offset_");

                                byte[] ValueBytes = BitConverter.GetBytes(Value);

                                Buffer.Append(ConvertToHexString(ValueBytes).Substring(2));
                            }
                            else
                            {
                                if (this.Immediate.Length == 1)
                                {
                                    byte[] Bytes = new byte[2] { this.Immediate[0], 0x00 };
                                    Value = BitConverter.ToInt16(Bytes, 0);
                                }
                                else if (this.Immediate.Length == 2)
                                {
                                    Value = BitConverter.ToInt16(this.Immediate, 0);
                                }
                                else if (this.Immediate.Length == 4)
                                {
                                    Value = BitConverter.ToInt32(this.Immediate, 0);
                                }
                                else
                                {
                                    Value = BitConverter.ToInt64(this.Immediate, 0);
                                }

                                byte[] ValueBytes = BitConverter.GetBytes(Value);

                                Buffer.Append(ConvertToHexString(ValueBytes));
                            }

                            break;
                        }                   
                    case OperandEncoding.I:
                        {
                            Buffer.Append(ConvertToHexString(this.Immediate));
                            break;
                        }
                    case OperandEncoding.M:
                        {
                            Buffer.Append(this.GetRegMemOperandString());

                            break;
                        }
                    case OperandEncoding.M1:
                        {
                            Buffer.Append(this.GetRegMemOperandString());
                            Buffer.Append(", 1");
                            break;
                        }
                    case OperandEncoding.MC:
                        {
                            Buffer.Append(this.GetRegMemOperandString());
                            Buffer.Append(", CL");
                            break;
                        }
                    case OperandEncoding.MI:
                        {
                            Buffer.Append(this.GetRegMemOperandString());
                            Buffer.Append(", ");
                            Buffer.Append(ConvertToHexString(this.Immediate));
                            break;
                        }
                    case OperandEncoding.MR:
                        {
                            Buffer.Append(this.GetRegMemOperandString());
                            Buffer.Append(", ");
                            Buffer.Append(Register.GetRegister(this.ModRM.REG).Name.ToLower());
                            break;
                        }
                    case OperandEncoding.O:
                        {
                            Buffer.Append(this.EncodedRegister.ToString().ToLower());
                            break;
                        }
                    case OperandEncoding.OI:
                        {
                            Buffer.Append(this.EncodedRegister.ToString().ToLower());
                            Buffer.Append(", ");
                            Buffer.Append(ConvertToHexString(this.Immediate));
                            break;
                        }
                    case OperandEncoding.RM:
                        {
                            Buffer.Append(Register.GetRegister(this.ModRM.REG).Name.ToLower());                            
                            Buffer.Append(", ");
                            Buffer.Append(this.GetRegMemOperandString());

                            break;
                        }
                    case OperandEncoding.RMI:
                        {
                            Buffer.Append(Register.GetRegister(this.ModRM.REG).Name.ToLower());
                            Buffer.Append(", ");
                            Buffer.Append(this.GetRegMemOperandString());
                            Buffer.Append(", ");
                            Buffer.Append(ConvertToHexString(this.Immediate));
                            break;
                        }
                    case OperandEncoding.TD:
                        {
                            Buffer.Append("Moffs\t\tEAX");
                            break;
                        }
                    case OperandEncoding.FD:
                        {
                            Buffer.Append("EAX\t\tMoffs");
                            break;
                        }
                    default:
                    case OperandEncoding.NP:
                    case OperandEncoding.ZO:
                        {
                            // No operands
                            break;
                        }
                }
            }

            return Buffer.ToString();
        }

        public static string ConvertToHexString(byte[] bytes)
        {
            long Value = 0;

            if (bytes.Length == 1)
            {
                byte[] Temp = new byte[] { bytes[0], 0x00 };
                Value = BitConverter.ToInt16(Temp, 0);
            }
            else if (bytes.Length == 2)
            {
                Value = BitConverter.ToInt16(bytes, 0);
            }
            else if (bytes.Length == 4)
            {
                Value = BitConverter.ToInt32(bytes, 0);
            }
            else if (bytes.Length == 8)
            {
                Value = BitConverter.ToInt64(bytes, 0);
            }
            else
            {
                throw new ArgumentOutOfRangeException("bytes", "The maximum length of the input is 8 bytes and must be 1, 2, 4, or 8.");
            }

            byte[] ValueBytes = BitConverter.GetBytes(Value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ValueBytes); // Convert from little endian
            }

            string HexString = BitConverter.ToString(ValueBytes);
            HexString = "0x" + HexString.Replace("-", "").PadLeft(8, '0').Tail(8);

            return HexString;
        }

        #endregion

        #region Private Methods

        private void Finalize(MemoryStream stream)
        {
            long Quantity = 1;

            if (this.OpCode != null)
            {
                Quantity = stream.Position - this.Address;
            }

            stream.Position = this.Address;
            this._Bytes = new byte[Quantity];
            stream.Read(this.Bytes, 0, (int)Quantity);
        }

        private string GetRegMemOperandString()
        {
            StringBuilder Buffer = new StringBuilder();

            if (this.OperandRequiresModification())
            {
                if (this.SIB != null)
                {
                    Buffer.Append(this.SIB.ToString(this.ModRM, this.Displacement));
                }
                else // Only other option is displacement if SIB is null
                {
                    Buffer.Append("[");

                    if (this.ModRM.MOD == MOD.RM && this.ModRM.RM == 0x05)
                    {
                        // Displacement only
                        Buffer.Append(ConvertToHexString(this.Displacement));
                    }
                    else
                    {
                        Buffer.Append(Register.GetRegister(this.ModRM.RM).Name.ToLower());
                        Buffer.Append("+");

                        if (ModRM.MOD == MOD.RM_BYTE)
                        {
                            Buffer.Append("0x");
                            Buffer.Append(ConvertToHexString(this.Displacement).Tail(2));
                        }
                        else
                        {
                            Buffer.Append(ConvertToHexString(this.Displacement));
                        }
                    }

                    Buffer.Append("]");
                }
            }
            else
            {
                if (this.ModRM != null)
                {
                    if (this.ModRM.MOD == MOD.RM)
                    {
                        Buffer.Append("[");
                        Buffer.Append(Register.GetRegister(this.ModRM.RM).Name.ToLower());
                        Buffer.Append("]");
                    }
                    else
                    {
                        Buffer.Append(Register.GetRegister(this.ModRM.RM).Name.ToLower());
                    }
                }
                else
                {
                    // Otherwise it should have a ModRM, which may mean this is last instruction
                    // and not everything was included
                    Buffer.Append("MODRM MISSING");
                }
            }

            return Buffer.ToString();
        }

        public bool OperandRequiresModification()
        {
            return ((this.Displacement != null && this.Displacement.Length > 0) || this.SIB != null);
        }

        #endregion
    }
}
