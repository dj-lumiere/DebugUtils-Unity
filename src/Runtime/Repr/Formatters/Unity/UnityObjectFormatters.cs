using System.Runtime.CompilerServices;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(GameObject))]
    [ReprOptions(needsPrefix: true)]
    internal class GameObjectFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (GameObject)obj;
            return
                $"{t.gameObject.GetScenePath()} @ {t.transform.position.Repr()}";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (GameObject)obj;
            var result = new JObject();
            var type = obj.GetType();
            var kind = type.GetTypeKind();

            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    [propertyName: "type"] = new JValue(value: type.GetReprTypeName()),
                    [propertyName: "kind"] = new JValue(value: type.GetTypeKind()),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            context = context.WithContainerConfig();

            result.Add(propertyName: "type", value: new JValue(value: type.GetReprTypeName()));
            result.Add(propertyName: "kind", value: new JValue(value: kind));
            result.Add(propertyName: "name", value: new JValue(value: t.name));
            result.Add(propertyName: "hashCode",
                value: new JValue(value: $"0x{RuntimeHelpers.GetHashCode(o: t):X8}"));
            if (context.Depth == 0)
            {
                result.Add(propertyName: "path",
                    value: new JValue(value: t.gameObject.GetScenePath()));
            }

            result.Add(propertyName: "position",
                value: t.transform.position.FormatAsJToken(
                    context: context.WithIncrementedDepth()));

            if (t.transform.childCount <= 0)
            {
                return result;
            }

            var children = new JArray();
            var childCount = t.transform.childCount;
            var maxChildren = childCount;
            if (context.Config.MaxElementsPerCollection >= 0 &&
                maxChildren > context.Config.MaxElementsPerCollection)
            {
                maxChildren = context.Config.MaxElementsPerCollection;
            }

            for (var i = 0; i < maxChildren; i++)
            {
                children.Add(item: t.transform
                                    .GetChild(index: i)
                                    .gameObject
                                    .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                childCount > context.Config.MaxElementsPerCollection)
            {
                var truncatedCount = childCount - context.Config.MaxElementsPerCollection;
                children.Add(item: $"... ({truncatedCount} more items)");
            }

            result.Add(propertyName: "childCount", value: new JValue(value: childCount));
            result.Add(propertyName: "children", value: children);

            return result;
        }
    }

    [ReprFormatter(typeof(Transform))]
    [ReprOptions(needsPrefix: true)]
    internal class TransformFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Transform)obj;
            return $"Transform {t.gameObject.GetScenePath()}/{t.name} @ {t.position.Repr()}";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            var t = (Transform)obj;
            var result = new JObject();
            var type = obj.GetType();
            var kind = type.GetTypeKind();

            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    [propertyName: "type"] = new JValue(value: type.GetReprTypeName()),
                    [propertyName: "kind"] = new JValue(value: type.GetTypeKind()),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            context = context.WithContainerConfig();
            result.Add(propertyName: "type", value: new JValue(value: type.GetReprTypeName()));
            result.Add(propertyName: "kind", value: new JValue(value: kind));
            result.Add(propertyName: "name", value: new JValue(value: t.name));
            result.Add(propertyName: "hashCode",
                value: new JValue(value: $"0x{RuntimeHelpers.GetHashCode(o: t):X8}"));
            if (context.Depth == 0)
            {
                result.Add(propertyName: "path",
                    value: new JValue(value: t.gameObject.GetScenePath()));
            }

            result.Add(propertyName: "position",
                value: t.position.FormatAsJToken(context: context.WithIncrementedDepth()));
            result.Add(propertyName: "rotation",
                value: t.rotation.FormatAsJToken(context: context.WithIncrementedDepth()));
            result.Add(propertyName: "scale",
                value: t.localScale.FormatAsJToken(context: context.WithIncrementedDepth()));

            if (t.childCount <= 0)
            {
                return result;
            }

            var children = new JArray();
            var childCount = t.childCount;
            var maxChildren = childCount;
            if (context.Config.MaxElementsPerCollection >= 0 &&
                maxChildren > context.Config.MaxElementsPerCollection)
            {
                maxChildren = context.Config.MaxElementsPerCollection;
            }

            for (var i = 0; i < maxChildren; i++)
            {
                children.Add(item: t.GetChild(index: i)
                                    .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                childCount > context.Config.MaxElementsPerCollection)
            {
                var truncatedCount = childCount - context.Config.MaxElementsPerCollection;
                children.Add(item: $"... ({truncatedCount} more items)");
            }

            result.Add(propertyName: "childCount", value: new JValue(value: childCount));
            result.Add(propertyName: "children", value: new JValue(value: children));

            return result;
        }
    }

    [ReprFormatter(typeof(Rigidbody))]
    [ReprOptions(needsPrefix: true)]
    internal class RigidbodyFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var rb = (Rigidbody)obj;

            var status = rb.isKinematic
                ? "kinematic"
                : $"dynamic mass={rb.mass:F1}";
            var vel = rb.linearVelocity.magnitude;
            return $"Rigidbody({status}, vel={vel:F2}, gravity={rb.useGravity})";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var rb = (Rigidbody)obj;

            context = context.WithContainerConfig();
            return new JObject
            {
                [propertyName: "type"] = new JValue(value: "Rigidbody"),
                [propertyName: "kind"] = new JValue(value: "class"),
                [propertyName: "hashCode"] =
                    new JValue(value: $"0x{RuntimeHelpers.GetHashCode(o: rb):X8}"),

                // Physics State
                [propertyName: "isKinematic"] = new JValue(value: rb.isKinematic
                   .ToString()
                   .ToLowerInvariant()),
                [propertyName: "useGravity"] = new JValue(value: rb.useGravity
                   .ToString()
                   .ToLowerInvariant()),
                [propertyName: "mass"] =
                    rb.mass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "linearDamping"] =
                    rb.linearDamping.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "angularDamping"] =
                    rb.angularDamping.FormatAsJToken(context: context.WithIncrementedDepth()),

                // Motion
                [propertyName: "linearVelocity"] =
                    rb.linearVelocity.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "angularVelocity"] =
                    rb.angularVelocity.FormatAsJToken(context: context.WithIncrementedDepth()),

                // Center of Mass & Inertia
                [propertyName: "centerOfMass"] =
                    rb.centerOfMass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "worldCenterOfMass"] =
                    rb.worldCenterOfMass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "inertiaTensor"] =
                    rb.inertiaTensor.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "inertiaTensorRotation"] =
                    rb.inertiaTensorRotation.FormatAsJToken(
                        context: context.WithIncrementedDepth()),

                // Constraints
                [propertyName: "freezeRotation"] = new JValue(value: rb.freezeRotation
                   .ToString()
                   .ToLowerInvariant()),
                [propertyName: "constraints"] =
                    rb.constraints.FormatAsJToken(context: context.WithIncrementedDepth()),

                // Sleep State
                [propertyName: "sleepThreshold"] =
                    rb.sleepThreshold.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "IsSleeping"] = new JValue(value: rb.IsSleeping()
                   .ToString()
                   .ToLowerInvariant()),

                // Interpolation
                [propertyName: "interpolation"] =
                    rb.interpolation.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "collisionDetectionMode"] =
                    rb.collisionDetectionMode.FormatAsJToken(
                        context: context.WithIncrementedDepth()),

                // GameObject reference
                [propertyName: "gameObject"] = new JValue(value: rb.gameObject.GetScenePath())
            };
        }
    }

    [ReprFormatter(typeof(Rigidbody2D))]
    [ReprOptions(needsPrefix: true)]
    internal class Rigidbody2DFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var rb = (Rigidbody2D)obj;

            var bodyType = rb.bodyType
                             .ToString()
                             .ToLower();
            var vel = rb.linearVelocity.magnitude;
            return $"Rigidbody2D({bodyType}, vel={vel:F2}, gravity={rb.gravityScale:F1})";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var rb = (Rigidbody2D)obj;

            context = context.WithContainerConfig();
            return new JObject
            {
                [propertyName: "type"] = new JValue(value: "Rigidbody2D"),
                [propertyName: "kind"] = new JValue(value: "class"),

                [propertyName: "hashCode"] =
                    new JValue(value: $"0x{RuntimeHelpers.GetHashCode(o: rb):X8}"),

                // Physics State
                [propertyName: "bodyType"] =
                    rb.bodyType.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "gravityScale"] =
                    rb.gravityScale.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "mass"] =
                    rb.mass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "linearDamping"] =
                    rb.linearDamping.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "angularDamping"] =
                    rb.angularDamping.FormatAsJToken(context: context.WithIncrementedDepth()),

                // Motion
                [propertyName: "linearVelocity"] =
                    rb.linearVelocity.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "angularVelocity"] =
                    rb.angularVelocity.FormatAsJToken(context: context.WithIncrementedDepth()),

                // Center of Mass & Inertia
                [propertyName: "centerOfMass"] =
                    rb.centerOfMass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "worldCenterOfMass"] =
                    rb.worldCenterOfMass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "inertia"] =
                    rb.inertia.FormatAsJToken(context: context.WithIncrementedDepth()),

                // Constraints
                [propertyName: "freezeRotation"] = new JValue(value: rb.freezeRotation
                   .ToString()
                   .ToLowerInvariant()),
                [propertyName: "constraints"] =
                    rb.constraints.FormatAsJToken(context: context.WithIncrementedDepth()),

                // Sleep State
                [propertyName: "sleepMode"] = rb.sleepMode
                                                .FormatAsJToken(
                                                     context: context.WithIncrementedDepth()),
                [propertyName: "IsSleeping"] = new JValue(value: rb.IsSleeping()
                   .ToString()
                   .ToLowerInvariant()),

                // Interpolation
                [propertyName: "interpolation"] =
                    rb.interpolation.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "collisionDetectionMode"] =
                    rb.collisionDetectionMode.FormatAsJToken(
                        context: context.WithIncrementedDepth()),
                [propertyName: "simulated"] = new JValue(value: rb.simulated
                   .ToString()
                   .ToLowerInvariant()),

                // GameObject reference
                [propertyName: "gameObject"] = new JValue(value: rb.gameObject.GetScenePath())
            };
        }
    }
}