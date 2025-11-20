using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Data;
using PROG6212_POE.Models;

namespace PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                TempData["Error"] = "ERROR 100: Invalid form submission. " + errors;
                return View(model);
            }

            try
            {
                // Step 1: Check if the email exists
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    TempData["Error"] = "ERROR 101: Email not found in the database.";
                    return View(model);
                }

                // Step 2: Compare password (plain text for now)
                if (user.Password != model.Password)
                {
                    TempData["Error"] = $"ERROR 102: Password does not match. Entered: '{model.Password}', Stored: '{user.Password}'";
                    return View(model);
                }

                // Step 3: Set session variables
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());

                // Step 4: Redirect based on role
                switch (user.Role)
                {
                    case UserRole.Lecturer:
                        return RedirectToAction("LecturerDashboard", "Lecturer");

                    case UserRole.Coordinator:
                        return RedirectToAction("CoordinatorDashboard", "Coordinator");

                    case UserRole.Manager:
                        return RedirectToAction("ManagerDashboard", "Manager");

                    default:
                        TempData["Error"] = "ERROR 103: Unknown user role.";
                        return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                // Step 5: Log exception and show error
                TempData["Error"] = $"ERROR 500: Unexpected exception: {ex.Message}";
                return View(model);
            }
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
