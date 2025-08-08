using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Collections;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Functions;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Generic;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Standard;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr
{
    internal static class ReprFormatterRegistry
    {
        private static readonly Dictionary<Type, IReprFormatter> ReprFormatters = new();

        private static readonly List<(Func<Type, bool>, IReprFormatter)>
            ConditionalReprFormatters =
                new();

        private static readonly Dictionary<Type, IReprTreeFormatter> ReprTreeFormatters = new();

        private static readonly List<(Func<Type, bool>, IReprTreeFormatter)>
            ConditionalReprTreeFormatters = new();

        static ReprFormatterRegistry()
        {
            DiscoverAttributedFormatters();
            RegisterConditionalFormatters();
        }

        private static void DiscoverAttributedFormatters()
        {
            // Only register exact type matches from attributes
            var formatterTypes = typeof(ReprFormatterRegistry).Assembly
                                                              .GetTypes()
                                                              .Where(predicate: t =>
                                                                   t.GetCustomAttribute<
                                                                       ReprFormatterAttribute>() !=
                                                                   null);

            foreach (var type in formatterTypes)
            {
                var attr = type.GetCustomAttribute<ReprFormatterAttribute>();
                if (Activator.CreateInstance(type: type) is IReprFormatter formatter)
                {
                    var targetTypes = attr?.TargetTypes ?? Array.Empty<Type>();
                    foreach (var targetType in targetTypes)
                    {
                        // Only register concrete types, not interfaces/base classes
                        if (targetType is { IsInterface: false, IsAbstract: false })
                        {
                            ReprFormatters[key: targetType] = formatter;
                        }
                    }
                }

                if (Activator.CreateInstance(type: type) is IReprTreeFormatter treeFormatter)
                {
                    var targetTypes = attr?.TargetTypes ?? Array.Empty<Type>();
                    foreach (var targetType in targetTypes)
                    {
                        // Only register concrete types, not interfaces/base classes
                        if (targetType is { IsInterface: false, IsAbstract: false })
                        {
                            ReprTreeFormatters[key: targetType] = treeFormatter;
                        }
                    }
                }

            }
        }
        private static void RegisterConditionalFormatters()
        {
            // Only register the interface/base class entries that can't use attributes
            ConditionalReprFormatters.AddRange(
                collection: new List<(Func<Type, bool>, IReprFormatter)>
                {
                    (t => t.IsEnum, new EnumFormatter()),
                    (t => t.IsRecordType(), new RecordFormatter()),
                    (t => t.IsDictionaryType(), new DictionaryFormatter()),
                    (t => t.IsTupleType(), new TupleFormatter()),
                    (t => t.IsArray, new ArrayFormatter()),
                    (t => t.IsSetType(), new SetFormatter()),
                    #if NET6_0_OR_GREATER
                    (t => t.IsPriorityQueueType(), new PriorityQueueFormatter()),
                    #endif
                    (t => typeof(Delegate).IsAssignableFrom(c: t), new FunctionFormatter()),
                    (t => typeof(IEnumerable).IsAssignableFrom(c: t),
                        new EnumerableFormatter()),
                    (t => t.IsAnonymousType(), new ObjectFormatter()),
                    (t => typeof(Type).IsAssignableFrom(c: t), new TypeFormatter()),
                    (t => t.OverridesToStringType(), new ToStringFormatter())
                });
            ConditionalReprTreeFormatters.AddRange(
                collection: new List<(Func<Type, bool>, IReprTreeFormatter)>
                {
                    (t => t.IsEnum, new EnumFormatter()),
                    (t => t.IsDictionaryType(), new DictionaryFormatter()),
                    (t => t.IsTupleType(), new TupleFormatter()),
                    (t => t.IsArray, new ArrayFormatter()),
                    #if NET6_0_OR_GREATER
                    (t => t.IsPriorityQueueType(), new PriorityQueueFormatter()),
                    #endif
                    (t => typeof(Delegate).IsAssignableFrom(c: t), new FunctionFormatter()),
                    (t => typeof(IEnumerable).IsAssignableFrom(c: t),
                        new EnumerableFormatter()),
                    (t => typeof(Type).IsAssignableFrom(c: t), new TypeFormatter()),
                    (t => t.IsAnonymousType(), new ObjectFormatter())
                });
        }
        public static IReprFormatter GetStandardFormatter(Type type, ReprContext context)
        {
            if (context.Config.FormattingMode == FormattingMode.Reflection)
            {
                return new ObjectFormatter();
            }

            if (ReprFormatters.TryGetValue(key: type, value: out var directFormatter))
            {
                return directFormatter;
            }

            foreach (var (condition, formatter) in ConditionalReprFormatters)
            {
                if (condition(arg: type))
                {
                    return formatter;
                }
            }

            return new ObjectFormatter();
        }
        public static IReprTreeFormatter GetTreeFormatter(Type type, ReprContext context)
        {
            if (ReprTreeFormatters.TryGetValue(key: type, value: out var directFormatter))
            {
                return directFormatter;
            }

            foreach (var (condition, formatter) in ConditionalReprTreeFormatters)
            {
                if (condition(arg: type))
                {
                    return formatter;
                }
            }

            return new ObjectFormatter();
        }
    }
}