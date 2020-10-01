using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class CurrencyMap : EntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            ToTable("Currency", "dbo");
        }
    }
}
