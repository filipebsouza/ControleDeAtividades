using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Model;
using Repository.Controllers;
using Repository.Logic;
using System.Linq.Expressions;
using Caioff;
using System.Drawing;

namespace ControleAtividades.Controllers
{
    public class SegurancaController : BaseController<tb_usuario>
    {
        Random aleatorio = new Random();

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index()
        {
            tb_usuario usuario = new tb_usuario();
            usuario.email = RecuperarEmailCookie();
            usuario.senha = RecuperarSenhaCookie();

            if (ValidarAcesso(where, repositorio, usuario))
            {
                Mensagem(TipoMensagem.Sucesso, "Bem-vindo, " + usuario.nome
                            + ". <br /><br /><strong>Biscoito do Sorte</strong><br />" + BiscoitoDaSorte());

                return RedirectToAction("Index", "Atividade");
            }
            else
            {
                PreencherVBIndex();

                return View(usuario);
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(tb_usuario usuario)
        {
            try
            {
                CriarCookie(usuario.email, usuario.senha);

                usuario.senha = new Caioff.clCaioff().Criptografar(usuario.senha);

                if (ValidarAcesso(where, repositorio, usuario))
                {
                    Mensagem(TipoMensagem.Sucesso, "Bem-vindo, " + usuario.nome
                        + ". <br /><br /><strong>Biscoito do Sorte</strong><br />" + BiscoitoDaSorte());

                    return RedirectToAction("Index", "Atividade");
                }
                else
                {
                    Mensagem(TipoMensagem.Aviso, "Acesso negado! Informações inválidas, tente novamente.");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                DestruirCookie();

                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Sair()
        {
            DestruirCookie();
            Session.RemoveAll();

            return RedirectToAction("Index", "Seguranca");
        }

        private void PreencherVBIndex()
        {
            ViewBag.ImagemLogin = aleatorio.Next(1, 6);
        }

        private string BiscoitoDaSorte()
        {
            try
            {
                #region Frases
                List<string> frases = new List<string>();
                frases.Add("A vida trará coisas boas se tiveres paciência.");
                frases.Add("Demonstre amor e alegria em todas as oportunidades e verás que a paz nasce dentro de você.");
                frases.Add("Não compense na ira o que lhe falta na razão.");
                frases.Add("Defeitos e virtudes são apenas dois lados da mesma moeda.");
                frases.Add("A maior de todas as torres começa no solo.");
                frases.Add("Não há que ser forte. Há que ser flexível.");
                frases.Add("Gente todo dia arruma os cabelos, por que não o coração?");
                frases.Add("Há três coisas que jamais voltam; a flecha lançada, a palavra dita e a oportunidade perdida.");
                frases.Add("A juventude não é uma época da vida, é um estado de espírito.");
                frases.Add("Podemos escolher o que semear, mas somos obrigados a colher o que plantamos.");
                frases.Add("Dê toda a atenção para a formação dos teus filhos, sobretudo por exemplos de tua própria vida.");
                frases.Add("Siga os bons e aprenda com eles.");
                frases.Add("Não importa o tamanho da montanha, ela não pode tapar o sol.");
                frases.Add("O bom-senso vai mais longe do que muito conhecimento.");
                frases.Add("Quem quer colher rosas deve suportar os espinhos.");
                frases.Add("São os nossos amigos que nos ensinam as mais valiosas lições.");
                frases.Add("Uma iniciativa mal-sucedida não significa o final de tudo.");
                frases.Add("Sempre existe uma nova oportunidade.");
                frases.Add("Aquele que se importa com o sentimento dos outros, não é um tolo.");
                frases.Add("A adversidade é um espelho que reflete o verdadeiro eu.");
                frases.Add("Lamentar aquilo que não temos é desperdiçar aquilo que já possuímos.");
                frases.Add("Uma bela flor é incompleta sem suas folhas.");
                frases.Add("Sem o fogo do entusiasmo, não há o calor da vitória.");
                frases.Add("Não há melhor negócio que a vida. A gente há obtém a troco de nada.");
                frases.Add("O riso é a menor distância entre duas pessoas.");
                frases.Add("Você é jovem apenas uma vez. Depois precisa inventar outra desculpa.");
                frases.Add("É mais fácil conseguir o perdão do que a permissão.");
                frases.Add("Os defeitos são mais fortes quando o amor é fraco.");
                frases.Add("Amizade e Amor são coisas que podem virar uma só num piscar de olhos.");
                frases.Add("Surpreender e ser surpreendido é o segredo do amor.");
                frases.Add("Faça pequenas coisas agora e maiores coisas lhe serão confiadas cada dia.");
                #endregion

                return frases.ElementAt(aleatorio.Next(0, frases.Count - 1));
            }
            catch (Exception ex)
            {
                return "ERRO na BiscoitoDaSorte():  " + ex.Message;
            }
        }

        [HttpGet]
        public ActionResult Erro()
        {
            string arquivo = "";
            var imagem = aleatorio.Next(0, 2);

            if (imagem == 0)
            {
                ViewBag.Frase = "Oh! Não encontramos sua página. Deve ser culpa desse cometa...<br />"
                    + "Podemos encaminhar você para a página de <a href='/Seguranca'>autenticação</a> ou <a href='/Atividade'>atividades</a>, você escolhe.";

                arquivo = "erro1";
            }
            else if (imagem == 1)
            {
                ViewBag.Frase = "Oh! Não encontramos sua página. Continue remando, ela está logo a frente...<br />"
                    + "Ou podemos encaminhar você para a página de <a href='/Seguranca'>autenticação</a> ou <a href='/Atividade'>atividades</a>, você escolhe.";

                arquivo = "erro3";
            }

            return View((object)arquivo);
        }
    }
}