using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Promotions;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Promotions
{

    public class PromotionalFreeItemMappingService : Service<PromotionalFreeItemMapping>, IPromotionalFreeItemMappingService
    {
        #region Ctor
        private readonly IRepository<PromotionalFreeItemMapping> _promotionalFreeItemMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PromotionalFreeItemMappingService(
            IRepository<PromotionalFreeItemMapping> promotionalFreeItemMappingRepository,
            IUnitOfWork unitOfWork) : base(promotionalFreeItemMappingRepository)
        {
            _promotionalFreeItemMappingRepository = promotionalFreeItemMappingRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        public void Assign(string[] customerIdList, string promotionalFreeItemId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                int promotionalFreeItemMappingId = Convert.ToInt32(_promotionalFreeItemMappingRepository.GetAutoNumber());
                int sequence = GetAutoSequence();
                List<PromotionalFreeItemMapping> promotionalDiscountMappingList = new List<PromotionalFreeItemMapping>();
                var promotionalDiscountMappingListDb = _promotionalFreeItemMappingRepository.GetAll(x =>
                    customerIdList.Contains(x.CustomerId) && x.PromotionalFreeItemId == promotionalFreeItemId).ToList();
                foreach (string customerId in customerIdList)
                {
                    if (promotionalDiscountMappingListDb.All(x => x.CustomerId != customerId))
                    {
                        PromotionalFreeItemMapping promotionalFreeItemMapping = new PromotionalFreeItemMapping
                        {
                            Id = promotionalFreeItemMappingId.ToString(),
                            Sequence = sequence,
                            PromotionalFreeItemId = promotionalFreeItemId,
                            CustomerId = customerId,
                            SynchronizationType = SynchronizationType.Server.ToString(),
                            Active = true,
                            AddedBy = identity.Name,
                            AddedDate = DateTime.Now,
                            AddedFromIp = identity.IpAddress
                        };
                        promotionalDiscountMappingList.Add(promotionalFreeItemMapping);
                        promotionalFreeItemMappingId++;
                        sequence++;
                    }
                }
                _promotionalFreeItemMappingRepository.Add(promotionalDiscountMappingList);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalFreeItemMapping> GetAll(string promotionalFreeItemId)
        {
            try
            {
                return _promotionalFreeItemMappingRepository.GetAll(x => !x.Archive && x.PromotionalFreeItemId == promotionalFreeItemId, "Customer,PromotionalFreeItem").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ChangeStatus(string customerId, string promotionalFreeItemId)
        {
            try
            {
                var dbdata = _promotionalFreeItemMappingRepository.GetOne(x => x.CustomerId == customerId && x.PromotionalFreeItemId == promotionalFreeItemId);
                dbdata.Active = !dbdata.Active;
                _promotionalFreeItemMappingRepository.Update(dbdata);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
