using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ProductBasicConfigurationCategoryMap : EntityTypeConfiguration<ProductBasicConfigurationCategory>
    {
        public ProductBasicConfigurationCategoryMap()
        {
            ToTable("ProductBasicConfigurationCategory", "dbo");
        }
    }
}
