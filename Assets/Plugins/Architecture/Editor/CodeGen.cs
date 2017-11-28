using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
    public class CodeGen
    {
        private static ArchitectureSettings settings;

        [MenuItem("Tools/Generate")]
        public static void Generate()
        {
            settings = ArchitectureSettings.GetSettings();
            GenerateNewVariables();
            GenerateNewReferences();
            GenerateReferenceDrawers();
            GenerateVariableDrawers();
            GenerateRuntimeSets();
            GenerateStaticSets();
            GenerateSingleComponentReferences();
            GenerateEvents();
            GenerateGameEventListeners();
            GenerateGameEvents();
            GenerateEventDrawers();
            GenerateGameEventDrawers();
        }

        public static void GenerateNewVariables()
        {
            foreach (SerializableSystemType systemType in settings.variableTypes)
            {
                if (systemType == null)
                    continue;

                Type type = systemType.SystemType;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string variableName = Utils.GetTypeName(type) + "Variable";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(variableName);

                var listType = new CodeTypeReference(typeof(Variable<>));
                listType.TypeArguments.Add(type);

                codeType.BaseTypes.Add(listType);
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CreateAssetMenuAttribute))));

                codeNamespace.Types.Add(codeType);

                WriteToFile(variableName, ccu);
            }
        }

        public static void GenerateNewReferences()
        {
            foreach (SerializableSystemType systemType in settings.variableTypes)
            {
                if (systemType == null)
                    continue;

                Type type = systemType.SystemType;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string variableName = Utils.GetTypeName(type) + "Variable";
                string referenceName = Utils.GetTypeName(type) + "Reference";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(referenceName);

                var listType = new CodeTypeReference(typeof(Reference<,>));
                listType.TypeArguments.Add(variableName);
                listType.TypeArguments.Add(type);

                codeType.BaseTypes.Add(listType);
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.SerializableAttribute))));

                CodeSnippetTypeMember implicitMethod = new CodeSnippetTypeMember(String.Format(@"public static implicit operator {0}({1} reference)
                        {{
                            if(reference.UseConstant)
                            {{
                                return reference.ConstantValue;
                            }}
                            return reference.Value;
                        }}
                ", Utils.GetTypeName(type).ToLower(), referenceName));

                codeType.Members.Add(implicitMethod);

                codeNamespace.Types.Add(codeType);

                WriteToFile(referenceName, ccu);
            }
        }

        public static void GenerateReferenceDrawers()
        {
            foreach (SerializableSystemType systemType in settings.variableTypes)
            {
                if (systemType == null)
                    continue;

                Type type = systemType.SystemType;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string referenceName = Utils.GetTypeName(type) + "Reference";
                string referenceDrawerName = Utils.GetTypeName(type) + "ReferenceDrawer";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(referenceDrawerName);

                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CustomPropertyDrawer)),
                                                                           new CodeAttributeArgument(new CodeTypeOfExpression(referenceName))));

                codeType.BaseTypes.Add(typeof(ReferenceDrawer));

                codeNamespace.Types.Add(codeType);

                WriteToFile("Editor/" + referenceDrawerName, ccu);
            }
        }

        public static void GenerateVariableDrawers()
        {
            foreach (SerializableSystemType systemType in settings.variableTypes)
            {
                if (systemType == null)
                    continue;

                Type type = systemType.SystemType;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string variableName = Utils.GetTypeName(type) + "Variable";
                string variableDrawerName = Utils.GetTypeName(type) + "VariableDrawer";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(variableDrawerName);

                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CustomEditor)),
                                                                           new CodeAttributeArgument(new CodeTypeOfExpression(variableName))));
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CanEditMultipleObjects))));

                codeType.BaseTypes.Add(typeof(VariableDrawer));

                codeNamespace.Types.Add(codeType);

                WriteToFile("Editor/" + variableDrawerName, ccu);
            }
        }

        public static void GenerateRuntimeSets()
        {
            foreach (SerializableSystemType systemType in settings.runtimeSetTypes)
            {
                if (systemType == null)
                    continue;

                Type type = systemType.SystemType;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string runtimeSetName = Utils.GetTypeName(type) + "RuntimeSet";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(runtimeSetName);

                var listType = new CodeTypeReference(typeof(RuntimeSet<>));
                listType.TypeArguments.Add(type);

                codeType.BaseTypes.Add(listType);
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CreateAssetMenuAttribute))));

                codeNamespace.Types.Add(codeType);

                WriteToFile(runtimeSetName, ccu);
            }
        }

        public static void GenerateStaticSets()
        {
            foreach (SerializableSystemType systemType in settings.staticSetTypes)
            {
                if (systemType == null)
                    continue;

                Type type = systemType.SystemType;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string staticSetName = Utils.GetTypeName(type) + "StaticSet";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(staticSetName);

                var listType = new CodeTypeReference(typeof(StaticSet<>));
                listType.TypeArguments.Add(type);

                codeType.BaseTypes.Add(listType);
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CreateAssetMenuAttribute))));

                codeNamespace.Types.Add(codeType);

                WriteToFile(staticSetName, ccu);
            }
        }

        public static void GenerateSingleComponentReferences()
        {
            foreach (SerializableSystemType systemType in settings.singleComponentReferenceTypes)
            {
                if (systemType == null)
                    continue;

                Type type = systemType.SystemType;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string name = Utils.GetTypeName(type) + "SingleComponentReference";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(name);

                var listType = new CodeTypeReference(typeof(SingleComponentReference<>));
                listType.TypeArguments.Add(type);

                codeType.BaseTypes.Add(listType);
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CreateAssetMenuAttribute))));

                codeNamespace.Types.Add(codeType);

                WriteToFile(name, ccu);
            }
        }

        public static void GenerateEvents()
        {
            foreach (EventSettings eventSettings in settings.eventTypes)
            {
                if (eventSettings.parameters.Length == 0)
                    continue;


                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);
                string name = eventSettings.name;
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(name);
                CodeTypeReference listType = null;
                if (eventSettings.parameters.Length == 1)
                    listType = new CodeTypeReference(typeof(Event<>));
                else if (eventSettings.parameters.Length == 2)
                    listType = new CodeTypeReference(typeof(Event<,>));
                else if (eventSettings.parameters.Length == 3)
                    listType = new CodeTypeReference(typeof(Event<,,>));
                else
                    listType = new CodeTypeReference(typeof(Event<,,,>));

                for (var i = 0; i < eventSettings.parameters.Length; i++)
                {
                    listType.TypeArguments.Add(eventSettings.parameters[i].SystemType);
                }

                codeType.BaseTypes.Add(listType);
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.SerializableAttribute))));


                codeNamespace.Types.Add(codeType);

                WriteToFile(name, ccu);
            }
        }

        public static void GenerateGameEvents()
        {
            foreach (EventSettings eventSettings in settings.eventTypes)
            {
                if (eventSettings.parameters.Length == 0)
                    continue;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);
                string name = eventSettings.name + "GameEvent";
                string listenerName = eventSettings.name + "GameEventListener";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(name);

                CodeMemberMethod invokeMethod = new CodeMemberMethod();
                invokeMethod.Name = "Invoke";
                string paramString = "";
                CodeVariableReferenceExpression[] parameterReferences= new CodeVariableReferenceExpression[eventSettings.parameters.Length]; 
                for (var i = 0; i < eventSettings.parameters.Length; i++)
                {
                    CodeMemberField testingField = new CodeMemberField(eventSettings.parameters[i].SystemType, "testVariable" + i);
                    testingField.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
                    codeType.Members.Add(testingField);
                    parameterReferences[i] = new CodeVariableReferenceExpression("testVariable" + i);
                    invokeMethod.Parameters.Add(new CodeParameterDeclarationExpression(eventSettings.parameters[i].SystemType, "param" + i));
                    paramString += "param" + i + ((i != eventSettings.parameters.Length - 1) ? ", " : "");
                }

                CodeTypeReference listType = new CodeTypeReference(typeof(List<>));
                listType.TypeArguments.Add(new CodeTypeReference(listenerName));

                CodeMemberField codeMemberField = new CodeMemberField(listType, "_listeners");
                codeMemberField.InitExpression = new CodeSnippetExpression("new List<" + listenerName + ">()");
                codeType.Members.Add(codeMemberField);
                CodeIterationStatement forLoop = new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("i"),
                                                                                                    new CodeSnippetExpression("_listeners.Count - 1")),
                                                                            new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
                                                                                                             CodeBinaryOperatorType.GreaterThanOrEqual,
                                                                                                             new CodePrimitiveExpression(0)),
                                                                            new CodeAssignStatement(new CodeVariableReferenceExpression("i"),
                                                                                                    new CodeBinaryOperatorExpression(
                                                                                                                                     new
                                                                                                                                         CodeVariableReferenceExpression("i"),
                                                                                                                                     CodeBinaryOperatorType
                                                                                                                                         .Subtract,
                                                                                                                                     new
                                                                                                                                         CodePrimitiveExpression(1))),
                                                                            new CodeSnippetStatement("_listeners[i].Invoke(" + paramString + ");"));
                invokeMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "i"));
                invokeMethod.Statements.Add(forLoop);
                invokeMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;
                
                CodeMemberMethod invokeWithTestingParams = new CodeMemberMethod();
                invokeWithTestingParams.Name = "InvokeWithTestingParams";
                invokeWithTestingParams.Attributes = MemberAttributes.Final | MemberAttributes.Public;
                invokeWithTestingParams.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Invoke", parameterReferences));

                CodeMemberMethod addListenerMethod = new CodeMemberMethod();
                addListenerMethod.Name = "AddListener";
                addListenerMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(listenerName), "param0"));
                addListenerMethod.Statements.Add(new CodeSnippetExpression("if(!_listeners.Contains(param0)) _listeners.Add(param0)"));
                addListenerMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;

                CodeMemberMethod removeListenerMethod = new CodeMemberMethod();
                removeListenerMethod.Name = "RemoveListener";
                removeListenerMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(listenerName), "param0"));
                removeListenerMethod.Statements.Add(new CodeSnippetExpression("_listeners.Remove(param0)"));
                removeListenerMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;

                codeType.Members.Add(invokeMethod);
                codeType.Members.Add(invokeWithTestingParams);
                codeType.Members.Add(addListenerMethod);
                codeType.Members.Add(removeListenerMethod);
                codeType.BaseTypes.Add(typeof(ScriptableObject));
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CreateAssetMenuAttribute))));

                codeNamespace.Types.Add(codeType);

                WriteToFile(name, ccu);
            }
        }

        public static void GenerateGameEventListeners()
        {
            foreach (EventSettings eventSettings in settings.eventTypes)
            {
                if (eventSettings.parameters.Length == 0)
                    continue;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);
                string eventName = eventSettings.name + "GameEvent";
                string listenerName = eventSettings.name + "GameEventListener";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(listenerName);
                codeType.BaseTypes.Add(typeof(MonoBehaviour));

                CodeTypeReference listType = null;
                if (eventSettings.parameters.Length == 1)
                    listType = new CodeTypeReference(typeof(Event<>));
                else if (eventSettings.parameters.Length == 2)
                    listType = new CodeTypeReference(typeof(Event<,>));
                else if (eventSettings.parameters.Length == 3)
                    listType = new CodeTypeReference(typeof(Event<,,>));
                else
                    listType = new CodeTypeReference(typeof(Event<,,,>));

                CodeMemberMethod invokeMethod = new CodeMemberMethod();
                invokeMethod.Name = "Invoke";
                string paramString = "";
                for (var i = 0; i < eventSettings.parameters.Length; i++)
                {
                    listType.TypeArguments.Add(eventSettings.parameters[i].SystemType);
                    invokeMethod.Parameters.Add(new CodeParameterDeclarationExpression(eventSettings.parameters[i].SystemType, "param" + i));
                    paramString += "param" + i + ((i != eventSettings.parameters.Length - 1) ? ", " : "");
                }

                invokeMethod.Statements.Add(new CodeSnippetStatement("Response.Invoke(" + paramString + ");"));
                invokeMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;

                CodeMemberField gameEventField = new CodeMemberField(new CodeTypeReference(eventName), "GameEvent");
                CodeMemberField gameEventListenerField = new CodeMemberField(eventSettings.name, "Response");
                gameEventField.Attributes = MemberAttributes.Final | MemberAttributes.Public;
                gameEventListenerField.Attributes = MemberAttributes.Final | MemberAttributes.Public;

                CodeMemberMethod onEnable = new CodeMemberMethod();
                onEnable.Name = "OnEnable";
                onEnable.Statements.Add(new CodeSnippetExpression("GameEvent.AddListener(this)"));

                CodeMemberMethod onDisable = new CodeMemberMethod();
                onDisable.Name = "OnDisable";
                onDisable.Statements.Add(new CodeSnippetExpression("GameEvent.RemoveListener(this)"));

                codeType.Members.Add(onEnable);
                codeType.Members.Add(onDisable);
                codeType.Members.Add(invokeMethod);
                codeType.Members.Add(gameEventListenerField);
                codeType.Members.Add(gameEventField);
                codeNamespace.Types.Add(codeType);

                WriteToFile(listenerName, ccu);
            }
        }

        public static void GenerateEventDrawers()
        {
            foreach (EventSettings eventSettings in settings.eventTypes)
            {
                if (eventSettings.parameters.Length == 0)
                    continue;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string drawerName = eventSettings.name + "EventDrawer";
                
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(drawerName);

                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CustomPropertyDrawer)),
                                                                           new CodeAttributeArgument(new CodeTypeOfExpression(eventSettings.name))));

                codeType.BaseTypes.Add(typeof(EventDrawer));

                codeNamespace.Types.Add(codeType);

                WriteToFile("Editor/" + drawerName, ccu);
            }
        }
        
        public static void GenerateGameEventDrawers()
        {
            foreach (EventSettings eventSettings in settings.eventTypes)
            {
                if (eventSettings.parameters.Length == 0)
                    continue;

                CodeCompileUnit ccu = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(settings.namespaceName);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                ccu.Namespaces.Add(codeNamespace);

                string drawerName = eventSettings.name + "GameEventDrawer";
                string gameEventName = eventSettings.name + "GameEvent";
                
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(drawerName);

                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CustomEditor)),
                                                                           new CodeAttributeArgument(new CodeTypeOfExpression(gameEventName))));
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CanEditMultipleObjects))));

                codeType.BaseTypes.Add(typeof(GameEventDrawer));

                codeNamespace.Types.Add(codeType);

                WriteToFile("Editor/" + drawerName, ccu);
            }
        }

        public static void WriteToFile(string fileName, CodeCompileUnit ccu)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();

            if (!fileName.EndsWith(".cs"))
            {
                if (provider.FileExtension.StartsWith("."))
                {
                    fileName += provider.FileExtension;
                }
                else
                {
                    fileName += "." + provider.FileExtension;
                }
            }

            string directory = Path.Combine(Application.dataPath, settings.sourceGenerationPath);
            string path = Path.Combine(directory, fileName);

            string finalDirectory = Path.GetDirectoryName(path);

            if (!Directory.Exists(finalDirectory))
            {
                Debug.Log(String.Format("Creating directory {0}", finalDirectory));
                Directory.CreateDirectory(finalDirectory);
            }

            using (StreamWriter sw = new StreamWriter(path, false))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

                provider.GenerateCodeFromCompileUnit(ccu, tw,
                                                     new CodeGeneratorOptions());

                tw.Close();
            }
        }
    }
}