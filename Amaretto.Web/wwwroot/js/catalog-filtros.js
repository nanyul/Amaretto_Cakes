function initCatalogFiltros({ endpoint, idField, detailUrlBase, tipo }) {

    async function aplicarFiltros() {
        const params = new URLSearchParams();

        const estadoEl = document.querySelector('input[name="estado"]:checked');
        if (estadoEl) params.append('estado', estadoEl.value);

        const precioMax = document.getElementById('priceRange')?.value;
        if (precioMax) params.append('precioMax', precioMax);

        document.querySelectorAll('input[name="categoria"]:checked').forEach(c => {
            if (c.value !== 'todos') params.append('categoriaIds', c.value);
        });

        const ordenarSelect = document.querySelector('.filter-select');
        if (ordenarSelect) params.append('ordenarPor', ordenarSelect.value);

        const res = await fetch(`${endpoint}?${params.toString()}`);
        const data = await res.json();
        renderGrid(data);
    }

    function renderGrid(items) {
        const grid = document.getElementById('productGrid');
        const contador = document.querySelector('.catalog-count strong');
        if (contador) contador.textContent = items.length;

        if (items.length === 0) {
            grid.innerHTML = `<div class="catalog-empty"><i class="bi bi-gift"></i><p>No hay ${tipo === 'combo' ? 'combos' : 'productos'} que coincidan con tu búsqueda.</p></div>`;
            return;
        }

        grid.innerHTML = items.map(item => {
            const id = item[idField];
            const hasSecondImage = !!item.imagen2;
            const carouselId = `carousel-${tipo}-${id}`;

            const badge = tipo === 'combo'
                ? `<span class="catalog-card-badge ${item.estado ? 'is-available' : 'is-unavailable'}">
                       <i class="bi bi-${item.estado ? 'check' : 'x'}-circle me-1"></i>${item.estado ? 'Disponible' : 'Inactivo'}
                   </span>`
                : (item.esPersonalizable
                    ? `<span class="catalog-card-badge is-custom"><i class="bi bi-stars me-1"></i>Personalizable</span>`
                    : '');

            return `
            <div class="col-lg-4 col-md-6 col-sm-6">
                <div class="catalog-card animate-on-scroll visible">
                    <div class="catalog-card-img ${hasSecondImage ? '' : 'single-image'}">
                        <div id="${carouselId}" class="carousel slide" data-bs-ride="${hasSecondImage ? 'carousel' : 'false'}" data-bs-interval="3500">
                            <div class="carousel-inner">
                                <div class="carousel-item active"><img src="${item.imagen1}" alt="${item.nombre}"></div>
                                ${hasSecondImage ? `<div class="carousel-item"><img src="${item.imagen2}" alt="${item.nombre}"></div>` : ''}
                            </div>
                            ${hasSecondImage ? `
                                <button class="carousel-control-prev" type="button" data-bs-target="#${carouselId}" data-bs-slide="prev"><span class="carousel-control-prev-icon"></span></button>
                                <button class="carousel-control-next" type="button" data-bs-target="#${carouselId}" data-bs-slide="next"><span class="carousel-control-next-icon"></span></button>
                                <div class="carousel-indicators">
                                    <button type="button" data-bs-target="#${carouselId}" data-bs-slide-to="0" class="active"></button>
                                    <button type="button" data-bs-target="#${carouselId}" data-bs-slide-to="1"></button>
                                </div>` : ''}
                        </div>
                        ${badge}
                        <button class="btn-wishlist" title="Guardar en favoritos"><i class="bi bi-heart-fill"></i></button>
                    </div>
                    <div class="catalog-card-body">
                        <h5>${item.nombre}</h5>
                        <p class="catalog-card-price">₡${Number(item.precio).toLocaleString('es-CR')}</p>
                    </div>
                    <div class="catalog-card-footer">
                        <a href="${detailUrlBase}/${id}" class="btn-catalog">Ver detalle <i class="bi bi-heart ms-1"></i></a>
                        <button class="btn-wishlist-footer" title="Favorito"><i class="bi bi-heart"></i></button>
                    </div>
                </div>
            </div>`;
        }).join('');
    }

    document.querySelectorAll('input[name="estado"], input[name="categoria"]').forEach(el =>
        el.addEventListener('change', aplicarFiltros)
    );
    document.querySelector('.filter-select')?.addEventListener('change', aplicarFiltros);
    document.getElementById('priceRange')?.addEventListener('change', aplicarFiltros); // al soltar el slider
}