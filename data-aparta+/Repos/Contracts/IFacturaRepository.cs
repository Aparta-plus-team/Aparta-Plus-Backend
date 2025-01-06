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
    public interface IFacturaRepository
    {
        Task<Factura?> GetFacturaByIdAsync(Guid id);
        Task<IEnumerable<Factura>> GetAllFacturasAsync();
        Task AddFacturaAsync(Factura factura);
        Task UpdateFacturaAsync(Factura factura);
    }
}
