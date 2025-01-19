using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using HotChocolate;

namespace aparta_.GraphQL
{
    [MutationType]
    public class InmuebleMutation
    {
        // Crear un nuevo inmueble
        public async Task<Inmueble> CreateInmueble(
            string codigo,
            DateTime fechaCreacion,
            bool ocupacion,
            bool tieneParqueo,
            int numBanos,
            int numHabitaciones,
            Guid? propiedadId,
            Guid? contratoId,
            [Service] IInmuebleRepository inmuebleRepository)
        {
            var inmueble = new Inmueble
            {
                Inmuebleid = Guid.NewGuid(),
                Codigo = codigo,
                Fechacreacion = DateOnly.FromDateTime(fechaCreacion),
                Ocupacion = ocupacion,
                Tieneparqueo = tieneParqueo,
                Numbanos = numBanos,
                Numhabitaciones = numHabitaciones,
                Propiedadid = propiedadId,
                Contratoid = contratoId
            };

            // Validación opcional
            if (contratoId.HasValue)
            {
                // Si contratoId se proporciona, asegúrate de que exista un contrato
                var contrato = await inmuebleRepository.GetInmuebleByIdAsync(contratoId.Value);
                if (contrato == null)
                {
                    throw new GraphQLException($"El contrato con ID {contratoId} no existe.");
                }
            }

            await inmuebleRepository.AddInmuebleAsync(inmueble);
            return inmueble;
        }

        // Actualizar un inmueble existente
        public async Task<Inmueble> UpdateInmueble(
            Guid inmuebleId,
            string? codigo,
            bool? ocupacion,
            bool? tieneParqueo,
            int? numBanos,
            int? numHabitaciones,
            Guid? propiedadId,
            Guid? contratoId,
            [Service] IInmuebleRepository inmuebleRepository)
        {
            var inmueble = await inmuebleRepository.GetInmuebleByIdAsync(inmuebleId);

            if (inmueble == null)
            {
                throw new GraphQLException($"Inmueble con ID {inmuebleId} no encontrado.");
            }

            // Actualiza solo los campos proporcionados
            if (!string.IsNullOrEmpty(codigo)) inmueble.Codigo = codigo;
            if (ocupacion.HasValue) inmueble.Ocupacion = ocupacion.Value;
            if (tieneParqueo.HasValue) inmueble.Tieneparqueo = tieneParqueo.Value;
            if (numBanos.HasValue) inmueble.Numbanos = numBanos.Value;
            if (numHabitaciones.HasValue) inmueble.Numhabitaciones = numHabitaciones.Value;
            if (propiedadId.HasValue) inmueble.Propiedadid = propiedadId.Value;
            if (contratoId.HasValue)
            {
                // Validación opcional: verificar si el contrato existe antes de actualizar
                var contrato = await inmuebleRepository.GetInmuebleByIdAsync(contratoId.Value);
                if (contrato == null)
                {
                    throw new GraphQLException($"El contrato con ID {contratoId} no existe.");
                }
                inmueble.Contratoid = contratoId.Value;
            }

            await inmuebleRepository.UpdateInmuebleAsync(inmueble);
            return inmueble;
        }

        // Eliminar un inmueble
        public async Task<bool> DeleteInmueble(
            Guid inmuebleId,
            [Service] IInmuebleRepository inmuebleRepository)
        {
            var inmueble = await inmuebleRepository.GetInmuebleByIdAsync(inmuebleId);

            if (inmueble == null)
            {
                throw new GraphQLException($"Inmueble con ID {inmuebleId} no encontrado.");
            }

            await inmuebleRepository.DeleteInmuebleAsync(inmuebleId);
            return true;
        }
    }
}
