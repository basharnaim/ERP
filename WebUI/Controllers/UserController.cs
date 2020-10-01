using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Web.Helpers;
using System.Web.Mvc;
using Library.Model.Core.Securities;
using Library.Service.Core.Securities;
using Library.ViewModel.Core.Securities;

namespace ERP.WebUI.Controllers
{
    public class UserController : BaseController
    {
        #region Ctor
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAll(companyId, branchId)));
                }
                if (!string.IsNullOrEmpty(companyId) )
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAll(companyId)));
                }
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAll()));
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
        #endregion

        #region Create
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
                _userService.AddUserFromAdmin(AutoMapperConfiguration.mapper.Map<User>(userVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/User"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<UserViewModel>(_userService.GetById(id)));
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
                _userService.UpdateFromAdmin(AutoMapperConfiguration.mapper.Map<User>(userVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/User"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Reset PWD
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<UserViewModel>(_userService.GetById(id)));
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
                _userService.ResetPassword(AutoMapperConfiguration.mapper.Map<User>(uservm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/User"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #endregion

        #region Get Image
        public FileContentResult GetPicture(string id)
        {
            var byteArray = _userService.GetById(id)?.Image;
            return byteArray != null
                ? new FileContentResult(byteArray, MediaTypeNames.Image.Jpeg)
                : null;
        }
        #endregion
    }
}
