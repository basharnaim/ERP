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

    public class PromotionalDiscountMappingService : Service<PromotionalDiscountMapping>, IPromotionalDiscountMappingService
    {
        #region Ctor
        private readonly IRepository<PromotionalDiscountMapping> _promotionalDiscountMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PromotionalDiscountMappingService(
            IRepository<PromotionalDiscountMapping> promotionalDiscountMappingRepository,
            IUnitOfWork unitOfWork) : base(promotionalDiscountMappingRepository)
        {
            _promotionalDiscountMappingRepository = promotionalDiscountMappingRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        public void Assign(string[] customerIdList, string promotionalDiscountId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                int promotionalDiscountMappingId = Convert.ToInt32(_promotionalDiscountMappingRepository.GetAutoNumber());
                int sequence = GetAutoSequence();
                List<PromotionalDiscountMapping> promotionalDiscountMappingList = new List<PromotionalDiscountMapping>();
                var promotionalDiscountMappingListDb = _promotionalDiscountMappingRepository.GetAll(x =>
                    customerIdList.Contains(x.CustomerId) && x.PromotionalDiscountId == promotionalDiscountId).ToList();
                foreach (string customerId in customerIdList)
                {
                    if (promotionalDiscountMappingListDb.All(x => x.CustomerId != customerId))
                    {
                        PromotionalDiscountMapping promotionalDiscountMapping = new PromotionalDiscountMapping
                        {
                            Id = promotionalDiscountMappingId.ToString(),
                            Sequence = sequence,
                            PromotionalDiscountId = promotionalDiscountId,
                            CustomerId = customerId,
                            SynchronizationType = SynchronizationType.Server.ToString(),
                            Active = true,
                            AddedBy = identity.Name,
                            AddedDate = DateTime.Now,
                            AddedFromIp = identity.IpAddress
                        };
                        promotionalDiscountMappingList.Add(promotionalDiscountMapping);
                        promotionalDiscountMappingId++;
                        sequence++;
                    }
                }
                _promotionalDiscountMappingRepository.Add(promotionalDiscountMappingList);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<PromotionalDiscountMapping> GetAll(string promotionalDiscountId)
        {
            try
            {
                return _promotionalDiscountMappingRepository.GetAll(x => !x.Archive && x.PromotionalDiscountId == promotionalDiscountId, "Customer,PromotionalDiscount").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ChangeStatus(string customerId, string promotionalDiscountId)
        {
            try
            {
                var dbdata = _promotionalDiscountMappingRepository.GetOne(x => x.CustomerId == customerId && x.PromotionalDiscountId == promotionalDiscountId);
                dbdata.Active = !dbdata.Active;
                _promotionalDiscountMappingRepository.Update(dbdata);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
