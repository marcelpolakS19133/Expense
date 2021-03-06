﻿using Expense.DTO;
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
            var expenses = Array.Empty<Expense>();
            if (dto.Expenses != null)
            {
                expenses = new Expense[dto.Expenses.Count];
                int i = 0;
                foreach (var expenseDTO in dto.Expenses)
                {
                    expenses[i++] = expenseDTO.ToExpense();
                }
            }
            return new Account
            {
                Id = dto.Id != null && dto.Id.Length == 24 ? new ObjectId(dto.Id) : ObjectId.GenerateNewId(),
                Name = dto.Name,
                Expenses = expenses
            };
        }
        public static AccountDTO ToAccountDTO(this Account account)
        {
            List<ExpenseDTO> expenses = null;
            if (account.Expenses != null)
            {
                expenses = new List<ExpenseDTO>(account.Expenses.Length);
                foreach (var expense in account.Expenses)
                {
                    expenses.Add(expense.ToExpenseDTO());
                }
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
