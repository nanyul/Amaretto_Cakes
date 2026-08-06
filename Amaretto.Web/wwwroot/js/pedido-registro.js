(function () {
    let totalPedidoActual = parseFloat(document.getElementById('txtTotal')?.dataset.raw || '0');
    const totalActual = () => totalPedidoActual;

    function metodoEntregaActual() {
        return document.querySelector('input[name="metodoEntrega"]:checked').value;
    }

    function actualizarResumen() {
        fetch(`/Pedido/Resumen?metodoEntrega=${metodoEntregaActual()}`)
            .then(r => r.json())
            .then(data => {
                document.getElementById('txtSubtotal').textContent = `₡${data.subtotal.toFixed(2)}`;
                document.getElementById('txtImpuesto').textContent = `₡${data.impuesto.toFixed(2)}`;
                document.getElementById('txtEnvio').textContent = `₡${data.costoEnvio.toFixed(2)}`;
                document.getElementById('txtTotal').textContent = `₡${data.total.toFixed(2)}`;
                document.getElementById('txtTotal').dataset.raw = data.total;
                totalPedidoActual = data.total;

                data.lineas.forEach(l => {
                    const fila = document.querySelector(`tr[data-id="${l.idItem}"][data-tipo="${l.tipo}"]`);
                    if (!fila) return;
                    fila.querySelector('.celda-subtotal').textContent = `₡${l.subtotal.toFixed(2)}`;
                    fila.querySelector('.celda-iva').textContent = `₡${l.iva.toFixed(2)}`;
                    fila.querySelector('.celda-total').textContent = `₡${l.total.toFixed(2)}`;
                });
                actualizarContadorCarrito();
            });
    }

    // Método de entrega
    document.querySelectorAll('input[name="metodoEntrega"]').forEach(r => {
        r.addEventListener('change', function () {
            document.getElementById('direccionWrap').classList.toggle('d-none', this.value !== 'Domicilio');
            actualizarResumen();
        });
    });

    // Cliente: al seleccionar, autocompletar teléfono/correo/ID y cargar su dirección guardada (editable)
    const selectCliente = document.getElementById('idCliente');
    if (selectCliente && selectCliente.tagName === 'SELECT') {
        selectCliente.addEventListener('change', function () {
            const opt = this.selectedOptions[0];
            const telefono = opt?.dataset.telefono || '';
            const email = opt?.dataset.email || '';
            const direccion = opt?.dataset.direccion || '';

            const telefonoInput = document.getElementById('clienteTelefono');
            const correoInput = document.getElementById('clienteCorreo');
            const idInput = document.getElementById('clienteId');
            if (telefonoInput) telefonoInput.value = telefono;
            if (correoInput) correoInput.value = email;
            if (idInput) idInput.value = this.value ? `C-${this.value}` : '';

            const direccionInput = document.getElementById('direccionEntrega');
            if (direccionInput) direccionInput.value = direccion;
        });
    }


    // Cantidad +/-
    document.getElementById('detalleBody').addEventListener('click', function (e) {
        const fila = e.target.closest('tr');
        if (!fila) return;
        const input = fila.querySelector('.qty-input-sm');
        const idItem = fila.dataset.id, tipo = fila.dataset.tipo;

        if (e.target.closest('.btn-mas')) input.value = parseInt(input.value) + 1;
        if (e.target.closest('.btn-menos')) input.value = Math.max(0, parseInt(input.value) - 1);

        if (e.target.closest('.btn-mas') || e.target.closest('.btn-menos')) {
            fetch('/Carrito/ActualizarCantidad', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `idItem=${encodeURIComponent(idItem)}&tipo=${tipo}&cantidad=${input.value}`
            }).then(r => {
                if (!r.ok) throw new Error('No se pudo actualizar la cantidad');
                if (parseInt(input.value) === 0) fila.remove();
                actualizarResumen();
            }).catch(() => {
                Swal.fire({ icon: 'error', title: 'No se pudo actualizar la cantidad' });
            });
        }

        if (e.target.closest('.btn-eliminar-linea')) {
            fetch('/Carrito/Eliminar', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `idItem=${encodeURIComponent(idItem)}&tipo=${tipo}`
            }).then(r => {
                if (!r.ok) throw new Error('No se pudo eliminar el ítem');
                fila.remove();
                actualizarResumen();
            }).catch(() => {
                Swal.fire({ icon: 'error', title: 'No se pudo eliminar el producto' });
            });
        }
    });

    // Cantidad escrita directo + Observaciones
    document.getElementById('detalleBody').addEventListener('change', function (e) {
        const fila = e.target.closest('tr');
        const idItem = fila.dataset.id, tipo = fila.dataset.tipo;

        if (e.target.classList.contains('qty-input-sm')) {
            fetch('/Carrito/ActualizarCantidad', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `idItem=${encodeURIComponent(idItem)}&tipo=${tipo}&cantidad=${e.target.value}`
            }).then(r => {
                if (!r.ok) throw new Error('No se pudo actualizar la cantidad');
                if (parseInt(e.target.value) === 0) fila.remove();
                actualizarResumen();
            }).catch(() => {
                Swal.fire({ icon: 'error', title: 'No se pudo actualizar la cantidad' });
            });
        }

        if (e.target.classList.contains('input-obs')) {
            fetch('/Carrito/ActualizarObservaciones', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `idItem=${encodeURIComponent(idItem)}&tipo=${tipo}&observaciones=${encodeURIComponent(e.target.value)}`
            }).then(r => {
                if (!r.ok) throw new Error('No se pudo guardar la observación');
            }).catch(() => {
                Swal.fire({ icon: 'error', title: 'No se pudo guardar la observación' });
            });
        }
    });

    // Modal de pago: toggle tarjeta/efectivo
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
        document.getElementById('txtVuelto').textContent = `₡${Math.max(0, vuelto).toFixed(2)}`;
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

    // Abrir modal
    document.getElementById('btnGuardarPedido').addEventListener('click', function () {
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

    // Confirmar pago -> guardar pedido completo
    document.getElementById('btnConfirmarPago').addEventListener('click', function () {
        const metodoPago = document.querySelector('input[name="metodoPago"]:checked').value;

        if (!validarPago(metodoPago)) return;

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
                if (!r.ok) {
                    throw new Error(data.mensaje || 'No se pudo guardar el pedido.');
                }
                return data;
            })
            .then(data => {
                bootstrap.Modal.getInstance(document.getElementById('modalPago')).hide();
                Swal.fire({
                    icon: 'success',
                    title: 'Pedido registrado',
                    text: `Total: ₡${data.resultado.total.toFixed(2)}` + (data.resultado.vuelto ? ` — Vuelto: ₡${data.resultado.vuelto.toFixed(2)}` : ''),
                }).then(() => window.location.href = '/Home/Index');
            })
            .catch(err => {
                Swal.fire({ icon: 'error', title: 'No se pudo guardar', text: err.message });
            });
    });

    // Simulación temporal de usuario (quitar cuando exista login)
    function cargarUsuariosSimulacion() {
        const rol = document.getElementById('simRol').value;
        fetch(`/Pedido/ObtenerUsuariosPorRol?rol=${rol}`)
            .then(r => r.json())
            .then(data => {
                const sel = document.getElementById('simUsuario');
                sel.innerHTML = data.map(u => `<option value="${u.idUsuario}">${u.nombreCompleto}</option>`).join('');
            });
    }
    document.getElementById('simRol')?.addEventListener('change', cargarUsuariosSimulacion);
    cargarUsuariosSimulacion();

    document.getElementById('btnAplicarSim')?.addEventListener('click', function () {
        const idUsuario = document.getElementById('simUsuario').value;
        const rol = document.getElementById('simRol').value;
        if (!idUsuario) return;
        fetch('/Pedido/SimularUsuario', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `idUsuario=${idUsuario}&rol=${rol}`
        }).then(() => location.reload());
    });
})();