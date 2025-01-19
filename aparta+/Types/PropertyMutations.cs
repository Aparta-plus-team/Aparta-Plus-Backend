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

        public async Task<Propiedad> CreateProperty(CreatePropertyInput input)
        {
            try
            {
                return await propertyRepository.CreateAsync(input);
            }
            catch (Exception ex)
            {
                throw new GraphQLException(ex.Message);
            }

        }
        
        public async Task<FileUploadResponse> UploadPortrait(IFile file, string propertyId)
        {
            try
            {
                return await propertyRepository.UploadPortrait(new FileUploadInput {
                    File = file,
                    Type = "portrait",
                    PropertyId = "1"

                }, propertyId);
            }
            catch (Exception ex)
            {
                throw new GraphQLException(ex.Message);
            }
        }

        public async Task<Propiedad?> UpdateProperty(string id, Propiedad updatedProperty)
        {
            return await propertyRepository.UpdateAsync(id, updatedProperty);
        }
        public async Task<bool> DeleteProperty(string id)
        {
            return await propertyRepository.DeleteAsync(id);
        }

}
}
