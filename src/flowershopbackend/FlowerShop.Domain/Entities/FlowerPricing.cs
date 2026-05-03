using FlowerShop.Domain.ValueObject;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Domain.Entities
{
    public class FlowerPricing
    {
        public long Id { get; set; }
        public long FlowerId { get; set; }
        public FlowerPrice Price { get; set; } = FlowerPrice.Zero;
        public EffectiveDate PriceEffective { get; set; } = new EffectiveDate(DateTime.UtcNow, DateTime.MaxValue);
        public void UpdatePrice(decimal amount, string currency)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            var newPricing = new FlowerPrice(amount, currency);
            Price = newPricing;
        }

        public void UpdateEffectiveDate(DateTime startDate, DateTime endDate)
        {
            var newEffectiveDate = new EffectiveDate(startDate, endDate);
            PriceEffective = newEffectiveDate;
        }
    }
}
