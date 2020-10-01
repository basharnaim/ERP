using System.Data.Entity.ModelConfiguration;
using Library.Model.EMS.Employees;

namespace Library.Model.EMS.Mapping.Employees
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            ToTable("Employee", "dbo");
        }
    }
}
