namespace BiermanTech.CriticalDog.Analytics
{
    public class ObservationChangeReport
    {
        public string SubjectName { get; set; }
        public string ObservationType { get; set; }
        public string StandardUnitName { get; set; }
        public string DisplayUnitName { get; set; }
        public List<Observation> Observations { get; set; } = new List<Observation>();
        public double? AverageRatePerDay { get; set; }
        public string TrendDescription { get; set; }
    }

    public class Observation
    {
        public DateTime RecordTime { get; set; }
        public decimal Value { get; set; }
        public double? PercentChangePerWeek { get; set; }
    }
}