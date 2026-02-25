using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ConferenceApp.Data;
using ConferenceApp.Models;

namespace ConferenceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var pendingSubmissions = await _context.SessionSubmissions
                .Include(s => s.Submitter)
                .Where(s => s.Status == SubmissionStatus.Pending)
                .CountAsync();

            var totalSessions = await _context.Sessions.CountAsync();
            var totalRegistrations = await _context.Registrations.CountAsync();
            var totalSubmissions = await _context.SessionSubmissions.CountAsync();

            ViewBag.PendingSubmissions = pendingSubmissions;
            ViewBag.TotalSessions = totalSessions;
            ViewBag.TotalRegistrations = totalRegistrations;
            ViewBag.TotalSubmissions = totalSubmissions;

            return View();
        }

        // GET: Admin/Submissions
        public async Task<IActionResult> Submissions(SubmissionStatus? status)
        {
            var submissions = _context.SessionSubmissions
                .Include(s => s.Submitter)
                .Include(s => s.Reviewer)
                .AsQueryable();

            if (status.HasValue)
            {
                submissions = submissions.Where(s => s.Status == status.Value);
            }

            ViewData["StatusFilter"] = status;
            return View(await submissions.OrderByDescending(s => s.SubmissionDate).ToListAsync());
        }

        // GET: Admin/ReviewSubmission/5
        public async Task<IActionResult> ReviewSubmission(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.SessionSubmissions
                .Include(s => s.Submitter)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // POST: Admin/ApproveSubmission/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSubmission(int id, string? reviewComments, 
            DateTime sessionDate, string room, int maxAttendees)
        {
            var submission = await _context.SessionSubmissions
                .Include(s => s.Submitter)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);
            if (reviewer == null)
            {
                return Challenge();
            }

            // Update submission
            submission.Status = SubmissionStatus.Approved;
            submission.ReviewComments = reviewComments;
            submission.ReviewerId = reviewer.Id;
            submission.ReviewDate = DateTime.UtcNow;

            // Create session from approved submission
            var session = new Session
            {
                Title = submission.Title,
                Description = submission.Description,
                SpeakerName = submission.Submitter.FullName,
                SpeakerBio = submission.SpeakerBio,
                Technology = submission.Technology,
                SessionDate = sessionDate,
                DurationMinutes = submission.PreferredDurationMinutes,
                Room = room,
                MaxAttendees = maxAttendees,
                Level = submission.Level,
                CreatedDate = DateTime.UtcNow
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Submission approved and session created successfully!";
            return RedirectToAction(nameof(Submissions));
        }

        // POST: Admin/RejectSubmission/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectSubmission(int id, string reviewComments)
        {
            var submission = await _context.SessionSubmissions
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);
            if (reviewer == null)
            {
                return Challenge();
            }

            submission.Status = SubmissionStatus.Rejected;
            submission.ReviewComments = reviewComments;
            submission.ReviewerId = reviewer.Id;
            submission.ReviewDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Submission rejected successfully.";
            return RedirectToAction(nameof(Submissions));
        }

        // POST: Admin/RequestRevision/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestRevision(int id, string reviewComments)
        {
            var submission = await _context.SessionSubmissions
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);
            if (reviewer == null)
            {
                return Challenge();
            }

            submission.Status = SubmissionStatus.NeedsRevision;
            submission.ReviewComments = reviewComments;
            submission.ReviewerId = reviewer.Id;
            submission.ReviewDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Revision requested successfully.";
            return RedirectToAction(nameof(Submissions));
        }

        // GET: Admin/Sessions
        public async Task<IActionResult> Sessions()
        {
            var sessions = await _context.Sessions
                .Include(s => s.Registrations)
                .OrderBy(s => s.SessionDate)
                .ToListAsync();

            return View(sessions);
        }

        // GET: Admin/Registrations
        public async Task<IActionResult> Registrations(int? sessionId)
        {
            var registrations = _context.Registrations
                .Include(r => r.Session)
                .Include(r => r.User)
                .AsQueryable();

            if (sessionId.HasValue)
            {
                registrations = registrations.Where(r => r.SessionId == sessionId.Value);
            }

            ViewBag.Sessions = await _context.Sessions
                .OrderBy(s => s.SessionDate)
                .Select(s => new { s.Id, s.Title })
                .ToListAsync();

            ViewData["SessionFilter"] = sessionId;
            return View(await registrations.OrderBy(r => r.RegistrationDate).ToListAsync());
        }
    }
}