﻿using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Service.Contracts;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService; 
        private readonly Lazy<IAuthenticationService> _authenticationService; 
        public ServiceManager(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IEmployeeLinks employeeLinks,
            UserManager<User> userManager, IOptions<JwtConfiguration> configuration)
        {
            _companyService = new Lazy<ICompanyService> (()=>new ComapanyService(repository, logger,mapper));
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repository, logger, mapper, employeeLinks));
            _authenticationService = new Lazy<IAuthenticationService> (()=> new AuthenticationService(logger,mapper,userManager,configuration));
        }
        public ICompanyService CompanyService => _companyService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
