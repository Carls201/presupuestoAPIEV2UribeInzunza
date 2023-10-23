using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace presupuestoAPIEV2UribeInzunza.Models;

public partial class PresupuestodbEv2Context : DbContext
{
    public PresupuestodbEv2Context()
    {
    }

    public PresupuestodbEv2Context(DbContextOptions<PresupuestodbEv2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Ahorro> Ahorros { get; set; }

    public virtual DbSet<CategoriaGasto> CategoriaGastos { get; set; }

    public virtual DbSet<FuenteIngreso> FuenteIngresos { get; set; }

    public virtual DbSet<Gasto> Gastos { get; set; }

    public virtual DbSet<Ingreso> Ingresos { get; set; }

    public virtual DbSet<MetaAhorro> MetaAhorros { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=.\\sqlexpress; initial catalog=presupuestodbEv2; trusted_connection=True; encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ahorro>(entity =>
        {
            entity.HasKey(e => e.IdAhorro);

            entity.HasIndex(e => e.IdMeta, "IX_Ahorros_id_meta");

            entity.HasIndex(e => e.IdUsuario, "IX_Ahorros_id_usuario");

            entity.Property(e => e.IdAhorro).HasColumnName("id_ahorro");
            entity.Property(e => e.IdMeta).HasColumnName("id_meta");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Monto).HasColumnName("monto");

            entity.HasOne(d => d.IdMetaNavigation).WithMany(p => p.Ahorros).HasForeignKey(d => d.IdMeta);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ahorros).HasForeignKey(d => d.IdUsuario);
        });

        modelBuilder.Entity<CategoriaGasto>(entity =>
        {
            entity.HasKey(e => e.IdCategoria);

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        modelBuilder.Entity<FuenteIngreso>(entity =>
        {
            entity.HasKey(e => e.IdFuente);

            entity.Property(e => e.IdFuente).HasColumnName("id_fuente");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.IdGasto);

            entity.HasIndex(e => e.IdCategoria, "IX_Gastos_id_categoria");

            entity.HasIndex(e => e.IdUsuario, "IX_Gastos_id_usuario");

            entity.Property(e => e.IdGasto).HasColumnName("id_gasto");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Monto).HasColumnName("monto");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Gastos).HasForeignKey(d => d.IdCategoria);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Gastos).HasForeignKey(d => d.IdUsuario);
        });

        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasKey(e => e.IdIngreso);

            entity.HasIndex(e => e.IdFuente, "IX_Ingresos_id_fuente");

            entity.HasIndex(e => e.IdUsuario, "IX_Ingresos_id_usuario");

            entity.Property(e => e.IdIngreso).HasColumnName("id_ingreso");
            entity.Property(e => e.IdFuente).HasColumnName("id_fuente");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Monto).HasColumnName("monto");

            entity.HasOne(d => d.IdFuenteNavigation).WithMany(p => p.Ingresos).HasForeignKey(d => d.IdFuente);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ingresos).HasForeignKey(d => d.IdUsuario);
        });

        modelBuilder.Entity<MetaAhorro>(entity =>
        {
            entity.HasKey(e => e.IdMeta);

            entity.Property(e => e.IdMeta).HasColumnName("id_meta");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Rol1).HasColumnName("rol");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.HasIndex(e => e.IdRol, "IX_Usuarios_id_rol");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Apellido).HasColumnName("apellido");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Pass).HasColumnName("pass");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios).HasForeignKey(d => d.IdRol);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
