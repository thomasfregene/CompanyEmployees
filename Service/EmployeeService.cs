using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public EmployeeDto GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
        {
            var company = _repository.CompanyRepository.GetCompany(companyId, false);

            if (company == null) 
                throw new CompanyNotFoundException(companyId);
            var employeeDb = _repository.EmployeeRepository.GetEmployee(companyId, employeeId, trackChanges);

            if(employeeDb == null)
                throw new EmployeeNotFoundException(employeeId);

            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }

        public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
        {
            var company = _repository.CompanyRepository.GetCompany(companyId, trackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeFromDb = _repository.EmployeeRepository.GetEmployees(companyId, trackChanges);

            var employeeDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeeFromDb);

            return employeeDto;
        }
    }
}
