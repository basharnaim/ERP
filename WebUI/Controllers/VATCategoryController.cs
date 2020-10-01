using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class VatCategoryController : BaseController
    {
        #region Ctor
        private readonly IVatCategoryService _vatCategoryService;
        public VatCategoryController(
            IVatCategoryService vatCategoryService
            )
        {
            _vatCategoryService = vatCategoryService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<VatCategoryViewModel>>(_vatCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetVatCategoryList()
        {
            try
            {
                return Json(new SelectList(_vatCategoryService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new VatCategoryViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(VatCategoryViewModel vatCategoryVm)
        {
            try
            {
                _vatCategoryService.Add(AutoMapperConfiguration.mapper.Map<VatCategory>(vatCategoryVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/VATCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit

        [HttpGet]
        [Authorize]
        public ActionResult Edit(string id)
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<VatCategoryViewModel>(_vatCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Edit(VatCategoryViewModel vatCategoryVm)
        {
            try
            {
                _vatCategoryService.Update(AutoMapperConfiguration.mapper.Map<VatCategory>(vatCategoryVm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/VATCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
