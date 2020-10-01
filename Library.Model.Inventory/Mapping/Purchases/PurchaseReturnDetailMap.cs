using Library.Model.Inventory.Purchases;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Purchases
{
    public class PurchaseReturnDetailMap : EntityTypeConfiguration<PurchaseReturnDetail>
    {
        public PurchaseReturnDetailMap()
        {
            ToTable("PurchaseReturnDetail", "dbo");
        }
    }
}
