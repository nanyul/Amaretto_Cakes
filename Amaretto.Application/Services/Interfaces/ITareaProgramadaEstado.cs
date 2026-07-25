using System;
using System.Collections.Generic;

namespace Amaretto.Application.Services.Interfaces
{
    public interface ITareaProgramadaEstado
    {
        DateTime? UltimaEjecucion { get; }
        int UltimaCantidadDesactivados { get; }
        List<string> UltimoDetalle { get; }

        void RegistrarEjecucion(int cantidadDesactivados, List<string> detalle);
    }
}
