namespace HypertensionControlUI.Collections
{
    public struct RangeEntry
    {
        public double LowerBound { get; set; }
        public double Value { get; set; }

        public RangeEntry( double lowerBound, double value ) : this()
        {
            LowerBound = lowerBound;
            Value = value;
        }
    }
}