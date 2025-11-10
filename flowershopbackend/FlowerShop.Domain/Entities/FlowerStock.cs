using FlowerShop.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Domain.Entities
{
    public class FlowerStock
    {
        public long Id { get; set; }
        public long FlowerId { get; set; }
        public DateTime ImportedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int Quantity { get; set; }
        public FlowerQuantityUnit QuantityUnit { get; set; }
        public void UpdateStock(int quantity, FlowerQuantityUnit unit)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
            if (unit != QuantityUnit && Quantity > 0)
                throw new InvalidOperationException("Cannot change unit when stock is not zero");

            Quantity += quantity;
            QuantityUnit = unit;
            LastModifiedDate = DateTime.Now;
        }
       
    }
    public enum FlowerQuantityUnit
    {
        Single,
        Package
    }
}
