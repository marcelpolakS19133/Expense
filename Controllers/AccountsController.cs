using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Expense.Services;
using System.Net;

namespace Expense.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {

        private readonly IExpensesDbService db;

		public AccountsController(IExpensesDbService dbService)
        {
            db = dbService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Account>> Get(bool withExpenses)
        {
            foreach (var acc in db.GetAccounts(true))
            {
                Console.WriteLine(acc.Id);
            }
            return Ok(db.GetAccounts(withExpenses));
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetById(string id, bool withExpenses)
        {
            return Ok(db.GetAccountById(id, withExpenses));
        }

        [HttpPost]
        public ActionResult<Account> Post(Account account)
        {
            var created = db.CreateAccount(account);
            return Created($"accounts/{created.Id}", created);
        }

        [HttpPut("{id}")]
        public ActionResult<Account> Modify(string id, Account account)
        {
            account.Id = new ObjectId(id);
            var modified = db.ModifyAccount(account);
            return modified == null ? StatusCode((int)HttpStatusCode.NotModified) : Ok(modified);
        }

        [HttpDelete("{id}")]
        public ActionResult<Account> Delete(string id)
        {
            return Ok(db.DeleteAccount(id));
        }

        [HttpPost("{accountId}/expenses")]
        public ActionResult<Expense> AddExpense(string accountId, Expense expense)
		{
            var created = db.AddExpense(accountId, expense);
            return Created($"accounts/{accountId}/expenses/{created.Id}", created);
		}

        [HttpPut("{accountId}/expenses/{expenseId}")]
        public ActionResult<Expense> ModifyExpense(string accountId, string expenseId, Expense expense)
        {
            expense.Id = new ObjectId(expenseId);
            var modified = db.ModifyExpense(accountId, expense);
            return Ok(modified);
        }

        [HttpDelete("{accountId}/expenses/{expenseId}")]
        public ActionResult<Expense> DeleteExpense(string accountId, string expenseId)
        {
            return Ok(db.DeleteExpense(accountId, expenseId));
        }
    }
}
