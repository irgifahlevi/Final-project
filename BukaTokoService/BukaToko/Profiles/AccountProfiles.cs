using AutoMapper;
using BukaToko.DTOS;
using BukaToko.Models;

namespace BukaToko.Profiles
{
    public class AccountProfiles : Profile
    {
        public AccountProfiles()
        {
            CreateMap<User, LoginUserDto>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<ReadUserDto, User>();
            CreateMap<User , ReadUserDto>();
            CreateMap<LoginUserDto,User>();

        }
    }
}
