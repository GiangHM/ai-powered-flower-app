using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Domain.Entities
{
    public class FlowerCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<Flower> Flowers { get; set; } = new List<Flower>();
    }
}
