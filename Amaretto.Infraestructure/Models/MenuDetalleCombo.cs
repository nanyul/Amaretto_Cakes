using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class MenuDetalleCombo
{
    public int IdMenuDetalleCombo { get; set; }

    public int IdMenuCombo { get; set; }

    public string IdCombo { get; set; } = null!;

    public virtual Combo IdComboNavigation { get; set; } = null!;

    public virtual MenuCombo IdMenuComboNavigation { get; set; } = null!;
}
