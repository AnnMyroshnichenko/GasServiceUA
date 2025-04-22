using GasServiceUA.Models;

namespace GasServiceUA.Areas.UserAccount.Models
{
    public class PaymentHistoryReportViewModel
    {
        public User User { get; set; }
        public IEnumerable<Payment> Payments { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
