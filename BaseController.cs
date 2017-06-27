using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using fav.Repository.Logic;
using System.Net.Mail;
using fav.WebUtil;

namespace fav.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Controller = filterContext.Controller.GetType().Name.Replace("Controller", "");
        }

        public enum TipoMensagem
        {
            Aviso,
            Erro,
            Sucesso
        }

        public enum ListaDeErros
        {
            Sim = 0,
            Nao = 1
        }

        /// <summary>
        /// Mensagem de retorno
        /// </summary>
        /// <param name="tipoMensagem">Enum de TipoMensagem Aviso, Erro, Sucesso</param>
        /// <param name="mensagem">Texto da mensagem</param>
        public void Mensagem(TipoMensagem tipoMensagem, string mensagem)
        {
            TempData[tipoMensagem.ToString()] = mensagem;
        }

        public JsonResult RetornoJson(TipoMensagem tipoMensagem, string mensagem = null, List<dynamic> camposExtras = null)
        {
            List<string> mensagens = new List<string>();
            JsonResult camposDinamicos = new JsonResult();

            string mensagemRetorno = tipoMensagem.ToString();

            if(camposExtras != null)
                camposDinamicos.Data = new { camposExtras };

            if (ViewData.ModelState.IsValid)
            {
                if (camposExtras != null)
                {
                    return Json(new
                    {
                        mensagem = mensagemRetorno,
                        resultado = (!string.IsNullOrWhiteSpace(mensagem) ? mensagem : "Registro salvo com sucesso!"),
                        tipo = ListaDeErros.Nao,
                        camposExtras = camposDinamicos.Data
                    });
                }
                else
                {
                    return Json(new
                    {
                        mensagem = mensagemRetorno,
                        resultado = (!string.IsNullOrWhiteSpace(mensagem) ? mensagem : "Registro salvo com sucesso!"),
                        tipo = ListaDeErros.Nao
                    });
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(mensagem))
                    mensagens.Add(mensagem);

                foreach (var chave in ViewData.ModelState.Keys)
                {
                    mensagens.AddRange(ViewData.ModelState[chave].Errors.Select(x => x.ErrorMessage));
                }

                if (mensagens.Count > 0)
                {
                    if (mensagens.Count > 1)
                    {
                        return Json(new
                        {
                            mensagem = mensagemRetorno,
                            resultado = mensagens,
                            tipo = ListaDeErros.Sim
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            mensagem = mensagemRetorno,
                            resultado = mensagens.ElementAt(0),
                            tipo = ListaDeErros.Nao
                        });
                    }
                }
                else
                {
                    return Json(new
                        {
                            mensagem = TipoMensagem.Erro,
                            resultado = "Ocorreu um erro: Model não é válida, mas não contém erros.",
                            tipo = ListaDeErros.Nao
                        });
                }
            }
        }

        /// <summary>
        /// Valida se um email é correto
        /// </summary>
        /// <param name="email">String com endereço do email</param>
        /// <returns>True se é válido e False se é inválido</returns>
        public bool ValidarEmail(string email)
        {
            try
            {
                MailAddress validarEmail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Limpa os dados da sessão
        /// </summary>
        protected void LimparSessao()
        {
            Sessao.Email = null;
            Sessao.IdUsuario = null;
            Sessao.Senha = null;
        }

        /// <summary>
        /// Verifica se a sessão está preenchida
        /// </summary>
        /// <returns>Se a sessão estiver preenchida retorna TRUE, senao FALSE</returns>
        protected bool ValidarSessao()
        {
            if (Sessao.Email != null && Sessao.IdUsuario != null && Sessao.Senha != null)
                return true;
            else
                return false;
        }

    }
}
