using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ROMMap : EntityTypeConfiguration<ROM>
    {
        public ROMMap()
        {
            ToTable("ROM", "dbo");
        }
    }
}
