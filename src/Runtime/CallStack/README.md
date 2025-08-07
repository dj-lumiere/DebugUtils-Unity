# DebugUtils.CallStack for Unity

A lightweight call stack tracking utility for Unity developers. **Stop wondering where errors and logs are coming from -
know exactly which method is executing in your Unity project.**

## Core Features

📍 **`CallStack.GetCallerName()`** - Get a simple `ClassName.MethodName` string.  
ℹ️ **`CallStack.GetCallerInfo()`** - Get a detailed `CallerInfo` object with file, line, and column numbers.  
⚡ **Performance-focused** - A hybrid approach balances performance and detail.  
🎮 **Unity-optimized** - Works seamlessly with MonoBehaviours and Unity's execution flow  
🎯 **Zero dependencies** - Just add to your Unity project and go  
🔍 **Robust Error Handling** - Gracefully handles failures and reports error messages.

## The Problem We Solve

When an error occurs deep in your Unity application, a simple log message often isn't enough.

```csharp
public class PlayerController : MonoBehaviour
{
    private void SavePlayerData(PlayerData data)
    {
        // Something fails deep in call stack
        throw new Exception("Save file corrupted"); // 😞 Where did this come from?
    }
}

// Console Output: Save file corrupted
// 😞 Which method? Which GameObject? Which scene?
```

## The Solution

Instantly enrich your Unity logs with precise location information.

### Option 1: Simple Caller Name

Use `GetCallerName()` for a clean, simple `ClassName.MethodName` string.

```csharp
// PlayerController.cs
using DebugUtils.CallStack;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void ProcessPlayerAction(PlayerAction action)
    {
        var caller = CallStack.GetCallerName();
        try
        {
            Debug.Log($"[{caller}] Processing player action: {action.Type}");
            SavePlayerData(action.Data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{caller}] ERROR: {ex.Message}");
            throw;
        }
    }
    
    private void SavePlayerData(PlayerData data)
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Saving player data to file...");
        throw new Exception("Save file corrupted");
    }
}

// Unity Console Output:
// [PlayerController.ProcessPlayerAction] Processing player action: Move
// [PlayerController.SavePlayerData] Saving player data to file...
// [PlayerController.ProcessPlayerAction] ERROR: Save file corrupted
```

### Option 2: Detailed Caller Info

Use `GetCallerInfo()` to get a rich `CallerInfo` object with the file, line, and column number.

```csharp
// PlayerController.cs
using DebugUtils.CallStack;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void ProcessPlayerAction(PlayerAction action)
    {
        var caller = CallStack.GetCallerInfo();
        try
        {
            Debug.Log($"[{caller}] Processing player action: {action.Type}");
            SavePlayerData(action.Data);
        }
        catch (Exception ex)
        {
            // The ToString() override provides a formatted string
            Debug.LogError($"[{caller}] ERROR: {ex.Message}");
            throw;
        }
    }
    
    private void SavePlayerData(PlayerData data)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Saving player data to file...");
        throw new Exception("Save file corrupted");
    }
}

// Unity Console Output:
// [PlayerController.ProcessPlayerAction@PlayerController.cs:8:12] Processing player action: Move
// [PlayerController.SavePlayerData@PlayerController.cs:21:12] Saving player data to file...
// [PlayerController.ProcessPlayerAction@PlayerController.cs:15:12] ERROR: Save file corrupted
// 😍 Now you know the exact file and line where each message came from!
```

## Unity-Specific Use Cases

### MonoBehaviour Debugging

Perfect for tracking Unity lifecycle methods and component interactions:

```csharp
public class GameManager : MonoBehaviour
{
    private void Start()
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] GameManager starting initialization...");
        
        InitializeGame();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var caller = CallStack.GetCallerName();
            Debug.Log($"[{caller}] Escape key pressed, pausing game...");
            PauseGame();
        }
    }
    
    public void OnPlayerDied(Player player)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Player {player.name} died");
        
        if (GetRemainingPlayers() == 0)
        {
            Debug.Log($"[{caller}] Game over condition met");
            EndGame();
        }
    }
}
```

### Component Interaction Tracking

Track complex component interactions and Unity event flows:

```csharp
public class InventorySystem : MonoBehaviour
{
    public void AddItem(Item item)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Adding item: {item.name}");
        
        if (!CanAddItem(item))
        {
            Debug.LogWarning($"[{caller}] Cannot add item: inventory full");
            return;
        }
        
        items.Add(item);
        OnItemAdded?.Invoke(item);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var caller = CallStack.GetCallerName();
        
        if (other.CompareTag("Item"))
        {
            var item = other.GetComponent<Item>();
            Debug.Log($"[{caller}] Item pickup detected: {item.name}");
            AddItem(item);
        }
    }
}
```

### Coroutine and Async Method Tracking

Track execution flow in Unity's asynchronous operations:

```csharp
public class NetworkManager : MonoBehaviour
{
    public async Task<PlayerData> LoadPlayerDataAsync(int playerId)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Loading player data for ID: {playerId}");
        
        try
        {
            var data = await FetchFromServerAsync(playerId);
            Debug.Log($"[{caller}] Successfully loaded player data");
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{caller}] Failed to load player data: {ex.Message}");
            throw;
        }
    }
    
    private IEnumerator SaveGameCoroutine()
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Starting save game coroutine...");
        
        yield return new WaitForSeconds(0.1f);
        
        Debug.Log($"[{caller}] Save game coroutine completed");
    }
}
```

### Error Handling in Unity Events

Enhanced error tracking for Unity's event system:

```csharp
public class UIManager : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    
    private void Start()
    {
        saveButton.onClick.AddListener(() =>
        {
            var caller = CallStack.GetCallerName();
            Debug.Log($"[{caller}] Save button clicked");
            
            try
            {
                SaveGame();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{caller}] Save failed: {ex.Message}");
                ShowErrorMessage("Save failed!");
            }
        });
        
        loadButton.onClick.AddListener(() =>
        {
            var caller = CallStack.GetCallerName();
            Debug.Log($"[{caller}] Load button clicked");
            
            try
            {
                LoadGame();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{caller}] Load failed: {ex.Message}");
                ShowErrorMessage("Load failed!");
            }
        });
    }
}
```

## Installation

Add the `CallStack.cs` and `CallerInfo.cs` files to your Unity project's Scripts folder.

## API Reference

### `CallStack.GetCallerName()`

```csharp
public static string GetCallerName()
```

**Returns:** A `string` in the format `"ClassName.MethodName"`.

**Possible Return Values:**

- `"PlayerController.Update"` - Normal case
- `"[unknown method]"` - When method information cannot be determined
- `"[unknown class].Update"` - When class information cannot be determined
- `"[error getting caller: exception message]"` - When an exception occurs during stack inspection

### `CallStack.GetCallerInfo()`

```csharp
public static CallerInfo GetCallerInfo(
    [CallerFilePath] string filePath = "", 
    [CallerLineNumber] int lineNumber = 0)
```

**Returns:** A `CallerInfo` struct containing detailed information about the call site.

**Remarks:** This method uses a hybrid approach. It gets the file path and line number at compile time using
attributes (for high performance) and inspects the `StackTrace` at runtime for the class name, method name, and column
number.

### `CallerInfo` Struct

Represents the information retrieved by `GetCallerInfo()`.

**Properties:**

- `string? ClassName { get; }`
- `string? MethodName { get; }`
- `string? FileName { get; }`
- `int LineNumber { get; }`
- `int ColumnNumber { get; }`
- `string? ErrorMessage { get; }`: If not `null`, an error occurred.
- `bool IsValid { get; }`: `true` if the information is valid and no error occurred.

**`ToString()` Override:**

- **Success:** `"ClassName.MethodName@FileName:LineNumber:ColumnNumber"`
- **Failure:** `"[Unknown Caller]"`
- **Error:** `"[Error getting caller info: The error message]"`

## Unity Performance Considerations

- **`GetCallerName()`**: Uses `StackTrace` for a single frame lookup. Safe for occasional logging but avoid in Update() methods.
- **`GetCallerInfo()`**: More expensive due to `StackTrace` inspection. Best used for error handling and initialization.
- **Unity-specific**: Works well with Unity's built-in profiler - call stack info appears in profiler samples.

### Best Practices for Unity

```csharp
// ✅ Good: Use in Start, initialization, and error handling
public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Player controller initialized");
    }
    
    public void TakeDamage(float damage)
    {
        try
        {
            ApplyDamage(damage);
        }
        catch (Exception ex)
        {
            var caller = CallStack.GetCallerName();
            Debug.LogError($"[{caller}] Damage calculation failed: {ex.Message}");
        }
    }
}

// ❌ Bad: Avoid in Update, FixedUpdate, or other high-frequency methods
public class BadExample : MonoBehaviour
{
    private void Update()
    {
        var caller = CallStack.GetCallerInfo(); // ⚠️ Called every frame!
        transform.position += Vector3.forward * Time.deltaTime;
    }
    
    private void OnTriggerStay(Collider other)
    {
        var caller = CallStack.GetCallerName(); // ⚠️ Called very frequently!
        // Processing...
    }
}

// ✅ Better: Cache or use conditionally
public class BetterExample : MonoBehaviour
{
    private bool debugMode = false;
    
    private void Update()
    {
        if (debugMode && Input.GetKeyDown(KeyCode.F1))
        {
            var caller = CallStack.GetCallerName(); // Only when needed
            Debug.Log($"[{caller}] Debug info requested");
        }
        
        transform.position += Vector3.forward * Time.deltaTime;
    }
}
```

## Unity Editor Integration

Works seamlessly with Unity's Console window and provides clickable stack traces:

```csharp
public class DebugHelper : MonoBehaviour
{
    [ContextMenu("Test Call Stack")]
    private void TestCallStack()
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"Context menu called from: {caller}");
        
        // The file path in CallerInfo enables clickable console entries
    }
    
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Debug/Log Current Scene Info")]
    private static void LogSceneInfo()
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
    }
#endif
}
```

## Unity Console Integration

The call stack information integrates perfectly with Unity's Console window:

- **File paths** become clickable links that jump directly to the source code
- **Line numbers** help you navigate to the exact location
- **Method names** provide context about the execution flow
- **Compatible** with Unity's stack trace filtering and console search

## Debugging Complex Unity Systems

### State Machine Debugging

```csharp
public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;
    
    public void ChangeState(PlayerState newState)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] State transition: {currentState} -> {newState}");
        
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }
}

public abstract class PlayerState
{
    public virtual void OnEnter()
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Entering state: {GetType().Name}");
    }
    
    public virtual void OnExit()
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Exiting state: {GetType().Name}");
    }
}
```

### Animation Event Debugging

```csharp
public class AnimationEventHandler : MonoBehaviour
{
    // Called from Animation Events
    public void OnFootstep()
    {
        var caller = CallStack.GetCallerName();
        Debug.Log($"[{caller}] Footstep animation event triggered");
        
        PlayFootstepSound();
    }
    
    public void OnAttackHit()
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Attack hit animation event triggered");
        
        DealDamage();
    }
    
    private void PlayFootstepSound()
    {
        var caller = CallStack.GetCallerName();
        // Now you know this was called from OnFootstep
        Debug.Log($"[{caller}] Playing footstep sound");
    }
}
```

### Physics and Collision Debugging

```csharp
public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var caller = CallStack.GetCallerInfo();
        Debug.Log($"[{caller}] Collision with {collision.gameObject.name}");
        
        ProcessCollision(collision);
    }
    
    private void ProcessCollision(Collision collision)
    {
        var caller = CallStack.GetCallerName();
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"[{caller}] Enemy collision detected");
            TakeDamage(10);
        }
        else if (collision.gameObject.CompareTag("Powerup"))
        {
            Debug.Log($"[{caller}] Powerup collision detected");
            CollectPowerup(collision.gameObject);
        }
    }
}
```

## Target Frameworks

- **Unity 2021.3 LTS** and higher
- **Unity 2022.3 LTS** (recommended)
- **Unity 2023.3 LTS** (latest)
- Compatible with all Unity render pipelines (Built-in, URP, HDRP)

## Platform Compatibility

- **All Unity-supported platforms** including:
  - Windows, Mac, Linux (Editor and Standalone)
  - iOS and Android (Mobile)
  - WebGL
  - Console platforms (PlayStation, Xbox, Nintendo Switch)

## Build Considerations

The CallStack utility works in both **Development Builds** and **Release Builds**:

- **Development Builds**: Full functionality with file paths and line numbers
- **Release Builds**: Method and class names still available, file paths may be stripped
- **IL2CPP**: Fully compatible with Unity's IL2CPP scripting backend

```csharp
// Conditional compilation for different build types
public void LogDebugInfo()
{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    var caller = CallStack.GetCallerInfo(); // Full info in development
    Debug.Log($"[{caller}] Debug info with file location");
#else
    var caller = CallStack.GetCallerName(); // Lighter in release builds
    Debug.Log($"[{caller}] Release build debug info");
#endif
}
```

## Contributing

Built to solve real debugging pain points in Unity development. If you have ideas for additional features or find bugs,
contributions are welcome!

**Ideas for new features:**

- Unity-specific stack frame filtering
- Integration with Unity Profiler
- Custom Unity Console window integration
- Performance profiling for call stack overhead

## License

This project follows the MIT license.

---

**Stop debugging blind in Unity. Know exactly where your code is executing. 🎮**