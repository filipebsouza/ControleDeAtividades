/********************************************************
* Verifica se há algo no valor enviado via param, já que
* algunas vezes um if (x == null) pode falhar pelo fato
* de que ele poderá ser x = "" e vice-versa.
* Acaba sendo necessário realizar ambas as verificações.
*
* Autor: Renan Siravegna
* E-mail: rsmoreira@fazenda.ms.gov.br
* Revisao: Filipe Bezerra de Souza
* Data: 19/11/2012
* E-mail: filipe.bsouza@gmail.com
*/
function verificaValor(valor) {
    return valor != "" && valor != null && valor != undefined && valor != "undefined";
}