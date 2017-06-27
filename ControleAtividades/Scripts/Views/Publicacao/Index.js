function excluir(id) {
    jQuery("#id").val(id);
    jQuery('#ModalExclusao').modal('show');
}

function alterar(id) {
    window.location = content + controller + "Alterar/" + id;
}