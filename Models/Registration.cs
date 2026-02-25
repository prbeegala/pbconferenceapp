using System.ComponentModel.DataAnnotations;

namespace ConferenceApp.Models
{
    public class Registration
    {
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Special Requirements")]
        [StringLength(500)]
        public string? SpecialRequirements { get; set; }

        [Display(Name = "Attendance Confirmed")]
        public bool AttendanceConfirmed { get; set; } = false;

        // Navigation properties
        public virtual Session Session { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}