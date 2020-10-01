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

    public class PointPolicyService : Service<PointPolicy>, IPointPolicyService
    {
        #region Ctor
        private readonly IRepository<PointPolicy> _pointPolicyRepository;
        private readonly IRepository<PointPolicyDetail> _pointPolicyDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PointPolicyService(
            IRepository<PointPolicy> pointPolicyRepository,
            IRepository<PointPolicyDetail> pointPolicyDetailRepository,
            IUnitOfWork unitOfWork
            ) : base(pointPolicyRepository)
        {
            _pointPolicyRepository = pointPolicyRepository;
            _pointPolicyDetailRepository = pointPolicyDetailRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        public void Add(PointPolicy pointPolicy)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region PointPolicy
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                pointPolicy.Id = GetAutoNumber();
                pointPolicy.Sequence = GetAutoSequence();
                pointPolicy.Active = true;
                pointPolicy.SynchronizationType = SynchronizationType.Server.ToString();
                pointPolicy.AddedBy = identity.Name;
                pointPolicy.AddedDate = DateTime.Now;
                pointPolicy.AddedFromIp = identity.IpAddress;
                #endregion

                #region PointPolicy Detail
                var detailId = GetAutoIntNumber("PointPolicyDetail");
                var sequence = GetAutoSequence("PointPolicyDetail");
                if (pointPolicy.PointPolicyDetails != null)
                {
                    foreach (var pointPolicyDetail in pointPolicy.PointPolicyDetails)
                    {
                        pointPolicyDetail.Id = detailId.ToString(); 
                        pointPolicyDetail.Sequence = sequence; 
                        pointPolicyDetail.PointPolicyId = pointPolicy.Id;
                        pointPolicyDetail.SynchronizationType = SynchronizationType.Server.ToString();
                        pointPolicyDetail.Active = true;
                        pointPolicyDetail.AddedBy = identity.Name;
                        pointPolicyDetail.AddedDate = DateTime.Now;
                        pointPolicyDetail.AddedFromIp = identity.IpAddress;
                        _pointPolicyDetailRepository.Add(pointPolicyDetail);
                        detailId++;
                        sequence++;
                    }
                }
                #endregion
                _pointPolicyRepository.Add(pointPolicy);
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

        public void Update(PointPolicy pointPolicy)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region PointPolicy
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var pointPolicyDb = _pointPolicyRepository.GetOne(x => x.Id == pointPolicy.Id);
                var pointPolicyDetailListDb = _pointPolicyDetailRepository.GetAll(x => x.PointPolicyId == pointPolicy.Id).ToList();
                pointPolicy.Sequence = pointPolicyDb.Sequence;
                pointPolicy.SynchronizationType = pointPolicyDb.SynchronizationType;
                pointPolicy.AddedBy = pointPolicyDb.AddedBy;
                pointPolicy.AddedDate = pointPolicyDb.AddedDate;
                pointPolicy.AddedFromIp = pointPolicyDb.AddedFromIp;
                pointPolicy.UpdatedBy = identity.Name;
                pointPolicy.UpdatedDate = DateTime.Now;
                pointPolicy.UpdatedFromIp = identity.IpAddress;
                #endregion

                #region PointPolicy Detail
                if (pointPolicy.PointPolicyDetails != null)
                {
                    var detailId = GetAutoIntNumber("PointPolicyDetail");
                    var sequence = GetAutoSequence("PointPolicyDetail");
                    foreach (var pointPolicyDetail in pointPolicy.PointPolicyDetails)
                    {
                        var discountPolicyDetailDb = _pointPolicyDetailRepository.GetOne(x => x.Id == pointPolicyDetail.Id);
                        if (discountPolicyDetailDb != null)
                        {
                            #region Update
                            pointPolicyDetail.SynchronizationType = discountPolicyDetailDb.SynchronizationType;
                            pointPolicyDetail.Active = discountPolicyDetailDb.Active;
                            pointPolicyDetail.AddedBy = discountPolicyDetailDb.AddedBy;
                            pointPolicyDetail.AddedDate = discountPolicyDetailDb.AddedDate;
                            pointPolicyDetail.AddedFromIp = discountPolicyDetailDb.AddedFromIp;
                            pointPolicyDetail.UpdatedBy = identity.Name;
                            pointPolicyDetail.UpdatedDate = DateTime.Now;
                            pointPolicyDetail.UpdatedFromIp = identity.IpAddress;
                            _pointPolicyDetailRepository.Update(pointPolicyDetail);
                            #endregion
                        }
                        else
                        {
                            #region Create
                            pointPolicyDetail.Id = detailId.ToString();
                            pointPolicyDetail.Sequence = sequence; 
                            pointPolicyDetail.PointPolicyId = pointPolicy.Id;
                            pointPolicyDetail.SynchronizationType = SynchronizationType.Server.ToString();
                            pointPolicyDetail.Active = true;
                            pointPolicyDetail.AddedBy = identity.Name;
                            pointPolicyDetail.AddedDate = DateTime.Now;
                            pointPolicyDetail.AddedFromIp = identity.IpAddress;
                            _pointPolicyDetailRepository.Add(pointPolicyDetail);
                            detailId++;
                            sequence++;
                            #endregion
                        }
                    }
                }
                if (pointPolicy.PointPolicyDetails == null)
                {
                    foreach (var item in pointPolicyDetailListDb)
                    {
                        item.Archive = true;
                        _pointPolicyDetailRepository.Update(item);
                    }
                }
                else
                {
                    foreach (var item in pointPolicyDetailListDb)
                    {
                        if (pointPolicy.PointPolicyDetails.All(r => r.Id != item.Id))
                        {
                            item.Archive = true;
                            _pointPolicyDetailRepository.Update(item);
                        }
                    }
                }
                #endregion
                _pointPolicyRepository.Update(pointPolicy);
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

        public PointPolicy GetById(string id)
        {
            try
            {
                return _pointPolicyRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PointPolicyDetail> GetAllPointPolicyDetailbyMasterId(string PointPolicyId)
        {
            try
            {
                return _pointPolicyDetailRepository.GetAll(x => !x.Archive && x.PointPolicyId == PointPolicyId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PointPolicy> GetAll()
        {
            try
            {
                return _pointPolicyRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PointPolicy> GetAll(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _pointPolicyRepository.GetAll(r => !r.Archive && r.DateFrom >= dateFrom && r.DateTo <= dateTo).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _pointPolicyRepository.GetAll(x => !x.Archive && x.Active).ToList().OrderBy(r => r.Sequence) select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
