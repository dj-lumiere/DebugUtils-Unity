# DebugUtils.Unity.SceneNavigator for Unity

A lightweight Unity scene navigation utility for Unity developers. **Stop manually traversing complex GameObject
hierarchies - find any GameObject or component with simple path strings.**

## Core Features

🎯 **`SceneNavigator.FindGameObjectAtPath()`** - Find GameObjects using slash-separated paths.  
⚡ **`SceneNavigator.FindComponentAtPath<T>()`** - Get components directly from path strings.  
📍 **`GameObject.RetrievePath()`** - Get the full hierarchy path of any GameObject.  
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

Use `FindGameObjectAtPath()` to locate GameObjects anywhere in your scene hierarchy.

```csharp
using DebugUtils.Unity.SceneNavigator;

public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // Clean, readable path-based navigation
        GameObject confirmButton = SceneNavigator.FindGameObjectAtPath("Canvas/MainPanel/ButtonGroup/ConfirmButton");
        
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

Use `FindComponentAtPath<T>()` to find and retrieve components in a single call.

```csharp
public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // Get the component directly - no intermediate steps
        Button confirmBtn = SceneNavigator.FindComponentAtPath<Button>("Canvas/MainPanel/ButtonGroup/ConfirmButton");
        Text statusText = SceneNavigator.FindComponentAtPath<Text>("Canvas/StatusPanel/StatusText");
        Slider volumeSlider = SceneNavigator.FindComponentAtPath<Slider>("Canvas/SettingsPanel/Audio/VolumeSlider");
        
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

Use the `RetrievePath()` extension method to get the full hierarchy path of any GameObject.

```csharp
public class DebugHelper : MonoBehaviour
{
    private void Start()
    {
        // Get the full path of this GameObject
        string myPath = this.gameObject.RetrievePath();
        Debug.Log($"This object is located at: {myPath}");
        
        // Example output: "SampleScene/Canvas/MainPanel/ButtonGroup/ConfirmButton"
    }
    
    private void OnTriggerEnter(Collider other)
    {
        string otherPath = other.gameObject.RetrievePath();
        Debug.Log($"Collision with object at: {otherPath}");
        
        // Example output: "SampleScene/Player/PlayerController"
    }
}
```

## Installation

Add this project as a reference to your Unity project or copy the `SceneNavigator.cs` file to your project's Scripts
folder.

## API Reference

### `SceneNavigator.FindGameObjectAtPath(string path)`

```csharp
public static GameObject FindGameObjectAtPath(string path)
```

**Parameters:**

- `path` - A slash-separated string representing the GameObject hierarchy (e.g., "Canvas/Panel/Button")

**Returns:** The `GameObject` at the specified path, or `null` if not found.

**Example Paths:**

- `"Player"` - Root GameObject named "Player"
- `"UI/Canvas/MainMenu"` - MainMenu under Canvas under UI
- `"Level/Enemies/Boss/HealthBar"` - Deeply nested GameObject

### `SceneNavigator.FindComponentAtPath<T>(string path)`

```csharp
public static T FindComponentAtPath<T>(string path) where T : Component
```

**Parameters:**

- `path` - A slash-separated string representing the GameObject hierarchy
- `T` - The type of component to retrieve

**Returns:** The component of type `T` attached to the GameObject at the specified path, or `null` if not found.

**Common Usage:**

```csharp
Button btn = SceneNavigator.FindComponentAtPath<Button>("UI/MainMenu/PlayButton");
AudioSource audio = SceneNavigator.FindComponentAtPath<AudioSource>("Audio/BackgroundMusic");
Rigidbody rb = SceneNavigator.FindComponentAtPath<Rigidbody>("Player/PlayerController");
```

### `GameObject.RetrievePath()` Extension Method

```csharp
public static string RetrievePath(this GameObject obj)
```

**Returns:** A string representing the GameObject's full hierarchy path, prefixed with the scene name.

**Possible Return Values:**

- `"SampleScene/Canvas/MainPanel/Button"` - Normal case
- `"[null gameObject]"` - When the GameObject is null
- `"[invalid scene]/GameObject/Path"` - When the scene is invalid

## Error Handling

SceneNavigator handles common error scenarios gracefully:

```csharp
// Null or empty paths
GameObject obj1 = SceneNavigator.FindGameObjectAtPath(null);        // Returns null
GameObject obj2 = SceneNavigator.FindGameObjectAtPath("");          // Returns null

// Missing objects in hierarchy
GameObject obj3 = SceneNavigator.FindGameObjectAtPath("NonExistent/Path");  // Returns null

// Missing components
Button btn = SceneNavigator.FindComponentAtPath<Button>("Canvas/TextObject");  // Returns null

// Null GameObject path retrieval
GameObject nullObj = null;
string path = nullObj.RetrievePath();  // Returns "[null gameObject]"
```

## Best Practices

### Use Descriptive Paths

```csharp
// ✅ Good: Clear, descriptive hierarchy
Button saveBtn = SceneNavigator.FindComponentAtPath<Button>("UI/MainMenu/SaveButton");

// ❌ Bad: Ambiguous or unclear
Button btn = SceneNavigator.FindComponentAtPath<Button>("Canvas/Button");
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
        confirmButton = SceneNavigator.FindComponentAtPath<Button>("UI/MainMenu/ConfirmButton");
        statusText = SceneNavigator.FindComponentAtPath<Text>("UI/StatusPanel/StatusText");
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
    Button playButton = SceneNavigator.FindComponentAtPath<Button>("MainMenu/PlayButton");
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