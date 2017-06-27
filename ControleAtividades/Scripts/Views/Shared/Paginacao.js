jQuery(document).ready(function () {
    jQuery(".paginacao_hide").html("");
    jQuery(".paginacao_hide").hide();

    jQuery("th a").click(function () {
        if (jQuery(this).attr('href').indexOf("sort") != -1) {
            jQuery(".paginacao_hide").html("");
            jQuery(".paginacao_hide").hide();
        }
    });
    var links = $('a[href*=page], a[href*=sort]'), form = $('form');

    links.click(function () {
        form.attr("action", this.href);
        $(this).attr("href", "javascript:");
        form.submit();
    });
});