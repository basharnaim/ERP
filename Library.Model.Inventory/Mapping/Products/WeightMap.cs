using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class WeightMap : EntityTypeConfiguration<Weight>
    {
        public WeightMap()
        {
            ToTable("Weight", "dbo");
        }
    }
}
