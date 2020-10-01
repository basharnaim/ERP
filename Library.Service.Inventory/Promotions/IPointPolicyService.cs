using System;
using System.Collections.Generic;
using Library.Model.Inventory.Promotions;

namespace Library.Service.Inventory.Promotions
{
    /// <summary>
    /// Interface IDiscountPolicyService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IPointPolicyService
    {
        void Add(PointPolicy PointPolicy);
        void Update(PointPolicy PointPolicy);
        PointPolicy GetById(string id);
        IEnumerable<PointPolicyDetail> GetAllPointPolicyDetailbyMasterId(string PointPolicyId);
        IEnumerable<PointPolicy> GetAll();
        IEnumerable<PointPolicy> GetAll(DateTime dateFrom, DateTime dateTo);
        IEnumerable<object> Lists();
    }
}
