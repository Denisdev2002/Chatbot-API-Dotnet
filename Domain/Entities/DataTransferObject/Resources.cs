using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DataTransferObject
{
    public class Resources
    {
        public string? nome { get; set; }
        public string? acao { get; set; }
    }
    public class ApiResponse
    {
        public string AppToken { get; set; }
        public List<Resources> Recursos { get; set; }
    }
}
