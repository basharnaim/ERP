using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class UomMap : EntityTypeConfiguration<Uom>
    {
        public UomMap()
        {
            ToTable("Uom", "dbo");
        }
    }
}
