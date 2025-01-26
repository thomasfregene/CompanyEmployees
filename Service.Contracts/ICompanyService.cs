﻿using Entities.Models;
using Shared.DataTransferObjects;

namespace Contracts
{
    public interface ICompanyService
    {
        IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);
    }
}
