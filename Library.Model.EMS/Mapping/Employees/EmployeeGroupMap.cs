using Library.Model.EMS.Employees;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.EMS.Mapping.Employees
{
    public class EmployeeGroupMap : EntityTypeConfiguration<EmployeeGroup>
    {
        public EmployeeGroupMap()
        {
            ToTable("EmployeeGroup", "dbo");
        }
    }
}
