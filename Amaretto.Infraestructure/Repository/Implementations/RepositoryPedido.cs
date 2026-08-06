using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Implementations
{
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly AmarettoContext _context;

        public RepositoryPedido(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<Pedido> CrearAsync(Pedido pedido)
        {
            // Al agregar el Pedido con sus colecciones de PedidoDetalle y Pago ya
            // pobladas, EF inserta el padre primero y luego los hijos en un solo
            // SaveChanges, respetando el orden de FKs automáticamente.
            _context.Pedido.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }
    }
}
