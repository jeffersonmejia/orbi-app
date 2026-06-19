using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.Security;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class AccountController : Controller
{
    private static readonly string[] SelfServiceRoles = ["Customer", "DeliveryDriver", "StoreOwner"];
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ActiveSessionStore _activeSessions;
    private readonly AppDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ActiveSessionStore activeSessions,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _activeSessions = activeSessions;
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        PopulateRegisterLookups();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateRegisterLookups(model.StoreCategoryId);
            return View(model);
        }

        if (!SelfServiceRoles.Contains(model.Role, StringComparer.Ordinal))
        {
            ModelState.AddModelError(nameof(model.Role), "Selected role is not available for self-service registration.");
            PopulateRegisterLookups(model.StoreCategoryId);
            return View(model);
        }

        if (await _userManager.FindByEmailAsync(model.Email) is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");
            PopulateRegisterLookups(model.StoreCategoryId);
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.Phone
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);

            PopulateRegisterLookups(model.StoreCategoryId);
            return View(model);
        }

        try
        {
            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                AddIdentityErrors(roleResult);
                PopulateRegisterLookups(model.StoreCategoryId);
                return View(model);
            }

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
        }
        catch (Exception)
        {
            await _userManager.DeleteAsync(user);
            ModelState.AddModelError(nameof(RegisterViewModel.Email), "The account could not be completed. Please review the form and try again.");
            PopulateRegisterLookups(model.StoreCategoryId);
            return View(model);
        }

        TempData["Success"] = "Account created successfully.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (_signInManager.IsSignedIn(User))
        {
            TempData["Error"] = "Ya tienes una sesion activa.";
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (_signInManager.IsSignedIn(User))
        {
            ModelState.AddModelError(string.Empty, "Ya tienes una sesion activa.");
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            if (_activeSessions.IsActive(user.Id))
            {
                ModelState.AddModelError(string.Empty, "Ya tienes una sesion activa.");
                return View(model);
            }

            await _signInManager.SignInAsync(user, model.RememberMe);
            _activeSessions.MarkActive(user.Id);
            TempData["Success"] = "Welcome back!";
            return RedirectToLocal(returnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account is temporarily locked. Try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userId = _userManager.GetUserId(User);
        await _signInManager.SignOutAsync();

        if (userId is not null)
        {
            _activeSessions.Clear(userId);
        }

        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    private void PopulateRegisterLookups(int? selectedStoreCategoryId = null)
    {
        ViewBag.Roles = SelfServiceRoles.Select(role => new SelectListItem(role, role)).ToList();
        ViewBag.StoreCategories = new SelectList(
            _context.StoreCategories.Where(sc => sc.IsActive),
            "Id",
            "Name",
            selectedStoreCategoryId);
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            var field = error.Code.StartsWith("Password", StringComparison.Ordinal)
                ? nameof(RegisterViewModel.Password)
                : error.Code is "DuplicateEmail" or "InvalidEmail" or "DuplicateUserName" or "InvalidUserName"
                    ? nameof(RegisterViewModel.Email)
                    : error.Code.Contains("Role", StringComparison.OrdinalIgnoreCase)
                        ? nameof(RegisterViewModel.Role)
                        : nameof(RegisterViewModel.Email);

            ModelState.AddModelError(field, error.Description);
        }
    }
}
