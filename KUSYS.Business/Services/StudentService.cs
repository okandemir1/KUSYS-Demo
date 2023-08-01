using FluentValidation;
using KUSYS.Business.Encyription;
using KUSYS.Business.Filters;
using KUSYS.Business.Mapper;
using KUSYS.Data.Repository;
using KUSYS.Dto;
using KUSYS.Dto.Validation;
using KUSYS.Model;
using Microsoft.EntityFrameworkCore;

namespace KUSYS.Business.Interfaces
{
    public class StudentService : IStudentService
    {
        private readonly IRepository<Student> _studentRepository;

        Cipher _cipher;
        public StudentService(IRepository<Student> _studentRepository)
        {
            this._studentRepository = _studentRepository;
            _cipher = new Cipher();
        }

        public Task<List<Student>> GetStudents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Yeni öğrenci eklememizi sağlar
        /// </summary>
        /// <param name="mDto"></param>
        /// <returns></returns>
        public async Task<DbOperationResult> Add(StudentActionDto mDto)
        {
            var validCheck = new StudentActionValidation().Validate(mDto);
            if (!validCheck.IsValid)
            {
                var errors = new List<string>();
                validCheck.Errors.ForEach(x => errors.Add(x.ErrorMessage));
                return new DbOperationResult(false, "Eksik veya hatalı veri girişi", errors);
            }

            if(string.IsNullOrWhiteSpace(mDto.Password))
                return new DbOperationResult(false, "Eksik veya hatalı veri girişi", new List<string>() { "Şifre boş bırakılamaz" });

            try
            {
                var hasExistUsername = await ExistUsername(mDto.Username);
                if (hasExistUsername)
                    return new DbOperationResult(false, "Bu kullanıcı adı kullanılıyor");

                mDto.Password = _cipher.Encrypt(mDto.Password);
                var model = ObjectMapper.Mapper.Map<Student>(mDto);
                var operationResult = await _studentRepository.Insert(model);
                return operationResult;

            }
            catch (Exception ex)
            {
                //Log tutulması gerekir...
                return new DbOperationResult(false, ex.Message);
            }
        }

        /// <summary>
        /// Öğrenci verilerini günceller
        /// </summary>
        /// <param name="mDto"></param>
        /// <returns></returns>
        public async Task<DbOperationResult> Edit(StudentActionDto mDto)
        {
            var validCheck = new StudentActionValidation().Validate(mDto);
            if (!validCheck.IsValid)
            {
                var errors = new List<string>();
                validCheck.Errors.ForEach(x => errors.Add(x.ErrorMessage));
                return new DbOperationResult(false, "Eksik veya hatalı veri girişi", errors);
            }

            try
            {
                var modelInDb = await _studentRepository.ListQueryable
                    .Where(x => x.StudentId == mDto.StudentId && !x.IsDeleted).FirstOrDefaultAsync();

                if (modelInDb == null)
                    return new DbOperationResult(false, "Kullanıcı verisine erişilemedi");

                if(mDto.Username != modelInDb.Username)
                {
                    var checkUsername = await ExistUsername(mDto.Username, mDto.StudentId);
                    if (checkUsername)
                        return new DbOperationResult(false, "Bu kullanıcı adı başka bir öğrenci tarafından kullanılıyor");
                    else
                        modelInDb.Username = mDto.Username;
                }

                modelInDb.BirthDate = mDto.BirthDate;
                modelInDb.FirstName = mDto.FirstName;
                modelInDb.LastName = mDto.LastName;

                var operationResult = await _studentRepository.Update(modelInDb);
                return operationResult;

            }
            catch (Exception ex)
            {
                //Log tutulması gerekir...
                return new DbOperationResult(false, ex.Message);
            }
        }

        /// <summary>
        /// Kullanıcı bilgilerini gizler
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DbOperationResult> Delete(string id)
        {
            var student = await _studentRepository.ListQueryable
                .FirstOrDefaultAsync(x => x.StudentId == id && !x.IsDeleted);

            if (student == null)
                return new DbOperationResult(false, "Kullanıcı verisine erişelemedi");

            student.IsDeleted = true;
            var operationResult = await _studentRepository.Update(student);
            return operationResult;
        }

        /// <summary>
        /// Böyle bir kullanıcı daha önce kullanılmış mı?
        /// studentId verisi zorunlu değildir. Güncelleme işlemi sırasında kullanılır
        /// </summary>
        /// <param name="username"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<bool> ExistUsername(string username, string studentId = "")
        {
            var exist = await _studentRepository.ListQueryableNoTracking
                .AnyAsync(x => x.Username == username && !string.IsNullOrWhiteSpace(studentId) ? x.StudentId != studentId : true && !x.IsDeleted);

            return exist;
        }

        /// <summary>
        /// Kullanıcı adı ve şifre kontrolünü sağlar hem güncelleme hemde giriş yapma sırasında burası kullanılabilir
        /// studentId verisi zorunlu değildir. Güncelleme işlemi sırasında kullanılır
        /// </summary>
        /// <param name="username"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<Student> PasswordCheck(string username, string password, string studentId = "")
        {
            var exist = await _studentRepository.ListQueryableNoTracking
                .FirstOrDefaultAsync(x => 
                    x.Username == username 
                    && x.Password == password 
                    && !string.IsNullOrWhiteSpace(studentId) ? x.StudentId == studentId : true 
                    && !x.IsDeleted);

            return exist;
        }

        public async Task<DbOperationResult<StudentSimpleDto>> Login(LoginDto mDto)
        {
            var validCheck = new LoginValidation().Validate(mDto);
            if (!validCheck.IsValid)
            {
                var errors = new List<string>();
                validCheck.Errors.ForEach(x => errors.Add(x.ErrorMessage));
                return new DbOperationResult<StudentSimpleDto>(false, "Eksik veya hatalı veri girişi", errors, null);
            }

            var hasExistUsername = await ExistUsername(mDto.Username);
            if (!hasExistUsername)
                return new DbOperationResult<StudentSimpleDto>(false, "Kullanıcı adı veya şifre hatalı");

            var login = await PasswordCheck(mDto.Username,mDto.Password);
            if(login == null)
                return new DbOperationResult<StudentSimpleDto>(false, "Kullanıcı adı veya şifre hatalı");

            var response = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == login.StudentId && !x.IsDeleted)
                .Select(x => new StudentSimpleDto()
                {
                    Fullname = $"{x.FirstName} {x.LastName}",
                    RoleId = x.RoleId,
                    StudentId = x.StudentId
                }).FirstOrDefaultAsync();

            if (response == null)
                return new DbOperationResult<StudentSimpleDto>(false, "Kullanıcı bulunamadı");

            return new DbOperationResult<StudentSimpleDto>(true, "Giriş Yapıldı", response);
        }

        //Sadece session için kullan
        public async Task<StudentSimpleDto> GetStudent(string id)
        {
            var data = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == id && !x.IsDeleted)
                .Select(x => new StudentSimpleDto()
                {
                    Fullname = $"{x.FirstName} {x.LastName}",
                    RoleId = x.RoleId,
                    StudentId = x.StudentId
                }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<DataTableViewModelResult<List<StudentSimpleDto>>> GetAll(StudentFilterModel filterModel)
        {
            var response = new DataTableViewModelResult<List<StudentSimpleDto>>();
            response.IsSucceeded = true;

            var result = _studentRepository.ListQueryableNoTracking
                .Where(x => !x.IsDeleted)
                .Select(x=> new StudentSimpleDto()
                {
                    Fullname = $"{x.FirstName} {x.LastName}",
                    StudentId = x.StudentId,
                });

            response.TotalCount = result.Count();
            response.RecordsFiltered = result.AddSearchFilters(filterModel).Count();
            response.Data = await result.AddSearchFilters(filterModel).AddOrderAndPageFilters(filterModel).ToListAsync();

            return response;
        }
    }
}
