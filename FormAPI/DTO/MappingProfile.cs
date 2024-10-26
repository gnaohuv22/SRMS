using AutoMapper;
using BusinessObject;

namespace FormAPI.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<Form, FormDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<FormDto, Form>()
                .ForPath(dest => dest.Category.Name, opt => opt.Ignore())
                .ForPath(dest => dest.User.Email, opt => opt.Ignore())
                .ForPath(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForPath(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<Response, ResponseDto>()
                .ForMember(dest => dest.FormSubject, opt => opt.MapFrom(src => src.Form.Subject))
                .ForMember(dest => dest.FormContent, opt => opt.MapFrom(src => src.Form.Content))
                .ForMember(dest => dest.StaffEmail, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<ResponseDto, Response>()
                .ForPath(dest => dest.Form.Subject, opt => opt.Ignore())
                .ForPath(dest => dest.Form.Content, opt => opt.Ignore())
                .ForPath(dest => dest.User.Email, opt => opt.Ignore())
                .ForPath(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
