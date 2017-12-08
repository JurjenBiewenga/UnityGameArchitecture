using System;
using UnityEngine;

namespace Architecture
{
    public abstract class Reference<T, X> : BaseReference where T : Variable<X>
    {
        [SerializeField]
        protected X ConstantValue;
        
        [SerializeField]
        protected T Variable;
        
        public bool UseConstant;

        public Reference() {
        }
        
        public Reference(X value) {
            this.ConstantValue = value;
            this.UseConstant = true;
        }
        
        public X Value {
            get {
                if (this.UseConstant) {
                    return this.ConstantValue;
                }
                else {
                    return this.Variable.CurrentValue;
                }
            }
            set {
                if (this.UseConstant) {
                    this.ConstantValue = value;
                }
                else {
                    this.Variable.CurrentValue = value;
                }
            }
        }

        public override string ToString()
        {
            if (UseConstant)
                return this.ConstantValue.ToString();
            else
            {
                if (this.Variable != null)
                    return this.Variable.CurrentValue.ToString();
                else
                    return base.ToString();
            }
        }
    }
}