using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ConferenceApp.Models;

namespace ConferenceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<SessionSubmission> SessionSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Registration entity
            builder.Entity<Registration>()
                .HasIndex(r => new { r.SessionId, r.UserId })
                .IsUnique();

            builder.Entity<Registration>()
                .HasOne(r => r.Session)
                .WithMany(s => s.Registrations)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure SessionSubmission entity
            builder.Entity<SessionSubmission>()
                .HasOne(s => s.Submitter)
                .WithMany(u => u.SessionSubmissions)
                .HasForeignKey(s => s.SubmitterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SessionSubmission>()
                .HasOne(s => s.Reviewer)
                .WithMany()
                .HasForeignKey(s => s.ReviewerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed initial sessions
            SeedSessions(builder);
        }

        private void SeedSessions(ModelBuilder builder)
        {
            var baseDate = new DateTime(2026, 6, 15, 9, 0, 0, DateTimeKind.Utc);

            builder.Entity<Session>().HasData(
                new Session
                {
                    Id = 1,
                    Title = "Getting Started with .NET 8",
                    Description = "Learn the fundamentals of .NET 8 and explore new features and improvements.",
                    SpeakerName = "Sarah Johnson",
                    SpeakerBio = "Senior Software Engineer at Microsoft with 8 years of experience in .NET development.",
                    Technology = ".NET",
                    SessionDate = baseDate,
                    DurationMinutes = 60,
                    Room = "Room A",
                    MaxAttendees = 100,
                    Level = SessionLevel.Beginner,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 2,
                    Title = "Advanced React Patterns and Performance",
                    Description = "Dive deep into advanced React patterns, hooks, and performance optimization techniques.",
                    SpeakerName = "Mark Thompson",
                    SpeakerBio = "Frontend Architect with expertise in React, performance optimization, and modern JavaScript.",
                    Technology = "React",
                    SessionDate = baseDate.AddHours(1.5),
                    DurationMinutes = 90,
                    Room = "Room B",
                    MaxAttendees = 75,
                    Level = SessionLevel.Advanced,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 3,
                    Title = "Microservices Architecture with Azure",
                    Description = "Design and implement scalable microservices using Azure Container Apps and Service Bus.",
                    SpeakerName = "Emily Chen",
                    SpeakerBio = "Cloud Solutions Architect specializing in Azure and microservices architecture.",
                    Technology = "Azure",
                    SessionDate = baseDate.AddHours(3),
                    DurationMinutes = 75,
                    Room = "Room C",
                    MaxAttendees = 60,
                    Level = SessionLevel.Intermediate,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 4,
                    Title = "Machine Learning with Python and TensorFlow",
                    Description = "Introduction to machine learning concepts and hands-on implementation with Python and TensorFlow.",
                    SpeakerName = "Dr. Robert Kim",
                    SpeakerBio = "Data Scientist and ML Engineer with PhD in Computer Science, 10+ years in ML research.",
                    Technology = "Python",
                    SessionDate = baseDate.AddDays(1),
                    DurationMinutes = 120,
                    Room = "Room D",
                    MaxAttendees = 40,
                    Level = SessionLevel.Intermediate,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 5,
                    Title = "DevOps Best Practices with GitHub Actions",
                    Description = "Learn how to implement CI/CD pipelines, automated testing, and deployment strategies using GitHub Actions.",
                    SpeakerName = "Alex Rodriguez",
                    SpeakerBio = "DevOps Engineer with experience in CI/CD, infrastructure as code, and cloud platforms.",
                    Technology = "DevOps",
                    SessionDate = baseDate.AddDays(1).AddHours(2),
                    DurationMinutes = 60,
                    Room = "Room A",
                    MaxAttendees = 80,
                    Level = SessionLevel.Intermediate,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 6,
                    Title = "Kubernetes Fundamentals and Container Orchestration",
                    Description = "Understanding Kubernetes concepts, deployment strategies, and container orchestration best practices.",
                    SpeakerName = "Lisa Park",
                    SpeakerBio = "Platform Engineer with extensive experience in Kubernetes, Docker, and cloud-native technologies.",
                    Technology = "Kubernetes",
                    SessionDate = baseDate.AddDays(1).AddHours(4),
                    DurationMinutes = 90,
                    Room = "Room B",
                    MaxAttendees = 50,
                    Level = SessionLevel.Advanced,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 7,
                    Title = "Modern JavaScript and TypeScript Development",
                    Description = "Explore modern JavaScript features, TypeScript benefits, and advanced development patterns.",
                    SpeakerName = "Kevin Walsh",
                    SpeakerBio = "Full-stack developer with expertise in TypeScript, Node.js, and modern frontend frameworks.",
                    Technology = "TypeScript",
                    SessionDate = baseDate.AddDays(2),
                    DurationMinutes = 75,
                    Room = "Room C",
                    MaxAttendees = 90,
                    Level = SessionLevel.Intermediate,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 8,
                    Title = "Cybersecurity in Cloud Applications",
                    Description = "Security best practices, threat modeling, and implementing security controls in cloud applications.",
                    SpeakerName = "Jennifer Martinez",
                    SpeakerBio = "Cybersecurity Consultant with 12+ years experience in application security and cloud security.",
                    Technology = "Security",
                    SessionDate = baseDate.AddDays(2).AddHours(2),
                    DurationMinutes = 60,
                    Room = "Room D",
                    MaxAttendees = 70,
                    Level = SessionLevel.Advanced,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 9,
                    Title = "Building Scalable APIs with GraphQL",
                    Description = "Learn GraphQL fundamentals, schema design, and how to build efficient, scalable APIs.",
                    SpeakerName = "Michael Brown",
                    SpeakerBio = "Backend Developer and API architect with experience in GraphQL, REST, and distributed systems.",
                    Technology = "GraphQL",
                    SessionDate = baseDate.AddDays(2).AddHours(4),
                    DurationMinutes = 90,
                    Room = "Room A",
                    MaxAttendees = 65,
                    Level = SessionLevel.Intermediate,
                    CreatedDate = DateTime.UtcNow
                },
                new Session
                {
                    Id = 10,
                    Title = "AI-Powered Development with GitHub Copilot",
                    Description = "Discover how AI pair programming tools can enhance productivity and code quality in modern development workflows.",
                    SpeakerName = "Amanda Davis",
                    SpeakerBio = "Developer Advocate focusing on AI tools, developer productivity, and modern development practices.",
                    Technology = "AI",
                    SessionDate = baseDate.AddDays(3),
                    DurationMinutes = 45,
                    Room = "Room B",
                    MaxAttendees = 120,
                    Level = SessionLevel.Beginner,
                    CreatedDate = DateTime.UtcNow
                }
            );
        }
    }
}