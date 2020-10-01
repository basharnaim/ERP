using Library.Model.EMS.Employees;
using System.Collections.Generic;

namespace Library.Service.EMS.Employees
{
    public interface IEmployeeGroupService
    {
        void Add(EmployeeGroup employeeGroup);
        void Update(EmployeeGroup employeeGroup);
        EmployeeGroup GetById(string id);
        IEnumerable<EmployeeGroup> GetAll();
        IEnumerable<object> Lists();
    }
}
