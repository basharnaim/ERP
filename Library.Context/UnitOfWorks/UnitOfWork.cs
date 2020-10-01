using System;
using System.Data;
using System.Data.Entity;

namespace Library.Context.UnitOfWorks
{
    /// <summary>
    /// Branch of work class.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private ErpdbEntities _context;
        private DbContextTransaction _transaction;
        private bool _disposed;

        public ErpdbEntities Context
        {
            get { return _context; }
        }

        public UnitOfWork(ErpdbEntities context)
        {
            this._context = context;
        }

        /// <summary>
        /// Save Changes.
        /// </summary>
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only

                try
                {
                    if (_context != null && _context.Database.Connection.State == ConnectionState.Open)
                    {
                        _context.Database.Connection.Close();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // do nothing, the objectContext has already been disposed
                }

                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        #endregion

        #region Branch of Work Transactions

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            if (_context.Database.Connection.State != ConnectionState.Open)
            {
                _context.Database.Connection.Open();
            }
            _transaction = _context.Database.BeginTransaction(isolationLevel);
        }

        public bool Commit()
        {
            _transaction.Commit();
            return true;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        #endregion
    }
}
