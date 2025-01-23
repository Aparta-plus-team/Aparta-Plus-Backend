using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Propiedades
{
    public class PropertyRepository : IPropertyRepository, IAsyncDisposable
    {
        private readonly IDbContextFactory<ApartaPlusContext> _dbContextFactory;
        private readonly S3Uploader _s3Uploader;
        private ApartaPlusContext? _context;

        public PropertyRepository(IDbContextFactory<ApartaPlusContext> dbContextFactory, S3Uploader s3)
        {
            _dbContextFactory = dbContextFactory;
            _s3Uploader = s3;
            _context = _dbContextFactory.CreateDbContext(); // Crear instancia del contexto
        }

        // Crear nueva propiedad
        public async Task<Propiedad> CreateAsync(CreatePropertyInput input)
        {
            EnsureContext();

            Propiedad propiedad = new Propiedad
            {
                Propiedadid = Guid.NewGuid(),
                Usuarioid = input.Usuarioid,
                Nombre = input.Nombre,
                Ubicacion = input.Ubicacion,
                Fechacreacion = DateOnly.FromDateTime(DateTime.UtcNow),
                Estado = true
            };

            if (propiedad.Usuarioid != null && !await _context!.Usuarios.AnyAsync(u => u.Usuarioid == propiedad.Usuarioid))
            {
                throw new ArgumentException("El usuario especificado no existe.");
            }

            await _context.Propiedads.AddAsync(propiedad);
            await _context.SaveChangesAsync();

            return propiedad;
        }

        // Actualizar propiedad existente
        public async Task<Propiedad?> UpdateAsync(string id, Propiedad updatedPropiedad)
        {
            EnsureContext();

            var existingPropiedad = await _context!.Set<Propiedad>().FindAsync(Guid.Parse(id));

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

        // Eliminar propiedad (borrado lógico)
        public async Task<bool> DeleteAsync(string id)
        {
            EnsureContext();

            var propiedad = await _context!.Propiedads.FirstOrDefaultAsync(p => p.Propiedadid == Guid.Parse(id));

            if (propiedad == null || propiedad.Estado == false)
                return false;

            propiedad.Estado = false;
            await _context.SaveChangesAsync();

            return true;
        }

        // Obtener propiedades por usuario
        public async Task<List<Propiedad>> GetPropiedadesByUsuarioId(Guid userId)
        {
            EnsureContext();

            return await _context!.Propiedads
                .Where(p => p.Usuarioid == userId && p.Estado == true)
                .ToListAsync();
        }

        // Subir archivo
        public async Task<FileUploadResponse> UploadPortrait(FileUploadInput fileInput)
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

                var result = await _s3Uploader.UploadFileAsync(input);

                //Cambiar en la base de datos
                var propiedad = await _context!.Propiedads.FirstOrDefaultAsync(p => p.Propiedadid == Guid.Parse(fileInput.PropertyId));
                propiedad.Portadaurl = result.Url;
                await _context.SaveChangesAsync();

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

        public async Task<bool> DeletePortrait (string propertyId)
        {
            var propiedad = await _context!.Propiedads.FirstOrDefaultAsync(p => p.Propiedadid == Guid.Parse(propertyId));

            if (propiedad == null || propiedad.Estado == false)
                return false;

            propiedad.Portadaurl = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<FileUploadResponse> UploadImages(FileUploadInput input)
        {
            if (input.File == null)
                throw new ArgumentException("No se ha proporcionado un archivo.");

            try
            {

                var newInput = new FileUploadInput
                {
                    File = input.File,
                    Type = "property-image" // o el tipo que corresponda
                };

                var result = await _s3Uploader.UploadFileAsync(input);

                //Cambiar en la base de datos
                Imagenespropiedade img = new Imagenespropiedade
                {
                    Imagenid = Guid.NewGuid(),
                    Propiedadid = Guid.Parse(input.PropertyId),
                    Imagenurl = result.Url,
                    Fechacreacion = DateOnly.FromDateTime(DateTime.UtcNow),
                    Estado = true
                };

                await _context!.Imagenespropiedades.AddAsync(img);
                await _context.SaveChangesAsync();


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

        public async Task<bool> DeleteImage(string imageId)
        {
            var image = await _context!.Imagenespropiedades.FirstOrDefaultAsync(i => i.Imagenid == Guid.Parse(imageId));

            if (image == null || image.Estado == false)
                return false;

            image.Estado = false;
            await _context.SaveChangesAsync();

            return true;
        }

        // Asegura que el contexto está inicializado
        private void EnsureContext()
        {
            if (_context == null)
            {
                _context = _dbContextFactory.CreateDbContext();
            }
        }

        // Implementación de IAsyncDisposable
        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
                _context = null;
            }
        }
    }
}
