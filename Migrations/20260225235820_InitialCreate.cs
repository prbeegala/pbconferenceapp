using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConferenceApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    SpeakerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SpeakerBio = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Technology = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SessionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Room = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MaxAttendees = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    SpeakerBio = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Technology = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PreferredDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubmitterId = table.Column<string>(type: "TEXT", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewComments = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ReviewerId = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionSubmissions_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SessionSubmissions_AspNetUsers_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SpecialRequirements = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AttendanceConfirmed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "CreatedDate", "Description", "DurationMinutes", "Level", "MaxAttendees", "Room", "SessionDate", "SpeakerBio", "SpeakerName", "Technology", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9436), "Learn the fundamentals of .NET 8 and explore new features and improvements.", 60, 0, 100, "Room A", new DateTime(2026, 6, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Senior Software Engineer at Microsoft with 8 years of experience in .NET development.", "Sarah Johnson", ".NET", "Getting Started with .NET 8" },
                    { 2, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9449), "Dive deep into advanced React patterns, hooks, and performance optimization techniques.", 90, 2, 75, "Room B", new DateTime(2026, 6, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Frontend Architect with expertise in React, performance optimization, and modern JavaScript.", "Mark Thompson", "React", "Advanced React Patterns and Performance" },
                    { 3, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9452), "Design and implement scalable microservices using Azure Container Apps and Service Bus.", 75, 1, 60, "Room C", new DateTime(2026, 6, 15, 12, 0, 0, 0, DateTimeKind.Utc), "Cloud Solutions Architect specializing in Azure and microservices architecture.", "Emily Chen", "Azure", "Microservices Architecture with Azure" },
                    { 4, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9456), "Introduction to machine learning concepts and hands-on implementation with Python and TensorFlow.", 120, 1, 40, "Room D", new DateTime(2026, 6, 16, 9, 0, 0, 0, DateTimeKind.Utc), "Data Scientist and ML Engineer with PhD in Computer Science, 10+ years in ML research.", "Dr. Robert Kim", "Python", "Machine Learning with Python and TensorFlow" },
                    { 5, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9459), "Learn how to implement CI/CD pipelines, automated testing, and deployment strategies using GitHub Actions.", 60, 1, 80, "Room A", new DateTime(2026, 6, 16, 11, 0, 0, 0, DateTimeKind.Utc), "DevOps Engineer with experience in CI/CD, infrastructure as code, and cloud platforms.", "Alex Rodriguez", "DevOps", "DevOps Best Practices with GitHub Actions" },
                    { 6, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9461), "Understanding Kubernetes concepts, deployment strategies, and container orchestration best practices.", 90, 2, 50, "Room B", new DateTime(2026, 6, 16, 13, 0, 0, 0, DateTimeKind.Utc), "Platform Engineer with extensive experience in Kubernetes, Docker, and cloud-native technologies.", "Lisa Park", "Kubernetes", "Kubernetes Fundamentals and Container Orchestration" },
                    { 7, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9464), "Explore modern JavaScript features, TypeScript benefits, and advanced development patterns.", 75, 1, 90, "Room C", new DateTime(2026, 6, 17, 9, 0, 0, 0, DateTimeKind.Utc), "Full-stack developer with expertise in TypeScript, Node.js, and modern frontend frameworks.", "Kevin Walsh", "TypeScript", "Modern JavaScript and TypeScript Development" },
                    { 8, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9466), "Security best practices, threat modeling, and implementing security controls in cloud applications.", 60, 2, 70, "Room D", new DateTime(2026, 6, 17, 11, 0, 0, 0, DateTimeKind.Utc), "Cybersecurity Consultant with 12+ years experience in application security and cloud security.", "Jennifer Martinez", "Security", "Cybersecurity in Cloud Applications" },
                    { 9, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9469), "Learn GraphQL fundamentals, schema design, and how to build efficient, scalable APIs.", 90, 1, 65, "Room A", new DateTime(2026, 6, 17, 13, 0, 0, 0, DateTimeKind.Utc), "Backend Developer and API architect with experience in GraphQL, REST, and distributed systems.", "Michael Brown", "GraphQL", "Building Scalable APIs with GraphQL" },
                    { 10, new DateTime(2026, 2, 25, 23, 58, 17, 477, DateTimeKind.Utc).AddTicks(9471), "Discover how AI pair programming tools can enhance productivity and code quality in modern development workflows.", 45, 0, 120, "Room B", new DateTime(2026, 6, 18, 9, 0, 0, 0, DateTimeKind.Utc), "Developer Advocate focusing on AI tools, developer productivity, and modern development practices.", "Amanda Davis", "AI", "AI-Powered Development with GitHub Copilot" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_SessionId_UserId",
                table: "Registrations",
                columns: new[] { "SessionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_UserId",
                table: "Registrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSubmissions_ReviewerId",
                table: "SessionSubmissions",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSubmissions_SubmitterId",
                table: "SessionSubmissions",
                column: "SubmitterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "SessionSubmissions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
