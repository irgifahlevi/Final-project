using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BukaToko.DTOS;
using BukaToko.Models;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BukaToko.Profiles
{
    public class OrderProfiles : Profile
    {
        public OrderProfiles()
        {
            CreateMap<Order, ReadOrderDto>();
            CreateMap<Int32,WalletPublishDto>();
        }
    }
}
