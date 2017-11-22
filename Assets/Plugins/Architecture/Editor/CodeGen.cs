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

public class CodeGen
{
    public static string path = "Scripts/SO/";

    private static ArchitectureSettings settings;

    [MenuItem("Tools/Generate")]
    public static void Generate()
    {
        settings = ArchitectureSettings.GetSettings();
        GenerateNewVariables();
        GenerateNewReferences();
        GenerateReferenceDrawers();
    }

    [MenuItem("Tools/Generate Variables")]
    public static void GenerateNewVariables()
    {
        foreach (SerializableSystemType systemType in settings.variableTypes)
        {
            Type type = systemType.SystemType;
            CodeCompileUnit ccu = new CodeCompileUnit();

            CodeNamespace codeNamespace = new CodeNamespace("Architecture");
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            ccu.Namespaces.Add(codeNamespace);

            string typeName = GetTypeName(type) + "Variable";
            CodeTypeDeclaration codeType = new CodeTypeDeclaration(typeName);

            codeType.BaseTypes.Add(typeof(ScriptableObject));
            codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CreateAssetMenuAttribute))));

            CodeMemberProperty property = new CodeMemberProperty();
            property.HasGet = true;
            property.HasSet = true;
            property.Type = new CodeTypeReference(type);
            property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            property.Name = "CurrentValue";

            CodeMemberField valueField = new CodeMemberField(type, "Value");
            CodeMemberField defaultValueField = new CodeMemberField(type, "DefaultValue");
            defaultValueField.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
            valueField.Attributes = MemberAttributes.Private;

            CodeFieldReferenceExpression valueReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Value");

            property.GetStatements.Add(new CodeMethodReturnStatement(valueReference));
            property.SetStatements.Add(new CodeAssignStatement(valueReference, new CodeSnippetExpression("value")));

            CodeMemberMethod onEnableMethod = new CodeMemberMethod();
            onEnableMethod.Name = "OnEnable";
            onEnableMethod.Attributes = MemberAttributes.Private;
            CodeAssignStatement cs1 =
                new CodeAssignStatement(valueReference, new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "DefaultValue"));
            onEnableMethod.Statements.Add(cs1);

            codeType.Members.Add(property);
            codeType.Members.Add(defaultValueField);
            codeType.Members.Add(valueField);
            codeType.Members.Add(onEnableMethod);

            codeNamespace.Types.Add(codeType);

            WriteToFile(typeName, ccu);
        }
    }

    [MenuItem("Tools/Generate References")]
    public static void GenerateNewReferences()
    {
        foreach (SerializableSystemType systemType in settings.variableTypes)
        {
            Type type = systemType.SystemType;

            CodeCompileUnit ccu = new CodeCompileUnit();

            CodeNamespace codeNamespace = new CodeNamespace("Architecture");
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            ccu.Namespaces.Add(codeNamespace);

            string referenceName = GetTypeName(type) + "Reference";
            string variableName = GetTypeName(type) + "Variable";
            CodeTypeDeclaration codeType = new CodeTypeDeclaration(referenceName);

            codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));

            CodeMemberProperty property = new CodeMemberProperty();
            property.HasGet = true;
            property.HasSet = true;
            property.Type = new CodeTypeReference(type);
            property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            property.Name = "Value";

            CodeMemberField useConstant = new CodeMemberField(typeof(bool), "UseConstant");
            CodeMemberField constantValue = new CodeMemberField(type, "ConstantValue");
            CodeMemberField variableValue = new CodeMemberField(variableName, "Variable");
            useConstant.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            constantValue.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            variableValue.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            CodeFieldReferenceExpression useConstantReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "UseConstant");
            CodeFieldReferenceExpression constantValueReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ConstantValue");
            CodeFieldReferenceExpression variableValueReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Variable.CurrentValue");

            CodeConditionStatement getBody = new CodeConditionStatement(useConstantReference,
                                                                        new CodeStatement[1] {new CodeMethodReturnStatement(constantValueReference)},
                                                                        new CodeStatement[1] {new CodeMethodReturnStatement(variableValueReference)});
            property.GetStatements.Add(getBody);
            CodeConditionStatement setBody = new CodeConditionStatement(useConstantReference,
                                                                        new CodeStatement[1]
                                                                        {
                                                                            new CodeAssignStatement(constantValueReference, new CodeSnippetExpression("value"))
                                                                        },
                                                                        new CodeStatement[1]
                                                                        {
                                                                            new CodeAssignStatement(variableValueReference, new CodeSnippetExpression("value"))
                                                                        });
            property.SetStatements.Add(setBody);

            CodeConstructor emptyConstructor = new CodeConstructor();
            emptyConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            CodeConstructor typeConstructor = new CodeConstructor();
            typeConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            typeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(type, "value"));
            typeConstructor.Statements.Add(new CodeAssignStatement(constantValueReference, new CodeSnippetExpression("value")));
            typeConstructor.Statements.Add(new CodeAssignStatement(useConstantReference, new CodeSnippetExpression("true")));

            CodeSnippetTypeMember implicitMethod = new CodeSnippetTypeMember(String.Format(@"public static implicit operator {0}({1} reference)
        {{
            return reference.Value;
        }}
", GetTypeName(type).ToLower(), referenceName));

            codeType.Members.Add(emptyConstructor);
            codeType.Members.Add(typeConstructor);
            codeType.Members.Add(property);
            codeType.Members.Add(constantValue);
            codeType.Members.Add(useConstant);
            codeType.Members.Add(variableValue);
            codeType.Members.Add(implicitMethod);

            codeNamespace.Types.Add(codeType);

            WriteToFile(referenceName, ccu);
        }
    }

    public static void GenerateReferenceDrawers()
    {
        foreach (SerializableSystemType systemType in settings.variableTypes)
        {
            Type type = systemType.SystemType;

            CodeCompileUnit ccu = new CodeCompileUnit();

            CodeNamespace codeNamespace = new CodeNamespace("Architecture");
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            ccu.Namespaces.Add(codeNamespace);

            string referenceName = GetTypeName(type) + "Reference";
            string referenceDrawerName = GetTypeName(type) + "ReferenceDrawer";
            string variableName = GetTypeName(type) + "Variable";
            CodeTypeDeclaration codeType = new CodeTypeDeclaration(referenceDrawerName);

            codeType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CustomPropertyDrawer)),
                                                                       new CodeAttributeArgument(new CodeTypeOfExpression(referenceName))));

            codeType.BaseTypes.Add(typeof(ReferenceDrawer));
            
            codeNamespace.Types.Add(codeType);

            WriteToFile("Editor/" + referenceDrawerName, ccu);
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

        string directory = Path.Combine(Application.dataPath, CodeGen.path);
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

            // Generate source code using the code provider.
            provider.GenerateCodeFromCompileUnit(ccu, tw,
                                                 new CodeGeneratorOptions());

            // Close the output file.
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