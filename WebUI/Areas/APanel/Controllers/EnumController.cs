using System;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Service.Core.Enums;

namespace ERP.WebUI.Areas.APanel.Controllers
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult PaymentType()
        {
            try
            {
                return Json(new SelectList(_enumService.PaymentType(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
