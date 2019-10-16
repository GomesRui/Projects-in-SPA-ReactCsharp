using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XCNTWebServer.Models
{
    public class Employee
    {
        [Key]
        [Required]
        public string UUID { get; set; }
        [Required]
        public string Last_name { get; set; }
        [Required]
        public string First_name { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }

    }
}
