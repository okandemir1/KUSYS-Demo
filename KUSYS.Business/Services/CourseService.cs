using KUSYS.Data.Repository;
using KUSYS.Dto;
using KUSYS.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KUSYS.Business.Interfaces
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<StudentCourse> _studentCourseRepository;
        private readonly IRepository<Student> _studentRepository;
        public CourseService(IRepository<Course> _courseRepository, IRepository<StudentCourse> studentCourseRepository, IRepository<Student> _studentRepository)
        {
            this._courseRepository = _courseRepository;
            _studentCourseRepository = studentCourseRepository;
            this._studentRepository = _studentRepository;

        }
        public async Task<List<Course>> GetCoursesByIdList(List<string> ids)
        {
            var courses = await _courseRepository.ListQueryableNoTracking
                .Where(x => ids.Contains(x.CourseId) && !x.IsDeleted).ToListAsync();

            return courses;
        }

        public async Task<List<Course>> GetAllCourse()
        {
            var courses = await _courseRepository.ListQueryableNoTracking
                .Where(x => !x.IsDeleted).ToListAsync();
            return courses;
        }

        public async Task<List<StudentCourseList>> GetStudentCourses(string studentId, bool isOperation = false)
        {
            var model = new List<StudentCourseList>();

            var studentCourses = await _studentCourseRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == studentId).ToListAsync();

            var courses = await GetAllCourse();
            foreach (var item in courses)
            {
                var exist = studentCourses.Where(x => x.CourseId == item.CourseId).FirstOrDefault();
                if (!isOperation && exist == null)
                    continue;

                model.Add(new StudentCourseList()
                {
                    CourseId = item.CourseId,
                    CourseName = item.CourseName,
                    IsActive = exist != null ? exist.IsActive ? true : false : false,
                });
            }

            return model;
        }

        public async Task<DbOperationResult> AddStudentCourse(List<string> courseIds, string studentId, bool isOperation = false)
        {
            var student = await _studentRepository.ListQueryableNoTracking
                .Where(x=>x.StudentId == studentId && !x.IsDeleted).FirstOrDefaultAsync();

            if (student == null)
                return new DbOperationResult(false, "Öğrenci verilerine erişlemedi");

            if(courseIds.Count() <= 0)
                return new DbOperationResult(false, "Atanacak dersler seçilmemiş");

            var studentCourseList = await _studentCourseRepository.ListQueryableNoTracking
                .Where(x => x.StudentId == studentId && !x.IsDeleted).ToListAsync();

            var studentCourseIds = studentCourseList.Select(x => x.CourseId).ToList();

            var insertStudentCourses = new List<StudentCourse>();
            var updateStudentCourses = new List<StudentCourse>();

            foreach (var item in courseIds)
            {
                if(!studentCourseIds.Contains(item) && !isOperation)
                {
                    return new DbOperationResult(false, "Hatalı veya yetkisiz işlem");
                }

                if (!studentCourseIds.Contains(item) && isOperation)
                {
                    insertStudentCourses.Add(new StudentCourse()
                    {
                        CourseId = item,
                        CreateDate = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        StudentId = studentId,
                        UpdateDate = DateTime.Now,
                    });
                }
                else if(studentCourseIds.Contains(item) && !isOperation)
                {
                    var studentCourseItem = studentCourseList.Where(x => x.CourseId == item && !x.IsActive).FirstOrDefault();
                    if(studentCourseItem != null)
                    {
                        studentCourseItem.IsActive = true;
                        updateStudentCourses.Add(studentCourseItem);
                    }
                    studentCourseIds.Remove(item);
                    continue;
                }

                studentCourseIds.Remove(item);
            }

            foreach (var item in studentCourseIds)
            {
                var studentCourseItem = studentCourseList.Where(x => x.CourseId == item).FirstOrDefault();
                if(studentCourseItem != null)
                {
                    studentCourseItem.IsActive = false;
                    updateStudentCourses.Add(studentCourseItem);
                }
            }

            if(insertStudentCourses.Count() > 0 && updateStudentCourses.Count() > 0)
            {
                var insert = await _studentCourseRepository.Insert(insertStudentCourses);
                if (insert.IsSucceed)
                {
                    var update = await _studentCourseRepository.Update(updateStudentCourses);
                    return update;
                }
                return insert;
            }
            else if (insertStudentCourses.Count() > 0 && updateStudentCourses.Count() <= 0)
            {
                var insert = await _studentCourseRepository.Insert(insertStudentCourses);
                return insert;
            }
            else if (insertStudentCourses.Count() <= 0 && updateStudentCourses.Count() > 0)
            {
                var update = await _studentCourseRepository.Update(updateStudentCourses);
                return update;
            }
            else
            {
                return new DbOperationResult(false, "Hatalı işlem");
            }
        }
    }
}
