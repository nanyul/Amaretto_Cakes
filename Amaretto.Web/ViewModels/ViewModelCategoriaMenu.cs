using Amaretto.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Amaretto.Web.ViewModels
{
    public class ViewModelCategoriaMenu
    {
        public string Categoria { get; set; }

        public List<ProductoDTO> Productos { get; set; }
                = new();
    }
}
