(function () {
    const estadoToggle = document.getElementById('estadoToggle');
    const estadoLabel = document.getElementById('estadoLabel');

    function actualizarEstadoLabel() {
        if (estadoToggle.checked) {
            estadoLabel.textContent = 'Activo';
            estadoLabel.classList.remove('is-no-disponible');
            estadoLabel.classList.add('is-disponible');
        } else {
            estadoLabel.textContent = 'Inactivo';
            estadoLabel.classList.remove('is-disponible');
            estadoLabel.classList.add('is-no-disponible');
        }
    }

    if (estadoToggle && estadoLabel) {
        estadoToggle.addEventListener('change', actualizarEstadoLabel);
        actualizarEstadoLabel();
    }

    const password = document.getElementById('Password');
    const confirmar = document.getElementById('ConfirmPassword');

    if (password && confirmar) {
        function validarCoincidencia() {
            if (confirmar.value && password.value !== confirmar.value) {
                confirmar.setCustomValidity('Las contraseñas no coinciden');
            } else {
                confirmar.setCustomValidity('');
            }
        }

        password.addEventListener('input', validarCoincidencia);
        confirmar.addEventListener('input', validarCoincidencia);
    }
})();
