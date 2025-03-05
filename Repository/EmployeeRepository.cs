using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(employeeId), trackChanges).SingleOrDefaultAsync();

        public async Task<PageList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChange)
        {
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId) 
            && (e.Age >= employeeParameters.MinAge && e.Age <= employeeParameters.MaxAge), trackChange)
            .OrderBy(e => e.Name)
            .ToListAsync();
            /*var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChange)
             .OrderBy(e => e.Name)
             .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
             .Take(employeeParameters.PageSize)
             .ToListAsync();*/

            var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChange).CountAsync();

            return PageList<Employee>.ToPageList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);
            //return new PageList<Employee>(employees, count, employeeParameters.PageNumber, employeeParameters.PageSize);
        }


        public void DeleteEmployee(Employee employee) => Delete(employee);
    }
}
