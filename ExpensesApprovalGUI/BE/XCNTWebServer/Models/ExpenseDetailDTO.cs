using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XCNTWebServer.Models
{
    public class ExpenseDetailDTO
    {

        [Key]
        public string UUID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Description { get; set; }
        public string Created_At { get; set; }
        public int Amount { get; set; }
        [MaxLength(3)]
        public string Currency { get; set; }
        public bool Approved { get; set; }
    }
}
