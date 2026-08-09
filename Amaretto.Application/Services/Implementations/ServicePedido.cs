using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServicePedido : IServicePedido
    {
        private const decimal COSTO_ENVIO = 2500m;
        private const decimal PORCENTAJE_IVA = 0.13m;

        // Identificadores de la tabla Rol
        private const int ROL_ADMINISTRADOR = 1;
        private const int ROL_ENCARGADO = 4;

        private readonly IServiceCarrito _carritoService;
        private readonly IRepositoryPedido _repoPedido;
        private readonly IMapper _mapper;

        /// <summary>
        /// Variable que identifica al usuario en sesión. Lee los claims en cada
        /// acceso, así que si cambia el usuario que inició sesión cambian también
        /// su Id y su rol, y con ellos las gestiones que el servicio permite.
        /// </summary>
        private readonly IServiceUsuarioActual _usuarioActual;

        private readonly IServiceNotificacion _serviceNotificacion;

        public ServicePedido(
            IServiceCarrito carritoService,
            IRepositoryPedido repoPedido,
            IServiceUsuarioActual usuarioActual,
            IServiceNotificacion serviceNotificacion,
            IMapper mapper)
        {
            _carritoService = carritoService;
            _repoPedido = repoPedido;
            _usuarioActual = usuarioActual;
            _serviceNotificacion = serviceNotificacion;
            _mapper = mapper;
        }

        public async Task<PedidoRegistroViewModel> PrepararRegistroAsync(IServiceUsuarioActual usuarioActual)
        {
            var esEncargado = usuarioActual.EstaAutenticado && usuarioActual.IdRol == ROL_ENCARGADO;

            var vm = new PedidoRegistroViewModel
            {
                Resumen = ObtenerResumen("Domicilio"),
                EsEncargado = esEncargado,
                UsuarioActual = new UsuarioDTO 
                { 
                    IdUsuario = usuarioActual.IdUsuario,
                    NombreCompleto = usuarioActual.NombreCompleto,
                    Email = usuarioActual.Email,
                    Telefono = usuarioActual.Telefono,
                    Direccion = usuarioActual.Direccion,
                    NombreRol = usuarioActual.NombreRol
                }
            };

            return vm;
        }

        public ResumenPedidoDTO ObtenerResumen(string metodoEntrega)
        {
            var carrito = _carritoService.ObtenerCarrito();

            return new ResumenPedidoDTO
            {
                Lineas = carrito.Select(c => new PedidoDetalleLineaDTO
                {
                    LineaId = c.LineaId,
                    IdItem = c.IdItem,
                    Tipo = c.Tipo,
                    Nombre = c.Nombre,
                    Imagen = c.Imagen,
                    PrecioUnitario = c.Precio,
                    Cantidad = c.Cantidad,
                    Observaciones = c.Observaciones,
                    Personalizacion = c.Personalizacion
                }).ToList(),
                CostoEnvio = metodoEntrega == "Domicilio" ? COSTO_ENVIO : 0m
            };
        }

        public async Task<PedidoResultadoDTO> RegistrarPedidoAsync(PedidoRegistroDTO dto)
        {
            var carrito = _carritoService.ObtenerCarrito();
            if (!carrito.Any())
                throw new InvalidOperationException("El carrito está vacío.");

            if (dto.MetodoEntrega == "Domicilio" && string.IsNullOrWhiteSpace(dto.DireccionEntrega))
                throw new InvalidOperationException("Debe indicar la dirección de entrega.");

            var resumen = ObtenerResumen(dto.MetodoEntrega);

            // El modal de pago solo permite confirmar cuando el pago ya se completó
            // (tarjeta procesada o efectivo recibido), así que el pedido nace pagado.
            const string estadoPago = "Completado";
            var estadoPedido = estadoPago == "Completado" ? "Pagado" : "Pendiente de pago";

            // Arma un resumen legible con las observaciones de cada línea, ej:
            // "Cupcake de Chocolate: Sin fresas | Combo Cupcakes Variados: Más nueces"
            var observacionesPorLinea = resumen.Lineas
                .Where(l => !string.IsNullOrWhiteSpace(l.Observaciones))
                .Select(l => $"{l.Nombre}: {l.Observaciones}");

            var observacionesFinal = string.Join(" | ", observacionesPorLinea);
            if (!string.IsNullOrWhiteSpace(dto.Observaciones))
                observacionesFinal = string.IsNullOrWhiteSpace(observacionesFinal)
                    ? dto.Observaciones
                    : $"{dto.Observaciones} | {observacionesFinal}";

            var pedido = new Pedido
            {
                IdUsuario = dto.IdCliente,
                IdEncargado = dto.IdEncargado,
                Estado = estadoPedido,
                MetodoEntrega = dto.MetodoEntrega,
                DireccionEntrega = dto.MetodoEntrega == "Domicilio" ? dto.DireccionEntrega : null,
                CostoEnvio = resumen.CostoEnvio,
                Subtotal = resumen.Subtotal,
                Impuesto = resumen.Impuesto,
                Total = resumen.Total,
                Observaciones = string.IsNullOrWhiteSpace(observacionesFinal) ? null : observacionesFinal,
                FechaPedido = DateTime.Now
            };

            foreach (var linea in resumen.Lineas)
            {
                var detalle = new PedidoDetalle
                {
                    IdProducto = linea.Tipo == "producto" ? linea.IdItem : null,
                    IdCombo = linea.Tipo == "combo" ? linea.IdItem : null,
                    Cantidad = linea.Cantidad,
                    PrecioUnitario = linea.PrecioUnitario,
                    Subtotal = linea.Subtotal,
                    Iva = linea.Iva,
                    Total = linea.Total,
                    Observaciones = linea.Observaciones,
                    Personalizado = linea.Personalizacion != null
                };

                if (linea.Personalizacion != null)
                {
                    detalle.PedidoDetallePersonalizacion = new PedidoDetallePersonalizacion
                    {
                        Tamano = linea.Personalizacion.Tamano,
                        MedidaCm = linea.Personalizacion.MedidaCm,
                        SaborBizcocho = linea.Personalizacion.SaborBizcocho,
                        TipoRelleno = linea.Personalizacion.TipoRelleno,
                        TipoDecoracion = linea.Personalizacion.TipoDecoracion,
                        DecoracionDetalle = linea.Personalizacion.DecoracionDetalle,
                        RutaImagenReferencia = linea.Personalizacion.RutaImagenReferencia,
                        Dedicatoria = linea.Personalizacion.Dedicatoria,
                        PrecioExtra = linea.Personalizacion.PrecioExtra
                    };
                }

                pedido.PedidoDetalle.Add(detalle);
            }

            decimal? vuelto = null;
            if (dto.MetodoPago == "Efectivo" && dto.MontoRecibido.HasValue)
                vuelto = Math.Round(dto.MontoRecibido.Value - resumen.Total, 2);

            pedido.Pago.Add(new Pago
            {
                MetodoPago = dto.MetodoPago,
                TipoTarjeta = dto.TipoTarjeta,
                UltimosDigitos = dto.UltimosDigitos,
                NombreTitular = dto.NombreTitular,
                MontoRecibido = dto.MontoRecibido,
                Vuelto = vuelto,
                FechaPago = DateTime.Now,
                Estado = estadoPago
            });

            await _repoPedido.CrearAsync(pedido);
            _carritoService.Vaciar();

            // Ya registrado y con el estado actualizado, se notifica al cliente:
            // el comprobante sale por correo y queda en su campana de avisos.
            // Se relee con los Include para armar el comprobante completo.
            var registrado = await _repoPedido.FindByIdAsync(pedido.IdPedido);
            var notificacion = registrado != null
                ? await _serviceNotificacion.NotificarPedidoRegistradoAsync(MapearDetalle(registrado, esGestor: false))
                : new ResultadoNotificacionDTO { CorreoEnviado = false, Detalle = "No se pudo recuperar el pedido para notificar." };

            return new PedidoResultadoDTO
            {
                IdPedido = pedido.IdPedido,
                Total = pedido.Total,
                Vuelto = vuelto,
                Estado = pedido.Estado,
                CorreoEnviado = notificacion.CorreoEnviado,
                DetalleNotificacion = notificacion.Detalle
            };
        }

        public async Task<PedidoHistorialViewModel> ObtenerHistorialAsync(DateTime? fechaDesde, DateTime? fechaHasta, string? estado)
        {
            var usuario = ObtenerUsuarioEnSesion();
            var esGestor = EsGestor(usuario.IdRol);

            // El cliente solo ve sus propios pedidos y sin filtros; el
            // administrador y el encargado ven todos y sí pueden filtrar.
            var pedidos = await _repoPedido.ListarHistorialAsync(
                idCliente: esGestor ? null : usuario.IdUsuario,
                fechaDesde: esGestor ? fechaDesde : null,
                fechaHasta: esGestor ? fechaHasta : null,
                estado: esGestor ? estado : null);

            return new PedidoHistorialViewModel
            {
                UsuarioActual = usuario,
                EsGestor = esGestor,
                FechaDesde = esGestor ? fechaDesde : null,
                FechaHasta = esGestor ? fechaHasta : null,
                Estado = esGestor ? estado : null,
                EstadosDisponibles = esGestor
                    ? (await _repoPedido.ListarEstadosAsync()).ToList()
                    : new List<string>(),
                Pedidos = pedidos.Select(p => new PedidoHistorialDTO
                {
                    IdPedido = p.IdPedido,
                    FechaPedido = p.FechaPedido,
                    Estado = p.Estado,
                    MetodoEntrega = p.MetodoEntrega,
                    NombreCliente = p.IdUsuarioNavigation?.NombreCompleto ?? "-",
                    NombreEncargado = p.IdEncargadoNavigation?.NombreCompleto,
                    CantidadArticulos = p.PedidoDetalle.Sum(d => d.Cantidad),
                    Total = p.Total
                }).ToList()
            };
        }

        public async Task<PedidoDetalleCompletoDTO?> ObtenerDetalleHistorialAsync(int idPedido)
        {
            var usuario = ObtenerUsuarioEnSesion();
            var esGestor = EsGestor(usuario.IdRol);

            var pedido = await _repoPedido.FindByIdAsync(idPedido);
            if (pedido == null)
                return null;

            // Quien no gestiona pedidos solo puede abrir el detalle de los propios.
            if (!esGestor && pedido.IdUsuario != usuario.IdUsuario)
                throw new UnauthorizedAccessException("El pedido no pertenece al usuario en sesión.");

            return MapearDetalle(pedido, esGestor);
        }

        private static PedidoDetalleCompletoDTO MapearDetalle(Pedido pedido, bool esGestor)
        {
            return new PedidoDetalleCompletoDTO
            {
                IdPedido = pedido.IdPedido,
                FechaPedido = pedido.FechaPedido,
                Estado = pedido.Estado,
                MetodoEntrega = pedido.MetodoEntrega,
                DireccionEntrega = pedido.DireccionEntrega,
                Observaciones = pedido.Observaciones,
                IdCliente = pedido.IdUsuario,
                NombreCliente = pedido.IdUsuarioNavigation?.NombreCompleto ?? "-",
                EmailCliente = pedido.IdUsuarioNavigation?.Email ?? "-",
                TelefonoCliente = pedido.IdUsuarioNavigation?.Telefono,
                NombreEncargado = pedido.IdEncargadoNavigation?.NombreCompleto,
                Subtotal = pedido.Subtotal,
                Impuesto = pedido.Impuesto,
                CostoEnvio = pedido.CostoEnvio,
                Total = pedido.Total,
                EsGestor = esGestor,
                Lineas = pedido.PedidoDetalle.Select(MapearLinea).ToList(),
                Pago = pedido.Pago
                    .OrderByDescending(pg => pg.FechaPago)
                    .Select(pg => new PagoHistorialDTO
                    {
                        MetodoPago = pg.MetodoPago,
                        TipoTarjeta = pg.TipoTarjeta,
                        UltimosDigitos = pg.UltimosDigitos,
                        NombreTitular = pg.NombreTitular,
                        MontoRecibido = pg.MontoRecibido,
                        Vuelto = pg.Vuelto,
                        FechaPago = pg.FechaPago,
                        Estado = pg.Estado
                    })
                    .FirstOrDefault()
            };
        }

        private static PedidoLineaHistorialDTO MapearLinea(PedidoDetalle detalle)
        {
            // Cada línea apunta a un producto o a un combo, nunca a los dos.
            var esCombo = detalle.IdCombo != null;

            var linea = new PedidoLineaHistorialDTO
            {
                IdDetalle = detalle.IdDetalle,
                Tipo = esCombo ? "combo" : "producto",
                Nombre = esCombo
                    ? detalle.IdComboNavigation?.Nombre ?? "Combo eliminado"
                    : detalle.IdProductoNavigation?.Nombre ?? "Producto eliminado",
                Imagen = esCombo
                    ? detalle.IdComboNavigation?.Imagen1
                    : detalle.IdProductoNavigation?.Imagen1,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Subtotal = detalle.Subtotal,
                Iva = detalle.Iva,
                Total = detalle.Total,
                Observaciones = detalle.Observaciones,
                Personalizado = detalle.Personalizado
            };

            var p = detalle.PedidoDetallePersonalizacion;
            if (p != null)
            {
                linea.Personalizacion = new PersonalizacionDTO
                {
                    IdProducto = detalle.IdProducto ?? string.Empty,
                    Cantidad = detalle.Cantidad,
                    Tamano = p.Tamano,
                    MedidaCm = p.MedidaCm,
                    SaborBizcocho = p.SaborBizcocho,
                    TipoRelleno = p.TipoRelleno,
                    TipoDecoracion = p.TipoDecoracion,
                    DecoracionDetalle = p.DecoracionDetalle,
                    RutaImagenReferencia = p.RutaImagenReferencia,
                    Dedicatoria = p.Dedicatoria,
                    PrecioExtra = p.PrecioExtra
                };
            }

            return linea;
        }

        /// <summary>
        /// Administrador y Encargado gestionan el historial completo.
        /// </summary>
        private static bool EsGestor(int idRol) =>
            idRol == ROL_ADMINISTRADOR || idRol == ROL_ENCARGADO;

        /// <summary>
        /// Toma los datos del usuario identificado en la sesión. Si nadie inició
        /// sesión no hay historial que mostrar.
        /// </summary>
        private UsuarioDTO ObtenerUsuarioEnSesion()
        {
            if (!_usuarioActual.EstaAutenticado)
                throw new UnauthorizedAccessException("Debe iniciar sesión para consultar el historial de pedidos.");

            return new UsuarioDTO
            {
                IdUsuario = _usuarioActual.IdUsuario,
                IdRol = _usuarioActual.IdRol,
                NombreCompleto = _usuarioActual.NombreCompleto,
                NombreRol = _usuarioActual.NombreRol,
                Email = _usuarioActual.Email,
                Telefono = _usuarioActual.Telefono,
                Direccion = _usuarioActual.Direccion
            };
        }
    }
}