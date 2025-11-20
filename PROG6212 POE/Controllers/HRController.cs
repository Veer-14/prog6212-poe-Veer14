using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;


namespace PROG6212_POE.Controllers
{
    public class HRController : Controller
    {
        private readonly AppDbContext _context;

        public HRController(AppDbContext context)
        {
            _context = context;
        }

        // HR Dashboard - list all users
        public async Task<IActionResult> HRDashboard()
        {
            var users = await _context.Users.ToListAsync();
            return View(users); // Make sure the view name is HRDashboard.cshtml
        }

        // Add new user
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(User user)
        {
            if (!ModelState.IsValid) return View(user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["Message"] = "User added successfully!";
            return RedirectToAction("HRDashboard");
        }

        // Edit user
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(User user)
        {
            if (!ModelState.IsValid) return View(user);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            TempData["Message"] = "User updated successfully!";
            return RedirectToAction("HRDashboard");
        }

        // Generate Report (example: list of lecturers)
        public async Task<IActionResult> GenerateReport()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Approved)
                .Include(c => c.User)
                .OrderBy(c => c.User.LastName)
                .ToListAsync();

            // Build PDF document
            var pdfBytes = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);

                    page.Header().Text("Approved Claims Report")
                        .FontSize(24)
                        .Bold()
                        .AlignCenter()
                        .FontColor(Colors.Blue.Medium);

                    page.Content().Table(table =>
                    {
                        // Columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        // Header row
                        table.Header(header =>
                        {
                            header.Cell().Text("Lecturer").Bold();
                            header.Cell().Text("Email").Bold();
                            header.Cell().Text("Month").Bold();
                            header.Cell().Text("Hours Worked").Bold();
                            header.Cell().Text("Total Amount").Bold();
                        });

                        // Data rows
                        foreach (var claim in approvedClaims)
                        {
                            table.Cell().Text($"{claim.User.FirstName} {claim.User.LastName}");
                            table.Cell().Text(claim.User.Email);
                            table.Cell().Text(claim.ClaimMonth);
                            table.Cell().Text(claim.HoursWorked.ToString());
                            table.Cell().Text("R " + claim.TotalAmount.ToString("0.00"));
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "ApprovedClaimsReport.pdf");
        }


    }
}
