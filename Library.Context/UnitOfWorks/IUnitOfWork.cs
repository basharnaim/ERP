using System.Data;

namespace Library.Context.UnitOfWorks
{
    /// <summary>
    /// Branch of Work interface.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Application DbContext class. 
        /// </summary>
        ErpdbEntities Context { get; }

        /// <summary>
        /// Dispose 
        /// </summary>
        /// <param name="disposing"></param>
        void Dispose(bool disposing);

        /// <summary>
        /// Begin Transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);

        /// <summary>
        /// Save changes.
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Commit Save changes.
        /// </summary>
        /// <returns></returns>
        bool Commit();

        /// <summary>
        /// Rollback Save changes.
        /// </summary>
        void Rollback();
    }
}
