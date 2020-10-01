using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Purchases;
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Core;
using Library.Context.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Purchases
{
    /// <summary>
    /// Class purchaseReturnReturnService.
    /// <remarks>Jahangir Hossain Sheikh, 3-11-2015</remarks>
    /// </summary>
    public class PurchaseReturnService : Service<PurchaseReturn>, IPurchaseReturnService
    {
        #region Ctor
        private readonly IRepository<PurchaseReturn> _purchaseReturnRepository;
        private readonly IRepository<PurchaseReturnDetail> _purchaseReturnDetailRepository;
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IRepository<PurchaseDetail> _purchaseDetailRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<SupplierLedger> _supplierLedgerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseReturnService(
            IRepository<PurchaseReturn> purchaseReturnRepository,
            IRepository<PurchaseReturnDetail> purchaseReturnDetailRepository,
            IRepository<Purchase> purchaseRepository,
            IRepository<PurchaseDetail> purchaseDetailRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<SupplierLedger> supplierLedgerRepository,
            IRepository<Product> productRepository,
             IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
        ) : base(purchaseReturnRepository)
        {
            _purchaseReturnRepository = purchaseReturnRepository;
            _purchaseReturnDetailRepository = purchaseReturnDetailRepository;
            _purchaseRepository = purchaseRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
            _supplierRepository = supplierRepository;
            _supplierLedgerRepository = supplierLedgerRepository;
            _productRepository = productRepository;
            _rawSqlService = rawSqlService;
            _unitOfWork = unitOfWork;
        }
        #endregion


        public override string GenerateAutoId(string companyId, string branchId, string tableName)
        {
            try
            {
                return base.GenerateAutoId(companyId, branchId, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ChangePurchaseStatus(string purchaseId)
        {
            try
            {
                var purchaseDetailList = _purchaseDetailRepository.GetAll(r => !r.Archive && r.PurchaseId == purchaseId).ToList();
                if (purchaseDetailList.All(r => r.Returned))
                {
                    var purchase = _purchaseRepository.GetOne(purchaseId);
                    purchase.FullyReturned = true;
                    _purchaseRepository.Update(purchase);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ChangePurchaseReturnStatus(string purchaseReturnId)
        {
            try
            {
                var purchaseReturnDetailList = _purchaseReturnDetailRepository.GetAll(r => !r.Archive && r.PurchaseId == purchaseReturnId).ToList();
                if (purchaseReturnDetailList.All(r => r.IsReturned))
                {
                    var purchaseReturn = _purchaseReturnRepository.GetOne(purchaseReturnId);
                    purchaseReturn.IsFullyReturned = true;
                    _purchaseReturnRepository.Update(purchaseReturn);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseReturn"></param>
        public void Add(PurchaseReturn purchaseReturn)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                #region PurchaseReturn
                purchaseReturn.Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "PurchaseReturn");
                purchaseReturn.Sequence = GetAutoSequence();
                purchaseReturn.PurchaseReturnDate = DateTime.Now;
                purchaseReturn.Active = true;
                purchaseReturn.SynchronizationType = SynchronizationType.Server.ToString();
                purchaseReturn.CompanyId = identity.CompanyId;
                purchaseReturn.BranchId = identity.BranchId;
                purchaseReturn.AddedBy = identity.Name;
                purchaseReturn.AddedDate = DateTime.Now;
                purchaseReturn.AddedFromIp = identity.IpAddress;
                _purchaseReturnRepository.Add(purchaseReturn);
                #endregion

                #region PurchaseReturn Products
                if (purchaseReturn.PurchaseReturnDetails != null)
                {
                    var sqnc = GetAutoSequence("PurchaseReturnDetail");
                    int detailId = Convert.ToInt32(GenerateAutoId(purchaseReturn.CompanyId, purchaseReturn.BranchId, "PurchaseReturnDetail"));
                    foreach (var purchaseReturnDetail in purchaseReturn.PurchaseReturnDetails)
                    {
                        var productDb = _productRepository.GetOne(x => x.Id == purchaseReturnDetail.ProductId);
                        var purchaseDetail = _purchaseDetailRepository.GetOne(r => r.Id == purchaseReturnDetail.PurchaseDetailId);
                        #region Purchase return detail
                        purchaseReturnDetail.Id = detailId.ToString();
                        purchaseReturnDetail.Sequence = sqnc;
                        purchaseReturnDetail.PurchaseReturnId = purchaseReturn.Id;
                        decimal alreadyReturnQty = GetAlreadyReturnQty(purchaseReturnDetail.PurchaseId, purchaseReturnDetail.ProductId);
                        purchaseReturnDetail.RemainingQuantity = purchaseReturnDetail.PurchaseQuantity - (alreadyReturnQty + purchaseReturnDetail.ReturnQuantity);
                        purchaseDetail.Returned = purchaseDetail.Quantity == alreadyReturnQty + purchaseReturnDetail.ReturnQuantity;
                        purchaseReturnDetail.ProductCategoryId = productDb.ProductCategoryId;
                        purchaseReturnDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                        purchaseReturnDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                        purchaseReturnDetail.UOMId = productDb.UOMId;
                        purchaseReturnDetail.BrandId = productDb.BrandId;
                        purchaseReturnDetail.GradeId = productDb.GradeId;
                        purchaseReturnDetail.PurchaseReturnDetailDate = DateTime.Now;
                        purchaseReturnDetail.CompanyId = identity.CompanyId;
                        purchaseReturnDetail.BranchId = identity.BranchId;
                        purchaseReturnDetail.Active = true;
                        purchaseReturnDetail.SynchronizationType = SynchronizationType.Server.ToString();
                        purchaseReturnDetail.AddedBy = identity.Name;
                        purchaseReturnDetail.AddedDate = DateTime.Now;
                        purchaseReturnDetail.AddedFromIp = identity.IpAddress;
                        _purchaseReturnDetailRepository.Add(purchaseReturnDetail);
                        detailId++;
                        sqnc++;
                        #endregion
                    }
                }
                #endregion
                #region Supplier Ledger
                var supplier = _supplierRepository.GetOne(x => x.Id == purchaseReturn.SupplierId);
                if (!string.IsNullOrEmpty(supplier?.Id))
                {
                    var supplierLedger = new SupplierLedger
                    {
                        Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "SupplierLedger"),
                        Sequence = GetAutoSequence("SupplierLedger"),
                        CompanyId = identity.CompanyId,
                        BranchId = identity.BranchId,
                        PurchaseId = purchaseReturn.Id,
                        TrackingNo = GenerateTrackingNo(identity.CompanyId, identity.BranchId, "SupplierLedger"),
                        SupplierId = supplier.Id,
                        SupplierPhone = supplier.Phone1,
                        Particulars = TransactionType.PurchaseReturn.ToString(),
                        TransactionType = TransactionType.PurchaseReturn.ToString(),
                        TransactionDate = DateTime.Now,
                        DebitAmount = purchaseReturn.TotalAmount,
                        CreditAmount = 0,
                        RunningBalance = 0,
                        Active = true,
                        SynchronizationType = SynchronizationType.Server.ToString(),
                        AddedBy = identity.Name,
                        AddedDate = DateTime.Now,
                        AddedFromIp = identity.IpAddress
                    };
                    _supplierLedgerRepository.Add(supplierLedger);
                }
                #endregion
                _unitOfWork.SaveChanges();
                ChangePurchaseStatus(purchaseReturn.PurchaseId);
                ChangePurchaseReturnStatus(purchaseReturn.Id);
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateSupplierLedgerRunningBalance(supplier?.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseReturn"></param>
        public void Update(PurchaseReturn purchaseReturn)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdataPurchaseReturns = _purchaseReturnRepository.GetOne(x => x.Id == purchaseReturn.Id);
                purchaseReturn.SynchronizationType = dbdataPurchaseReturns.SynchronizationType;
                purchaseReturn.PurchaseReturnDate = dbdataPurchaseReturns.PurchaseReturnDate;
                purchaseReturn.CompanyId = dbdataPurchaseReturns.CompanyId;
                purchaseReturn.BranchId = dbdataPurchaseReturns.BranchId;
                purchaseReturn.AddedBy = dbdataPurchaseReturns.AddedBy;
                purchaseReturn.AddedDate = dbdataPurchaseReturns.AddedDate;
                purchaseReturn.AddedFromIp = dbdataPurchaseReturns.AddedFromIp;
                purchaseReturn.Active = dbdataPurchaseReturns.Active;
                purchaseReturn.UpdatedBy = identity.Name;
                purchaseReturn.UpdatedDate = DateTime.Now;
                purchaseReturn.UpdatedFromIp = identity.IpAddress;
                purchaseReturn.Active = dbdataPurchaseReturns.Active;
                _purchaseReturnRepository.Update(purchaseReturn);

                #region Purchase Products
                if (purchaseReturn.PurchaseReturnDetails != null)
                {
                    foreach (var purchaseReturnDetail in purchaseReturn.PurchaseReturnDetails)
                    {
                        var dbpurchaseReturnDetails = _purchaseReturnDetailRepository.GetOne(x => x.Id == purchaseReturnDetail.Id);
                        var productDb = _productRepository.GetOne(x => x.Id == purchaseReturnDetail.ProductId);
                        var purchaseDetail = _purchaseDetailRepository.GetOne(r => r.Id == purchaseReturnDetail.PurchaseDetailId);
                        decimal alreadyReturnQty = GetRemainingQty(purchaseReturnDetail.PurchaseId, purchaseReturnDetail.ProductId, purchaseReturnDetail.Id);
                        purchaseReturnDetail.RemainingQuantity = purchaseReturnDetail.PurchaseQuantity - (alreadyReturnQty + purchaseReturnDetail.ReturnQuantity);
                        purchaseDetail.Returned = purchaseDetail.Quantity == alreadyReturnQty + purchaseReturnDetail.ReturnQuantity;
                        purchaseReturnDetail.ProductCategoryId = productDb.ProductCategoryId;
                        purchaseReturnDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                        purchaseReturnDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                        purchaseReturnDetail.PurchaseReturnDate = dbpurchaseReturnDetails.PurchaseReturnDate;
                        purchaseReturnDetail.PurchaseReturnDetailDate = dbpurchaseReturnDetails.PurchaseReturnDetailDate;
                        purchaseReturnDetail.UOMId = productDb.UOMId;
                        purchaseReturnDetail.ProductId = productDb.Id;
                        purchaseReturnDetail.BrandId = productDb.BrandId;
                        purchaseReturnDetail.GradeId = productDb.GradeId;
                        purchaseReturnDetail.PurchaseReturnId = dbpurchaseReturnDetails.PurchaseReturnId;
                        purchaseReturnDetail.CompanyId = dbpurchaseReturnDetails.CompanyId;
                        purchaseReturnDetail.BranchId = dbpurchaseReturnDetails.BranchId;
                        purchaseReturnDetail.AddedBy = dbpurchaseReturnDetails.AddedBy;
                        purchaseReturnDetail.AddedDate = dbpurchaseReturnDetails.AddedDate;
                        purchaseReturnDetail.AddedFromIp = dbpurchaseReturnDetails.AddedFromIp;
                        purchaseReturnDetail.SynchronizationType = dbpurchaseReturnDetails.SynchronizationType;
                        purchaseReturnDetail.UpdatedBy = identity.Name;
                        purchaseReturnDetail.UpdatedDate = DateTime.Now;
                        purchaseReturnDetail.UpdatedFromIp = identity.IpAddress;
                        purchaseReturnDetail.Active = dbpurchaseReturnDetails.Active;
                        _purchaseReturnDetailRepository.Update(purchaseReturnDetail);
                    }
                }
                #endregion
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PurchaseReturn GetById(string id)
        {
            try
            {
                return _purchaseReturnRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseReturn> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                List<PurchaseReturn> saleReturns = _purchaseReturnRepository.GetAll(r => r.CompanyId == companyId && r.BranchId == branchId && !r.Archive && r.PurchaseDate >= dateFrom && r.PurchaseDate <= dateTo).GroupBy(g => new { g.PurchaseId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
                return saleReturns;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseReturn> GetAll(string companyId, string branchId)
        {
            try
            {
                List<PurchaseReturn> saleReturns = _purchaseReturnRepository.GetAll(r => r.CompanyId == companyId && r.BranchId == branchId && !r.Archive).GroupBy(g => new { g.PurchaseId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
                return saleReturns;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseReturn> GetAll(string companyId)
        {
            try
            {
                List<PurchaseReturn> saleReturns = _purchaseReturnRepository.GetAll(r => r.CompanyId == companyId && !r.Archive).GroupBy(g => new { g.PurchaseId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
                return saleReturns;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PurchaseReturn> GetAll()
        {
            try
            {
                List<PurchaseReturn> saleReturns = _purchaseReturnRepository.GetAll(r => !r.Archive).GroupBy(g => new { g.PurchaseId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
                return saleReturns;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseReturnDetail> GetAllPurchaseReturnDetailbyMasterId(string masterId)
        {
            try
            {
                return _purchaseReturnDetailRepository.GetAll(x => !x.Archive && x.PurchaseReturnId == masterId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="salesId"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public decimal GetAlreadyReturnQty(string purchaseId, string ProductId)
        {
            try
            {
                return _purchaseReturnDetailRepository.GetAll(r => r.PurchaseId == purchaseId && r.ProductId == ProductId).Sum(r => r.ReturnQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="salesId">Sales Id(Master Id)</param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="id">Delivery History Id (KEY)</param>
        /// <returns></returns>
        public decimal GetRemainingQty(string purchaseId, string ProductId, string id)
        {
            try
            {
                return _purchaseReturnDetailRepository.GetAll(r => r.PurchaseId == purchaseId && r.ProductId == ProductId && r.Id != id).Sum(r => r.ReturnQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseReturn> GetAllPurchaseReturnByInvoiceNo(string invoiceNo)
        {
            try
            {
                return _purchaseReturnRepository.GetAll(x => !x.Archive && x.PurchaseId == invoiceNo).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseId"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public decimal GetSumOfAlreadyReturnQty(string purchaseId, string ProductId)
        {
            try
            {
                return _purchaseReturnDetailRepository.GetAll(r => r.PurchaseId == purchaseId && r.ProductId == ProductId).Sum(r => r.ReturnQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseId"></param>
        /// <param name="ProductId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public decimal GetSumOfAlreadyReturnQty(string purchaseId, string ProductId, string id)
        {
            try
            {
                return _purchaseReturnDetailRepository.GetAll(r => r.PurchaseId == purchaseId && r.ProductId == ProductId && r.Id != id).Sum(r => r.ReturnQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PurchaseReturn> GetAllForReport()
        {
            try
            {
                return _purchaseReturnRepository.GetAll(x => !x.Archive).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseReturnDetailId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public decimal GetReturnQty(string purchaseReturnDetailId, string productId)
        {
            try
            {
                return _purchaseReturnDetailRepository.GetOne(x => x.Id == purchaseReturnDetailId && x.ProductId == productId).ReturnQuantity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PurchaseReturnDetail> GetAllPurchaseReturnDetailbyPurchaseId(string purchaseId)
        {
            try
            {
                return _purchaseReturnDetailRepository.GetAll(x => !x.Archive && x.PurchaseId == purchaseId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
