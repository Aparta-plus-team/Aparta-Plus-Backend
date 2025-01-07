using data_aparta_.Context;
using data_aparta_.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Payments
{
    public class InvoiceRepository
    {

        private readonly ApartaPlusContext _context;
        public InvoiceRepository(ApartaPlusContext context) { 
            _context = context;
        }

        public async Task<List<Factura>> GenerateMonthlyInvoices()
        {
            var invoices = new List<Factura>();
            var properties = await _context.Inmuebles.Include(x => x.Contrato).ToListAsync();

            foreach (var property in properties)
            {

                //Verificar si ya se generó la factura para este mes con pagos adelantados
                var createdInvoice = await _context.Facturas.FirstOrDefaultAsync(x =>
                   x.Inmuebleid == property.Inmuebleid &&
                   x.Fechapago.HasValue &&
                   x.Fechapago.Value.Month == DateTime.Now.Month &&
                   x.Fechapago.Value.Year == DateTime.Now.Year);

                var invoice = new Factura
                {
                    Facturaid = Guid.NewGuid(),
                    Inmuebleid = property.Inmuebleid,
                    Monto = property.Contrato.Precioalquiler,
                    Fechapago = DateOnly.FromDateTime(DateTime.Now),
                    Estado = "Pendiente"
                };

                invoices.Add(invoice);
            }

            return invoices;
        }


        public async Task<List<Factura>> CreateInvoicesInAdvanced(int monthAmount, string inmuebleId)
        {
            var invoices = new List<Factura>();
            var property = await _context.Inmuebles.Include(x => x.Contrato).FirstOrDefaultAsync(x => x.Inmuebleid == Guid.Parse(inmuebleId));

            for (int i = 0; i < monthAmount; i++)
            {
                var invoice = new Factura
                {
                    Facturaid = Guid.NewGuid(),
                    Inmuebleid = property.Inmuebleid,
                    Monto = property.Contrato.Precioalquiler,
                    Fechapago = DateOnly.FromDateTime(DateTime.Now.AddMonths(i)),
                    Estado = "Pendiente"
                };

                invoices.Add(invoice);
            }

            return invoices;
        }

    }
}
