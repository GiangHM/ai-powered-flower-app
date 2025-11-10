using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Application.Dtos
{
    public class FlowerResponseItem
    {
        public float Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitCurrency { get; set; }
        public string CategoryName { get; set; }
        public string Image { get; set; }
    }
    public class FlowerAdminResponse : FlowerResponseItem
    {
        public bool Status { get; set; }
    }
}
