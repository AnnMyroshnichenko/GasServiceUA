using GasServiceUA.Models;

namespace GasServiceUA.Areas.UserAccount.Models
{
    public class UserAccountViewModel
    {
        public User User { get; set; }
        public IEnumerable<Payment> Payments { get; set; } = new List<Payment>();
        public IEnumerable<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
        public IEnumerable<Bill> Bills { get; set; } = new List<Bill>();
    }
}
