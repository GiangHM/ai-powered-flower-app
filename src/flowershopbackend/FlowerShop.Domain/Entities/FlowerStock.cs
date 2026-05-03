using FlowerShop.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Domain.Entities
{
    public enum StockUpdateMode
    {
        Increase,
        Decrease
    }

    public class FlowerStock
    {
        public long Id { get; set; }
        public long FlowerId { get; set; }
        public DateTime ImportedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int Quantity { get; set; }
        public FlowerQuantityUnit QuantityUnit { get; set; }

        public void UpdateStock(int quantity, FlowerQuantityUnit unit, StockUpdateMode mode = StockUpdateMode.Increase)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            if (unit != QuantityUnit && Quantity > 0)
                throw new InvalidOperationException("Cannot change unit when stock is not zero");

            var delta = mode == StockUpdateMode.Increase ? quantity : -quantity;
            var nextQuantity = Quantity + delta;
            if (nextQuantity < 0)
                throw new InvalidOperationException("Insufficient stock for this operation");

            Quantity = nextQuantity;
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
