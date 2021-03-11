using Expense.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Expense.DTOExtensions;

namespace Expense.Services
{
    public static class AccountDTOMapper
    {
        public static Account ToAccount(this AccountDTO dto)
        {
            var expenses = new Expense[dto.Expenses.Count];
            int i = 0;
            foreach (var expenseDTO in dto.Expenses)
            {
                expenses[i++]=expenseDTO.ToExpense();
            }
            return new Account
            {
                Id = new ObjectId(dto.Id),
                Name = dto.Name,
                Expenses = expenses
            };
        }
        public static AccountDTO ToAccountDTO(this Account account)
        {
            var expenses = new List<ExpenseDTO>(account.Expenses.Length);
            foreach (var expense in account.Expenses)
            {
                expenses.Add(expense.ToExpenseDTO());
            }
            return new AccountDTO
            {
                Id = account.Id.ToString(),
                Name = account.Name,
                Expenses = expenses
            };
            
        }
    }
}
