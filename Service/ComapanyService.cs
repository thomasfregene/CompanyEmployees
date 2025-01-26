using Contracts;
using Entities.Models;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class ComapanyService : ICompanyService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        public ComapanyService(ILoggerManager loggerManager, IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _logger = loggerManager;

        }
        public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
        {
            try
            {
                var companies = _repositoryManager.CompanyRepository.GetAllCompanies(trackChanges);
                var companiesDto = companies.Select(c => new CompanyDto(c.Id, c.Name ?? "", string.Join(' ', c.Address, c.Country))).ToList();
                return companiesDto;
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong in {nameof(GetAllCompanies)} service method {ex}");
                throw;
            }
        }
    }
}
