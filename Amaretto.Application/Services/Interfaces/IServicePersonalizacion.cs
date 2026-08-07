using Amaretto.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServicePersonalizacion
    {
        List<string> ObtenerTematicas();
        decimal CalcularPrecioExtra(PersonalizacionDTO dto);
    }
}
