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

namespace Controleca_atividades.Controllers
{
    public class AtividadeController : BaseController<ca_atividade>
    {
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(string page, string sort, string filtro)
        {
            RelatorioDeAtividades();

            PreencherViewBag();

            return View(Consultar(page, sort, "", filtro));
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(string page, string sort, string sortdir, string filtro)
        {
            RelatorioDeAtividades();

            PreencherViewBag(filtro);

            return View(Consultar(page, sort, sortdir, filtro));
        }

        [HttpGet]
        public ActionResult Incluir()
        {
            ViewBag.id_usuario = DDLUsuarios();

            ViewBag.Submit = "Incluir";
            ViewBag.BotaoSubmit = "primary";
            ViewBag.Voltar = "Voltar";

            return View();
        }

        [HttpPost]
        public ActionResult Incluir(FormCollection form)
        {
            try
            {
                ViewBag.Submit = "Incluir";
                ViewBag.BotaoSubmit = "primary";
                ViewBag.Voltar = "Voltar";

                ca_atividade atividade = new ca_atividade();
                TryUpdateModel(atividade, form);

                if (ViewData.ModelState.IsValid)
                {
                    repositorio.Incluir(atividade);
                    repositorio.Salvar();

                    EmailAtividade(atividade, true);

                    Mensagem(TipoMensagem.Sucesso, "Atividade incluída com sucesso!");

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.id_usuario = DDLUsuarios();

                    Mensagem(TipoMensagem.Aviso, "Preencha os campos corretamente");

                    return View();
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

                ca_atividade atividade = repositorio.Obter(where);

                ViewBag.Submit = "Alterar";
                ViewBag.BotaoSubmit = "warning";
                ViewBag.Voltar = "Voltar";

                if (atividade != null)
                {
                    PreencherVBAlteracao(atividade);

                    return View(atividade);
                }
                else
                {
                    Mensagem(TipoMensagem.Erro, "ERRO: Não foi possível encontrar a Atividade(id = " + id + ").");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return RedirectToAction("Index");
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

                ca_atividade atividade = new ca_atividade();
                TryUpdateModel(atividade, form);

                if (ViewData.ModelState.IsValid)
                {
                    repositorio.Alterar(atividade);
                    repositorio.Salvar();

                    EmailAtividade(atividade);

                    Mensagem(TipoMensagem.Sucesso, "Atividade alterada com sucesso!");

                    return RedirectToAction("Index");
                }
                else
                {
                    PreencherVBAlteracao(atividade);

                    Mensagem(TipoMensagem.Aviso, "Preencha os campos corretamente");

                    return View(atividade);
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

                ca_atividade atividade = repositorio.Obter(where);

                if (atividade != null)
                {
                    if (atividade.ca_atividadesPublicadas != null && atividade.ca_atividadesPublicadas.Count > 0)
                    {
                        string ap = "";

                        foreach (var item in atividade.ca_atividadesPublicadas)
                        {
                            ap += (item.ca_publicacao.dt_publicacao.HasValue ?
                                ("Data da publicação: " + item.ca_publicacao.dt_publicacao.Value.ToShortDateString()) : "Ainda não publicada,")
                                + " realizada por " + item.ca_publicacao.ca_usuario.nome;
                        }

                        Mensagem(TipoMensagem.Aviso,
                            "Atividade não deletada, pois está vinculada a seguinte publicação: " + ap);
                    }
                    else
                    {
                        repositorio.Deletar(atividade);
                        repositorio.Salvar();

                        Mensagem(TipoMensagem.Sucesso, "Atividade deletada com sucesso!");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    Mensagem(TipoMensagem.Erro, "ERRO: Não foi possível encontrar a Atividade(id = " + id + ").");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Mensagem(TipoMensagem.Erro, "ERRO: " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        private GridVM Consultar(string page, string sort, string sortdir, string filtro = null)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                where = a => !a.ca_atividadesPublicadas.Any();
            }
            else
            {
                int prioridadeOuID;

                if (int.TryParse(filtro, out prioridadeOuID))
                    where = a => !a.ca_atividadesPublicadas.Any() && (a.prioridade == prioridadeOuID || a.id == prioridadeOuID);
                else
                    where = a => !a.ca_atividadesPublicadas.Any() && (a.sistema.Contains(filtro) || a.descricao.Contains(filtro));
            }

            IQueryable<ca_atividade> atividadesAuxiliar = repositorio.Listar(where)
                .OrderByDescending(a => a.prioridade).ThenByDescending(a => a.dt_inicio)
                .ToList()
                .OrderByDescending(a => a.prioridade).ThenByDescending(a => a.dt_inicio).AsQueryable();

            int pagina = 0;
            int totalRegistros = repositorio.Listar(where).Count();

            GridVM grid = new GridVM();
            grid.PageLimit = base.pageLimit;
            grid.PageSize = base.pageSize;

            if (totalRegistros > base.pageSize)
            {
                grid.TotalPages = int.Parse((totalRegistros / base.pageSize).ToString()) + 1;
            }
            else
            {
                page = "1";
                grid.TotalPages = 1;
            }

            if (Int32.TryParse(page, out pagina))
                atividadesAuxiliar = atividadesAuxiliar.Skip((pagina - 1) * base.pageSize).Take(base.pageSize);
            else
                atividadesAuxiliar = atividadesAuxiliar.Take(base.pageSize);

            grid.PageIndex = pagina;

            List<ca_atividadeVM> atividades = new List<ca_atividadeVM>();

            foreach (var item in atividadesAuxiliar)
            {
                var atividade = new ca_atividadeVM();

                atividade.usuario = item.ca_usuario == null ? "Não atribuído" : item.ca_usuario.nome;
                atividade.descricao = "<div style='width: 400px;'>" + item.id + " - " + item.descricao.Replace("\r\n", "<br />") + "</div>";
                atividade.prioridade = item.prioridade;
                atividade.dt_inicio = item.dt_inicio.HasValue ? item.dt_inicio.Value.ToShortDateString() : "Não iniciado";
                atividade.dt_fim = item.dt_inicio.HasValue ? (item.dt_fim.HasValue ? item.dt_fim.Value.ToShortDateString() : "Não finalizado") : "";
                atividade.sistema = item.sistema;
                atividade.operacoes =
                    "<span style='padding-right: 3px;'><a href='#' class='btn btn-info' href='#' onclick='alterar(" + item.id
                    + ");'><i class='icon-white icon-edit'></i></a></span>"
                    + "<br /><br />"
                    + "<span style='padding-right: 3px;'><a href='#' class='btn btn-danger' onclick='excluir(" + item.id
                    + ");'><i class='icon-white icon-trash'></i></a></span>";

                atividades.Add(atividade);
            }

            grid.Lista = atividades;
            grid.Tipo = typeof(ca_atividadeVM);

            return grid;
        }

        private void EmailAtividade(ca_atividade atividade, bool inclusao = false)
        {
            string mensagem = atividade.descricao.Replace("\r\n", "<br />");
            string mensagemPush = "";

            if (atividade.id_usuario.HasValue)
            {
                GenericRepository<ca_usuario> repUsuario = new GenericRepository<ca_usuario>();
                ca_usuario usuario = repUsuario.Obter(u => u.id == atividade.id_usuario);

                mensagemPush += usuario.nome + ", " + (inclusao ? "foi atribuído a você a seguinte atividade: " : "a seguinte atividade foi alterada: ");
                mensagemPush += atividade.id + " - ";
                mensagemPush += mensagem;

                if (usuario.nome.ToLower().Contains("caio"))
                {
                    PushLink("Controle de Atividades", mensagemPush, "http://controle.caioff.com.br/Atividade/Alterar/" + atividade.id);
                }
                else
                {
                    var contato = PushContacts().FirstOrDefault(c => c.name.ToLower().Contains("filipe"));

                    if (contato != null)
                        PushLink("Controle de Atividades", mensagemPush, "http://controle.caioff.com.br/Atividade/Alterar/" + atividade.id, contato.email);
                }
            }
            else
            {
                mensagemPush += mensagem;

                PushLink("Controle de Atividades", mensagemPush, "http://controle.caioff.com.br/Atividade/Alterar/" + atividade.id);

                var contato = PushContacts().FirstOrDefault(c => c.name.ToLower().Contains("filipe"));

                if (contato != null)
                    PushLink("Controle de Atividades", mensagemPush, "http://controle.caioff.com.br/Atividade/Alterar/" + atividade.id, contato.email);
            }

            mensagem += "<div itemscope itemtype=\"http://schema.org/EmailMessage\">"
                + "<div itemprop=\"action\" itemscope itemtype=\"http://schema.org/ConfirmAction\">"
                + "<meta itemprop=\"name\" content=\"Approve Expense\"/>"
                + "<div itemprop=\"handler\" itemscope itemtype=\"http://schema.org/HttpActionHandler\">"
                + "<link itemprop=\"url\" href=\"https://myexpenses.com/approve?expenseId=abc123\"/>"
                + "</div>"
                + "</div>"
                + "<meta itemprop=\"description\" content=\"Approval request for John's $10.13 expense for office supplies\"/>"
                + "</div>";

            if (atividade.id_usuario.HasValue)
            {
                if (inclusao)
                {
                    EnviarEmail(atividade.id_usuario.Value, "Nova Atividade" + (string.IsNullOrEmpty(atividade.sistema) ? "" : " - " + atividade.sistema),
                        "Olá,"
                        + "<br />"
                        + "<br />"
                        + "A Atividade <i>\""
                        + mensagem
                        + "\"</i>, foi atribuída a você."
                        + "<br />"
                        + "Iniciada em: " + (atividade.dt_inicio.HasValue ? atividade.dt_inicio.Value.ToShortDateString() : " --/--/----")
                        + "<br />"
                        + "Encerrada: " + (atividade.dt_fim.HasValue ? atividade.dt_fim.Value.ToShortDateString() : "--/--/----")
                        );
                }
                else
                {
                    var antigaAtividade = repositorio.Obter(where = i => i.id == atividade.id, true);

                    if (antigaAtividade.id_usuario.Value == atividade.id_usuario.Value)
                    {
                        EnviarEmail(atividade.id_usuario.Value, "Atividade alterada" + (string.IsNullOrEmpty(atividade.sistema) ? "" : " - " + atividade.sistema),
                            "Olá,"
                            + "<br />"
                            + "<br />"
                            + "A Atividade <i>\""
                            + mensagem
                            + "\"</i>, foi alterada."
                            + "<br />"
                            + "Iniciada em: " + (atividade.dt_inicio.HasValue ? atividade.dt_inicio.Value.ToShortDateString() : " --/--/----")
                            + "<br />"
                            + "Encerrada: " + (atividade.dt_fim.HasValue ? atividade.dt_fim.Value.ToShortDateString() : "--/--/----")
                        );
                    }
                    else
                    {
                        EnviarEmail(atividade.id_usuario.Value, "Atividade alterada" + (string.IsNullOrEmpty(atividade.sistema) ? "" : " - " + atividade.sistema),
                        "Olá,"
                        + "<br />"
                        + "<br />"
                        + "A Atividade <i>\""
                        + mensagem
                        + "\"</i>, foi alterada e não está mais atribuída a você."
                        + "<br />"
                        + "Iniciada em: " + (atividade.dt_inicio.HasValue ? atividade.dt_inicio.Value.ToShortDateString() : " --/--/----")
                        + "<br />"
                        + "Encerrada: " + (atividade.dt_fim.HasValue ? atividade.dt_fim.Value.ToShortDateString() : "--/--/----")
                        );
                    }
                }
            }
            else
            {
                var antigaAtividade = repositorio.Obter(where = i => i.id == atividade.id, true);

                if (antigaAtividade != null && antigaAtividade.id_usuario.HasValue && antigaAtividade.id_usuario.Value != 0)
                {
                    EnviarEmail(antigaAtividade.id_usuario.Value, "Atividade alterada" + (string.IsNullOrEmpty(atividade.sistema) ? "" : " - " + atividade.sistema),
                        "Olá,"
                        + "<br />"
                        + "<br />"
                        + "A Atividade <i>\""
                        + mensagem
                        + "\"</i>, foi alterada e não está mais atribuída a você."
                        + "<br />"
                        + "Iniciada em: " + (atividade.dt_inicio.HasValue ? atividade.dt_inicio.Value.ToShortDateString() : " --/--/----")
                        + "<br />"
                        + "Encerrada: " + (atividade.dt_fim.HasValue ? atividade.dt_fim.Value.ToShortDateString() : "--/--/----")
                        );
                }
            }
        }

        private void PreencherVBAlteracao(ca_atividade atividade)
        {
            ViewBag.id_usuario = DDLUsuarios(atividade.id_usuario);
        }

        private void PreencherVBAtividades()
        {
            try
            {
                List<ItemRelatorioAtividades> itens = new List<ItemRelatorioAtividades>();
                ItemRelatorioAtividades item;

                double totalAtividades = repositorio.Listar().Count();
                double qtdeAux, percentual;

                // Atividades iniciadas
                qtdeAux = 0;
                percentual = 0;
                item = new ItemRelatorioAtividades();

                where = a => a.dt_inicio.HasValue && !a.dt_fim.HasValue && !a.ca_atividadesPublicadas.Any();
                qtdeAux = repositorio.Listar(where).Count();

                item.Descricao = "Atividades iniciadas";
                item.Qtde = qtdeAux.ToString() + "/" + totalAtividades.ToString();
                percentual = (qtdeAux * 100) / totalAtividades;
                item.Percentual = String.Format("{0:0.00}", percentual) + "%";

                itens.Add(item);

                // Atividades encerradas e não publicadas
                qtdeAux = 0;
                percentual = 0;
                item = new ItemRelatorioAtividades();

                where = a => a.dt_fim.HasValue && (!a.ca_atividadesPublicadas.Any() || a.ca_atividadesPublicadas.Any(ap => !ap.ca_publicacao.dt_publicacao.HasValue));
                qtdeAux = repositorio.Listar(where).Count();

                item.Descricao = "Atividades encerradas e não publicadas";
                item.Qtde = qtdeAux.ToString() + "/" + totalAtividades.ToString();
                percentual = (qtdeAux * 100) / totalAtividades;
                item.Percentual = String.Format("{0:0.00}", percentual) + "%";

                itens.Add(item);

                // Atividades publicadas
                qtdeAux = 0;
                percentual = 0;
                item = new ItemRelatorioAtividades();

                where = a => a.ca_atividadesPublicadas.Any(ap => ap.ca_publicacao.dt_publicacao.HasValue);
                qtdeAux = repositorio.Listar(where).Count();

                item.Descricao = "Atividades publicadas";
                item.Qtde = qtdeAux.ToString() + "/" + totalAtividades.ToString();
                percentual = (qtdeAux * 100) / totalAtividades;
                item.Percentual = String.Format("{0:0.00}", percentual) + "%";

                itens.Add(item);

                ViewBag.RelatorioDeAtividades = itens;
            }
            catch
            {
            }
        }

        private void PreencherVBPrioridades()
        {
            try
            {
                List<string> itensPrioridade = new List<string>();
                string totalAtividades;
                string totalAtividadesPublicadas;
                string totalAtividadesNaoPublicadas;
                string totalAtividadesNaoIniciadas;
                string totalAtividadesIniciadas;

                for (int prioridade = 1; prioridade <= 5; prioridade++)
                {
                    where = a => a.prioridade == prioridade;
                    totalAtividades = repositorio.Listar(where).Count().ToString();

                    where = a => a.prioridade == prioridade && a.dt_inicio.HasValue && !a.dt_fim.HasValue && !a.ca_atividadesPublicadas.Any();
                    totalAtividadesIniciadas = repositorio.Listar(where).Count().ToString();

                    where = a => a.prioridade == prioridade && !a.dt_inicio.HasValue && !a.ca_atividadesPublicadas.Any();
                    totalAtividadesNaoIniciadas = repositorio.Listar(where).Count().ToString();

                    where = a => a.prioridade == prioridade && a.ca_atividadesPublicadas.Any(ap => ap.ca_publicacao.dt_publicacao.HasValue);
                    totalAtividadesPublicadas = repositorio.Listar(where).Count().ToString();

                    where = a => a.prioridade == prioridade && a.dt_fim.HasValue
                        && (!a.ca_atividadesPublicadas.Any() || a.ca_atividadesPublicadas.Any(ap => !ap.ca_publicacao.dt_publicacao.HasValue));
                    totalAtividadesNaoPublicadas = repositorio.Listar(where).Count().ToString();

                    if (!string.IsNullOrWhiteSpace(totalAtividades))
                    {
                        string classe = "label ";

                        if (prioridade == 2)
                            classe += "label-success";
                        else if (new[] { 3, 4 }.Contains(prioridade))
                            classe += "label-warning";
                        else if (prioridade == 5)
                            classe += "label-important";

                        totalAtividades = "<span class='" + classe + "'>"
                            + "<b>Prioridade:</b> " + prioridade + "<br />"
                            + "<b>Total:</b> " + totalAtividades + "<br />"
                            + "<b>Iniciada:</b> " + totalAtividadesIniciadas + "<br />"
                            + "<b>Não iniciada:</b> " + totalAtividadesNaoIniciadas + "<br />"
                            + "<b>Publicada:</b> " + totalAtividadesPublicadas + "<br />"
                            + "<b>Não publicada:</b> " + totalAtividadesNaoPublicadas + "<br />"
                            + "</span>";

                        itensPrioridade.Add(totalAtividades);
                    }
                }

                ViewBag.RelatorioDeAtividadesPorPrioridade = itensPrioridade;
            }
            catch { }
        }

        private void RelatorioDeAtividades()
        {
            try
            {
                PreencherVBAtividades();

                //============================================================================

                PreencherVBPrioridades();
            }
            catch
            {
            }
        }

        private void PreencherViewBag(string filtro = null)
        {
            ViewBag.Incluir = "Novo";

            if (!string.IsNullOrEmpty(filtro))
                ViewBag.filtro = filtro;
        }
    }
}
