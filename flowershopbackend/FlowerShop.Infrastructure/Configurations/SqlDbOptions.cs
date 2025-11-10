using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Infrastructure.Configurations
{
    public class SqlDbOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public bool EnableDetailedErrors { get; set; }
    }
}
