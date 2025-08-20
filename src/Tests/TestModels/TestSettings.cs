#nullable enable
using System.Collections.Generic;

namespace DebugUtils.Unity.Tests.TestModels
{
    public record TestSettings(string EquipmentName, Dictionary<string, double> EquipmentSettings);
}