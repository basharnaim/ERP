using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class FloorMap : EntityTypeConfiguration<Floor>
    {
        public FloorMap()
        {
            ToTable("Floor", "dbo");
        }
    }
}
