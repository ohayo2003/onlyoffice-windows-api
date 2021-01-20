using System;
using AutoMapper;
using WebApi.Entity;
using WebApi.Web.ViewModels;

namespace WebApi.Web.Automapper.Profiles
{
    public class SchoolAdminDto2UserInfo : Profile
    {
        public SchoolAdminDto2UserInfo()
        {
            CreateMap<SchoolAdminCreateDto, UserInfo>()
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(s => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(s => (int) UserStatus.Available))
                .ForMember(dest => dest.RoleType, opt => opt.MapFrom(s => Role.reviewer))
                .ForMember(dest => dest.PhoneNum, opt => opt.MapFrom(s => string.Empty))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(s => DateTime.MaxValue));
        }
    }
    public class TeacherCreateDto2UserInfo : Profile
    {
        public TeacherCreateDto2UserInfo()
        {
            CreateMap<TeacherCreateDto, UserInfo>()
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(s => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(s => (int) UserStatus.Available))
                .ForMember(dest => dest.RoleType, opt => opt.MapFrom(s => Role.reviewer))
                .ForMember(dest => dest.PhoneNum, opt => opt.MapFrom(s => string.Empty))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(s => DateTime.MaxValue));
        }
    }



    public class StudentCreateDto2UserInfo : Profile
    {
        public StudentCreateDto2UserInfo()
        {
            CreateMap<RequesterCreateDto, UserInfo>()
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(s => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(s => (int) UserStatus.Available))
                .ForMember(dest => dest.RoleType, opt => opt.MapFrom(s => Role.requester))
                .ForMember(dest => dest.UserAccount, opt => opt.MapFrom(s => string.Empty))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(s => DateTime.MaxValue));

        }
    }
    public class UserLoginDto2UserInfo : Profile
    {
        public UserLoginDto2UserInfo()
        {
            CreateMap<UserLoginDto, UserInfo>()
                .ForMember(dest => dest.UserAccount, opt => opt.MapFrom(s => s.UserName))
                .ForMember(dest => dest.Password, opt =>opt.MapFrom(s => s.Pin))
                .ForMember(dest => dest.PhoneNum, opt => opt.MapFrom(s => s.PhoneNum??string.Empty));
        }
    }
}