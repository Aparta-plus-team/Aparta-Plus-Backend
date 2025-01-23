using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace aparta_.Types
{
    [MutationType]
    public class PropertyMutations
    {
        private readonly IPropertyRepository propertyRepository;

        public PropertyMutations(IPropertyRepository repo)
        {
            propertyRepository = repo;
        }

        // Crear propiedad
        public async Task<Propiedad> CreateProperty(CreatePropertyInput input)
        {
            if (string.IsNullOrEmpty(input.Nombre))
            {
                throw new GraphQLException("El nombre de la propiedad es obligatorio.");
            }

            if (string.IsNullOrEmpty(input.Ubicacion))
            {
                throw new GraphQLException("La ubicación de la propiedad es obligatoria.");
            }

            try
            {
                return await propertyRepository.CreateAsync(input);
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error al crear la propiedad: {ex.Message}");
            }
        }

        // Subir archivo (portrait)
        public async Task<FileUploadResponse> UploadPortrait(IFile file, string propertyId)
        {
            if (file == null)
            {
                throw new GraphQLException("No se ha proporcionado un archivo.");
            }

            if (string.IsNullOrEmpty(propertyId))
            {
                throw new GraphQLException("El ID de la propiedad es obligatorio.");
            }

            try
            {
                return await propertyRepository.UploadPortrait(new FileUploadInput
                {
                    File = file,
                    Type = "portrait",
                    PropertyId = propertyId
                });
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error al subir el archivo: {ex.Message}");
            }
        }

        // Actualizar propiedad
        public async Task<Propiedad?> UpdateProperty(string id, Propiedad updatedProperty)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new GraphQLException("El ID de la propiedad es obligatorio.");
            }

            if (string.IsNullOrEmpty(updatedProperty.Nombre))
            {
                throw new GraphQLException("El nombre de la propiedad es obligatorio.");
            }

            try
            {
                var updated = await propertyRepository.UpdateAsync(id, updatedProperty);
                if (updated == null)
                {
                    throw new GraphQLException($"No se encontró la propiedad con ID {id} o está desactivada.");
                }
                return updated;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error al actualizar la propiedad: {ex.Message}");
            }
        }

        // Eliminar propiedad
        public async Task<bool> DeleteProperty(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new GraphQLException("El ID de la propiedad es obligatorio.");
            }

            try
            {
                var deleted = await propertyRepository.DeleteAsync(id);
                if (!deleted)
                {
                    throw new GraphQLException($"No se pudo eliminar la propiedad con ID {id}. Es posible que no exista o ya esté desactivada.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error al eliminar la propiedad: {ex.Message}");
            }
        }

        // Eliminar portrait
        public async Task<bool> DeletePortrait(string propertyId)
        {
            if (string.IsNullOrEmpty(propertyId))
            {
                throw new GraphQLException("El ID de la propiedad es obligatorio.");
            }

            try
            {
                return await propertyRepository.DeletePortrait(propertyId);
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error al eliminar el portrait: {ex.Message}");
            }
        }

        // Subir imágenes
        public async Task<FileUploadResponse> UploadImages(IFile file, string propertyId)
        {
            if (file == null)
            {
                throw new GraphQLException("No se ha proporcionado un archivo.");
            }

            if (string.IsNullOrEmpty(propertyId))
            {
                throw new GraphQLException("El ID de la propiedad es obligatorio.");
            }

            try
            {
                return await propertyRepository.UploadImages(new FileUploadInput
                {
                    File = file,
                    Type = "property-image",
                    PropertyId = propertyId
                });
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error al subir las imágenes: {ex.Message}");
            }
        }

        // Eliminar imagen
        public async Task<bool> DeleteImage(string imageId)
        {
            if (string.IsNullOrEmpty(imageId))
            {
                throw new GraphQLException("El ID de la imagen es obligatorio.");
            }

            try
            {
                return await propertyRepository.DeleteImage(imageId);
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error al eliminar la imagen: {ex.Message}");
            }
        }
    }
}
