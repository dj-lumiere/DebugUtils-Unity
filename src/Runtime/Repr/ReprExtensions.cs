using System;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.Plastic.Newtonsoft.Json;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr
{
    /// <summary>
    /// Provides extension methods for generating detailed string representations of .NET objects.
    /// This class contains the core functionality for the Repr system, offering Python-like repr() 
    /// capabilities with extensive customization options for debugging, logging, and diagnostic purposes.
    /// </summary>
    /// <remarks>
    /// <para>The ReprExtensions class serves as the main entry point for object representation functionality.
    /// It combines several key areas of functionality:</para>
    /// <list type="bullet">
    /// <item><description><strong>Object Representation:</strong> The primary Repr() extension method that converts any object to a detailed string representation</description></item>
    /// <item><description><strong>Type Name Resolution:</strong> Methods for getting human-readable type names, handling generics, nullable types, and special .NET types</description></item>
    /// <item><description><strong>Configuration Management:</strong> Type mappings and configuration utilities for controlling formatting behavior</description></item>
    /// </list>
    /// 
    /// <para><b>Key Features:</b></para>
    /// <list type="bullet">
    /// <item><description>Automatic circular reference detection and prevention</description></item>
    /// <item><description>Configurable formatting for numeric types, floats, and containers</description></item>
    /// <item><description>Support for hierarchical JSON output mode</description></item>
    /// <item><description>Extensible formatter registry system</description></item>
    /// <item><description>Thread-safe operation with per-call state isolation</description></item>
    /// <item><description>Comprehensive type system integration</description></item>
    /// </list>
    /// 
    /// <para><strong>Performance Characteristics:</strong></para>
    /// <list type="bullet">
    /// <item><description>Optimized type name lookups using static dictionaries</description></item>
    /// <item><description>Efficient circular reference detection using RuntimeHelpers.GetHashCode</description></item>
    /// <item><description>Minimal allocations for simple object representations</description></item>
    /// <item><description>Automatic cleanup prevents memory leaks</description></item>
    /// </list>
    /// 
    /// <para><strong>Extensibility:</strong></para>
    /// <list type="bullet">
    /// <item><description>Custom formatters can be registered via IReprFormatter interface</description></item>
    /// <item><description>Attribute-based configuration using ReprOptionsAttribute</description></item>
    /// <item><description>Configurable container handling strategies</description></item>
    /// <item><description>Support for custom type name mappings</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="Repr{T}(T, ReprConfig?)"/>
    /// <seealso cref="ReprTree{T}(T, ReprConfig?)"/>
    /// <seealso cref="Repr{T}(T, ReprContext)"/>
    /// <seealso cref="FormatAsJToken{T}(T, ReprContext)"/>
    /// <seealso cref="TypeNaming.GetReprTypeName(Type)"/> 
    /// <seealso cref="TypeNaming.GetReprTypeName{T}(T)"/>
    /// <seealso cref="ReprConfig"/>
    /// <seealso cref="ReprContext"/> 
    /// <seealso cref="IReprFormatter"/>
    /// <seealso cref="IReprTreeFormatter"/> 
    public static class ReprExtensions
    {
        #region End User API - Simple Configuration

        /// <summary>
        /// Generates a detailed string representation of any object using optional configuration.
        /// This is the primary method for end users who want to customize formatting behavior
        /// without dealing with internal state management.
        /// </summary>
        /// <typeparam name="T">The type of object to represent.</typeparam>
        /// <param name="obj">The object to represent. Can be null.</param>
        /// <param name="config">
        /// Optional configuration controlling formatting behavior. If null, uses default configuration.
        /// Contains settings for numeric formatting, type display, limits, and other formatting options.
        /// </param>
        /// <returns>
        /// A detailed string representation of the object with human-readable text and optional type prefixes.
        /// Circular references are detected and displayed appropriately.
        /// For tree-like JSON structure, use ReprTree() instead.
        /// </returns>
        /// <remarks>
        /// <para>This method is designed for end users and provides:</para>
        /// <list type="bullet">
        /// <item><description>Automatic circular reference detection and prevention</description></item>
        /// <item><description>Configurable formatting for numbers, floats, and containers</description></item>
        /// <item><description>Extensible formatter registry system</description></item>
        /// <item><description>Special handling for nullable types</description></item>
        /// <item><description>Thread-safe operation with per-call state isolation</description></item>
        /// </list>
        /// <para>Performance considerations:</para>
        /// <list type="bullet">
        /// <item><description>Uses RuntimeHelpers.GetHashCode for object identity</description></item>
        /// <item><description>Maintains a visited set only for reference types</description></item>
        /// <item><description>Automatic cleanup prevents memory leaks</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Basic usage
        /// var list = new List&lt;int&gt; { 1, 2, 3 };
        /// Console.WriteLine(list.Repr()); 
        /// // Output: [1, 2, 3]
        /// 
        /// // With custom configuration
        /// var config = new ReprConfig(FloatMode: FloatReprMode.Exact);
        /// Console.WriteLine(3.14f.Repr(config)); 
        /// // Output: 3.1400001049041748046875E0
        /// 
        /// // Nullable types
        /// int? nullable = 123;
        /// Console.WriteLine(nullable.Repr()); 
        /// // Output: 123
        /// 
        /// // Circular reference detection
        /// var parent = new Node { Name = "Parent" };
        /// var child = new Node { Name = "Child", Parent = parent };
        /// parent.Child = child;
        /// Console.WriteLine(parent.Repr());
        /// // Output: Name: "Parent", Child: Name: "Child", Parent: &lt;Circular Reference to Node @A1B2C3D4&gt;
        /// </code>
        /// </example>
        /// <exception cref="StackOverflowException">
        /// Should not occur due to circular reference detection, but could theoretically happen
        /// with extremely deep object hierarchies exceeding system stack limits.
        /// </exception>
        public static string Repr<T>(this T obj, ReprConfig? config = null)
        {
            var context = config == null
                ? new ReprContext()
                : new ReprContext(config: config);
            return obj.ToRepr(context: context);
        }

        /// <summary>
        /// Generates a hierarchical JSON representation of any object for structured analysis and debugging.
        /// This method produces detailed JSON output with complete type information, object relationships,
        /// and metadata suitable for debugging tools, IDEs, and automated analysis systems.
        /// </summary>
        /// <typeparam name="T">The type of object to represent.</typeparam>
        /// <param name="obj">The object to represent. Can be null.</param>
        /// <param name="config">
        /// Optional configuration controlling formatting behavior. If null, uses default configuration.
        /// Should be configured for hierarchical mode for optimal results.
        /// </param>
        /// <returns>
        /// A formatted JSON string representing the complete structure of the object, including:
        /// - Type information for all values
        /// - Object relationships and hierarchies  
        /// - Circular reference markers where detected
        /// - Null value representations with type context
        /// - Collection metadata (counts, truncation info)
        /// </returns>
        /// <remarks>
        /// <para><strong>⚠️ Important:</strong> This JSON is NOT intended for data serialization or transfer.
        /// It's designed to reveal the underlying object structure for debugging and analysis purposes.
        /// Use System.Text.Json, Newtonsoft.Json, or similar libraries for actual data serialization.</para>
        /// 
        /// <para>This method is the core of hierarchical formatting and provides:</para>
        /// <list type="bullet">
        /// <item><description><strong>Complete Type Information:</strong> Every value includes its .NET type</description></item>
        /// <item><description><strong>Structured Output:</strong> JSON format suitable for machine processing</description></item>
        /// <item><description><strong>Circular Reference Handling:</strong> Safe processing of self-referencing objects</description></item>
        /// <item><description><strong>Debugging Metadata:</strong> Additional context not available in standard JSON serialization</description></item>
        /// </list>
        /// <para>Unlike standard JSON serializers, this method preserves debugging information and handles
        /// circular references gracefully, making it ideal for development and diagnostic scenarios.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var person = new Person { Name = "John", Age = 30 };
        /// var config = new ReprConfig(EnablePrettyPrintForReprTree: true);
        /// var jsonResult = person.ReprTree(config);
        ///
        /// // jsonResult contains structured representation:
        /// // {
        /// //   "type": "Person",
        /// //   "kind": "class",
        /// //   "hashCode": "0xAAAAAAAA",
        /// //   "Name": { "type": "string", "kind": "class", "length": 4, "hashCode": "0xBBBBBBBB", "value": "John" },
        /// //   "Age": { "type": "int", "kind": "struct", "value": "30" }
        /// // }
        /// // hashCode can vary depending on when it got executed.
        /// </code>
        /// </example>
        public static string ReprTree<T>(this T obj, ReprConfig? config = null)
        {
            var context = config == null
                ? new ReprContext()
                : new ReprContext(config: config);
            var settings = new JsonSerializerSettings
            {
                Formatting = (config?.EnablePrettyPrintForReprTree ?? false) ? Formatting.Indented : Formatting.None
            };
            return JsonConvert.SerializeObject(obj.ToReprTree(context: context), settings);
        }

        #endregion

        #region Plugin/Formatter API - Advanced State Management

        /// <summary>
        /// Generates a detailed string representation using the provided context.
        /// This method is primarily intended for plugin developers and custom formatters
        /// who need precise control over state management and circular reference tracking.
        /// </summary>
        /// <typeparam name="T">The type of object to represent.</typeparam>
        /// <param name="obj">The object to represent. Can be null.</param>
        /// <param name="context">
        /// The context controlling formatting behavior and tracking state. Must not be null.
        /// Contains configuration settings, circular reference tracking, and depth management.
        /// </param>
        /// <returns>
        /// A detailed string representation of the object with human-readable text and optional type prefixes.
        /// Circular references are detected and displayed appropriately.
        /// </returns>
        /// <remarks>
        /// <para><strong>Target Audience:</strong> This method is designed for advanced users including:</para>
        /// <list type="bullet">
        /// <item><description>Custom formatter implementers (IReprFormatter)</description></item>
        /// <item><description>Plugin developers building on top of the Repr system</description></item>
        /// <item><description>Library authors who need precise state control</description></item>
        /// </list>
        /// <para><strong>State Management:</strong> The provided context maintains shared state across
        /// the entire representation operation, including visited object tracking and depth management.
        /// This enables proper circular reference detection and depth limiting in nested scenarios.</para>
        /// <para><strong>For End Users:</strong> Consider using the Repr(ReprConfig?) overload instead,
        /// which automatically manages context creation and cleanup.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // In a custom formatter:
        /// public string ToRepr(object obj, ReprContext context)
        /// {
        ///     var person = (Person)obj;
        ///     var parts = new List&lt;string&gt;();
        ///     
        ///     // ✅ Always pass context to child object representations
        ///     parts.Add($"Name: {person.Name.Repr(context.WithIncrementedDepth())}");
        ///     parts.Add($"Manager: {person.Manager?.Repr(context.WithIncrementedDepth()) ?? "null"}");
        ///     
        ///     return string.Join(", ", parts);
        /// }
        /// 
        /// // Plugin usage:
        /// var sharedContext = new ReprContext(myConfig);
        /// var result1 = obj1.Repr(sharedContext);
        /// var result2 = obj2.Repr(sharedContext); // Shares circular reference tracking
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
        /// <seealso cref="Repr{T}(T, ReprConfig?)"/>
        public static string Repr<T>(this T obj, ReprContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return obj.ToRepr(context: context);
        }

        /// <summary>
        /// Gets the JToken representation of an object for use in custom tree formatters.
        /// This method is primarily intended for developers implementing IReprTreeFormatter
        /// who need to build complex hierarchical structures with precise state control.
        /// </summary>
        /// <typeparam name="T">The type of object to represent.</typeparam>
        /// <param name="obj">The object to represent. Can be null - null objects are handled gracefully.</param>
        /// <param name="context">
        /// The context controlling formatting behavior and tracking state. Must not be null.
        /// Contains configuration settings, circular reference tracking, and depth management.
        /// </param>
        /// <returns>
        /// A JToken representation of the object with complete type information, structure,
        /// and metadata. This JToken can be incorporated into larger tree structures or
        /// converted to a JSON string as needed.
        /// </returns>
        /// <remarks>
        /// <para><strong>⚠️ Important:</strong> The resulting JToken is NOT intended for data serialization.
        /// It reveals the underlying object structure with debugging metadata and type information.</para>
        /// 
        /// <para><strong>Target Audience:</strong> This method is designed for advanced formatter developers who need to:</para>
        /// <list type="bullet">
        /// <item><description>Build custom tree formatters that include child objects</description></item>
        /// <item><description>Create complex nested JToken structures</description></item>
        /// <item><description>Access the raw JToken before string conversion</description></item>
        /// <item><description>Combine multiple objects into custom JSON representations</description></item>
        /// </list>
        /// <para><strong>State Management:</strong> The context parameter ensures proper circular reference
        /// detection and depth tracking when building nested structures. Always use 
        /// context.WithIncrementedDepth() when processing child objects.</para>
        /// <para><strong>For End Users:</strong> For simple JToken access, consider using the convenience
        /// overload that accepts ReprConfig? instead of requiring context management.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // In a custom tree formatter:
        /// public JToken ToReprTree(object obj, ReprContext context)
        /// {
        ///     var container = (MyContainer)obj;
        ///     return new JObject
        ///     {
        ///         ["type"] = "MyContainer",
        ///         ["kind"] = "class",
        ///         ["Items"] = new JArray(
        ///             container.Items.Select(item => 
        ///                 item.FormatAsJToken(context.WithIncrementedDepth())
        ///             ).ToArray()
        ///         ),
        ///         ["Metadata"] = container.Meta.FormatAsJToken(context.WithIncrementedDepth())
        ///     };
        /// }
        /// 
        /// // Plugin building custom structure:
        /// var context = new ReprContext(config);
        /// var customStructure = new JObject
        /// {
        ///     ["timestamp"] = DateTime.Now.ToString(),
        ///     ["data"] = myObject.FormatAsJToken(context),
        ///     ["debug"] = debugInfo.FormatAsJToken(context)
        /// };
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
        /// <seealso cref="ReprTree{T}(T, ReprConfig?)"/>
        public static JToken FormatAsJToken<T>(this T obj, ReprContext? context = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return obj.ToReprTree(context: context);
        }

        #endregion
    }
}