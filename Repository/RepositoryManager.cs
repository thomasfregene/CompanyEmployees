using Contracts;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        /*
         we are utilizing the 
         Lazy class to ensure the lazy initialization of our services
        */
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<ICompanyRepository> _companyRepository;
        private readonly Lazy<IEmployeeRepository> _employeeRepository;
        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _companyRepository = new Lazy<ICompanyRepository>(()=>new CompanyRepository(repositoryContext));
            _employeeRepository = new Lazy<IEmployeeRepository>(()=>new EmployeeRepository(repositoryContext));
        }
        public ICompanyRepository CompanyRepository => _companyRepository.Value;

        public IEmployeeRepository EmployeeRepository => _employeeRepository.Value;

        public void Save() => _repositoryContext.SaveChanges();
    }
}
