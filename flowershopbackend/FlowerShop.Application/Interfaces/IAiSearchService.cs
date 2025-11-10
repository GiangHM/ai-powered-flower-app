using FlowerShop.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Application.Interfaces
{
    public interface IAiSearchService
    {
        Task<AiSearchResponse> Search(string searchString);
    }
}
