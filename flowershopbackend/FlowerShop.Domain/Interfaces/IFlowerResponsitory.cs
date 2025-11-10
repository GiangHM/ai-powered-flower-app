using FlowerShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Domain.Interfaces
{
    public interface IFlowerResponsitory
    {
        Task<Flower?> GetByIdAsync(long id);
        Task<IEnumerable<Flower>> GetAllAsync();
        Task<IEnumerable<Flower>> GetActivatedFlowerAsync();
        Task<IEnumerable<Flower>> GetByIdsAsync(IEnumerable<long> ids);
        Task<Flower> AddAsync(Flower item);
        Task UpdateAsync(Flower item);
        Task DeleteAsync(long id);
        Task<IEnumerable<Flower>> SearchAsync(string keyword);
    }
}
