using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Core.Menus;
using Library.Service.Core.Menus;
using Library.ViewModel.Core.Menus;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.CPanel.Controllers
{
    public class MenuController : BaseController
    {
        #region Ctor
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(Mapper.Map<IEnumerable<MenuViewModel>>(_menuService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                return View(Mapper.Map<MenuViewModel>(_menuService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Edit(MenuViewModel comvm)
        {
            try
            {
                _menuService.Update(Mapper.Map<Menu>(comvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/CPanel/Menu"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
