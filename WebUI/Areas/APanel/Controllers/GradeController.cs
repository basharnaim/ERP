using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class GradeController : BaseController
    {
        #region Ctor
        private readonly IGradeService _gradeService;
        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }
        #endregion

        #region Get
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<GradeViewModel>>(_gradeService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetGradeList()
        {
            try
            {
                return Json(new SelectList(_gradeService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new GradeViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Create(GradeViewModel gradevm)
        {
            try
            {
                _gradeService.Add(AutoMapperConfiguration.mapper.Map<Grade>(gradevm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/Grade"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<GradeViewModel>(_gradeService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Edit(GradeViewModel gradevm)
        {
            try
            {
                _gradeService.Update(AutoMapperConfiguration.mapper.Map<Grade>(gradevm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Grade"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
