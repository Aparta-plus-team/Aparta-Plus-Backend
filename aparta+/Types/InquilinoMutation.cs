using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;
using HotChocolate;

namespace aparta_.GraphQL
{
    [MutationType]
    public class InquilinoMutation
    {

        // Crear Inquilino
        public async Task<Inquilino> CreateInquilino(
    string? inquilinoNombre,
    string? inquilinoTelefono,
    string? inquilinoCorreo,
    bool? inquilinoGenero,
    bool? estado,
    [Service] IInquilinoRepository inquilinoRepository)
        {
            if (string.IsNullOrEmpty(inquilinoNombre))
            {
                throw new GraphQLException("El nombre del inquilino es obligatorio.");
            }

            if (string.IsNullOrEmpty(inquilinoCorreo))
            {
                throw new GraphQLException("El correo del inquilino es obligatorio.");
            }

            var inquilino = new Inquilino
            {
                Inquilinoid = Guid.NewGuid(),
                Inquilinonombre = inquilinoNombre,
                Inquilinotelefono = inquilinoTelefono,
                Inquilinocorreo = inquilinoCorreo,
                Inquilinogenero = inquilinoGenero,
                Estado = estado
            };

            await inquilinoRepository.AddInquilinoAsync(inquilino);
            return inquilino;
        }

        // Actualizar Inquilino
        public async Task<Inquilino> UpdateInquilino(
    Guid inquilinoId,
    string? inquilinoNombre,
    string? inquilinoTelefono,
    string? inquilinoCorreo,
    bool? inquilinoGenero,
    bool? estado,
    [Service] IInquilinoRepository inquilinoRepository)
        {
            var inquilino = await inquilinoRepository.GetInquilinoByIdAsync(inquilinoId);
            if (inquilino == null)
            {
                throw new GraphQLException($"Inquilino con ID {inquilinoId} no encontrado.");
            }

            // Validar y actualizar solo los campos proporcionados
            bool isUpdated = false;

            if (!string.IsNullOrEmpty(inquilinoNombre))
            {
                inquilino.Inquilinonombre = inquilinoNombre;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(inquilinoTelefono))
            {
                inquilino.Inquilinotelefono = inquilinoTelefono;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(inquilinoCorreo))
            {
                inquilino.Inquilinocorreo = inquilinoCorreo;
                isUpdated = true;
            }

            if (inquilinoGenero.HasValue)
            {
                inquilino.Inquilinogenero = inquilinoGenero;
                isUpdated = true;
            }

            if (estado.HasValue)
            {
                inquilino.Estado = estado;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                throw new GraphQLException("No se proporcionaron campos válidos para actualizar.");
            }

            await inquilinoRepository.UpdateInquilinoAsync(inquilino);
            return inquilino;
        }

        // Eliminar Inquilino
        public async Task<bool> DeleteInquilino(
    Guid inquilinoId,
    [Service] IInquilinoRepository inquilinoRepository)
{
    var inquilino = await inquilinoRepository.GetInquilinoByIdAsync(inquilinoId);
    if (inquilino == null)
    {
        throw new GraphQLException($"Inquilino con ID {inquilinoId} no encontrado.");
    }

    try
    {
        await inquilinoRepository.DeleteInquilinoAsync(inquilinoId);
        return true;
    }
    catch (DbUpdateException ex)
    {
        // Manejar error relacionado con restricciones de clave foránea
        throw new GraphQLException($"No se puede eliminar el inquilino debido a restricciones de relaciones. {ex.Message}");
    }
}
    }
}
