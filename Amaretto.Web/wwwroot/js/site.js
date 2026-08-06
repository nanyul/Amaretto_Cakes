// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function actualizarContadorCarrito() {
    fetch('/Carrito/Cantidad')
        .then(r => r.json())
        .then(data => {
            const badge = document.getElementById('cartBadge');
            if (badge) badge.textContent = data.cantidadTotal;
        })
        .catch(err => console.error('Error al actualizar el carrito', err));
}

document.addEventListener('DOMContentLoaded', actualizarContadorCarrito);
window.actualizarContadorCarrito = actualizarContadorCarrito;