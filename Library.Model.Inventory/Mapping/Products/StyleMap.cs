using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class StyleMap : EntityTypeConfiguration<Style>
    {
        public StyleMap()
        {
            ToTable("Style", "dbo");
        }
    }
}
