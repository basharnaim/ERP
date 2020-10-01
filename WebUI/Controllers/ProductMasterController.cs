#region Using

using Library.Crosscutting.Securities;
using Library.Context.Repositories;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

#endregion

namespace ERP.WebUI.Controllers
{
    public class ProductMasterController : BaseController
    {
        #region Ctor
        private readonly IProductMasterService _productMasterService;
        private readonly IRawSqlService _rawSqlService;
        public ProductMasterController(
            IProductMasterService productMasterService,
            IRawSqlService rawSqlService
        )
        {
            _productMasterService = productMasterService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductMasterViewModel>>(_productMasterService.GetAll(companyId, branchId)));
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
                return Json(new SelectList(_productMasterService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
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


        #region PartialView
        public ActionResult GetDailogProductList()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return PartialView("_ProductList", AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productMasterService.GetProductDialogProductList(identity.CompanyId, identity.BranchId)));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult GetDialogProduct(string productId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return Json(AutoMapperConfiguration.mapper.Map<ProductStockViewModel>(_rawSqlService.GetProductDetailWithStock(identity.CompanyId, identity.BranchId, productId)));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productMasterService.GetProductMasterTemplate(companyId, branchId, productCategoryId)));
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
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/ProductMaster/?companyId=" + templateCompany + "&&branchId=" + templateBranch}')");
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
