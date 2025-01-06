using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace aparta_.GraphQL
{
    [MutationType]
    public class ContratoMutations
    {
        // Método para actualizar un contrato existente
        public async Task<Contrato> UpdateContrato(
            Guid contratoId, // ID del contrato a actualizar
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
            // Obtener el contrato existente
            var existingContrato = await contratoRepository.GetContratoByIdAsync(contratoId);

            if (existingContrato == null)
            {
                throw new GraphQLException($"Contrato con ID {contratoId} no encontrado.");
            }

            // Actualizar solo los campos proporcionados
            if (inquilinoId.HasValue) existingContrato.Inquilinoid = inquilinoId;
            if (!string.IsNullOrEmpty(contratourl)) existingContrato.Contratourl = contratourl;
            if (diapago.HasValue) existingContrato.Diapago = diapago;
            if (mora.HasValue) existingContrato.Mora = mora;
            if (precioalquiler.HasValue) existingContrato.Precioalquiler = precioalquiler;
            if (fechafirma.HasValue) existingContrato.Fechafirma = fechafirma;
            if (fechaterminacion.HasValue) existingContrato.Fechaterminacion = fechaterminacion;
            if (!string.IsNullOrEmpty(fiadornombre)) existingContrato.Fiadornombre = fiadornombre;
            if (!string.IsNullOrEmpty(fiadortelefono)) existingContrato.Fiadortelefono = fiadortelefono;
            if (!string.IsNullOrEmpty(fiadorcorreo)) existingContrato.Fiadorcorreo = fiadorcorreo;
            if (estado.HasValue) existingContrato.Estado = estado;

            // Guardar los cambios
            await contratoRepository.UpdateContratoAsync(existingContrato);
            return existingContrato;
        }
    }
}
