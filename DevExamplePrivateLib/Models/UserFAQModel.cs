using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models
{
    public class UserFAQModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
    }
}
