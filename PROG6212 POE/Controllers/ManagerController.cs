using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System.Threading.Tasks;
using System.Linq;

namespace PROG6212_POE.Controllers
{
    public class ManagerController : Controller
    {
        private readonly AppDbContext _context;

        public ManagerController(AppDbContext context)
        {
            _context = context;
        }

        private IActionResult CheckRole()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string role = HttpContext.Session.GetString("UserRole");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (role != "Manager")
                return RedirectToAction("AccessDenied", "Account");

            return null;
        }

        public async Task<IActionResult> ManagerDashboard()
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            var claims = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.Attachments)
                .Where(c => c.Status == ClaimStatus.Verified)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagerDashboard));
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagerDashboard));
        }
    }
}
