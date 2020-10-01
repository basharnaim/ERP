using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class SizeMap : EntityTypeConfiguration<Size>
    {
        public SizeMap()
        {
            ToTable("Size", "dbo");
        }
    }
}
