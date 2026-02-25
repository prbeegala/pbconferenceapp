using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceApp.Data;
using ConferenceApp.Models;

namespace ConferenceApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var upcomingSessions = await _context.Sessions
            .Include(s => s.Registrations)
            .Where(s => s.SessionDate >= DateTime.UtcNow)
            .OrderBy(s => s.SessionDate)
            .Take(6)
            .ToListAsync();

        var totalSessions = await _context.Sessions.CountAsync();
        var totalRegistrations = await _context.Registrations.CountAsync();

        ViewBag.UpcomingSessions = upcomingSessions;
        ViewBag.TotalSessions = totalSessions;
        ViewBag.TotalRegistrations = totalRegistrations;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
