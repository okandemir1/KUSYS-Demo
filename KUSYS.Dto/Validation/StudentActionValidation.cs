using FluentValidation;

namespace KUSYS.Dto.Validation
{
    public class StudentActionValidation : AbstractValidator<StudentActionDto>
    {
        public StudentActionValidation()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı Adı Boş Olamaz");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Adı Boş Olamaz");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyadı Boş Olamaz");
            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Doğum Tarihi Boş Olamaz");
        }
    }
}
