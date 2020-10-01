using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Crosscutting.Helper
{
    [Serializable]
    public abstract class BaseModel : IModelState
    {
        private ModelState state;

        [NotMapped]
        [Browsable(false)]
        public ModelState ModelState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        [Browsable(false)]
        public bool IsAdded
        {
            get
            {
                return state.Equals(ModelState.Added);
            }
        }

        [Browsable(false)]
        public bool IsModified
        {
            get
            {
                return state.Equals(ModelState.Modified);
            }
        }

        [Browsable(false)]
        public bool IsDeleted
        {
            get
            {
                return state.Equals(ModelState.Deleted);
            }
        }

        [Browsable(false)]
        public bool IsUnchanged
        {
            get
            {
                return state.Equals(ModelState.Unchanged);
            }
        }

        [Browsable(false)]
        public bool IsDetached
        {
            get
            {
                return state.Equals(ModelState.Detached);
            }
        }

        public void SetAdded()
        {
            state = ModelState.Added;
        }

        public void SetModified()
        {
            state = ModelState.Modified;
        }

        public void SetDeleted()
        {
            state = IsAdded ? ModelState.Detached : ModelState.Deleted;
        }

        public void SetDetached()
        {
            state = ModelState.Detached;
        }

        public void SetUnchanged()
        {
            state = ModelState.Unchanged;
        }

        public T Copy<T>()
        {
            return (T)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("Name = {0}, State = {1}", GetType().Name, state);
        }

        protected bool PropertyChanged<T>(T oldValue, T newValue)
        {
            if (!oldValue.NotEquals(newValue))
                return false;
            if (IsUnchanged)
                SetModified();
            return true;
        }

        public Dictionary<string, object> GetParameterInfo { get; set; }
    }
}