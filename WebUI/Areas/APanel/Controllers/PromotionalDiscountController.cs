using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Promotions;
using Library.Service.Inventory.Promotions;
using Library.ViewModel.Inventory.Promotions;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class PromotionalDiscountController : BaseController
    {
        #region Ctor
        private readonly IPromotionalDiscountService _promotionalDiscountService;

        public PromotionalDiscountController(
            IPromotionalDiscountService promotionalDiscountService
            )
        {
            _promotionalDiscountService = promotionalDiscountService;
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
                    return View(Mapper.Map<IEnumerable<PromotionalDiscountViewModel>>(_promotionalDiscountService.GetAll(dfrom.Value, dto.Value)));
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
        public JsonResult GetPromotionalDiscountList()
        {
            return Json(new SelectList(_promotionalDiscountService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Partial
        [HttpPost, Authorize]
        public ViewResult BlankItem()
        {
            try
            {
                return View("_PromotionalDiscountDetailRow", new PromotionalDiscountDetailViewModel());
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
                var promotionalDiscount = new PromotionalDiscountViewModel
                {
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now,
                    Active = true,
                    PromotionalDiscountDetails = new List<PromotionalDiscountDetailViewModel>
                    {
                        new PromotionalDiscountDetailViewModel()
                    }
                };
                return View(promotionalDiscount);
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JavaScriptResult Create(PromotionalDiscountViewModel promotionalDiscountVm)
        {
            try
            {
                if (promotionalDiscountVm.PromotionalDiscountDetails.All(x => x.EligibleQuantity > 0))
                {
                    var promotionalDiscount = Mapper.Map<PromotionalDiscount>(promotionalDiscountVm);
                    var promotionalDiscountDetails = Mapper.Map<List<PromotionalDiscountDetail>>(promotionalDiscountVm.PromotionalDiscountDetails);
                    promotionalDiscount.PromotionalDiscountDetails = new List<PromotionalDiscountDetail>();
                    foreach (var item in promotionalDiscountDetails)
                    {
                        promotionalDiscount.PromotionalDiscountDetails.Add(item);
                    }
                    _promotionalDiscountService.Add(promotionalDiscount);
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PromotionalDiscount/?dateFrom=" + promotionalDiscountVm.DateFrom.ToString("dd-MMM-yyyy") + "&dateTo=" + promotionalDiscountVm.DateTo.ToString("dd-MMM-yyyy")}')");
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
                var promotionalDiscount = Mapper.Map<PromotionalDiscountViewModel>(_promotionalDiscountService.GetById(id));
                var promotionalDiscountDetails = Mapper.Map<List<PromotionalDiscountDetailViewModel>>(_promotionalDiscountService.GetAllPromotionalDiscountDetailbyMasterId(id));
                promotionalDiscount.PromotionalDiscountDetails = new List<PromotionalDiscountDetailViewModel>();
                promotionalDiscount.PromotionalDiscountDetails.AddRange(promotionalDiscountDetails);
                return View(promotionalDiscount);
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(PromotionalDiscountViewModel promotionalDiscountVm)
        {
            try
            {
                if (promotionalDiscountVm.PromotionalDiscountDetails.All(x => x.EligibleQuantity > 0))
                {
                    var promotionalDiscount = Mapper.Map<PromotionalDiscount>(promotionalDiscountVm);
                    var promotionalDiscountDetails = Mapper.Map<List<PromotionalDiscountDetail>>(promotionalDiscountVm.PromotionalDiscountDetails);
                    promotionalDiscount.PromotionalDiscountDetails = new List<PromotionalDiscountDetail>();
                    foreach (var item in promotionalDiscountDetails)
                    {
                        promotionalDiscount.PromotionalDiscountDetails.Add(item);
                    }
                    _promotionalDiscountService.Update(promotionalDiscount);
                    return JavaScript($"ShowResult('{"Data Updated successfully."}','{"success"}','{"redirect"}','{"/APanel/PromotionalDiscount/?dateFrom=" + promotionalDiscountVm.DateFrom.ToString("dd-MMM-yyyy") + "&dateTo=" + promotionalDiscountVm.DateTo.ToString("dd-MMM-yyyy")}')");
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