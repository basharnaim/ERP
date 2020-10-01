using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class FlavorMap : EntityTypeConfiguration<Flavor>
    {
        public FlavorMap()
        {
            ToTable("Flavor", "dbo");
        }
    }
}
