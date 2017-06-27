jQuery(document).ready(function () {
    jQuery(".field - validation - error").addClass("alert alert-error");
    jQuery(".alert").fadeOut(10000);
});

jQuery(document).ready(function () {
    //Mensagens de Erro
    jQuery(".field-validation-error").each(function (item) {
        var mensagemDeErro = this.innerHTML;
        jQuery(this).html("");
        jQuery(this).addClass("alert alert-error");
        jQuery(this).html("<b>Erro:</b> " + mensagemDeErro);
    });

    //Input erro
    var flagErro = 0;
    jQuery(".input-validation-error").each(function (item) {
        jQuery(this).parent().addClass("control-group error");
        flagErro++;
    });

    //Tirando a cor Vermelha do Input
    jQuery(".input-validation-error").focus(function () {
        if (flagErro > 0) {
            jQuery(this).parent().removeClass("control-group error");
            flagErro--;
        }
    });
});

function mostrarMensagem(tipoMensagem, mensagem, listaDeMensagens) {
    jQuery("#txtTitleModalMensagem").html(tipoMensagem);
    if (listaDeMensagens == 0) {
        jQuery("#txtModalMensagem").html(mensagem);
    }
    else {
        var msg = "";

        jQuery(mensagem).each(function () {
            msg += this + "<br />";
        });
    }
    jQuery('#ModalMensagem').modal('show');
}