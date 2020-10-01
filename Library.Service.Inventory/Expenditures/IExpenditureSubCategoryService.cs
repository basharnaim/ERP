using System.Collections.Generic;
using Library.Model.Inventory.Expenditures;

namespace Library.Service.Inventory.Expenditures
{
    /// <summary>
    /// Interface IexpenditureSubCategoryService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IExpenditureSubCategoryService
    {
        void Add(ExpenditureSubCategory expenditureSubCategory);
        
        void Update(ExpenditureSubCategory expenditureSubCategory);
        
        ExpenditureSubCategory GetById(string id);
        
        IEnumerable<ExpenditureSubCategory> GetAll();

        IEnumerable<ExpenditureSubCategory> GetAll(string expenditureCategoryId);

        IEnumerable<object> Lists(string expenditureCategoryId);

        int GetAutoSequence();
    }
}
