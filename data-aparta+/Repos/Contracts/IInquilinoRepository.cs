using Amazon.Util.Internal;
using data_aparta_.DTOs;
using data_aparta_.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Contracts
{
     public interface IInquilinoRepository
    {
        Task<Inquilino?> GetInquilinoByIdAsync(Guid id);
        Task<IEnumerable<Inquilino>> GetAllInquilinosAsync();
        Task AddInquilinoAsync(Inquilino inquilino);
        Task UpdateInquilinoAsync(Inquilino inquilino);
        Task DeleteInquilinoAsync(Guid id);
    }
}
