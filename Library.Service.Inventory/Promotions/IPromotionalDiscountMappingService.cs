using System.Collections.Generic;
using Library.Model.Inventory.Promotions;

namespace Library.Service.Inventory.Promotions
{
    public interface IPromotionalDiscountMappingService
    {
        void Assign(string[] customerIdList, string promotionalDiscountId);
        IEnumerable<PromotionalDiscountMapping> GetAll(string promotionalDiscountId);
        void ChangeStatus(string customerId, string promotionalDiscountId);
    }
}
