using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reference  {
        
    public bool UseConstant;

    public object ConstantValue;
        
    public object Variable;
    
    public object Value {
        get {
            if (this.UseConstant) {
                return this.ConstantValue;
            }
            else {
                return this.Variable;
            }
        }
        set {
            if (this.UseConstant) {
                this.ConstantValue = value;
            }
            else {
                this.Variable = value;
            }
        }
    }
}
