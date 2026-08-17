using Amaretto.Application.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Amaretto.Application.Services.Implementations
{
    public class TareaProgramadaEstado : ITareaProgramadaEstado
    {
        private readonly object _lock = new();
        private DateTime? _ultimaEjecucion;
        private int _ultimaCantidad;
        private List<string> _ultimoDetalle = new();

        public DateTime? UltimaEjecucion
        {
            get { lock (_lock) { return _ultimaEjecucion; } }
        }

        public int UltimaCantidadDesactivados
        {
            get { lock (_lock) { return _ultimaCantidad; } }
        }

        public List<string> UltimoDetalle
        {
            get { lock (_lock) { return new List<string>(_ultimoDetalle); } }
        }

        public void RegistrarEjecucion(int cantidadDesactivados, List<string> detalle)
        {
            lock (_lock)
            {
                _ultimaEjecucion = DateTime.Now;
                _ultimaCantidad = cantidadDesactivados;
                _ultimoDetalle = detalle ?? new List<string>();
            }
        }
    }
}
