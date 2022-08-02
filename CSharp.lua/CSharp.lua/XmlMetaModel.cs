using System;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.CodeAnalysis;

namespace CSharpLua {
  public sealed partial class XmlMetaProvider {
    [XmlRoot("meta")]
    public sealed class XmlMetaModel {
      public sealed class TemplateModel {
        [XmlAttribute]
        public string Template;
      }

      public class MemberModel {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string Baned;

        protected static bool TryTryParseBool(string v, out bool b) {
          b = false;
          if (v != null) {
            if (v.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)) {
              b = true;
              return true;
            }
            if (v.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase)) {
              b = false;
              return false;
            }
          }
          return false;
        }

        internal bool IsBaned {
          get {
            if (!string.IsNullOrEmpty(Baned)) {
              if (TryTryParseBool(Baned, out bool b)) {
                return b;
              }
              return true;
            }
            return false;
          }
        }

        private string BanedMessage {
          get {
            if (!string.IsNullOrEmpty(Baned)) {
              if (TryTryParseBool(Baned, out bool b)) {
                return b ? "cannot use" : null;
              }
              return Baned;
            }
            return null;
          }
        }

        public void CheckBaned(ISymbol symbol) {
          if (IsBaned) {
            throw new CompilationErrorException($"{symbol} is baned, {BanedMessage}");
          }
        }
      }

      public sealed class PropertyModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlElement]
        public TemplateModel set;
        [XmlElement]
        public TemplateModel get;
        [XmlAttribute]
        public string IsField;

        public bool? CheckIsField {
          get {
            if (TryTryParseBool(IsField, out bool b)) {
              return b;
            }
            return null;
          }
        }
      }

      public sealed class FieldModel : MemberModel {
        [XmlAttribute]
        public string Template;
        [XmlAttribute]
        public bool IsProperty;
      }

      public sealed class ArgumentModel {
        [XmlAttribute]
        public string type;
        [XmlElement("arg")]
        public ArgumentModel[] GenericArgs;
      }

      public sealed class MethodModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Template;
        [XmlAttribute]
        public int ArgCount = -1;
        [XmlElement("arg")]
        public ArgumentModel[] Args;
        [XmlAttribute]
        public string RetType;
        [XmlAttribute]
        public int GenericArgCount = -1;
        [XmlAttribute]
        public bool IgnoreGeneric;

        internal string GetMetaInfo(MethodMetaType type) {
          switch (type) {
            case MethodMetaType.Name: {
                return Name;
              }
            case MethodMetaType.CodeTemplate: {
                return Template;
              }
            case MethodMetaType.IgnoreGeneric: {
                return IgnoreGeneric ? bool.TrueString : bool.FalseString;
              }
            default: {
                throw new InvalidOperationException();
              }
          }
        }
      }

      public sealed class ClassModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlElement("property")]
        public PropertyModel[] Properties;
        [XmlElement("field")]
        public FieldModel[] Fields;
        [XmlElement("method")]
        public MethodModel[] Methods;
        [XmlAttribute]
        public bool IgnoreGeneric;
        [XmlAttribute]
        public bool Readonly;
      }

      public sealed class NamespaceModel : MemberModel {
        [XmlAttribute]
        public string Name;
        [XmlElement("class")]
        public ClassModel[] Classes;
      }

      public sealed class AssemblyModel {
        [XmlElement("namespace")]
        public NamespaceModel[] Namespaces;
        [XmlElement("class")]
        public ClassModel[] Classes;
      }

      [XmlElement("assembly")]
      public AssemblyModel Assembly;
    }
  }
}
