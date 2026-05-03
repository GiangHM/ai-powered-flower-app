using Confluent.Kafka;

namespace FlowerShop.Application.Interfaces
{
    public interface IKafakaProducerService<TKey, TValue>
    {
        Task<bool> ProduceAsync(string topic, TKey key, TValue value);
    }
}
