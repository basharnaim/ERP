using Library.Model.Core.Organizations;
using System.Collections.Generic;

namespace Library.Service.EMS.Employees
{
    public interface IDesignationService
    {
        void Add(Designation designation);
        void Update(Designation designation);
        Designation GetById(string id);
        IEnumerable<Designation> GetAll();
        IEnumerable<object> Lists();
    }
}
