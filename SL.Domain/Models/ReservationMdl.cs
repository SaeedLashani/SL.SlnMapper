using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SL.Domain.Validation;

namespace SL.Domain.Models
{
    public class ReservationMdl
    {
        [Required]
        [GreaterThanZero]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        
        [Required, StringLength(200, ErrorMessage = "Customer name cannot exceed 200 characters.")]
        public string CustomerName { get; set; }
    }
}
