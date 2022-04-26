$(document).ready(function () {
    $("#btnIngresar").click(function () {
        Validar();
    });
});

function Validar() {
    if ($("#txtUsuario").val() === "") {
        alert('Debe ingresar el usuario.');
        $("#txtUsuario").focus();
        return;
    }
    if ($("#txtContrasena").val() === "") {
        alert('Debe ingresar la contraseña.');
        $("#txtContrasena").focus();
        return;
    }

    var pmtPeticion = new Object();
    pmtPeticion.Usuario = $("#txtUsuario").val();
    pmtPeticion.Password = $("#txtPassword").val();
    
    $.ajax({
        type: 'POST',
        url: urlValidar,
        data: pmtPeticion,
        beforeSend: function () {
            MensajeCargando();
        },
        success: function (data) {
            if (data === true) {
                window.location.href = '/Sftp';
            }
            else {
                alert('Usuario o contraseña no validos');
            }
            bootbox.hideAll();
        },
        error: function () {
            bootbox.hideAll();
        }
    });
}