using AutoMapper;
using BukaToko.DTOS;
using BukaToko.Models;

namespace BukaToko.Profiles
{
    public class WalletProfiles : Profile
    {
        public WalletProfiles() 
        {
            CreateMap<TopUpPublishedDto, ReadTopUpDto>();
            CreateMap<Wallet, ReadWalletDto>();
            CreateMap<CashToSaldoDto, ReadTopUpDto>();

        }
    }
}
