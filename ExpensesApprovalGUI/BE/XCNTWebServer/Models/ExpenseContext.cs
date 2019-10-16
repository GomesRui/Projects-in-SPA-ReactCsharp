using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace XCNTWebServer.Models
{
    public class ExpenseContext : DbContext
    {

        public ExpenseContext(DbContextOptions<ExpenseContext> options)
             : base(options)
        { }



        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Employee> Employees { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
         {
             modelBuilder.Entity<Employee>().HasData(
                 new Employee
                 {
                     EmployeeId = 1,
                     UUID = "858142ac-299a-48f0-b221-7d6de9439454",
                     First_Name = "Birthe",
                     Last_Name = "Meier",
                     ExpenseId = 1
                 });

             modelBuilder.Entity<Expense>().HasData(
               new Expense
               {
                   ExpenseId = 1,
                   Amount = 2293,
                   UUID = "92b19fc6-5386-4985-bf5c-dc56c903dd24",
                   Description = "Itaque fugiat repellendus velit deserunt praesentium.",
                   Created_At = "2019-09-22T23:07:01",
                   Currency = "UZS",
                   Approved = true
               });
         }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(c => c.Expenses)
                .WithOne(e => e.Employee)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
