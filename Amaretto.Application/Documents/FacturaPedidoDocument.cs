using Amaretto.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace Amaretto.Application.Documents
{
    public class FacturaPedidoDocument : IDocument
    {
        // Paleta de amaretto.css
        private const string VerdeOscuro = "#0A3323";
        private const string VerdeMusgo = "#839958";
        private const string RosaPalo = "#D3968C";
        private const string Beige = "#F7F4D5";
        private const string BeigeClaro = "#FFFDF6";
        private const string AzulNoche = "#105666";

        private static readonly CultureInfo Cultura = new("es-CR");

        private readonly PedidoDetalleCompletoDTO _pedido;

        public FacturaPedidoDocument(PedidoDetalleCompletoDTO pedido)
        {
            _pedido = pedido;
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = $"Factura Pedido #{_pedido.IdPedido}",
            Author = "Amaretto Cakes",
            Subject = "Comprobante de pedido"
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(VerdeOscuro).FontFamily(Fonts.Calibri));

                page.Header().Element(ComponerEncabezado);
                page.Content().Element(ComponerContenido);
                page.Footer().Element(ComponerPie);
            });
        }

        private void ComponerEncabezado(IContainer container)
        {
            container.PaddingBottom(18).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Amaretto Cakes")
                        .FontSize(24).Bold().FontColor(VerdeOscuro);
                    col.Item().Text("Pastelería artesanal")
                        .FontSize(9).Italic().FontColor(RosaPalo);
                    col.Item().PaddingTop(4).Text("San José, Costa Rica  ·  +506 8800 0000")
                        .FontSize(8).FontColor(AzulNoche);
                });

                row.ConstantItem(180).Column(col =>
                {
                    col.Item().AlignRight().Text("FACTURA")
                        .FontSize(20).Bold().FontColor(VerdeMusgo);
                    col.Item().AlignRight().Text($"Pedido #{_pedido.IdPedido}")
                        .FontSize(12).SemiBold();
                    col.Item().AlignRight().Text(_pedido.FechaPedido.ToString("dd/MM/yyyy hh:mm tt", Cultura))
                        .FontSize(9).FontColor(AzulNoche);
                    col.Item().PaddingTop(6).AlignRight().Background(Beige)
                        .PaddingVertical(3).PaddingHorizontal(10)
                        .Text(_pedido.Estado).FontSize(9).Bold().FontColor(VerdeOscuro);
                });
            });
        }

        private void ComponerContenido(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(14);
                col.Item().Element(ComponerDatosGenerales);
                col.Item().Element(ComponerTablaDetalle);
                col.Item().Element(ComponerTotales);

                if (!string.IsNullOrWhiteSpace(_pedido.Observaciones))
                    col.Item().Element(ComponerObservaciones);
            });
        }

        /*  Encabezado del pedido  */

        private void ComponerDatosGenerales(IContainer container)
        {
            container.Row(row =>
            {
                row.Spacing(12);

                row.RelativeItem().Element(c => Ficha(c, "CLIENTE", col =>
                {
                    col.Item().Text(_pedido.NombreCliente).SemiBold();
                    col.Item().Text(_pedido.EmailCliente).FontSize(8).FontColor(AzulNoche);
                    if (!string.IsNullOrWhiteSpace(_pedido.TelefonoCliente))
                        col.Item().Text(_pedido.TelefonoCliente).FontSize(8).FontColor(AzulNoche);
                }));

                row.RelativeItem().Element(c => Ficha(c, "ENCARGADO", col =>
                {
                    var encargado = string.IsNullOrWhiteSpace(_pedido.NombreEncargado)
                        ? "Pedido en línea"
                        : _pedido.NombreEncargado;

                    col.Item().Text(encargado).SemiBold();
                    col.Item().Text(string.IsNullOrWhiteSpace(_pedido.NombreEncargado)
                            ? "Registrado por el cliente"
                            : "Registrado en tienda")
                        .FontSize(8).FontColor(AzulNoche);
                }));

                row.RelativeItem().Element(c => Ficha(c, "ENTREGA", col =>
                {
                    col.Item().Text(_pedido.MetodoEntrega).SemiBold();
                    col.Item().Text(string.IsNullOrWhiteSpace(_pedido.DireccionEntrega)
                            ? "Retiro en tienda"
                            : _pedido.DireccionEntrega)
                        .FontSize(8).FontColor(AzulNoche);
                }));

                row.RelativeItem().Element(c => Ficha(c, "PAGO", col =>
                {
                    if (_pedido.Pago == null)
                    {
                        col.Item().Text("Sin pago registrado").SemiBold();
                        return;
                    }

                    col.Item().Text(_pedido.Pago.MetodoPago).SemiBold();

                    if (!string.IsNullOrWhiteSpace(_pedido.Pago.TipoTarjeta))
                    {
                        var tarjeta = _pedido.Pago.TipoTarjeta;
                        if (!string.IsNullOrWhiteSpace(_pedido.Pago.UltimosDigitos))
                            tarjeta += $" ····{_pedido.Pago.UltimosDigitos}";
                        col.Item().Text(tarjeta).FontSize(8).FontColor(AzulNoche);
                    }

                    if (_pedido.Pago.MontoRecibido.HasValue)
                    {
                        col.Item().Text($"Recibido {Moneda(_pedido.Pago.MontoRecibido.Value)}")
                            .FontSize(8).FontColor(AzulNoche);
                        col.Item().Text($"Vuelto {Moneda(_pedido.Pago.Vuelto ?? 0m)}")
                            .FontSize(8).FontColor(AzulNoche);
                    }
                }));
            });
        }

        private static void Ficha(IContainer container, string titulo, Action<ColumnDescriptor> contenido)
        {
            container
                .Border(1).BorderColor(VerdeMusgo)
                .Background(BeigeClaro)
                .Padding(9)
                .Column(col =>
                {
                    col.Item().PaddingBottom(4).Text(titulo)
                        .FontSize(7).Bold().FontColor(VerdeMusgo).LetterSpacing(0.08f);
                    contenido(col);
                });
        }

        /*  Líneas de detalle  */

        private void ComponerTablaDetalle(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);      // Producto / combo
                    columns.ConstantColumn(62);     // Precio
                    columns.ConstantColumn(40);     // Cantidad
                    columns.ConstantColumn(62);     // Subtotal
                    columns.ConstantColumn(58);     // Impuesto
                    columns.ConstantColumn(66);     // Total
                });

                table.Header(header =>
                {
                    header.Cell().Element(CeldaEncabezado).Text("Producto / Combo");
                    header.Cell().Element(CeldaEncabezado).AlignRight().Text("Precio");
                    header.Cell().Element(CeldaEncabezado).AlignCenter().Text("Cant.");
                    header.Cell().Element(CeldaEncabezado).AlignRight().Text("Subtotal");
                    header.Cell().Element(CeldaEncabezado).AlignRight().Text("Impuesto");
                    header.Cell().Element(CeldaEncabezado).AlignRight().Text("Total");
                });

                foreach (var linea in _pedido.Lineas)
                {
                    table.Cell().Element(Celda).Column(col =>
                    {
                        col.Item().Text(linea.Nombre).SemiBold();

                        if (linea.Personalizado && linea.Personalizacion != null)
                        {
                            var p = linea.Personalizacion;
                            col.Item().Text($"Personalizado · {p.Tamano} {p.MedidaCm} cm · {p.SaborBizcocho} · {p.TipoRelleno}")
                                .FontSize(7).FontColor(VerdeMusgo);
                            if (!string.IsNullOrWhiteSpace(p.Dedicatoria))
                                col.Item().Text($"Dedicatoria: {p.Dedicatoria}")
                                    .FontSize(7).Italic().FontColor(AzulNoche);
                        }

                        if (!string.IsNullOrWhiteSpace(linea.Observaciones))
                            col.Item().Text($"Obs: {linea.Observaciones}")
                                .FontSize(7).Italic().FontColor(AzulNoche);
                    });

                    table.Cell().Element(Celda).AlignRight().Text(Moneda(linea.PrecioUnitario));
                    table.Cell().Element(Celda).AlignCenter().Text(linea.Cantidad.ToString());
                    table.Cell().Element(Celda).AlignRight().Text(Moneda(linea.Subtotal));
                    table.Cell().Element(Celda).AlignRight().Text(Moneda(linea.Iva));
                    table.Cell().Element(Celda).AlignRight().Text(Moneda(linea.Total)).SemiBold();
                }
            });
        }

        private static IContainer CeldaEncabezado(IContainer container) =>
            container.Background(VerdeMusgo).PaddingVertical(6).PaddingHorizontal(6)
                     .DefaultTextStyle(x => x.FontColor(BeigeClaro).Bold().FontSize(8));

        private static IContainer Celda(IContainer container) =>
            container.BorderBottom(1).BorderColor(Beige).PaddingVertical(6).PaddingHorizontal(6);

        /*  Totales  */

        private void ComponerTotales(IContainer container)
        {
            container.AlignRight().Width(250).Column(col =>
            {
                col.Item().Element(c => FilaTotal(c, "Total sin impuestos", Moneda(_pedido.Subtotal)));
                col.Item().Element(c => FilaTotal(c, "Impuesto (13%)", Moneda(_pedido.Impuesto)));
                col.Item().Element(c => FilaTotal(c, "Costo de envío", Moneda(_pedido.CostoEnvio)));

                col.Item().PaddingTop(6).Background(VerdeOscuro).Padding(9).Row(row =>
                {
                    row.RelativeItem().Text("TOTAL CON IMPUESTO")
                        .FontSize(10).Bold().FontColor(BeigeClaro);
                    row.ConstantItem(95).AlignRight().Text(Moneda(_pedido.Total))
                        .FontSize(12).Bold().FontColor(Beige);
                });
            });
        }

        private static void FilaTotal(IContainer container, string etiqueta, string valor)
        {
            container.PaddingVertical(3).Row(row =>
            {
                row.RelativeItem().Text(etiqueta).FontSize(9).FontColor(AzulNoche);
                row.ConstantItem(95).AlignRight().Text(valor).FontSize(9).SemiBold();
            });
        }

        private void ComponerObservaciones(IContainer container)
        {
            container.Background(Beige).Padding(10).Column(col =>
            {
                col.Item().Text("OBSERVACIONES DEL PEDIDO")
                    .FontSize(7).Bold().FontColor(VerdeMusgo).LetterSpacing(0.08f);
                col.Item().PaddingTop(3).Text(_pedido.Observaciones).FontSize(8).Italic();
            });
        }

        private void ComponerPie(IContainer container)
        {
            container.BorderTop(1).BorderColor(Beige).PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text("Gracias por tu compra · Amaretto Cakes")
                    .FontSize(8).Italic().FontColor(RosaPalo);

                row.ConstantItem(100).AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(AzulNoche));
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        }

        private static string Moneda(decimal valor) => "₡" + valor.ToString("N2", Cultura);
    }
}
