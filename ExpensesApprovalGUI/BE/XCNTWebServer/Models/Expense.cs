using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XCNTWebServer.Models
{
    public class Expense
    {
        [Key]
        [Required]
        public string UUID { get; set; }
        public string Description { get; set; }
        [Required]
        public string Created_at { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }
        public Employee Employee { get; set; }
        [Required]
        public bool Approved { get; set; }
    }

}
