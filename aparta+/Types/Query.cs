using aparta_.Services;
using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using HotChocolate.Data;
using data_aparta_.Repos;

namespace aparta_.Types
{
    [QueryType]
    public static class Query
    {
        public static Book GetBook()
            => new Book("C# in depth.", new Author("Jon Skeet"));

        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public static async Task<IQueryable<Factura>> GetFacturas(ApartaPlusContext dbContext) =>
            dbContext.Facturas;

        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public static async Task<IQueryable<Usuario>> GetUsuarios(ApartaPlusContext dbContext) =>
            dbContext.Usuarios;

        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public static IQueryable<Inmueble> GetInmuebles(ApartaPlusContext dbContext) =>
            dbContext.Inmuebles;

        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public static IQueryable<Contrato> GetContratos(ApartaPlusContext dbContext) =>
            dbContext.Contratos;

        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public static IQueryable<Inquilino> GetInquilinos(ApartaPlusContext dbContext) =>
            dbContext.Inquilinos;

        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public static IQueryable<Propiedad> GetPropiedads(ApartaPlusContext dbContext) =>
            dbContext.Propiedads;

        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public static async Task<List<ReporteMorosidadDto>> GetReporteMorosidadPorAnio(
            int anio,
            Guid userId,
            Guid? propertyId,
            [Service] ReporteMorosidadRepository reporteMorosidadRepository)
        {
            return await reporteMorosidadRepository.GetReporteMorosidadPorUsuarioPropiedadYAnioAsync(userId, propertyId, anio);
        }

        public static Task<DashboardStatisticsDTO> GetDashboardStatistics(
            Guid userId,
            [Service] DashboardStatisticsService dashboardService)
        {
            return dashboardService.GetDashboardStatisticsAsync(userId);
        }

        // Query para Reporte de Ventas Anual
        public static Task<List<ReporteVentasDto>> GetReporteVentasAsync(
        [Service] ReporteVentasRepository repository,
        Guid userId,
        int year)
        {
            return repository.GetReporteVentasAnual(userId, year);
        }

        // Query para Ganancia por Inmueble
        public static Task<List<GananciaPropiedadDto>> GananciaPropiedads(
            Guid userId,
            [Service] GananciaPropiedadRepository gananciaInmuebleRepository)
        {
            return gananciaInmuebleRepository.GetGananciaPorPropiedad(userId);
        }

        public static Task<List<InquilinoDeudaDto>> GetInquilinosConDeudas(
        Guid userId,
        [Service] InquilinoDeudaRepository inquilinoRepository)
        {
            return inquilinoRepository.GetInquilinosConDeudasAsync(userId);
        }

        public static Task<List<EstadisticaFinancieraDto>> GetEstadisticaFinanciera(
            Guid userId,
        [Service] EstadisticaFinancieraRepository estadisticaFinancieraRepository)
        {
            return estadisticaFinancieraRepository.GetEstadisticaFinancieraAsync(userId);
        }

        public static Task<List<MorosidadPorPropiedadDto>> GetMorosidadsInquilino(
        Guid userId,
        [Service] MorosidadPorPropiedadRepository morosidadPorPropiedadRepository)
        {
            return morosidadPorPropiedadRepository.GetMorosidadPorUsuarioAsync(userId);
        }




    }


}
