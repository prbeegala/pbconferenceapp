using System.ComponentModel.DataAnnotations;

namespace ConferenceApp.Models
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Session Title")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Description")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Speaker Name")]
        [StringLength(100)]
        public string SpeakerName { get; set; } = string.Empty;

        [Display(Name = "Speaker Bio")]
        [StringLength(500)]
        public string SpeakerBio { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Technology/Category")]
        [StringLength(50)]
        public string Technology { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Session Date")]
        [DataType(DataType.DateTime)]
        public DateTime SessionDate { get; set; }

        [Required]
        [Display(Name = "Duration (minutes)")]
        [Range(15, 480)]
        public int DurationMinutes { get; set; }

        [Required]
        [Display(Name = "Room")]
        [StringLength(50)]
        public string Room { get; set; } = string.Empty;

        [Display(Name = "Max Attendees")]
        [Range(1, 1000)]
        public int MaxAttendees { get; set; } = 50;

        [Display(Name = "Level")]
        public SessionLevel Level { get; set; } = SessionLevel.Beginner;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        [Display(Name = "Current Registrations")]
        public int CurrentRegistrationCount => Registrations?.Count ?? 0;

        [Display(Name = "Available Spots")]
        public int AvailableSpots => MaxAttendees - CurrentRegistrationCount;

        [Display(Name = "Is Full")]
        public bool IsFull => CurrentRegistrationCount >= MaxAttendees;
    }

    public enum SessionLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
}