using System;
using System.Collections.Generic;
using Library.Model.Inventory.Promotions;

namespace Library.Service.Inventory.Promotions
{
    /// <summary>
    /// Interface IDiscountPolicyService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IPromotionalDiscountService
    {
        void Add(PromotionalDiscount promotionalDiscount);
        void Update(PromotionalDiscount promotionalDiscount);
        PromotionalDiscount GetById(string id);
        IEnumerable<PromotionalDiscountDetail> GetAllPromotionalDiscountDetailbyMasterId(string promotionalDiscountId);
        IEnumerable<PromotionalDiscount> GetAll();
        IEnumerable<PromotionalDiscount> GetAll(DateTime dateFrom, DateTime dateTo);
        IEnumerable<object> Lists();
    }
}
