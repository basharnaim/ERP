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

namespace ERP.WebUI.Areas.CPanel.Controllers
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
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAllForCPanel()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public JsonResult GetUserList()
        {
            return Json(new SelectList(_userService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
        public JavaScriptResult Create(UserViewModel uservm)
        {
            try
            {
                uservm.SysAdmin = true;
                if (uservm.ImageFile != null)
                {
                    var extension = Path.GetExtension(uservm.ImageFile.FileName)?.Trim().ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        var reader = new BinaryReader(uservm.ImageFile.InputStream);
                        var byteArray = reader.ReadBytes(uservm.ImageFile.ContentLength);
                        var img = new WebImage(byteArray).Resize(300, 300, false, true);
                        uservm.Image = img.GetBytes();
                    }
                    else
                        throw new Exception("Please upload .jpg, PNG, gif file only.");
                }
                _userService.AddUserFromCPanel(AutoMapperConfiguration.mapper.Map<User>(uservm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"."}')");
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
        public JavaScriptResult Edit(UserViewModel uservm)
        {
            try
            {
                uservm.SysAdmin = true;
                if (uservm.ImageFile != null)
                {
                    var extension = Path.GetExtension(uservm.ImageFile.FileName)?.Trim().ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        var reader = new BinaryReader(uservm.ImageFile.InputStream);
                        var byteArray = reader.ReadBytes(uservm.ImageFile.ContentLength);
                        var img = new WebImage(byteArray).Resize(300, 300, false, true);
                        uservm.Image = img.GetBytes();
                    }
                    else
                        throw new Exception("Please upload .jpg, PNG, gif file only.");
                }
                _userService.UpdateFromCPanel(AutoMapperConfiguration.mapper.Map<User>(uservm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/CPanel/User"}')");
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
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/CPanel/User"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #region Get Image
        public FileContentResult GetPicture(string id)
        {
            var byteArray = _userService.GetById(id)?.Image;
            return byteArray != null
                ? new FileContentResult(byteArray, MediaTypeNames.Image.Jpeg)
                : null;
        }
        #endregion
        #endregion
    }
}
