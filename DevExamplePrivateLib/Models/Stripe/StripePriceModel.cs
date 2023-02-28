using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models.Stripe
{
    // Stripe prices are not stored in the product class
    // Instead the price class specifies what product it is attached too.
    // Prices cannot be attached to multiple products

    public class StripePriceModel
    {
        // The product this price is attached to
        public string Product_Id { get; set; }
        public string Nickname { get; set; }
        public string Currency { get; set; }
        public long Unit_Amount { get; set; }
        public bool Active { get; set; }

        // recurring or one_time
        public string Type { get; set; }

        // day, week, month or year
        public string Interval { get; set; }
        public string Interval_Count { get; set; }
        public string Aggregate_Usage { get; set; }
        public bool Is_Default_Price { get; set; }


    }
}
