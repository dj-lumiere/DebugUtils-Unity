using System.Runtime.CompilerServices;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using DebugUtils.Unity.SceneNavigator;
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
                value: t.transform.position.FormatAsJToken(context: context));
            if (t.transform.childCount > 0)
            {
                var children = new JArray();
                for (var i = 0; i < t.transform.childCount; i++)
                {
                    children.Add(item: t.transform
                                        .GetChild(index: i)
                                        .gameObject
                                        .FormatAsJToken(context: context));
                }

                result.Add(propertyName: "children", value: children);
            }

            return result;
        }
    }

    [ReprFormatter(typeof(Transform))]
    [ReprOptions(true)]
    internal class TransformFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var t = (Transform)obj;
            return $"Transform {t.gameObject.RetrievePath()}/{t.name} @ {t.position.Repr()}";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
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
                value: t.position.FormatAsJToken(context: context));
            if (t.childCount > 0)
            {
                var children = new JArray();
                for (var i = 0; i < t.childCount; i++)
                {
                    children.Add(item: t.GetChild(index: i)
                                        .FormatAsJToken(context: context));
                }

                result.Add(propertyName: "children", value: children);
            }

            return result;
        }
    }
}