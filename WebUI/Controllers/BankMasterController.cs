#region Using

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Library.Model.Core.Banks;
using Library.Service.Core.Banks;
using Library.ViewModel.Core.Banks;

#endregion

namespace ERP.WebUI.Controllers
{
    public class BankMasterController : BaseController
    {
        #region Ctor
        private readonly IBankMasterService _bankMasterService;
        public BankMasterController(
            IBankMasterService bankMasterService
            )
        {
            _bankMasterService = bankMasterService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(Mapper.Map<IEnumerable<BankMasterViewModel>>(_bankMasterService.GetAll(companyId, branchId)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #endregion

        #region JSon
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBankList()
        {
            return Json(new SelectList(_bankMasterService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bankBranchId"></param>
        /// <returns></returns>
        public JsonResult GetAccountNoList(string bankBranchId)
        {
            return Json(new SelectList(_bankMasterService.AccountNoListByBankBranchId(bankBranchId), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new BankMasterViewModel { Active = true, OpeningDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Create(BankMasterViewModel bankMasterVM)
        {

            try
            {
                _bankMasterService.Add(Mapper.Map<BankMaster>(bankMasterVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/BankMaster/?companyId=" + bankMasterVM.CompanyId + "&&branchId=" + bankMasterVM.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public void ChangeStatus(string id)
        {
            _bankMasterService.ChangeStatus(id);
        }
    }
}
