using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Products;
using Library.Service.Core.Core;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Class GradeService.
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class GradeService : Service<Grade>, IGradeService
    {
        #region Ctor
        private readonly IRepository<Grade> _gradeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GradeService(
            IRepository<Grade> gradeRepository,
            IUnitOfWork unitOfWork) : base(gradeRepository)
        {
            _gradeRepository = gradeRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Grade model)
        {
            try
            {
                CheckUniqueColumn("Code", model.Code, r => r.Id != model.Id && r.Code == model.Code);
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(Grade grade)
        {
            try
            {
                Check(grade);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                grade.Id = GetAutoNumber();
                grade.Sequence = GetAutoSequence();
                grade.SynchronizationType = SynchronizationType.Server.ToString();
                grade.AddedBy = identity.Name;
                grade.AddedDate = DateTime.Now;
                grade.AddedFromIp = identity.IpAddress;
                _gradeRepository.Add(grade);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade"></param>
        public void Update(Grade grade)
        {
            try
            {
                Check(grade);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var gradeDb = _gradeRepository.GetOne(grade.Id);
                grade.Sequence = gradeDb.Sequence;
                grade.SynchronizationType = gradeDb.SynchronizationType;
                grade.AddedBy = gradeDb.AddedBy;
                grade.AddedDate = gradeDb.AddedDate;
                grade.AddedFromIp = gradeDb.AddedFromIp;
                grade.UpdatedBy = identity.Name;
                grade.UpdatedDate = DateTime.Now;
                grade.UpdatedFromIp = identity.IpAddress;
                _gradeRepository.Update(grade);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Grade GetById(string id)
        {
            try
            {
                return _gradeRepository.GetOne(id);
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
        public IEnumerable<Grade> GetAll()
        {
            try
            {
                return _gradeRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _gradeRepository.GetAll(r => !r.Archive && r.Active)
                                                 .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetGradeNameById(string id)
        {
            try
            {
                return _gradeRepository.GetOne(id).Name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
