using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XCNTWebServer.Models
{
    public class ExpenseDTO
    {
        [Key]
        public string ExpenseUUID { get; set; }
        public string EmployeeUUID { get; set; }
        public string First_Name { get; set; }
        public int Amount { get; set; }
        public bool Approved { get; set; }
    }
}
