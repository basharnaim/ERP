using System;
using System.Collections.Generic;
using Library.Model.Inventory.Promotions;

namespace Library.Service.Inventory.Promotions
{
    /// <summary>
    /// Interface IDiscountPolicyService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IPromotionalFreeItemService
    {
        void Add(PromotionalFreeItem promotionalFreeItem);
        void Update(PromotionalFreeItem promotionalFreeItem);
        PromotionalFreeItem GetById(string id);
        IEnumerable<PromotionalFreeItemDetail> GetAllTradeOfferOnFreeItemDetailbyMasterId(string promotionalFreeItemId);
        IEnumerable<PromotionalFreeItem> GetAll();
        IEnumerable<PromotionalFreeItem> GetAll(DateTime dateFrom, DateTime dateTo);
        IEnumerable<object> Lists();
    }
}
