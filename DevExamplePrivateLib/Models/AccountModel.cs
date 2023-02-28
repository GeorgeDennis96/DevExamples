using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stripe;

namespace DevExample.Private.Models
{
    [DataContract]
    public class AccountModel
    {
        public dynamic _id { get; set; } = Guid.NewGuid();
        public string firstVisit { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public FileModel profilePictureSource { get; set; }
        public bool isDefaultProfilePicture { get; set; } = true;
        public string nameIdentifier { get; set; }
        public bool acceptedTerms { get; set; }
        public string termsRevision { get; set; }
        public bool ironMan { get; set; } = false;
        public string stripeCustomerId { get; set; }
        public string partnershipId { get; set; }
    }
}
