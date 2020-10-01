using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Core.Banks;
using Library.Service.Core.Banks;
using Library.ViewModel.Core.Banks;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class BankBranchController : BaseController
    {
        #region Ctor
        private readonly IBankBranchService _bankBranchService;
        public BankBranchController(
            IBankBranchService bankBranchService
            )
        {
            _bankBranchService = bankBranchService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<BankBranchViewModel>>(_bankBranchService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #endregion

        #region JSon
        public JsonResult GetBankBranchList(string bankId)
        {
            try
            {
                return Json(new SelectList(_bankBranchService.Lists(bankId), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new BankBranchViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(BankBranchViewModel bankBranchvm)
        {
            try
            {
                _bankBranchService.Add(AutoMapperConfiguration.mapper.Map<BankBranch>(bankBranchvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/BankBranch"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<BankBranchViewModel>(_bankBranchService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(BankBranchViewModel bankBranchvm)
        {
            try
            {
                _bankBranchService.Update(AutoMapperConfiguration.mapper.Map<BankBranch>(bankBranchvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/BankBranch"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
