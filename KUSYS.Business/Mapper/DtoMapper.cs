using AutoMapper;
using KUSYS.Dto;
using KUSYS.Model;

namespace KUSYS.Business.Mapper
{
    internal class DtoMapper:Profile
    {
        public DtoMapper()
        {
            CreateMap<StudentActionDto, Student>().ReverseMap();
        }
    }
}
