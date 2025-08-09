using System;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;
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
                $"{t.gameObject.RetrievePath()}/{t.GetType().Name} @ {t.transform.position.Repr()}";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JValue(value: "<Max Depth Reached>");
            }

            var t = (GameObject)obj;
            var result = new JObject();
            var type = obj.GetType();
            var kind = type.GetTypeKind();
            result.Add(propertyName: "type", value: type.GetReprTypeName());
            result.Add(propertyName: "kind", value: kind);
            result.Add(propertyName: "name", value: t.name);
            result.Add(propertyName: "hashCode", value: $"{RuntimeHelpers.GetHashCode(o: t):X8}");
            result.Add(propertyName: "path", value: t.gameObject.RetrievePath());
            result.Add(propertyName: "position",
                value: t.transform.position.FormatAsJToken(
                    context: context.WithIncrementedDepth()));

            if (t.transform.childCount <= 0)
            {
                return result;
            }

            var children = new JArray();
            var childCount = t.transform.childCount;
            var maxChildren = context.Config.MaxElementsPerCollection >= 0
                ? Math.Min(val1: childCount, val2: context.Config.MaxElementsPerCollection)
                : childCount;

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
                children.Add(item: new JValue(value: $"... {truncatedCount} more children"));
            }

            result.Add(propertyName: "children", value: children);
            result.Add(propertyName: "childCount", value: childCount);

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
            return $"Transform {t.gameObject.RetrievePath()}/{t.name} @ {t.position.Repr()}";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JValue(value: "<Max Depth Reached>");
            }

            var t = (Transform)obj;
            var result = new JObject();
            var type = obj.GetType();
            var kind = type.GetTypeKind();
            result.Add(propertyName: "type", value: type.GetReprTypeName());
            result.Add(propertyName: "kind", value: kind);
            result.Add(propertyName: "name", value: t.name);
            result.Add(propertyName: "hashCode", value: $"{RuntimeHelpers.GetHashCode(o: t):X8}");
            result.Add(propertyName: "path", value: t.gameObject.RetrievePath());
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
            var maxChildren = context.Config.MaxElementsPerCollection >= 0
                ? Math.Min(val1: childCount, val2: context.Config.MaxElementsPerCollection)
                : childCount;

            for (var i = 0; i < maxChildren; i++)
            {
                children.Add(item: t.GetChild(index: i)
                                    .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                childCount > context.Config.MaxElementsPerCollection)
            {
                var truncatedCount = childCount - context.Config.MaxElementsPerCollection;
                children.Add(item: new JValue(value: $"... {truncatedCount} more children"));
            }

            result.Add(propertyName: "children", value: children);
            result.Add(propertyName: "childCount", value: childCount);

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
            if (rb == null)
            {
                return "null";
            }

            var status = rb.isKinematic
                ? "kinematic"
                : $"dynamic mass={rb.mass:F1}";
            var vel = rb.linearVelocity.magnitude;
            return $"Rigidbody({status}, vel={vel:F2}, gravity={rb.useGravity})";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var rb = (Rigidbody)obj;
            if (rb == null)
            {
                return JValue.CreateNull();
            }

            return new JObject
            {
                [propertyName: "type"] = "Rigidbody",
                [propertyName: "kind"] = "class",
                [propertyName: "hashCode"] = $"0x{RuntimeHelpers.GetHashCode(o: rb):X8}",

                // Physics State
                [propertyName: "isKinematic"] = rb.isKinematic,
                [propertyName: "useGravity"] = rb.useGravity,
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
                [propertyName: "freezeRotation"] = rb.freezeRotation,
                [propertyName: "constraints"] = rb.constraints.ToString(),

                // Sleep State
                [propertyName: "sleepThreshold"] = rb.sleepThreshold,
                [propertyName: "IsSleeping"] = rb.IsSleeping(),

                // Interpolation
                [propertyName: "interpolation"] = rb.interpolation.ToString(),
                [propertyName: "collisionDetectionMode"] = rb.collisionDetectionMode.ToString(),

                // GameObject reference
                [propertyName: "gameObject"] = rb.gameObject.RetrievePath()
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

            return new JObject
            {
                [propertyName: "type"] = "Rigidbody2D",
                [propertyName: "kind"] = "class",
                [propertyName: "hashCode"] = $"0x{RuntimeHelpers.GetHashCode(o: rb):X8}",

                // 2D Physics State
                [propertyName: "bodyType"] = rb.bodyType.ToString(),
                [propertyName: "gravityScale"] =
                    rb.gravityScale.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "mass"] =
                    rb.mass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "linearDamping"] =
                    rb.linearDamping.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "angularDamping"] =
                    rb.angularDamping.FormatAsJToken(context: context.WithIncrementedDepth()),

                // 2D Motion
                [propertyName: "linearVelocity"] =
                    rb.linearVelocity.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "angularVelocity"] =
                    rb.angularVelocity.FormatAsJToken(context: context.WithIncrementedDepth()),

                // 2D Center of Mass & Inertia
                [propertyName: "centerOfMass"] =
                    rb.centerOfMass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "worldCenterOfMass"] =
                    rb.worldCenterOfMass.FormatAsJToken(context: context.WithIncrementedDepth()),
                [propertyName: "inertia"] =
                    rb.inertia.FormatAsJToken(context: context.WithIncrementedDepth()),

                // 2D Constraints
                [propertyName: "freezeRotation"] = rb.freezeRotation,
                [propertyName: "constraints"] = rb.constraints.ToString(),

                // Sleep State
                [propertyName: "sleepMode"] = rb.sleepMode.ToString(),
                [propertyName: "IsSleeping"] = rb.IsSleeping(),

                // 2D Specific
                [propertyName: "interpolation"] = rb.interpolation.ToString(),
                [propertyName: "collisionDetectionMode"] = rb.collisionDetectionMode.ToString(),
                [propertyName: "simulated"] = rb.simulated,

                // GameObject reference  
                [propertyName: "gameObject"] = rb.gameObject.RetrievePath()
            };
        }
    }
}