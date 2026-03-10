using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Customerapp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }
        public IActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Apply(Account model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var customerId = int.Parse(userIdClaim.Value);

            var lastAccount = _dbContext.Accounts
                            .OrderByDescending(a => a.AccountId)
                            .FirstOrDefault();

            int nextNumber = lastAccount == null
                ? 100001
                : lastAccount.AccountId + 100001;

            var accountNumber = "ACC" + nextNumber;

            var account = new Account
            {
                AccountNumber = accountNumber,
                AccountType = model.AccountType,
                Balance = 0,
                CreatedAt = DateTime.Now,
                CustomerId = customerId
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("MyAccounts");
        }

        public IActionResult MyAccounts()
        {
            var customerId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier).Value
            );

            var accounts = _dbContext.Accounts
                .Where(a => a.CustomerId == customerId)
                .ToList();

            return View(accounts);
        }

        public IActionResult Details(int id)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier).Value
            );

            var account = _dbContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefault(a => a.AccountId == id
                                  && a.CustomerId == userId);

            if (account == null)
                return Unauthorized();

            return View(account);
        }
    }
}
