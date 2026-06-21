using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Models
{
    public class ComboProducto
    {
        public string IdCombo { get; set; } = null!;
        public string IdProducto { get; set; } = null!;

        public virtual Combo IdComboNavigation { get; set; } = null!;
        public virtual Producto IdProductoNavigation { get; set; } = null!;
    }
}
