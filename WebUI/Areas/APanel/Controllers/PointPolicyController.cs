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
    public class PointPolicyController : BaseController
    {
        #region Ctor
        private readonly IPointPolicyService _PointPolicyService;

        public PointPolicyController(
            IPointPolicyService PointPolicyService
            )
        {
            _PointPolicyService = PointPolicyService;
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
                    return View(Mapper.Map<IEnumerable<PointPolicyViewModel>>(_PointPolicyService.GetAll(dfrom.Value, dto.Value)));
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
        public JsonResult GetPointPolicyList()
        {
            return Json(new SelectList(_PointPolicyService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Partial
        [HttpPost, Authorize]
        public ViewResult BlankItem()
        {
            try
            {
                return View("_PointPolicyDetailRow", new PointPolicyDetailViewModel());
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
                var PointPolicy = new PointPolicyViewModel
                {
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now,
                    Active = true,
                    PointPolicyDetails = new List<PointPolicyDetailViewModel>
                    {
                        new PointPolicyDetailViewModel()
                    }
                };
                return View(PointPolicy);
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JavaScriptResult Create(PointPolicyViewModel PointPolicyVm)
        {
            try
            {
                if (PointPolicyVm.PointPolicyDetails.All(x => x.SaleAmount > 0))
                {
                    var PointPolicy = Mapper.Map<PointPolicy>(PointPolicyVm);
                    var PointPolicyDetails = Mapper.Map<List<PointPolicyDetail>>(PointPolicyVm.PointPolicyDetails);
                    PointPolicy.PointPolicyDetails = new List<PointPolicyDetail>();
                    foreach (var item in PointPolicyDetails)
                    {
                        PointPolicy.PointPolicyDetails.Add(item);
                    }
                    _PointPolicyService.Add(PointPolicy);
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PointPolicy/?dateFrom=" + PointPolicyVm.DateFrom.ToString("dd-MMM-yyyy") + "&dateTo=" + PointPolicyVm.DateTo.ToString("dd-MMM-yyyy")}')");
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
                var PointPolicy = Mapper.Map<PointPolicyViewModel>(_PointPolicyService.GetById(id));
                var PointPolicyDetails = Mapper.Map<List<PointPolicyDetailViewModel>>(_PointPolicyService.GetAllPointPolicyDetailbyMasterId(id));
                PointPolicy.PointPolicyDetails = new List<PointPolicyDetailViewModel>();
                PointPolicy.PointPolicyDetails.AddRange(PointPolicyDetails);
                return View(PointPolicy);
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(PointPolicyViewModel PointPolicyVm)
        {
            try
            {
                if (PointPolicyVm.PointPolicyDetails.All(x => x.SaleAmount > 0))
                {
                    var PointPolicy = Mapper.Map<PointPolicy>(PointPolicyVm);
                    var PointPolicyDetails = Mapper.Map<List<PointPolicyDetail>>(PointPolicyVm.PointPolicyDetails);
                    PointPolicy.PointPolicyDetails = new List<PointPolicyDetail>();
                    foreach (var item in PointPolicyDetails)
                    {
                        PointPolicy.PointPolicyDetails.Add(item);
                    }
                    _PointPolicyService.Update(PointPolicy);
                    return JavaScript($"ShowResult('{"Data Updated successfully."}','{"success"}','{"redirect"}','{"/APanel/PointPolicy/?dateFrom=" + PointPolicyVm.DateFrom.ToString("dd-MMM-yyyy") + "&dateTo=" + PointPolicyVm.DateTo.ToString("dd-MMM-yyyy")}')");
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