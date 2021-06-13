using AutoMapper;
using SetakTest.Entities;
using SetakTest.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Infrastrucrure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, TokenViewModel>()
               .ForMember(r => r.UserId, mo => mo.MapFrom(f => f.Id));


        }
    }
}
