

namespace ITI_GProject.Data.GContext
{
    public class AppGConetxt : IdentityDbContext<ApplicationUser>

    {
        public AppGConetxt(DbContextOptions<AppGConetxt> options) : base(options)
        {


        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Assessments> Assessments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentAttempts> StudentAttempts { get; set; }
        public DbSet<StudentResponse> StudentResponses { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<StudentLessonProgress> StudentLessonProgresses { get; set; }
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        // UpdatedAt 
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Course>())
                if (entry.State == EntityState.Modified)
                    entry.Entity.UpdatedAt = DateTime.UtcNow;

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentCourse>()
                           .HasOne(sc => sc.Course)
                           .WithMany(c => c.StudentCourses)
                           .HasForeignKey(sc => sc.Course_Id);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.Student_Id);

     
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<StudentLessonProgress>()
                .HasIndex(p => new { p.StudentId, p.LessonId })
                .IsUnique();

         
            modelBuilder.Entity<Assessments>()
                .HasOne(a => a.Lesson)
                .WithMany(l => l.Quizzes)
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

      
            modelBuilder.Entity<StudentAttempts>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentQuizzes)   
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentAttempts>()
                .HasOne(sa => sa.Assessment)
                .WithMany(a => a.StudentAttempts)
                .HasForeignKey(sa => sa.AssessmentId)
                .OnDelete(DeleteBehavior.NoAction);

     
            modelBuilder.Entity<Question>()
                .HasOne(q => q.assessment)
                .WithMany(a => a.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Choice>()
                .HasOne(c => c.question)
                .WithMany(q => q.choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

         

            modelBuilder.Entity<StudentResponse>()
                .HasOne(sr => sr.Question)
                .WithMany(q => q.StudentResponses)
                .HasForeignKey(sr => sr.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentResponse>()
                .HasOne(sr => sr.Attempt)
                .WithMany(a => a.StudentResponses)
                .HasForeignKey(sr => sr.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentResponse>()
                .HasOne(sr => sr.Choice)
                .WithMany()
                .HasForeignKey(sr => sr.ChoiceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentResponse>()
                .HasIndex(sr => new { sr.AttemptId, sr.QuestionId })
                .IsUnique();

            modelBuilder.Entity<StudentResponse>()
                .HasIndex(sr => sr.AttemptId);

            modelBuilder.Entity<StudentResponse>()
                .HasIndex(sr => sr.QuestionId);

            modelBuilder.Entity<StudentResponse>()
                .HasIndex(sr => sr.ChoiceId);

           
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.StudentProfile)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Invoices
            modelBuilder.Entity<Invoice>(b =>
            {
                b.HasOne(i => i.Student)
                 .WithMany()
                 .HasForeignKey(i => i.StudentId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(i => i.Course)
                 .WithMany()
                 .HasForeignKey(i => i.CourseId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.Property(i => i.Amount).HasColumnType("decimal(18,2)");
                b.Property(i => i.Currency).HasMaxLength(8).HasDefaultValue("EGP");

                b.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                b.Property(i => i.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                b.HasIndex(i => new { i.StudentId, i.Status, i.CreatedAt });

                b.HasIndex(i => i.InvoiceNo)
                 .IsUnique()
                 .HasFilter("[InvoiceNo] IS NOT NULL");
            });

            modelBuilder.Entity<Notification>(b =>
            {
                b.HasOne(n => n.Student)
                 .WithMany()
                 .HasForeignKey(n => n.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.Property(n => n.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                b.HasIndex(n => new { n.StudentId, n.IsRead, n.CreatedAt });

                b.Property(n => n.ActionUrl).HasMaxLength(1024);

            });

        }
    }
}
