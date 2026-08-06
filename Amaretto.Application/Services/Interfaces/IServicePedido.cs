using Amaretto.Application.DTOs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServicePedido
    {
        Task<PedidoRegistroViewModel> PrepararRegistroAsync();
        ResumenPedidoDTO ObtenerResumen(string metodoEntrega);
        Task<PedidoResultadoDTO> RegistrarPedidoAsync(PedidoRegistroDTO dto);
    }
}
