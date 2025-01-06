using data_aparta_.Context;
using data_aparta_.Models;
using HotChocolate.Data;

namespace aparta_.Types
{
    [QueryType]
    public static class Query
    {
        public static Book GetBook()
            => new Book("C# in depth.", new Author("Jon Skeet"));

        [UseOffsetPaging]
        [UseSorting]
        public static async Task<IQueryable<Factura>> GetFacturas(ApartaPlusContext dbContext) => dbContext.Facturas;

        [UseOffsetPaging]
        [UseSorting]
        [UseFiltering]
        public static async Task<IQueryable<Usuario>> GetUsuarios(ApartaPlusContext dbContext) => dbContext.Usuarios;

        [UseOffsetPaging]
        [UseSorting]
        [UseFiltering]
        public static IQueryable<Inmueble> GetInmuebles(ApartaPlusContext dbContext) => dbContext.Inmuebles;

        [UseOffsetPaging]
        [UseSorting]
        [UseFiltering]
        public static IQueryable<Contrato> GetContratos(ApartaPlusContext dbContext) => dbContext.Contratos;

        [UseOffsetPaging]
        [UseSorting]
        [UseFiltering]
        public static IQueryable<Inquilino> GetInquilinos(ApartaPlusContext dbContext) => dbContext.Inquilinos;

        [UseOffsetPaging]
        [UseSorting]
        [UseFiltering]
        public static IQueryable<Propiedad> GetPropiedads(ApartaPlusContext dbContext) => dbContext.Propiedads;
        
       
    }
}
