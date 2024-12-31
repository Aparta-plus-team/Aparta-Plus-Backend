using data_aparta_.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_aparta_.Models;

namespace data_aparta_.Repos.Contracts
{
    public interface IInmuebleRepository
    {
        Task<Inmueble> GetInmuebleByIdAsync(Guid id);
        Task<IEnumerable<Inmueble>> GetAllInmueblesAsync();
        Task<IEnumerable<Inmueble>> GetInmueblesByEstadoAsync(bool ocupacion);
        Task AddInmuebleAsync(Inmueble inmueble);
        Task UpdateInmuebleAsync(Inmueble inmueble);
        Task DeleteInmuebleAsync(Guid id);
    }
}
