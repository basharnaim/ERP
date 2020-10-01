#region Using

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

#endregion

namespace ERP.WebUI.Controllers
{
    public class RAMController : BaseController
    {
        #region Ctor
        private readonly IRAMService _ramService;
        public RAMController(
            IRAMService ramService
            )
        {
            _ramService = ramService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<RAMViewModel>>(_ramService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetRAMList()
        {
            try
            {
                return Json(new SelectList(_ramService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new RAMViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(RAMViewModel RAMvm)
        {
            try
            {
                _ramService.Add(AutoMapperConfiguration.mapper.Map<RAM>(RAMvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/RAM"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<RAMViewModel>(_ramService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(RAMViewModel RAMvm)
        {
            try
            {
                _ramService.Update(AutoMapperConfiguration.mapper.Map<RAM>(RAMvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/RAM"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
