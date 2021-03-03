using AutoMapper;
using Library.Model.Core.Organizations;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
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
                var data = Mapper.Map<IEnumerable<CompanyViewModel>>(_companyService.GetAll().Where(r => r.Active));
                return View(data);
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
                return View(Mapper.Map<CompanyViewModel>(_companyService.GetById(id)));
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
                _companyService.Update(Mapper.Map<Company>(comvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Company"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
