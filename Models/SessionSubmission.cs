using System.ComponentModel.DataAnnotations;

namespace ConferenceApp.Models
{
    public class SessionSubmission
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

        [Display(Name = "Speaker Email")]
        [EmailAddress]
        [StringLength(150)]
        public string? SpeakerEmail { get; set; }

        [Display(Name = "Speaker Bio")]
        [StringLength(500)]
        public string SpeakerBio { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Technology/Category")]
        [StringLength(50)]
        public string Technology { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Preferred Duration (minutes)")]
        [Range(15, 480)]
        public int PreferredDurationMinutes { get; set; }

        [Display(Name = "Level")]
        public SessionLevel Level { get; set; } = SessionLevel.Beginner;

        [Display(Name = "Presentation Format")]
        public PresentationFormat Format { get; set; } = PresentationFormat.Talk;

        [Display(Name = "Room Preference")]
        [StringLength(50)]
        public string? RoomPreference { get; set; }

        [Display(Name = "Special Requirements")]
        [StringLength(300)]
        public string? SpecialRequirements { get; set; }

        [Display(Name = "Additional Notes")]
        [StringLength(500)]
        public string? AdditionalNotes { get; set; }

        [Required]
        public string SubmitterId { get; set; } = string.Empty;

        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Status")]
        public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;

        [Display(Name = "Review Comments")]
        [StringLength(1000)]
        public string? ReviewComments { get; set; }

        public string? ReviewerId { get; set; }

        public DateTime? ReviewDate { get; set; }

        // Navigation properties
        public virtual ApplicationUser Submitter { get; set; } = null!;
        public virtual ApplicationUser? Reviewer { get; set; }
    }

    public enum SubmissionStatus
    {
        Pending,
        Approved,
        Rejected,
        NeedsRevision
    }

    public enum PresentationFormat
    {
        Talk,
        Workshop,
        Panel,
        Demo,
        Lightning
    }
}