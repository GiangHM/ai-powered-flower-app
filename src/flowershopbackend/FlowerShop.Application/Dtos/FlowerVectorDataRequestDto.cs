using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Application.Dtos
{
    public class FlowerVectorDataRequestDto
    {
        public float Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitCurrency { get; set; }
        public string Description { get; set; }
    }
    public class InitVectorDataRequest
    {
        public string Action { get; set; } = "Upsert";
        public List<FlowerVectorDataRequestDto> Payload { get; set; }
    }
}
