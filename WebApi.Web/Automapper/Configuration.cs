using AutoMapper;

namespace WebApi.Web.Automapper
{
    /// <summary>
    /// 
    /// </summary>
    public class Configuration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                //cfg.AddProfile<Profiles.SchoolAdminDto2UserInfo>();
                //cfg.AddProfile<Profiles.StudentCreateDto2UserInfo>();
                //cfg.AddProfile<Profiles.UserLoginDto2UserInfo>();

                //cfg.AddProfile<Profiles.TeacherCreateDto2UserInfo>();
            });
        }
    }
}