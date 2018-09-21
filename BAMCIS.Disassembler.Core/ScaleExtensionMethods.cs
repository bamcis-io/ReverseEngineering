namespace BAMCIS.Disassembler.Core
{
    public static class ScaleExtensionMethods
    {
        public static int GetScaleFactor(this Scale scale)
        {
            switch (scale)
            {
                case Scale.TIMES_ONE:
                    {
                        return 1;
                    }
                case Scale.TIMES_TWO:
                    {
                        return 2;
                    }
                case Scale.TIMES_FOUR:
                    {
                        return 4;
                    }
                case Scale.TIMES_EIGHT:
                    {
                        return 8;
                    }
                default:
                    {
                        return 1;
                    }
            }
        }
    }
}
