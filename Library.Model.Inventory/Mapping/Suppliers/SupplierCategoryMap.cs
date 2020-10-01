using Library.Model.Inventory.Suppliers;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Suppliers
{
    public class SupplierCategoryMap : EntityTypeConfiguration<SupplierCategory>
    {
        public SupplierCategoryMap()
        {
            ToTable("SupplierCategory", "dbo");
        }
    }
}
