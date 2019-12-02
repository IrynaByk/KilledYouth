namespace HypertensionControl.Domain.Models.Values
{
    public struct Range
    {
        public double LowerBound { get; private set; }
        public double Value { get; private set; }

        public Range( double lowerBound, double value ) : this()
        {
            LowerBound = lowerBound;
            Value = value;
        }
    }

    public static class GenesNames
    {
        public const string Agt = "AGT";
        public const string Agtr2 = "AGTR2";
        public const string Nos = "NOS";
        public const string Agtr1 = "AGTR1";
        public const string Cyp11B2 = "CYP11B2";
        public const string Mthfr = "MTHFR";
    }
}