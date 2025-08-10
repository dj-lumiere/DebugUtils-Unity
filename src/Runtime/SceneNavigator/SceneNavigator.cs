using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugUtils.Unity
{
    /// <summary>
    /// Provides utility methods for navigating and interacting with GameObjects
    /// in the Unity scene hierarchy. These tools are designed for debugging, 
    /// development, and emergency fixes - not for production architecture.
    /// </summary>
    /// <remarks>
    /// SceneNavigator is intended as a debugging utility for when proper Unity 
    /// workflows are broken or unavailable. Use sparingly and avoid in production code.
    /// For production systems, prefer component references, <c>Object.FindFirstObjectByType</c>,
    /// <c>Object.FindAnyObjectByType</c>, or tags.
    /// <para>Path Format: "SceneName:/GameObject[index]/Child[index]"</para>
    /// <para>- Scene name is optional (uses the active scene if omitted)</para>
    /// <para>- Indices start at 0 and disambiguate objects with same names</para>
    /// <para>- Supports backward indexing with ^N syntax (^1 = last, ^2 = second-to-last)</para>
    /// <para>- Reserved characters: ':', '/', '[', ']' cannot be used in GameObject names</para>
    /// </remarks>
    /// <seealso cref="UnityEngine.Object.FindFirstObjectByType{T}(FindObjectsInactive)"/>
    /// <seealso cref="UnityEngine.Object.FindAnyObjectByType{T}(FindObjectsInactive)"/>
    public static class SceneNavigator
    {
        #region Public API

        /// <summary>
        /// Finds a GameObject in the scene hierarchy using a structured path with scene and index information.
        /// </summary>
        /// <param name="path">
        /// Structured path in the format <c>SceneName:/GameObject[index]/Child[index]</c>.
        /// Scene name is optional (defaults to an active scene if omitted).
        /// Index is optional (defaults to the first match [0] if omitted).
        /// </param>
        /// <returns>
        /// The GameObject at the specified path, or null if not found.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides flexible object finding with optional explicit indexing:
        /// - Without indices: Finds the first matching object at each level
        /// - With indices: Finds the specific object by position among same-named siblings
        /// - Mixed usage: Can mix indexed and non-indexed segments in the same path
        /// </para>
        /// <para>
        /// Format options:
        /// <list type="bullet">
        /// <item>Scene prefix: <c>SceneName:/</c> (optional, defaults to active scene)</item>
        /// <item>Simple hierarchy: <c>Parent/Child</c> (finds first match at each level)</item>
        /// <item>Explicit indexing: <c>Parent[0]/Child[2]</c> (finds specific instances)</item>
        /// <item>Backwards indexing: <c>Parent[0]/Child[^2]</c> (finds child second to last)</item>
        /// <item>Mixed format: <c>Parent/Child[1]</c> (first Parent, second Child)</item>
        /// </list>
        /// </para>
        /// <para>Scene Handling:</para>
        /// <list type="bullet">
        /// <item>If no scene specified: Uses the active scene</item>
        /// <item>If a scene specified but not loaded: Returns null and logs warning (Editor only)</item>
        /// <item>If a scene specified and loaded: Searches in that scene</item>
        /// </list>
        /// <para>
        /// This is a debugging utility intended for development use only.
        /// For production code, use proper component references or Unity's Find methods.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Simple paths - find the first match at each level
        /// // This works if that scene is loaded.
        /// var player = SceneNavigator.FindGameObjectByPath("Player");                    // First "Player" in active scene
        /// var weapon = SceneNavigator.FindGameObjectByPath("Player/Equipment/Weapon");   // First weapon found
        ///
        /// 
        /// // Explicit scene, auto-index objects
        /// var enemy = SceneNavigator.FindGameObjectByPath("BattleScene:/Enemies/Goblin"); // First Goblin in BattleScene
        ///
        /// 
        /// // Explicit indices when you need specific objects
        /// var secondEnemy = SceneNavigator.FindGameObjectByPath("Enemies/Goblin[1]");     // Second Goblin specifically
        /// var lastButton = SceneNavigator.FindGameObjectByPath("UI/Menu/Button[^1]");     // Last button
        ///
        /// 
        /// // Mixed usage - some indexed, some not
        /// var specificWeapon = SceneNavigator.FindGameObjectByPath("Player/Equipment[0]/Weapon[2]"); // First equipment slot, third weapon
        ///
        /// 
        /// // Multi-scene with explicit indices
        /// var specificUI = SceneNavigator.FindGameObjectByPath("UIScene:/Canvas[0]/Panel[1]/Button[0]");
        ///
        /// 
        /// // Exploratory debugging - just find first matches
        /// var someObject = SceneNavigator.FindGameObjectByPath("Level/SomeGroup/SomeObject"); // Quick and easy
        ///
        /// 
        /// // Handle not found
        /// var missing = SceneNavigator.FindGameObjectByPath("NonExistent/Path");
        /// if (missing == null)
        /// {
        ///     Debug.LogWarning("Object not found - try using explicit indices or check hierarchy");
        /// }
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">
        /// Thrown when path format is invalid (malformed brackets, invalid indices, etc.).
        /// </exception>
        /// <seealso cref="FindComponentByPath{T}"/>
        /// <seealso cref="GetScenePath"/>
        public static GameObject FindGameObjectByPath(string path)
        {
            if (String.IsNullOrEmpty(value: path))
            {
                return null;
            }

            // GetScenePath roundtrip
            if (path == "[null gameObject]")
            {
                return null;
            }

            if (!TryParseScenePath(path: path, hierarchyPath: out var hierarchyPath,
                    currentScene: out var currentScene))
            {
                return null;
            }

            ExtractPathIndicesAndNames(hierarchyPath: hierarchyPath,
                cleanNames: out var cleanNames, indices: out var indices);

            // Select the root object by index
            var rootObject =
                currentScene.PickRootByNameAndIndex(name: cleanNames[0], index: indices[0]);

            if (rootObject == null)
            {
                return null;
            }

            // Navigate through the path
            var current = rootObject.transform;
            for (var i = 1; i < cleanNames.Length; i += 1)
            {
                current = current.PickChildByNameAndIndex(name: cleanNames[i], index: indices[i]);
                if (current == null)
                {
                    return null;
                }
            }

            return current.gameObject;
        }

        /// <summary>
        /// Finds a GameObject using a flexible path format and retrieves a component of the specified type.
        /// Combines object finding and component retrieval in a single operation.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve (must inherit from Component).</typeparam>
        /// <param name="path">
        /// Flexible path in the format "SceneName:/GameObject/Child" or "SceneName:/GameObject[index]/Child[index]".
        /// Scene name is optional (defaults to the active scene if omitted).
        /// Indices are optional (defaults to the first match [0] if omitted).
        /// Same format as FindGameObjectByPath.
        /// </param>
        /// <returns>
        /// The component of type T if found on the GameObject at the specified path, or null if not found.
        /// Returns null if either the GameObject is not found or the component is not attached.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method combines FindGameObjectByPath() with GetComponent&lt;T&gt;() For convenient component access.
        /// It uses the same flexible path format with optional scene names and indices.
        /// </para>
        /// <para>
        /// Path format options:
        /// <list type="bullet">
        /// <item>Scene prefix: "SceneName:/" (optional, defaults to active scene)</item>
        /// <item>Simple hierarchy: "Parent/Child" (finds first match at each level)</item>
        /// <item>Explicit indexing: "Parent[0]/Child[2]" (finds specific instances)</item>
        /// <item>Mixed format: "Parent/Child[1]" (first Parent, second Child)</item>
        /// </list>
        /// </para>
        /// <para>
        /// Returns null safely for both missing GameObjects and missing components,
        /// allowing safe usage without null reference exceptions.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Simple component finding - first match at each level
        /// Button playBtn = SceneNavigator.FindComponentByPath&lt;Button&gt;("UI/MainMenu/PlayButton");
        /// Rigidbody playerRB = SceneNavigator.FindComponentByPath&lt;Rigidbody&gt;("Player");
        ///
        /// 
        /// // Explicit scene specification
        /// AudioSource bgMusic = SceneNavigator.FindComponentByPath&lt;AudioSource&gt;("AudioScene:/BackgroundMusic");
        ///
        /// 
        /// // Explicit indexing when you need specific instances
        /// Text firstScore = SceneNavigator.FindComponentByPath&lt;Text&gt;("UI/Scoreboard/PlayerScore[0]");
        /// Text secondScore = SceneNavigator.FindComponentByPath&lt;Text&gt;("UI/Scoreboard/PlayerScore[1]");
        ///
        /// 
        /// // Mixed usage - some indexed, some auto-index
        /// Slider volumeSlider = SceneNavigator.FindComponentByPath&lt;Slider&gt;("Settings/Audio[0]/VolumeSlider");
        ///
        /// 
        /// // Safe usage with null checking
        /// var healthBar = SceneNavigator.FindComponentByPath&lt;Slider&gt;("UI/HUD/HealthBar");
        /// if (healthBar != null)
        /// {
        ///     healthBar.value = currentHealth / maxHealth;
        /// }
        /// else
        /// {
        ///     Debug.LogWarning("Health bar not found or missing Slider component");
        /// }
        ///
        /// 
        /// // Returns null if GameObject exists but component doesn't
        /// Rigidbody uiRigidbody = SceneNavigator.FindComponentByPath&lt;Rigidbody&gt;("UI/Canvas");
        /// // uiRigidbody will be null - UI objects typically don't have Rigidbody components
        ///
        /// 
        /// // Complex nested component finding
        /// WeaponController weaponCtrl = SceneNavigator.FindComponentByPath&lt;WeaponController&gt;("Player/Equipment[0]/Weapon[2]");
        ///
        /// 
        /// // Use with explicit paths from GetScenePath for reliable access
        /// string weaponPath = someWeapon.GetScenePath(); // "GameScene:/Player[0]/Equipment[0]/Sword[1]"
        /// WeaponController sameWeapon = SceneNavigator.FindComponentByPath&lt;WeaponController&gt;(weaponPath);
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">
        /// Thrown when path format is invalid (malformed brackets, invalid indices, etc.).
        /// Same validation rules as FindGameObjectByPath.
        /// </exception>
        /// <seealso cref="FindGameObjectByPath"/>
        /// <seealso cref="GetScenePath"/>
        /// <seealso cref="UnityEngine.Component.GetComponent{T}()"/>
        public static T FindComponentByPath<T>(string path) where T : Component
        {
            var obj = FindGameObjectByPath(path: path);
            return obj != null
                ? obj.GetComponent<T>()
                : null;
        }

        /// <summary>
        /// Gets the complete explicit path of a GameObject including scene name and exact sibling indices.
        /// Always returns a fully qualified path that can reliably locate the same object instance.
        /// </summary>
        /// <param name="obj">The GameObject to get the path for (can be null).</param>
        /// <returns>
        /// An explicit path string in the format <c>SceneName:/Parent[index]/Child[index]/Object[index]</c>.
        /// Always includes scene name and sibling indices for deterministic object identification.
        /// Returns <c>[null gameObject]</c> if obj is <c>null</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method always produces fully explicit paths for reliable object identification:
        /// <list type="bullet">
        /// <item>Scene name: Always included, even for the active scene</item>
        /// <item>Indices: Always included for every path segment based on sibling order</item>
        /// <item>Deterministic: The same object always produces the same path</item>
        /// <item>Round-trip safe: Path can be used with <c>FindGameObjectByPath()</c> to find the same object</item>
        /// </list>
        /// </para>
        /// <para>
        /// The explicit format ensures that the returned path unambiguously identifies
        /// the exact object instance, even in complex hierarchies with duplicate names.
        /// This is essential for debugging, logging, and reliable object tracking.
        /// </para>
        /// <para>
        /// Indices represent the object's position among all siblings with the same name
        /// under the same parent, making paths stable and predictable.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Always get explicit paths with scene and indices
        /// GameObject player = GameObject.Find("Player");
        /// string playerPath = player.GetScenePath();
        /// Debug.Log(playerPath); // Output: "GameScene:/Player[0]" (always explicit)
        ///
        /// 
        /// // Even simple objects get full paths
        /// GameObject canvas = GameObject.Find("Canvas");
        /// string canvasPath = canvas.GetScenePath();
        /// Debug.Log(canvasPath); // Output: "UIScene:/Canvas[0]" (not just "Canvas")
        ///
        /// 
        /// // Nested objects show complete explicit hierarchy
        /// Transform weapon = player.transform.Find("Equipment/Weapon");
        /// string weaponPath = weapon.gameObject.GetScenePath();
        /// Debug.Log(weaponPath); // Output: "GameScene:/Player[0]/Equipment[0]/Weapon[1]"
        ///
        /// 
        /// // Perfect for collision debugging - get exact object identity
        /// void OnTriggerEnter(Collider other)
        /// {
        ///     string colliderPath = other.gameObject.GetScenePath();
        ///     Debug.Log($"Collision with: {colliderPath}");
        ///     // Output: "BattleScene:/Enemies[0]/Goblin[2]/Weapon[0]" - know exactly which Goblin!
        /// }
        ///
        /// 
        /// // Round-trip reliability - explicit path works with FindGameObjectByPath
        /// string explicitPath = myGameObject.GetScenePath();
        /// GameObject foundObject = SceneNavigator.FindGameObjectByPath(explicitPath);
        /// Debug.Assert(foundObject == myGameObject); // Always true with explicit paths
        ///
        /// 
        /// // Use explicit paths for reliable object tracking
        /// Dictionary&lt;string, float&gt; objectHealths = new Dictionary&lt;string, float&gt;();
        /// foreach (var enemy in enemies)
        /// {
        ///     string enemyPath = enemy.GetScenePath(); // Unique identifier
        ///     objectHealths[enemyPath] = enemy.GetComponent&lt;Health&gt;().currentHealth;
        /// }
        ///
        /// 
        /// // Handle edge cases
        /// GameObject nullObj = null;
        /// string nullPath = nullObj.GetScenePath(); // Returns "[null gameObject]"
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the object name contains reserved letters. SceneNavigator reserves ':', '/', '[', ']'
        /// for internal purposes, so please change them before using SceneNavigator.
        /// </exception>
        /// <seealso cref="FindGameObjectByPath"/>
        /// <seealso cref="FindComponentByPath{T}"/>
        public static string GetScenePath(this GameObject obj)
        {
            if (obj == null)
            {
                return "[null gameObject]";
            }

            // Pass 1: Count depth (avoid List resizing)
            var depth = 0;
            for (var transform = obj.transform; transform != null; transform = transform.parent)
            {
                depth += 1;
            }

            var names = new string[depth];
            var indices = new int[depth];

            var k = depth - 1;
            // Pass 2: Fill pre-allocated arrays
            for (var t = obj.transform; t != null; t = t.parent, k -= 1)
            {
                names[k] = t.name;
                foreach (var reservedChar in new[] { ':', '/', '[', ']' })
                {
                    var index = names[k]
                       .IndexOf(reservedChar);
                    if (index >= 0)
                    {
                        throw new ArgumentException(
                            message:
                            $"Reserved character '{reservedChar}' found at position {index} in object name '{names[k]}'. Please rename before using SceneNavigator features",
                            paramName: nameof(obj));
                    }
                }

                indices[k] = t.GetSameNameIndex();
            }

            // Pass 3: Build string with optimal capacity
            var scene = obj.scene;
            var sceneName = scene.IsValid()
                ? scene.name
                : "[invalid Scene]";
            var cap = sceneName.Length + 2;
            for (var i = 0; i < depth; i += 1)
            {
                // [000]/ overhead. This is rough guessing to avoid growth of StringBuilder.
                cap += names[i].Length + 6;
            }

            var sb = new StringBuilder(capacity: cap);
            sb.Append(value: sceneName)
              .Append(value: ":/");

            for (var i = 0; i < depth; i += 1)
            {
                if (i != 0)
                {
                    sb.Append(value: '/');
                }

                sb.Append(value: names[i])
                  .Append(value: '[')
                  .Append(value: indices[i])
                  .Append(value: ']');
            }

            return sb.ToString();
        }

        #endregion

        #region Path Parsing

        private static bool TryParseScenePath(string path, out string hierarchyPath,
            out Scene currentScene)
        {
            var sep = path.IndexOf(value: ":/", comparisonType: StringComparison.Ordinal);
            if (sep >= 0)
            {
                // Check for multiple separators
                var second = path.IndexOf(value: ":/", startIndex: sep + 2,
                    comparisonType: StringComparison.Ordinal);
                if (second >= 0)
                {
                    throw new ArgumentException(
                        message:
                        $"Syntax Error at character {second}: Path contains multiple scene separators.",
                        paramName: nameof(path));
                }

                var sceneName = path[..sep];
                hierarchyPath = path[(sep + 2)..];
                currentScene = SceneManager.GetSceneByName(name: sceneName);

                if (currentScene.IsValid())
                {
                    return true;
                }

                #if UNITY_EDITOR
                Debug.LogWarning(
                    message: $"Scene '{sceneName}' not found - returning null");
                #endif
                return false;
            }

            currentScene = SceneManager.GetActiveScene();
            hierarchyPath = path;
            return true;
        }

        private static void ExtractPathIndicesAndNames(string hierarchyPath,
            out string[] cleanNames,
            out int[] indices)
        {
            var pathParts = hierarchyPath.Split(separator: '/');

            cleanNames = new string[pathParts.Length];
            indices = new int[pathParts.Length];
            for (var i = 0; i < pathParts.Length; i += 1)
            {
                indices[i] = 0;
                var itemParts = pathParts[i]
                   .Split(separator: '[');
                cleanNames[i] = itemParts[0];

                if (itemParts.Length == 1)
                {
                    continue;
                }

                if (itemParts.Length == 0)
                {
                    var position = 0;
                    for (var j = 0; j < i; j += 1)
                    {
                        position += pathParts[j].Length + 1;
                    }

                    position += itemParts[0].Length;
                    throw new ArgumentException(
                        message:
                        $"Syntax Error at character {position}: Empty object name before '['",
                        paramName: nameof(hierarchyPath));
                }

                if (itemParts.Length > 2)
                {
                    var position = 0;
                    for (var j = 0; j < i; j += 1)
                    {
                        position += pathParts[j].Length + 1;
                    }

                    position += itemParts[0].Length + 1 + itemParts[1].Length;
                    throw new ArgumentException(
                        message: $"Syntax Error at character {position}: Too many brackets",
                        paramName: nameof(hierarchyPath));
                }

                if (itemParts[0].Length == 0)
                {
                    var position = 0;
                    for (var j = 0; j < i; j += 1)
                    {
                        position += pathParts[j].Length + 1;
                    }

                    position += itemParts[0].Length;
                    throw new ArgumentException(
                        message:
                        $"Syntax Error at character {position}: Empty object name before '['",
                        paramName: nameof(hierarchyPath));
                }

                var indexParts = itemParts[1]
                   .Split(separator: ']');
                if (indexParts.Length != 2 || !String.IsNullOrEmpty(value: indexParts[1]))
                {
                    var position = 0;
                    for (var j = 0; j < i; j += 1)
                    {
                        position += pathParts[j].Length + 1;
                    }

                    position += itemParts[0].Length + 1 + indexParts[0].Length;
                    throw new ArgumentException(
                        message:
                        $"Syntax Error at character {position}: malformed brackets in '{pathParts[i]}'",
                        paramName: nameof(hierarchyPath));
                }

                var indexString = indexParts[0];
                var shouldFlip = false;
                if (indexString.StartsWith(value: "^"))
                {
                    shouldFlip = true;
                    indexString = indexString.Substring(startIndex: 1);
                }

                if (!Int32.TryParse(s: indexString, result: out var index))
                {
                    var position = 0;
                    for (var j = 0; j < i; j += 1)
                    {
                        position += pathParts[j].Length + 1;
                    }

                    position += itemParts[0].Length + 1;
                    throw new ArgumentException(
                        message:
                        $"Syntax Error at character {position}: invalid index '{indexString}'. Expected non-negative integer or ^N for backwards indexing.",
                        paramName: nameof(hierarchyPath));
                }

                if (!shouldFlip && index < 0)
                {
                    var position = 0;
                    for (var j = 0; j < i; j += 1)
                    {
                        position += pathParts[j].Length + 1;
                    }

                    position += itemParts[0].Length + 1;

                    throw new ArgumentException(
                        message:
                        $"Syntax Error at character {position}: invalid index '{indexString}'. Expected non-negative integer for indexing.",
                        paramName: nameof(hierarchyPath));
                }

                if (shouldFlip && index <= 0)
                {
                    var position = 0;
                    for (var j = 0; j < i; j += 1)
                    {
                        position += pathParts[j].Length + 1;
                    }

                    position += itemParts[0].Length + 1;

                    throw new ArgumentException(
                        message:
                        $"Syntax Error at character {position}: invalid index '^{indexString}'. Expected ^N (N > 0) for backwards indexing.",
                        paramName: nameof(hierarchyPath));
                }

                indices[i] = shouldFlip
                    ? -index
                    : index;
            }
        }

        #endregion

        #region Object Selection

        private static GameObject PickRootByNameAndIndex(this Scene scene, string name, int index)
        {
            var roots = scene.GetRootGameObjects();
            var count = 0;
            if (index >= 0)
            {
                foreach (var t in roots)
                {
                    if (t.name != name)
                    {
                        continue;
                    }

                    if (count == index)
                    {
                        return t;
                    }

                    count += 1;
                }

                return null;
            }

            var targetFromEnd = -index - 1;
            for (var i = roots.Length - 1; i >= 0; i -= 1)
            {
                if (roots[i].name != name)
                {
                    continue;
                }

                if (count == targetFromEnd)
                {
                    return roots[i];
                }

                count += 1;
            }

            return null;
        }

        private static Transform PickChildByNameAndIndex(this Transform parent, string name,
            int index)
        {
            var count = 0;
            if (index >= 0)
            {
                for (var i = 0; i < parent.childCount; i += 1)
                {
                    var child = parent.GetChild(index: i);
                    if (child.name != name)
                    {
                        continue;
                    }

                    if (count == index)
                    {
                        return child;
                    }

                    count += 1;
                }

                return null;
            }

            var targetFromEnd = -index - 1;
            for (var i = parent.childCount - 1; i >= 0; i -= 1)
            {
                var child = parent.GetChild(index: i);
                if (child.name != name)
                {
                    continue;
                }

                if (count == targetFromEnd)
                {
                    return child;
                }

                count += 1;
            }

            return null;
        }

        private static int GetSameNameIndex(this Transform transform)
        {
            var parent = transform.parent;
            var sameNameIndex = 0;
            // root object
            if (parent == null)
            {
                var currentScene = transform.gameObject.scene;
                var roots = currentScene.GetRootGameObjects();
                for (var i = 0; i < roots.Length; i += 1)
                {
                    var rootCandidate = roots[i];
                    if (rootCandidate.name != transform.name)
                    {
                        continue;
                    }

                    if (ReferenceEquals(objA: rootCandidate.transform, objB: transform))
                    {
                        return sameNameIndex;
                    }

                    sameNameIndex += 1;
                }

                return 0;
            }

            // Find siblings with the same name
            for (int i = 0, n = parent.childCount; i < n; i += 1)
            {
                var child = parent.GetChild(index: i);
                if (child.name != transform.name)
                {
                    continue;
                }

                if (ReferenceEquals(objA: child, objB: transform))
                {
                    return sameNameIndex;
                }

                sameNameIndex += 1;
            }

            return 0; // fallback
        }

        #endregion
    }
}