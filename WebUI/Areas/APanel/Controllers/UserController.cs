using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Core.Securities;
using Library.Service.Core.Securities;
using Library.ViewModel.Core.Securities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index(string companyId, string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(Mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAll(companyId, branchId)));
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(Mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAll(companyId)));
                }
                return View(Mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public JsonResult GetUserList()
        {
            try
            {
                return Json(new SelectList(_userService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult GetSalesPersonLists(string branchId)
        {
            try
            {
                return Json(new SelectList(_userService.SalesPersonLists(branchId), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new UserViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Create(UserViewModel userVm)
        {
            try
            {
                if (userVm.ImageFile != null)
                {
                    var extension = Path.GetExtension(userVm.ImageFile.FileName)?.Trim().ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        var reader = new BinaryReader(userVm.ImageFile.InputStream);
                        var byteArray = reader.ReadBytes(userVm.ImageFile.ContentLength);
                        var img = new WebImage(byteArray).Resize(300, 300, false, true);
                        userVm.Image = img.GetBytes();
                    }
                    else
                        throw new Exception("Please upload .jpg, PNG, gif file only.");
                }
                _userService.AddUserFromAdmin(Mapper.Map<User>(userVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/User"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                return View(Mapper.Map<UserViewModel>(_userService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel userVm)
        {
            try
            {
                if (userVm.ImageFile != null)
                {
                    var extension = Path.GetExtension(userVm.ImageFile.FileName)?.Trim().ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        var reader = new BinaryReader(userVm.ImageFile.InputStream);
                        var byteArray = reader.ReadBytes(userVm.ImageFile.ContentLength);
                        var img = new WebImage(byteArray).Resize(300, 300, false, true);
                        userVm.Image = img.GetBytes();
                    }
                    else
                        throw new Exception("Please upload .jpg, PNG, gif file only.");
                }
                _userService.UpdateFromAdmin(Mapper.Map<User>(userVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/User"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            try
            {
                return View(Mapper.Map<UserViewModel>(_userService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult ResetPassword(UserViewModel uservm)
        {
            try
            {
                _userService.ResetPassword(Mapper.Map<User>(uservm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/User"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public FileContentResult GetPicture(string id)
        {
            var byteArray = _userService.GetById(id)?.Image;
            return byteArray != null
                ? new FileContentResult(byteArray, MediaTypeNames.Image.Jpeg)
                : null;
        }

    }
}
