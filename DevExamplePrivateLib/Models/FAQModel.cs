//using DevExample.Private.Attributes;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace DevExample.Private.Models
//{
//    [AutoData(Database: "DevExamplePlatform", Collection: "FAQ")]
//    [DataContract]
//    public class FAQModel : AutoUIBaseModel
//    {
//        [JsonProperty]
//        [AutoUI("textbox")]
//        public string Title { get; set; }

//        [JsonProperty]
//        [AutoUI("textbox", ShowInTable:false)]
//        public string Content { get; set; }

//        [JsonProperty]
//        [AutoUI("textbox")]
//        public string AddedBy { get; set; }
        
//        [JsonProperty]
//        [AutoUI("textbox", ReadOnly:true)]
//        public string AddedDate { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm");

//        [JsonProperty]
//        [AutoUI("textbox", showInEditView:false, ReadOnly:true)]
//        public string LastUpdated { get; set; } = "No Updates";

//        public List<string> Likes { get; set; }

//        [JsonProperty]
//        [AutoUI("textbox", ShowInTable:true)]
//        public int LikesCount { get; set; }

//        [JsonProperty]
//        [AutoUI("toggle", ShowInTable:true)]
//        public bool IsVisible { get; set; }

//        [JsonProperty]
//        [AutoUI("button", TableLabel: "")]
//        public string Edit { get { return "/admin/support/faq/edit/" + Id; } set { } } //url
//    }
//}
