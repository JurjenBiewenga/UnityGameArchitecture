﻿namespace Architecture
{
    public abstract class Reference<T, X> where T : Variable<X>
    {
        protected X ConstantValue;
        
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
    }
}