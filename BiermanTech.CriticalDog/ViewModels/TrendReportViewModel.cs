namespace BiermanTech.CriticalDog.ViewModels
{
    public class TrendReportViewModel
    {
        public string SubjectName { get; set; }
        public string ObservationTypeName { get; set; }
        public string StandardUnitName { get; set; }
        public string StandardUnitSymbol { get; set; }
        public string DisplayUnitName { get; set; }
        public string DisplayUnitSymbol { get; set; }
        public List<TrendReportRecordViewModel> Records { get; set; } = new List<TrendReportRecordViewModel>();
        public double? AverageWeeklyRate { get; set; }
        public string TrendDescription { get; set; }
    }

    public class TrendReportRecordViewModel
    {
        public DateTime RecordTime { get; set; }
        public decimal Value { get; set; }
        public double? PercentChangePerWeek { get; set; }
        public decimal AmountOfChange { get; set; }
    }
}