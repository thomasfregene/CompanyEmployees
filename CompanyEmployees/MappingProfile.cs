﻿using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace CompanyEmployees
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /*This time, we are not using the ForMember method but the 
ForCtorParam method to specify the name of the parameter in the 
constructor that AutoMapper needs to map to
            
             This happens because AutoMapper is not able to find the specific 
FullAddress property as we specified in the MappingProfile class*/

            //CreateMap<Company, CompanyDto>()
            //    .ForCtorParam("FullAddress", 
            //    opt => opt.MapFrom(x=>string.Join(' ', x.Address, x.Country)));
            CreateMap<Company, CompanyDto>()
                .ForMember(c=>c.FullAddress, 
                opt=> opt.MapFrom(x=>string.Join(' ', x.Address, x.Country)));

            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<CompanyForUpdateDto, Company>();

            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeForCreationDto, Employee>();
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();

            CreateMap<UserForRegistrationRecordDto, User>();
        }
    }
}
