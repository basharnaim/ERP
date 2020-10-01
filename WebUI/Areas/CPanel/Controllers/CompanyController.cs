using ERP.WebUI.Controllers;
using Library.Model.Core.Organizations;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.CPanel.Controllers
{
    public class CompanyController : BaseController
    {
        #region Ctor
        private readonly ICompanyService _companyService;
        public CompanyController(
            ICompanyService companyService
            )
        {
            _companyService = companyService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<CompanyViewModel>>(_companyService.GetAllForCPanel()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #endregion

        #region JSon
        public JsonResult GetCompanyList()
        {
            return Json(new SelectList(_companyService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new CompanyViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]

        public JavaScriptResult Create(CompanyViewModel comvm)
        {
            try
            {
                _companyService.Add(AutoMapperConfiguration.mapper.Map<Company>(comvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/CPanel/Company"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<CompanyViewModel>(_companyService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]

        public ActionResult Edit(CompanyViewModel comvm)
        {
            try
            {
                _companyService.Update(AutoMapperConfiguration.mapper.Map<Company>(comvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/CPanel/Company"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
