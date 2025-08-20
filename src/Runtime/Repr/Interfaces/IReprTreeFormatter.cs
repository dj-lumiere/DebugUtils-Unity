#nullable enable
using DebugUtils.Unity.Repr.Extensions;
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

namespace DebugUtils.Unity.Repr.Interfaces
{
    /// <summary>
    /// Defines the contract for custom object formatters in the Repr system.
    /// Implement this interface to create specialized formatting logic for specific types.
    /// </summary>
    /// <remarks>
    /// <para>Formatters are registered with the ReprFormatterRegistry and automatically
    /// invoked when objects of their target types are encountered during representation.</para>
    /// <para>Custom formatters provide full control over how objects are represented,
    /// including the ability to inspect configuration settings and handle circular references.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [ReprFormatter(typeof(MyCustomType))]
    /// public class MyCustomTreeFormatter : IReprTreeFormatter
    /// {
    ///     public JsonNode ToReprTree(object obj, ReprContext context)
    ///     {
    ///         var custom = (MyCustomType)obj;
    ///         return new JsonObject 
    ///         {
    ///             ["type"] = "MyCustomType",
    ///             ["kind"] = "class",
    ///             ["ImportantProperty"] = custom.ImportantProperty?.FormatAsJsonNode(context.WithIncrementedDepth())
    ///         };
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IReprTreeFormatter
    {
        JToken ToReprTree(object obj, ReprContext context);
    }
}