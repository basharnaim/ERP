using Library.Model.EMS.Employees;
using System.Collections.Generic;

namespace Library.Service.EMS.Employees
{
    public interface IEmployeeTypeService
    {
        void Add(EmployeeType employeeType);
        void Update(EmployeeType employeeType);
        EmployeeType GetById(string id);
        IEnumerable<EmployeeType> GetAll();
        IEnumerable<object> Lists();
    }
}
