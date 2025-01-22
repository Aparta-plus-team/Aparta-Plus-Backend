using data_aparta_.Repos.Reportes;
using Microsoft.AspNetCore.Mvc;

namespace aparta_.Types
{

    [MutationType]
    public class ReportesMutations
{
        public async Task<string> DescargarReportePagos([Service] ReportesRepository reportes)
        {
            try
            {
                return await reportes.GenerateReport("a65e5bbe-0880-47bb-a560-e605c65fa78c", 2025);
            }catch(Exception e)
            {
                throw new GraphQLException(e.Message);
            }

        }

        public async Task<string> DescargarReporteIngresoMorosidad([Service] ReportesRepository reportes)
        {
            try
            {
                return await reportes.GenerateReporteIngresosMorosidad("2025", "d4b88408-20e1-700a-9777-c4c0e50bfaf5");
            }catch(Exception e)
            {
                throw new GraphQLException(e.Message);
            }

        }
        
    }
}
