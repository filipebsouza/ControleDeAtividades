using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using fav.Repository.Logic;
using fav.Domain.Model;

namespace fav.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public JsonResult CadastrarUsuario(FormCollection collection)
        {
            try
            {
                var usuario = new Usuario();

                TryUpdateModel(usuario, collection);

                if (ModelState.IsValid)
                {
                    var repositorio = new GenericRepository<Usuario>();
                    repositorio.Incluir(usuario);
                    repositorio.Salvar();

                    return Json(new { mensagem = "OK", resultado = ViewData.ModelState.Values.Where(e => e.Errors.Count > 0) });
                }
                else
                {
                    return Json(new { mensagem = "ERRO", resultado = ViewData.ModelState.Values.Where(e => e.Errors.Count > 0) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { mensagem = "ERRO", resultado = ex.Message });
            }

        }

    }
}
