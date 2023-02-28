using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models.Stripe
{
    public class StripeProductModel
    {

        public string Name { get; set; }
        public bool Active { get; set; }
        public string Default_Price { get; set; }
        public string Description { get; set; }

        // Extra information about a product which will appear on your customer’s credit card statement.
        public string Statement_Descriptor { get; set; }

        public string Image_Data { get; set; }
    }
}
