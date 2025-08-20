# DebugUtils for Unity

[![Unity 2021.3+](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.0-orange.svg)](package.json)

A comprehensive collection of debugging utilities for Unity developers that provides Python-like repr() functionality, advanced object representation, scene navigation, and call stack analysis tools.

## 📋 Table of Contents

- [Core Features](#core-features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Features](#features)
- [Configuration Options](#configuration-options)
- [Real-World Unity Use Cases](#real-world-unity-use-cases)
- [Performance Considerations](#performance-considerations)
- [Troubleshooting](#troubleshooting)
- [Unity Version Compatibility](#unity-version-compatibility)
- [Dependencies](#dependencies)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

## Core Features

## 🔍 Object Representation ([`Repr`, `ReprTree` Documentation](src/Runtime/Repr/README.md))

Stop getting useless `ToString()` output. See actual object contents in Unity.

## 📍 Call Stack Tracking ([`CallStack` Documentation](src/Runtime/CallStack/README.md))

Know exactly where your code is executing and which methods are calling what in your Unity project.

## 🎮 Scene Navigation ([`SceneNavigator` Documentation](src/Runtime/SceneNavigator/README.md))

Navigate Unity scene hierarchies with simple path strings instead of manual GameObject traversal.

## Installation

### Method 1: Unity Package Manager (Git URL)

1. Open Unity Package Manager (Window → Package Manager)
2. Click the **+** button in the top-left corner
3. Select **"Add package from git URL..."**
4. Enter: `https://github.com/dj-lumiere/DebugUtils-Unity.git#upm`
5. Click **Add**

### Method 2: OpenUPM (Coming Soon)

```bash
# Once published to OpenUPM
openupm add com.lumi.debugutils.unity
```

### Method 3: Manual Installation

1. Download the latest release from [GitHub Releases](https://github.com/dj-lumiere/DebugUtils-Unity/releases)
2. Extract the package
3. Copy the `DebugUtils.Unity` folder to your project's `Assets` folder

### Installing Dependencies

**Newtonsoft.Json for Unity** (required for ReprTree functionality):
1. Open Package Manager (Window → Package Manager)
2. Select "Unity Registry"
3. Search for "com.unity.nuget.newtonsoft-json"
4. Click Install

## Quick Start
### Simple Examples

```csharp
using DebugUtils.Repr;
using UnityEngine;

// See what's actually in your objects
var player = new { Name = "Alice", Health = 100, Position = new Vector3(1, 0, 3) };
Debug.Log(player.Repr());
// Output: Anonymous(Name: "Alice", Health: 100_i32, Position: Vector3(1, 0, 3))

// Track where your code is executing
using DebugUtils.CallStack;

void OnPlayerDamage(int damage) 
{
    Debug.Log($"[{CallStack.GetCallerName()}] Player took {damage} damage");
    // Output: [Enemy.Attack] Player took 25 damage
}

// Navigate Unity scenes easily
using DebugUtils.Unity.SceneNavigator;

var healthBar = SceneNavigator.FindComponentByPath<Slider>("Canvas/HUD/HealthBar");
if (healthBar != null) 
{
    healthBar.value = 0.75f;
}
```

### Complete Example

```csharp
using DebugUtils.CallStack;
using DebugUtils.Repr;
using DebugUtils.Unity.SceneNavigator;
using UnityEngine;
using UnityEngine.UI;

public class GameDebugExample : MonoBehaviour
{
    private void Start()
    {
        // 🔍 See your data clearly
        var inventory = new[] { "Sword", "Shield", "Potion x3" };
        Debug.Log($"Inventory: {inventory.Repr()}");
        // Output: Inventory: ["Sword", "Shield", "Potion x3"]
        
        // 📍 Know where you are in the code
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        Debug.Log($"[{CallStack.GetCallerName()}] Initializing game systems...");
        // Output: [GameDebugExample.Start] Initializing game systems...
        
        // 🎮 Find UI elements easily
        var startButton = SceneNavigator.FindComponentByPath<Button>("Canvas/MainMenu/StartButton");
        if (startButton != null)
        {
            Debug.Log($"Found button at: {startButton.gameObject.GetScenePath()}");
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // See complete collision data
        Debug.Log($"Collision: {collision.gameObject.Repr()}");
        Debug.Log($"At: {collision.contacts[0].point.Repr()}");
    }
}

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
new[] {1, 2, 3}.Repr()                    // 1DArray([1_i32, 2_i32, 3_i32])
new[,] {{1, 2}, {3, 4}}.Repr()              // 2DArray([[1_i32, 2_i32], [3_i32, 4_i32]])
new[][] {{1, 2}, {3, 4, 5}}.Repr()           // JaggedArray([[1_i32, 2_i32], [3_i32, 4_i32, 5_i32]])

// Lists, Sets, Dictionaries
new List<int> {1, 2, 3}.Repr()           // [1_i32, 2_i32, 3_i32]
new HashSet<string> {"a", "b"}.Repr()    // {"a", "b"}
new Dictionary<string, int> {{"x", 1}}.Repr() // {"x": 1_i32}
```

### Numeric Types

```csharp
// Integers with different representations (now with Rust-style type suffixes)
42.Repr()                                              // 42_i32
42.Repr(new ReprConfig(IntFormatString: "X"))          // 0x2A_i32
42.Repr(new ReprConfig(IntFormatString: "O"))          // 0o52_i32  (octal)
42.Repr(new ReprConfig(IntFormatString: "B"))          // 0b101010_i32
((byte)255).Repr()                                     // 255_u8
((long)-1000).Repr()                                   // -1000_i64

// Floating point with exact representation
// You can now recognize the real floating point value
// and find what went wrong when doing arithmetics!
(0.1 + 0.2).Repr()                            
// 3.00000000000000444089209850062616169452667236328125E-001_f64
0.3.Repr()                                    
// 2.99999999999999988897769753748434595763683319091796875E-001_f64

(0.1 + 0.2).Repr(new ReprConfig(FloatFormatString: "G"))
// 0.30000000000000004_f64

// Special formatting modes
3.14f.Repr(new ReprConfig(FloatFormatString: "HP"))    // 0x1.91EB86p+001_f32 (hex power)
3.14f.Repr(new ReprConfig(FloatFormatString: "EX"))    // 3.14000010490417480468750E+000_f32 (exact)
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
        Button playBtn = SceneNavigator.FindComponentByPath<Button>("Canvas/MainMenu/PlayButton");
        Text scoreText = SceneNavigator.FindComponentByPath<Text>("Canvas/HUD/ScoreText");
        Slider volumeSlider = SceneNavigator.FindComponentByPath<Slider>("Canvas/Settings/Audio/VolumeSlider");
        
        if (playBtn != null)
        {
            Debug.Log($"Found play button at: {playBtn.gameObject.GetScenePath()}");
            playBtn.onClick.AddListener(StartGame);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var caller = CallStack.GetCallerName();
        string otherPath = other.gameObject.GetScenePath();
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

### Member Ordering

For object representation, DebugUtils uses deterministic alphabetical ordering within member categories:

1. **Public fields** (alphabetical by name)
2. **Public auto-properties** (alphabetical by name)
3. **Private fields** (alphabetical by name, prefixed with "private_")
4. **Private auto-properties** (alphabetical by name, prefixed with "private_")

```csharp
public class ClassifiedData
{
    public long Id = 5;                    // Category 1: Public field
    public int Age = 10;                   // Category 1: Public field
    public string Writer { get; set; }     // Category 2: Public auto-property
    public string Name { get; set; }       // Category 2: Public auto-property
    private DateTime Date = DateTime.Now;  // Category 3: Private field
    private string Password = "secret";    // Category 3: Private field
    private string Data { get; set; }      // Category 4: Private auto-property
    private Guid Key { get; set; }         // Category 4: Private auto-property
}

// Output with ShowNonPublicProperties: true
// ClassifiedData(Age: 10_i32, Id: 5_i64, Name: "Alice", Writer: "Bob", 
//                private_Date: DateTime(...), private_Password: "secret",
//                private_Data: "info", private_Key: Guid(...))
```

## Configuration Options

### Float Formatting (NEW: Format Strings)

```csharp
// Format strings for floats
var exact = new ReprConfig(FloatFormatString: "EX");
3.14159.Repr(exact);      // 3.14158999999999988261834005243051797151565551757812500E+000_f64

var scientific = new ReprConfig(FloatFormatString: "E5");
3.14159.Repr(scientific); // 3.14159E+000_f64

var rounded = new ReprConfig(FloatFormatString: "F2");
3.14159.Repr(rounded);    // 3.14_f64

// Special debugging modes
var hexPower = new ReprConfig(FloatFormatString: "HP");
3.14f.Repr(hexPower);     // 0x1.91EB86p+001_f32 (hexadecimal floating-point)

// Decimal type also supports HP format (but differently)
var dec = 3.14159m;
dec.Repr();               // 3.14159_m
dec.Repr(new ReprConfig(FloatFormatString: "HP")); // 0x00000000_00000000_0004CB2Fp10-005_m
```

### Integer Formatting (NEW: Format Strings)

```csharp
// Format strings for integers  
var hex = new ReprConfig(IntFormatString: "X");
255.Repr(hex);            // 0xFF_i32

var octal = new ReprConfig(IntFormatString: "O");
255.Repr(octal);          // 0o377_i32

var quaternary = new ReprConfig(IntFormatString: "Q");
255.Repr(quaternary);     // 0q3333_i32

var binary = new ReprConfig(IntFormatString: "B");
255.Repr(binary);         // 0b11111111_i32

// Width specifiers for padding
42.Repr(new ReprConfig(IntFormatString: "X8"));  // 0x0000002A_i32
42.Repr(new ReprConfig(IntFormatString: "B16")); // 0b0000000000101010_i32
```

### Type Display

```csharp
var hideTypes = new ReprConfig(
    TypeMode: TypeReprMode.AlwaysHide,
    ContainerReprMode: ContainerReprMode.UseParentConfig
    );
playerInventory.ToArray().Repr(hideTypes);  // ["sword", "potion", "key"] (no type prefixes)

var showTypes = new ReprConfig(TypeMode: TypeReprMode.AlwaysShow);
playerStats.Repr(showTypes);  // PlayerStats(Health: 100_i32, Mana: 50_i32, Level: 15_i32)

// Note: Numeric types always show their bit-width suffix (_i32, _f64, etc.) 
// regardless of TypeMode setting for clarity about precision
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
        Debug.Log($"[{caller}] Object path: {collision.gameObject.GetScenePath()}");
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


- **Unity 2021.3 LTS** and higher
- Compatible with all Unity render pipelines (Built-in, URP, HDRP)
- Works in both Editor and Runtime

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