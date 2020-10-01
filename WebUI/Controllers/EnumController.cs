using Library.Service.Core.Enums;
using System;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class EnumController : BaseController
    {
        #region Ctor
        private readonly IEnumService _enumService;
        public EnumController(
            IEnumService enumService
            )
        {
            _enumService = enumService;
        }
        #endregion

        #region Action
        [HttpGet, Authorize]
        public JsonResult GetPaymentType()
        {
            try
            {
                return Json(new SelectList(_enumService.PaymentType(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }
        public JsonResult TransactionType()
        {
            try
            {
                return Json(new SelectList(_enumService.TransactionType(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult BankTransectionType()
        {
            try
            {
                return Json(new SelectList(_enumService.BankTransectionType(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult CardType()
        {
            try
            {
                return Json(new SelectList(_enumService.CardType(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult CustomerType()
        {
            try
            {
                return Json(new SelectList(_enumService.CustomerType(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
