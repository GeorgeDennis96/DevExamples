using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models
{



    public class SubscriptionModel
    {

        public dynamic _id { get; set; } = Guid.NewGuid();
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public string productName { get; set; }
        public string userNameIdentifier { get; set; }

        public dynamic pricingOverrides { get; set; }
        public bool cancelled { get; set; } = false;
        public bool paused { get; set; } = false;
        public string SubscriptionType {get;set;}
        public List<SubscriptionInvoiceModel> Invoices { get; set; } //generate paid and unpaid invoices
        public List<MeteredUnitModel> MeteredUnits { get; set; } // generated metered billing units

        
    }

public static class SubscriptionType
{
    public static String OneTimePayment { get { return "onetime"; } }
    public static String RecurringPayment { get { return "recurring"; } }
    public static String RecurringMeteredPayment { get { return "metered"; } }

}

}
