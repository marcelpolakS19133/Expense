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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ApiUser")]
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
        public ActionResult<IEnumerable<AccountDTO>> Get(bool withExpenses)
        {
            var loggedInUser = User.FindFirst("id").Value;
            return Ok(db.GetAccounts(withExpenses, loggedInUser));
        }

        [HttpGet("{id}")]
        public ActionResult<AccountDTO> GetById(string id, bool withExpenses)
        {
            if (db.IsAccountOwner(User.FindFirst("id").Value, id))
            {
                return Ok(db.GetAccountById(id, withExpenses));
            }
            else
            {
                return new ForbidResult();
            }
        }

        [HttpPost]
        public ActionResult<AccountDTO> Post(AccountDTO account)
        {
            var loggedInUser = User.FindFirst("id").Value;
            var created = db.CreateAccount(account, loggedInUser);
            return Created($"accounts/{created.Id}", created);
        }

        [HttpPut("{id}")]
        public ActionResult<AccountDTO> Modify(string id, AccountDTO account)
        {
            if (db.IsAccountOwner(User.FindFirst("id").Value, id))
            {
                account.Id = id;
                var modified = db.ModifyAccount(account);
                return modified == null ? StatusCode((int)HttpStatusCode.NotModified) : Ok(modified);
            }
            else
            {
                return new ForbidResult();
            }
            
        }

        [HttpDelete("{id}")]
        public ActionResult<AccountDTO> Delete(string id)
        {
            if (db.IsAccountOwner(User.FindFirst("id").Value, id))
            {

                return Ok(db.DeleteAccount(id));
            }
            else
            {
                return new ForbidResult();
            }
        }

        [HttpGet("{accountId}/expenses")]
        public ActionResult<IEnumerable<ExpenseDTO>> GetExpenses(string accountId)
        {
            if (db.IsAccountOwner(User.FindFirst("id").Value, accountId))
            {

                return Ok(db.GetExpenses(accountId));
            }
            else
            {
                return new ForbidResult();
            }
        }

        [HttpPost("{accountId}/expenses")]
        public ActionResult<ExpenseDTO> AddExpense(string accountId, ExpenseDTO expense)
        {
            if (db.IsAccountOwner(User.FindFirst("id").Value, accountId))
            {
                var created = db.AddExpense(accountId, expense);
                return Created($"accounts/{accountId}/expenses/{created.Id}", created);
            }
            else
            {
                return new ForbidResult();
            }
            
        }

        [HttpPut("{accountId}/expenses/{expenseId}")]
        public ActionResult<ExpenseDTO> ModifyExpense(string accountId, string expenseId, ExpenseDTO expense)
        {
            if (db.IsAccountOwner(User.FindFirst("id").Value, accountId))
            {
                expense.Id = expenseId;
                var modified = db.ModifyExpense(accountId, expense);
                return Ok(modified);
            }
            else
            {
                return new ForbidResult();
            }
            
        }

        [HttpDelete("{accountId}/expenses/{expenseId}")]
        public ActionResult<ExpenseDTO> DeleteExpense(string accountId, string expenseId)
        {
            if (db.IsAccountOwner(User.FindFirst("id").Value, accountId))
            {
                return Ok(db.DeleteExpense(accountId, expenseId));
            }
            else
            {
                return new ForbidResult();
            }
        }
    }
}
