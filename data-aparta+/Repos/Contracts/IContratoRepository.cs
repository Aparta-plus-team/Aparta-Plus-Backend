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
    public interface IContratoRepository
    {
        Task<Contrato?> GetContratoByIdAsync(Guid id);
        Task<IEnumerable<Contrato>> GetAllContratosAsync();
        Task AddContratoAsync(Contrato contrato);
        Task UpdateContratoAsync(Contrato contrato);
        Task DeleteContratoAsync(Guid id);
    }
}
