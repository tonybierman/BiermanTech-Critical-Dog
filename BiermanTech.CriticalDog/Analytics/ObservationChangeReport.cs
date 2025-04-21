namespace BiermanTech.CriticalDog.Analytics
{
    public class ObservationChangeReport
    {
        public string SubjectName { get; set; }
        public string ObservationTypeName { get; set; }
        public string StandardUnitName { get; set; }
        public string StandardUnitSymbol { get; set; }
        public string DisplayUnitName { get; set; }
        public string DisplayUnitSymbol { get; set; }
        public List<ObservationRecord> ObservationRecords { get; set; } = new List<ObservationRecord>();
        public double? AverageWeeklyRate { get; set; }
        public string TrendDescription { get; set; }
    }

    public class ObservationRecord
    {
        public DateTime RecordTime { get; set; }
        public decimal Value { get; set; }
        public double? PercentChangePerWeek { get; set; }
        public decimal AmountOfChange { get; set; }
    }
}