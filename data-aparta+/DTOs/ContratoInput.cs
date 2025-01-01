using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_aparta_.DTOs;




public class ContratoDto
{
    public Guid Contratoid { get; set; }
    public Guid? Inquilinoid { get; set; }
    public string? Contratourl { get; set; }
    public int? Diapago { get; set; }
    public int? Mora { get; set; }
    public int? Precioalquiler { get; set; }
    public DateOnly? Fechafirma { get; set; }
    public DateOnly? Fechaterminacion { get; set; }
    public string? Fiadornombre { get; set; }
    public string? Fiadortelefono { get; set; }
    public string? Fiadorcorreo { get; set; }
    public bool? Estado { get; set; }

    // Relaciones
    public string? InquilinoNombre { get; set; }
    public ICollection<RegisterInput>? Inmuebles { get; set; }
}

public class CreateContratoDto
{
    public Guid? Inquilinoid { get; set; }
    public string? Contratourl { get; set; }
    public int? Diapago { get; set; }
    public int? Mora { get; set; }
    public int? Precioalquiler { get; set; }
    public DateOnly? Fechafirma { get; set; }
    public DateOnly? Fechaterminacion { get; set; }
    public string? Fiadornombre { get; set; }
    public string? Fiadortelefono { get; set; }
    public string? Fiadorcorreo { get; set; }
    public bool? Estado { get; set; }
}

public class UpdateContratoDto
{
    public string? Contratourl { get; set; }
    public int? Diapago { get; set; }
    public int? Mora { get; set; }
    public int? Precioalquiler { get; set; }
    public DateOnly? Fechafirma { get; set; }
    public DateOnly? Fechaterminacion { get; set; }
    public string? Fiadornombre { get; set; }
    public string? Fiadortelefono { get; set; }
    public string? Fiadorcorreo { get; set; }
    public bool? Estado { get; set; }
}

