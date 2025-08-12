#nullable enable
using System.Collections.Generic;

namespace DebugUtils.Unity.Tests
{
    public record TestSettings(string EquipmentName, Dictionary<string, double> EquipmentSettings);
}