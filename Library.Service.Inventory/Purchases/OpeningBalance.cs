using Library.Context;
using Library.Context.Pos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service.Inventory.Purchases
{
    public class OpeningBalance : IOpeningBalance
    {
        //private IPurchaseRepository _purchaseRepository;
        public OpeningBalance()
        {
           // this._purchaseRepository = new PurchaseRepository(new PosContext());
        }
        public IEnumerable<OpeningBalance> GetAll()
        {
            throw new NotImplementedException();
        }

        public OpeningBalance GetById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
