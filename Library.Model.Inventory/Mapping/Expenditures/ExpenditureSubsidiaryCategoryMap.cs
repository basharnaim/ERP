using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Expenditures;

namespace Library.Model.Inventory.Mapping.Expenditures
{
    public class ExpenditureSubsidiaryCategoryMap : EntityTypeConfiguration<ExpenditureSubsidiaryCategory>
    {
        public ExpenditureSubsidiaryCategoryMap()
        {
            ToTable("ExpenditureSubsidiaryCategory", "dbo");
        }
    }
}
