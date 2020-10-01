using Library.Model.Inventory.Purchases;
using System.Data.Entity;

namespace Library.Context
{
    public class PosContext : DbContext
    { 
        public PosContext() : base("name=ErpdbEntities") { }
       
        public DbSet<OpeningBlance> OpeningBlances { get; set; }  
    }
}
