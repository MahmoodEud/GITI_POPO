using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;      
using System.Security.Claims;            
using System.Linq;                       
using System;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
            private readonly IInvoicesService _invoices;
            private readonly AppGConetxt _ctx;

            public InvoicesController(IInvoicesService invoices, AppGConetxt ctx)
            {
                _invoices = invoices;
                _ctx = ctx;
            }

        private string? CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task<int?> GetCurrentStudentIdAsync()
        {
            var uid = CurrentUserId;
            if (string.IsNullOrWhiteSpace(uid)) return null;
            var s = await _ctx.Students.AsNoTracking()
                .Where(x => x.UserId == uid)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();
            return s;
        }

        [HttpGet("{id:int}")]
            [Authorize(Roles = "Admin,Assistance")]
            public async Task<ActionResult<InvoiceDto>> GetById(int id)
            {
                var dto = await _invoices.GetByIdAsync(id);
                if (dto is null) return NotFound();
                return Ok(dto);
            }

            [HttpGet]
            [Authorize(Roles = "Admin,Assistance")]
            public async Task<ActionResult<PagedResult<InvoiceDto>>> Search(
                [FromQuery] int? studentId,
                [FromQuery] int? courseId,
                [FromQuery] ITI_GProject.Data.Models.InvoiceStatus? status,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var filter = new InvoiceSearchFilter
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    Status = status,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    Page = page,
                    PageSize = pageSize
                };
                var res = await _invoices.SearchAsync(filter);
                return Ok(res);
            }

            [HttpGet("by-student/{studentId:int}")]
            [Authorize(Roles = "Admin,Assistance")]
            public async Task<ActionResult<IEnumerable<InvoiceDto>>> ByStudent(int studentId)
            {
                var list = await _invoices.GetByStudentAsync(studentId);
                return Ok(list);
            }

            [HttpGet("mine")]
            [Authorize]
            public async Task<ActionResult<IEnumerable<InvoiceDto>>> Mine()
            {
                var uid = CurrentUserId;
                if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
                var list = await _invoices.GetMineAsync(uid);
                return Ok(list);
            }

            [HttpGet("mine/{id:int}")]
            [Authorize]
            public async Task<ActionResult<InvoiceDto>> MineById(int id)
            {
                var uid = CurrentUserId;
                if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
                var dto = await _invoices.GetMineByIdAsync(uid, id);
                if (dto is null) return NotFound();
                return Ok(dto);
            }

            [HttpPost]
            [Authorize(Roles = "Admin,Assistance")]
            public async Task<ActionResult<InvoiceDto>> CreateDraft([FromBody] InvoiceCreateDto dto)
            {
                var uid = CurrentUserId ?? "";
                var created = await _invoices.CreateDraftAsync(dto, uid);
                if (created is null) return BadRequest();
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }

            [HttpPost("{id:int}/send")]
            [Authorize(Roles = "Admin,Assistance")]
            public async Task<ActionResult<InvoiceDto>> Send(int id)
            {
                var uid = CurrentUserId ?? "";
                var dto = await _invoices.SendAsync(id, uid);
                if (dto is null) return NotFound();
                return Ok(dto);
            }

            [HttpPost("{id:int}/pay")]
            [Authorize(Roles = "Admin,Assistance")]
            public async Task<ActionResult<InvoiceDto>> MarkPaid(int id, [FromBody] InvoicePaymentDto body)
            {
                var uid = CurrentUserId ?? "";
                var dto = await _invoices.MarkPaidAsync(id, body, uid);
                if (dto is null) return NotFound();
                return Ok(dto);
            }

                [HttpPost("{id:int}/cancel")]
                [Authorize(Roles = "Admin,Assistance")]
                public async Task<ActionResult<InvoiceDto>> Cancel(int id, [FromBody] InvoiceCancelDto body)
                {
                    var uid = CurrentUserId ?? "";
                    var dto = await _invoices.CancelAsync(id, body?.Reason, uid);
                    if (dto is null) return NotFound();
                    return Ok(dto);
                }

        [HttpPost("{id:int}/attachment")]
        [Authorize(Roles = "Admin,Assistance")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<InvoiceDto>> UploadAttachment(int id, [FromForm] InvoiceAttachmentDto form)
        {
            if (form.File == null || form.File.Length == 0) return BadRequest("Empty file");

            var uid = CurrentUserId ?? "";
            var dto = await _invoices.UploadAttachmentAsync(id, form.File, uid);
            if (dto is null) return NotFound();
            return Ok(dto);
        }
        [Authorize(Roles = "Admin,Assistance")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDraft(int id, [FromBody] InvoiceUpdateDraftDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var inv = await _ctx.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (inv is null) return NotFound(new { message = "الفاتورة غير موجودة." });
            if (inv.Status != InvoiceStatus.Draft)
                return Conflict(new { message = "لا يمكن تعديل إلا الفواتير المسودة." });

            var updated = await _invoices.UpdateDraftAsync(id, dto, userId);
            if (updated is null) return BadRequest(new { message = "تعذّر حفظ التعديلات. راجع البيانات." });

            return Ok(updated);
        }
        [Authorize(Roles = "Admin,Assistance")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDraft(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var ok = await _invoices.DeleteDraftAsync(id, userId);
            if (!ok)
                return Conflict(new { message = "لا يمكن حذف إلا الفواتير المسودة أو الفاتورة غير موجودة." });

            return NoContent();
        }



    }
}
