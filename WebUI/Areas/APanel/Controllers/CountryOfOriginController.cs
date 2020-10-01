﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Core.Addresses;
using Library.Service.Core.Addresses;
using Library.ViewModel.Core.Addresses;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class CountryOfOriginController : BaseController
    {
        #region Ctor
        private readonly ICountryService _countryService;
        public CountryOfOriginController(ICountryService countryService)
        {
            _countryService = countryService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<CountryViewModel>>(_countryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetCountryList()
        {
            return Json(new SelectList(_countryService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new CountryViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(CountryViewModel countryVm)
        {
            try
            {
                _countryService.Add(AutoMapperConfiguration.mapper.Map<Country>(countryVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"."}')");
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
                return View(AutoMapperConfiguration.mapper.Map<CountryViewModel>(_countryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(CountryViewModel countryVm)
        {
            try
            {
                _countryService.Update(AutoMapperConfiguration.mapper.Map<Country>(countryVm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"../"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
