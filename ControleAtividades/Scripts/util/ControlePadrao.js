var currentPage = 0;

function irParaPagina(indice) {
    currentPage = indice;
    jQuery.ajax({
        type: "POST",
        url: content + "PropriedadeAlimento/Index",
        data: { indice: indice },
        success: function (data) {
            montarGrid(data);
        },
        dataType: "json"
    });
}

function alterar(id) {
    window.location = content + controller + '/Alterar/?id=' + id;
}

function visualizar(id) {
    window.location = content + controller + '/Visualizar/?id=' + id;
}

function excluir(id) {
    jQuery('#Id').val(id);
}

function confirmarExclusao() {
    if (verificaValor(jQuery('#Id').val())) {
        jQuery.ajax({
            type: "POST",
            url: content + controller + '/Excluir',
            data: { id: jQuery('Id').val() },
            success: function (data) {
                if (data.mensagem == "ok") {
                    irParaPagina(currentPage);
                    jQuery("#ModalExclusao").modal("hide");
                    jQuery("#result").html("<div class='alert alert-success'>" + data.resultado +"</div>");
                }
                else if (data.mensagem == "erro") {
                    jQuery("#txtTitleModalMensagem").html("Erro");
                    jQuery("#txtModalMensagem").html(data.resultado);
                    jQuery('#ModalMensagem').modal('show');
                }
            },
            dataType: "json"
        });
    }
}