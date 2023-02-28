using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DevExample.Private.Models
{
    [DataContract]
    public class TicketModel
    {
        public string _id { get; set; } = Guid.NewGuid().ToString();
        public string userNameIdentifier { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public List<CommentModel> Comments { get; set; }
        public List<FileModel> Attachments { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public bool Disabled { get; set; }
        public string Created { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        public string CreatedBy { get; set; }
        public string ViewedBy { get; set; } = "Not Viewed Yet";
        public string LastOpened { get; set; }

    }
}
