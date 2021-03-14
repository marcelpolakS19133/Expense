using Expense.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.Services
{
    public interface IExpensesDbService
    {
        #region accounts
        public IEnumerable<AccountDTO> GetAccounts(bool withExpenses, string loggedInUser);
        public AccountDTO GetAccountById(string id, bool withExpenses);
        public AccountDTO CreateAccount(AccountDTO accountDTO, string loggedInUser);
        public AccountDTO ModifyAccount(AccountDTO accountDTO);
        public AccountDTO DeleteAccount(string id);
        #endregion

        #region expenses
        public IEnumerable<ExpenseDTO> GetExpenses(string accountId);
        public ExpenseDTO AddExpense(string accountId, ExpenseDTO expenseDTO);
        public ExpenseDTO ModifyExpense(string accountId, ExpenseDTO expenseDTO);
        public ExpenseDTO DeleteExpense(string accountId, string expenseId);
        #endregion

        #region helpers
        public bool IsAccountOwner(string ownerId, string accountId);
        #endregion
    }
}
