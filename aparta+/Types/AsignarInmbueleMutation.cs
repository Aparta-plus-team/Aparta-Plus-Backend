using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using HotChocolate;

namespace aparta_.Mutations
{
    [MutationType]
    public class InmuebleMutations
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IContratoRepository _contratoRepository;

        public InmuebleMutations(IInmuebleRepository inmuebleRepository, IContratoRepository contratoRepository)
        {
            _inmuebleRepository = inmuebleRepository;
            _contratoRepository = contratoRepository;
        }

        public async Task<Inmueble> AssignContractToInmueble(Guid inmuebleId, Guid contratoId)
        {
            // Verificar si el inmueble existe
            var inmueble = await _inmuebleRepository.GetInmuebleByIdAsync(inmuebleId);
            if (inmueble == null)
            {
                throw new GraphQLException($"Inmueble con ID {inmuebleId} no encontrado.");
            }

            // Verificar si el contrato existe
            var contrato = await _contratoRepository.GetContratoByIdAsync(contratoId);
            if (contrato == null)
            {
                throw new GraphQLException($"Contrato con ID {contratoId} no encontrado.");
            }

            // Asignar el contrato al inmueble
            inmueble.Contratoid = contratoId;

            // Actualizar el inmueble en la base de datos
            await _inmuebleRepository.UpdateInmuebleAsync(inmueble);

            return inmueble;
        }
    }
}
