using Library.Model.Inventory.Expenditures;
using System.Collections.Generic;

namespace Library.Service.Inventory.Expenditures
{
    /// <summary>
    /// Interface IexpenditureSubsidiaryCategoryService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IExpenditureSubsidiaryCategoryService
    {
        void Add(ExpenditureSubsidiaryCategory expenditureSubsidiaryCategory);
        
        void Update(ExpenditureSubsidiaryCategory expenditureSubsidiaryCategory);
        
        ExpenditureSubsidiaryCategory GetById(string id);
        
        IEnumerable<ExpenditureSubsidiaryCategory> GetAll();
        
        IEnumerable<ExpenditureSubsidiaryCategory> GetAll(string expenditureCategoryId);

        IEnumerable<ExpenditureSubsidiaryCategory> GetAll(string expenditureCategoryId, string expenditureSubcategoryId);
        
        IEnumerable<object> Lists(string expenditureSubCategoryId);

        int GetAutoSequence();
    }
}
