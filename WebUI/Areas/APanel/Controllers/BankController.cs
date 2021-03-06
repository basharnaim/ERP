﻿#region Using

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Core.Banks;
using Library.Service.Core.Banks;
using Library.ViewModel.Core.Banks;

#endregion

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class BankController : BaseController
    {
        #region Ctor
        private readonly IBankService _bankService;
        public BankController(
            IBankService bankService
            )
        {
            _bankService = bankService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(Mapper.Map<IEnumerable<BankViewModel>>(_bankService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetBankList()
        {
            try
            {
                return Json(new SelectList(_bankService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new BankViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(BankViewModel bankvm)
        {
            try
            {
                _bankService.Add(Mapper.Map<Bank>(bankvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/Bank"}')");
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
                return View(Mapper.Map<BankViewModel>(_bankService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(BankViewModel bankvm)
        {
            try
            {
                _bankService.Update(Mapper.Map<Bank>(bankvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Bank"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
