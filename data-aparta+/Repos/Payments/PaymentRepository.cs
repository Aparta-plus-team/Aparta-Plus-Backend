using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Payments
{
    public class PaymentRepository : IPaymentRepository
    {

        private readonly StripeService _stripeService;
        private readonly ApartaPlusContext _context;

        public PaymentRepository(StripeService stripeService, ApartaPlusContext apartaPlusContext) { 
            _stripeService = stripeService;
            _context = apartaPlusContext;
        }

        public async Task<StripeSessionResponse> CreatePaymentSession(int quantity, string inmuebleId)
        {
            var contract = await GetContract(inmuebleId);
            string description = "";

            if (contract == null) return null;

            //Ver si hay deudas, y, hacer la sesionde pago con la deuda de todas las facturas NO PAGADAS
            if (await IsInDebt(inmuebleId))
            {
                var debt = await CalculateDebt(contract, inmuebleId);
                quantity = 1;
                description = "Deuda correspondiente a " + debt.quantity + " meses pendientes adicionando la mora";

                var debtResponse = await _stripeService.CreatePaymentSession(debt.debt, description, quantity, inmuebleId);
                return debtResponse;
            }

            //Si no hay deuda, ver si hay que aplicar mora por deuda PENDIENTE
            if (await IsInMora(contract, inmuebleId))
            {
                var mora = await CalculateMora(contract, inmuebleId);
                quantity = 1;
                description = "Pago de renta con mora correspondiente a " + mora + " por atraso";

                var moraResponse = await _stripeService.CreatePaymentSession(mora, description, quantity, inmuebleId);
                return moraResponse;
            }

            //Si no hay mora, crear la sesion de pago con pagos adelantados (si es requerido)
            description = "Pago de renta correspondiente a " + quantity + " mes/es por adelantado";
            var response = await _stripeService.CreatePaymentSession(contract.Precioalquiler ?? 0, description, quantity, inmuebleId);
            return response;
        }


        public async Task<bool> ProcessPayment(string sessionId)
        {
            var invoices = await _context.Facturas.Where(f => f.SessionId == sessionId)
                .Include(f => f.Inmueble)
                .Include(f => f.Inmueble.Contrato)
                .ToListAsync();

            foreach (var invoice in invoices)
            {

                if (invoice.Estado == "Pendiente Adelanto")
                {
                    invoice.Estado = "Adelantado";
                    await _context.SaveChangesAsync();
                    continue;
                }
                
                if (invoice.Inmueble.Contrato.Diapago > DateTime.Now.Day && invoice.Estado == "Pendiente")
                {
                    invoice.Estado = "Atrasado";
                }
                else
                {
                    invoice.Estado = "Pagado";
                }

                var delayedInvoices = await _context.Facturas.Where(f => f.Inmuebleid == invoice.Inmuebleid && f.Estado == "No Pagado")
                    .ToListAsync();
                foreach (var delayed in delayedInvoices)
                {
                    delayed.Estado = "Cancelado";
                }
                await _context.SaveChangesAsync();
            }
            // Procesar el pago
            return true;
        }


        #region Private Methods
        private async Task<bool> IsInDebt(string inmuebleId)
        {
            // Ver si hay deudas asociadas al contrato
            var debts = await _context.Facturas.Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) && f.Estado == "No Pagado").ToListAsync();

            //Colocar las facturas como vencidas

            if (debts.Count > 0)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> IsInMora(Contrato contract, string inmuebleId)
        {
            // Ver si hay deudas asociadas al contrato

            var debts = await _context.Facturas.Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) && f.Estado == "Atrasado").ToListAsync();
            DateTime fechaPago = new DateTime(DateTime.Now.Year, DateTime.Now.Month, contract.Diapago ?? 15);

            foreach (var debt in debts)
            {
                if (DateTime.Now.AddDays(5) > fechaPago) //Agregar dias de penalizacion en la DB
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<Contrato> GetContract(string inmuebleId)
        {
            var inmueble = await _context.Inmuebles.Where(i => i.Inmuebleid == Guid.Parse(inmuebleId)).Include(i => i.Contrato).FirstOrDefaultAsync();

            if (inmueble.Contrato == null)
            {
                return null;
            }

            return inmueble.Contrato;
        }

        struct Debt
        {
            public decimal debt { get; set; }
            public int quantity { get; set; }
        }

        private async Task<Debt> CalculateDebt(Contrato contract, string inmuebleId)
        {
            var debts = await _context.Facturas.Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) && f.Estado == "No Pagado").ToListAsync();
            decimal debt = 0;

            foreach (var factura in debts)
            {
                debt += factura.Monto ?? 0;
            }

            debt = debt * ((contract.Mora / 100) ?? 2);

            return new Debt { debt = debt, quantity = debts.Count };

        }

        private async Task<decimal> CalculateMora(Contrato contract, string inmuebleId)
        {
            var debts = await _context.Facturas.Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) && f.Estado == "Atrasado").ToListAsync();
            decimal debt = 0;

            foreach (var factura in debts)
            {
                debt += factura.Monto ?? 0;
            }

            return debt * ((contract.Mora / 100) ?? 2);

        }
        #endregion


    }
}
