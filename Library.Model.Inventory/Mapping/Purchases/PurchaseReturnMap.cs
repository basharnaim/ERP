using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Purchases;

namespace Library.Model.Inventory.Mapping.Purchases
{
    public class PurchaseReturnMap : EntityTypeConfiguration<PurchaseReturn>
    {
        public PurchaseReturnMap()
        {
            ToTable("PurchaseReturn", "dbo");
        }
    }
}
