﻿#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace DebugUtils.Unity.Repr.Formatters
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
            return typeObject switch
            {
                _ when typeObject.IsGenericTypeDefinition =>
                    $"Type<{typeObject.FullName}> (generic definition)",
                _ when typeObject.IsConstructedGenericType =>
                    $"Type<{typeObject.GetReprTypeName()}> (constructed)",
                _ => $"Type<{typeObject.FullName}>"
            };
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var typeObject = (Type)obj;
            var type = typeObject.GetType();
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var assemblyInfo = new JObject
            {
                {
                    "name",
                    typeObject.Assembly.GetName()
                              .Name
                },
                {
                    "version",
                    typeObject.Assembly
                              .GetName()
                              .Version
                             ?.ToString()
                },
                {
                    "publicKeyToken",
                    typeObject.Assembly
                              .GetName()
                              .GetPublicKeyToken()
                             ?.ToHexString() ?? "null"
                },
                {
                    "culture",
                    typeObject.Assembly.GetName()
                              .CultureName ?? "neutral"
                }
            };
            var propertiesStartsWithIs = type
                                        .GetProperties(bindingAttr: BindingFlags.Public |
                                             BindingFlags.Instance)
                                        .Where(predicate: p =>
                                             p.CanRead && p.PropertyType == typeof(bool) &&
                                             !p.Name.IsCompilerGeneratedName() &&
                                             p.Name.StartsWith(value: "Is"))
                                        .OrderByDescending(keySelector: p =>
                                             (bool)p.GetValue(obj: obj)!)
                                        .ThenBy(keySelector: p => p.Name);
            var properties = new JArray();
            var availableProperties = new JArray();
            var propertyCount = 0;
            foreach (var property in propertiesStartsWithIs)
            {
                if (context.Config.MaxItemsPerContainer >= 0 &&
                    propertyCount >= context.Config.MaxItemsPerContainer)
                {
                    if (availableProperties.Count != 0)
                    {
                        availableProperties.Add(item: "...");
                    }

                    if (properties.Count != 0)
                    {
                        properties.Add(item: "...");
                    }

                    break;
                }

                var propertyValue = property.GetValue(obj: obj);
                if (propertyValue is true)
                {
                    properties.Add(item: property.Name[2..]
                                                 .ToLowerInvariant());
                }

                availableProperties.Add(item: property.Name[2..]
                                                      .ToLowerInvariant());
                propertyCount += 1;
            }

            return new JObject
            {
                {
                    "type",
                    type.GetReprTypeName()
                },
                {
                    "kind",
                    type.GetTypeKind()
                },
                {
                    "hashCode",
                    $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}"
                },
                {
                    "namespace",
                    typeObject.Namespace
                },
                {
                    "name",
                    typeObject.Name
                },
                {
                    "fullName",
                    typeObject.FullName
                },
                {
                    "assembly",
                    assemblyInfo
                },
                {
                    "guid",
                    typeObject.GUID.ToString()
                },
                {
                    "typeHandle",
                    typeObject.TypeHandle.Value.ToRepr(context: context)
                },
                {
                    "baseType",
                    typeObject.BaseType?.GetReprTypeName()
                },
                {
                    "properties",
                    properties
                },
                {
                    "availableProperties",
                    availableProperties
                }
            };
        }
    }

    internal static class AssemblyExtensions
    {
        public static string ToHexString(this byte[]? bytes)
        {
            return bytes == null
                ? "null"
                : BitConverter.ToString(value: bytes)
                              .Replace(oldValue: "-", newValue: "")
                              .ToLowerInvariant();
        }
    }
}