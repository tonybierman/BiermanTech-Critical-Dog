namespace BiermanTech.CriticalDog.Analytics
{
    public class WeightChangeReport
    {
        public string SubjectName { get; set; }
        public List<WeightObservation> Observations { get; set; } = new List<WeightObservation>();
        public double? AverageRateKgPerDay { get; set; }
        public string TrendDescription { get; set; }
    }

    public class WeightObservation
    {
        public DateTime RecordTime { get; set; }
        public decimal WeightKg { get; set; } // Standardized to Kilograms
    }
}