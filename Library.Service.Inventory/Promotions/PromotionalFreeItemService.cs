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

    public class PromotionalFreeItemService : Service<PromotionalFreeItem>, IPromotionalFreeItemService
    {
        #region Ctor
        private readonly IRepository<PromotionalFreeItem> _promotionalFreeItemRepository;
        private readonly IRepository<PromotionalFreeItemDetail> _promotionalFreeItemDetailRepository;
        private readonly IRepository<Product> _itemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PromotionalFreeItemService(
            IRepository<PromotionalFreeItem> promotionalFreeItemRepository,
            IRepository<PromotionalFreeItemDetail> promotionalFreeItemDetailRepository,
            IRepository<Product> itemRepository,
            IUnitOfWork unitOfWork) : base(promotionalFreeItemRepository)
        {
            _promotionalFreeItemRepository = promotionalFreeItemRepository;
            _promotionalFreeItemDetailRepository = promotionalFreeItemDetailRepository;
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        public void Add(PromotionalFreeItem promotionalFreeItem)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region PromotionalFreeItem
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                promotionalFreeItem.Id = GetAutoNumber();
                promotionalFreeItem.Sequence = GetAutoSequence();
                promotionalFreeItem.Active = true;
                promotionalFreeItem.SynchronizationType = SynchronizationType.Server.ToString();
                promotionalFreeItem.AddedBy = identity.Name;
                promotionalFreeItem.AddedDate = DateTime.Now;
                promotionalFreeItem.AddedFromIp = identity.IpAddress;
                #endregion

                #region PromotionalFreeItem Detail
                var detailId = GetAutoIntNumber("PromotionalFreeItemDetail");
                var sequence = GetAutoSequence("PromotionalFreeItemDetail");
                if (promotionalFreeItem.PromotionalFreeItemDetails != null)
                {
                    foreach (var item in promotionalFreeItem.PromotionalFreeItemDetails)
                    {
                        var itemDb = _itemRepository.GetOne(item.ProductId);
                        item.Id = detailId.ToString(); detailId++;
                        item.Sequence = sequence; sequence++;
                        item.PromotionalFreeItemId = promotionalFreeItem.Id;
                        item.ProductCode = itemDb?.Code;
                        item.Active = true;
                        item.AddedBy = identity.Name;
                        item.AddedDate = DateTime.Now;
                        item.AddedFromIp = identity.IpAddress;
                        _promotionalFreeItemDetailRepository.Add(item);
                    }
                }
                #endregion
                _promotionalFreeItemRepository.Add(promotionalFreeItem);
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

        public void Update(PromotionalFreeItem promotionalFreeItem)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region PromotionalDiscount
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var promotionalFreeItemDb = _promotionalFreeItemRepository.GetOne(x => x.Id == promotionalFreeItem.Id);
                var promotionalFreeItemDetailListDb = _promotionalFreeItemDetailRepository.GetAll(x => x.PromotionalFreeItemId == promotionalFreeItem.Id).ToList();
                promotionalFreeItem.Sequence = promotionalFreeItemDb.Sequence;
                promotionalFreeItem.SynchronizationType = promotionalFreeItemDb.SynchronizationType;
                promotionalFreeItem.AddedBy = promotionalFreeItemDb.AddedBy;
                promotionalFreeItem.AddedDate = promotionalFreeItemDb.AddedDate;
                promotionalFreeItem.AddedFromIp = promotionalFreeItemDb.AddedFromIp;
                promotionalFreeItem.UpdatedBy = identity.Name;
                promotionalFreeItem.UpdatedDate = DateTime.Now;
                promotionalFreeItem.UpdatedFromIp = identity.IpAddress;
                #endregion

                #region PromotionalDiscount Detail
                if (promotionalFreeItem.PromotionalFreeItemDetails != null)
                {
                    var detailId = GetAutoIntNumber("PromotionalFreeItemDetail");
                    var sequence = GetAutoSequence("PromotionalFreeItemDetail");
                    foreach (var item in promotionalFreeItem.PromotionalFreeItemDetails)
                    {
                        var itemDb = _itemRepository.GetOne(item.ProductId);
                        var discountPolicyDetailDb = _promotionalFreeItemDetailRepository.GetOne(x => x.Id == item.Id);
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
                            _promotionalFreeItemDetailRepository.Update(item);
                            #endregion
                        }
                        else
                        {
                            #region Create
                            item.Id = detailId.ToString(); detailId++;
                            item.Sequence = sequence; sequence++;
                            item.PromotionalFreeItemId = promotionalFreeItem.Id;
                            item.ProductCode = itemDb?.Code;
                            item.AddedBy = identity.Name;
                            item.AddedDate = DateTime.Now;
                            item.AddedFromIp = identity.IpAddress;
                            item.Active = true;
                            item.AddedBy = identity.Name;
                            item.AddedDate = DateTime.Now;
                            item.AddedFromIp = identity.IpAddress;
                            _promotionalFreeItemDetailRepository.Add(item);
                            #endregion
                        }
                    }
                }
                if (promotionalFreeItem.PromotionalFreeItemDetails == null)
                {
                    foreach (var item in promotionalFreeItemDetailListDb)
                    {
                        item.Archive = true;
                        _promotionalFreeItemDetailRepository.Update(item);
                    }
                }
                else
                {
                    foreach (var item in promotionalFreeItemDetailListDb)
                    {
                        if (promotionalFreeItem.PromotionalFreeItemDetails.All(r => r.Id != item.Id))
                        {
                            item.Archive = true;
                            _promotionalFreeItemDetailRepository.Update(item);
                        }
                    }
                }
                #endregion
                _promotionalFreeItemRepository.Update(promotionalFreeItem);
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

        public PromotionalFreeItem GetById(string id)
        {
            try
            {
                return _promotionalFreeItemRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalFreeItemDetail> GetAllTradeOfferOnFreeItemDetailbyMasterId(string promotionalFreeItemId)
        {
            try
            {
                return _promotionalFreeItemDetailRepository.GetAll(x => !x.Archive && x.PromotionalFreeItemId == promotionalFreeItemId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalFreeItem> GetAll()
        {
            try
            {
                return _promotionalFreeItemRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalFreeItem> GetAll(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _promotionalFreeItemRepository.GetAll(r => !r.Archive && r.DateFrom >= dateFrom && r.DateTo <= dateTo).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _promotionalFreeItemRepository.GetAll(x => !x.Archive && x.Active).ToList().OrderBy(r => r.Sequence) select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
