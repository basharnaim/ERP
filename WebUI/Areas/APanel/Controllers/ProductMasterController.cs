#region Using

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

#endregion

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ProductMasterController : BaseController
    {
        #region Ctor
        private readonly IProductMasterService _productMasterService;
        public ProductMasterController(IProductMasterService productMasterService)
        {
            _productMasterService = productMasterService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(Mapper.Map<IEnumerable<ProductMasterViewModel>>(_productMasterService.GetAll(companyId, branchId)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetProductList()
        {
            try
            {
               var data = _productMasterService.Lists();
                return Json(new SelectList(_productMasterService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetProductListbySupplier(string Id, string subId, string cId)
        {
            return Json(new SelectList(_productMasterService.ListsBySupplier(Id, subId, cId), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductCategoryList(string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(branchId))
                    return Json(new SelectList(_productMasterService.GetProductCategoryByCompanyBranch(branchId), "Value", "Text"), JsonRequestBehavior.AllowGet);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetProductByCompanyBranch(string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(branchId))
                    return Json(new SelectList(_productMasterService.GetProductByCompanyBranch(branchId), "Value", "Text"), JsonRequestBehavior.AllowGet);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        [HttpGet]
        public ActionResult Create(string companyId, string branchId, string productCategoryId, string others)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productMasterService.GetProductMasterTemplate(companyId, branchId, productCategoryId)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }

        [HttpPost]
        public ActionResult Create(string templateIds, string templateCompany, string templateBranch)
        {
            try
            {
                string[] ids = new JavaScriptSerializer().Deserialize<string[]>(templateIds);
                _productMasterService.Add(ids, templateCompany, templateBranch);
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductMaster/?companyId=" + templateCompany + "&&branchId=" + templateBranch}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }

        public void ChangeStatus(string id)
        {
            _productMasterService.ChangeStatus(id);
        }
    }
}
