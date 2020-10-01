using Library.Model.Inventory.Expenditures;
using System.Collections.Generic;

namespace Library.Service.Inventory.Expenditures
{
    /// <summary>
    /// Interface IExpenditureCategoryService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IExpenditureCategoryService
    {
        void Add(ExpenditureCategory expenditureCategory);
        
        void Update(ExpenditureCategory expenditureCategory);
        
        ExpenditureCategory GetById(string id);
        
        IEnumerable<ExpenditureCategory> GetAll();

        IEnumerable<object> Lists();

        int  GetAutoSequence();
    }
}
