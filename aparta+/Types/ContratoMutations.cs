using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace aparta_.GraphQL 
{
    [MutationType]

    public class ContratoMutations
    {
        public async Task<Contrato> CreateContrato(
    Guid? inquilinoId,
    string? contratourl,
    int? diapago,
    int? mora,
    int? precioalquiler,
    DateOnly? fechafirma,
    DateOnly? fechaterminacion,
    string? fiadornombre,
    string? fiadortelefono,
    string? fiadorcorreo,
    bool? estado,
    [Service] IContratoRepository contratoRepository)
{
    var contrato = new Contrato
    {
        Contratoid = Guid.NewGuid(),
        Inquilinoid = inquilinoId,
        Contratourl = contratourl,
        Diapago = diapago,
        Mora = mora,
        Precioalquiler = precioalquiler,
        Fechafirma = fechafirma,
        Fechaterminacion = fechaterminacion,
        Fiadornombre = fiadornombre,
        Fiadortelefono = fiadortelefono,
        Fiadorcorreo = fiadorcorreo,
        Estado = estado
    };

    await contratoRepository.AddContratoAsync(contrato);
    return contrato;
}
    }
    

}

