using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Customers;

namespace Library.Model.Inventory.Mapping.Customers
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            ToTable("Customer", "dbo");
        }
    }
}
