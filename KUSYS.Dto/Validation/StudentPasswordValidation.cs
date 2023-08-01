using FluentValidation;

namespace KUSYS.Dto.Validation
{
    public class StudentPasswordValidation : AbstractValidator<StudentPasswordDto>
    {
        public StudentPasswordValidation()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Yeni Şifre Boş Olamaz");
            RuleFor(x => x.RPassword)
                .NotEmpty().WithMessage("Yeni Şifre Tekrar Boş Olamaz");
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Eski Şifre Boş Olamaz");
            RuleFor(x => x.Password == x.RPassword)
                .NotEqual(x => x.Password == x.RPassword).WithMessage("Yeni Şifreler Uyuşmuyor");
        }
    }
}
