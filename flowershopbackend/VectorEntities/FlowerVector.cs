using Microsoft.Extensions.VectorData;

namespace FlowerShop.Domain.VectorEntities
{
    public class FlowerVector
    {
        [VectorStoreKey]
        public float Id { get; set; }
        [VectorStoreData]
        public string? Name { get; set; }
        [VectorStoreData]
        public string? Description { get; set; }
        [VectorStoreData]
        public decimal Price { get; set; }
        [VectorStoreVector(1536)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
}
