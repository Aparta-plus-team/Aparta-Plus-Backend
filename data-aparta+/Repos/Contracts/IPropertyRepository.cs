using Amazon.Util.Internal;
using data_aparta_.DTOs;
using data_aparta_.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Contracts
{
    public interface IPropertyRepository
    {
        Task<Propiedad> CreateAsync(CreatePropertyInput input);
        Task<FileUploadResponse> UploadPortrait (FileUploadInput input, string id);
        Task<Propiedad?> UpdateAsync(string id, Propiedad propiedad);
        Task<bool> DeleteAsync(string id);

        Task<List<Propiedad>> GetPropiedadesByUsuarioId(Guid userId);

        Task<bool> DeletePortrait(string propertyId);
        Task<FileUploadResponse> UploadImages(FileUploadInput input);
        Task<bool> DeleteImage(string imageId);

    }
}
