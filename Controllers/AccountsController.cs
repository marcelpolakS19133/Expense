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
using Expense.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Expense.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(Policy = "ApiUser")]
        public ActionResult<IEnumerable<AccountDTO>> Get(bool withExpenses)
        {
            return Ok(db.GetAccounts(withExpenses));
        }

        [HttpGet("{id}")]
        public ActionResult<AccountDTO> GetById(string id, bool withExpenses)
        {
            return Ok(db.GetAccountById(id, withExpenses));
        }

        [HttpPost]
        public ActionResult<AccountDTO> Post(AccountDTO account)
        {
            var created = db.CreateAccount(account);
            return Created($"accounts/{created.Id}", created);
        }

        [HttpPut("{id}")]
        public ActionResult<AccountDTO> Modify(string id, AccountDTO account)
        {
            account.Id = id;
            var modified = db.ModifyAccount(account);
            return modified == null ? StatusCode((int)HttpStatusCode.NotModified) : Ok(modified);
        }

        [HttpDelete("{id}")]
        public ActionResult<AccountDTO> Delete(string id)
        {
            return Ok(db.DeleteAccount(id));
        }

        [HttpGet("{accountId}/expenses")]
        public ActionResult<IEnumerable<ExpenseDTO>> GetExpenses(string accountId)
        {
            return Ok(db.GetExpenses(accountId));
        }

        [HttpPost("{accountId}/expenses")]
        public ActionResult<ExpenseDTO> AddExpense(string accountId, ExpenseDTO expense)
        {
            var created = db.AddExpense(accountId, expense);
            return Created($"accounts/{accountId}/expenses/{created.Id}", created);
        }

        [HttpPut("{accountId}/expenses/{expenseId}")]
        public ActionResult<ExpenseDTO> ModifyExpense(string accountId, string expenseId, ExpenseDTO expense)
        {
            expense.Id = expenseId;
            var modified = db.ModifyExpense(accountId, expense);
            return Ok(modified);
        }

        [HttpDelete("{accountId}/expenses/{expenseId}")]
        public ActionResult<ExpenseDTO> DeleteExpense(string accountId, string expenseId)
        {
            return Ok(db.DeleteExpense(accountId, expenseId));
        }
    }
}
