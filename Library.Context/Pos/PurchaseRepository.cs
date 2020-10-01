using Library.Model.Inventory.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Context.Pos
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private PosContext _posContext;
        public PurchaseRepository(PosContext posContext)
        {
            this._posContext = posContext;
        }
        public void DeleteOpeningBlance(int openingBlanceID)
        {
            throw new NotImplementedException();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _posContext.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public OpeningBlance GetOpeningBlanceByID(int openingBlanceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpeningBlance> GetOpeningBlances()
        {
            return _posContext.OpeningBlances.ToList();
        }

        public void InsertOpeningBlance(OpeningBlance openingBlance)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void UpdateOpeningBlance(OpeningBlance openingBlance)
        {
            throw new NotImplementedException();
        }
    }
}
