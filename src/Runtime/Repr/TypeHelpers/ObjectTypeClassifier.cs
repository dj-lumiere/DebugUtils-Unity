﻿#nullable enable
using DebugUtils.Unity.Repr.Extensions;
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

namespace DebugUtils.Unity.Repr.TypeHelpers
{
    internal static class ObjectTypeClassifier
    {
        public static bool IsSignedPrimitive<T>(this T obj)
        {
            var type = obj?.GetType();
            return type?.IsSignedPrimitiveType() ?? false;
        }

        public static bool IsIntegerPrimitive<T>(this T obj)
        {
            var type = obj?.GetType();
            return type?.IsIntegerPrimitiveType() ?? false;
        }

        public static bool IsFloat<T>(this T obj)
        {
            var type = obj?.GetType();
            return type?.IsFloatType() ?? false;
        }

        public static bool IsDictionary<T>(this T obj)
        {
            var type = obj?.GetType();
            return type?.IsDictionaryType() ?? false;
        }

        public static bool IsSet<T>(this T obj)
        {
            var type = obj?.GetType();
            return type?.IsSetType() ?? false;
        }

        public static bool IsRecord<T>(this T obj)
        {
            var type = obj?.GetType() ?? typeof(T);
            return type.IsRecordType();
        }

        public static bool IsEnum<T>(this T obj)
        {
            var type = obj?.GetType() ?? typeof(T);
            return type.IsEnumType();
        }

        public static bool IsNullableStruct<T>(this T obj)
        {
            // Check the generic parameter first
            var compileType = typeof(T);
            // For cases where T is an object but obj is actually nullable
            var runtimeType = obj?.GetType();
            return compileType.IsNullableStructType() ||
                   (runtimeType?.IsNullableStructType() ?? false);
        }

        public static bool NeedsTypePrefix<T>(this T obj)
        {
            var type = obj?.GetType() ?? typeof(T);
            return type.NeedsTypePrefixType();
        }
    }
}