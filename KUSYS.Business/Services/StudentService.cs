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
        private readonly IRepository<StudentCourse> _studentCourseRepository;
        private readonly ICourseService _courseService;

        Cipher _cipher;
        public StudentService(IRepository<Student> _studentRepository, IRepository<StudentCourse> _studentCourseRepository, ICourseService _courseService)
        {
            this._studentRepository = _studentRepository;
            this._studentCourseRepository = _studentCourseRepository;
            _cipher = new Cipher();
            this._courseService = _courseService;

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

                var studentId = Guid.NewGuid().ToString().Split('-')[0];
                var existStudentIds = await GetStudentIds();
                while (existStudentIds.Contains(studentId))
                {
                    studentId = Guid.NewGuid().ToString().Split('-')[0];
                }

                model.StudentId = studentId;
                var operationResult = await _studentRepository.Insert(model);
                return operationResult;

            }
            catch (Exception ex)
            {
                //Log tutulması gerekir...
                return new DbOperationResult(false, ex.Message);
            }
        }

        public async Task<List<string>> GetStudentIds()
        {
            var studentIds = await _studentRepository.ListQueryableNoTracking
                .Where(x => !x.IsDeleted)
                .GroupBy(x => x.StudentId).Select(x => x.Key).ToListAsync();

            return studentIds;
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
                    var checkUsername = await ExistUsernameOther(mDto.Username, mDto.StudentId);
                    if (checkUsername)
                        return new DbOperationResult(false, "Bu kullanıcı adı başka bir öğrenci tarafından kullanılıyor");
                    else
                        modelInDb.Username = mDto.Username;
                }

                modelInDb.BirthDate = mDto.BirthDate;
                modelInDb.FirstName = mDto.FirstName;
                modelInDb.LastName = mDto.LastName;
                modelInDb.RoleId = mDto.RoleId;

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
        /// </summary>
        /// <param name="username"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<bool> ExistUsername(string username)
        {
            var exist = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.Username == username 
                    && !x.IsDeleted)
                    .FirstOrDefaultAsync();

            return exist != null ? true : false;
        }

        public async Task<bool> ExistUsernameOther(string username, string studentId)
        {
            var exist = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.Username == username
                    && x.StudentId != studentId
                    && !x.IsDeleted)
                    .FirstOrDefaultAsync();

            return exist != null ? true : false;
        }

        /// <summary>
        /// Kullanıcı adı ve şifre kontrolünü sağlar hem güncelleme hemde giriş yapma sırasında burası kullanılabilir
        /// </summary>
        /// <param name="username"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<Student> PasswordCheck(string username, string password)
        {
            var exist = await _studentRepository.ListQueryableNoTracking
                .FirstOrDefaultAsync(x => 
                    x.Username == username 
                    && x.Password == password 
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

            var encryptPass = _cipher.Encrypt(mDto.Password);

            var login = await PasswordCheck(mDto.Username, encryptPass);
            if(login == null)
                return new DbOperationResult<StudentSimpleDto>(false, "Kullanıcı adı veya şifre hatalı");

            var response = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == login.StudentId && !x.IsDeleted)
                .Select(x => new StudentSimpleDto()
                {
                    Firstname = x.FirstName, 
                    Lastname = x.LastName,
                    RoleId = x.RoleId,
                    StudentId = x.StudentId
                }).FirstOrDefaultAsync();

            if (response == null)
                return new DbOperationResult<StudentSimpleDto>(false, "Kullanıcı bulunamadı");

            return new DbOperationResult<StudentSimpleDto>(true, "Giriş Yapıldı", response);
        }

        public async Task<StudentActionDto> GetStudentInfo(string id)
        {
            var data = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            var mapData = ObjectMapper.Mapper.Map<StudentActionDto>(data);
            return mapData;
        }

        //Sadece session için kullan
        public async Task<StudentSimpleDto> GetStudent(string id)
        {
            var data = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == id && !x.IsDeleted)
                .Select(x => new StudentSimpleDto()
                {
                    Firstname = x.FirstName, 
                    Lastname = x.LastName,
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
                    Firstname = x.FirstName,
                    Lastname = x.LastName,
                    StudentId = x.StudentId,
                });

            response.TotalCount = await result.CountAsync();
            response.RecordsFiltered = await result.AddSearchFilters(filterModel).CountAsync();
            response.Data = await result.AddSearchFilters(filterModel).AddOrderAndPageFilters(filterModel).ToListAsync();

            return response;
        }

        public async Task<StudentCourseViewList> GetAllWithCourses()
        {
            var students = await _studentRepository.ListQueryableNoTracking
                .Where(x => !x.IsDeleted).ToListAsync();

            var studentIds = students.GroupBy(x => x.StudentId).Select(x => x.Key).ToList();

            var studentCourses = await _studentCourseRepository.ListQueryableNoTracking
                .Where(x => studentIds.Contains(x.StudentId) && x.IsActive).ToListAsync();

            var studentCourseIds = studentCourses.GroupBy(x => x.CourseId).Select(x => x.Key).ToList();

            var getCourses = await _courseService.GetCoursesByIdList(studentCourseIds);

            var model = new StudentCourseViewList()
            {
                Courses = getCourses,
                Students = students,
                StudentCourses = studentCourses,
            };

            return model;
        }

        public async Task<StudentCourseViewList> GetStudentWithCourses(string studentId)
        {
            var students = await _studentRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == studentId && !x.IsDeleted).ToListAsync();

            var studentCourses = await _studentCourseRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == studentId && x.IsActive && !x.IsDeleted).ToListAsync();

            var studentCourseIds = studentCourses.GroupBy(x => x.CourseId).Select(x => x.Key).ToList();

            var getCourses = await _courseService.GetCoursesByIdList(studentCourseIds);

            var model = new StudentCourseViewList()
            {
                Courses = getCourses,
                Students = students,
                StudentCourses = studentCourses,
            };

            return model;
        }
    }
}
