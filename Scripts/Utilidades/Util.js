
function openNav() {
    document.getElementById("mySidenav").style.width = "250px";
    document.getElementById("main").style.marginLeft = "250px";
    document.body.style.backgroundColor = "rgba(0,0,0,0.4)";
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
    document.getElementById("main").style.marginLeft = "0";
    document.body.style.backgroundColor = "white";
}

function MensajeCargando() {
    bootbox.dialog({
        message: '<div class="loader"></div><br/><label class="ui-widget ui-state-default ui-corner-all">Cargando....</label>',
        closeButton: false
    }).css({
        'margin-top': function () {
            var w = $(window).height();
            var b = $(".modal-dialog").height();
            // should not be (w-h)/2
            var h = (w) / 2;
            return h + "px";
        },
        'margin-left': function () {
            var w = $(window).height();
            var b = $(".modal-dialog").height();
            // should not be (w-h)/2
            var h = (w) / 2;
            return h + "px";
        },
        'width': "150px"
    });
}

function LlenarCombo(Elemento, Datos) {
    Datos.forEach(function (Dato, index) {
        Elemento.append('<option value="' + Dato.Valor + '">' + Dato.Texto + '</option>');
    });
}


function getCookie(name) {
    var parts = document.cookie.split(name + "=");
    console.log(parts)
    if (parts.length == 2) return parts.pop().split(";").shift();
}

function expireCookie(cName) {
    document.cookie =
        encodeURIComponent(cName) + "=deleted; expires=" + new Date(0).toUTCString();
}

var downloadTimer;
var attempts = 30;

// Prevents double-submits by waiting for a cookie from the server.
function blockResubmit() {
    var downloadToken = "Export";
    
    MensajeCargando()
    downloadTimer = window.setInterval(function () {
        var token = getCookie("Export");

        if ((token == downloadToken) || (attempts == 0)) {
            unblockSubmit();
        }

        attempts--;
    }, 1000);
}

function unblockSubmit() {
    bootbox.hideAll()
    window.clearInterval(downloadTimer);
    $.removeCookie('Export')
    attempts = 30;
}