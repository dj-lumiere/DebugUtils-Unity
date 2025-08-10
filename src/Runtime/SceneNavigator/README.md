# DebugUtils.Unity.SceneNavigator for Unity

A lightweight Unity scene navigation utility for Unity developers. **Stop manually traversing complex GameObject
hierarchies—find any GameObject or component with simple path strings.**

## Core Features

🎯 **`SceneNavigator.FindGameObjectByPath()`** - Find GameObjects using scene-qualified paths with optional indexing.  
⚡ **`SceneNavigator.FindComponentByPath<T>()`** - Get components directly from path strings with explicit object
selection.  
📍 **`GameObject.GetScenePath()`** - Get fully explicit paths with scene names and sibling indices.  
🔢 **Smart Indexing** - Handle duplicate names with [0], [1], [^1] (last item) syntax.  
🌐 **Multi-Scene Support** - Target specific scenes with `SceneName:/` prefix.  
🔍 **Robust Error Handling** - Gracefully handles missing objects and invalid scenes.  
🎮 **Unity-focused** - Built specifically for Unity's scene hierarchy system.

## The Problem We Solve

When working with complex Unity scenes, finding specific GameObjects buried deep in the hierarchy becomes tedious and
error-prone.

```csharp
public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // The painful way - fragile and verbose
        GameObject canvas = GameObject.Find("Canvas");
        Transform panel = canvas.transform.Find("MainPanel");
        Transform button = panel.Find("ButtonGroup").Find("ConfirmButton");
        Button confirmBtn = button.GetComponent<Button>();
        
        // 😞 What if the hierarchy changes? What if objects are missing?
    }
}
```

## The Solution

Navigate Unity scenes with simple, readable path strings that mirror your hierarchy structure.

### Finding GameObjects by Path

Use `FindGameObjectByPath()` to locate GameObjects anywhere in your scene hierarchy.

```csharp
using DebugUtils.Unity.SceneNavigator;

public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // Clean, readable path-based navigation
        GameObject confirmButton = SceneNavigator.FindGameObjectByPath("Canvas/MainPanel/ButtonGroup/ConfirmButton");
        
        // Or be explicit about which objects when you have duplicates
        GameObject secondButton = SceneNavigator.FindGameObjectByPath("Canvas/MainPanel/ButtonGroup[1]/ConfirmButton[0]");
        
        // Target specific scenes
        GameObject menuButton = SceneNavigator.FindGameObjectByPath("MenuScene:/UI/MainMenu/StartButton");
        
        if (confirmButton != null)
        {
            Debug.Log("Found confirm button!");
        }
        else
        {
            Debug.LogWarning("Confirm button not found in hierarchy");
        }
    }
}
```

### Getting Components Directly

Use `FindComponentByPath<T>()` to find and retrieve components in a single call.

```csharp
public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // Get the component directly - no intermediate steps
        Button confirmBtn = SceneNavigator.FindComponentByPath<Button>("Canvas/MainPanel/ButtonGroup/ConfirmButton");
        Text statusText = SceneNavigator.FindComponentByPath<Text>("Canvas/StatusPanel/StatusText");
        Slider volumeSlider = SceneNavigator.FindComponentByPath<Slider>("Canvas/SettingsPanel/Audio/VolumeSlider");
        
        // Use indexing for precise object selection
        Button firstBtn = SceneNavigator.FindComponentByPath<Button>("UI/ButtonPanel/ActionButton[0]");  // First button
        Button lastBtn = SceneNavigator.FindComponentByPath<Button>("UI/ButtonPanel/ActionButton[^1]");   // Last button
        
        // Multi-scene component access
        AudioSource bgMusic = SceneNavigator.FindComponentByPath<AudioSource>("AudioScene:/BackgroundMusic");
        
        if (confirmBtn != null)
        {
            confirmBtn.onClick.AddListener(OnConfirmClicked);
        }
        
        if (statusText != null)
        {
            statusText.text = "System Ready";
        }
    }
    
    private void OnConfirmClicked()
    {
        Debug.Log("Confirm button was clicked!");
    }
}
```

### Getting GameObject Paths

Use the `GetScenePath()` extension method to get the full hierarchy path of any GameObject.

```csharp
public class DebugHelper : MonoBehaviour
{
    private void Start()
    {
        // Get the full explicit path of this GameObject
        string myPath = this.gameObject.GetScenePath();
        Debug.Log($"This object is located at: {myPath}");
        
        // Example output: "SampleScene:/Canvas[0]/MainPanel[0]/ButtonGroup[0]/ConfirmButton[0]"
    }
    
    private void OnTriggerEnter(Collider other)
    {
        string otherPath = other.gameObject.GetScenePath();
        Debug.Log($"Collision with object at: {otherPath}");
        
        // Example output: "SampleScene:/Player[0]/PlayerController[0]"
    }
}
```

## Installation

Add this project as a reference to your Unity project or copy the `SceneNavigator.cs` file to your project's Scripts
folder.

## API Reference

### `SceneNavigator.FindGameObjectByPath(string path)`

```csharp
public static GameObject FindGameObjectByPath(string path)
```

**Parameters:**

- `path` - A flexible path string with optional scene prefix and indexing (e.g., "SceneName:/Canvas[0]/Panel[1]/Button")

**Returns:** The `GameObject` at the specified path, or `null` if not found.

**Path Format Options:**

- `"Player"` - Root GameObject named "Player" (first match, active scene)
- `"Player[1]"` - Second GameObject named "Player"
- `"Player[^1]"` - Last GameObject named "Player"
- `"UI/Canvas/MainMenu"` - Simple hierarchy path (first matches)
- `"UI/Canvas[0]/MainMenu[1]"` - Explicit indexing for precision
- `"MenuScene:/UI/MainMenu"` - Target specific scene
- `"Level/Enemies/Boss[2]/HealthBar"` - Mixed indexing (third Boss, first HealthBar)

### `SceneNavigator.FindComponentByPath<T>(string path)`

```csharp
public static T FindComponentByPath<T>(string path) where T : Component
```

**Parameters:**

- `path` - A flexible path string with optional scene prefix and indexing (same format as FindGameObjectByPath)
- `T` - The type of component to retrieve

**Returns:** The component of type `T` attached to the GameObject at the specified path, or `null` if not found.

**Common Usage:**

```csharp
// Simple paths (first match)
Button btn = SceneNavigator.FindComponentByPath<Button>("UI/MainMenu/PlayButton");
AudioSource audio = SceneNavigator.FindComponentByPath<AudioSource>("Audio/BackgroundMusic");
Rigidbody rb = SceneNavigator.FindComponentByPath<Rigidbody>("Player/PlayerController");

// Explicit indexing for precision
Button secondBtn = SceneNavigator.FindComponentByPath<Button>("UI/MainMenu/PlayButton[1]");
Text lastScoreText = SceneNavigator.FindComponentByPath<Text>("UI/Scoreboard/PlayerScore[^1]");

// Scene-qualified paths
AudioSource menuMusic = SceneNavigator.FindComponentByPath<AudioSource>("MenuScene:/BackgroundMusic");
```

### `GameObject.GetScenePath()` Extension Method

```csharp
public static string GetScenePath(this GameObject obj)
```

**Returns:** A fully explicit string with scene name and exact sibling indices for deterministic object identification.

**Return Format:** Always returns paths in the format `SceneName:/Object[index]/Child[index]`

**Possible Return Values:**

- `"SampleScene:/Canvas[0]/MainPanel[0]/Button[0]"` - Normal case with explicit indices
- `"SampleScene:/Canvas[0]/MainPanel[1]/Button[2]"` - Third button in second MainPanel
- `"[null gameObject]"` - When the GameObject is null
- `"[invalid Scene]:/Object[0]/Path[0]"` - When the scene is invalid

**Limitations: GameObject names cannot contain: `:`, `/`, `[`, `]`**

## Advanced Path Features

### Explicit Indexing

When you have multiple GameObjects with the same name, use explicit indices to select specific instances:

```csharp
// Create multiple enemies with the same name
// Enemy[0], Enemy[1], Enemy[2] in scene

// Find specific enemies by index
GameObject firstEnemy = SceneNavigator.FindGameObjectByPath("Enemies/Enemy[0]");   // First enemy
GameObject secondEnemy = SceneNavigator.FindGameObjectByPath("Enemies/Enemy[1]");  // Second enemy
GameObject lastEnemy = SceneNavigator.FindGameObjectByPath("Enemies/Enemy[^1]");   // Last enemy (^1)
GameObject secondToLast = SceneNavigator.FindGameObjectByPath("Enemies/Enemy[^2]"); // Second to last (^2)

// Mix indexed and non-indexed segments
GameObject specificWeapon = SceneNavigator.FindGameObjectByPath("Player/Equipment[0]/Weapon[2]");
// First Equipment slot, third Weapon
```

### Scene-Qualified Paths

Target GameObjects in specific scenes using the `SceneName:/` prefix:

```csharp
// Find objects in specific scenes (useful for multi-scene setups)
GameObject menuButton = SceneNavigator.FindGameObjectByPath("MenuScene:/UI/StartButton");
GameObject gameUI = SceneNavigator.FindGameObjectByPath("GameScene:/HUD/HealthBar");
GameObject audioManager = SceneNavigator.FindGameObjectByPath("AudioScene:/AudioManager");

// Components work the same way
Button menuBtn = SceneNavigator.FindComponentByPath<Button>("MenuScene:/UI/StartButton");
AudioSource bgMusic = SceneNavigator.FindComponentByPath<AudioSource>("AudioScene:/BackgroundMusic");
```

### Round-Trip Path Resolution

Use `GetScenePath()` to get exact paths that can reliably find the same object later:

```csharp
public class ObjectTracker : MonoBehaviour
{
    private Dictionary<string, GameObject> trackedObjects = new Dictionary<string, GameObject>();
    
    public void TrackObject(GameObject obj)
    {
        string exactPath = obj.GetScenePath();
        trackedObjects[exactPath] = obj;
        Debug.Log($"Tracking object at: {exactPath}");
        // Output: "GameScene:/Player[0]/Equipment[1]/Sword[2]"
    }
    
    public GameObject FindTrackedObject(string exactPath)
    {
        // Use the exact path to find the same object later
        return SceneNavigator.FindGameObjectByPath(exactPath);
    }
    
    public void ValidateTrackedObjects()
    {
        foreach (var kvp in trackedObjects)
        {
            GameObject found = SceneNavigator.FindGameObjectByPath(kvp.Key);
            bool isValid = found == kvp.Value;
            Debug.Log($"Object at {kvp.Key} is {(isValid ? "valid" : "invalid")}");
        }
    }
}
```

## Error Handling

SceneNavigator handles common error scenarios gracefully:

```csharp
// Null or empty paths
GameObject obj1 = SceneNavigator.FindGameObjectByPath(null);        // Returns null
GameObject obj2 = SceneNavigator.FindGameObjectByPath("");          // Returns null

// Missing objects in hierarchy
GameObject obj3 = SceneNavigator.FindGameObjectByPath("NonExistent/Path");  // Returns null
GameObject obj4 = SceneNavigator.FindGameObjectByPath("Player[99]");         // Returns null (index out of range)

// Missing components
Button btn = SceneNavigator.FindComponentByPath<Button>("Canvas/TextObject");  // Returns null

// Invalid scene names
GameObject obj5 = SceneNavigator.FindGameObjectByPath("FakeScene:/Player");   // Returns null

// Null GameObject path retrieval
GameObject nullObj = null;
string path = nullObj.GetScenePath();  // Returns "[null gameObject]"
```

## Best Practices

### Use Descriptive Paths

```csharp
// ✅ Good: Clear, descriptive hierarchy
Button saveBtn = SceneNavigator.FindComponentByPath<Button>("UI/MainMenu/SaveButton");

// ✅ Good: Explicit indexing when needed
Button firstSaveBtn = SceneNavigator.FindComponentByPath<Button>("UI/MainMenu/SaveButton[0]");
Button lastSaveBtn = SceneNavigator.FindComponentByPath<Button>("UI/MainMenu/SaveButton[^1]");

// ✅ Good: Scene-qualified for multi-scene setups
Button menuSaveBtn = SceneNavigator.FindComponentByPath<Button>("MenuScene:/UI/SaveButton");

// ❌ Bad: Ambiguous or unclear
Button btn = SceneNavigator.FindComponentByPath<Button>("Canvas/Button");
```

### Cache References When Possible

```csharp
public class UIManager : MonoBehaviour
{
    private Button confirmButton;
    private Text statusText;
    
    private void Start()
    {
        // Cache references during initialization
        confirmButton = SceneNavigator.FindComponentByPath<Button>("UI/MainMenu/ConfirmButton");
        statusText = SceneNavigator.FindComponentByPath<Text>("UI/StatusPanel/StatusText");
    }
    
    private void Update()
    {
        // Use cached references instead of searching every frame
        if (confirmButton != null && Input.GetKeyDown(KeyCode.Return))
        {
            confirmButton.onClick.Invoke();
        }
    }
}
```

### Handle Missing Objects Gracefully

```csharp
private void SetupUI()
{
    Button playButton = SceneNavigator.FindComponentByPath<Button>("MainMenu/PlayButton");
    if (playButton == null)
    {
        Debug.LogWarning("Play button not found! Check hierarchy path: MainMenu/PlayButton");
        return;
    }
    
    playButton.onClick.AddListener(StartGame);
}
```

## Performance Considerations

- **Path Resolution**: Uses Unity's built-in `Transform.Find()` method, which is reasonably efficient for moderate
  hierarchy depths.
- **Caching**: For frequently accessed objects, cache the references rather than calling path methods repeatedly.
- **Scene Search**: Only searches the active scene's root GameObjects, keeping searches focused and performant.

## Unity Version Compatibility

- **Unity 2021.3 LTS** and higher
- Compatible with all Unity render pipelines (Built-in, URP, HDRP)
- Works in both Editor and Runtime

## Contributing

If you find bugs or have ideas for improvements, contributions are welcome!

## License

This project follows the MIT license.