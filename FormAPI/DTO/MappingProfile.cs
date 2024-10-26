using AutoMapper;
using BusinessObject;

namespace FormAPI.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<DepartmentDto, Department>().ReverseMap();
            CreateMap<Form, FormDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<FormDto, Form>()
                .ForPath(dest => dest.Category.Name, opt => opt.Ignore())
                .ForPath(dest => dest.User.Email, opt => opt.Ignore())
                .ForPath(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForPath(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<Response, ResponseDto>().ReverseMap();
        }
    }
}
