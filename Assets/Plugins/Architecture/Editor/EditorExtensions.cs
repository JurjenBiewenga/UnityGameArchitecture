using UnityEditor;
using UnityEngine;

namespace Architecture
{
	public static class EditorExtensions  {

		[MenuItem("CONTEXT/MonoBehaviour/Generate empty reference fields", false)]
		public static void FillEmptyReferences(MenuCommand menuCommand)
		{
			Object target = menuCommand.context;
			SerializedObject serializedTarget = new SerializedObject(target);
			SerializedProperty it = serializedTarget.GetIterator();
			do
			{
				string type = Utils.GetPropertyType(it);
				if (type.EndsWith("Reference"))
				{
					Component go = (it.serializedObject.targetObject as Component);
					string path = "";
					if (go != null)
					{
						if (go.gameObject.scene.IsValid())
						{
							path = string.Format(@"Scriptable Objects\{0}\{1}\{2}.asset", go.gameObject.scene.name, it.serializedObject.targetObject.name,
							                     it.displayName);
						}
						else
						{
							path = string.Format(@"Scriptable Objects\{0}\{1}\{2}.asset", "Prefabs", it.serializedObject.targetObject.name,
							                     it.displayName);
						}

						SerializedProperty useConstantProperty = it.FindPropertyRelative("UseConstant");
						SerializedProperty variableProperty = it.FindPropertyRelative("Variable");

						Utils.GenerateAndSetVariable(useConstantProperty, variableProperty, path);
					}
				}
			} while (it.Next(true));
		}
	}
}
