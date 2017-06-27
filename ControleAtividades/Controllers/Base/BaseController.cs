using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using Repository.Logic;
using Domain.Model;
using System.Net;
using System.Net.Mail;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Repository.Controllers
{
    public class BaseController<T> : Controller
        where T : class
    {
        protected GenericRepository<T> repositorio = new GenericRepository<T>();
        protected Expression<Func<T, bool>> where;

        private GenericRepository<ca_usuario> repUsuario = new GenericRepository<ca_usuario>();
        protected int pageSize = 10, pageLimit = 5;
        protected string nomeCookie = "cAutenticacao";
        //private int id_usuario;
        //private string assunto;
        //private string texto;

        [OutputCache(NoStore = true, Duration = 0)]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var nomeControler = filterContext.Controller.GetType().Name.Replace("Controller", "");

            if (ValidarAcesso(nomeControler))
            {
                ViewBag.Controller = nomeControler;
            }
            else
            {
                if (nomeControler != "Seguranca")
                {
                    filterContext.Result = new RedirectResult(Url.Content("~/Seguranca/Index"));

                    return;
                }
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        private bool ValidarAcesso(string nomeControler)
        {
            try
            {
                if (VerificarCookies())
                {
                    return true;
                }
                else
                {
                    if (nomeControler != "Seguranca")
                        Mensagem(TipoMensagem.Aviso, "Acesso negado! Realize a autenticação do seu usuário abaixo.");

                    return false;
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "Ocorreu o seguinte erro: " + ex.Message
                    + (ex.InnerException == null ? "" : (" Detalhes: " + ex.InnerException.Message)));

                return false;
            }
        }

        private bool VerificarCookies()
        {
            var email = RecuperarEmailCookie();
            var senha = RecuperarSenhaCookie();

            Expression<Func<tb_usuario, bool>> where = null;
            var rep = new GenericRepository<tb_usuario>();

            return ValidarAcesso(where, rep, new tb_usuario { email = email, senha = senha });
        }

        protected bool ValidarAcesso(Expression<Func<tb_usuario, bool>> where, GenericRepository<tb_usuario> repositorio, tb_usuario usuario)
        {
            try
            {
                where = u => u.email == usuario.email
                    && u.senha == usuario.senha
                    && u.situacao == 1;

                var consulta = repositorio.Obter(where);

                if (consulta == null)
                {
                    return false;
                }
                else
                {
                    usuario.nome = consulta.nome;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, ex.Message);

                return false;
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public JsonResult VerificarSeguranca()
        {
            if (ValidarAcesso(""))
                return Json(new { mensagem = "" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { mensagem = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public void Mensagem(TipoMensagem tipoMensagem, string mensagem)
        {
            TempData[tipoMensagem.ToString()] = mensagem;
        }

        public SelectList DDLUsuarios(int? id = null)
        {
            List<KeyValuePair<string, string>> usuarios = new List<KeyValuePair<string, string>>();
            usuarios.Add(new KeyValuePair<string, string>("", "Selecione..."));

            foreach (var usuario in repUsuario.Listar().OrderBy(u => u.nome))
            {
                usuarios.Add(new KeyValuePair<string, string>(usuario.id.ToString(), usuario.nome));
            }

            if (id.HasValue)
                return new SelectList(usuarios, "Key", "Value", id.Value);
            else
                return new SelectList(usuarios, "Key", "Value");
        }

        public void EnviarEmail(int idUsuario, string assunto, string texto)
        {
            try
            {
                this.idUsuario = idUsuario;
                this.assunto = assunto;
                this.texto = texto;

                System.Threading.Thread envio = new System.Threading.Thread(EnviarEmailT);
                envio.Start();

                //EnviarEmailT();
            }
            catch (Exception ex) { throw ex; }
        }

        int idUsuario;
        string assunto, texto;

        private void EnviarEmailT()
        {
            try
            {
                Expression<Func<ca_usuario, bool>> where = i => i.id == idUsuario;
                ca_usuario usuario = repUsuario.Obter(where);

                var fromAddress = new MailAddress("endereço de email", "Controle de Atividades");
                var fromPassword = "senha do email";
                var toAddress = new MailAddress(usuario.email, usuario.nome);

                MailMessage mail = new MailMessage();
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                mail.From = fromAddress;
                mail.To.Add(toAddress);
                mail.Subject = assunto;
                mail.Body = texto;

                SmtpClient smtp = new SmtpClient("endereco smtp", 587);
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(fromAddress.User, fromPassword);

                smtp.Send(mail);
            }
            catch (Exception ex) { throw ex; }
        }

        public void CriarCookie(string email, string senha)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(nomeCookie);

                cookie.Values.Add("email", email);
                cookie.Values.Add("senha", new Caioff.clCaioff().Criptografar(DateTime.Now.ToString("ss") + senha));

                this.Response.AppendCookie(cookie);
            }
            catch { }
        }

        public void DestruirCookie()
        {
            try
            {
                if (Request.Cookies[nomeCookie] != null)
                {
                    HttpCookie cookie = new HttpCookie(nomeCookie);
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cookie);
                }
            }
            catch { }
        }

        public string RecuperarEmailCookie()
        {
            try
            {
                var cookie = this.Request.Cookies[nomeCookie];

                if (cookie == null)
                    return "";
                else
                    return cookie.Values["email"];
            }
            catch
            {
                return "";
            }
        }

        public string RecuperarSenhaCookie()
        {
            try
            {
                var cookie = this.Request.Cookies[nomeCookie];

                if (cookie == null)
                    return "";
                else
                    return cookie.Values["senha"].Substring(2);
            }
            catch
            {
                return "";
            }
        }

        private string apiKey = "chave da api do pushBullet";

        public void PushLink(string title, string body, string url)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    // Authentication
                    // In HTTP, basic authentication is a pair of credentials (username:password) encoded in Base64 and attached
                    // to the header. Here we are encoding the Access Key for pushbullet and adding it to the request header.
                    var auth = new NameValueCollection();
                    auth["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.apiKey + ":"));

                    wb.Headers.Add(auth);

                    // Data
                    // Creating a collection of name-values to store our parameters.
                    var data = new NameValueCollection();

                    data["type"] = "link";
                    data["title"] = title;
                    data["body"] = body;
                    data["url"] = url;

                    //// Add Channel tag (if it exists)
                    //if (channelTag.Trim().Length != 0)
                    //    data["channel_tag"] = channelTag;

                    // Finally push
                    wb.UploadValues("https://api.pushbullet.com/v2/pushes", "POST", data);
                }
            }
            catch { }
        }

        public void PushLink(string title, string body, string url, string contatoEmail)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    // Authentication
                    // In HTTP, basic authentication is a pair of credentials (username:password) encoded in Base64 and attached
                    // to the header. Here we are encoding the Access Key for pushbullet and adding it to the request header.
                    var auth = new NameValueCollection();
                    auth["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.apiKey + ":"));

                    wb.Headers.Add(auth);

                    // Data
                    // Creating a collection of name-values to store our parameters.
                    var data = new NameValueCollection();

                    data["type"] = "link";
                    data["title"] = title;
                    data["body"] = body;
                    data["url"] = url;
                    data["email"] = contatoEmail;

                    //// Add Channel tag (if it exists)
                    //if (channelTag.Trim().Length != 0)
                    //    data["channel_tag"] = channelTag;

                    // Finally push
                    wb.UploadValues("https://api.pushbullet.com/v2/pushes", "POST", data);
                }
            }
            catch { }
        }

        public void PushNote(string title, string body)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    // Authentication
                    // In HTTP, basic authentication is a pair of credentials (username:password) encoded in Base64 and attached
                    // to the header. Here we are encoding the Access Key for pushbullet and adding it to the request header.
                    var auth = new NameValueCollection();
                    auth["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.apiKey + ":"));

                    wb.Headers.Add(auth);

                    // Data
                    // Creating a collection of name-values to store our parameters.
                    var data = new NameValueCollection();

                    data["type"] = "link";
                    data["title"] = title;
                    data["body"] = body;

                    //// Add Channel tag (if it exists)
                    //if (channelTag.Trim().Length != 0)
                    //    data["channel_tag"] = channelTag;

                    // Finally push
                    wb.UploadValues("https://api.pushbullet.com/v2/pushes", "POST", data);
                }
            }
            catch { }
        }

        public void PushNote(string title, string body, string emailContato)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    // Authentication
                    // In HTTP, basic authentication is a pair of credentials (username:password) encoded in Base64 and attached
                    // to the header. Here we are encoding the Access Key for pushbullet and adding it to the request header.
                    var auth = new NameValueCollection();
                    auth["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.apiKey + ":"));

                    wb.Headers.Add(auth);

                    // Data
                    // Creating a collection of name-values to store our parameters.
                    var data = new NameValueCollection();

                    data["type"] = "link";
                    data["title"] = title;
                    data["body"] = body;
                    data["email"] = emailContato;

                    //// Add Channel tag (if it exists)
                    //if (channelTag.Trim().Length != 0)
                    //    data["channel_tag"] = channelTag;

                    // Finally push
                    wb.UploadValues("https://api.pushbullet.com/v2/pushes", "POST", data);
                }
            }
            catch { }
        }

        public List<Contact> PushContacts()
        {
            List<Contact> contatos = new List<Contact>();

            try
            {
                using (var wb = new WebClient())
                {
                    // Authentication
                    // In HTTP, basic authentication is a pair of credentials (username:password) encoded in Base64 and attached
                    // to the header. Here we are encoding the Access Key for pushbullet and adding it to the request header.
                    var auth = new NameValueCollection();
                    auth["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.apiKey + ":"));

                    wb.Headers.Add(auth);

                    // Data
                    // Creating a collection of name-values to store our parameters.
                    var data = new NameValueCollection();

                    // Finally push
                    var contato = wb.DownloadString("https://api.pushbullet.com/v2/contacts");

                    try
                    {
                        var person = JsonConvert.DeserializeObject<Contatos>(contato);

                        contatos = person.contacts.ToList();
                    }
                    catch { }
                }
            }
            catch { }

            return contatos;
        }

        public enum TipoMensagem
        {
            Aviso,
            Erro,
            Sucesso
        }
    }

    public class Contatos
    {
        public Contact[] contacts { get; set; }
    }

    public class Contact
    {
        public string iden { get; set; }
        public string name { get; set; }
        public float? created { get; set; }
        public float? modified { get; set; }
        public string email { get; set; }
        public string email_normalized { get; set; }
        public bool active { get; set; }
    }
}
