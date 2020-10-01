using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class RAMMap : EntityTypeConfiguration<RAM>
    {
        public RAMMap()
        {
            ToTable("RAM", "dbo");
        }
    }
}
