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
    /// For production systems, prefer component references, FindObjectOfType, or tags.
    /// </remarks>
    public static class SceneNavigator
    {
        /// <summary>
        /// Finds a GameObject in the scene hierarchy using a structured path with scene and index information.
        /// </summary>
        /// <param name="path">
        /// Structured path in the format "SceneName:/GameObject[index]/Child[index]".
        /// Scene name is optional (defaults to active scene if omitted).
        /// Index is optional (defaults to first match [0] if omitted).
        /// </param>
        /// <returns>
        /// The GameObject at the specified path, or null if not found.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides flexible object finding with optional explicit indexing:
        /// - Without indices: Finds first matching object at each level
        /// - With indices: Finds specific object by position among same-named siblings
        /// - Mixed usage: Can mix indexed and non-indexed segments in same path
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
        /// // Simple paths - find first match at each level
        /// // This works if that scene is loaded.
        /// var player = SceneNavigator.FindGameObjectAtPath("Player");                    // First "Player" in active scene
        /// var weapon = SceneNavigator.FindGameObjectAtPath("Player/Equipment/Weapon");   // First weapon found
        ///
        /// 
        /// // Explicit scene, auto-index objects
        /// var enemy = SceneNavigator.FindGameObjectAtPath("BattleScene:/Enemies/Goblin"); // First Goblin in BattleScene
        ///
        /// 
        /// // Explicit indices when you need specific objects
        /// var secondEnemy = SceneNavigator.FindGameObjectAtPath("Enemies/Goblin[1]");     // Second Goblin specifically
        /// var lastButton = SceneNavigator.FindGameObjectAtPath("UI/Menu/Button[^1]");     // Last button
        ///
        /// 
        /// // Mixed usage - some indexed, some not
        /// var specificWeapon = SceneNavigator.FindGameObjectAtPath("Player/Equipment[0]/Weapon[2]"); // First equipment slot, third weapon
        ///
        /// 
        /// // Multi-scene with explicit indices
        /// var specificUI = SceneNavigator.FindGameObjectAtPath("UIScene:/Canvas[0]/Panel[1]/Button[0]");
        ///
        /// 
        /// // Exploratory debugging - just find first matches
        /// var someObject = SceneNavigator.FindGameObjectAtPath("Level/SomeGroup/SomeObject"); // Quick and easy
        ///
        /// 
        /// // Handle not found
        /// var missing = SceneNavigator.FindGameObjectAtPath("NonExistent/Path");
        /// if (missing == null)
        /// {
        ///     Debug.LogWarning("Object not found - try using explicit indices or check hierarchy");
        /// }
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">
        /// Thrown when path format is invalid (malformed brackets, invalid indices, etc.).
        /// </exception>
        /// <seealso cref="FindComponentAtPath{T}(string)"/>
        /// <seealso cref="RetrievePath(GameObject)"/>
        public static GameObject FindGameObjectAtPath(string path)
        {
            if (String.IsNullOrEmpty(value: path))
            {
                return null;
            }

            Scene currentScene;
            string hierarchyPath;
            var sep = path.IndexOf(value: ":/", comparisonType: StringComparison.Ordinal);
            if (sep >= 0)
            {
                // Check for multiple separators
                if (path.IndexOf(value: ":/", startIndex: sep + 2,
                        comparisonType: StringComparison.Ordinal) >= 0)
                {
                    throw new ArgumentException(
                        message: "Invalid path format: too many scene separators",
                        paramName: nameof(path));
                }

                var sceneName = path[..sep];
                hierarchyPath = path[(sep + 2)..];
                currentScene = SceneManager.GetSceneByName(name: sceneName);

                if (!currentScene.IsValid())
                {
                    #if UNITY_EDITOR
                    Debug.LogWarning(
                        message: $"Scene '{sceneName}' not found - using active scene instead");
                    #endif
                    return null;
                }
            }
            else
            {
                currentScene = SceneManager.GetActiveScene();
                hierarchyPath = path;
            }

            var pathParts = hierarchyPath.Split(separator: '/');

            var cleanNames = new string[pathParts.Length];
            var indices = new int[pathParts.Length];
            for (var i = 0; i < pathParts.Length; i++)
            {
                indices[i] = 0;
                var itemParts = pathParts[i]
                   .Split(separator: '[');
                cleanNames[i] = itemParts[0];

                if (itemParts.Length == 1)
                {
                    continue;
                }

                if (itemParts.Length != 2)
                {
                    throw new ArgumentException(message: "Invalid path format",
                        paramName: nameof(path));
                }

                var indexParts = itemParts[1]
                   .Split(separator: ']');
                if (indexParts.Length != 2 || !String.IsNullOrEmpty(value: indexParts[1]))
                {
                    throw new ArgumentException(
                        message: $"Invalid path format: malformed brackets in '{pathParts[i]}'",
                        paramName: nameof(path));
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
                    throw new ArgumentException(
                        message:
                        $"Invalid path format: invalid index '{indexString}' in '{pathParts[i]}'",
                        paramName: nameof(path));
                }

                indices[i] = shouldFlip
                    ? -index
                    : index;
            }

            // Select the root object by index
            var rootObject =
                currentScene.PickRootByNameAndIndex(name: cleanNames[0], index: indices[0]);

            if (rootObject == null)
            {
                return rootObject;
            }

            // Navigate through the path
            var current = rootObject.transform;
            for (var i = 1; i < pathParts.Length; i++)
            {
                current = current.PickChildByNameAndIndex(name: cleanNames[i], index: indices[i]);
                if (current == null)
                {
                    return null;
                }
            }

            return current.gameObject;
        }

        private static GameObject PickRootByNameAndIndex(this Scene scene, string name, int index)
        {
            var roots = scene.GetRootGameObjects();
            var count = 0;
            if (index >= 0)
            {
                for (var i = 0; i < roots.Length; i++)
                {
                    if (roots[i].name != name)
                    {
                        continue;
                    }

                    if (count == index)
                    {
                        return roots[i];
                    }

                    count += 1;
                }

                return null;
            }

            var targetFromEnd = -index - 1;
            for (var i = roots.Length - 1; i >= 0; i--)
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
                for (var i = 0; i < parent.childCount; i++)
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
            for (var i = parent.childCount - 1; i >= 0; i--)
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

        /// <summary>
        /// Finds a GameObject using a flexible path format and retrieves a component of the specified type.
        /// Combines object finding and component retrieval in a single operation.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve (must inherit from Component).</typeparam>
        /// <param name="path">
        /// Flexible path in the format "SceneName:/GameObject/Child" or "SceneName:/GameObject[index]/Child[index]".
        /// Scene name is optional (defaults to the active scene if omitted).
        /// Indices are optional (defaults to the first match [0] if omitted).
        /// Same format as FindGameObjectAtPath.
        /// </param>
        /// <returns>
        /// The component of type T if found on the GameObject at the specified path, or null if not found.
        /// Returns null if either the GameObject is not found or the component is not attached.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method combines FindGameObjectAtPath() with GetComponent&lt;T&gt;() For convenient component access.
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
        /// Button playBtn = SceneNavigator.FindComponentAtPath&lt;Button&gt;("UI/MainMenu/PlayButton");
        /// Rigidbody playerRB = SceneNavigator.FindComponentAtPath&lt;Rigidbody&gt;("Player");
        ///
        /// 
        /// // Explicit scene specification
        /// AudioSource bgMusic = SceneNavigator.FindComponentAtPath&lt;AudioSource&gt;("AudioScene:/BackgroundMusic");
        ///
        /// 
        /// // Explicit indexing when you need specific instances
        /// Text firstScore = SceneNavigator.FindComponentAtPath&lt;Text&gt;("UI/Scoreboard/PlayerScore[0]");
        /// Text secondScore = SceneNavigator.FindComponentAtPath&lt;Text&gt;("UI/Scoreboard/PlayerScore[1]");
        ///
        /// 
        /// // Mixed usage - some indexed, some auto-index
        /// Slider volumeSlider = SceneNavigator.FindComponentAtPath&lt;Slider&gt;("Settings/Audio[0]/VolumeSlider");
        ///
        /// 
        /// // Safe usage with null checking
        /// var healthBar = SceneNavigator.FindComponentAtPath&lt;Slider&gt;("UI/HUD/HealthBar");
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
        /// Rigidbody uiRigidbody = SceneNavigator.FindComponentAtPath&lt;Rigidbody&gt;("UI/Canvas");
        /// // uiRigidbody will be null - UI objects typically don't have Rigidbody components
        ///
        /// 
        /// // Complex nested component finding
        /// WeaponController weaponCtrl = SceneNavigator.FindComponentAtPath&lt;WeaponController&gt;("Player/Equipment[0]/Weapon[2]");
        ///
        /// 
        /// // Use with explicit paths from RetrievePath for reliable access
        /// string weaponPath = someWeapon.RetrievePath(); // "GameScene:/Player[0]/Equipment[0]/Sword[1]"
        /// WeaponController sameWeapon = SceneNavigator.FindComponentAtPath&lt;WeaponController&gt;(weaponPath);
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">
        /// Thrown when path format is invalid (malformed brackets, invalid indices, etc.).
        /// Same validation rules as FindGameObjectAtPath.
        /// </exception>
        /// <seealso cref="FindGameObjectAtPath(string)"/>
        /// <seealso cref="RetrievePath(GameObject)"/>
        /// <seealso cref="UnityEngine.Component.GetComponent{T}()"/>
        public static T FindComponentAtPath<T>(string path) where T : Component
        {
            var obj = FindGameObjectAtPath(path: path);
            if (obj != null)
            {
                return obj.GetComponent<T>();
            }

            return null;
        }

        /// <summary>
        /// Retrieves the complete explicit path of a GameObject including scene name and exact sibling indices.
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
        /// <item>Round-trip safe: Path can be used with <c>FindGameObjectAtPath()</c> to find the same object</item>
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
        /// string playerPath = player.RetrievePath();
        /// Debug.Log(playerPath); // Output: "GameScene:/Player[0]" (always explicit)
        ///
        /// 
        /// // Even simple objects get full paths
        /// GameObject canvas = GameObject.Find("Canvas");
        /// string canvasPath = canvas.RetrievePath();
        /// Debug.Log(canvasPath); // Output: "UIScene:/Canvas[0]" (not just "Canvas")
        ///
        /// 
        /// // Nested objects show complete explicit hierarchy
        /// Transform weapon = player.transform.Find("Equipment/Weapon");
        /// string weaponPath = weapon.gameObject.RetrievePath();
        /// Debug.Log(weaponPath); // Output: "GameScene:/Player[0]/Equipment[0]/Weapon[1]"
        ///
        /// 
        /// // Perfect for collision debugging - get exact object identity
        /// void OnTriggerEnter(Collider other)
        /// {
        ///     string colliderPath = other.gameObject.RetrievePath();
        ///     Debug.Log($"Collision with: {colliderPath}");
        ///     // Output: "BattleScene:/Enemies[0]/Goblin[2]/Weapon[0]" - know exactly which Goblin!
        /// }
        ///
        /// 
        /// // Round-trip reliability - explicit path works with FindGameObjectAtPath
        /// string explicitPath = myGameObject.RetrievePath();
        /// GameObject foundObject = SceneNavigator.FindGameObjectAtPath(explicitPath);
        /// Debug.Assert(foundObject == myGameObject); // Always true with explicit paths
        ///
        /// 
        /// // Use explicit paths for reliable object tracking
        /// Dictionary&lt;string, float&gt; objectHealths = new Dictionary&lt;string, float&gt;();
        /// foreach (var enemy in enemies)
        /// {
        ///     string enemyPath = enemy.RetrievePath(); // Unique identifier
        ///     objectHealths[enemyPath] = enemy.GetComponent&lt;Health&gt;().currentHealth;
        /// }
        ///
        /// 
        /// // Handle edge cases
        /// GameObject nullObj = null;
        /// string nullPath = nullObj.RetrievePath(); // Returns "[null gameObject]"
        /// </code>
        /// </example>
        /// <seealso cref="FindGameObjectAtPath(string)"/>
        /// <seealso cref="FindComponentAtPath{T}(string)"/>
        public static string RetrievePath(this GameObject obj)
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
            for (var t = obj.transform; t != null; t = t.parent, k--)
            {
                names[k] = t.name;
                indices[k] = t.GetSameNameIndex();
            }

            // Pass 3: Build string with optimal capacity
            var scene = obj.scene;
            var sceneName = scene.IsValid()
                ? scene.name
                : "[invalid Scene]";
            var cap = sceneName.Length + 2;
            for (var i = 0; i < depth; i++)
            {
                // [000]/ overhead. This is rough guessing to avoid growth of StringBuilder.
                cap += names[i].Length + 6;
            }

            var sb = new StringBuilder(capacity: cap);
            sb.Append(value: sceneName)
              .Append(value: ":/");

            for (var i = 0; i < depth; i++)
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
        private static int GetSameNameIndex(this Transform transform)
        {
            var parent = transform.parent;
            var sameNameIndex = 0;
            // root object
            if (parent == null)
            {
                var currentScene = transform.gameObject.scene;
                var roots = currentScene.GetRootGameObjects();
                for (var i = 0; i < roots.Length; i++)
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
            for (int i = 0, n = parent.childCount; i < n; i++)
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
    }
}