using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ConferenceApp.Data;
using ConferenceApp.Models;

namespace ConferenceApp.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Sessions
        public async Task<IActionResult> Index(string? technology, string? searchString, SessionLevel? level)
        {
            var sessions = _context.Sessions.Include(s => s.Registrations).AsQueryable();

            // Filter by technology
            if (!string.IsNullOrEmpty(technology))
            {
                sessions = sessions.Where(s => s.Technology == technology);
            }

            // Filter by search string
            if (!string.IsNullOrEmpty(searchString))
            {
                sessions = sessions.Where(s => 
                    s.Title.Contains(searchString) || 
                    s.Description.Contains(searchString) ||
                    s.SpeakerName.Contains(searchString));
            }

            // Filter by level
            if (level.HasValue)
            {
                sessions = sessions.Where(s => s.Level == level.Value);
            }

            // Get distinct technologies for filter dropdown
            ViewBag.Technologies = await _context.Sessions
                .Select(s => s.Technology)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewData["CurrentFilter"] = searchString;
            ViewData["TechnologyFilter"] = technology;
            ViewData["LevelFilter"] = level;

            return View(await sessions.OrderBy(s => s.SessionDate).ToListAsync());
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Registrations)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (session == null)
            {
                return NotFound();
            }

            // Check if current user is registered
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    ViewBag.IsRegistered = session.Registrations.Any(r => r.UserId == user.Id);
                }
            }

            return View(session);
        }

        // POST: Sessions/Register/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int id, string? specialRequirements)
        {
            var session = await _context.Sessions
                .Include(s => s.Registrations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Check if session is full
            if (session.IsFull)
            {
                TempData["Error"] = "This session is already full.";
                return RedirectToAction(nameof(Details), new { id = session.Id });
            }

            // Check if user is already registered
            var existingRegistration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.SessionId == id && r.UserId == user.Id);

            if (existingRegistration != null)
            {
                TempData["Error"] = "You are already registered for this session.";
                return RedirectToAction(nameof(Details), new { id = session.Id });
            }

            // Create new registration
            var registration = new Registration
            {
                SessionId = id,
                UserId = user.Id,
                SpecialRequirements = specialRequirements,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Successfully registered for the session!";
            return RedirectToAction(nameof(Details), new { id = session.Id });
        }

        // POST: Sessions/Unregister/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unregister(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.SessionId == id && r.UserId == user.Id);

            if (registration == null)
            {
                TempData["Error"] = "You are not registered for this session.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Successfully unregistered from the session.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: Sessions/MyRegistrations
        [Authorize]
        public async Task<IActionResult> MyRegistrations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var registrations = await _context.Registrations
                .Include(r => r.Session)
                .Where(r => r.UserId == user.Id)
                .OrderBy(r => r.Session.SessionDate)
                .ToListAsync();

            return View(registrations);
        }

        // GET: Sessions/Submit
        [Authorize]
        public IActionResult Submit()
        {
            return View();
        }

        // POST: Sessions/Submit
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SessionSubmission submission)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                submission.SubmitterId = user.Id;
                submission.SubmissionDate = DateTime.UtcNow;
                submission.Status = SubmissionStatus.Pending;

                _context.SessionSubmissions.Add(submission);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Your session has been submitted for review!";
                return RedirectToAction(nameof(MySubmissions));
            }

            return View(submission);
        }

        // GET: Sessions/MySubmissions
        [Authorize]
        public async Task<IActionResult> MySubmissions()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var submissions = await _context.SessionSubmissions
                .Include(s => s.Reviewer)
                .Where(s => s.SubmitterId == user.Id)
                .OrderByDescending(s => s.SubmissionDate)
                .ToListAsync();

            return View(submissions);
        }
    }
}