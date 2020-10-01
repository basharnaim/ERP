using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Core.Organizations;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class CompanyController : BaseController
    {
        #region Ctor
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<CompanyViewModel>>(_companyService.GetAll().Where(r => r.Active)));
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
            try
            {
                return Json(new SelectList(_companyService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
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
                comvm.Active = true;
                _companyService.Update(AutoMapperConfiguration.mapper.Map<Company>(comvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Company"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
