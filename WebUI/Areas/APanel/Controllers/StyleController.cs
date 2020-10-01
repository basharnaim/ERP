using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class StyleController : BaseController
    {
        #region Ctor
        private readonly IStyleService _styleService;
        public StyleController(
            IStyleService styleService
            )
        {
            _styleService = styleService;
        }
        #endregion

        #region Get
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<StyleViewModel>>(_styleService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetStyleList()
        {
            try
            {
                return Json(new SelectList(_styleService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                return View(new StyleViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Create(StyleViewModel Stylevm)
        {
            try
            {
                _styleService.Add(AutoMapperConfiguration.mapper.Map<Style>(Stylevm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/Style"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit

        [HttpGet]
        [Authorize]
        public ActionResult Edit(string id)
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<StyleViewModel>(_styleService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Edit(StyleViewModel Stylevm)
        {
            try
            {
                _styleService.Update(AutoMapperConfiguration.mapper.Map<Style>(Stylevm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Style"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Upload
        [HttpGet]
        [Authorize]
        public ActionResult Upload()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    var result = _styleService.UploadFromDirectory(file);
                    WriteExcelFile(result, "Error");
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Style"}')");
                }
                return JavaScript($"ShowResult('Please select file','failure')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
