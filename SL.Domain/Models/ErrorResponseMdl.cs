using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Domain.Models
{
    public class ErrorResponseMdl
    {
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public bool IsShowMessage { get; set; } = false;
    }
}
