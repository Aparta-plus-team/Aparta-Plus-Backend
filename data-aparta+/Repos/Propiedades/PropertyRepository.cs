using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Propiedades
{
    public class PropertyRepository : IPropertyRepository
    {

        private readonly ApartaPlusContext _context;
        private readonly S3Uploader s3Uploader;

        public PropertyRepository(ApartaPlusContext context, S3Uploader s3) { 
            _context = context;
            s3Uploader = s3;
        }
        public async Task<Propiedad> CreateAsync(CreatePropertyInput input)
        {
            Propiedad propiedad = new Propiedad { 
                Propiedadid = Guid.NewGuid(),
                Usuarioid = input.Usuarioid,
                Nombre = input.Nombre,
                Ubicacion = input.Ubicacion,
                Fechacreacion = DateOnly.FromDateTime(DateTime.UtcNow),
                Estado = true
            };

            propiedad.Propiedadid = Guid.NewGuid();
            propiedad.Fechacreacion = DateOnly.FromDateTime(DateTime.UtcNow);
            propiedad.Estado = true;

            if (propiedad.Usuarioid != null && !await _context.Usuarios.AnyAsync(u => u.Usuarioid == propiedad.Usuarioid))
            {
                throw new ArgumentException("El usuario especificado no existe.");
            }

            await _context.Propiedads.AddAsync(propiedad);
            await _context.SaveChangesAsync();

            return propiedad;
        }

        // Update an existing Propiedad
        public async Task<Propiedad?> UpdateAsync(string id, Propiedad updatedPropiedad)
        {
            var existingPropiedad = await _context.Set<Propiedad>().FindAsync(id);

            if (existingPropiedad == null || existingPropiedad.Estado == false)
                return null;

            if (updatedPropiedad.Usuarioid != null && !await _context.Set<Usuario>().AnyAsync(u => u.Usuarioid == updatedPropiedad.Usuarioid))
            {
                throw new ArgumentException("El usuario especificado no existe.");
            }

            existingPropiedad.Ubicacion = updatedPropiedad.Ubicacion;
            existingPropiedad.Nombre = updatedPropiedad.Nombre;
            existingPropiedad.Portadaurl = updatedPropiedad.Portadaurl;
            existingPropiedad.Usuarioid = updatedPropiedad.Usuarioid;

            await _context.SaveChangesAsync();

            return existingPropiedad;
        }

        // Soft delete a Propiedad
        public async Task<bool> DeleteAsync(string id)
        {
          var propiedad = await _context.Propiedads.FirstOrDefaultAsync(p => p.Propiedadid == Guid.Parse(id));

            if (propiedad == null || propiedad.Estado == false)
                return false;

            propiedad.Estado = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<FileUploadResponse> UploadPortrait(FileUploadInput fileInput, string propertyId)
        {
            if (fileInput.File == null)
                throw new ArgumentException("No se ha proporcionado un archivo.");

            try
            {
                var input = new FileUploadInput
                {
                    File = fileInput.File,
                    Type = "cover" // o el tipo que corresponda
                };

                var result = await s3Uploader.UploadFileAsync(input);

                var property = await _context.Propiedads.FirstOrDefaultAsync(p => p.Propiedadid == Guid.Parse(propertyId));
                if (property == null)
                    throw new ArgumentException("La propiedad especificada no existe.");

                property.Portadaurl = result.Url;
                await _context.SaveChangesAsync();

                return new FileUploadResponse { 
                    Key = result.Key,
                    Url = result.Url
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<FileUploadResponse> UploadImages(FileUploadInput fileInput, string propertyId)
        {
            if (fileInput.File == null)
                throw new ArgumentException("No se ha proporcionado un archivo.");

            try
            {
                var input = new FileUploadInput
                {
                    File = fileInput.File,
                    Type = "property-image",
                    PropertyId = propertyId
                };

                var result = await s3Uploader.UploadFileAsync(input);

                var propiedad = await _context.Propiedads.FirstOrDefaultAsync(p => p.Propiedadid == Guid.Parse(propertyId));
                if (propiedad == null)
                    throw new ArgumentException("La propiedad especificada no existe.");


                Imagenespropiedade imagenespropiedade = new Imagenespropiedade
                {
                    Imagenespropiedadeid = Guid.NewGuid(),
                    Propiedadid = Guid.Parse(propertyId),
                    Url = result.Url,
                    Fechacreacion = DateOnly.FromDateTime(DateTime.UtcNow),
                    Estado = true,
                };

                return new FileUploadResponse
                {
                    Key = result.Key,
                    Url = result.Url
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Imagenespropiedade> DeleteImage(string id)
        {
            var imagen = await _context.Imagenespropiedades.FirstOrDefaultAsync(i => i.Imagenespropiedadeid == Guid.Parse(id));

            if (imagen == null || imagen.Estado == false)
                throw new ArgumentException("La imagen especificada no existe.");

            imagen.Estado = false;
            await _context.SaveChangesAsync();

            return imagen;
        }

        public async Task<string> DeleteCover(string id)
        {
            var propiedad = await _context.Propiedads.FirstOrDefaultAsync(p => p.Propiedadid == Guid.Parse(id));

            if (propiedad == null || propiedad.Estado == false)
                throw new ArgumentException("La propiedad especificada no existe.");

            propiedad.Portadaurl = null;
            await _context.SaveChangesAsync();

            return "Portada eliminada exitosamente.";
        }

    }
}

