using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models
{
    public class CommentModel
    {
        public string _id { get; set; } = Guid.NewGuid().ToString();
        public string ReplierName { get; set; }
        public string Message { get; set; }
        public bool CanEdit { get; set; } = true;
        public string RepliedOn { get; set; }
        public string CreatedOn { get; set; }
        public string LastUpdated { get; set; }

    }
}
