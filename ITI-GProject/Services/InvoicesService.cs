using System.Linq.Expressions;

namespace ITI_GProject.Services
{
    public class InvoicesService: IInvoicesService
    {
        private readonly AppGConetxt _ctx;
        private readonly ILocalClock _clock;
        private readonly IStudentCoursesService _studentCourses;
        private readonly INotificationsService _notifications;
        private readonly IAttachment _attachment; 

        public InvoicesService(
            AppGConetxt ctx,
            ILocalClock clock,
            IStudentCoursesService studentCourses,
            INotificationsService notifications,
            IAttachment attachment)
        {
            _ctx = ctx;
            _clock = clock;
            _studentCourses = studentCourses;
            _notifications = notifications;
            _attachment = attachment;
        }

        public async Task<InvoiceDto?> CreateDraftAsync(InvoiceCreateDto dto, string createdByUserId)
        {
            var studentExists = await _ctx.Students.AnyAsync(s => s.Id == dto.StudentId);
            if (!studentExists) return null;

            if (dto.CourseId.HasValue)
            {
                var courseExists = await _ctx.Courses.AnyAsync(c => c.Id == dto.CourseId.Value);
                if (!courseExists) return null;
            }

            var inv = new Invoice
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Amount = dto.Amount,
                Currency = dto.Currency ?? "EGP",
                Notes = dto.Notes,
                PaymentRef = dto.PaymentRef,
                AccessStart = dto.AccessStart,
                AccessEnd = dto.AccessEnd,
                Status = InvoiceStatus.Draft,
                CreatedAt = _clock.Now(),
                CreatedByUserId = createdByUserId
            };

            _ctx.Invoices.Add(inv);
            await _ctx.SaveChangesAsync();

            return await MapOne(inv.Id);
        }

        public async Task<InvoiceDto?> SendAsync(int invoiceId, string updatedByUserId)
        {
            var inv = await _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (inv is null) return null;
            if (inv.Status != InvoiceStatus.Draft && inv.Status != InvoiceStatus.Cancelled)
                return null;

            if (string.IsNullOrWhiteSpace(inv.InvoiceNo))
                inv.InvoiceNo = await GenerateInvoiceNoAsync();

            inv.Status = InvoiceStatus.Sent;
            inv.UpdatedAt = _clock.Now();
            inv.UpdatedByUserId = updatedByUserId;

            await _ctx.SaveChangesAsync();

            await _notifications.CreateAsync(inv.StudentId,
                "تم إصدار فاتورة",
                $"تم إصدار فاتورة رقم {inv.InvoiceNo} بمبلغ {inv.Amount} {inv.Currency}.");

            return await MapOne(inv.Id);
        }

        public async Task<InvoiceDto?> MarkPaidAsync(int invoiceId, InvoicePaymentDto payment, string updatedByUserId)
        {
            var inv = await _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (inv is null) return null;

            inv.PaymentMethod = payment.Method;
            inv.PaymentRef = payment.PaymentRef ?? inv.PaymentRef;
            inv.Notes = string.IsNullOrWhiteSpace(payment.Notes) ? inv.Notes : payment.Notes;

            if (payment.PaidAmount.HasValue && payment.PaidAmount.Value > 0)
                inv.Amount = payment.PaidAmount.Value;

            // لو الرقم مش مولّد قبل كده نولده
            if (string.IsNullOrWhiteSpace(inv.InvoiceNo))
                inv.InvoiceNo = await GenerateInvoiceNoAsync();

            inv.Status = InvoiceStatus.Paid;
            inv.UpdatedAt = _clock.Now();
            inv.UpdatedByUserId = updatedByUserId;

            await _ctx.SaveChangesAsync();

            // إنرول في الكورس لو الفاتورة مرتبطة بكورس
            if (inv.CourseId.HasValue)
            {
                await _studentCourses.EnrollAsync(inv.StudentId, inv.CourseId.Value, allowPaid: true);
            }

            await _notifications.CreateAsync(inv.StudentId,
                "تم تأكيد الدفع",
                $"تم دفع فاتورة رقم {inv.InvoiceNo}. المبلغ: {inv.Amount} {inv.Currency}.");

            return await MapOne(inv.Id);
        }

        public async Task<InvoiceDto?> CancelAsync(int invoiceId, string? reason, string updatedByUserId)
        {
            var inv = await _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (inv is null) return null;

            inv.Status = InvoiceStatus.Cancelled;
            inv.Notes = string.IsNullOrWhiteSpace(reason) ? inv.Notes : $"{inv.Notes}\n[{reason}]";
            inv.UpdatedAt = _clock.Now();
            inv.UpdatedByUserId = updatedByUserId;

            await _ctx.SaveChangesAsync();

            await _notifications.CreateAsync(inv.StudentId,
                "إلغاء فاتورة",
                $"تم إلغاء الفاتورة {(string.IsNullOrWhiteSpace(inv.InvoiceNo) ? $"#{inv.Id}" : inv.InvoiceNo)}.");

            return await MapOne(inv.Id);
        }

        public async Task<InvoiceDto?> UploadAttachmentAsync(int invoiceId, IFormFile file, string updatedByUserId)
        {
            var inv = await _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (inv is null) return null;

            var path = await _attachment.UploadAsync(file, "invoices");
            if (!string.IsNullOrWhiteSpace(path))
            {
                inv.AttachmentPath = path;
                inv.UpdatedAt = _clock.Now();
                inv.UpdatedByUserId = updatedByUserId;
                await _ctx.SaveChangesAsync();
            }

            return await MapOne(inv.Id);
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id) => await MapOne(id);

        public async Task<PagedResult<InvoiceDto>> SearchAsync(InvoiceSearchFilter filter)
        {
            var q = _ctx.Invoices
                .AsNoTracking()
                .Include(i => i.Student).ThenInclude(s => s.User)
                .Include(i => i.Course)
                .AsQueryable();

            if (filter.StudentId.HasValue) q = q.Where(i => i.StudentId == filter.StudentId.Value);
            if (filter.Status.HasValue) q = q.Where(i => i.Status == filter.Status.Value);
            if (filter.DateFrom.HasValue) q = q.Where(i => i.CreatedAt >= filter.DateFrom.Value);
            if (filter.DateTo.HasValue) q = q.Where(i => i.CreatedAt < filter.DateTo.Value.AddDays(1));

            q = q.OrderByDescending(i => i.CreatedAt);

            var page = Math.Max(1, filter.Page);
            var pageSize = Math.Clamp(filter.PageSize, 10, 100);

            var totalCount = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(SelectDto())
                .ToListAsync();

            return new PagedResult<InvoiceDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetByStudentAsync(int studentId)
        {
            var list = await _ctx.Invoices
                .AsNoTracking()
                .Include(i => i.Student).ThenInclude(s => s.User)
                .Include(i => i.Course)
                .Where(i => i.StudentId == studentId)
                .OrderByDescending(i => i.CreatedAt)
                .Select(SelectDto())
                .ToListAsync();

            return list;
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetMineAsync(string userId)
        {
            var studentId = await GetStudentIdByUserId(userId);
            if (studentId == null) return Array.Empty<InvoiceDto>();

            return (await GetByStudentAsync(studentId.Value)).ToList();
        }

        public async Task<InvoiceDto?> GetMineByIdAsync(string userId, int invoiceId)
        {
            var studentId = await GetStudentIdByUserId(userId);
            if (studentId == null) return null;

            return await _ctx.Invoices
                .AsNoTracking()
                .Include(i => i.Student).ThenInclude(s => s.User)
                .Include(i => i.Course)
                .Where(i => i.Id == invoiceId && i.StudentId == studentId.Value)
                .Select(SelectDto())
                .FirstOrDefaultAsync();
        }

        // Helpers
        private async Task<InvoiceDto?> MapOne(int id)
        {
            return await _ctx.Invoices
                .AsNoTracking()
                .Include(i => i.Student).ThenInclude(s => s.User)
                .Include(i => i.Course)
                .Where(i => i.Id == id)
                .Select(SelectDto())
                .FirstOrDefaultAsync();
        }

        private static Expression<Func<Invoice, InvoiceDto>> SelectDto()
        {
            return i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNo = i.InvoiceNo,
                StudentId = i.StudentId,
                StudentName = i.Student.User.Name,
                CourseId = i.CourseId,
                CourseTitle = i.Course != null ? i.Course.Title : null,
                Amount = i.Amount,
                Currency = i.Currency,
                Status = i.Status,
                PaymentMethod = i.PaymentMethod,
                PaymentRef = i.PaymentRef,
                Notes = i.Notes,
                AttachmentUrl = i.AttachmentPath, // لو عايز تحولها لـ URL كاملة اعمل ذلك هنا
                AccessStart = i.AccessStart,
                AccessEnd = i.AccessEnd,
                CreatedAt = i.CreatedAt,
                CreatedByUserId = i.CreatedByUserId,
                UpdatedAt = i.UpdatedAt,
                UpdatedByUserId = i.UpdatedByUserId
            };
        }

        private async Task<string> GenerateInvoiceNoAsync()
        {
            // نمط بسيط: INV-YYYYMM-#### (متسلسل شهري)
            var now = _clock.Now();
            var prefix = $"INV-{now:yyyyMM}-";

            var last = await _ctx.Invoices
                .AsNoTracking()
                .Where(i => i.InvoiceNo!.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNo)
                .Select(i => i.InvoiceNo)
                .FirstOrDefaultAsync();

            int next = 1;
            if (!string.IsNullOrWhiteSpace(last) && int.TryParse(last.Substring(prefix.Length), out var n))
                next = n + 1;

            return prefix + next.ToString("D4");
        }

        private async Task<int?> GetStudentIdByUserId(string userId)
        {
            return await _ctx.Students
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync();
        }


        public async Task<InvoiceDto?> UpdateDraftAsync(int id, InvoiceUpdateDraftDto dto, string updatedByUserId)
        {
            var inv = await _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == id);
            if (inv is null) return null;

            if (inv.Status != InvoiceStatus.Draft)
                return null; 

            if (dto.StudentId.HasValue)
            {
                var studentExists = await _ctx.Students.AnyAsync(s => s.Id == dto.StudentId.Value);
                if (!studentExists) return null; 
            }

            if (dto.CourseId.HasValue)
            {
                var courseExists = await _ctx.Courses.AnyAsync(c => c.Id == dto.CourseId.Value);
                if (!courseExists) return null; 
            }

            if (dto.Amount.HasValue && dto.Amount.Value <= 0)
                return null; 

            if (dto.AccessStart.HasValue && dto.AccessEnd.HasValue &&
                dto.AccessStart.Value > dto.AccessEnd.Value)
                return null; 

            if (dto.StudentId.HasValue) inv.StudentId = dto.StudentId.Value;
            inv.CourseId = dto.CourseId; 
            if (dto.Amount.HasValue) inv.Amount = dto.Amount.Value;

            inv.Currency = "EGP";

            if (!string.IsNullOrWhiteSpace(dto.PaymentRef)) inv.PaymentRef = dto.PaymentRef;
            if (dto.Notes != null) inv.Notes = dto.Notes; 

            inv.AccessStart = dto.AccessStart;
            inv.AccessEnd = dto.AccessEnd;

            inv.UpdatedAt = _clock.Now();              
            inv.UpdatedByUserId = updatedByUserId;

            await _ctx.SaveChangesAsync();
            return await MapOne(inv.Id);
        }

        public async Task<bool> DeleteDraftAsync(int id, string deletedByUserId)
        {
            var inv = await _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == id);
            if (inv is null) return false;                   
            if (inv.Status != InvoiceStatus.Draft) return false;



            _ctx.Invoices.Remove(inv);

  

            await _ctx.SaveChangesAsync();
            return true;
        }

    }
}

