using Library.Model.EMS.Employees;
using System.Collections.Generic;

namespace Library.Service.EMS.Employees
{
    public interface IEmployeeService
    {
        void Add(Employee employee);
        void Update(Employee employee);
        Employee GetById(string id);
        IEnumerable<Employee> GetAll();
        IEnumerable<object> Lists();
    }
}
