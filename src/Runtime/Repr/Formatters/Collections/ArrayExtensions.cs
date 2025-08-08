﻿using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
{
    internal static class ArrayExtensions
    {
        public static string ArrayToReprRecursive(this Array array, int[] indices, int dimension,
            ReprContext context)
        {
            if (dimension == array.Rank - 1)
            {
                // Last dimension - collect actual values
                var items = new List<string>();
                for (var i = 0; i < array.GetLength(dimension: dimension); i++)
                {
                    indices[dimension] = i;
                    if (context.Config.MaxElementsPerCollection >= 0 &&
                        i >= context.Config.MaxElementsPerCollection)
                    {
                        var truncatedItemCount = array.GetLength(dimension: dimension) -
                                                 context.Config.MaxElementsPerCollection;
                        items.Add(item: $"... {truncatedItemCount} more items");
                        break;
                    }

                    var value = array.GetValue(indices: indices);
                    if (value is Array innerArray)
                    {
                        // If the element is a jagged array, recurse directly to format its content
                        // without adding another "Array(...)" wrapper.
                        items.Add(item: innerArray.ArrayToReprRecursive(
                            indices: new int[innerArray.Rank], dimension: 0,
                            context: context.WithIncrementedDepth()));
                    }
                    else
                    {
                        // Otherwise, format the element normally.
                        items.Add(
                            item: value?.Repr(context: context.WithIncrementedDepth()) ?? "null");
                    }
                }

                return "[" + String.Join(separator: ", ", values: items) + "]";
            } // Not last dimension - recurse deeper

            var subArrays = new List<string>();
            for (var i = 0; i < array.GetLength(dimension: dimension); i++)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    var truncatedItemCount = array.GetLength(dimension: dimension) -
                                             context.Config.MaxElementsPerCollection;
                    subArrays.Add(item: $"... {truncatedItemCount} more items");
                    break;
                }

                indices[dimension] = i;
                subArrays.Add(item: ArrayToReprRecursive(array: array, indices: indices,
                    dimension: dimension + 1, context: context.WithIncrementedDepth()));
            }

            return "[" + String.Join(separator: ", ", values: subArrays) + "]";
        }

        public static JToken ArrayToHierarchicalReprRecursive(this Array array, int[] indices,
            int dimension,
            ReprContext context)
        {
            if (dimension == array.Rank - 1)
            {
                // Last dimension - collect actual values
                var items = new JArray();
                for (var i = 0; i < array.GetLength(dimension: dimension); i++)
                {
                    indices[dimension] = i;
                    if (context.Config.MaxElementsPerCollection >= 0 &&
                        i >= context.Config.MaxElementsPerCollection)
                    {
                        var truncatedItemCount = array.GetLength(dimension: dimension) -
                                                 context.Config.MaxElementsPerCollection;
                        items.Add(item: $"... ({truncatedItemCount} more items)");
                        break;
                    }

                    var value = array.GetValue(indices: indices);
                    if (value is Array innerArray)
                    {
                        // If the element is a jagged array, recurse directly to format its content
                        // without adding another "Array(...)" wrapper.
                        items.Add(
                            item: innerArray.FormatAsJToken(
                                context: context.WithIncrementedDepth()));
                    }
                    else
                    {
                        // Otherwise, format the element normally.
                        // Also, visited must be not null at this point, because the Repr call
                        // at the first time should have made visited not null.
                        items.Add(
                            item: value.FormatAsJToken(context: context.WithIncrementedDepth()));
                    }
                }

                return items;
            } // Not last dimension - recurse deeper

            var subArrays = new JArray();
            for (var i = 0; i < array.GetLength(dimension: dimension); i++)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    var truncatedItemCount = array.GetLength(dimension: dimension) -
                                             context.Config.MaxElementsPerCollection;
                    subArrays.Add(item: $"... ({truncatedItemCount} more items)");
                    break;
                }

                indices[dimension] = i;
                subArrays.Add(item: array.ArrayToHierarchicalReprRecursive(indices: indices,
                    dimension: dimension + 1, context: context.WithIncrementedDepth()));
            }

            return subArrays;
        }
    }
}