public class StudentResponse
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "رقم المحاولة مطلوب")]
    public int AttemptId { get; set; }

    [ForeignKey(nameof(AttemptId))]
    public virtual StudentAttempts StudentAttempt { get; set; } = null!;

    [Required(ErrorMessage = "رقم السؤال مطلوب")]
    public int QuestionId { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public virtual Question Question { get; set; } = null!;

    [Required(ErrorMessage = "الاختيار مطلوب")]
    public int ChoiceId { get; set; }

    [ForeignKey(nameof(ChoiceId))]
    public virtual Choice SelectedChoice { get; set; } = null!;

    [Required]
    public bool IsCorrect { get; set; }
}
