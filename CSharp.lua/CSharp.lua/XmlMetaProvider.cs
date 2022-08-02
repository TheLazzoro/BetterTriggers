/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.CodeAnalysis;

namespace CSharpLua {
  public sealed partial class XmlMetaProvider {
    private static XmlMetaProvider currentXmlMetaProvider_;
    private readonly Dictionary<string, string> fieldMetadata_ = new();

    internal static string GetFieldMetadata(string fieldDocumentationId) {
      if (currentXmlMetaProvider_ is null) {
        return null;
      }
      return currentXmlMetaProvider_.fieldMetadata_.TryGetValue(fieldDocumentationId, out var value) ? value : null;
    }

    internal enum MethodMetaType {
      Name,
      CodeTemplate,
      IgnoreGeneric,
    }

    private sealed class MethodMetaInfo {
      private readonly List<XmlMetaModel.MethodModel> models_ = new();
      private bool isSingleModel_;

      public void Add(XmlMetaModel.MethodModel model) {
        models_.Add(model);
        CheckIsSingleModel();
      }

      private void CheckIsSingleModel() {
        bool isSingle = false;
        if (models_.Count == 1) {
          var model = models_.First();
          if (model.ArgCount == -1 && model.Args == null && model.RetType == null && model.GenericArgCount == -1) {
            isSingle = true;
          }
        }
        isSingleModel_ = isSingle;
      }

      private static string GetTypeString(ITypeSymbol symbol) {
        if (symbol.Kind == SymbolKind.TypeParameter) {
          return symbol.Name;
        }

        StringBuilder sb = new StringBuilder();
        INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
        var namespaceSymbol = typeSymbol.ContainingNamespace;

        if (symbol.ContainingType != null) {
          sb.Append(GetTypeString(symbol.ContainingType));
          sb.Append('.');
        } else if (!namespaceSymbol.IsGlobalNamespace) {
          sb.Append(namespaceSymbol);
          sb.Append('.');
        }
        sb.Append(symbol.Name);
        if (typeSymbol.TypeArguments.Length > 0) {
          sb.Append('`');
          sb.Append(typeSymbol.TypeArguments.Length);
        }
        return sb.ToString();
      }

      private static bool IsTypeMatch(ITypeSymbol symbol, string typeString) {
        if (symbol.Kind == SymbolKind.ArrayType) {
          var typeSymbol = (IArrayTypeSymbol)symbol;
          string elementTypeName = GetTypeString(typeSymbol.ElementType);
          return elementTypeName + "[]" == typeString;
        }

        string name = GetTypeString(symbol);
        return name == typeString;
      }

      private static bool IsArgMatch(ITypeSymbol symbol, XmlMetaModel.ArgumentModel parameterModel) {
        if (!IsTypeMatch(symbol, parameterModel.type)) {
          return false;
        }

        if (parameterModel.GenericArgs != null) {
          var typeSymbol = (INamedTypeSymbol)symbol;
          if (typeSymbol.TypeArguments.Length != parameterModel.GenericArgs.Length) {
            return false;
          }

          int index = 0;
          foreach (var typeArgument in typeSymbol.TypeArguments) {
            var genericArgModel = parameterModel.GenericArgs[index];
            if (!IsArgMatch(typeArgument, genericArgModel)) {
              return false;
            }
            ++index;
          }
        }

        return true;
      }

      private bool IsMethodMatch(XmlMetaModel.MethodModel model, IMethodSymbol symbol) {
        if (model.name != symbol.Name) {
          return false;
        }

        if (model.ArgCount != -1) {
          if (symbol.Parameters.Length != model.ArgCount) {
            return false;
          }
        }

        if (model.GenericArgCount != -1) {
          if (symbol.TypeArguments.Length != model.GenericArgCount) {
            return false;
          }
        }

        if (!string.IsNullOrEmpty(model.RetType)) {
          if (!IsTypeMatch(symbol.ReturnType, model.RetType)) {
            return false;
          }
        }

        if (model.Args != null) {
          if (symbol.Parameters.Length != model.Args.Length) {
            return false;
          }

          int index = 0;
          foreach (var parameter in symbol.Parameters) {
            var parameterModel = model.Args[index];
            if (!IsArgMatch(parameter.Type, parameterModel)) {
              return false;
            }
            ++index;
          }
        }

        return true;
      }

      private XmlMetaModel.MethodModel GetMethodModel(IMethodSymbol symbol, bool isCheckBaned) {
        var methodModel = isSingleModel_
          ? models_.First()
          : models_.Find(i => IsMethodMatch(i, symbol));
        if (methodModel != null && isCheckBaned) {
          methodModel.CheckBaned(symbol);
        }
        return methodModel;
      }

      public string GetMetaInfo(IMethodSymbol symbol, MethodMetaType type) {
        return GetMethodModel(symbol, type == MethodMetaType.CodeTemplate)?.GetMetaInfo(type);
      }
    }

    private sealed class TypeMetaInfo {
      private readonly XmlMetaModel.ClassModel model_;
      private readonly Dictionary<string, XmlMetaModel.FieldModel> fields_ = new();
      private readonly Dictionary<string, XmlMetaModel.PropertyModel> properties_ = new();
      private readonly Dictionary<string, MethodMetaInfo> methods_ = new();

      public TypeMetaInfo(XmlMetaModel.ClassModel model) {
        model_ = model;
        Field();
        Property();
        Method();
      }

      public XmlMetaModel.ClassModel Model {
        get {
          return model_;
        }
      }

      private void Field() {
        if (model_.Fields != null) {
          foreach (var fieldModel in model_.Fields) {
            if (string.IsNullOrEmpty(fieldModel.name)) {
              throw new ArgumentException($"type [{model_.name}] has a field name is empty");
            }

            if (fields_.ContainsKey(fieldModel.name)) {
              throw new ArgumentException($"type [{model_.name}]'s field [{fieldModel.name}] is already exists");
            }
            fields_.Add(fieldModel.name, fieldModel);
          }
        }
      }

      private void Property() {
        if (model_.Properties != null) {
          foreach (var propertyModel in model_.Properties) {
            if (string.IsNullOrEmpty(propertyModel.name)) {
              throw new ArgumentException($"type [{model_.name}] has a property name is empty");
            }

            if (fields_.ContainsKey(propertyModel.name)) {
              throw new ArgumentException($"type [{model_.name}]'s property [{propertyModel.name}] is already exists");
            }
            properties_.Add(propertyModel.name, propertyModel);
          }
        }
      }

      private void Method() {
        if (model_.Methods != null) {
          foreach (var methodModel in model_.Methods) {
            if (string.IsNullOrEmpty(methodModel.name)) {
              throw new ArgumentException($"type [{model_.name}] has a method name is empty");
            }

            var info = methods_.GetOrDefault(methodModel.name);
            if (info == null) {
              info = new MethodMetaInfo();
              methods_.Add(methodModel.name, info);
            }
            info.Add(methodModel);
          }
        }
      }

      public XmlMetaModel.FieldModel GetFieldModel(string name) {
        return fields_.GetOrDefault(name);
      }

      public XmlMetaModel.PropertyModel GetPropertyModel(string name) {
        return properties_.GetOrDefault(name);
      }

      public MethodMetaInfo GetMethodMetaInfo(string name) {
        return methods_.GetOrDefault(name);
      }
    }

    private readonly Dictionary<string, XmlMetaModel.NamespaceModel> namespaceNameMaps_ = new();
    private readonly Dictionary<string, TypeMetaInfo> typeMetas_ = new();

    public XmlMetaProvider(IEnumerable<Stream> streams) {
      currentXmlMetaProvider_ = this;

      using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(MetaResources.System))) {
        DeserializeXmlFile(memoryStream);
      }
      foreach (var stream in streams) {
        DeserializeXmlFile(stream);
      }
    }

    private void DeserializeXmlFile(Stream xmlFileStream) {
      var rootElementName = GetXmlRootElementName(xmlFileStream);
      xmlFileStream.Position = 0;
      switch (rootElementName) {
        case "meta": DeserializeXmlMetaFile(xmlFileStream); break;
        case "doc": DeserializeXmlDocFile(xmlFileStream); break;
        default: throw new InvalidOperationException($"Xml root <{rootElementName}> was not expected.");
      }
    }

    private string GetXmlRootElementName(Stream xmlFileStream) {
      // https://stackoverflow.com/questions/4498423/how-do-i-get-the-xml-root-node-with-c/8046938#8046938
      using (var reader = XmlReader.Create(xmlFileStream)) {
        while (reader.Read()) {
          // first element is the root element
          if (reader.NodeType == XmlNodeType.Element) {
            return reader.Name;
          }
        }
      }
      return null;
    }

    private void DeserializeXmlMetaFile(Stream metaFileStream) {
      var serializer = new XmlSerializer(typeof(XmlMetaModel));
      try {
        XmlMetaModel model = (XmlMetaModel)serializer.Deserialize(metaFileStream);
        var assembly = model.Assembly;
        if (assembly != null) {
          if (assembly.Namespaces != null) {
            foreach (var namespaceModel in assembly.Namespaces) {
              LoadNamespace(namespaceModel);
            }
          }
          if (assembly.Classes != null) {
            LoadType(string.Empty, assembly.Classes);
          }
        }
      } catch (Exception e) {
        throw new Exception($"load <meta> xml file wrong at {(metaFileStream is FileStream fs ? fs.Name : "(embedded resource file)")}", e);
      }
    }

    private void DeserializeXmlDocFile(Stream docFileStream) {
      var serializer = new XmlSerializer(typeof(doc));
      try {
        doc model = (doc)serializer.Deserialize(docFileStream);
        var assembly = model.Items[0] as docAssembly;
        var members = model.Items[1] as docMembers;
        const string globalNamespace = "global::";
        var namespaces = new Dictionary<string, XmlMetaModel.NamespaceModel>();
        var classes = new Dictionary<string, List<XmlMetaModel.ClassModel>>(); // key: namespaceName
        var fields = new Dictionary<string, List<XmlMetaModel.FieldModel>>(); // key: className
        var properties = new Dictionary<string, List<XmlMetaModel.PropertyModel>>(); // key: className
        var methods = new Dictionary<string, List<XmlMetaModel.MethodModel>>(); // key: className

        bool IsClassAdded(string fullName) {
          return fields.ContainsKey(fullName);
        }

        void TryAddClass(string fullName) {
          if (IsClassAdded(fullName)) {
            return;
          }
          var @class = new XmlMetaModel.ClassModel();
          @class.name = GetShortName(fullName);
          var namespaceName = GetContainer(fullName);
          if (string.IsNullOrEmpty(namespaceName)) {
            namespaceName = globalNamespace;
          }
          if (!namespaces.ContainsKey(namespaceName)) {
            var namespaceModel = new XmlMetaModel.NamespaceModel();
            namespaceModel.name = namespaceName;
            namespaces.Add(namespaceName, namespaceModel);
            classes.Add(namespaceName, new List<XmlMetaModel.ClassModel>());
          }
          classes[namespaceName].Add(@class);
          // classes.Add(fullName, @class);
          fields.Add(fullName, new List<XmlMetaModel.FieldModel>());
          properties.Add(fullName, new List<XmlMetaModel.PropertyModel>());
          methods.Add(fullName, new List<XmlMetaModel.MethodModel>());
        }

        foreach (var member in members.member) {
          ParseDocMemberName(member.name, out var type, out var fullName, out var parameters);
          switch (type) {
            case 'T':
              TryAddClass(fullName);
              break;

            case 'F':
              var field = new XmlMetaModel.FieldModel();
              field.name = GetShortName(fullName);
              field.Template = Utility.TryGetCodeTemplateFromAttributeText(member.node.FirstOrDefault()?.InnerText);
              fieldMetadata_.Add(member.name, field.Template);
              var container = GetContainer(fullName);
              TryAddClass(container);
              fields[container].Add(field);
              break;

            // TODO: P (property), E (event), N (namespace)

            case 'M':
              var method = new XmlMetaModel.MethodModel();
              method.name = GetShortName(fullName);
              method.Template = Utility.TryGetCodeTemplateFromAttributeText(member.node.FirstOrDefault()?.InnerText);
              method.ArgCount = parameters?.Length ?? -1;
              if (method.ArgCount > 0) {
                method.Args = parameters.Select(param => {
                  var argModel = new XmlMetaModel.ArgumentModel();
                  // argModel.type = param.Replace('{', '<').Replace('}', '>');
                  var split = param.Split('{');
                  if (split.Length > 1) {
                    argModel.type = $"{split[0]}`{split[1].Split(',').Length}";
                  } else {
                    argModel.type = param;
                  }
                  return argModel;
                }).ToArray();
              }
              container = GetContainer(fullName);
              TryAddClass(container);
              methods[container].Add(method);
              break;
          }
        }
        foreach (var @namespace in namespaces) {
          var namespaceName = @namespace.Key;
          var namespaceModel = @namespace.Value;
          foreach (var classModel in classes[namespaceName]) {
            var fullName = $"{namespaceName}.{classModel.name}";
            classModel.Fields = fields[fullName].ToArray();
            classModel.Properties = properties[fullName].ToArray();
            classModel.Methods = methods[fullName].ToArray();
          }
          namespaceModel.Classes = classes[namespaceName].ToArray();
          LoadNamespace(namespaceModel);
        }
        /*foreach (var className in classes.Keys) {
          var @class = classes[className];
          @class.Fields = fields[className].ToArray();
          @class.Properties = properties[className].ToArray();
          @class.Methods = methods[className].ToArray();
          LoadType(GetContainer(className), @class);
        }*/
      } catch (Exception e) {
        throw new Exception($"load <doc> xml file wrong at {(docFileStream is FileStream fs ? fs.Name : "(embedded resource file)")}", e);
      }
    }

    private static void ParseDocMemberName(string name, out char type, out string fullyQualifiedName, out string[] parameters) {
      type = name[0];
      Contract.Assert(name[1] == ':');
      name = name.Substring(2);
      switch (type) {
        case 'T':
        case 'F':
          fullyQualifiedName = name;
          parameters = null;
          break;

        case 'M':
          var split = name.Split('(');
          if (split.Length == 1) {
            fullyQualifiedName = name;
            parameters = null;
          } else {
            fullyQualifiedName = split[0];
            parameters = new string(split[1].Take(split[1].Length - 1).ToArray()).Split(',');
          }
          break;

        default: throw new InvalidDataException($"Unrecognized member type: {type}");
      }
    }

    private static string GetContainer(string name) {
      var split = name.Split('.');
      if (split.Length == 1) {
        return string.Empty;
      }

      return split.Take(split.Length - 1).Aggregate((accum, next) => $"{accum}.{next}");
    }

    private static string GetShortName(string name) {
      return name.Split('.').Last();
    }

    private void LoadNamespace(XmlMetaModel.NamespaceModel model) {
      string namespaceName = model.name;
      if (namespaceName == null) {
        throw new ArgumentException("namespace.name is null");
      }

      if (namespaceName.Length > 0) {
        if (namespaceNameMaps_.ContainsKey(namespaceName)) {
          throw new ArgumentException($"namespace [{namespaceName}] is already has");
        }
        if (!string.IsNullOrEmpty(model.Name) || model.IsBaned) {
          namespaceNameMaps_.Add(namespaceName, model);
        }
      }

      if (model.Classes != null) {
        string name = !string.IsNullOrEmpty(model.Name) ? model.Name : namespaceName;
        LoadType(name, model.Classes);
      }
    }

    private void LoadType(string namespaceName, XmlMetaModel.ClassModel[] classes) {
      foreach (var classModel in classes) {
        LoadType(namespaceName, classModel);
      }
    }

    private void LoadType(string namespaceName, XmlMetaModel.ClassModel classModel) {
      string className = classModel.name;
      if (string.IsNullOrEmpty(className)) {
        throw new ArgumentException($"namespace [{namespaceName}] has a class's name is empty");
      }

      string classesFullName = namespaceName.Length > 0 ? namespaceName + '.' + className : className;
      classesFullName = classesFullName.Replace('`', '_');
      if (typeMetas_.ContainsKey(classesFullName)) {
        throw new ArgumentException($"type [{classesFullName}] is already has");
      }
      TypeMetaInfo info = new TypeMetaInfo(classModel);
      typeMetas_.Add(classesFullName, info);
    }

    public string GetNamespaceMapName(INamespaceSymbol symbol, string original) {
      var info = namespaceNameMaps_.GetOrDefault(original);
      if (info != null) {
        info.CheckBaned(symbol);
        return info.Name;
      }
      return null;
    }

    internal bool MayHaveCodeMeta(ISymbol symbol) {
      return symbol.DeclaredAccessibility == Accessibility.Public && symbol.IsFromAssembly();
    }

    private string GetTypeShortString(ISymbol symbol) {
      INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol.OriginalDefinition;
      return typeSymbol.GetTypeShortName(GetNamespaceMapName);
    }

    internal string GetTypeMapName(ISymbol symbol, string shortName) {
      if (MayHaveCodeMeta(symbol)) {
        var info = GetTypeMetaInfo(symbol, shortName);
        return info?.Model.Name;
      }
      return null;
    }

    internal bool IsTypeIgnoreGeneric(INamedTypeSymbol typeSymbol) {
      if (MayHaveCodeMeta(typeSymbol)) {
        var info = GetTypeMetaInfo(typeSymbol);
        return info != null && info.Model.IgnoreGeneric;
      }
      return false;
    }

    internal bool IsTypeReadOnly(INamedTypeSymbol typeSymbol) {
      if (MayHaveCodeMeta(typeSymbol)) {
        var info = GetTypeMetaInfo(typeSymbol);
        return info != null && info.Model.Readonly;
      }
      return false;
    }

    private TypeMetaInfo GetTypeMetaInfo(ISymbol symbol, string shortName) {
      var info = typeMetas_.GetOrDefault(shortName);
      info?.Model.CheckBaned(symbol);
      return info;
    }

    private TypeMetaInfo GetTypeMetaInfo(INamedTypeSymbol typeSymbol) {
      string shortName = GetTypeShortString(typeSymbol);
      return GetTypeMetaInfo(typeSymbol, shortName);
    }

    private TypeMetaInfo GetTypeMetaInfo(ISymbol memberSymbol) {
      return GetTypeMetaInfo(memberSymbol.ContainingType);
    }

    private XmlMetaModel.FieldModel GetFieldMetaInfo(IFieldSymbol symbol) {
      if (MayHaveCodeMeta(symbol)) {
        return GetTypeMetaInfo(symbol)?.GetFieldModel(symbol.Name);
      }
      return null;
    }

    public string GetFieldCodeTemplate(IFieldSymbol symbol) {
      return GetFieldMetaInfo(symbol)?.Template ?? symbol.GetCodeTemplateFromAttribute();
    }

    public bool IsFieldForceProperty(IFieldSymbol symbol) {
      return GetFieldMetaInfo(symbol)?.IsProperty ?? false;
    }

    private XmlMetaModel.PropertyModel GetPropertyMetaInfo(IPropertySymbol symbol) {
      if (MayHaveCodeMeta(symbol)) {
        var info = GetTypeMetaInfo(symbol)?.GetPropertyModel(symbol.Name);
        info?.CheckBaned(symbol);
        return info;
      }
      return null;
    }

    public bool? IsPropertyField(IPropertySymbol symbol) {
      return GetPropertyMetaInfo(symbol)?.CheckIsField;
    }

    public string GetPropertyCodeTemplate(IPropertySymbol symbol, bool isGet) {
      var info = GetPropertyMetaInfo(symbol);
      if (info != null) {
        return isGet ? info.get?.Template : info.set?.Template;
      }
      return null;
    }

    public string GetPropertyMapName(IPropertySymbol symbol) {
      return GetPropertyMetaInfo(symbol)?.Name;
    }

    private string GetInternalMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType) {
      Contract.Assert(symbol != null);
      if (!symbol.IsPublic()) {
        return null;
      }

      string metaInfo = null;
      if (symbol.IsFromAssembly()) {
        metaInfo = GetTypeMetaInfo(symbol)?.GetMethodMetaInfo(symbol.Name)?.GetMetaInfo(symbol, metaType);
      }

      if (metaInfo == null) {
        if (symbol.IsOverride) {
          if (symbol.OverriddenMethod != null) {
            metaInfo = GetInternalMethodMetaInfo(symbol.OverriddenMethod, metaType);
          }
        } else {
          var interfaceImplementations = symbol.InterfaceImplementations();
          if (interfaceImplementations != null) {
            foreach (IMethodSymbol interfaceMethod in interfaceImplementations) {
              metaInfo = GetInternalMethodMetaInfo(interfaceMethod, metaType);
              if (metaInfo != null) {
                break;
              }
            }
          }
        }
      }
      return metaInfo;
    }

    private string GetMethodMetaInfo(IMethodSymbol symbol, MethodMetaType metaType) {
      Utility.CheckMethodDefinition(ref symbol);
      return GetInternalMethodMetaInfo(symbol, metaType);
    }

    public string GetMethodMapName(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.Name);
    }

    public string GetMethodCodeTemplate(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.CodeTemplate) ?? symbol.GetCodeTemplateFromAttribute();
    }

    public bool IsMethodIgnoreGeneric(IMethodSymbol symbol) {
      return GetMethodMetaInfo(symbol, MethodMetaType.IgnoreGeneric) == bool.TrueString;
    }
  }
}
