namespace ITI_GProject.Data.Models
{
    [Index(nameof(AttemptId))]
    [Index(nameof(QuestionId))]
    [Index(nameof(ChoiceId))]
    [Index(nameof(AttemptId), nameof(QuestionId), IsUnique = true)] 
    public class StudentResponse
    {
        [Key] public int Id { get; set; }

        [Required] public int AttemptId { get; set; }
        [ForeignKey(nameof(AttemptId))] public StudentAttempts Attempt { get; set; } = null!;

        [Required] public int QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))] public Question Question { get; set; } = null!;

        [Required] public int ChoiceId { get; set; }
        [ForeignKey(nameof(ChoiceId))] public Choice Choice { get; set; } = null!;

        [Required] public bool IsCorrect { get; set; }
    }
}
