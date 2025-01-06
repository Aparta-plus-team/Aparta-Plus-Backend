using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using HotChocolate;

namespace aparta_.GraphQL
{
    [MutationType]
    public class FacturaMutation
    {
        // Create Factura
        public async Task<Factura> CreateFactura(
            Guid? inmuebleId,
            decimal? monto,
            DateOnly? fechaPago,
            string? estado,
            [Service] IFacturaRepository facturaRepository)
        {
            var factura = new Factura
            {
                Facturaid = Guid.NewGuid(),
                Inmuebleid = inmuebleId,
                Monto = monto,
                Fechapago = fechaPago,
                Estado = estado
            };

            await facturaRepository.AddFacturaAsync(factura);
            return factura;
        }

        // Update Factura
        public async Task<Factura> UpdateFactura(
            Guid facturaId,
            Guid? inmuebleId,
            decimal? monto,
            DateOnly? fechaPago,
            string? estado,
            [Service] IFacturaRepository facturaRepository)
        {
            var existingFactura = await facturaRepository.GetFacturaByIdAsync(facturaId);
            if (existingFactura == null)
            {
                throw new GraphQLException($"Factura with ID {facturaId} not found.");
            }

            if (inmuebleId.HasValue) existingFactura.Inmuebleid = inmuebleId;
            if (monto.HasValue) existingFactura.Monto = monto;
            if (fechaPago.HasValue) existingFactura.Fechapago = fechaPago;
            if (!string.IsNullOrEmpty(estado)) existingFactura.Estado = estado;

            await facturaRepository.UpdateFacturaAsync(existingFactura);
            return existingFactura;
        }

    }
}
