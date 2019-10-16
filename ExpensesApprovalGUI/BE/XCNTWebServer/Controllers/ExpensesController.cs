using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XCNTWebServer.Models;

namespace XCNTWebServer.Controllers
{
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ExpenseContext _context;

        public ExpensesController(ExpenseContext context)
        {
            _context = context;
        }

        // GET: api/ExpensesDTO
        [Route("api/ExpensesDTO")]
        [ResponseType(typeof(List<ExpenseDTO>))]
        public IEnumerable<ExpenseDTO> GetExpensesDTO()
        {
            var expenses = from x in _context.Expenses
                           select new ExpenseDTO()
                           {
                               ExpenseUUID = x.UUID,
                               EmployeeUUID = x.Employee.UUID,
                               First_Name = x.Employee.First_name,
                               Amount = x.Amount,
                               Approved = x.Approved
                           };
            return expenses;
        }

        // GET: api/Expenses
        [Route("api/Expenses")]
        [ResponseType(typeof(List<Expense>))]
        public async Task<IActionResult> GetExpenses()
        {
            var dbData = await _context.Expenses.AsNoTracking().Include(exp => exp.Employee).ToListAsync();
            return Ok(dbData);
        }

        // GET: api/Expenses/5
        [Route("api/Expenses/{id}")]
        [ResponseType(typeof(ExpenseDetailDTO))]
        public async Task<IActionResult> GetExpense([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var expense = await _context.Expenses.AsNoTracking().Include(x => x.Employee).Select(x => new ExpenseDetailDTO()
            {
                
                UUID = x.UUID,
                Description = x.Description,
                Amount = x.Amount,
                Currency = x.Currency,
                Approved = x.Approved,
                Created_At = x.Created_at,
                First_Name = x.Employee.First_name,
                Last_Name = x.Employee.Last_name

            }).SingleOrDefaultAsync(b => b.UUID == id);

            if (expense == null)
            {
                return NotFound();
            }

            return Ok(expense);
        }

        // PUT: api/Expenses/5
        [HttpPut]
        [Route("api/Expenses/{id}")]
        public async Task<IActionResult> PutExpense([FromRoute] string id, [FromQuery]bool approved)
        {
            var _expense = await _context.Expenses.AsNoTracking().Include("Employee").SingleOrDefaultAsync(_exp => _exp.UUID == id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_expense == null)
            {
                return NotFound($"Expense not found with the UUID {id}");
            }

            var newExpense = new Expense()
            {
                Description = _expense.Description,
                Amount = _expense.Amount,
                Created_at = _expense.Created_at,
                Approved = approved,
                Currency = _expense.Currency,
                Employee = _expense.Employee,
                UUID = id
            };

            try
            {
                _context.Entry(newExpense).Property(x => x.Approved).IsModified = true; //just update approved field
                await _context.SaveChangesAsync();

            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }


            await _context.Entry(newExpense).Reference(x => x.Employee).LoadAsync();

            var expenseDto = new ExpenseDTO()
            {
                ExpenseUUID = newExpense.UUID,
                EmployeeUUID = newExpense.Employee.UUID,
                First_Name = newExpense.Employee.First_name,
                Amount = newExpense.Amount,
                Approved = newExpense.Approved
            };

            return CreatedAtAction("GetExpense", new { id = newExpense.UUID }, expenseDto);
        
    }

        // POST: api/Expenses
        [HttpPost]
        [Route("api/Expenses")]
        public async Task<IActionResult> PostExpense([FromBody] List<Expense> expenses)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Expense> newExpenses = new List<Expense>(expenses);

            for (int i = 0; i < newExpenses.Count; i++)
            {

                //Expense newExpense;
                var _employee = await _context.Employees.SingleOrDefaultAsync(_emp => _emp.UUID == expenses[i].Employee.UUID);
                var _expense = await _context.Expenses.SingleOrDefaultAsync(_exp => _exp.UUID == expenses[i].UUID);

                if (_expense == null) //in case employees have more than 1 expense
                {
                    if(_employee != null)
                    {
                        newExpenses[i].Employee = expenses[i].Employee;
                    }
          
                }
                else
                {
                    return BadRequest($"ERROR: The expense UUID {_expense.UUID} already exists!");
                }

            }

            try
            {
                await _context.Expenses.AddRangeAsync(newExpenses);
                await _context.SaveChangesAsync();

            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
           

            //Convert from Simple to DTO Expense to best show results

            var expensesDTO = new List<ExpenseDTO>(newExpenses.Count);

            foreach(var newExpense in newExpenses)
            {
                await _context.Entry(newExpense).Reference(x => x.Employee).LoadAsync();

                var expenseDTO = new ExpenseDTO()
                {
                    ExpenseUUID = newExpense.UUID,
                    EmployeeUUID = newExpense.Employee.UUID,
                    First_Name = newExpense.Employee.First_name,
                    Amount = newExpense.Amount,
                    Approved = newExpense.Approved
                };
                expensesDTO.Add(expenseDTO);
            }
           
            return CreatedAtAction("GetExpensesDTO", expensesDTO);
        }

        [HttpDelete]
        [Route("api/Expenses")]
        public async Task<ActionResult<List<Expense>>> Delete([FromBody] List<Expense> expenses)
        {
            try
            {
                _context.RemoveRange(expenses);
                await _context.SaveChangesAsync();
               

            } catch (Exception error)
            {
                return BadRequest(error.Message);
            }

            return Ok("Expenses got deleted!");
        }

        private bool ExpenseExists(string id)
        {
            return _context.Expenses.Any(e => e.UUID == id);
        }
    }
}