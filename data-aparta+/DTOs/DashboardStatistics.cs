namespace data_aparta_.DTOs
{
    public class DashboardStatisticsDTO
    {
        // Resumen Superior
        public int TotalPropiedades { get; set; } // Total de propiedades del usuario
        public int PropiedadesAlquiladas { get; set; } // Total de propiedades alquiladas
        public decimal GananciaMensual { get; set; } // Ganancia total del mes actual

        // Estadísticas de Ganancias Mensuales (últimos 7 meses)
        public List<GananciaMensualDto> GananciasMensuales { get; set; } = new List<GananciaMensualDto>();

        // Desglose de Ingresos por Ubicación
        public List<DesgloseUbicacionDto> DesglosePorUbicacion { get; set; } = new List<DesgloseUbicacionDto>();

        // Transacciones recientes (últimos movimientos financieros)
        public List<TransaccionRecienteDto> TransaccionesRecientes { get; set; } = new List<TransaccionRecienteDto>();

        // Morosidad (tareas pendientes o facturas atrasadas)
        public List<MorosidadDto> Morosidades { get; set; } = new List<MorosidadDto>();
    }

    // DTO para Ganancias Mensuales
    public class GananciaMensualDto
    {
        public string Mes { get; set; } // Ejemplo: "Enero"
        public decimal Ganancia { get; set; } // Ganancia en ese mes
    }

    // DTO para Desglose de Ingresos por Ubicación
    public class DesgloseUbicacionDto
    {
        public string Ubicacion { get; set; } // Ejemplo: "Punta Cana"
        public decimal Ganancia { get; set; } // Ganancia total de esa ubicación
        public double Porcentaje { get; set; } // Porcentaje de la ganancia total
    }

    // DTO para Transacciones Recientes
    public class TransaccionRecienteDto
    {
        public string PropiedadNombre { get; set; } // Ejemplo: "Cebastian's House"
        public DateTime Fecha { get; set; } // Fecha de la transacción
        public decimal Monto { get; set; } // Monto de la transacción
    }

    // DTO para Morosidad
    public class MorosidadDto
    {
        public string Servicio { get; set; } // Ejemplo: "Plomero"
        public string Detalle { get; set; } // Ejemplo: "Basura Rota"
        public string PropiedadNombre { get; set; } // Ejemplo: "721 Meadowview"
        public string Inquilino { get; set; } // Ejemplo: "Jacob Jones"
    }
}
