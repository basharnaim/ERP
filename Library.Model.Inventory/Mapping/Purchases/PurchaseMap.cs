using Library.Model.Inventory.Purchases;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Purchases
{
    public class PurchaseMap : EntityTypeConfiguration<Purchase>
    {
        public PurchaseMap()
        {
            ToTable("Purchase", "dbo");
        }
    }
}
