using System.Data.Entity.ModelConfiguration;
using Library.Model.EMS.Employees;

namespace Library.Model.EMS.Mapping.Employees
{
    public class EmployeeTypeMap : EntityTypeConfiguration<EmployeeType>
    {
        public EmployeeTypeMap()
        {
            ToTable("EmployeeType", "dbo");
        }
    }
}
