using Confluent.Kafka;
using FlowerShop.Application.Common;
using FlowerShop.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Infrastructure.KafkaServices
{
    public class KafkaProducerService<TKey, TValue> : IKafakaProducerService<TKey, TValue>
    {
        private readonly IProducer<TKey, TValue> _producer;
        private const int MaxRetryCount = 3;
        private readonly TimeSpan _retryDelay;
        private readonly ILogger<KafkaProducerService<TKey, TValue>> _logger;

        public KafkaProducerService(IProducer<TKey, TValue> producer
            , ILogger<KafkaProducerService<TKey, TValue>> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryDelay = TimeSpan.FromSeconds(2);
        }

        /// <summary>
        /// Produces a message to the specified Kafka topic, retrying up to 3 times in case of consistent errors.
        /// </summary>
        /// <param name="topic">Kafka topic name</param>
        /// <param name="key">Message key</param>
        /// <param name="value">Message value</param>
        /// <returns>A Task representing the async operation</returns>
        public async Task<bool> ProduceAsync(string topic, TKey key, TValue value)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Topic must not be null or whitespace.", nameof(topic));

            int attempt = 0;
            Exception? lastException = null;
            DeliveryResult<TKey, TValue>? result = null!;
            while (attempt < MaxRetryCount)
            {
                try
                {
                    var message = new Message<TKey, TValue> { Key = key, Value = value };
                    result = await _producer.ProduceAsync(topic, message);

                    _logger.LogInformation("Producing message to Kafka sucessfully,Pattition {0}, Offset {1}"
                        , result.Partition.Value, result.Offset.Value);
                    return true;
                }
                catch (ProduceException<TKey, TValue> ex)
                {
                    lastException = ex;
                    attempt++;
                    if (attempt < MaxRetryCount)
                    {
                        await Task.Delay(_retryDelay);
                    }
                    _logger.LogError(ex, "Error producing Kafka message. Attempt {Attempt} of {MaxRetryCount}.", attempt, MaxRetryCount);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while producing Kafka message.");
                }
            }
            return false;
        }
    }
}
