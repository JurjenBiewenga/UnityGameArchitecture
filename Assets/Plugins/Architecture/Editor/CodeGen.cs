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

                string variableName = GetTypeName(type) + "Variable";
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

                string variableName = GetTypeName(type) + "Variable";
                string referenceName = GetTypeName(type) + "Reference";
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
                ", GetTypeName(type).ToLower(), referenceName));

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

                string referenceName = GetTypeName(type) + "Reference";
                string referenceDrawerName = GetTypeName(type) + "ReferenceDrawer";
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

                string variableName = GetTypeName(type) + "Variable";
                string variableDrawerName = GetTypeName(type) + "VariableDrawer";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(variableDrawerName);

                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CustomEditor)),
                                                                           new CodeAttributeArgument(new CodeTypeOfExpression(variableName))));

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

                string runtimeSetName = GetTypeName(type) + "RuntimeSet";
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

                string staticSetName = GetTypeName(type) + "StaticSet";
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

                string name = GetTypeName(type) + "SingleComponentReference";
                CodeTypeDeclaration codeType = new CodeTypeDeclaration(name);

                var listType = new CodeTypeReference(typeof(SingleComponentReference<>));
                listType.TypeArguments.Add(type);

                codeType.BaseTypes.Add(listType);
                codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CreateAssetMenuAttribute))));

                codeNamespace.Types.Add(codeType);

                WriteToFile(name, ccu);
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

        public static string GetTypeName(Type type)
        {
            if (type == typeof(float))
            {
                return "Float";
            }
            else if (type == typeof(int))
            {
                return "Int";
            }
            else if (type == typeof(bool))
            {
                return "Bool";
            }

            return type.Name;
        }
    }
}