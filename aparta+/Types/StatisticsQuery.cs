using aparta_.Services;
using data_aparta_.DTOs;

namespace aparta_.GraphQL
{
    [QueryType]
    public class StatisticsQuery
    {
        private readonly DashboardStatisticsService _statisticsService;

        public StatisticsQuery(DashboardStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task<DashboardStatisticsDTO> GetDashboardStatistics(Guid propiedadId)
        {
            return await _statisticsService.GetDashboardStatisticsAsync(propiedadId);
        }
    }
}
