using AutoMapper;
using Chat.Data.Entities;
using Chat.Models;

namespace Chat.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dst => dst.Username, opt => opt.MapFrom(x => x.UserName));
            CreateMap<UserViewModel, User>();
        }
    }
}