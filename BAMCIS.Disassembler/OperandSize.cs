using System;
using System.Collections.Generic;
using System.Text;

namespace BAMCIS.Disassembler
{
    public enum OperandSize : Int32
    {
        NONE = 0x00,

        EIGHT = 0x08,

        SIXTEEN = 0x10,

        THIRTY_TWO = 0x20,

        SIXTY_FOUR = 0x40,

        THIRTYTWO_AND_SIXTYFOUR = 0x41,

        VARIABLE = 0x01
    }
}
