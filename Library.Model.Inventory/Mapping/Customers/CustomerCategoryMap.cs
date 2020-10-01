using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Customers;

namespace Library.Model.Inventory.Mapping.Customers
{
    public class CustomerCategoryMap : EntityTypeConfiguration<CustomerCategory>
    {
        public CustomerCategoryMap()
        {
            ToTable("CustomerCategory", "dbo");
        }
    }
}
