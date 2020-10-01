using System.Collections.Generic;
using Library.Model.Inventory.Promotions;

namespace Library.Service.Inventory.Promotions
{
    public interface IPromotionalFreeItemMappingService
    {
        void Assign(string[] customerIdList, string promotionalFreeItemId);
        IEnumerable<PromotionalFreeItemMapping> GetAll(string promotionalFreeItemId);
        void ChangeStatus(string customerId, string promotionalFreeItemId);
    }
}
