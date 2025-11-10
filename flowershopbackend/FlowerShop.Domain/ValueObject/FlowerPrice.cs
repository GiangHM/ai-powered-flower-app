using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Domain.ValueObject
{
    public class FlowerPrice
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public FlowerPrice(decimal amount, string currency = "VND")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public static FlowerPrice Zero => new(0);

        public static implicit operator decimal(FlowerPrice price) => price.Amount;

        public override string ToString() => $"{Amount:N0} {Currency}";
    }
}
