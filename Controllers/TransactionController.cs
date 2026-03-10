using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Customerapp.Models;

namespace Customerapp.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Deposit(int accountId)
        {
            return View(new Transaction { AccountId = accountId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Deposit(Transaction model)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == model.AccountId);

            if (account == null)
                return NotFound();

            if (model.Amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than 0");
                return View(model);
            }

            account.Balance += model.Amount;

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = model.Amount,
                Type = "Deposit",
                TransactionDate = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return RedirectToAction("MyAccounts", "Account");
        }

        [Authorize]
        public IActionResult Withdraw(int accountId)
        {
            return View(new Transaction { AccountId = accountId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Withdraw(Transaction model)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == model.AccountId);

            if (account == null)
                return NotFound();

            if (model.Amount <= 0)
            {
                ModelState.AddModelError("", "Invalid amount");
                return View(model);
            }

            if (account.Balance < model.Amount)
            {
                ModelState.AddModelError("", "Insufficient balance");
                return View(model);
            }

            account.Balance -= model.Amount;

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = model.Amount,
                Type = "Withdraw",
                TransactionDate = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return RedirectToAction("MyAccounts", "Account");
        }

        [Authorize]
        public IActionResult History(int accountId)
        {
            var transactions = _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return View(transactions);
        }
    }
}