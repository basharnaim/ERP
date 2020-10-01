using Library.Model.Inventory.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Context.Pos
{
    public interface IPurchaseRepository : IDisposable
    {
        IEnumerable<OpeningBlance> GetOpeningBlances();
        OpeningBlance GetOpeningBlanceByID(int openingBlanceId);
        void InsertOpeningBlance(OpeningBlance openingBlance); 
        void DeleteOpeningBlance(int openingBlanceID);
        void UpdateOpeningBlance(OpeningBlance openingBlance); 
        void Save();
    }

}
