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

        private readonly IServiceCarrito _carritoService;
        private readonly IRepositoryPedido _repoPedido;
        private readonly IMapper _mapper;

        public ServicePedido(
            IServiceCarrito carritoService,
            IRepositoryPedido repoPedido,
            IMapper mapper)
        {
            _carritoService = carritoService;
            _repoPedido = repoPedido;
            _mapper = mapper;
        }

        public async Task<PedidoRegistroViewModel> PrepararRegistroAsync(IServiceUsuarioActual usuarioActual)
        {
            var esEncargado = usuarioActual.EstaAutenticado && usuarioActual.IdRol == 4; // Encargado = 4

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

            return new PedidoResultadoDTO
            {
                IdPedido = pedido.IdPedido,
                Total = pedido.Total,
                Vuelto = vuelto
            };
        }
    }
}