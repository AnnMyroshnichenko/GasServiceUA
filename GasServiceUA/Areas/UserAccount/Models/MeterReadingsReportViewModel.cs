using GasServiceUA.Models;

namespace GasServiceUA.Areas.UserAccount.Models
{
    public class MeterReadingsReportViewModel
    {
        public User User { get; set; }
        public IEnumerable<MeterReading> MeterReadings { get; set; }
        public IEnumerable<Bill> Bills { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
