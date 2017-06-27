//jQuery(document).ready(function () {
//    window.setInterval(function () {
//        jQuery.ajax(
//        {
//            url: content + "Atividade/VerificarSeguranca/",
//            type: "post",
//            dataType: "json",
//            error: function () { },
//            success: function (data) {

//                if (data.mensagem == "OK") {
//                    window.location = content + "Seguranca";
//                }
//                else { }
//            }
//        });
//    }, 150000);
//});

jQuery(document).ready(function () {
    jQuery('#collapseOne').collapse("hide");
});

function excluir(id) {
    jQuery("#id").val(id);
    jQuery('#ModalExclusao').modal('show');
}

function alterar(id) {
    window.location = content + controller + "Alterar/" + id;
}