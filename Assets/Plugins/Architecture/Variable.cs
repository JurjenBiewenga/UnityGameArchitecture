using UnityEngine;

namespace Architecture
{
    public abstract class Variable<T> : BaseVariable
    {
        public bool Persistent;

        [UnityEngine.SerializeField()]
        private T DefaultValue;

        [UnityEngine.SerializeField()]
        private T Value;

        public T CurrentValue
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        private void OnEnable()
        {
            if (!this.Persistent && Application.isPlaying)
            {
                this.Value = this.DefaultValue;
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override object GetValue()
        {
            return this.Value;
        }
        
        public override void SetValue(object obj)
        {
            T castedObj = (T)obj;
            if (this.Persistent)
            {
                this.Value = castedObj;
            }
            else
            {
                DefaultValue = castedObj;
            }
        }
    }
}