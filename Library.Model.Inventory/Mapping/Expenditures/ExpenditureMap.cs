using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Expenditures;

namespace Library.Model.Inventory.Mapping.Expenditures
{
    public class ExpenditureMap : EntityTypeConfiguration<Expenditure>
    {
        public ExpenditureMap()
        {
            ToTable("Expenditure", "dbo");
        }
    }
}
