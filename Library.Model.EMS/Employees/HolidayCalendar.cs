#region Using

using Library.Crosscutting.Helper;
using System;

#endregion

namespace Library.Model.EMS.Employees
{
    public class HolidayCalendar : BaseModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        #endregion

        #region Audit
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedFromIp { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedFromIp { get; set; }
        #endregion
    }
}
