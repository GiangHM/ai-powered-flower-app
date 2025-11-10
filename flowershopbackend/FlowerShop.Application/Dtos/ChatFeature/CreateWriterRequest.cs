using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Application.Dtos.ChatFeature
{
    public class CreateWriterRequest
    {
        public required string Research { get; init; }
        public required string Writing { get; init; }
    }
}
