using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Models;
using PROG6212_POE.Data;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PROG6212_POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LecturerController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Role check helper
        private IActionResult CheckRole()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string role = HttpContext.Session.GetString("UserRole");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (role != "Lecturer")
                return RedirectToAction("AccessDenied", "Account");

            return null; // role valid
        }

        // Lecturer Dashboard
        public async Task<IActionResult> LecturerDashboard()
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            int userId = HttpContext.Session.GetInt32("UserID").Value;

            var claims = await _context.Claims
                .Where(c => c.UserID == userId)
                .Include(c => c.Attachments)
                .Include(c => c.User)
                .ToListAsync();

            return View(claims);
        }

        // Show New Claim form
        public async Task<IActionResult> NewClaim()
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            int userId = HttpContext.Session.GetInt32("UserID").Value;
            var user = await _context.Users.FindAsync(userId);

            var claim = new Claim
            {
                UserID = user.UserID,
                HourlyRate = user.HourlyRate
            };

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewClaim(Claim claim, IFormFile? FileUpload)
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            int userId = HttpContext.Session.GetInt32("UserID").Value;
            var user = await _context.Users.FindAsync(userId);

            claim.UserID = user.UserID;
            claim.HourlyRate = user.HourlyRate;
            claim.Status = ClaimStatus.Pending;
            claim.DateSubmitted = DateTime.Now;

            if (claim.HoursWorked > 180)
            {
                ModelState.AddModelError("HoursWorked", "You cannot submit more than 180 hours.");
                return View(claim);
            }

            claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;

            claim.Attachments ??= new List<ClaimAttachment>();

            if (FileUpload != null && FileUpload.Length > 0)
            {
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };
                var ext = Path.GetExtension(FileUpload.FileName).ToLower();
                if (!allowed.Contains(ext))
                {
                    TempData["Error"] = "Only PDF, DOCX, and XLSX files are allowed.";
                    return View(claim);
                }

                var uploadDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadDir);

                var uniqueName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploadDir, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await FileUpload.CopyToAsync(stream);

                claim.Attachments.Add(new ClaimAttachment
                {
                    FileName = FileUpload.FileName,
                    FilePath = "/uploads/" + uniqueName
                });
            }

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction("LecturerDashboard");
        }
    }
}
