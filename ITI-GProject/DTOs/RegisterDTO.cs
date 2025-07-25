namespace ITI_GProject.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        [RegularExpression(@"^([a-zA-Z\u0600-\u06FF]{2,}\s){3}[a-zA-Z\u0600-\u06FF]{2,}$",
    ErrorMessage = "الاسم يجب أن يتكون من 4 مقاطع باللغة العربية أو الإنجليزية فقط بدون رموز أو أرقام")]
        public string Name { get; set; }
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [Display(Name = "اسم المستخدم")]
        [RegularExpression(@"^(?!.*[_.]{2})(?![_.])[a-zA-Z0-9._]{4,18}(?<![_.])$",
    ErrorMessage = "الاسم يجب أن يتكون من 4 مقاطع باللغة العربية أو الإنجليزية فقط بدون رموز أو أرقام")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Display(Name = "رقم الهاتف")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم الهاتف يجب أن يكون مكون من 11 رقم ويبدأ بـ 010 أو 011 أو 012 أو 015")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "رقم ولي الأمر مطلوب")]
        [Display(Name = "رقم ولي الأمر")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم ولي الأمر يجب أن يكون مكون من 11 رقم ويبدأ بـ 010 أو 011 أو 012 أو 015")]
        public string Pphone { get; set; }

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الميلاد")]
        public DateTime Birthdate { get; set; }
        [Required(ErrorMessage = "السنة الدراسية مطلوبة")]
        [Display(Name = "السنة الدراسية")]
        public StudentYear studentYear { get; set; }
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل 6 حروف")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string ConfirmPassword { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}
