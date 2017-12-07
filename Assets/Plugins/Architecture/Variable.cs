using UnityEngine;

namespace Architecture
{
    public abstract class Variable<T> : ScriptableObject
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
            if (!this.Persistent)
            {
                this.Value = this.DefaultValue;
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}