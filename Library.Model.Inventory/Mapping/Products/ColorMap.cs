using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ColorMap : EntityTypeConfiguration<Color>
    {
        public ColorMap()
        {
            ToTable("Color", "dbo");
        }
    }
}
