using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Promotions;
using Library.Service.Inventory.Promotions;
using Library.ViewModel.Inventory.Promotions;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class PromotionalFreeItemController : BaseController
    {
        #region Ctor
        private readonly IPromotionalFreeItemService _promotionalFreeItemService;

        public PromotionalFreeItemController(
            IPromotionalFreeItemService promotionalFreeItemService
            )
        {
            _promotionalFreeItemService = promotionalFreeItemService;
        }

        #endregion

        #region Get
        public ActionResult Index(string dateFrom, string dateTo)
        {
            try
            {
                DateTime? dfrom = null;
                DateTime? dto = null;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                }
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<PromotionalFreeItemViewModel>>(_promotionalFreeItemService.GetAll(dfrom.Value, dto.Value)));
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }
        #endregion

        #region JSon
        public JsonResult GetPromotionalFreeItemList()
        {
            return Json(new SelectList(_promotionalFreeItemService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Partial
        [HttpPost, Authorize]
        public ViewResult BlankItem()
        {
            try
            {
                return View("_PromotionalFreeItemDetailRow", new PromotionalFreeItemDetailViewModel());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var promotionalFreeItem = new PromotionalFreeItemViewModel
                {
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now,
                    Active = true,
                    PromotionalFreeItemDetails = new List<PromotionalFreeItemDetailViewModel>()
                };
                promotionalFreeItem.PromotionalFreeItemDetails.Add(new PromotionalFreeItemDetailViewModel());
                return View(promotionalFreeItem);
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JavaScriptResult Create(PromotionalFreeItemViewModel promotionalFreeItemVm)
        {
            try
            {
                if (promotionalFreeItemVm.PromotionalFreeItemDetails.All(x => x.EligibleQuantity > 0))
                {
                    var promotionalFreeItem = AutoMapperConfiguration.mapper.Map<PromotionalFreeItem>(promotionalFreeItemVm);
                    var promotionalFreeItemDetails = AutoMapperConfiguration.mapper.Map<List<PromotionalFreeItemDetail>>(promotionalFreeItemVm.PromotionalFreeItemDetails);
                    promotionalFreeItem.PromotionalFreeItemDetails = new List<PromotionalFreeItemDetail>();
                    foreach (var item in promotionalFreeItemDetails)
                    {
                        promotionalFreeItem.PromotionalFreeItemDetails.Add(item);
                    }
                    _promotionalFreeItemService.Add(promotionalFreeItem);
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PromotionalFreeItem/?dateFrom=" + promotionalFreeItemVm.DateFrom.ToString("dd-MMM-yyyy") + "&dateTo=" + promotionalFreeItemVm.DateTo.ToString("dd-MMM-yyyy")}')");
                }
                return JavaScript($"ShowResult('{"Enter quantity!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                var promotionalFreeItem = AutoMapperConfiguration.mapper.Map<PromotionalFreeItemViewModel>(_promotionalFreeItemService.GetById(id));
                var promotionalFreeItemDetails = AutoMapperConfiguration.mapper.Map<List<PromotionalFreeItemDetailViewModel>>(_promotionalFreeItemService.GetAllTradeOfferOnFreeItemDetailbyMasterId(id));
                promotionalFreeItem.PromotionalFreeItemDetails = new List<PromotionalFreeItemDetailViewModel>();
                promotionalFreeItem.PromotionalFreeItemDetails.AddRange(promotionalFreeItemDetails);
                return View(promotionalFreeItem);
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(PromotionalFreeItemViewModel promotionalFreeItemVm)
        {
            try
            {
                if (promotionalFreeItemVm.PromotionalFreeItemDetails.All(x => x.EligibleQuantity > 0))
                {
                    var promotionalFreeItem = AutoMapperConfiguration.mapper.Map<PromotionalFreeItem>(promotionalFreeItemVm);
                    var promotionalFreeItemDetails = AutoMapperConfiguration.mapper.Map<List<PromotionalFreeItemDetail>>(promotionalFreeItemVm.PromotionalFreeItemDetails);
                    promotionalFreeItem.PromotionalFreeItemDetails = new List<PromotionalFreeItemDetail>();
                    foreach (var item in promotionalFreeItemDetails)
                    {
                        promotionalFreeItem.PromotionalFreeItemDetails.Add(item);
                    }
                    _promotionalFreeItemService.Update(promotionalFreeItem);
                    return JavaScript($"ShowResult('{"Data Updated successfully."}','{"success"}','{"redirect"}','{"/APanel/PromotionalFreeItem/?dateFrom=" + promotionalFreeItemVm.DateFrom.ToString("dd-MMM-yyyy") + "&dateTo=" + promotionalFreeItemVm.DateTo.ToString("dd-MMM-yyyy")}')");
                }
                return JavaScript($"ShowResult('{"Enter quantity!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion
    }
}