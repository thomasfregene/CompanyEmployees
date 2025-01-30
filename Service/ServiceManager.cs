using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService; 
        public ServiceManager(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _companyService = new Lazy<ICompanyService> (()=>new ComapanyService(repository, logger,mapper));
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repository, logger, mapper));
        }
        public ICompanyService CompanyService => _companyService.Value;
    }
}
