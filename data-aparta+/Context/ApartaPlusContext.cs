using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using data_aparta_.Models;

namespace data_aparta_.Context;

public partial class ApartaPlusContext : DbContext
{
    public ApartaPlusContext()
    {
    }

    public ApartaPlusContext(DbContextOptions<ApartaPlusContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contrato> Contratos { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Imagenespropiedade> Imagenespropiedades { get; set; }

    public virtual DbSet<Inmueble> Inmuebles { get; set; }

    public virtual DbSet<Inquilino> Inquilinos { get; set; }

    public virtual DbSet<Propiedad> Propiedads { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(e => e.Contratoid).HasName("contrato_pkey");

            entity.ToTable("contrato");

            entity.Property(e => e.Contratoid)
                .ValueGeneratedNever()
                .HasColumnName("contratoid");
            entity.Property(e => e.Contratourl)
                .HasMaxLength(255)
                .HasColumnName("contratourl");
            entity.Property(e => e.Diapago).HasColumnName("diapago");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Fechafirma).HasColumnName("fechafirma");
            entity.Property(e => e.Fechaterminacion).HasColumnName("fechaterminacion");
            entity.Property(e => e.Fiadorcorreo)
                .HasMaxLength(255)
                .HasColumnName("fiadorcorreo");
            entity.Property(e => e.Fiadornombre)
                .HasMaxLength(255)
                .HasColumnName("fiadornombre");
            entity.Property(e => e.Fiadortelefono)
                .HasMaxLength(20)
                .HasColumnName("fiadortelefono");
            entity.Property(e => e.Inquilinoid).HasColumnName("inquilinoid");
            entity.Property(e => e.Mora).HasColumnName("mora");
            entity.Property(e => e.Precioalquiler).HasColumnName("precioalquiler");

            entity.HasOne(d => d.Inquilino).WithMany(p => p.Contratos)
                .HasForeignKey(d => d.Inquilinoid)
                .HasConstraintName("contrato_inquilinoid_fkey");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Facturaid).HasName("factura_pkey");

            entity.ToTable("factura");

            entity.Property(e => e.Facturaid)
                .ValueGeneratedNever()
                .HasColumnName("facturaid");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.Fechapago).HasColumnName("fechapago");
            entity.Property(e => e.Inmuebleid).HasColumnName("inmuebleid");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");
            entity.Property(e => e.SessionId)
                .HasColumnType("character varying")
                .HasColumnName("session_id");
            entity.Property(e => e.Url)
                .HasColumnType("character varying")
                .HasColumnName("url");

            entity.HasOne(d => d.Inmueble).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.Inmuebleid)
                .HasConstraintName("factura_inmuebleid_fkey");
        });

        modelBuilder.Entity<Imagenespropiedade>(entity =>
        {
            entity.HasKey(e => e.Imagenid).HasName("imagenespropiedades_pkey");

            entity.ToTable("imagenespropiedades");

            entity.Property(e => e.Imagenid)
                .ValueGeneratedNever()
                .HasColumnName("imagenid");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Fechacreacion).HasColumnName("fechacreacion");
            entity.Property(e => e.Imagenurl)
                .HasMaxLength(255)
                .HasColumnName("imagenurl");
            entity.Property(e => e.Propiedadid).HasColumnName("propiedadid");

            entity.HasOne(d => d.Propiedad).WithMany(p => p.Imagenespropiedades)
                .HasForeignKey(d => d.Propiedadid)
                .HasConstraintName("imagenespropiedades_propiedadid_fkey");
        });

        modelBuilder.Entity<Inmueble>(entity =>
        {
            entity.HasKey(e => e.Inmuebleid).HasName("inmueble_pkey");

            entity.ToTable("inmueble");

            entity.Property(e => e.Inmuebleid)
                .ValueGeneratedNever()
                .HasColumnName("inmuebleid");
            entity.Property(e => e.Codigo)
                .HasMaxLength(100)
                .HasColumnName("codigo");
            entity.Property(e => e.Contratoid).HasColumnName("contratoid");
            entity.Property(e => e.Fechacreacion).HasColumnName("fechacreacion");
            entity.Property(e => e.Numbanos).HasColumnName("numbanos");
            entity.Property(e => e.Numhabitaciones).HasColumnName("numhabitaciones");
            entity.Property(e => e.Ocupacion).HasColumnName("ocupacion");
            entity.Property(e => e.Propiedadid).HasColumnName("propiedadid");
            entity.Property(e => e.Tieneparqueo).HasColumnName("tieneparqueo");

            entity.HasOne(d => d.Contrato).WithMany(p => p.Inmuebles)
                .HasForeignKey(d => d.Contratoid)
                .HasConstraintName("inmueble_contratoid_fkey");

            entity.HasOne(d => d.Propiedad).WithMany(p => p.Inmuebles)
                .HasForeignKey(d => d.Propiedadid)
                .HasConstraintName("inmueble_propiedadid_fkey");
        });

        modelBuilder.Entity<Inquilino>(entity =>
        {
            entity.HasKey(e => e.Inquilinoid).HasName("inquilino_pkey");

            entity.ToTable("inquilino");

            entity.Property(e => e.Inquilinoid)
                .ValueGeneratedNever()
                .HasColumnName("inquilinoid");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Inquilinocorreo)
                .HasMaxLength(255)
                .HasColumnName("inquilinocorreo");
            entity.Property(e => e.Inquilinogenero).HasColumnName("inquilinogenero");
            entity.Property(e => e.Inquilinonombre)
                .HasMaxLength(255)
                .HasColumnName("inquilinonombre");
            entity.Property(e => e.Inquilinotelefono)
                .HasMaxLength(20)
                .HasColumnName("inquilinotelefono");
        });

        modelBuilder.Entity<Propiedad>(entity =>
        {
            entity.HasKey(e => e.Propiedadid).HasName("propiedad_pkey");

            entity.ToTable("propiedad");

            entity.Property(e => e.Propiedadid)
                .ValueGeneratedNever()
                .HasColumnName("propiedadid");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Fechacreacion).HasColumnName("fechacreacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Portadaurl)
                .HasMaxLength(255)
                .HasColumnName("portadaurl");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(255)
                .HasColumnName("ubicacion");
            entity.Property(e => e.Usuarioid).HasColumnName("usuarioid");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Propiedads)
                .HasForeignKey(d => d.Usuarioid)
                .HasConstraintName("propiedad_usuarioid_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Usuarioid).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.Property(e => e.Usuarioid)
                .ValueGeneratedNever()
                .HasColumnName("usuarioid");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Refreshtoken)
                .HasMaxLength(255)
                .HasColumnName("refreshtoken");
            entity.Property(e => e.Usuariocorreo)
                .HasMaxLength(255)
                .HasColumnName("usuariocorreo");
            entity.Property(e => e.Usuariohash)
                .HasMaxLength(255)
                .HasColumnName("usuariohash");
            entity.Property(e => e.Usuarionombre)
                .HasMaxLength(255)
                .HasColumnName("usuarionombre");
            entity.Property(e => e.Usuariotelefono)
                .HasMaxLength(20)
                .HasColumnName("usuariotelefono");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
