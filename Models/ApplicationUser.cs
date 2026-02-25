using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ConferenceApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public virtual ICollection<SessionSubmission> SessionSubmissions { get; set; } = new List<SessionSubmission>();
    }
}