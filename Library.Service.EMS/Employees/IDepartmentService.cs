using Library.Model.Core.Organizations;
using System.Collections.Generic;

namespace Library.Service.EMS.Employees
{
    public interface IDepartmentService
    {
        void Add(Department department);
        void Update(Department department);
        Department GetById(string id);
        IEnumerable<Department> GetAll();
        IEnumerable<object> Lists();
    }
}
