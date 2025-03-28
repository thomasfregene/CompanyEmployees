using System.Dynamic;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,
            IEmployeeLinks employeeLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
            _repository.EmployeeRepository.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeForCompany = await _repository.EmployeeRepository.GetEmployeeAsync(companyId, id, trackChanges);
            if(employeeForCompany == null)
                throw new EmployeeNotFoundException(id);

            _repository.EmployeeRepository.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeDb = await _repository.EmployeeRepository.GetEmployeeAsync(companyId, employeeId, trackChanges);

            if(employeeDb is null)
                throw new EmployeeNotFoundException(employeeId);

            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync
            (Guid companyId, LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeWithMetaData = await _repository.EmployeeRepository.GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);

            var employeeDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeeWithMetaData);

            var links = _employeeLinks.TryGenerateLinks(employeeDto, linkParameters.EmployeeParameters.Fields,companyId,linkParameters.Context);

            return (linkResponse: links, metaData: employeeWithMetaData.MetaData);
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
           (Guid companyId, Guid id, bool compTrackChange, bool empTrackChage)
        {
            await CheckIfCompanyExists(companyId, compTrackChange);

            var employeeEntity = await _repository.EmployeeRepository.GetEmployeeAsync(companyId, id, empTrackChage);
            if (employeeEntity is null)
                throw new EmployeeNotFoundException(id);

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

            return (employeeToPatch, employeeEntity);
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, 
            bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employeeEntity = await _repository.EmployeeRepository.GetEmployeeAsync(companyId, id, empTrackChanges);
            if(employeeEntity is null)
                throw new EmployeeNotFoundException(id);

            _mapper.Map(employeeForUpdate, employeeEntity);
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExists(Guid compayId, bool trackChanges)
        {
            var company = await _repository.CompanyRepository.GetCompanyAsync(compayId, trackChanges);
            if(company is null)
                throw new CompanyNotFoundException(compayId);
        }
    }
}
