using data_aparta_.Repos.Reportes;
using Microsoft.AspNetCore.Mvc;

namespace aparta_.Types
{

    [MutationType]
    public class ReportesMutations
{
        public async Task<string> DescargarReportePagos([Service] ReportesRepository reportes, string propiedadId, int year)
        {
            try
            {
                return await reportes.GenerateReport(propiedadId, year);
            }catch(Exception e)
            {
                throw new GraphQLException(e.Message);
            }

        }

        public async Task<string> DescargarReporteIngresoMorosidad([Service] ReportesRepository reportes, string year, string userId)
        {
            try
            {
                return await reportes.GenerateReporteIngresosMorosidad(year, userId);
            }catch(Exception e)
            {
                throw new GraphQLException(e.Message);
            }

        }
        
    }
}
