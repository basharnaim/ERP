using Library.Model.Inventory.Suppliers;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Suppliers
{
    public class BrandMap : EntityTypeConfiguration<Brand>
    {
        public BrandMap()
        {
            ToTable("Brand", "dbo");
        }
    }
}
