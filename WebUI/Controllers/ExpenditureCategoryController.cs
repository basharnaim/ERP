using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Inventory.Expenditures;
using Library.Service.Inventory.Expenditures;
using Library.ViewModel.Inventory.Expenditures;

namespace ERP.WebUI.Controllers
{
    public class ExpenditureCategoryController : BaseController
    {
        #region Ctor
        private readonly IExpenditureCategoryService _expenditureCategoryService;

        public ExpenditureCategoryController(IExpenditureCategoryService expenditureCategoryService)
        {
            _expenditureCategoryService = expenditureCategoryService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureCategoryViewModel>>(_expenditureCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        #endregion

        #region JSon

        public JsonResult GetExpenditureCategoryList()
        {
            try
            {
                return Json(new SelectList(_expenditureCategoryService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new ExpenditureCategoryViewModel { Active = true, Sequence = _expenditureCategoryService.GetAutoSequence() });

            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ExpenditureCategoryViewModel expenditureCategoryVM)
        {
            try
            {
                _expenditureCategoryService.Add(AutoMapperConfiguration.mapper.Map<ExpenditureCategory>(expenditureCategoryVM));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/ExpenditureCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<ExpenditureCategoryViewModel>(_expenditureCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ExpenditureCategoryViewModel expenditureCategoryvm)
        {
            try
            {
                _expenditureCategoryService.Update(AutoMapperConfiguration.mapper.Map<ExpenditureCategory>(expenditureCategoryvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/ExpenditureCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
