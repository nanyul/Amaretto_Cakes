(function () {
    const RECARGO_TAMANO = { "Pequeño": 0, "Mediano": 2500, "Grande": 5000 };
    const RECARGO_DECORACION = { "Sin decoración especial": 0, "Temática de catálogo": 3000, "Imagen de referencia": 7000 };
    const CM_TAMANO = { "Pequeño": 15, "Mediano": 20, "Grande": 25 };

    function calcularExtra() {
        const tamano = document.querySelector('input[name="Tamano"]:checked')?.value;
        const decoracion = document.querySelector('input[name="TipoDecoracion"]:checked')?.value;
        return (RECARGO_TAMANO[tamano] || 0) + (RECARGO_DECORACION[decoracion] || 0);
    }

    function actualizarResumen() {
        const extra = calcularExtra();
        const texto = `₡${extra.toFixed(2)}`;
        document.getElementById('txtPrecioExtra').textContent = texto;
        document.getElementById('txtPrecioExtraTotal').textContent = texto;
        document.getElementById('txtPrecioExtraFinal').textContent = texto;
        actualizarResumenFinal();
    }

    // Sincroniza la sección "2. Resumen" con lo elegido en el formulario.
    function actualizarResumenFinal() {
        const tamano = document.querySelector('input[name="Tamano"]:checked')?.value || '';
        const cm = CM_TAMANO[tamano] || document.getElementById('medidaCmHidden').value;
        const sabor = document.getElementById('selectSabor').value;
        const relleno = document.getElementById('selectRelleno').value;
        const tipoDecoracion = document.querySelector('input[name="TipoDecoracion"]:checked')?.value || '';

        let partes = [];
        if (tamano) partes.push(`${tamano} (${cm} cm)`);
        if (sabor) partes.push(sabor);
        if (relleno) partes.push(relleno);
        let texto = partes.join(' • ');

        if (tipoDecoracion === 'Temática de catálogo') {
            const tematica = document.getElementById('selectTematica').value;
            if (tematica) texto += `\nTemática: ${tematica}`;
        }

        document.getElementById('txtCaracteristicas').textContent = texto;

        const dedicatoria = document.getElementById('dedicatoriaInput').value.trim();
        document.getElementById('txtDedicatoriaResumen').textContent =
            dedicatoria ? `"${dedicatoria}"` : 'Escribe tu dedicatoria arriba.';
    }

    document.querySelectorAll('input[name="Tamano"]').forEach(r => {
        r.addEventListener('change', function () {
            const cm = this.dataset.cm;
            document.getElementById('medidaCmHidden').value = cm;
            document.getElementById('medidaCmDisplay').value = cm;
            actualizarResumen();
        });
    });

    document.querySelectorAll('input[name="TipoDecoracion"]').forEach(r => {
        r.addEventListener('change', function () {
            document.getElementById('wrapTematica').style.display = this.value === 'Temática de catálogo' ? '' : 'none';
            document.getElementById('wrapImagen').style.display = this.value === 'Imagen de referencia' ? '' : 'none';
            actualizarResumen();
        });
    });

    document.getElementById('selectSabor').addEventListener('change', actualizarResumenFinal);
    document.getElementById('selectRelleno').addEventListener('change', actualizarResumenFinal);
    document.getElementById('selectTematica').addEventListener('change', actualizarResumenFinal);

    document.getElementById('detalleDecoracion').addEventListener('input', function () {
        document.getElementById('contDetalle').textContent = this.value.length;
    });
    document.getElementById('dedicatoriaInput').addEventListener('input', function () {
        document.getElementById('contDedicatoria').textContent = this.value.length;
        actualizarResumenFinal();
    });

    const cantidadInput = document.getElementById('cantidadInput');
    document.getElementById('cantMinus').addEventListener('click', () => {
        cantidadInput.value = Math.max(1, parseInt(cantidadInput.value) - 1);
    });
    document.getElementById('cantPlus').addEventListener('click', () => {
        cantidadInput.value = parseInt(cantidadInput.value) + 1;
    });

    const uploadBox = document.getElementById('uploadBox');
    const imagenInput = document.getElementById('imagenInput');
    uploadBox.addEventListener('click', () => imagenInput.click());

    imagenInput.addEventListener('change', function () {
        const archivo = this.files[0];
        if (!archivo) return;

        if (archivo.size > 5 * 1024 * 1024) {
            Swal.fire({ icon: 'warning', title: 'La imagen supera los 5MB' });
            this.value = '';
            return;
        }

        document.getElementById('previewNombre').textContent =
            `${archivo.name} (${(archivo.size / 1024 / 1024).toFixed(1)} MB)`;

        const reader = new FileReader();
        reader.onload = e => { document.getElementById('previewImg').src = e.target.result; };
        reader.readAsDataURL(archivo);

        document.getElementById('previewImagen').style.display = '';
        uploadBox.style.display = 'none';
    });

    document.getElementById('btnQuitarImagen').addEventListener('click', function () {
        imagenInput.value = '';
        document.getElementById('previewImagen').style.display = 'none';
        uploadBox.style.display = '';
    });

    // Combina la temática del catálogo (select) con el detalle libre (textarea)
    // en un único string, ya que la BD solo tiene una columna DecoracionDetalle.
    function construirDecoracionDetalle(tipoDecoracion) {
        const tematica = document.getElementById('selectTematica').value.trim();
        const detalleLibre = document.getElementById('detalleDecoracion').value.trim();

        if (tipoDecoracion === 'Temática de catálogo') {
            return detalleLibre ? `${tematica} — ${detalleLibre}` : tematica;
        }

        // Sin decoración especial o Imagen de referencia: solo el texto libre, si hay.
        return detalleLibre;
    }

    document.getElementById('formPersonalizacion').addEventListener('submit', function (e) {
        e.preventDefault();

        const tipoDecoracion = document.querySelector('input[name="TipoDecoracion"]:checked').value;

        if (tipoDecoracion === 'Temática de catálogo' && !document.getElementById('selectTematica').value) {
            Swal.fire({ icon: 'warning', title: 'Seleccioná una temática' });
            return;
        }
        if (tipoDecoracion === 'Imagen de referencia' && !imagenInput.files[0]) {
            Swal.fire({ icon: 'warning', title: 'Subí una imagen de referencia' });
            return;
        }
        if (!document.getElementById('dedicatoriaInput').value.trim()) {
            Swal.fire({ icon: 'warning', title: 'La dedicatoria es obligatoria' });
            return;
        }

        const formData = new FormData(this);
        formData.set('Cantidad', cantidadInput.value);
        formData.set('DecoracionDetalle', construirDecoracionDetalle(tipoDecoracion));

        fetch('/Personalizacion/Guardar', { method: 'POST', body: formData })
            .then(async r => {
                const data = await r.json();
                if (!r.ok) throw new Error(data.mensaje || 'No se pudo guardar la personalización.');
                return data;
            })
            .then(() => {
                if (window.actualizarContadorCarrito) window.actualizarContadorCarrito();
                Swal.fire({
                    icon: 'success',
                    title: 'Pastel personalizado agregado',
                    toast: true,
                    position: 'bottom-end',
                    showConfirmButton: false,
                    timer: 1600,
                    timerProgressBar: true
                }).then(() => window.location.href = '/Producto/Catalogo');
            })
            .catch(err => {
                Swal.fire({ icon: 'error', title: 'No se pudo agregar', text: err.message });
            });

    });

    // Sincroniza la medida en cm con el radio de tamaño marcado por defecto al cargar la página.
    const tamanoInicial = document.querySelector('input[name="Tamano"]:checked');
    if (tamanoInicial) {
        document.getElementById('medidaCmHidden').value = tamanoInicial.dataset.cm;
        document.getElementById('medidaCmDisplay').value = tamanoInicial.dataset.cm;
    }

    actualizarResumen();
})();