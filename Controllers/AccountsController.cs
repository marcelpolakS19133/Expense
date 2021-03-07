using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Expense.Services;

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
        public IEnumerable<Account> Get(bool withExpenses)
        {
            return db.GetAccounts(withExpenses);
        }

        [HttpGet("{name}")]
        public Account GetByName(string name)
        {
            return db.GetAccountByName(name);
        }

        [HttpGet("{name}/expenses")]
        public IEnumerable<Expense> GetExpensesByAccountName(string name)
        {

            return db.GetExpensesForAccountName(name);
        }

        [HttpPost("{name}/add")]
        public IEnumerable<Expense> AddExpense(string name, Expense expense)
		{
            return db.AddExpense(name, expense);
		}
    }
}
