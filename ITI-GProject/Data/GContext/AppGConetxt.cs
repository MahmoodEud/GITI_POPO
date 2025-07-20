using ITI_GProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI_GProject.Data.GContext
{
    public class AppGConetxt :DbContext

    {

        public AppGConetxt(DbContextOptions<AppGConetxt> options):base(options) 
        {
        
        
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentQuiz> StudentQuizzes { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }


        //Entities Configurations 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentQuiz>()
                .HasKey(sq => new { sq.Student_Id, sq.Quiz_Id });
            modelBuilder.Entity<StudentAssignment>()
                .HasKey(sa => new { sa.Studet_Id, sa.Assignemt_Id });
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.Student_Id, sc.Course_Id });

            // Configure relationships
            modelBuilder.Entity<StudentQuiz>()
                .HasOne(sq => sq.Student)
                .WithMany(s => s.StudentQuizzes)
                .HasForeignKey(sq => sq.Student_Id);

            modelBuilder.Entity<StudentQuiz>()
                .HasOne(sq => sq.Quiz)
                .WithMany(q => q.StudentQuizzes)
                .HasForeignKey(sq => sq.Quiz_Id);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAssignments)
                .HasForeignKey(sa => sa.Studet_Id);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Assignment)
                .WithMany(a => a.StudentAssignments)
                .HasForeignKey(sa => sa.Assignemt_Id);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAssignments)
                .HasForeignKey(sa => sa.Studet_Id);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.Course_Id);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(a => a.StudentCourses)
                .HasForeignKey(sc => sc.Student_Id);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Lesson)
                .WithMany(l => l.Quizzes)
                .HasForeignKey(q => q.LessonId);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Lesson)
                .WithMany(l => l.Assignments)
                .HasForeignKey(a => a.LessonId);

        }



    }
}
