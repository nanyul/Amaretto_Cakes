using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    /// <summary>
    /// Datos del formulario de crear y editar usuarios. La contraseña viaja en
    /// texto plano solo desde el formulario hasta el servicio, que es el único
    /// que la encripta antes de guardarla.
    /// </summary>
    public class UsuarioMantenimientoDTO
    {
        [ValidateNever]
        public int IdUsuario { get; set; }

        [Display(Name = "Nombre completo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string NombreCompleto { get; set; } = null!;

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [EmailAddress(ErrorMessage = "Ingrese un {0} válido")]
        [StringLength(150, ErrorMessage = "{0} no puede superar los {1} caracteres")]
        public string Email { get; set; } = null!;

        [Display(Name = "Rol")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public int IdRol { get; set; }

        [Display(Name = "Teléfono")]
        [StringLength(20, ErrorMessage = "{0} no puede superar los {1} caracteres")]
        public string? Telefono { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(300, ErrorMessage = "{0} no puede superar los {1} caracteres")]
        public string? Direccion { get; set; }

        [Display(Name = "Sexo")]
        [StringLength(1)]
        public string? Sexo { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        /// <summary>
        /// Al crear es obligatoria. Al editar, si se deja vacía se conserva la
        /// contraseña que el usuario ya tenía.
        /// </summary>
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        /// <summary>
        /// Distingue el alta de la edición para aplicar las reglas de contraseña.
        /// </summary>
        [ValidateNever]
        public bool EsEdicion => IdUsuario > 0;
    }
}
