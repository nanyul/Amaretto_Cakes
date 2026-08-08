// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*  Cantidad del pedido en el encabezado  */
// Muestra la sumatoria de las cantidades de todos los productos y combos del
// pedido. Se llama desde cada acción que modifica el carrito.
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

/*  Campana de notificaciones  */

(function () {
    const campana = document.getElementById('campanaNotificaciones');
    if (!campana) return; // no hay sesión iniciada

    const badge = document.getElementById('notifBadge');
    const lista = document.getElementById('notifLista');
    const btnMarcar = document.getElementById('btnMarcarLeidas');

    function escapar(texto) {
        const div = document.createElement('div');
        div.textContent = texto ?? '';
        return div.innerHTML;
    }

    function pintar(data) {
        badge.textContent = data.noLeidas;
        badge.classList.toggle('d-none', data.noLeidas === 0);

        if (!data.notificaciones.length) {
            lista.innerHTML = '<p class="notif-vacio">Todavía no tenés notificaciones.</p>';
            return;
        }

        lista.innerHTML = data.notificaciones.map(n => {
            const destino = n.idPedido ? `/Pedido/Detalle/${n.idPedido}` : '#';
            const icono = n.correoEnviado ? 'bi-envelope-check' : 'bi-bell';
            return `
                <a class="notif-item ${n.leida ? '' : 'is-nueva'}" href="${destino}">
                    <i class="bi ${icono} notif-item-icono"></i>
                    <span class="notif-item-cuerpo">
                        <span class="notif-item-titulo">${escapar(n.titulo)}</span>
                        <span class="notif-item-texto">${escapar(n.mensaje)}</span>
                        <span class="notif-item-fecha">${escapar(n.fecha)}</span>
                    </span>
                </a>`;
        }).join('');
    }

    function cargarNotificaciones() {
        fetch('/Notificacion/Mias')
            .then(r => r.json())
            .then(pintar)
            .catch(() => {
                lista.innerHTML = '<p class="notif-vacio">No se pudieron cargar las notificaciones.</p>';
            });
    }

    btnMarcar?.addEventListener('click', function (e) {
        e.stopPropagation();
        fetch('/Notificacion/MarcarLeidas', { method: 'POST' })
            .then(r => r.json())
            .then(cargarNotificaciones)
            .catch(() => console.error('No se pudieron marcar las notificaciones'));
    });

    campana.addEventListener('click', cargarNotificaciones);
    document.addEventListener('DOMContentLoaded', cargarNotificaciones);
})();
