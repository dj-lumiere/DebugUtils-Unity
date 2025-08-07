using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Generic;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Standard
{
    [ReprOptions(needsPrefix: false)]
    internal class TypeFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var typeObject = (Type)obj;

            if (typeObject.IsGenericTypeDefinition)
            {
                return $"Type<{typeObject.FullName}> (generic definition)";
            }

            if (typeObject.IsConstructedGenericType)
            {
                return $"Type<{typeObject.GetReprTypeName()}> (constructed)";
            }

            return $"Type<{typeObject.FullName}>";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            var typeObject = (Type)obj;
            var type = typeObject.GetType();

            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    ["type"] = type.GetReprTypeName(),
                    ["kind"] = type.GetTypeKind(),
                    ["maxDepthReached"] = "true",
                    ["depth"] = context.Depth
                };
            }

            var result = new JObject();
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("hashCode", $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            result.Add("namespace", typeObject.Namespace);
            result.Add("name", typeObject.Name);
            result.Add("fullName", typeObject.FullName);
            var assemblyInfo = new JObject();
            assemblyInfo.Add("name", typeObject.Assembly.GetName()
               .Name);
            assemblyInfo.Add("version", typeObject.Assembly
               .GetName()
               .Version
              ?.ToString());
            assemblyInfo.Add("publicKeyToken", typeObject.Assembly
               .GetName()
               .GetPublicKeyToken()
              ?.ToHexString() ?? "null");
            assemblyInfo.Add("culture", typeObject.Assembly.GetName()
               .CultureName ?? "neutral");
            result.Add("assembly", assemblyInfo);
            result.Add("guid", typeObject.GUID.ToString());
            result.Add("typeHandle",
                typeObject.TypeHandle.Value.ToRepr(context: context));
            result.Add("baseType", typeObject.BaseType?.GetReprTypeName());
            var propertiesStartsWithIs = type
                                        .GetProperties(bindingAttr: BindingFlags.Public |
                                             BindingFlags.Instance)
                                        .Where(predicate: p =>
                                             p.CanRead &&
                                             p.PropertyType == typeof(bool) &&
                                             !p.Name.IsCompilerGeneratedName() &&
                                             p.Name.StartsWith("Is"))
                                        .OrderByDescending(keySelector: p =>
                                             (bool)p.GetValue(obj: obj)!)
                                        .ThenBy(keySelector: p => p.Name);
            var properties = new JArray();
            var availableProperties = new JArray();
            var propertyCount = 0;
            foreach (var property in propertiesStartsWithIs)
            {
                if (context.Config.MaxPropertiesPerObject >= 0 &&
                    propertyCount >= context.Config.MaxPropertiesPerObject)
                {
                    if (availableProperties.Count != 0)
                    {
                        availableProperties.Add("...");
                    }

                    if (properties.Count != 0)
                    {
                        properties.Add("...");
                    }

                    break;
                }

                var propertyValue = property.GetValue(obj: obj);
                if (propertyValue is true)
                {
                    properties.Add(property.Name[2..]
                                                  .ToLowerInvariant());
                }

                availableProperties.Add(property.Name[2..]
                                                       .ToLowerInvariant());

                propertyCount += 1;
            }

            result.Add("properties", properties);
            result.Add("availableProperties", availableProperties);

            return result;
        }
    }

    internal static class AssemblyExtensions
    {
        public static string ToHexString(this byte[]? bytes)
        {
            return bytes == null
                ? "null"
                : BitConverter.ToString(bytes)
                              .Replace(oldValue: "-", newValue: "")
                              .ToLowerInvariant();
        }
    }
}