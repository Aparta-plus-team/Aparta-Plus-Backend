namespace data_aparta_.DTOs
{
    public class DashboardStatisticsDTO
    {
        public int TotalInmuebles { get; set; }
        public int TotalInmueblesDesocupados { get; set; }
        public double DuracionPromedioContratos { get; set; } // En meses
        public decimal IngresoMensualPropiedades { get; set; }
        public decimal CostoOportunidad { get; set; } // Dinero perdido por desocupación
        public double PorcentajeOcupacion { get; set; } // En porcentaje
        public int TotalContratosActivos { get; set; }
        public double DuracionTotalContratosActivos { get; set; } // En meses

        // Nueva propiedad para inquilinos por género
        public int TotalHombres { get; set; }
        public int TotalMujeres { get; set; }
    }
}
