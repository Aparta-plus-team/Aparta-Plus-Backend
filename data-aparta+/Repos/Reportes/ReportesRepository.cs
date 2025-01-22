using data_aparta_.Context;
using data_aparta_.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Reportes
{
    public class ReportesRepository : IAsyncDisposable
    {
        private readonly ApartaPlusContext _context;

        public ReportesRepository(IDbContextFactory<ApartaPlusContext> dbContextFactory)
        {
            _context = dbContextFactory.CreateDbContext();
        }

        public async Task<string> GenerateReport(string propiedadId, int year)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Obtener la propiedad
            var propiedad = await _context.Propiedads.FindAsync(Guid.Parse(propiedadId));
            if (propiedad == null)
            {
                throw new Exception("No se encontró la propiedad.");
            }

            // Obtener los inmuebles asociados a la propiedad
            var inmuebles = _context.Inmuebles
                .Where(i => i.Propiedadid == Guid.Parse(propiedadId))
                .ToList();

            if (!inmuebles.Any())
            {
                throw new Exception("No se encontraron inmuebles para esta propiedad.");
            }

            // Obtener las facturas asociadas a los inmuebles y al año dado
            var facturas = _context.Facturas
                .Where(f => inmuebles.Select(i => i.Inmuebleid).Contains(f.Inmuebleid.Value) &&
                            f.Fechapago.HasValue &&
                            f.Fechapago.Value.Year == year)
                .ToList();

            // Crear el documento PDF
            var fileName = $"Reporte_Propiedad_{propiedad.Nombre}_{year}.pdf";
            var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    // Encabezado
                    page.Header().Column(column =>
                    {
                        column.Spacing(5); // Espaciado entre los elementos
                        column.Item().AlignCenter().Text($"Reporte de Morosidad").FontSize(24).Bold().FontColor(Colors.Blue.Darken3);
                        column.Item().AlignCenter().Text($"{propiedad.Nombre} - {year}").FontSize(18).Italic();
                    });

                    // Contenido
                    page.Content().Column(column =>
                    {
                        column.Spacing(20);

                        // Tabla principal
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Columna más ancha para el nombre del inmueble
                                foreach (var _ in Enumerable.Range(1, 12))
                                    columns.RelativeColumn(1); // Columnas iguales para los meses
                            });

                            // Encabezados de tabla
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Inmueble").Bold().AlignCenter();
                                foreach (var month in Enumerable.Range(1, 12))
                                    header.Cell().Background(Colors.Blue.Lighten3).Text(new DateTime(year, month, 1).ToString("MMM")).Bold().AlignCenter();
                            });

                            // Filas de la tabla
                            foreach (var inmueble in inmuebles)
                            {
                                table.Cell().BorderBottom(1).Text($"{inmueble.Codigo}").Bold();

                                foreach (var month in Enumerable.Range(1, 12))
                                {
                                    var factura = facturas.FirstOrDefault(f =>
                                        f.Inmuebleid == inmueble.Inmuebleid &&
                                        f.Fechapago.HasValue &&
                                        f.Fechapago.Value.Month == month);

                                    var estado = factura?.Estado ?? "N/A";
                                    var color = estado switch
                                    {
                                        "Pendiente" or "Pendiente Adelanto" => Colors.Yellow.Lighten1,
                                        "Cancelado" or "No pagado" or "Atrasado" => Colors.Red.Lighten1,
                                        "Pagado" => Colors.Green.Lighten1,
                                        "Por Adelantado" => Colors.Blue.Lighten1,
                                        _ => Colors.Grey.Lighten3
                                    };

                                    table.Cell().Background(color).BorderBottom(1).MinHeight(20).AlignCenter();
                                }
                            }
                        });

                        // Leyenda
                        column.Item().AlignCenter().Row(row =>
                        {
                            row.Spacing(10);

                            void AddLegendItem(string text, string color)
                            {
                                row.RelativeItem().Row(legendItem =>
                                {
                                    legendItem.ConstantItem(10).Background(color).Height(10).Width(10); // Cuadro indicativo
                                    legendItem.RelativeItem().AlignLeft().Text(text).FontSize(10);     // Texto al lado del cuadro
                                });
                            }

                            AddLegendItem("Pendiente, Pendiente Adelanto", Colors.Yellow.Lighten3);
                            AddLegendItem("Cancelado, No pagado, Atrasado", Colors.Red.Lighten3);
                            AddLegendItem("Pagado", Colors.Green.Lighten3);
                            AddLegendItem("Por Adelantado", Colors.Blue.Lighten3);
                        });
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text($"Generado el: {DateTime.Now:dd/MM/yyyy}").FontSize(10).Italic();
                });
            }).GeneratePdf(filePath);

            return filePath; // Retorna la ruta del archivo generado
        }


        public async Task<string> GenerateReporteIngresosMorosidad(string year, string userId)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Obtener las propiedades del usuario
            var propiedades = _context.Propiedads.Where(p => p.Usuarioid == Guid.Parse(userId)).ToList();
            if (!propiedades.Any())
            {
                throw new Exception("El usuario no tiene propiedades registradas.");
            }

            var inmuebles = _context.Inmuebles
               .Where(i => i.Propiedadid.HasValue && propiedades.Select(p => p.Propiedadid).Contains(i.Propiedadid.Value))
               .ToList();

            var facturas = _context.Facturas
                .Where(f => f.Fechapago.HasValue && f.Fechapago.Value.Year == int.Parse(year))
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Contrato)
                .ThenInclude(c => c.Inquilino)
                .Where(f => inmuebles.Select(i => i.Inmuebleid).Contains(f.Inmuebleid.Value))
                .ToList();

            var inquilinos = _context.Inquilinos.ToList();

            var fileName = $"Reporte_Ingresos_Morosidad_{year}_{userId}.pdf";
            var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    // Encabezado
                    page.Header().Column(column =>
                    {
                        column.Spacing(5);
                        column.Item().AlignCenter().Text("Reporte de Ingresos y Morosidad").FontSize(24).Bold().FontColor(Colors.Blue.Darken3);
                        column.Item().AlignCenter().Text($"Año {year}").FontSize(18).Italic();
                    });

                    // Contenido
                    page.Content().Column(column =>
                    {
                        column.Spacing(20);

                        // Reporte de Ingresos
                        column.Item().Text("1. Reporte de Ingresos").FontSize(16).Bold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4); // Propiedad
                                columns.RelativeColumn(2); // Ingresos Mensuales
                                columns.RelativeColumn(2); // Ingresos Anuales
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Propiedad").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Promedio Ingresos Mensuales").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Ingresos Anuales").Bold().AlignLeft();
                            });

                            foreach (var propiedad in propiedades)
                            {
                                var inmueblesPropiedad = inmuebles.Where(i => i.Propiedadid == propiedad.Propiedadid).ToList();
                                var facturasPropiedad = facturas.Where(f => inmueblesPropiedad.Select(i => i.Inmuebleid).Contains(f.Inmuebleid.Value)).ToList();
                                var ingresosMensuales = facturasPropiedad.GroupBy(f => f.Fechapago.Value.Month).Select(g => g.Sum(f => f.Monto));
                                var ingresosAnuales = facturasPropiedad.Sum(f => f.Monto);

                                table.Cell().Text(propiedad.Nombre);
                                table.Cell().Text($"${ingresosMensuales.Average():0.00}");
                                table.Cell().Text($"${ingresosAnuales:0.00}");
                            }
                        });

                        // Reporte de Morosidad
                        column.Item().Text("2. Reporte de Morosidad").FontSize(16).Bold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Propiedad
                                columns.RelativeColumn(2); // Facturas a Tiempo
                                columns.RelativeColumn(2); // Facturas Morosas
                                columns.RelativeColumn(2); // Porcentaje de Morosidad
                                columns.RelativeColumn(2); // Deuda Total Mensual
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Propiedad").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Facturas a Tiempo").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Facturas Morosas").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("% Morosidad").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Deuda Mensual").Bold().AlignLeft();
                            });

                            foreach (var propiedad in propiedades)
                            {
                                var inmueblesPropiedad = inmuebles.Where(i => i.Propiedadid == propiedad.Propiedadid).ToList();
                                var facturasPropiedad = facturas.Where(f => inmueblesPropiedad.Select(i => i.Inmuebleid).Contains(f.Inmuebleid.Value)).ToList();

                                var facturasATiempo = facturasPropiedad.Count(f => f.Estado == "Pagado");
                                var facturasMorosas = facturasPropiedad.Count(f => f.Estado == "Pendiente" || f.Estado == "Atrasado");
                                var porcentajeMorosidad = (facturasMorosas / (double)(facturasATiempo + facturasMorosas)) * 100;
                                var deudaTotal = facturasPropiedad.Where(f => f.Estado == "Pendiente" || f.Estado == "Atrasado").Sum(f => f.Monto);

                                table.Cell().Text(propiedad.Nombre);
                                table.Cell().Text(facturasATiempo.ToString());
                                table.Cell().Text(facturasMorosas.ToString());
                                table.Cell().Text($"{porcentajeMorosidad:0.00}%");
                                table.Cell().Text($"${deudaTotal:0.00}");
                            }
                        });

                        // Inquilinos con mayor morosidad
                        column.Item().Text("3. Inquilinos con Mayor Morosidad").FontSize(16).Bold();
                        var inquilinosMorosos = facturas
                            .Where(f => f.Estado == "Pendiente" || f.Estado == "Atrasado")
                            .GroupBy(f => f.Inmueble.Contrato.Inquilinoid)
                            .Select(g => new
                            {
                                Inquilino = inquilinos.FirstOrDefault(i => i.Inquilinoid == g.Key),
                                DeudaTotal = g.Sum(f => f.Monto),
                                PagosAtrasados = g.Count(f => f.Estado == "Atrasado")
                            })
                            .OrderByDescending(x => x.DeudaTotal)
                            .Take(5)
                            .ToList();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4); // Inquilino
                                columns.RelativeColumn(3); // Deuda Total
                                columns.RelativeColumn(3); // Pagos Atrasados
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Inquilino").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Deuda Total").Bold().AlignLeft();
                                header.Cell().Background(Colors.Blue.Lighten3).Text("Pagos Atrasados").Bold().AlignLeft();
                            });

                            if (inquilinosMorosos.Count == 0)
                            {
                                table.Cell().RowSpan(3).Text("No hay inquilinos morosos.").Italic().AlignCenter();
                            }

                            foreach (var item in inquilinosMorosos)
                            {
                                table.Cell().Text(item.Inquilino?.Inquilinonombre ?? "Desconocido");
                                table.Cell().Text($"${item.DeudaTotal:0.00}");
                                table.Cell().Text(item.PagosAtrasados.ToString());
                            }
                        });
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text($"Generado el: {DateTime.Now:dd/MM/yyyy}").FontSize(10).Italic();
                });
            }).GeneratePdf(filePath);

            return filePath;
        }


        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }
    }
}