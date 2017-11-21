using System.Collections;
using System.Collections.Generic;
using Bodybuilder.Editor.Drawers;
using UnityEngine;

[CreateAssetMenu]
public class ArchitectureSettings : ScriptableObject
{
    public SerializableSystemType[] variableTypes;
    public SerializableSystemType[] setTypes;
    public SerializableSystemType[] eventTypes;
}
