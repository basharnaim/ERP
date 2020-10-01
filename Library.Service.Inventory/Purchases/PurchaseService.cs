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
    public class PurchaseService : Service<Purchase>, IPurchaseService
    {
        #region Ctor
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IRepository<PurchaseDetail> _purchaseDetailRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<SupplierLedger> _supplierLedgerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseService(
            IRepository<Purchase> purchaseRepository,
            IRepository<PurchaseDetail> purchaseDetailRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<SupplierLedger> supplierLedgerRepository,
            IRepository<Product> productRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ) : base(purchaseRepository)
        {
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

        public void Add(Purchase purchase)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region Purchase
                var totalQty = 0m;
                var totalAmt = 0m;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                purchase.Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "Purchase");
                purchase.Sequence = GetAutoSequence("Purchase");
                if (purchase.MemoWiseDiscount <= 0)
                {
                    purchase.NetAmount = purchase.TotalAmount;
                }
                purchase.Active = true;
                purchase.SynchronizationType = SynchronizationType.Server.ToString();
                purchase.CompanyId = identity.CompanyId;
                purchase.BranchId = identity.BranchId;
                purchase.AddedBy = identity.Name;
                purchase.AddedDate = DateTime.Now;
                purchase.AddedFromIp = identity.IpAddress;


                #region Purchase Detail
                if (purchase.PurchaseDetails != null)
                {
                    var sqnc = GetAutoSequence("PurchaseDetail");
                    int detailId = Convert.ToInt32(GenerateAutoId(purchase.CompanyId, purchase.BranchId, "PurchaseDetail"));
                    foreach (var purchaseDetail in purchase.PurchaseDetails)
                    {
                        var productDb = _productRepository.GetOne(x => x.Id == purchaseDetail.ProductId);
                        if (!string.IsNullOrEmpty(productDb?.Id))
                        {
                            totalQty += purchaseDetail.Quantity;
                            totalAmt += purchaseDetail.TotalAmount;
                            purchaseDetail.Id = detailId.ToString();
                            purchaseDetail.Sequence = sqnc;
                            purchaseDetail.PurchaseId = purchase.Id;
                            purchaseDetail.PurchaseDate = purchase.PurchaseDate;
                            purchaseDetail.PurchaseDetailDate = DateTime.Now;
                            purchaseDetail.CompanyId = identity.CompanyId;
                            purchaseDetail.BranchId = identity.BranchId;
                            purchaseDetail.ProductCode = productDb.Code;
                            purchaseDetail.ProductName = productDb.Name;
                            purchaseDetail.UOMId = productDb.UOMId;
                            purchaseDetail.RAMId = productDb.RAMId;
                            purchaseDetail.ROMId = productDb.ROMId;
                            purchaseDetail.SizeId = productDb.SizeId;
                            purchaseDetail.ColorId = productDb.ColorId;
                            purchaseDetail.StyleId = productDb.StyleId;
                            purchaseDetail.GradeId = productDb.GradeId;
                            purchaseDetail.SupplierId = purchase.SupplierId;
                            purchaseDetail.BrandId = productDb.BrandId;
                            purchaseDetail.SalePrice = productDb.RetailPrice;
                            purchaseDetail.ProductCategoryId = productDb.ProductCategoryId;
                            purchaseDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                            purchaseDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                            purchaseDetail.Active = true;
                            purchaseDetail.SynchronizationType = SynchronizationType.Server.ToString();
                            purchaseDetail.AddedBy = identity.Name;
                            purchaseDetail.AddedDate = DateTime.Now;
                            purchaseDetail.AddedFromIp = identity.IpAddress;
                            _purchaseDetailRepository.Add(purchaseDetail);
                            detailId++;
                            sqnc++;
                        }
                    }
                }
                #endregion

                #region Supplier Ledger
                var supplier = _supplierRepository.GetOne(x => x.Id == purchase.SupplierId);
                if (!string.IsNullOrEmpty(supplier?.Id))
                {
                    var supplierLedger = new SupplierLedger
                    {
                        Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "SupplierLedger"),
                        Sequence = GetAutoSequence("SupplierLedger"),
                        CompanyId = identity.CompanyId,
                        BranchId = identity.BranchId,
                        PurchaseId = purchase.Id,
                        TrackingNo = GenerateTrackingNo(identity.CompanyId, identity.BranchId, "SupplierLedger"),
                        SupplierId = supplier.Id,
                        SupplierPhone = supplier.Phone1,
                        Particulars = TransactionType.Purchase.ToString(),
                        TransactionType = TransactionType.Purchase.ToString(),
                        TransactionDate = DateTime.Now,
                        DebitAmount = 0,
                        CreditAmount = purchase.NetAmount,
                        RunningBalance = purchase.NetAmount,
                        Active = true,
                        SynchronizationType = SynchronizationType.Server.ToString(),
                        AddedBy = identity.Name,
                        AddedDate = DateTime.Now,
                        AddedFromIp = identity.IpAddress
                    };
                    _supplierLedgerRepository.Add(supplierLedger);
                }
                #endregion
                purchase.TotalQuantity = totalQty;
                purchase.TotalAmount = totalAmt;
                _purchaseRepository.Add(purchase);
                #endregion
                _unitOfWork.SaveChanges();
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

        public void Update(Purchase purchase)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                var supplier = _supplierRepository.GetOne(x => x.Id == purchase.SupplierId);
                flag = true;
                #region Purchase
                var totalQty = 0m;
                var totalAmt = 0m;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var purchaseDb = _purchaseRepository.GetOne(x => x.Id == purchase.Id);
                var purchaseDetailsDb = _purchaseDetailRepository.GetAll(x => x.PurchaseId == purchase.Id).ToList();
                purchase.Active = purchaseDb.Active;
                purchase.Archive = purchaseDb.Archive;
                purchase.Sequence = purchaseDb.Sequence;
                purchase.SynchronizationType = purchaseDb.SynchronizationType;
                purchase.CompanyId = purchaseDb.CompanyId;
                purchase.BranchId = purchaseDb.BranchId;
                purchase.AddedBy = purchaseDb.AddedBy;
                purchase.AddedDate = purchaseDb.AddedDate;
                purchase.AddedFromIp = purchaseDb.AddedFromIp;
                if (purchase.MemoWiseDiscount <= 0)
                {
                    purchase.NetAmount = purchase.TotalAmount;
                }
                purchase.UpdatedBy = identity.Name;
                purchase.UpdatedDate = DateTime.Now;
                purchase.UpdatedFromIp = identity.IpAddress;
                #endregion

                #region Purchase Products
                if (purchase.PurchaseDetails != null)
                {
                    var sqnc = GetAutoSequence("PurchaseDetail");
                    int detailId = Convert.ToInt32(GenerateAutoId(purchase.CompanyId, purchase.BranchId, "PurchaseDetail"));
                    foreach (var purchaseDetail in purchase.PurchaseDetails)
                    {
                        var purchaseDetailDb = _purchaseDetailRepository.GetOne(x => x.Id == purchaseDetail.Id);
                        var productDb = _productRepository.GetOne(x => x.Id == purchaseDetail.ProductId);
                        if (purchaseDetailDb != null)
                        {
                            #region Update
                            totalQty += purchaseDetail.Quantity;
                            totalAmt += purchaseDetail.TotalAmount;
                            purchaseDetail.ProductCode = productDb.Code;
                            purchaseDetail.ProductName = productDb.Name;
                            purchaseDetail.UOMId = productDb.UOMId;
                            purchaseDetail.RAMId = productDb.RAMId;
                            purchaseDetail.ROMId = productDb.ROMId;
                            purchaseDetail.SizeId = productDb.SizeId;
                            purchaseDetail.GradeId = productDb.GradeId;
                            purchaseDetail.ColorId = productDb.ColorId;
                            purchaseDetail.StyleId = productDb.StyleId;
                            purchaseDetail.SupplierId = productDb.SupplierId;
                            purchaseDetail.BrandId = productDb.BrandId;
                            purchaseDetail.SalePrice = productDb.RetailPrice;
                            purchaseDetail.ProductCategoryId = purchaseDetailDb.ProductCategoryId;
                            purchaseDetail.ProductSubCategoryId = purchaseDetailDb.ProductSubCategoryId;
                            purchaseDetail.ProductSubsidiaryCategoryId = purchaseDetailDb.ProductSubsidiaryCategoryId;
                            purchaseDetail.Sequence = purchaseDetailDb.Sequence;
                            purchaseDetail.PurchaseDate = purchaseDetailDb.PurchaseDate;
                            purchaseDetail.PurchaseDetailDate = purchaseDetailDb.PurchaseDetailDate;
                            purchaseDetail.Active = purchaseDetailDb.Active;
                            purchaseDetail.Archive = purchaseDetailDb.Archive;
                            purchaseDetail.PurchaseId = purchaseDetailDb.PurchaseId;
                            purchaseDetail.CompanyId = purchaseDetailDb.CompanyId;
                            purchaseDetail.BranchId = purchaseDetailDb.BranchId;
                            purchaseDetail.SynchronizationType = purchaseDetailDb.SynchronizationType;
                            purchaseDetail.AddedBy = purchaseDetailDb.AddedBy;
                            purchaseDetail.AddedDate = purchaseDetailDb.AddedDate;
                            purchaseDetail.AddedFromIp = purchaseDetailDb.AddedFromIp;
                            purchaseDetail.UpdatedBy = identity.Name;
                            purchaseDetail.UpdatedDate = DateTime.Now;
                            purchaseDetail.UpdatedFromIp = identity.IpAddress;
                            _purchaseDetailRepository.Update(purchaseDetail);
                            #endregion
                        }
                        else
                        {
                            #region Create
                            totalQty += purchaseDetail.Quantity;
                            totalAmt += purchaseDetail.TotalAmount;
                            purchaseDetail.Id = detailId.ToString();
                            purchaseDetail.Sequence = sqnc;
                            purchaseDetail.PurchaseId = purchase.Id;
                            purchaseDetail.PurchaseDate = purchase.PurchaseDate;
                            purchaseDetail.PurchaseDetailDate = DateTime.Now;
                            purchaseDetail.CompanyId = identity.CompanyId;
                            purchaseDetail.BranchId = identity.BranchId;
                            purchaseDetail.ProductCode = productDb.Code;
                            purchaseDetail.ProductName = productDb.Name;
                            purchaseDetail.UOMId = productDb.UOMId;
                            purchaseDetail.RAMId = productDb.RAMId;
                            purchaseDetail.ROMId = productDb.ROMId;
                            purchaseDetail.SizeId = productDb.SizeId;
                            purchaseDetail.ColorId = productDb.ColorId;
                            purchaseDetail.StyleId = productDb.StyleId;
                            purchaseDetail.GradeId = productDb.GradeId;
                            purchaseDetail.SupplierId = purchase.SupplierId;
                            purchaseDetail.BrandId = productDb.BrandId;
                            purchaseDetail.SalePrice = productDb.RetailPrice;
                            purchaseDetail.ProductCategoryId = productDb.ProductCategoryId;
                            purchaseDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                            purchaseDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                            purchaseDetail.Active = true;
                            purchaseDetail.SynchronizationType = SynchronizationType.Server.ToString();
                            purchaseDetail.AddedBy = identity.Name;
                            purchaseDetail.AddedDate = DateTime.Now;
                            purchaseDetail.AddedFromIp = identity.IpAddress;
                            _purchaseDetailRepository.Add(purchaseDetail);
                            detailId++;
                            sqnc++;
                            #endregion
                        }
                    }
                }
                if (purchase.PurchaseDetails == null)
                {
                    foreach (var purchaseDetail in purchaseDetailsDb)
                    {
                        _purchaseDetailRepository.Delete(purchaseDetail.Id);
                    }
                }
                else
                {
                    foreach (var purchaseDetail in purchaseDetailsDb)
                    {
                        if (purchase.PurchaseDetails.All(r => r.Id != purchaseDetail.Id))
                        {
                            _purchaseDetailRepository.Delete(purchaseDetail.Id);
                        }
                    }
                }
                #endregion

                #region Supplier Ledger
                var supplierLedger = _supplierLedgerRepository.GetOne(x => x.SupplierId == purchase.SupplierId && x.PurchaseId == purchase.Id);
                if (supplierLedger != null)
                {
                    supplierLedger.DebitAmount = 0;
                    supplierLedger.CreditAmount = purchase.NetAmount;
                    supplierLedger.RunningBalance = purchase.NetAmount;
                    supplierLedger.UpdatedBy = identity.Name;
                    supplierLedger.UpdatedDate = DateTime.Now;
                    supplierLedger.UpdatedFromIp = identity.IpAddress;
                    _supplierLedgerRepository.Update(supplierLedger);
                }
                #endregion
                purchase.TotalQuantity = totalQty;
                purchase.TotalAmount = totalAmt;
                _purchaseRepository.Update(purchase);
                _unitOfWork.SaveChanges();
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
        /// <param name="id"></param>
        /// <returns></returns>
        public Purchase GetById(string id)
        {
            try
            {
                return _purchaseRepository.GetOne(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAllForReport(string id)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && r.Id == id, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PurchaseDetail> GetAllPurchaseDetailbyMasterIdForReport(string purchaseId)
        {
            try
            {
                return _purchaseDetailRepository.GetAll(x => !x.Archive && x.PurchaseId == purchaseId).AsEnumerable();
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
        public IEnumerable<Purchase> GetAll()
        {
            try
            {
                return _purchaseRepository.GetAll(x => !x.Archive).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAllBySupplier(string supplierId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.SupplierId == supplierId, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
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
        public IEnumerable<Purchase> GetAllByCompany(string companyId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => r.CompanyId == companyId && !r.Archive).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
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
        public IEnumerable<Purchase> GetAll(string companyId, string branchId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAll(string companyId, string branchId, string supplierId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.SupplierId == supplierId, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
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
        /// <returns></returns>
        public IEnumerable<PurchaseDetail> GetAllPurchaseDetailbyMasterId(string purchaseId)
        {
            try
            {
                return _purchaseDetailRepository.GetAll(x => !x.Archive && !x.Returned && x.PurchaseId == purchaseId, "UOM").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.PurchaseDate >= dateFrom && r.PurchaseDate <= dateTo).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo, string supplierId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.PurchaseDate >= dateFrom && r.PurchaseDate <= dateTo && r.SupplierId == supplierId, "Supplier").OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
