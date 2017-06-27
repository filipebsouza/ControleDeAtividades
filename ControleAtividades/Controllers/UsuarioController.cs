using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository.Controllers;
using Repository.Logic;
using Domain.Model;
using System.Linq.Expressions;

namespace ControleAtividades.Controllers
{
    public class UsuarioController : BaseController<ca_usuario>
    {
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index()
        {
            return View(repositorio.Listar().OrderBy(u => u.nome));
        }

        [HttpGet]
        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Incluir(FormCollection form)
        {
            try
            {
                ca_usuario usuario = new ca_usuario();
                TryUpdateModel(usuario, form);

                if (ViewData.ModelState.IsValid)
                {
                    repositorio.Incluir(usuario);
                    repositorio.Salvar();

                    Mensagem(TipoMensagem.Sucesso, "Usuário incluído com sucesso!");

                    return RedirectToAction("Index");
                }
                else
                {
                    Mensagem(TipoMensagem.Aviso, "Preencha os campos corretamente");

                    return View();
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return View();
            }
        }

        [HttpGet]
        public ActionResult Alterar(int id)
        {
            try
            {
                where = i => i.id == id;

                ca_usuario usuario = repositorio.Obter(where);

                if (usuario != null)
                {
                    return View(usuario);
                }
                else
                {
                    Mensagem(TipoMensagem.Erro, "ERRO: Não foi possível encontrar a Usuário(id = " + id + ").");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return View();
            }
        }

        [HttpPost]
        public ActionResult Alterar(FormCollection form)
        {
            try
            {
                ca_usuario usuario = new ca_usuario();
                TryUpdateModel(usuario, form);

                if (ViewData.ModelState.IsValid)
                {
                    repositorio.Alterar(usuario);
                    repositorio.Salvar();

                    Mensagem(TipoMensagem.Sucesso, "Usuário alterado com sucesso!");

                    return RedirectToAction("Index");
                }
                else
                {
                    Mensagem(TipoMensagem.Aviso, "Preencha os campos corretamente");

                    return View();
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return View();
            }
        }

        [HttpGet]
        public ActionResult Deletar(int id)
        {
            try
            {
                where = i => i.id == id;

                ca_usuario usuario = repositorio.Obter(where);

                if (usuario != null)
                {
                    if (usuario.ca_atividade != null && usuario.ca_atividade.Count > 0)
                    {
                        Mensagem(TipoMensagem.Aviso,
                            "Não é possível deletar o usuário selecionado, pois ele está vinculado a " + usuario.ca_atividade.Count + " atividade(s)!");
                    }
                    else
                    {
                        repositorio.Deletar(usuario);
                        repositorio.Salvar();

                        Mensagem(TipoMensagem.Sucesso, "Usuário deletada com sucesso!");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    Mensagem(TipoMensagem.Erro, "ERRO: Não foi possível encontrar a Usuário(id = " + id + ").");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return RedirectToAction("Index");
            }
        }

    }
}
