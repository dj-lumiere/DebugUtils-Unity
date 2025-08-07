# DebugUtils.Repr for Unity

A comprehensive object representation library for Unity developers. **Stop wasting time with useless `ToString()` output
and get meaningful debugging information instantly.**

## Core Features

🔍 **`.Repr()`** - See actual content instead of type names  
🌳 **`.ReprTree()`** - Structured JSON-like output for complex analysis  
🔧 **`.FormatAsJsonNode()`** - Build custom tree formatters using Newtonsoft.Json  
⚡ **Performance-focused** - Built for Unity debugging and production logging    
🎮 **Unity-optimized** - Works seamlessly with Unity objects and components  
🔌 **Extensible** - Create custom formatters for your types

## The Problems We Solve

### Useless ToString() Output

```csharp
var arr = new int[] {1, 2, 3, 4};
Debug.Log(arr.ToString());  // 😞 "System.Int32[]"

var dict = new Dictionary<string, int> {{"a", 1}, {"b", 2}};
Debug.Log(dict.ToString()); // 😞 "System.Collections.Generic.Dictionary`2[System.String,System.Int32]"

// Unity GameObjects are especially bad
GameObject player = GameObject.Find("Player");
Debug.Log(player.ToString()); // 😞 "Player (UnityEngine.GameObject)"
```

## The Solutions

### Meaningful Data Representation

```csharp
using DebugUtils.Repr;

var arr = new int[] {1, 2, 3, 4};
Debug.Log(arr.Repr());  // 😍 "[int(1), int(2), int(3), int(4)]"

var dict = new Dictionary<string, int> {{"a", 1}, {"b", 2}};
Debug.Log(dict.Repr()); // 😍 "{"a": int(1), "b": int(2)}"

// Unity GameObjects with meaningful info
GameObject player = GameObject.Find("Player");
Debug.Log(player.Repr()); // 😍 "GameObject(Player, active: true, layer: 0, tag: Player)"
```

## Features

### 🔍 Object Representation (`.Repr()`)

Works with any type - see actual data instead of useless type names.

### Collections

```csharp
// Arrays (1D, 2D, jagged)
new[] {1, 2, 3}.Repr()                    // 1DArray([int(1), int(2), int(3)])
new[,] {{1, 2}, {3, 4}}.Repr()              // 2DArray([[int(1), int(2)], [int(3), int(4)]])
new[][] {{1, 2}, {3, 4, 5}}.Repr()           // JaggedArray([[int(1), int(2)], [int(3), int(4), int(5)]])

// Lists, Sets, Dictionaries
new List<int> {1, 2, 3}.Repr()           // [int(1), int(2), int(3)]
new HashSet<string> {"a", "b"}.Repr()    // {"a", "b"}
new Dictionary<string, int> {{"x", 1}}.Repr() // {"x": int(1)}
```

### Unity-Specific Types

```csharp
// Vector types
Vector3 pos = new Vector3(1.5f, 2.0f, 3.7f);
pos.Repr()                              // Vector3(1.5, 2, 3.7)

// GameObjects and Components
GameObject player = GameObject.Find("Player");
player.Repr()                           // GameObject(Player, active: true, layer: 0, tag: Player)

Transform transform = player.transform;
transform.Repr()                        // Transform(position: Vector3(0, 1, 0), rotation: Quaternion(0, 0, 0, 1))

// Component arrays
Component[] components = player.GetComponents<Component>();
components.Repr()                       // [Transform(...), Rigidbody(...), PlayerController(...)]
```

### Numeric Types

```csharp
// Integers with different representations
42.Repr()                                              // int(42)
42.Repr(new ReprConfig(IntMode: IntReprMode.Hex))      // int(0x2A)
42.Repr(new ReprConfig(IntMode: IntReprMode.Binary))   // int(0b101010)

// Floating point with exact representation
// Perfect for debugging Unity's floating point precision issues!
(0.1f + 0.2f).Repr()                            
// float(3.00000011920928955078125E-1)
0.3f.Repr()                                    
// float(2.99999988079071044921875E-1)

(0.1f + 0.2f).Repr(new ReprConfig(FloatMode: FloatReprMode.General))
// float(0.30000001)
```

### 🌳 Tree Representation (`.ReprTree()`)

Get structured, JSON-like output perfect for understanding complex object hierarchies using **Newtonsoft.Json**:

```csharp
public class PlayerData : MonoBehaviour
{
    public string PlayerName { get; set; }
    public int Level { get; set; }
    public List<string> Inventory { get; set; }
    public Transform SpawnPoint { get; set; }
}

var playerData = GetComponent<PlayerData>();
playerData.PlayerName = "Alice";
playerData.Level = 15;
playerData.Inventory = new List<string> {"sword", "potion"};
playerData.SpawnPoint = GameObject.Find("SpawnPoint").transform;

Debug.Log(playerData.ReprTree());
// Uses Newtonsoft.Json.Linq.JObject for structured output
// hashCode can vary depending on when it got executed.
// Output: {
//   "type": "PlayerData",
//   "kind": "class",
//   "hashCode": "0xABCDABCD"
//   "PlayerName": { "type": "string", "kind": "class", "length": 5, "hashCode": "0xAAAAAAAA", "value": "Alice" },
//   "Level": { "type": "int", "kind": "struct", "value": "15" },
//   "Inventory": {
//     "type": "List",
//     "kind": "class",
//     "count": 2,
//     "value": [
//       { "type": "string", "kind": "class", "length": 5, "hashCode": "0xBBBBBBBB", "value": "sword" },
//       { "type": "string", "kind": "class", "length": 6, "hashCode": "0xCCCCCCCC", "value": "potion" }
//     ]
//   },
//   "SpawnPoint": {
//     "type": "Transform",
//     "kind": "class",
//     "hashCode": "0xDDDDDDDD",
//     "position": { "type": "Vector3", "kind": "struct", "value": "(0, 0, 0)" },
//     "rotation": { "type": "Quaternion", "kind": "struct", "value": "(0, 0, 0, 1)" }
//   }
// }
```

### 🔧 Custom Formatters (`.FormatAsJsonNode()`)

Create your own formatters for specialized types using **Newtonsoft.Json**:

```csharp
using Newtonsoft.Json.Linq;

[ReprFormatter(typeof(Vector3))]
public class Vector3Formatter : IReprFormatter, IReprTreeFormatter
{
    public string ToRepr(object obj, ReprContext context)
    {
        var v = (Vector3)obj;
        return $"Vector3({v.x}, {v.y}, {v.z})";
    }

    public JToken ToReprTree(object obj, ReprContext context)
    {
        var v = (Vector3)obj;
        return new JObject
        {
            ["type"] = "Vector3",
            ["kind"] = "struct",
            ["x"] = v.x.FormatAsJsonNode(context.WithIncrementedDepth()),
            ["y"] = v.y.FormatAsJsonNode(context.WithIncrementedDepth()),
            ["z"] = v.z.FormatAsJsonNode(context.WithIncrementedDepth())
        };
    }
}

// Custom GameObject formatter
[ReprFormatter(typeof(GameObject))]
public class GameObjectFormatter : IReprFormatter, IReprTreeFormatter
{
    public string ToRepr(object obj, ReprContext context)
    {
        var go = (GameObject)obj;
        if (go == null) return "null";
        
        return $"GameObject({go.name}, active: {go.activeSelf}, layer: {go.layer}, tag: {go.tag})";
    }

    public JToken ToReprTree(object obj, ReprContext context)
    {
        var go = (GameObject)obj;
        if (go == null) return JValue.CreateNull();
        
        return new JObject
        {
            ["type"] = "GameObject",
            ["kind"] = "class",
            ["hashCode"] = $"0x{go.GetHashCode():X8}",
            ["name"] = go.name,
            ["activeSelf"] = go.activeSelf,
            ["layer"] = go.layer,
            ["tag"] = go.tag,
            ["transform"] = go.transform.FormatAsJsonNode(context.WithIncrementedDepth())
        };
    }
}
```

## Configuration Options

### Float Formatting

```csharp
var config = new ReprConfig(FloatMode: FloatReprMode.Exact);
3.14159f.Repr(config);     // Exact decimal representation down to very last digit.

var scientific = new ReprConfig(FloatMode: FloatReprMode.Scientific);  
3.14159f.Repr(scientific); // Scientific notation

var rounded = new ReprConfig(FloatMode: FloatReprMode.Round, FloatPrecision: 2);
3.14159f.Repr(rounded);    // Rounded to 2 decimal places
```

### Integer Formatting

```csharp
var hex = new ReprConfig(IntMode: IntReprMode.Hex);
255.Repr(hex);            // Hexadecimal Representation

var binary = new ReprConfig(IntMode: IntReprMode.Binary);
255.Repr(binary);         // Binary Representation

var bytes = new ReprConfig(IntMode: IntReprMode.HexBytes);
255.Repr(bytes);          // Bytestream representation
```

### Type Display

```csharp
var hideTypes = new ReprConfig(
    TypeMode: TypeReprMode.AlwaysHide,
    ContainerReprMode: ContainerReprMode.UseParentConfig
    );
new[] {1, 2, 3}.Repr(hideTypes);  // [1, 2, 3] (no type prefix to child element.)

var showTypes = new ReprConfig(TypeMode: TypeReprMode.AlwaysShow);
new[] {1, 2, 3}.Repr(showTypes);  // 1DArray([int(1), int(2), int(3)])
```

## Real-World Unity Use Cases

### Game Development Debugging

Debug game state and algorithms instantly:

```csharp
public class GameManager : MonoBehaviour
{
    public List<Player> players;
    public Dictionary<string, int> scores;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"Players: {players.Repr()}");
            Debug.Log($"Scores: {scores.Repr()}");
        }
    }
    
    public void ProcessTurn(Player player)
    {
        Debug.Log($"Processing turn for: {player.Repr()}");
        
        var gameState = GetCurrentGameState();
        Debug.Log($"Game State: {gameState.ReprTree()}");  // Full structure for debugging
    }
}
```

### Component Interaction Debugging

```csharp
public class PlayerController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision with: {collision.gameObject.Repr()}");
        Debug.Log($"Contact points: {collision.contacts.Repr()}");
        Debug.Log($"Collision details: {collision.ReprTree()}");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var components = other.GetComponents<Component>();
        Debug.Log($"Triggered by object with components: {components.Repr()}");
    }
}
```

### AI and Pathfinding Debug

```csharp
public class AIController : MonoBehaviour
{
    public void FindPath(Vector3 start, Vector3 end)
    {
        Debug.Log($"Pathfinding from {start.Repr()} to {end.Repr()}");
        
        var path = CalculatePath(start, end);
        Debug.Log($"Calculated path: {path.Repr()}");
        
        if (path.Length == 0)
        {
            var obstacles = DetectObstacles(start, end);
            Debug.Log($"Obstacles detected: {obstacles.ReprTree()}");
        }
    }
}
```

### Unit Testing in Unity

```csharp
[Test]
public void TestPlayerMovement()
{
    var player = CreateTestPlayer();
    var initialPosition = player.transform.position;
    var inputVector = new Vector3(1, 0, 0);
    
    player.Move(inputVector);
    var finalPosition = player.transform.position;
    
    // Amazing error messages when tests fail
    Assert.AreNotEqual(initialPosition, finalPosition, 
        $"Initial: {initialPosition.Repr()}\nInput: {inputVector.Repr()}\nFinal: {finalPosition.Repr()}");
}
```

## API Reference

### Main Methods

```csharp
// String representation (human-readable)
obj.Repr()                           // Uses default config
obj.Repr(config)                     // Uses custom config

// Tree representation (structured JSON-like using Newtonsoft.Json)
obj.ReprTree()                       // Uses default config  
obj.ReprTree(config)                 // Uses custom config

// JToken for custom formatters (requires Newtonsoft.Json)
obj.FormatAsJsonNode(context)        // For building custom tree structures, should pass context.
```

### Configuration

```csharp
// Create configuration
var config = new ReprConfig(
    FloatMode: FloatReprMode.Exact,
    IntMode: IntReprMode.Decimal,
    TypeMode: TypeReprMode.HideObvious,
    MaxDepth: 5,
    MaxElementsPerCollection: 50,
    EnablePrettyPrintForHierarchical: true
);

// Use with any method
obj.Repr(config);
obj.ReprTree(config);
```

## Circular Reference Handling

Automatically detects and handles circular references safely (important for Unity's component references):

```csharp
public class PlayerController : MonoBehaviour
{
    public Transform target;
    public PlayerController buddy;
}

var player1 = playerObject1.GetComponent<PlayerController>();
var player2 = playerObject2.GetComponent<PlayerController>();
player1.buddy = player2;
player2.buddy = player1;  // Circular reference

Debug.Log(player1.Repr());
// Output: PlayerController(target: Transform(...), buddy: PlayerController(target: Transform(...), buddy: <Circular Reference to PlayerController @0x12345678>))
```

## Dependencies

- **Unity 2021.3 LTS** or higher
- **Newtonsoft.Json for Unity** (available through Package Manager)

## Installation

1. Install **Newtonsoft.Json** package via Unity Package Manager:
    - Open Package Manager (Window → Package Manager)
    - Select "Unity Registry"
    - Search for "com.unity.nuget.newtonsoft-json"
    - Click Install

2. Add the DebugUtils.Repr scripts to your Unity project's Scripts folder

## Performance

- **Efficient circular reference detection** using RuntimeHelpers.GetHashCode
- **Minimal allocations** for simple representations
- **Automatic cleanup** prevents memory leaks
- **Depth limiting** prevents stack overflow on deep hierarchies
- **Unity-optimized** - Works well in both Editor and Runtime

## Unity Editor Integration

Works seamlessly in Unity's development workflow:

```csharp
public class DebugInspector : MonoBehaviour
{
    [ContextMenu("Debug This Object")]
    private void DebugThisObject()
    {
        Debug.Log($"GameObject: {this.gameObject.Repr()}");
        Debug.Log($"All Components: {GetComponents<Component>().Repr()}");
        Debug.Log($"Full Object Tree: {this.ReprTree()}");
    }
}
```

## Contributing

Built to solve real debugging pain points in Unity development. If you have ideas for additional features or find bugs,
contributions are welcome!

**Ideas for new features:**

- Unity-specific component formatters
- Scene hierarchy visualization
- Performance profiling integration
- Custom inspector integration

## License

This project follows MIT license.

---

**Stop debugging blind in Unity. See your actual data with crystal clarity. 🎯**