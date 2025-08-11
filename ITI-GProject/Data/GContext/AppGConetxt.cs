

namespace ITI_GProject.Data.GContext
{
    public class AppGConetxt : IdentityDbContext<ApplicationUser>

    {
        public AppGConetxt(DbContextOptions<AppGConetxt> options):base(options) 
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

        //Entities Configurations 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<StudentCourse>()
                   .HasOne(sc => sc.Course)
                   .WithMany(s => s.StudentCourses)
                   .HasForeignKey(sc => sc.Course_Id);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(a => a.StudentCourses)
                .HasForeignKey(sc => sc.Student_Id);

            // Lesson -> Course
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId);

            // Assessment -> Lesson
            modelBuilder.Entity<Assessments>()
                .HasOne(q => q.Lesson)
                .WithMany(l => l.Quizzes)
                .HasForeignKey(q => q.LessonId);

            // StudentAttempts -> Student
            modelBuilder.Entity<StudentAttempts>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentQuizzes)
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // StudentAttempts -> Assessment
            modelBuilder.Entity<StudentAttempts>()
                .HasOne(sa => sa.Assessment)
                .WithMany(a => a.StudentAttempts)
                .HasForeignKey(sa => sa.AssessmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Questions -> Assessment
            modelBuilder.Entity<Question>()
                .HasOne(q => q.assessment)
                .WithMany(a => a.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.NoAction);

            // Choices -> Question
            modelBuilder.Entity<Choice>()
                .HasOne(c => c.question)
                .WithMany(q => q.choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            // StudentResponse -> Question
            modelBuilder.Entity<StudentResponse>()
                .HasOne(sr => sr.Question)
                .WithMany(q => q.StudentResponses)
                .HasForeignKey(sr => sr.QuestionId)
                .OnDelete(DeleteBehavior.NoAction); 

            // StudentResponse -> StudentAttempts
            modelBuilder.Entity<StudentResponse>()
                .HasOne(sr => sr.StudentAttempt)
                .WithMany(sa => sa.StudentResponses)
                .HasForeignKey(sr => sr.AttemptId)
                .OnDelete(DeleteBehavior.NoAction);
            //////////////////////////////////////
            ///



            modelBuilder.Entity<Assessments>()
                .HasMany(a => a.Questions)
                .WithOne(q => q.assessment)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.choices)
                .WithOne(c => c.question)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.StudentProfile)
            .WithOne(s => s.User)
            .HasForeignKey<Student>(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);

        }



    }
}
