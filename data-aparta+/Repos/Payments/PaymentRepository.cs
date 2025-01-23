using Amazon.S3.Model.Internal.MarshallTransformations;
using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Payments
{
    public class PaymentRepository : IPaymentRepository, IAsyncDisposable
    {

        private readonly StripeService _stripeService;
        private readonly EmailService emailService;
        private ApartaPlusContext? _context;
        private readonly IDbContextFactory<ApartaPlusContext> _dbContextFactory;

        public PaymentRepository(StripeService stripeService, IDbContextFactory<ApartaPlusContext> dbContextFactory, EmailService email) {
            _dbContextFactory = dbContextFactory;
            _stripeService = stripeService;
            emailService = email;
            _context = dbContextFactory.CreateDbContext();
        }

        public async Task<StripeSessionResponse> CreatePaymentSession(int quantity, string inmuebleId)
        {
            EnsureContext();
            var contract = await GetContract(inmuebleId);
            if (quantity == 0) quantity = 1;
            string description = "";

            if (contract == null) return null;

            if (await CheckPaid(inmuebleId))
            {
                return new StripeSessionResponse
                {
                    SessionId = "Pagado",
                    Url = "success web"

                };
            }

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
            EnsureContext();
            var invoices = await _context.Facturas.Where(f => f.SessionId == sessionId)
                .Include(f => f.Inmueble)
                .Include(f => f.Inmueble.Contrato)
                .Include(f => f.Inmueble.Contrato.Inquilino)
                .ToListAsync();

            Console.WriteLine(invoices);

            foreach (var invoice in invoices)
            {

                if (invoice.Estado == "Pendiente Adelanto")
                {
                    invoice.Estado = "Por Adelantado";
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
            await emailService.EnviarFacturasAsync(invoices.FirstOrDefault().Inmueble.Contrato.Inquilino.Inquilinocorreo, invoices);

            // Procesar el pago
            return true;
        }

        public async Task<decimal> CalcularDeudaTotal(string inmuebleId)
        {
            EnsureContext();
            var inmueble = await _context.Inmuebles
                .Include(i => i.Contrato)
                .FirstOrDefaultAsync(i => i.Inmuebleid == Guid.Parse(inmuebleId));

            if (inmueble == null || inmueble.Contrato == null)
            {
                throw new Exception("Inmueble o contrato no encontrado");
            }

            // Obtener facturas pendientes y atrasadas
            var facturasPendientes = await _context.Facturas
                .Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) &&
                       (f.Estado == "No Pagado" || f.Estado == "Pendiente" || f.Estado == "Atrasado"))
                .OrderBy(f => f.Fechapago)
                .ToListAsync();

            decimal deudaTotal = 0;

            foreach (var factura in facturasPendientes)
            {
                decimal montoFactura = factura.Monto ?? 0;

                // Agregar mora si la factura está atrasada
                if (factura.Estado == "Atrasado")
                {
                    var porcentajeMora = inmueble.Contrato.Mora ?? 2;
                    montoFactura += montoFactura * (porcentajeMora / 100);
                }

                deudaTotal += montoFactura;
            }

            // Si no hay deuda pero el mes actual está pendiente, agregar el monto del alquiler
            if (deudaTotal == 0)
            {
                var facturaActual = await _context.Facturas
                    .AnyAsync(f => f.Inmuebleid == Guid.Parse(inmuebleId) &&
                           (f.Estado == "Pagado" || f.Estado == "Por Adelantado") &&
                           f.Fechapago.Value.Month == DateTime.Now.Month &&
                           f.Fechapago.Value.Year == DateTime.Now.Year);

                if (!facturaActual)
                {
                    deudaTotal = inmueble.Contrato.Precioalquiler ?? 0;

                    // Si ya pasó el día de pago, agregar mora
                    if (DateTime.Now.Day > inmueble.Contrato.Diapago)
                    {
                        var porcentajeMora = inmueble.Contrato.Mora ?? 2;
                        deudaTotal += deudaTotal * (porcentajeMora / 100);
                    }
                }
            }

            return deudaTotal;
        }

        public async Task<PaymentStatusResponse> GetPaymentStatus(string inmuebleId)
        {
            EnsureContext();
            var response = new PaymentStatusResponse();
            response.InmuebleId = inmuebleId;
            var contract = await GetContract(inmuebleId);

            if (contract == null)
            {
                throw new Exception("No se encontró contrato para este inmueble");
            }

            // Verificar si ya pagó el mes actual
            response.IsCurrentMonthPaid = await CheckPaid(inmuebleId);

            // Verificar si tiene factura pendiente del mes actual
            var currentMonthPending = await _context.Facturas
                .AnyAsync(f => f.Inmuebleid == Guid.Parse(inmuebleId)
                    && f.Estado == "Pendiente"
                    && f.Fechapago.Value.Month == DateTime.Now.Month
                    && f.Fechapago.Value.Year == DateTime.Now.Year);

            response.HasPendingCurrentMonth = currentMonthPending;

            // Verificar deudas
            var debts = await _context.Facturas
                .Where(f => f.Inmuebleid == Guid.Parse(inmuebleId)
                    && f.Estado == "No Pagado" || f.Estado == "Pendiente")
                .ToListAsync();

            response.HasDebt = debts.Any();
            if (response.HasDebt)
            {
                response.DebtMonths = debts.Count;
                response.DebtAmount = debts.Sum(d => d.Monto ?? 0);

                // Calcular el monto total incluyendo la mora
                decimal moraPercentage = contract.Mora ?? 2;
                response.DebtAmount += response.DebtAmount * (moraPercentage / 100);
            }

            // Verificar si tendrá mora
            response.WillHaveMora = await IsInMora(contract, inmuebleId);
            if (response.WillHaveMora)
            {
                response.MoraAmount = await CalculateMora(contract, inmuebleId);
            }
            return response;
        }

        public async Task<ManualPaymentResponse> ProcessManualPayment(ManualPaymentRequest request)
        {
            EnsureContext();

            var deudaTotal = await CalcularDeudaTotal(request.InmuebleId);

            if (deudaTotal == 0)
            {
                return new ManualPaymentResponse
                {
                    Success = false,
                    DeudaTotal = 0,
                    Message = "No hay deuda pendiente para este inmueble"
                };
            }
            
            var inmueble = await _context.Inmuebles
                .Include(i => i.Contrato)
                    .ThenInclude(c => c.Inquilino)
                .FirstOrDefaultAsync(i => i.Inmuebleid == Guid.Parse(request.InmuebleId));

            var facturasPendientes = await _context.Facturas
                .Where(f => f.Inmuebleid == Guid.Parse(request.InmuebleId) &&
                       (f.Estado == "No Pagado" || f.Estado == "Pendiente"))
                .OrderBy(f => f.Fechapago)
                .ToListAsync();

            // Si no hay facturas pendientes pero hay deuda, crear la factura del mes actual
            if (!facturasPendientes.Any())
            {
              /*  var nuevaFactura = new Factura
                {
                    Facturaid = Guid.NewGuid(),
                    Inmuebleid = Guid.Parse(request.InmuebleId),
                    Monto = deudaTotal,
                    Estado = "Pagado",
                    Fechapago = DateOnly.FromDateTime(DateTime.Now),
                    Descripcion = "Pago presencial",
                    Inmueble = inmueble
                };

                await _context.Facturas.AddAsync(nuevaFactura);
                facturasPendientes = new List<Factura> { nuevaFactura };*/
            }
            else
            {
                // Marcar todas las facturas pendientes como pagadas
                foreach (var factura in facturasPendientes)
                {
                    factura.Estado = "Pagado";
                    factura.Descripcion = "Pago presencial";
                }
            }

            await _context.SaveChangesAsync();

            // Enviar correo de confirmación
            await emailService.EnviarFacturasAsync(
                inmueble.Contrato.Inquilino.Inquilinocorreo,
                facturasPendientes
            );

            return new ManualPaymentResponse
            {
                Success = true,
                DeudaTotal = deudaTotal,
                Message = "Pago procesado exitosamente",
                InmuebleId = request.InmuebleId
            };
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusByInquilino(string inquilinoId)
        {
            EnsureContext();
            var response = new PaymentStatusResponse();

            // Obtener todos los inmuebles asociados al inquilino
            var inmuebles = await _context.Inmuebles
                .Include(i => i.Contrato)
                .Where(i => i.Contrato.Inquilinoid == Guid.Parse(inquilinoId))
                .ToListAsync();

            if (!inmuebles.Any())
            {
                throw new Exception("No se encontraron inmuebles para este inquilino");
            }

            response.IsCurrentMonthPaid = true;
            response.HasDebt = false;
            response.DebtMonths = 0;
            response.DebtAmount = 0;
            response.WillHaveMora = false;
            response.MoraAmount = 0;

            foreach (var inmueble in inmuebles)
            {
                // Verificar si ya pagó el mes actual para cada inmueble
                if (!await CheckPaid(inmueble.Inmuebleid.ToString()))
                {
                    response.IsCurrentMonthPaid = false;
                }

                // Verificar si tiene factura pendiente del mes actual para cada inmueble
                var currentMonthPending = await _context.Facturas
                    .AnyAsync(f => f.Inmuebleid == inmueble.Inmuebleid
                        && f.Estado == "Pendiente"
                        && f.Fechapago.Value.Month == DateTime.Now.Month
                        && f.Fechapago.Value.Year == DateTime.Now.Year);

                response.HasPendingCurrentMonth = response.HasPendingCurrentMonth || currentMonthPending;

                // Verificar deudas para cada inmueble
                var debts = await _context.Facturas
                    .Where(f => f.Inmuebleid == inmueble.Inmuebleid
                        && (f.Estado == "No Pagado" || f.Estado == "Pendiente"))
                    .ToListAsync();

                if (debts.Any())
                {
                    response.HasDebt = true;
                    response.DebtMonths += debts.Count;
                    response.DebtAmount += debts.Sum(d => d.Monto ?? 0);

                    // Calcular el monto total incluyendo la mora
                    decimal moraPercentage = inmueble.Contrato.Mora ?? 2;
                    response.DebtAmount += response.DebtAmount * (moraPercentage / 100);
                }

                // Verificar si tendrá mora
                if (await IsInMora(inmueble.Contrato, inmueble.Inmuebleid.ToString()))
                {
                    response.WillHaveMora = true;
                    response.MoraAmount += await CalculateMora(inmueble.Contrato, inmueble.Inmuebleid.ToString());
                }
            }

            return response;
        }

        public async Task<ManualPaymentResponse> ProcessManualPaymentByInquilino(string inquilinoId)
        {
            EnsureContext();
            var inmuebles = await _context.Inmuebles
                .Include(i => i.Contrato)
                    .ThenInclude(c => c.Inquilino)
                .Where(i => i.Contrato.Inquilinoid == Guid.Parse(inquilinoId))
                .ToListAsync();

            if (!inmuebles.Any())
            {
                return new ManualPaymentResponse
                {
                    Success = false,
                    DeudaTotal = 0,
                    Message = "No se encontraron inmuebles para este inquilino"
                };
            }

            decimal deudaTotal = 0;
            var facturasPendientes = new List<Factura>();

            foreach (var inmueble in inmuebles)
            {
                deudaTotal += await CalcularDeudaTotal(inmueble.Inmuebleid.ToString());

                var facturas = await _context.Facturas
                    .Where(f => f.Inmuebleid == inmueble.Inmuebleid &&
                           (f.Estado == "No Pagado" || f.Estado == "Pendiente"))
                    .OrderBy(f => f.Fechapago)
                    .ToListAsync();

                facturasPendientes.AddRange(facturas);

                // Si no hay facturas pendientes pero hay deuda, crear la factura del mes actual
                if (!facturas.Any() && deudaTotal > 0)
                {
                    var nuevaFactura = new Factura
                    {
                        Facturaid = Guid.NewGuid(),
                        Inmuebleid = inmueble.Inmuebleid,
                        Monto = deudaTotal,
                        Estado = "Pagado",
                        Fechapago = DateOnly.FromDateTime(DateTime.Now),
                        Descripcion = "Pago presencial",
                        Inmueble = inmueble
                    };

                    await _context.Facturas.AddAsync(nuevaFactura);
                    facturasPendientes.Add(nuevaFactura);
                }
                else
                {
                    // Marcar todas las facturas pendientes como pagadas
                    foreach (var factura in facturas)
                    {
                        factura.Estado = "Pagado";
                        factura.Descripcion = "Pago presencial";
                    }
                }
            }

            if (deudaTotal == 0)
            {
                return new ManualPaymentResponse
                {
                    Success = false,
                    DeudaTotal = 0,
                    Message = "No hay deuda pendiente para este inquilino"
                };
            }

            await _context.SaveChangesAsync();

            // Enviar correo de confirmación
            var inquilino = inmuebles.First().Contrato.Inquilino;
            await emailService.EnviarFacturasAsync(
                inquilino.Inquilinocorreo,
                facturasPendientes
            );

            return new ManualPaymentResponse
            {
                Success = true,
                DeudaTotal = deudaTotal,
                Message = "Pago procesado exitosamente"
            };
        }

        #region Private Methods
        private async Task<bool> IsInDebt(string inmuebleId)
        {
            EnsureContext();
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
            EnsureContext();

            if (contract.Diapago < DateTime.Now.Day)
            {
                return true;
            }
            return false;
        }

        private async Task<Contrato> GetContract(string inmuebleId)
        {
            EnsureContext();

            var inmueble = await _context.Inmuebles.Where(i => i.Inmuebleid == Guid.Parse(inmuebleId))
                .Include(i => i.Contrato).FirstOrDefaultAsync();


            var inmueble2 = await _context.Inmuebles.FirstOrDefaultAsync();
            Console.WriteLine(inmueble2.Codigo);

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
            EnsureContext();
            var debts = await _context.Facturas.Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) && f.Estado == "No Pagado").ToListAsync();
            decimal debt = 0;

            foreach (var factura in debts)
            {
                debt += factura.Monto ?? 0;
            }

            debt = debt + ( debt * (contract.Mora / 100) ?? 2);

            return new Debt { debt = debt, quantity = debts.Count };

        }

        private async Task<bool> CheckPaid(string inmuebleId)
        {
            EnsureContext();
            var pid = await _context.Facturas.Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) 
            && (f.Estado == "Pagado" || f.Estado == "Por Adelantado") && f.Fechapago.Value.Month == DateTime.Now.Month &&
                   f.Fechapago.Value.Year == DateTime.Now.Year).ToListAsync();

            if (pid.Count > 0 && await IsInDebt(inmuebleId) == false)
            {
                return true;
            }

            return false;
        }

        private async Task<decimal> CalculateMora(Contrato contract, string inmuebleId)
        {
            EnsureContext();
            var debts = await _context.Facturas.Where(f => f.Inmuebleid == Guid.Parse(inmuebleId) && f.Estado == "No Pagado" || f.Estado == "Pendiente").ToListAsync();
            decimal debt = 0;

            foreach (var factura in debts)
            {
                debt += factura.Monto ?? 0;
            }

            decimal result = debt + (contract.Precioalquiler ?? 1) + ((contract.Precioalquiler ?? 1) * ( 1 + (contract.Mora / 100) ?? 2));


            return result;

        }

        private void EnsureContext()
        {
            if (_context == null)
            {
                _context = _dbContextFactory.CreateDbContext();
            }
        }

        // Implementación de IAsyncDisposable
        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
                _context = null;
            }
        }
        #endregion


    }
}
