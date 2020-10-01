using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Expenditures;
using Library.Service.Inventory.Expenditures;
using Library.ViewModel.Inventory.Expenditures;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ExpenditureSubsidiaryCategoryController : BaseController
    {
        #region Ctor
        private readonly IExpenditureSubsidiaryCategoryService _expenditureSubsidiaryCategoryService;

        public ExpenditureSubsidiaryCategoryController(IExpenditureSubsidiaryCategoryService expenditureSubsidiaryCategoryService)
        {
            _expenditureSubsidiaryCategoryService = expenditureSubsidiaryCategoryService;
        }
        #endregion

        #region Get

        public ActionResult Index(string expenditureCategoryId, string expenditureSubCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(expenditureCategoryId) && !string.IsNullOrEmpty(expenditureSubCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureSubsidiaryCategoryViewModel>>(_expenditureSubsidiaryCategoryService.GetAll(expenditureCategoryId, expenditureSubCategoryId)));
                if (!string.IsNullOrEmpty(expenditureSubCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureSubsidiaryCategory>, IEnumerable<ExpenditureSubsidiaryCategoryViewModel>>(_expenditureSubsidiaryCategoryService.GetAll(expenditureSubCategoryId)));
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureSubsidiaryCategory>, IEnumerable<ExpenditureSubsidiaryCategoryViewModel>>(_expenditureSubsidiaryCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public JsonResult GetExpenditureSubsidiaryCategoryList(string expenditureSubCategoryId)
        {
            try
            {
                return Json(new SelectList(_expenditureSubsidiaryCategoryService.Lists(expenditureSubCategoryId), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new ExpenditureSubsidiaryCategoryViewModel {Active=true, Sequence= _expenditureSubsidiaryCategoryService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
            
        }

        [HttpPost]
        public JavaScriptResult Create(ExpenditureSubsidiaryCategoryViewModel expenditureSubsidiaryCategoryvm)
        {
            try
            {
                _expenditureSubsidiaryCategoryService.Add(AutoMapperConfiguration.mapper.Map<ExpenditureSubsidiaryCategory>(expenditureSubsidiaryCategoryvm));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ExpenditureSubsidiaryCategory/?ProductCategory=" + expenditureSubsidiaryCategoryvm.ExpenditureCategoryId + "&&ProductSubCategoryId=" + expenditureSubsidiaryCategoryvm.ExpenditureSubCategoryId}')");
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
                return View(AutoMapperConfiguration.mapper.Map<ExpenditureSubsidiaryCategoryViewModel>(_expenditureSubsidiaryCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ExpenditureSubsidiaryCategoryViewModel expenditureSubsidiaryCategoryvm)
        {
            try
            {
                _expenditureSubsidiaryCategoryService.Update(AutoMapperConfiguration.mapper.Map<ExpenditureSubsidiaryCategory>(expenditureSubsidiaryCategoryvm));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ExpenditureSubsidiaryCategory/?ProductCategory=" + expenditureSubsidiaryCategoryvm.ExpenditureCategoryId + "&&ProductSubCategoryId=" + expenditureSubsidiaryCategoryvm.ExpenditureSubCategoryId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
