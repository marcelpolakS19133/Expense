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
        public IEnumerable<AccountDTO> GetAccounts(bool withExpenses);
        public AccountDTO GetAccountById(string id, bool withExpenses);
        public AccountDTO CreateAccount(AccountDTO accountDTO);
        public AccountDTO ModifyAccount(AccountDTO accountDTO);
        public AccountDTO DeleteAccount(string id);
        #endregion

        #region expenses
        public ExpenseDTO AddExpense(string accountId, ExpenseDTO expenseDTO);
        public ExpenseDTO ModifyExpense(string accountId, ExpenseDTO expenseDTO);
        public ExpenseDTO DeleteExpense(string accountId, string expenseId);
        #endregion
    }
}
