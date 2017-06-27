using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository.Controllers;
using Repository.Logic;
using Domain.Model;
using System.Linq.Expressions;
using Domain.Model.ViewModel;
using Util;
using System.Net;
using System.IO;

namespace Controleca_atividades.Controllers
{
    public class PublicacaoController : BaseController<ca_publicacao>
    {
        GenericRepository<ca_atividadesPublicadas> repositorioAP = new GenericRepository<ca_atividadesPublicadas>();
        GenericRepository<ca_atividade> repositorioA = new GenericRepository<ca_atividade>();

        Expression<Func<ca_atividade, bool>> whereA;
        Expression<Func<ca_usuario, bool>> whereCAU;
        Expression<Func<ca_atividade, bool>> atividadesFinalizadas_e_NaoPublicadas = a => a.dt_inicio.HasValue && a.dt_fim.HasValue && !a.ca_atividadesPublicadas.Any();
        Expression<Func<ca_atividadesPublicadas, bool>> whereAP;

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(string page, string sort, string sortdir)
        {
            ViewBag.Incluir = "Novo";

            return View(Consultar(page, sort, sortdir));
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(string page, string sort, string sortdir, string filtro)
        {
            ViewBag.Incluir = "Novo";

            return View(Consultar(page, sort, sortdir, filtro));
        }

        private GridVM Consultar(string page, string sort, string sortdir, string filtro = null)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                where = p => true;
            else
                where = p => p.sistema.Contains(filtro) || p.ca_atividadesPublicadas.Count(ap => ap.ca_atividade.descricao.Contains(filtro)) > 0;

            GridVM grid = new GridVM();
            List<ca_publicacaoVM> publicacoes = new List<ca_publicacaoVM>();
            IQueryable<ca_publicacao> aux;
            aux = repositorio.Listar(where).OrderBy(u => u.dt_publicacao);

            int pagina = 0;
            int totalRegistros = repositorio.Listar(where).Count();

            grid.PageLimit = base.pageLimit;
            grid.PageSize = base.pageSize;

            if (totalRegistros > base.pageSize)
                grid.TotalPages = int.Parse((totalRegistros / base.pageSize).ToString()) + 1;
            else
            {
                page = "1";
                grid.TotalPages = 1;
            }

            if (Int32.TryParse(page, out pagina))
                aux = aux.Skip((pagina - 1) * base.pageSize).Take(base.pageSize);
            else
                aux = aux.Take(base.pageSize);

            grid.PageIndex = pagina;

            foreach (var item in aux)
            {
                var publicacao = new ca_publicacaoVM();

                publicacao.usuario = item.ca_usuario.nome;
                publicacao.dt_publicacao = item.dt_publicacao.HasValue ? item.dt_publicacao.Value.ToShortDateString() : "Não publicado";
                publicacao.sistema = item.sistema;

                var atividades = item.ca_atividadesPublicadas.Select(ap => ap.ca_atividade);

                foreach (var atividade in atividades)
                {
                    string texto = "<strong>Prioridade:</strong> " + atividade.prioridade
                        + "<br /><strong>Período: </strong> " + (atividade.dt_inicio.HasValue ? atividade.dt_inicio.Value.ToShortDateString() : "--/--/----")
                        + " <strong>até</strong> " + (atividade.dt_fim.HasValue ? atividade.dt_fim.Value.ToShortDateString() : "--/--/----")
                        + "<br /><strong>Descrição:</strong> " + atividade.descricao;

                    if (string.IsNullOrWhiteSpace(publicacao.atividades))
                        publicacao.atividades += texto;
                    else
                        publicacao.atividades += "<br /><br />" + texto;
                }

                publicacao.operacoes =
                    "<span style='padding-right: 3px;'><a href='#' class='btn btn-info' href='#' onclick='alterar(" + item.id
                    + ");'><i class='icon-white icon-edit'></i></a></span>"
                    + "<br /><br />"
                    + "<span style='padding-right: 3px;'><a href='#' class='btn btn-danger' onclick='excluir(" + item.id
                    + ");'><i class='icon-white icon-trash'></i></a></span>";

                publicacoes.Add(publicacao);
            }


            grid.Lista = publicacoes;
            grid.Tipo = typeof(ca_publicacaoVM);

            return grid;
        }

        [HttpGet]
        public ActionResult Incluir()
        {
            PreencherVBInclusao();

            ViewBag.Submit = "Incluir";
            ViewBag.BotaoSubmit = "primary";
            ViewBag.Voltar = "Voltar";

            return View(new ca_publicacao());
        }

        [HttpPost]
        public ActionResult Incluir(FormCollection form)
        {
            try
            {
                ViewBag.Submit = "Incluir";
                ViewBag.BotaoSubmit = "primary";
                ViewBag.Voltar = "Voltar";

                ca_publicacao publicacao = new ca_publicacao();
                TryUpdateModel(publicacao, form);

                if (ViewData.ModelState.IsValid)
                {
                    ca_atividadesPublicadas atividadePublicada;

                    foreach (var item in form.AllKeys.Where(f => f.Contains("Atividade_")))
                    {
                        if (form[item].Contains("true"))
                        {
                            atividadePublicada = new ca_atividadesPublicadas();
                            atividadePublicada.id_publicacao = publicacao.id;
                            atividadePublicada.id_atividade = int.Parse(item.Replace("Atividade_", ""));

                            publicacao.ca_atividadesPublicadas.Add(atividadePublicada);
                        }
                    }

                    repositorio.Incluir(publicacao);
                    repositorio.Salvar();

                    EmailPublicacao(publicacao);

                    Mensagem(TipoMensagem.Sucesso, "Publicação realizada com sucesso!");

                    return RedirectToAction("Index");
                }
                else
                {
                    PreencherVBInclusao();

                    Mensagem(TipoMensagem.Aviso, "Preencha os campos corretamente");

                    return View(publicacao);
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Alterar(int id)
        {
            try
            {
                where = i => i.id == id;

                ca_publicacao publicacao = repositorio.Obter(where);

                ViewBag.Submit = "Alterar";
                ViewBag.BotaoSubmit = "warning";
                ViewBag.Voltar = "Voltar";

                if (publicacao != null)
                {
                    PreencherVBAlteracao(publicacao);

                    return View(publicacao);
                }
                else
                {
                    Mensagem(TipoMensagem.Erro, "ERRO: Não foi possível encontrar a Publicação(id = " + id + ").");

                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult Alterar(FormCollection form)
        {
            try
            {
                ViewBag.Submit = "Alterar";
                ViewBag.BotaoSubmit = "warning";
                ViewBag.Voltar = "Voltar";

                ca_publicacao publicacao = new ca_publicacao();
                TryUpdateModel(publicacao, form);

                if (ViewData.ModelState.IsValid)
                {
                    #region Adicionando e Removendo AtividesPublicadas

                    whereAP = ap => ap.id_publicacao == publicacao.id;
                    List<ca_atividadesPublicadas> apAntigas = repositorioAP.Listar(whereAP).ToList();
                    List<int> naoAdicionar = new List<int>();
                    ca_atividadesPublicadas atividadePublicada;

                    foreach (var antigas in apAntigas)
                    {
                        bool achou = false;

                        foreach (var item in form.AllKeys.Where(f => f.Contains("Atividade_")))
                        {
                            atividadePublicada = new ca_atividadesPublicadas();

                            if (form[item].Contains("true"))
                            {
                                if (antigas.id_atividade == int.Parse(item.Replace("Atividade_", "")))
                                    achou = true;
                            }
                        }

                        if (achou)
                            naoAdicionar.Add(antigas.id_atividade);
                        else
                            repositorioAP.Deletar(antigas);
                    }

                    repositorioAP.Salvar();

                    foreach (var item in form.AllKeys.Where(f => f.Contains("Atividade_")))
                    {
                        atividadePublicada = new ca_atividadesPublicadas();

                        if (form[item].Contains("true"))
                        {
                            if (!naoAdicionar.Any(n => n == int.Parse(item.Replace("Atividade_", ""))))
                            {
                                atividadePublicada.id_publicacao = publicacao.id;
                                atividadePublicada.id_atividade = int.Parse(item.Replace("Atividade_", ""));

                                repositorioAP.Incluir(atividadePublicada);
                            }
                        }
                    }

                    #endregion

                    repositorio.Alterar(publicacao);
                    repositorio.Salvar();

                    repositorioAP.Salvar();

                    EmailPublicacao(publicacao);

                    Mensagem(TipoMensagem.Sucesso, "Publicação realizada com sucesso!");

                    return RedirectToAction("Index");
                }
                else
                {
                    PreencherVBAlteracao(publicacao);

                    Mensagem(TipoMensagem.Aviso, "Preencha os campos corretamente");

                    return View(publicacao);
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Deletar(int id)
        {
            try
            {
                where = i => i.id == id;

                ca_publicacao publicacao = repositorio.Obter(where);

                if (publicacao != null)
                {
                    whereAP = i => i.id_publicacao == id;

                    foreach (var item in repositorioAP.Listar(whereAP))
                    {
                        repositorioAP.Deletar(item);
                    }

                    repositorioAP.Salvar();

                    repositorio.Deletar(publicacao);
                    repositorio.Salvar();

                    Mensagem(TipoMensagem.Sucesso, "Publicação deletada com sucesso!");

                    return RedirectToAction("Index");
                }
                else
                {
                    Mensagem(TipoMensagem.Erro, "ERRO: Não foi possível encontrar a Publicação(id = " + id + ").");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        private void PreencherVBInclusao()
        {
            ViewBag.id_usuario = DDLUsuarios();
            ViewBag.atividades = repositorioA.Listar(atividadesFinalizadas_e_NaoPublicadas).OrderByDescending(a => a.prioridade);
        }

        private void PreencherVBAlteracao(ca_publicacao publicacao)
        {
            atividadesFinalizadas_e_NaoPublicadas = a => a.dt_inicio.HasValue && a.dt_fim.HasValue && !a.ca_atividadesPublicadas.Any(ap => ap.id_publicacao != publicacao.id);

            ViewBag.atividades = repositorioA.Listar(atividadesFinalizadas_e_NaoPublicadas).OrderByDescending(a => a.prioridade);
            ViewBag.id_usuario = DDLUsuarios(publicacao.id_usuario);

            whereAP = i => i.id_publicacao == publicacao.id;
            ViewBag.aps = repositorioAP.Listar(whereAP);
        }

        private void EmailPublicacao(ca_publicacao publicacao)
        {
            try
            {
                whereCAU = cau => !cau.email.Contains("xxx");
                var repUsuario = new GenericRepository<ca_usuario>();
                var usuarios = repUsuario.Listar(whereCAU, true).ToList();

                foreach (var usuario in usuarios)
                {
                    try
                    {
                        string textoAtidade = "";

                        whereA = a => a.ca_atividadesPublicadas.Any(ap => ap.id_publicacao == publicacao.id);

                        var atividades = repositorioA.Listar(whereA, true).Select(a => new
                        {
                            descricao = a.descricao,
                            inicio = a.dt_inicio,
                            fim = a.dt_fim
                        });

                        foreach (var atividade in atividades)
                        {
                            textoAtidade += "- " + atividade.descricao + ". Inciada: "
                                + (atividade.inicio.HasValue ? atividade.inicio.Value.ToShortDateString() : "---")
                                + " / Finalizada: "
                                + (atividade.fim.HasValue ? atividade.fim.Value.ToShortDateString() : "---")
                                + "<br />";
                        }

                        EnviarEmail(usuario.id, "Publicação",
                            "Olá,"
                            + "<br />"
                            + "<br />"
                            + "Foi incluida uma publicação para o " + publicacao.sistema
                            + (publicacao.dt_publicacao.HasValue ?
                                (", com data de publicação: " + publicacao.dt_publicacao.Value.ToShortDateString()) : ", mas sem data de publicação")
                            + ".<br /><br />"
                            + (string.IsNullOrWhiteSpace(textoAtidade) ?
                                "Porém nenhuma atividade foi vinculada a ela." : ("As seguintes atividades estão vinculadas: <br />" + textoAtidade))
                            );
                    }
                    catch { }
                }

                var responsavel = repUsuario.Obter(u => u.id == publicacao.id_usuario);

                PushLink("Publicação do " + publicacao.sistema,
                    "Responsável: " + responsavel.nome
                    + "\r\nEnvolvendo " + publicacao.ca_atividadesPublicadas.Count + " atividade(s).\r\n"
                    + (publicacao.dt_publicacao.HasValue ? ("Data de publicação: " + publicacao.dt_publicacao.Value.ToShortDateString()) : "Sem data de publicação"),
                    "http://controle.caioff.com.br/Publicacao/Alterar/" + publicacao.id);

                var contato = PushContacts().FirstOrDefault(c => c.name.ToLower().Contains("filipe"));

                if (contato != null)
                {
                    PushLink("Publicação do " + publicacao.sistema,
                        "Responsável: " + responsavel.nome
                        + "\r\nEnvolvendo " + publicacao.ca_atividadesPublicadas.Count + " atividade(s).\r\n"
                        + (publicacao.dt_publicacao.HasValue ? ("Data de publicação: " + publicacao.dt_publicacao.Value.ToShortDateString()) : "Sem data de publicação"),
                        "http://controle.caioff.com.br/Publicacao/Alterar/" + publicacao.id,
                        contato.email);
                }
            }
            catch { }
        }
    }
}
