using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class AccountController : Controller
{
    private static readonly string[] SelfServiceRoles = ["Customer", "DeliveryDriver", "StoreOwner"];
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Roles = new List<SelectListItem>
        {
            new("Customer", "Customer"),
            new("DeliveryDriver", "DeliveryDriver"),
            new("StoreOwner", "StoreOwner")
        };
        ViewBag.StoreCategories = new SelectList(_context.StoreCategories.Where(sc => sc.IsActive), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new("Customer", "Customer"),
                new("DeliveryDriver", "DeliveryDriver"),
                new("StoreOwner", "StoreOwner")
            };
            ViewBag.StoreCategories = new SelectList(_context.StoreCategories.Where(sc => sc.IsActive), "Id", "Name", model.StoreCategoryId);
            return View(model);
        }

        if (!SelfServiceRoles.Contains(model.Role, StringComparer.Ordinal))
        {
            ModelState.AddModelError(nameof(model.Role), "Selected role is not available for self-service registration.");
            ViewBag.Roles = SelfServiceRoles.Select(role => new SelectListItem(role, role)).ToList();
            ViewBag.StoreCategories = new SelectList(_context.StoreCategories.Where(sc => sc.IsActive), "Id", "Name", model.StoreCategoryId);
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.Phone
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ViewBag.Roles = new List<SelectListItem>
            {
                new("Customer", "Customer"),
                new("DeliveryDriver", "DeliveryDriver"),
                new("StoreOwner", "StoreOwner")
            };
            ViewBag.StoreCategories = new SelectList(_context.StoreCategories.Where(sc => sc.IsActive), "Id", "Name", model.StoreCategoryId);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, model.Role);

        if (model.Role == "Customer")
        {
            var customer = new Customer
            {
                UserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(model.StreetAddress))
            {
                var address = new Address
                {
                    CustomerId = customer.Id,
                    Street = model.StreetAddress,
                    City = model.City ?? "",
                    State = model.State ?? "",
                    ZipCode = model.ZipCode ?? "",
                    Country = model.Country ?? ""
                };
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
            }
        }
        else if (model.Role == "DeliveryDriver")
        {
            var driver = new DeliveryDriver
            {
                UserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                IsAvailable = true
            };
            _context.DeliveryDrivers.Add(driver);
            await _context.SaveChangesAsync();
        }
        else if (model.Role == "StoreOwner")
        {
            var store = new Store
            {
                UserId = user.Id,
                CategoryId = model.StoreCategoryId ?? 1,
                Name = model.StoreName ?? model.FirstName + "'s Store",
                Description = model.StoreDescription,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.StreetAddress ?? ""
            };
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        TempData["Success"] = "Account created successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            TempData["Success"] = "Welcome back!";
            return RedirectToLocal(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }
}
