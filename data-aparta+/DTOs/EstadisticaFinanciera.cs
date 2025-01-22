namespace data_aparta_.DTOs
{
    public class EstadisticaFinancieraDto
    {
        public int Mes { get; set; }
        public int Anio  { get; set; }
        public decimal TotalGanancias { get; set; }
        public decimal TotalDeudas { get; set; }
    }
}
