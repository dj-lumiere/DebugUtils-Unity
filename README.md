# DebugUtils for Unity

A collection of debugging utilities for Unity developers.

## Core Features

## 🔍 Object Representation ([`Repr`, `ReprTree` Documentation](src/Runtime/Repr/README.md))

Stop getting useless `ToString()` output. See actual object contents in Unity.

## 📍 Call Stack Tracking ([`CallStack` Documentation](src/Runtime/CallStack/README.md))

Know exactly where your code is executing and which methods are calling what in your Unity project.

## 🎮 Scene Navigation ([`SceneNavigator` Documentation](src/Runtime/SceneNavigator/README.md))

Navigate Unity scene hierarchies with simple path strings instead of manual GameObject traversal.

## Installation

1. **Install Newtonsoft.Json for Unity** (required for ReprTree functionality):
    - Open Package Manager (Window → Package Manager)
    - Select "Unity Registry"
    - Search for "com.unity.nuget.newtonsoft-json"
    - Click Install

2. **Add DebugUtils scripts** to your Unity project's Scripts folder

## Quick Start

```csharp
using DebugUtils.CallStack;
using DebugUtils.Repr;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.SceneNavigator;
using UnityEngine;

// 🔍 Better object representation in Unity
var playerData = new { Name = "Alice", Level = 30, Items = new[] {"sword", "potion", "key"} };
Debug.Log(playerData.Repr());
// Output: Anonymous(Name: "Alice", Level: int(30), Items: 1DArray([string("sword"), string("potion"), string("key")]))

// 🌳 Structured tree output for complex analysis (uses Newtonsoft.Json)
Debug.Log(playerData.ReprTree());
// Output: {
//   "type": "Anonymous",
//   "kind": "class",
//   "hashCode": "0xAAAAAAAA",
//   "Name": { "type": "string", "kind": "class", "hashCode": "0xBBBBBBBB", "length": 5, "value": "Alice" },
//   "Level": { "type": "int", "kind": "struct", "value": "30" },
//   "Items": {
//     "type": "1DArray",
//     "kind": "class",
//     "hashCode": "0xCCCCCCCC",
//     "rank": 1,
//     "dimensions": [3],
//     "elementType": "string",
//     "value": [
//       { "type": "string", "kind": "class", "length": 5, "hashCode": "0xDDDDDDDD", "value": "sword" },
//       { "type": "string", "kind": "class", "length": 6, "hashCode": "0xEEEEEEEE", "value": "potion" },
//       { "type": "string", "kind": "class", "length": 3, "hashCode": "0xFFFFFFFF", "value": "key" }
//     ]
//   }
// } (hashCode may vary.)

// 📍 Caller tracking for Unity debugging
public class PlayerController : MonoBehaviour
{
    public void ProcessPlayerAction()
    {
        Debug.Log($"[{CallStack.GetCallerName()}] Processing player action...");
        
        var gameState = GetCurrentGameState();
        Debug.Log($"[{CallStack.GetCallerInfo()}] Game state: {gameState.Repr()}");
    }
}

// 🎮 Unity scene navigation
public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // Find UI elements with simple paths
        Button playButton = SceneNavigator.FindComponentAtPath<Button>("Canvas/MainMenu/PlayButton");
        Text statusText = SceneNavigator.FindComponentAtPath<Text>("Canvas/HUD/StatusText");
        
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
            Debug.Log($"Play button found at: {playButton.gameObject.RetrievePath()}");
        }
    }
}

// Output: [PlayerController.ProcessPlayerAction] Processing player action...
// Output: [PlayerController.ProcessPlayerAction@PlayerController.cs:21:8] Game state: GameState(Score: int(1500), Lives: int(3))
// Output: Play button found at: SampleScene/Canvas/MainMenu/PlayButton
```

## Features

### 🔍 Object Representation (`.Repr()`)

Works with any type - see actual data instead of useless type names, including Unity-specific types.

### Unity Objects and Components

```csharp
// GameObjects and Components
GameObject player = GameObject.Find("Player");
player.Repr()                           // GameObject(Player, active: true, layer: 0, tag: Player)

Transform transform = player.transform;
transform.Repr()                        // Transform(position: Vector3(0, 1, 0), rotation: Quaternion(0, 0, 0, 1))

// Unity Vector types
Vector3 position = new Vector3(1.5f, 2.0f, 3.7f);
position.Repr()                         // Vector3(1.5, 2, 3.7)

// Component arrays
Component[] components = player.GetComponents<Component>();
components.Repr()                       // [Transform(...), Rigidbody(...), PlayerController(...)]
```

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

### Numeric Types (Unity Float Precision)

```csharp
// Integers with different representations
42.Repr()                                              // int(42)
42.Repr(new ReprConfig(IntMode: IntReprMode.Hex))      // int(0x2A)
42.Repr(new ReprConfig(IntMode: IntReprMode.Binary))   // int(0b101010)

// Floating point with exact representation - perfect for Unity's float precision issues!
(0.1f + 0.2f).Repr()                            
// float(3.00000011920928955078125E-1)
0.3f.Repr()                                    
// float(2.99999988079071044921875E-1)

(0.1f + 0.2f).Repr(new ReprConfig(FloatMode: FloatReprMode.General))
// float(0.30000001)
```

### 📍 Caller Method Tracking (`GetCallerName()`)

Perfect for Unity logging, error tracking, and debugging call flows:

```csharp
// PlayerController.cs
public class PlayerController : MonoBehaviour
{
    public void ProcessInput(Vector3 inputVector)
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Processing input: {inputVector.Repr()}");
        
        if (inputVector.magnitude > 1.0f)
        {
            Debug.LogWarning($"[{caller}] Input vector too large, normalizing");
            inputVector = inputVector.normalized;
        }
        
        try 
        {
            MovePlayer(inputVector);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{caller}] Movement failed: {ex.Message}");
            throw;
        }
    }
    
    private void MovePlayer(Vector3 direction)
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Moving player in direction: {direction.Repr()}");
        transform.Translate(direction * Time.deltaTime);
    }
}

// Unity Console Output: [PlayerController.ProcessInput] Processing input: Vector3(0.8, 0, 0.6)
// Unity Console Output: [PlayerController.MovePlayer] Moving player in direction: Vector3(0.8, 0, 0.6)
```

### ℹ️ Detailed Caller Information (`GetCallerInfo()`)

Get the file, line, and column number for even more precise Unity debugging.

```csharp
// GameManager.cs
public class GameManager : MonoBehaviour
{
    public void StartLevel(int levelIndex)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Starting level: {levelIndex}");
        
        if (levelIndex < 0 || levelIndex >= totalLevels)
        {
            Debug.LogError($"[{caller}] Invalid level index: {levelIndex}");
            return;
        }
        
        LoadLevel(levelIndex);
    }
}

// Unity Console Output: [GameManager.StartLevel@GameManager.cs:12:8] Starting level: 3
```

### 🎮 Scene Navigation (`SceneNavigator`)

Navigate Unity scenes with simple path strings:

```csharp
public class UIController : MonoBehaviour
{
    private void Start()
    {
        // Find UI elements with readable paths
        Button playBtn = SceneNavigator.FindComponentAtPath<Button>("Canvas/MainMenu/PlayButton");
        Text scoreText = SceneNavigator.FindComponentAtPath<Text>("Canvas/HUD/ScoreText");
        Slider volumeSlider = SceneNavigator.FindComponentAtPath<Slider>("Canvas/Settings/Audio/VolumeSlider");
        
        if (playBtn != null)
        {
            Debug.Log($"Found play button at: {playBtn.gameObject.RetrievePath()}");
            playBtn.onClick.AddListener(StartGame);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var caller = CallStack.GetCallerName();
        string otherPath = other.gameObject.RetrievePath();
        Debug.Log($"[{caller}] Collision with object at: {otherPath}");
    }
}

// Output: Found play button at: SampleScene/Canvas/MainMenu/PlayButton
// Output: [UIController.OnTriggerEnter] Collision with object at: SampleScene/Level/Player/PlayerController
```

**Use cases:**

- **Component debugging** - Track MonoBehaviour method execution
- **Unity event debugging** - Know exactly which UI event fired
- **Physics debugging** - Trace collision and trigger events
- **Animation debugging** - Track animation event callbacks
- **State machine debugging** - Monitor state transitions

## Configuration Options

### Float Formatting (Perfect for Unity's Precision Issues)

```csharp
var config = new ReprConfig(FloatMode: FloatReprMode.Exact);
transform.position.x.Repr(config);     // Exact decimal representation down to very last digit.

var scientific = new ReprConfig(FloatMode: FloatReprMode.Scientific, FloatPrecision: 5);  
Time.deltaTime.Repr(scientific);       // Scientific notation with 5 valid digits.

var rounded = new ReprConfig(FloatMode: FloatReprMode.Round, FloatPrecision: 3);
Vector3.Distance(pointA, pointB).Repr(rounded);    // Rounded to 3 decimal places.
```

### Integer Formatting

```csharp
var hex = new ReprConfig(IntMode: IntReprMode.Hex);
gameObject.GetInstanceID().Repr(hex);            // Hexadecimal Representation

var binary = new ReprConfig(IntMode: IntReprMode.Binary);
layerMask.Repr(binary);         // Binary Representation for layer masks

var bytes = new ReprConfig(IntMode: IntReprMode.HexBytes);
colorInt.Repr(bytes);          // Bytestream representation for color values
```

### Type Display

```csharp
var hideTypes = new ReprConfig(
    TypeMode: TypeReprMode.AlwaysHide,
    ContainerReprMode: ContainerReprMode.UseParentConfig
    );
playerInventory.ToArray().Repr(hideTypes);  // ["sword", "potion", "key"] (no type prefixes)

var showTypes = new ReprConfig(TypeMode: TypeReprMode.AlwaysShow);
playerStats.Repr(showTypes);  // PlayerStats(Health: int(100), Mana: int(50), Level: int(15))
```

### Hierarchical Display (Using Newtonsoft.Json)

```csharp
public class PlayerData : MonoBehaviour
{
    public string PlayerName { get; set; }
    public int Level { get; set; }
    public Vector3 SpawnPosition { get; set; }

    public PlayerData(string name, int level, Vector3 spawn)
    {
        PlayerName = name;
        Level = level;
        SpawnPosition = spawn;
    }
}

var playerData = GetComponent<PlayerData>();
Debug.Log(playerData.ReprTree());
//  Output: {
//     "type": "PlayerData",
//     "kind": "class",
//     "hashCode": "0xAAAAAAAA",
//     "PlayerName": {"type": "string", "kind": "class", "hashCode": "0xBBBBBBBB", "length": 4, "value": "Alice"},
//     "Level": {"type": "int", "kind": "struct", "value": "15"},
//     "SpawnPosition": {
//       "type": "Vector3", 
//       "kind": "struct", 
//       "x": {"type": "float", "kind": "struct", "value": "0"},
//       "y": {"type": "float", "kind": "struct", "value": "1"}, 
//       "z": {"type": "float", "kind": "struct", "value": "0"}
//     }
// } (hashCode may vary)
```

## Real-World Unity Use Cases

### Game Development Debugging

Debug game mechanics and state instantly:

```csharp
public class GameManager : MonoBehaviour
{
    public List<Player> activePlayers;
    public Dictionary<string, int> levelScores;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var caller = CallStack.GetCallerInfo();
            Debug.Log($"[{caller}] Active players: {activePlayers.Repr()}");
            Debug.Log($"[{caller}] Level scores: {levelScores.Repr()}");
        }
    }
    
    public void ProcessGameTick()
    {
        var caller = CallStack.GetCallerName();
        var gameState = GetCurrentGameState();
        Debug.Log($"[{caller}] Game state: {gameState.ReprTree()}");  // Full structure
    }
}
```

### Component Interaction Debugging

```csharp
public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Collision with: {collision.gameObject.Repr()}");
        Debug.Log($"[{caller}] Contact points: {collision.contacts.Repr()}");
        Debug.Log($"[{caller}] Object path: {collision.gameObject.RetrievePath()}");
    }
}
```

### Unity Testing

```csharp
[Test]
public void TestPlayerMovement()
{
    var player = CreateTestPlayer();
    var initialPos = player.transform.position;
    var inputVector = new Vector3(1, 0, 0);
    
    player.ProcessInput(inputVector);
    var finalPos = player.transform.position;
    
    // Amazing error messages when tests fail
    var caller = CallStack.GetCallerInfo();
    Debug.Log($"[{caller}] Initial: {initialPos.Repr()}");
    Debug.Log($"[{caller}] Input: {inputVector.Repr()}");
    Debug.Log($"[{caller}] Final: {finalPos.Repr()}");
    
    Assert.AreNotEqual(initialPos, finalPos);
}
```

## Unity Version Compatibility

- **Unity 2021.3 LTS** and higher (recommended)
- **Unity 2022.3 LTS**
- **Unity 2023.3 LTS**
- **Unity 6000.0**
- **Unity 6000.1** (latest)
- Compatible with all Unity render pipelines (Built-in, URP, HDRP)
- Works on all Unity-supported platforms

## Dependencies

- **Unity 2021.3 LTS+**
- **Newtonsoft.Json for Unity** (com.unity.nuget.newtonsoft-json) - Required for `.ReprTree()` functionality

## Roadmap

**Current Features:**

✅ `.Repr()` - Comprehensive object representation with Unity types  
✅ `.ReprTree()` - Structured JSON tree output using Newtonsoft.Json  
✅ `.FormatAsJsonNode()` - Custom formatter building blocks  
✅ `GetCallerName()` - Simple call stack tracking  
✅ `GetCallerInfo()` - Detailed call stack tracking  
✅ `SceneNavigator` - Unity scene hierarchy navigation  
✅ Unity 2021.3+ support  
✅ Zero additional dependencies (except Newtonsoft.Json)  
✅ Circular reference detection  
✅ Custom formatter system for Unity types

**Planned Features:**

- 🔄 Unity Profiler integration
- 🔄 Component dependency visualization
- 🔄 Scene hierarchy debugging tools
- 🔄 Animation state debugging utilities
- 🔄 Physics debugging helpers
- 🔄 Custom Unity Console window integration

*This library started as a solution for Unity debugging pain points and is growing into a comprehensive Unity debugging
toolkit.*

## Contributing

Built out of frustration with Unity debugging challenges. If you have ideas for additional Unity debugging utilities or find
bugs, contributions are welcome!

**Ideas for new features:**

- Unity-specific performance profiling
- GameObject lifecycle tracking
- Component state change monitoring
- Unity event system debugging
- Custom Unity inspector integration

## License

This project follows MIT license.

---

**Stop debugging blind in Unity. See your actual data with crystal clarity and know exactly where your code executes. 🎮**