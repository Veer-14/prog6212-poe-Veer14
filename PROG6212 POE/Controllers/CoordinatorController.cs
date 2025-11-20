using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System.Threading.Tasks;
using System.Linq;

namespace PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly AppDbContext _context;

        public CoordinatorController(AppDbContext context)
        {
            _context = context;
        }

        private IActionResult CheckRole()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string role = HttpContext.Session.GetString("UserRole");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (role != "Coordinator")
                return RedirectToAction("AccessDenied", "Account");

            return null;
        }

        public async Task<IActionResult> CoordinatorDashboard()
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            var pendingClaims = await _context.Claims
                .Include(c => c.User)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(pendingClaims);
        }


       
        [HttpPost]
        public async Task<IActionResult> VerifyClaim(int id)
        {
            var roleCheck = CheckRole();
            if (roleCheck != null) return roleCheck;

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Verified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CoordinatorDashboard));
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

            return RedirectToAction(nameof(CoordinatorDashboard));
        }
    }
}
