using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
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

            if (!string.IsNullOrEmpty(inquilinoNombre)) inquilino.Inquilinonombre = inquilinoNombre;
            if (!string.IsNullOrEmpty(inquilinoTelefono)) inquilino.Inquilinotelefono = inquilinoTelefono;
            if (!string.IsNullOrEmpty(inquilinoCorreo)) inquilino.Inquilinocorreo = inquilinoCorreo;
            if (inquilinoGenero.HasValue) inquilino.Inquilinogenero = inquilinoGenero;
            if (estado.HasValue) inquilino.Estado = estado;

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

            await inquilinoRepository.DeleteInquilinoAsync(inquilinoId);
            return true;
        }
    }
}
