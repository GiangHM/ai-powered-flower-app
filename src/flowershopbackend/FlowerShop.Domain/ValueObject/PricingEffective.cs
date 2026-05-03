using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Domain.ValueObject
{
    public class EffectiveDate
    {
        public EffectiveDate() { }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public EffectiveDate(DateTime fromDate, DateTime toDate)
        {
            From = fromDate;
            To = toDate;
        }
    }
}
