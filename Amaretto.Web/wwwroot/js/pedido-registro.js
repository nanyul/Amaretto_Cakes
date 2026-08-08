(function () {
    let totalPedidoActual = parseFloat(document.getElementById('txtTotal')?.dataset.raw || '0');
    const totalActual = () => totalPedidoActual;

    const detalleBody = document.getElementById('detalleBody');
    const money = v => `₡${Number(v).toFixed(2)}`;

    function metodoEntregaActual() {
        return document.querySelector('input[name="metodoEntrega"]:checked').value;
    }

    // Solo dígitos. Se usa sobre inputs type=text porque en type=number el
    // navegador deja escribir "e", "+" y "-" y devuelve value vacío al leerlos.
    function soloNumeros(input) {
        const limpio = input.value.replace(/\D/g, '');
        if (input.value !== limpio) input.value = limpio;
    }

    /*  Detalle del pedido  */

    function filaHtml(l, indice) {
        const imagen = l.imagen || '/images/placeholder-producto.png';
        const obs = l.observaciones || '';
        return `
            <tr data-linea="${l.lineaId}" data-id="${l.idItem}" data-tipo="${l.tipo}">
                <td>${indice}</td>
                <td><img src="${imagen}" class="detalle-thumb" /> ${l.nombre}</td>
                <td class="precio-unit">${money(l.precioUnitario)}</td>
                <td>
                    <div class="qty-control-sm">
                        <button type="button" class="qty-btn-sm btn-menos">-</button>
                        <input type="text" class="qty-input-sm" inputmode="numeric"
                               autocomplete="off" value="${l.cantidad}" data-cantidad="${l.cantidad}" />
                        <button type="button" class="qty-btn-sm btn-mas">+</button>
                    </div>
                </td>
                <td class="celda-subtotal">${money(l.subtotal)}</td>
                <td class="celda-iva">${money(l.iva)}</td>
                <td class="celda-total">${money(l.total)}</td>
                <td><input type="text" class="form-control form-control-sm input-obs" value="${obs.replace(/"/g, '&quot;')}" /></td>
                <td><button type="button" class="btn-eliminar-linea"><i class="bi bi-trash"></i></button></td>
            </tr>`;
    }

    // Sincroniza la tabla con el carrito del servidor: agrega las líneas nuevas,
    // quita las que ya no están y refresca los montos de las que siguen.
    function pintarLineas(lineas) {
        const vistas = new Set();

        lineas.forEach(l => {
            vistas.add(l.lineaId);
            const fila = detalleBody.querySelector(`tr[data-linea="${l.lineaId}"]`);

            if (!fila) {
                detalleBody.insertAdjacentHTML('beforeend', filaHtml(l, 0));
                return;
            }

            fila.querySelector('.celda-subtotal').textContent = money(l.subtotal);
            fila.querySelector('.celda-iva').textContent = money(l.iva);
            fila.querySelector('.celda-total').textContent = money(l.total);

            const qty = fila.querySelector('.qty-input-sm');
            qty.dataset.cantidad = l.cantidad;
            // No se pisa mientras el usuario escribe en ese mismo campo
            if (document.activeElement !== qty) qty.value = l.cantidad;
        });

        detalleBody.querySelectorAll('tr[data-linea]').forEach(fila => {
            if (!vistas.has(fila.dataset.linea)) fila.remove();
        });

        // Renumera la columna "#"
        detalleBody.querySelectorAll('tr[data-linea]').forEach((fila, i) => {
            fila.querySelector('td').textContent = i + 1;
        });
    }

    function actualizarResumen() {
        return fetch(`/Pedido/Resumen?metodoEntrega=${metodoEntregaActual()}`)
            .then(r => r.json())
            .then(data => {
                document.getElementById('txtSubtotal').textContent = money(data.subtotal);
                document.getElementById('txtImpuesto').textContent = money(data.impuesto);
                document.getElementById('txtEnvio').textContent = money(data.costoEnvio);
                document.getElementById('txtTotal').textContent = money(data.total);
                document.getElementById('txtTotal').dataset.raw = data.total;
                document.getElementById('txtCostoEnvioHeader').textContent = money(data.costoEnvio);
                totalPedidoActual = data.total;

                pintarLineas(data.lineas);
                actualizarContadorCarrito();
            });
    }

    function enviarCantidad(fila, cantidad) {
        const cuerpo = `idItem=${encodeURIComponent(fila.dataset.id)}&tipo=${fila.dataset.tipo}` +
            `&lineaId=${encodeURIComponent(fila.dataset.linea)}&cantidad=${cantidad}`;

        return fetch('/Carrito/ActualizarCantidad', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: cuerpo
        }).then(r => {
            if (!r.ok) throw new Error('No se pudo actualizar la cantidad');
            return actualizarResumen();
        }).catch(() => {
            Swal.fire({ icon: 'error', title: 'No se pudo actualizar la cantidad' });
        });
    }

    // Método de entrega
    document.querySelectorAll('input[name="metodoEntrega"]').forEach(r => {
        r.addEventListener('change', function () {
            document.getElementById('direccionWrap').classList.toggle('d-none', this.value !== 'Domicilio');
            document.getElementById('envioWrap').classList.toggle('d-none', this.value !== 'Domicilio');
            actualizarResumen();
        });
    });

    // Botones + / - y eliminar
    detalleBody.addEventListener('click', function (e) {
        const fila = e.target.closest('tr');
        if (!fila) return;
        const input = fila.querySelector('.qty-input-sm');

        if (e.target.closest('.btn-mas') || e.target.closest('.btn-menos')) {
            const actual = parseInt(input.dataset.cantidad || input.value || '1', 10);
            const nueva = e.target.closest('.btn-mas') ? actual + 1 : Math.max(0, actual - 1);
            input.value = nueva;
            enviarCantidad(fila, nueva);
        }

        if (e.target.closest('.btn-eliminar-linea')) {
            fetch('/Carrito/Eliminar', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `idItem=${encodeURIComponent(fila.dataset.id)}&tipo=${fila.dataset.tipo}&lineaId=${encodeURIComponent(fila.dataset.linea)}`
            }).then(r => {
                if (!r.ok) throw new Error('No se pudo eliminar el ítem');
                return actualizarResumen();
            }).catch(() => {
                Swal.fire({ icon: 'error', title: 'No se pudo eliminar el producto' });
            });
        }
    });

    // Cantidad escrita a mano: se filtran los no dígitos y se envía con retardo.
    // Si la caja queda vacía NO se envía nada, así el usuario puede borrar el
    // número para escribir otro sin que se elimine la línea.
    let temporizadorCantidad;
    detalleBody.addEventListener('input', function (e) {
        if (!e.target.classList.contains('qty-input-sm')) return;

        soloNumeros(e.target);
        if (e.target.value === '') return;

        const fila = e.target.closest('tr');
        const cantidad = parseInt(e.target.value, 10);

        clearTimeout(temporizadorCantidad);
        temporizadorCantidad = setTimeout(() => enviarCantidad(fila, cantidad), 450);
    });

    // Al salir del campo vacío se restaura la última cantidad válida
    detalleBody.addEventListener('focusout', function (e) {
        if (!e.target.classList.contains('qty-input-sm')) return;
        if (e.target.value.trim() === '') e.target.value = e.target.dataset.cantidad || 1;
    });

    // Observaciones por línea
    detalleBody.addEventListener('change', function (e) {
        if (!e.target.classList.contains('input-obs')) return;
        const fila = e.target.closest('tr');

        fetch('/Carrito/ActualizarObservaciones', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `idItem=${encodeURIComponent(fila.dataset.id)}&tipo=${fila.dataset.tipo}` +
                `&lineaId=${encodeURIComponent(fila.dataset.linea)}&observaciones=${encodeURIComponent(e.target.value)}`
        }).then(r => {
            if (!r.ok) throw new Error('No se pudo guardar la observación');
        }).catch(() => {
            Swal.fire({ icon: 'error', title: 'No se pudo guardar la observación' });
        });
    });

    /*  Agregar línea desde el propio formulario  */

    const selItem = document.getElementById('selItem');
    const cantItem = document.getElementById('cantItem');
    const precioItem = document.getElementById('precioItem');

    fetch('/Pedido/ItemsDisponibles')
        .then(r => r.json())
        .then(data => {
            const grupo = (etiqueta, items) => {
                if (!items.length) return '';
                const opciones = items.map(i =>
                    `<option value="${i.id}" data-tipo="${i.tipo}" data-precio="${i.precio}">${i.nombre}</option>`
                ).join('');
                return `<optgroup label="${etiqueta}">${opciones}</optgroup>`;
            };
            selItem.insertAdjacentHTML('beforeend', grupo('Productos', data.productos) + grupo('Combos', data.combos));
        })
        .catch(() => {
            Swal.fire({ icon: 'error', title: 'No se pudo cargar el catálogo de ítems' });
        });

    selItem.addEventListener('change', function () {
        const opcion = this.selectedOptions[0];
        precioItem.textContent = money(opcion?.dataset.precio || 0);
    });

    cantItem.addEventListener('input', function () { soloNumeros(this); });
    cantItem.addEventListener('focusout', function () { if (this.value.trim() === '') this.value = 1; });

    document.getElementById('btnAgregarLinea').addEventListener('click', function () {
        const opcion = selItem.selectedOptions[0];
        if (!selItem.value) {
            Swal.fire({ icon: 'warning', title: 'Elegí un producto o combo' });
            return;
        }

        const cantidad = parseInt(cantItem.value || '0', 10);
        if (!cantidad || cantidad < 1) {
            Swal.fire({ icon: 'warning', title: 'Indicá una cantidad mayor a cero' });
            return;
        }

        const esCombo = opcion.dataset.tipo === 'combo';
        const url = esCombo ? '/Carrito/AgregarCombo' : '/Carrito/AgregarProducto';
        const campo = esCombo ? 'idCombo' : 'idProducto';

        fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `${campo}=${encodeURIComponent(selItem.value)}&cantidad=${cantidad}`
        })
            .then(async r => {
                const data = await r.json().catch(() => ({}));
                if (!r.ok) throw new Error(data.mensaje || 'No se pudo agregar el ítem');
                return data;
            })
            .then(() => {
                selItem.value = '';
                cantItem.value = 1;
                precioItem.textContent = money(0);
                return actualizarResumen();
            })
            .catch(err => {
                Swal.fire({ icon: 'error', title: 'No se pudo agregar', text: err.message });
            });
    });

    /*  Pago  */

    document.querySelectorAll('input[name="metodoPago"]').forEach(r => {
        r.addEventListener('change', function () {
            document.getElementById('panelTarjeta').classList.toggle('d-none', this.value !== 'Tarjeta');
            document.getElementById('panelEfectivo').classList.toggle('d-none', this.value !== 'Efectivo');
        });
    });

    document.getElementById('ultimosDigitos')?.addEventListener('input', function () {
        this.value = this.value.replace(/\D/g, '').slice(0, 4);
    });

    document.getElementById('montoRecibido')?.addEventListener('input', function () {
        if (this.value !== '' && parseFloat(this.value) < 0) this.value = 0;
        const vuelto = parseFloat(this.value || 0) - totalActual();
        document.getElementById('txtVuelto').textContent = money(Math.max(0, vuelto));
    });

    function validarPago(metodoPago) {
        if (metodoPago === 'Tarjeta') {
            const nombreTitular = document.getElementById('nombreTitular').value.trim();
            const ultimosDigitos = document.getElementById('ultimosDigitos').value.trim();

            if (!nombreTitular) {
                Swal.fire({ icon: 'warning', title: 'Indicá el nombre del titular' });
                return false;
            }
            if (nombreTitular.length < 3 || !/^[A-Za-zÁÉÍÓÚÑáéíóúñ\s.'-]+$/.test(nombreTitular)) {
                Swal.fire({ icon: 'warning', title: 'Nombre del titular inválido', text: 'Ingresá un nombre válido (solo letras).' });
                return false;
            }
            if (!/^\d{4}$/.test(ultimosDigitos)) {
                Swal.fire({ icon: 'warning', title: 'Últimos dígitos inválidos', text: 'Deben ser exactamente 4 números, sin letras ni símbolos.' });
                return false;
            }
        }

        if (metodoPago === 'Efectivo') {
            const montoInput = document.getElementById('montoRecibido');
            const monto = parseFloat(montoInput.value);

            if (montoInput.value.trim() === '' || isNaN(monto) || monto <= 0) {
                Swal.fire({ icon: 'warning', title: 'Indicá el monto recibido' });
                return false;
            }
            if (monto < totalActual()) {
                Swal.fire({ icon: 'warning', title: 'Monto insuficiente', text: 'El monto recibido no puede ser menor al total del pedido.' });
                return false;
            }
        }

        return true;
    }

    document.getElementById('btnGuardarPedido').addEventListener('click', function () {
        if (!detalleBody.querySelector('tr[data-linea]')) {
            Swal.fire({ icon: 'warning', title: 'Agregá al menos un producto o combo' });
            return;
        }
        const idCliente = document.getElementById('idCliente').value;
        if (!idCliente) {
            Swal.fire({ icon: 'warning', title: 'Seleccioná un cliente' });
            return;
        }
        if (metodoEntregaActual() === 'Domicilio' && !document.getElementById('direccionEntrega').value.trim()) {
            Swal.fire({ icon: 'warning', title: 'Indicá la dirección de entrega' });
            return;
        }
        new bootstrap.Modal(document.getElementById('modalPago')).show();
    });

    // Confirmar pago -> guardar pedido. La notificación al usuario la da el
    // comprobante del servidor, no un mensaje de JavaScript.
    document.getElementById('btnConfirmarPago').addEventListener('click', function () {
        const metodoPago = document.querySelector('input[name="metodoPago"]:checked').value;
        if (!validarPago(metodoPago)) return;

        const boton = this;
        boton.disabled = true;

        const dto = {
            idCliente: parseInt(document.getElementById('idCliente').value),
            idEncargado: document.getElementById('idEncargado').value ? parseInt(document.getElementById('idEncargado').value) : null,
            metodoEntrega: metodoEntregaActual(),
            direccionEntrega: document.getElementById('direccionEntrega').value,
            observaciones: null,
            metodoPago: metodoPago,
            tipoTarjeta: metodoPago === 'Tarjeta' ? document.getElementById('tipoTarjeta').value : null,
            ultimosDigitos: metodoPago === 'Tarjeta' ? document.getElementById('ultimosDigitos').value : null,
            nombreTitular: metodoPago === 'Tarjeta' ? document.getElementById('nombreTitular').value : null,
            montoRecibido: metodoPago === 'Efectivo' ? parseFloat(document.getElementById('montoRecibido').value || 0) : null
        };

        fetch('/Pedido/Guardar', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        })
            .then(async r => {
                let data;
                try {
                    data = await r.json();
                } catch {
                    throw new Error('El servidor respondió con un error inesperado.');
                }
                if (!r.ok) throw new Error(data.mensaje || 'No se pudo guardar el pedido.');
                return data;
            })
            .then(data => {
                bootstrap.Modal.getInstance(document.getElementById('modalPago')).hide();
                window.location.href = `/Pedido/Comprobante/${data.resultado.idPedido}`;
            })
            .catch(err => {
                boton.disabled = false;
                Swal.fire({ icon: 'error', title: 'No se pudo guardar', text: err.message });
            });
    });

    /*  Búsqueda de clientes (solo Encargado)  */

    const buscarClienteInput = document.getElementById('buscarCliente');
    const resultadosDiv = document.getElementById('resultadosBusqueda');
    let temporizadorBusqueda;

    if (buscarClienteInput) {
        const ficha = document.getElementById('fichaCliente');
        const fichaVacia = document.getElementById('fichaClienteVacia');

        buscarClienteInput.addEventListener('input', function () {
            const termino = this.value.trim();
            clearTimeout(temporizadorBusqueda);

            if (termino.length < 2) {
                resultadosDiv.style.display = 'none';
                return;
            }

            temporizadorBusqueda = setTimeout(() => {
                fetch(`/Pedido/BuscarClientes?termino=${encodeURIComponent(termino)}`)
                    .then(r => r.json())
                    .then(data => {
                        if (data.length === 0) {
                            resultadosDiv.innerHTML = '<div class="dropdown-item text-muted">No se encontraron clientes</div>';
                        } else {
                            // La lista muestra solo el nombre; el correo, el teléfono y la
                            // dirección se despliegan en la ficha al seleccionar.
                            resultadosDiv.innerHTML = data.map(c =>
                                `<a class="dropdown-item" href="#" data-id="${c.idUsuario}"
                                    data-nombre="${c.nombreCompleto}" data-telefono="${c.telefono || ''}"
                                    data-email="${c.email || ''}" data-direccion="${c.direccion || ''}">
                                    <i class="bi bi-person-circle me-2"></i>${c.nombreCompleto}
                                </a>`
                            ).join('');
                        }
                        resultadosDiv.style.display = 'block';
                    });
            }, 300);
        });

        resultadosDiv.addEventListener('click', function (e) {
            const item = e.target.closest('.dropdown-item[data-id]');
            if (!item) return;
            e.preventDefault();

            document.getElementById('idCliente').value = item.dataset.id;
            document.getElementById('fichaNombre').textContent = item.dataset.nombre;
            document.getElementById('fichaCorreo').textContent = item.dataset.email || '-';
            document.getElementById('fichaTelefono').textContent = item.dataset.telefono || '-';
            document.getElementById('fichaDireccion').textContent = item.dataset.direccion || '-';
            document.getElementById('direccionEntrega').value = item.dataset.direccion;

            ficha.classList.remove('d-none');
            fichaVacia.classList.add('d-none');

            buscarClienteInput.value = item.dataset.nombre;
            resultadosDiv.style.display = 'none';
        });

        document.addEventListener('click', function (e) {
            if (!resultadosDiv.contains(e.target) && e.target !== buscarClienteInput) {
                resultadosDiv.style.display = 'none';
            }
        });
    }
})();
