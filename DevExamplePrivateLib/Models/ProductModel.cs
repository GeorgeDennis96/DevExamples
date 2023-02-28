using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models
{
    public class ProductModel
    {
        public dynamic _id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool CatalogVisible { get; set; }
        public bool Subscribable { get; set; }
        public string DashboardPath { get; set; }
        public bool MenuVisible { get; set; } = false;
        public string SubscriptionType { get; set; } = "onetime";
        public string RequiredPermission { get; set; }

        public dynamic PricingDefinitions { get; set; }

        public string TermsPath { get; set; }

        public string ViewPermission { get; set; }
        public string AdminPermission { get; set; }
        public bool AdminOnly { get; set; } = false;
        public string AdminDashboardPath { get; set; }



    }
}
