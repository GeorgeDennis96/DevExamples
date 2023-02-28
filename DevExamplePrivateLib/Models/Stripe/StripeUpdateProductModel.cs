﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models.Stripe
{
    public class StripeUpdateProductModel
    {
        public string Product_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Default_Price_Id { get; set; }
    }
}
