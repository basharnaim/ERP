using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Sales;
using Library.Service.Core.Core;
using Library.Context.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Sales
{
    public class SaleReturnService : Service<SaleReturn>, ISaleReturnService
    {
        #region Ctor
        private readonly IRepository<Sale> _saleRepository;
        private readonly IRepository<SaleDetail> _saleDetailRepository;
        private readonly IRepository<SaleReturn> _saleReturnRepository;
        private readonly IRepository<SaleReturnDetail> _saleReturnDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerLedger> _customerLedgerRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        public SaleReturnService(
            IRepository<SaleReturn> saleReturnRepository,
            IRepository<SaleReturnDetail> saleReturnDetailRepository,
            IRepository<Sale> saleRepository,
            IRepository<SaleDetail> saleDetailRepository,
            IRepository<Product> ProductRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerLedger> customerLedgerRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ):base(saleReturnRepository)
        {
            _saleRepository = saleRepository;
            _saleDetailRepository = saleDetailRepository;
            _saleReturnRepository = saleReturnRepository;
            _saleReturnDetailRepository = saleReturnDetailRepository;
            _productRepository = ProductRepository;
            _customerRepository = customerRepository;
            _rawSqlService = rawSqlService;
            _customerLedgerRepository = customerLedgerRepository;
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

        public void ChangeSaleStatus(string saleId)
        {
            try
            {
                var saleDetailList = _saleDetailRepository.GetAll(r => !r.Archive && r.SaleId == saleId).ToList();
                if (saleDetailList.All(r => r.IsReturned))
                {
                    var sale = _saleRepository.GetOne(saleId);
                    sale.IsFullyReturned = true;
                    _saleRepository.Update(sale);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ChangeSaleReturnStatus(string saleReturnId, string type)
        {
            try
            {
                var saleReturnDetailList = _saleReturnDetailRepository.GetAll(r => !r.Archive && r.SaleReturnId == saleReturnId).ToList();
                if (saleReturnDetailList.All(r => r.IsReturned))
                {
                    var sale = _saleReturnRepository.GetOne(saleReturnId);
                    if (type == "Return")
                    {
                        sale.IsFullyDamage = true;
                    }
                    sale.IsFullyReturned = true;
                    _saleReturnRepository.Update(sale);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(SaleReturn saleReturn, string type)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region saleReturn
                decimal totalQty = 0m;
                decimal totalAmt = 0m;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                saleReturn.Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "SaleReturn");
                saleReturn.Sequence = GetAutoSequence("SaleReturn");
                saleReturn.Active = true;
                saleReturn.SynchronizationType = SynchronizationType.Server.ToString();
                saleReturn.CompanyId = identity.CompanyId;
                saleReturn.BranchId = identity.BranchId;
                saleReturn.AddedBy = identity.Name;
                saleReturn.AddedDate = DateTime.Now;
                saleReturn.AddedFromIp = identity.IpAddress;
                #endregion

                #region saleReturn Items
                var detailId = Convert.ToInt32(GenerateAutoId(identity.CompanyId, identity.BranchId, "SaleReturnDetail"));
                if (saleReturn.SaleReturnDetails != null)
                {
                    var sqnc = GetAutoSequence("SaleReturnDetail");
                    foreach (var saleReturnDetail in saleReturn.SaleReturnDetails)
                    {
                        var productDb = _productRepository.GetOne(x => x.Id == saleReturnDetail.ProductId);
                        totalQty += saleReturnDetail.ReturnQuantity;
                        totalAmt += saleReturnDetail.ReturnAmount;
                        saleReturnDetail.Id = detailId.ToString();
                        saleReturnDetail.Sequence = sqnc;
                        saleReturnDetail.SaleReturnId = saleReturn.Id;
                        saleReturnDetail.ProductCode = productDb.Code;
                        saleReturnDetail.ProductName = productDb.Name;
                        saleReturnDetail.UOMId = productDb.UOMId;
                        saleReturnDetail.GradeId = productDb.GradeId;
                        saleReturnDetail.SizeId = productDb.SizeId;
                        saleReturnDetail.ColorId = productDb.ColorId;
                        saleReturnDetail.SupplierId = productDb.SupplierId;
                        saleReturnDetail.ProductCategoryId = productDb.ProductCategoryId;
                        saleReturnDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                        saleReturnDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;

                        SaleDetail saleDetail = _saleDetailRepository.GetOne(r => r.Id == saleReturnDetail.SaleDetailId);
                        decimal alreadyReturnQty = GetSumOfAlreadyReturnQty(saleReturnDetail.SaleId, saleReturnDetail.ProductId);
                        saleReturnDetail.RemainingQuantity = saleReturnDetail.SoldQuantity - (alreadyReturnQty + saleReturnDetail.ReturnQuantity);
                        saleDetail.IsReturned = saleDetail.Quantity == alreadyReturnQty + saleReturnDetail.ReturnQuantity;
                        if (type == "Return")
                        {
                            saleReturnDetail.IsDamage = true;
                        }
                        saleReturnDetail.IsReturned = saleDetail.IsReturned;

                        saleReturnDetail.SalesReturnDate = saleReturn.SalesReturnDate;
                        saleReturnDetail.SaleReturnDetailDate = DateTime.Now;
                        saleReturnDetail.CompanyId = identity.CompanyId;
                        saleReturnDetail.BranchId = identity.BranchId;
                        saleReturnDetail.Active = true;
                        saleReturnDetail.SynchronizationType = SynchronizationType.Server.ToString();
                        saleReturnDetail.AddedBy = identity.Name;
                        saleReturnDetail.AddedDate = DateTime.Now;
                        saleReturnDetail.AddedFromIp = identity.IpAddress;
                        _saleReturnDetailRepository.Add(saleReturnDetail);
                        _saleDetailRepository.Update(saleDetail);
                        detailId++;
                        sqnc++;
                    }
                }
                #endregion

                #region CustomerLedger
                saleReturn.TotalAmount = totalAmt;
                Customer customer = _customerRepository.GetOne(x => x.Id == saleReturn.CustomerId);
                if (saleReturn.CustomerId != null)
                {
                    CustomerLedger customerLedger = new CustomerLedger
                    {
                        Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "CustomerLedger"),
                        Sequence = GetAutoSequence("CustomerLedger"),
                        TrackingNo = GenerateTrackingNo(identity.CompanyId, identity.BranchId, "CustomerLedger"),
                        SaleId = saleReturn.SaleId,
                        CustomerId = customer.Id,
                        CustomerMobileNumber = customer.Phone1,
                        TransactionType = TransactionType.Sales.ToString(),
                        Particulars = TransactionType.Sales.ToString(),
                        TransactionDate = DateTime.Now,
                        DebitAmount = saleReturn.TotalAmount,
                        CreditAmount = 0,
                        RunningBalance=0,
                        CompanyId = identity.CompanyId,
                        BranchId = identity.BranchId,
                        Active = true,
                        SynchronizationType = SynchronizationType.Server.ToString(),
                        AddedBy = identity.Name,
                        AddedDate = DateTime.Now,
                        AddedFromIp = identity.IpAddress
                    };
                    _customerLedgerRepository.Add(customerLedger);
                }
                #endregion
                saleReturn.TotalQuantity = totalQty;
                saleReturn.TotalAmount = totalAmt;
                _saleReturnRepository.Add(saleReturn);

                _unitOfWork.SaveChanges();
                ChangeSaleStatus(saleReturn.SaleId);
                ChangeSaleReturnStatus(saleReturn.Id, type);
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer?.Id);
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

        public void Update(SaleReturn saleReturn)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdatasaleReturns = _saleReturnRepository.GetOne(x => x.Id == saleReturn.Id);
                saleReturn.SynchronizationType = dbdatasaleReturns.SynchronizationType;
                saleReturn.SalesReturnDate = dbdatasaleReturns.SalesReturnDate;
                saleReturn.CompanyId = dbdatasaleReturns.CompanyId;
                saleReturn.BranchId = dbdatasaleReturns.BranchId;
                saleReturn.AddedBy = dbdatasaleReturns.AddedBy;
                saleReturn.AddedDate = dbdatasaleReturns.AddedDate;
                saleReturn.AddedFromIp = dbdatasaleReturns.AddedFromIp;
                saleReturn.Active = dbdatasaleReturns.Active;
                saleReturn.UpdatedBy = identity.Name;
                saleReturn.UpdatedDate = DateTime.Now;
                saleReturn.UpdatedFromIp = identity.IpAddress;
                saleReturn.Active = dbdatasaleReturns.Active;
                _saleReturnRepository.Update(saleReturn);

                #region sale Products
                if (saleReturn.SaleReturnDetails != null)
                {
                    foreach (var saleReturnDetail in saleReturn.SaleReturnDetails)
                    {
                        var dbsaleReturnDetails = _saleReturnDetailRepository.GetOne(x => x.Id == saleReturnDetail.Id);
                        var productDb = _productRepository.GetOne(x => x.Id == saleReturnDetail.ProductId);
                        var saleDetail = _saleDetailRepository.GetOne(r => r.Id == saleReturnDetail.SaleDetailId);
                        decimal alreadyReturnQty = GetSumOfAlreadyReturnQty(saleReturnDetail.SaleId, saleReturnDetail.ProductId, saleReturnDetail.Id);
                        saleReturnDetail.RemainingQuantity = saleReturnDetail.ReturnQuantity - (alreadyReturnQty + saleReturnDetail.ReturnQuantity);
                        saleDetail.IsReturned = saleDetail.Quantity == alreadyReturnQty + saleReturnDetail.ReturnQuantity;
                        saleReturnDetail.ProductCategoryId = productDb.ProductCategoryId;
                        saleReturnDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                        saleReturnDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                        saleReturnDetail.SalesReturnDate = dbsaleReturnDetails.SalesReturnDate;
                        saleReturnDetail.SaleReturnDetailDate = dbsaleReturnDetails.SaleReturnDetailDate;
                        saleReturnDetail.UOMId = productDb.UOMId;
                        saleReturnDetail.ProductId = productDb.Id;
                        saleReturnDetail.GradeId = productDb.GradeId;
                        saleReturnDetail.SaleReturnId = dbsaleReturnDetails.SaleReturnId;
                        saleReturnDetail.CompanyId = dbsaleReturnDetails.CompanyId;
                        saleReturnDetail.BranchId = dbsaleReturnDetails.BranchId;
                        saleReturnDetail.AddedBy = dbsaleReturnDetails.AddedBy;
                        saleReturnDetail.AddedDate = dbsaleReturnDetails.AddedDate;
                        saleReturnDetail.AddedFromIp = dbsaleReturnDetails.AddedFromIp;
                        saleReturnDetail.SynchronizationType = dbsaleReturnDetails.SynchronizationType;
                        saleReturnDetail.UpdatedBy = identity.Name;
                        saleReturnDetail.UpdatedDate = DateTime.Now;
                        saleReturnDetail.UpdatedFromIp = identity.IpAddress;
                        saleReturnDetail.Active = dbsaleReturnDetails.Active;
                        _saleReturnDetailRepository.Update(saleReturnDetail);
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

        public SaleReturn GetById(string id)
        {
            try
            {
               return _saleReturnRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleReturn> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleReturnRepository.GetAll(r => !r.Archive && !r.IsFullyReturned  && r.CompanyId == companyId && r.BranchId == branchId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleReturn> GetAll(string companyId, string branchId)
        {
            try
            {
                List<SaleReturn> saleReturns = _saleReturnRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).GroupBy(g => new { g.SaleId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
                return saleReturns;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleReturn> GetAll(string companyId)
        {
            try
            {
                List<SaleReturn> saleReturns = _saleReturnRepository.GetAll(r => !r.Archive && r.CompanyId == companyId).GroupBy(g => new { g.SaleId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
                return saleReturns;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleReturn> GetAll()
        {
            try
            {
                List<SaleReturn> saleReturns = _saleReturnRepository.GetAll(r => !r.Archive).GroupBy(g => new { g.SaleId })
                         .Select(g => g.FirstOrDefault())
                         .ToList();
                return saleReturns;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleReturn> GetAllForReport(string id)
        {
            try
            {
                return _saleReturnRepository.GetAll(r => !r.Archive && r.Id == id, "Customer").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleReturnDetail> GetAllSaleReturnDetailbyMasterId(string masterId)
        {
            try
            {
                return _saleReturnDetailRepository.GetAll(x => !x.Archive && x.SaleReturnId == masterId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleReturnDetail> GetAllSaleDetailbyMasterIdForReport(string masterId)
        {
            try
            {
                return _saleReturnDetailRepository.GetAll(x => !x.Archive && x.SaleId == masterId, "Product").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Sale> GetAllSale()
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Sale> GetAllSale(string companyId)
        {
            try
            {
                return _saleRepository.GetAll(r => r.CompanyId == companyId).AsEnumerable();
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
        public IEnumerable<Sale> GetAllSale(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(r => r.CompanyId == companyId && r.BranchId == branchId && !r.Archive).AsEnumerable();
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
        public IEnumerable<Sale> GetAllSale(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => r.CompanyId == companyId && r.BranchId == branchId && !r.Archive && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleId"></param>
        /// <returns></returns>
        public IEnumerable<SaleDetail> GetAllSaleDetailbyMasterId(string saleId)
        {
            try
            {
                return _saleDetailRepository.GetAll(x => x.Active && x.SaleId == saleId).AsEnumerable();
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
        public decimal GetSumOfAlreadyReturnQty(string saleId, string ProductId)
        {
            try
            {
                //decimal alreadyDelivered = 0m;
                decimal alreadyReturned = 0m;
                //decimal totalMinus = 0m;
                //var anyalreadyDelivered = db.DeliveredHistories.Where(r => !r.IsArchive && r.SaleId == saleId && r.ProductId == ProductId).Any();
                //if (anyalreadyDelivered)
                //{
                //    alreadyDelivered = db.DeliveredHistories.Where(r => !r.IsArchive && r.SaleId == saleId && r.ProductId == ProductId).Sum(r => r.DeliveryQuantity);
                //}
                var anyalreadyReturned = _saleReturnDetailRepository.GetAll(r => !r.Archive && r.SaleId == saleId && r.ProductId == ProductId).Any();
                if (anyalreadyReturned)
                {
                    alreadyReturned = _saleReturnDetailRepository.GetAll(r => !r.Archive && r.SaleId == saleId && r.ProductId == ProductId).Sum(r => r.ReturnQuantity);
                }
                return alreadyReturned;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="salesId">Sales Id(Master Id)</param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="id">Delivery History Id (KEY)</param>
        /// <returns></returns>
        public decimal GetSumOfAlreadyReturnQty(string saleId, string ProductId, string id)
        {
            try
            {
                //decimal alreadyDelivered = 0m;
                decimal alreadyReturned = 0m;
                //decimal totalMinus = 0m;
                //var anyalreadyDelivered = db.DeliveredHistories.Where(r => !r.IsArchive && r.SaleId == saleId && r.ProductId == ProductId).Any();
                //if (anyalreadyDelivered)
                //{
                //    alreadyDelivered = db.DeliveredHistories.Where(r => !r.IsArchive && r.SaleId == saleId && r.ProductId == ProductId).Sum(r => r.DeliveryQuantity);
                //}
                var anyalreadyReturned = _saleReturnDetailRepository.GetAll(r => !r.Archive && r.SaleId == saleId && r.ProductId == ProductId).Any();
                if (anyalreadyReturned)
                {
                    alreadyReturned = _saleReturnDetailRepository.GetAll(r => !r.Archive && r.SaleId == saleId && r.ProductId == ProductId && r.Id != id).Sum(r => r.ReturnQuantity);
                }
                //totalMinus = alreadyDelivered + alreadyReturned;
                return alreadyReturned;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        public IEnumerable<SaleReturn> GetAllSaleReturnByInvoiceNo(string invoiceNo, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleReturnRepository.GetAll(x => !x.Archive && x.SaleId == invoiceNo && x.SalesReturnDate >= dateFrom && x.SalesReturnDate <= dateTo).AsEnumerable();
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
        public IEnumerable<SaleReturn> GetAllSaleReturnByInvoiceNo(string invoiceNo)
        {
            try
            {
                return _saleReturnRepository.GetAll(x => !x.Archive && x.SaleId == invoiceNo).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleReturnDetailId"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public decimal GetReturnQty(string saleReturnDetailId, string ProductId)
        {
            try
            {
                return _saleReturnDetailRepository.GetOne(x => x.Id == saleReturnDetailId && x.ProductId == ProductId).ReturnQuantity;
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
        public IEnumerable<SaleReturn> GetAllForReport()
        {
            try
            {
                return _saleReturnRepository.GetAll(x => !x.Archive).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleId"></param>
        /// <returns></returns>
        public IEnumerable<SaleReturnDetail> GetAllSaleReturnDetailbySaleId(string saleId)
        {
            try
            {
                return _saleReturnDetailRepository.GetAll(x => !x.Archive && x.SaleId == saleId).AsEnumerable();
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
        public IEnumerable<SaleReturn> GetAllSaleReturnForReport(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleReturnRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.SalesReturnDate >= dateFrom && r.SalesReturnDate <= dateTo,"Customer").GroupBy(g => new { g.SaleId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
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
        public IEnumerable<SaleReturn> GetAllSaleReturnForReport(string companyId, string branchId)
        {
            try
            {
                return _saleReturnRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId,"Customer").GroupBy(g => new { g.SaleId })
                        .Select(g => g.FirstOrDefault())
                        .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
