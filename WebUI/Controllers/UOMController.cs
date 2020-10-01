using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Controllers
{
    public class UOMController : BaseController
    {
        #region Ctor
        private readonly IUomService _uomService;
        public UOMController(
            IUomService uomService
            )
        {
            _uomService = uomService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<UomViewModel>>(_uomService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetUOMList()
        {
            try
            {
                return Json(new SelectList(_uomService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new UomViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(UomViewModel uOMvm)
        {
            try
            {
                _uomService.Add(AutoMapperConfiguration.mapper.Map<Uom>(uOMvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/UOM"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<UomViewModel>(_uomService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(UomViewModel uOMvm)
        {
            try
            {
                _uomService.Update(AutoMapperConfiguration.mapper.Map<Uom>(uOMvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/UOM"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
