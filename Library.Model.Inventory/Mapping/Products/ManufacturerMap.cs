using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ManufacturerMap : EntityTypeConfiguration<Manufacturer>
    {
        public ManufacturerMap()
        {
            ToTable("Manufacturer", "dbo");
        }
    }
}
