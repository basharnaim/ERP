using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service.Inventory.Purchases
{
    public interface IOpeningBalance
    {
        IEnumerable<OpeningBalance> GetAll();
        OpeningBalance GetById(string id);
    }
}
