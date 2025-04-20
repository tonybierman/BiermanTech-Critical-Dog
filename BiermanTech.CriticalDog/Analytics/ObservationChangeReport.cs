namespace BiermanTech.CriticalDog.Analytics
{
    public class ObservationChangeReport
    {
        public string SubjectName { get; set; }
        public string ObservationType { get; set; } // e.g., WeighIn, TempCheck
        public string UnitName { get; set; } // e.g., Kilograms, DegreesCelsius
        public List<Observation> Observations { get; set; } = new List<Observation>();
        public double? AverageRatePerDay { get; set; } // Rate in units/day
        public string TrendDescription { get; set; }
    }

    public class Observation
    {
        public DateTime RecordTime { get; set; }
        public decimal Value { get; set; } // Value in the specified unit
        public double? PercentChangePerWeek { get; set; } // Percent change per week from previous observation
    }
}