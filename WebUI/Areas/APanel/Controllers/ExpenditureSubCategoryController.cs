using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Expenditures;
using Library.Service.Inventory.Expenditures;
using Library.ViewModel.Inventory.Expenditures;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ExpenditureSubCategoryController : BaseController
    {
        #region Ctor
        private readonly IExpenditureSubCategoryService _expenditureSubCategoryService;
        public ExpenditureSubCategoryController(IExpenditureSubCategoryService expenditureSubCategoryService)
        {
            _expenditureSubCategoryService = expenditureSubCategoryService;
        }
        #endregion

        #region Get

        public ActionResult Index(string expenditureCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(expenditureCategoryId))
                    return View(Mapper.Map<IEnumerable<ExpenditureSubCategoryViewModel>>(_expenditureSubCategoryService.GetAll(expenditureCategoryId)));
                return View(Mapper.Map<IEnumerable<ExpenditureSubCategoryViewModel>>(_expenditureSubCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        public JsonResult GetExpenditureSubCategoryList(string expenditureCategoryId)
        {
            try
            {
                return Json(new SelectList(_expenditureSubCategoryService.Lists(expenditureCategoryId), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                ExpenditureSubCategoryViewModel expenditureSubCategoryvm = new ExpenditureSubCategoryViewModel
                {
                    Active = true,
                    Sequence = _expenditureSubCategoryService.GetAutoSequence(),
                };
                return View(expenditureSubCategoryvm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
            
        }

        [HttpPost]
        public JavaScriptResult Create(ExpenditureSubCategoryViewModel expenditureSubCategoryVM)
        {
            try
            {
                _expenditureSubCategoryService.Add(Mapper.Map<ExpenditureSubCategory>(expenditureSubCategoryVM));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ExpenditureSubCategory/?ProductCategoryId=" + expenditureSubCategoryVM.ExpenditureCategoryId}')");
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
                return View(Mapper.Map<ExpenditureSubCategoryViewModel>(_expenditureSubCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ExpenditureSubCategoryViewModel expenditureSubCategoryVM)
        {
            try
            {
                _expenditureSubCategoryService.Update(Mapper.Map<ExpenditureSubCategory>(expenditureSubCategoryVM));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ExpenditureSubCategory/?ProductCategoryId=" + expenditureSubCategoryVM.ExpenditureCategoryId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
