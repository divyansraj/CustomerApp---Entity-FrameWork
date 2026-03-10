using Customerapp.Models;
using Customerapp.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;

namespace Customerapp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerController(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("CustomerDetails");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Customer customer)
        {
            if(_dbContext.Customers.Any(c => c.Email == customer.Email))
            {
                ModelState.AddModelError("Email", "User with this email already exists");
            }

            //if (!ModelState.IsValid)
            //{
            //    return View(customer);
            //}
            customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
            await SignInUser(customer);

            return RedirectToAction("CustomerDetails");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("CustomerDetails");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if(!ModelState.IsValid)
            {
                return View(login);
            }
            var customer = _dbContext.Customers.FirstOrDefault(c => c.Email == login.Email);
            
            if (customer == null || !BCrypt.Net.BCrypt.Verify(login.Password, customer.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(login);
            }
            await SignInUser(customer);

            return RedirectToAction("CustomerDetails");
        }

        [Authorize]
        public IActionResult CustomerDetails()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var customer = _dbContext.Customers
                            .FirstOrDefault(c => c.CustomerId == userId);

            if (customer == null)
                return NotFound();
            var customerDetails = new CustomerDetailsDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
            return View("CustomerDetails",customerDetails);
        }

        [Authorize]
        public IActionResult Edit()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var customer = _dbContext.Customers
                            .FirstOrDefault(c => c.CustomerId == userId);
            if (customer == null)
                return NotFound();
            var editDto = new EditCustomerDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Address = customer.Address
            };

            return View(editDto);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(EditCustomerDTO updatedCustomer)
        {
            if (!ModelState.IsValid)
                return View(updatedCustomer);

            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            var customer = _dbContext.Customers
                            .FirstOrDefault(c => c.CustomerId == userId);

            if (customer == null)
                return NotFound();

            //update customer details
            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;
            customer.Phone = updatedCustomer.Phone;
            customer.Address = updatedCustomer.Address;
            _dbContext.SaveChanges();

            return RedirectToAction("CustomerDetails");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            var customer = _dbContext.Customers
                            .FirstOrDefault(c => c.CustomerId == userId);

            if (customer == null)
                return NotFound();

            _dbContext.Customers.Remove(customer);
            _dbContext.SaveChanges();

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }


        private async Task SignInUser(Customer customer)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                new Claim(ClaimTypes.Name, customer.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
