using System;
using System.Collections.Generic;
using Amaretto.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Amaretto.Infraestructure.Data;

public partial class AmarettoContext : DbContext
{
    public AmarettoContext(DbContextOptions<AmarettoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<CocinaOrden> CocinaOrden { get; set; }

    public virtual DbSet<Combo> Combo { get; set; }

    public virtual DbSet<EstacionCocina> EstacionCocina { get; set; }

    public virtual DbSet<Ingrediente> Ingrediente { get; set; }

    public virtual DbSet<MenuCombo> MenuCombo { get; set; }

    public virtual DbSet<MenuDetalleCombo> MenuDetalleCombo { get; set; }

    public virtual DbSet<MenuDetalleProducto> MenuDetalleProducto { get; set; }

    public virtual DbSet<MenuProducto> MenuProducto { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<PedidoDetalle> PedidoDetalle { get; set; }

    public virtual DbSet<PedidoDetallePersonalizacion> PedidoDetallePersonalizacion { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<ProductoIngrediente> ProductoIngrediente { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A1019A7F8FD");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__75E3EFCF39F9D091").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CocinaOrden>(entity =>
        {
            entity.HasKey(e => e.IdCocinaOrden).HasName("PK__CocinaOr__62A9068BE854F277");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");

            entity.HasOne(d => d.IdDetalleNavigation).WithMany(p => p.CocinaOrden)
                .HasForeignKey(d => d.IdDetalle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CocinaOrden_PedidoDetalle");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.CocinaOrden)
                .HasForeignKey(d => d.IdEstacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CocinaOrd__IdEst__619B8048");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.IdCombo).HasName("PK__Combo__D65BF2C8962BA20F");

            entity.HasIndex(e => e.Nombre, "UQ__Combo__75E3EFCF17881A18").IsUnique();

            entity.Property(e => e.IdCombo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Imagen1)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Imagen2)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");


            entity.HasMany(d => d.IdProducto).WithMany(p => p.IdCombo)
                .UsingEntity<Dictionary<string, object>>(
                    "ComboProducto",
                    r => r.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ComboProducto_Producto"),
                    l => l.HasOne<Combo>().WithMany()
                        .HasForeignKey("IdCombo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ComboProducto_Combo"),
                    j =>
                    {
                        j.HasKey("IdCombo", "IdProducto");
                        j.IndexerProperty<string>("IdCombo")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("IdProducto")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<EstacionCocina>(entity =>
        {
            entity.HasKey(e => e.IdEstacion).HasName("PK__Estacion__F0C18C423B759888");

            entity.HasIndex(e => e.Nombre, "UQ__Estacion__75E3EFCF756405DC").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdIngrediente).HasName("PK__Ingredie__3DA4DD60A8D068BF");

            entity.HasIndex(e => e.Nombre, "UQ__Ingredie__75E3EFCF140FEA3F").IsUnique();

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MenuCombo>(entity =>
        {
            entity.HasKey(e => e.IdMenuCombo).HasName("PK__MenuComb__31AA6328BB42BAE1");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MenuDetalleCombo>(entity =>
        {
            entity.HasKey(e => e.IdMenuDetalleCombo).HasName("PK__MenuDeta__54DB625083E952E0");

            entity.Property(e => e.IdCombo)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.MenuDetalleCombo)
                .HasForeignKey(d => d.IdCombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuDetalleCombo_Combo");

            entity.HasOne(d => d.IdMenuComboNavigation).WithMany(p => p.MenuDetalleCombo)
                .HasForeignKey(d => d.IdMenuCombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuDetalleCombo_MenuCombo");
        });

        modelBuilder.Entity<MenuDetalleProducto>(entity =>
        {
            entity.HasKey(e => e.IdMenuDetalleProducto).HasName("PK__MenuDeta__CAAB58625AD5C93B");

            entity.Property(e => e.IdProducto)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdMenuProductoNavigation).WithMany(p => p.MenuDetalleProducto)
                .HasForeignKey(d => d.IdMenuProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MenuDetal__IdMen__571DF1D5");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.MenuDetalleProducto)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__MenuDetal__IdPro__5812160E");
        });

        modelBuilder.Entity<MenuProducto>(entity =>
        {
            entity.HasKey(e => e.IdMenuProducto).HasName("PK__Menu__4D7EA8E1E81E1F98");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__FC851A3A7099CBA6");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Completado");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MontoRecibido).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NombreTitular)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TipoTarjeta)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UltimosDigitos)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Vuelto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__IdPedido__7D439ABD");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__Pedido__9D335DC3E35FCDC7");

            entity.Property(e => e.CostoEnvio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DireccionEntrega)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MetodoEntrega)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pedido__IdUsuari__66603565");
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__PedidoDe__E43646A5F35D57EE");

            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.IdCombo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IdProducto)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.IdCombo)
                .HasConstraintName("FK__PedidoDet__IdCom__70DDC3D8");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoDet__IdPed__6EF57B66");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__PedidoDet__IdPro__6FE99F9F");
        });

        modelBuilder.Entity<PedidoDetallePersonalizacion>(entity =>
        {
            entity.HasKey(e => e.IdPersonalizacion).HasName("PK__PedidoDe__643859B2B5761828");

            entity.HasIndex(e => e.IdDetalle, "UQ__PedidoDe__E43646A4119E022B").IsUnique();

            entity.Property(e => e.Imagen1)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Imagen2)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Mensaje)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.PrecioExtra).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RutaImagenReferencia)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDetalleNavigation).WithOne(p => p.PedidoDetallePersonalizacion)
                .HasForeignKey<PedidoDetallePersonalizacion>(d => d.IdDetalle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoDet__IdDet__76969D2E");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__098892107F95A74B");

            entity.HasIndex(e => e.Nombre, "UQ__Producto__75E3EFCF80503EA4").IsUnique();

            entity.Property(e => e.IdProducto)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Imagen1)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Imagen2)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Producto)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producto__IdCate__3D5E1FD2");
        });

        modelBuilder.Entity<ProductoIngrediente>(entity =>
        {
            entity.HasKey(e => new { e.IdProducto, e.IdIngrediente }).HasName("PK__Producto__1A52DFC6506D966F");

            entity.Property(e => e.IdProducto)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdIngredienteNavigation).WithMany(p => p.ProductoIngrediente)
                .HasForeignKey(d => d.IdIngrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductoI__IdIng__49C3F6B7");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoIngrediente)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductoI__IdPro__48CFD27E");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584CDF0C5C96");

            entity.HasIndex(e => e.Descripcion, "UQ__Rol__92C53B6C2646689D").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97A7351689");

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D10534C11BE912").IsUnique();

            entity.Property(e => e.Direccion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__IdRol__286302EC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
