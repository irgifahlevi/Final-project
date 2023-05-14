using AdaDanaService.Dtos;
using AdaDanaService.Models;
using AutoMapper;

namespace AdaDanaService.Profiles
{
    public class AdaDanaProfiles : Profile
    {
        public AdaDanaProfiles()
        {
            // Source => destination
            CreateMap<User, ReadUserDto>();
            CreateMap<User, ReadUserDto>();
            CreateMap<User, RegisterUserDto>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<Wallet, UserBelanceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.Saldo))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            CreateMap<TopupWalletPublishDto, Wallet>();
            CreateMap<CashToSaldoDto, CashOutDto>();
            CreateMap<TopUpDto, TopupWalletPublishDto>();

        }
    }
}
