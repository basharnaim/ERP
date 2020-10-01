using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Promotions;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Promotions
{

    public class PromotionalDiscountService : Service<PromotionalDiscount>, IPromotionalDiscountService
    {
        #region Ctor
        private readonly IRepository<PromotionalDiscount> _promotionalDiscountRepository;
        private readonly IRepository<PromotionalDiscountDetail> _promotionalDiscountDetailRepository;
        private readonly IRepository<Product> _itemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PromotionalDiscountService(
            IRepository<PromotionalDiscount> promotionalDiscountRepository,
            IRepository<PromotionalDiscountDetail> promotionalDiscountDetailRepository,
            IRepository<Product> itemRepository,
            IUnitOfWork unitOfWork) : base(promotionalDiscountRepository)
        {
            _promotionalDiscountRepository = promotionalDiscountRepository;
            _promotionalDiscountDetailRepository = promotionalDiscountDetailRepository;
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        public void Add(PromotionalDiscount promotionalDiscount)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region PromotionalDiscount
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                promotionalDiscount.Id = GetAutoNumber();
                promotionalDiscount.Sequence = GetAutoSequence();
                promotionalDiscount.Active = true;
                promotionalDiscount.SynchronizationType = SynchronizationType.Server.ToString();
                promotionalDiscount.AddedBy = identity.Name;
                promotionalDiscount.AddedDate = DateTime.Now;
                promotionalDiscount.AddedFromIp = identity.IpAddress;
                #endregion

                #region PromotionalDiscount Detail
                var detailId = GetAutoIntNumber("PromotionalDiscountDetail");
                var sequence = GetAutoSequence("PromotionalDiscountDetail");
                if (promotionalDiscount.PromotionalDiscountDetails != null)
                {
                    foreach (var item in promotionalDiscount.PromotionalDiscountDetails)
                    {
                        var itemDb = _itemRepository.GetOne(item.ProductId);
                        item.Id = detailId.ToString(); 
                        item.Sequence = sequence;
                        item.PromotionalDiscountId = promotionalDiscount.Id;
                        item.ProductCode = itemDb?.Code;
                        item.Active = true;
                        item.AddedBy = identity.Name;
                        item.AddedDate = DateTime.Now;
                        item.AddedFromIp = identity.IpAddress;
                        _promotionalDiscountDetailRepository.Add(item);
                        detailId++;
                        sequence++;
                    }
                }
                #endregion
                _promotionalDiscountRepository.Add(promotionalDiscount);
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

        public void Update(PromotionalDiscount promotionalDiscount)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region PromotionalDiscount
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var promotionalDiscountDb = _promotionalDiscountRepository.GetOne(x => x.Id == promotionalDiscount.Id);
                var promotionalDiscountDetailListDb = _promotionalDiscountDetailRepository.GetAll(x => x.PromotionalDiscountId == promotionalDiscount.Id).ToList();
                promotionalDiscount.Sequence = promotionalDiscountDb.Sequence;
                promotionalDiscount.SynchronizationType = promotionalDiscountDb.SynchronizationType;
                promotionalDiscount.AddedBy = promotionalDiscountDb.AddedBy;
                promotionalDiscount.AddedDate = promotionalDiscountDb.AddedDate;
                promotionalDiscount.AddedFromIp = promotionalDiscountDb.AddedFromIp;
                promotionalDiscount.UpdatedBy = identity.Name;
                promotionalDiscount.UpdatedDate = DateTime.Now;
                promotionalDiscount.UpdatedFromIp = identity.IpAddress;
                #endregion

                #region PromotionalDiscount Detail
                if (promotionalDiscount.PromotionalDiscountDetails != null)
                {
                    var detailId = GetAutoIntNumber("PromotionalDiscountDetail");
                    var sequence = GetAutoSequence("PromotionalDiscountDetail");
                    foreach (var item in promotionalDiscount.PromotionalDiscountDetails)
                    {
                        var itemDb = _itemRepository.GetOne(item.ProductId);
                        var discountPolicyDetailDb = _promotionalDiscountDetailRepository.GetOne(x => x.Id == item.Id);
                        if (discountPolicyDetailDb != null)
                        {
                            #region Update
                            item.Active = discountPolicyDetailDb.Active;
                            item.AddedBy = discountPolicyDetailDb.AddedBy;
                            item.AddedDate = discountPolicyDetailDb.AddedDate;
                            item.AddedFromIp = discountPolicyDetailDb.AddedFromIp;
                            item.ProductCode = itemDb?.Code;
                            item.UpdatedBy = identity.Name;
                            item.UpdatedDate = DateTime.Now;
                            item.UpdatedFromIp = identity.IpAddress;
                            _promotionalDiscountDetailRepository.Update(item);
                            #endregion
                        }
                        else
                        {
                            #region Create
                            item.Id = detailId.ToString(); detailId++;
                            item.Sequence = sequence; sequence++;
                            item.PromotionalDiscountId = promotionalDiscount.Id;
                            item.ProductCode = itemDb?.Code;
                            item.AddedBy = identity.Name;
                            item.AddedDate = DateTime.Now;
                            item.AddedFromIp = identity.IpAddress;
                            item.Active = true;
                            item.AddedBy = identity.Name;
                            item.AddedDate = DateTime.Now;
                            item.AddedFromIp = identity.IpAddress;
                            _promotionalDiscountDetailRepository.Add(item);
                            #endregion
                        }
                    }
                }
                if (promotionalDiscount.PromotionalDiscountDetails == null)
                {
                    foreach (var item in promotionalDiscountDetailListDb)
                    {
                        item.Archive = true;
                        _promotionalDiscountDetailRepository.Update(item);
                    }
                }
                else
                {
                    foreach (var item in promotionalDiscountDetailListDb)
                    {
                        if (promotionalDiscount.PromotionalDiscountDetails.All(r => r.Id != item.Id))
                        {
                            item.Archive = true;
                            _promotionalDiscountDetailRepository.Update(item);
                        }
                    }
                }
                #endregion
                _promotionalDiscountRepository.Update(promotionalDiscount);
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

        public PromotionalDiscount GetById(string id)
        {
            try
            {
                return _promotionalDiscountRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalDiscountDetail> GetAllPromotionalDiscountDetailbyMasterId(string promotionalDiscountId)
        {
            try
            {
                return _promotionalDiscountDetailRepository.GetAll(x => !x.Archive && x.PromotionalDiscountId == promotionalDiscountId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalDiscount> GetAll()
        {
            try
            {
                return _promotionalDiscountRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalDiscount> GetAll(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _promotionalDiscountRepository.GetAll(r => !r.Archive && r.DateFrom >= dateFrom && r.DateTo <= dateTo).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _promotionalDiscountRepository.GetAll(x => !x.Archive && x.Active).ToList().OrderBy(r => r.Sequence) select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
