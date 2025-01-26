using Contracts;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        public ServiceManager(IRepositoryManager repository, ILoggerManager logger)
        {
            _companyService = new Lazy<ICompanyService> (()=>new ComapanyService(logger, repository));
        }
        public ICompanyService CompanyService => _companyService.Value;
    }
}
