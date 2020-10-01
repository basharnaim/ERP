using Library.Model.Inventory.Suppliers;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Suppliers
{
    public class SupplierMap : EntityTypeConfiguration<Supplier>
    {
        public SupplierMap()
        {
            ToTable("Supplier", "dbo");
        }
    }
}
