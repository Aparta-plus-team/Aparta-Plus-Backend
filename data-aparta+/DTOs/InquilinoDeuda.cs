namespace data_aparta_.DTOs{
      public class InquilinoDeudaDto
    {
        public Guid InquilinoId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string EstadoFactura { get; set; }
        public int CantidadFacturas { get; set; }
        public decimal TotalDeuda { get; set; }
    }
}