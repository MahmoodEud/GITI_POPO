namespace ITI_GProject.Data.Models
{
        public enum InvoiceStatus { Draft = 0, Sent = 1, Paid = 2, Cancelled = 3 }
        public enum PaymentMethod { Cash = 0, Bank = 1, Wallet = 2, Other = 3 }
    public class Invoice
    {
            public int Id { get; set; }

            [MaxLength(30)]
            public string? InvoiceNo { get; set; }

            [Required]
            public int StudentId { get; set; }
            public Student Student { get; set; } = null!;

            public int? CourseId { get; set; }
            public Course? Course { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal Amount { get; set; }

            [MaxLength(8)]
            public string Currency { get; set; } = "EGP";

            public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

            public PaymentMethod? PaymentMethod { get; set; }

            [MaxLength(120)]
            public string? PaymentRef { get; set; }

            [MaxLength(500)]
            public string? Notes { get; set; }

            [MaxLength(300)]
            public string? AttachmentPath { get; set; }

            public DateTime? AccessStart { get; set; }
            public DateTime? AccessEnd { get; set; }

            public DateTime CreatedAt { get; set; }
            [MaxLength(450)]
            public string? CreatedByUserId { get; set; }

            public DateTime? UpdatedAt { get; set; }
            [MaxLength(450)]
            public string? UpdatedByUserId { get; set; } 
        }
    }
