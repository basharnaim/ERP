using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Promotions;

namespace Library.Model.Inventory.Mapping.Promotions
{
    public class PointPolicyDetailMap : EntityTypeConfiguration<PointPolicyDetail>
    {
        public PointPolicyDetailMap()
        {
            ToTable("PointPolicyDetail", "dbo");
        }
    }
}
