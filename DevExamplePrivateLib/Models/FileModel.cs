using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DevExample.Private.Models
{
    public class FileModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Data { get; set; }
        public string Name { get; set; }
    }
}
