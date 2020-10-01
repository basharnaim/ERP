using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class VatCategoryMap : EntityTypeConfiguration<VatCategory>
    {
        public VatCategoryMap()
        {
            ToTable("VatCategory", "dbo");
        }
    }
}
