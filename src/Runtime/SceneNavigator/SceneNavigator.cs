using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugUtils.Unity.SceneNavigator
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
        /// Finds a GameObject in the scene hierarchy based on a provided path.
        /// Only searches in the currently active scene. This is a debugging utility
        /// intended for development and emergency fixes.
        /// </summary>
        /// <param name="path">
        /// Slash-separated path string starting from root GameObject (e.g., "Canvas/MainMenu/PlayButton").
        /// Path matching is case-sensitive and must exactly match GameObject names.
        /// Returns null if path is null, empty, or if any part of the path doesn't exist.
        /// </param>
        /// <returns>
        /// The GameObject at the specified path if found; null if path is invalid, 
        /// empty, or any object in the path doesn't exist.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is intended for debugging and development use only.
        /// It trades type safety and performance for convenience during development.
        /// </para>
        /// <para>
        /// WARNING: This method is inherently fragile - any hierarchy changes will break
        /// the paths. Use only for debugging, testing, or emergency fixes during development.
        /// </para>
        /// <para>
        /// For production code, use proper component references, FindObjectOfType&lt;T&gt;(), 
        /// or GameObject.FindWithTag() instead.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Basic usage - find UI elements for debugging
        /// var button = SceneNavigator.FindGameObjectAtPath("Canvas/MainMenu/PlayButton");
        /// if (button != null)
        /// {
        ///     Debug.Log($"Found button: {button.name}");
        /// }
        /// else
        /// {
        ///     Debug.LogWarning("Button not found - check hierarchy path");
        /// }
        /// 
        /// // Emergency fix during development
        /// var brokenUIPanel = SceneNavigator.FindGameObjectAtPath("UI/BrokenPanel");
        /// if (brokenUIPanel != null)
        /// {
        ///     brokenUIPanel.SetActive(false); // Quick fix for demo
        ///     Debug.LogWarning("EMERGENCY FIX: Disabled broken panel - fix properly later!");
        /// }
        /// 
        /// // Debug complex hierarchies
        /// var enemyBoss = SceneNavigator.FindGameObjectAtPath("Level/Enemies/Boss/BossController");
        /// Debug.Log($"Boss found: {enemyBoss != null}");
        /// 
        /// // Returns null for invalid paths:
        /// var missing = SceneNavigator.FindGameObjectAtPath("Does/Not/Exist");      // null
        /// var empty = SceneNavigator.FindGameObjectAtPath("");                     // null  
        /// var nullPath = SceneNavigator.FindGameObjectAtPath(null);               // null
        /// </code>
        /// </example>
        /// <seealso cref="FindComponentAtPath{T}(string)"/>
        /// <seealso cref="RetrievePath(GameObject)"/>
        /// <seealso cref="UnityEngine.GameObject.Find(string)"/>
        /// <seealso cref="UnityEngine.GameObject.FindWithTag(string)"/>
        public static GameObject FindGameObjectAtPath(string path)
        {
            if (String.IsNullOrEmpty(value: path))
            {
                return null;
            }

            var pathParts = path.Split(separator: '/');

            // Find the root object
            GameObject rootObject = null;
            foreach (var root in SceneManager.GetActiveScene()
                                             .GetRootGameObjects())
            {
                if (root.name == pathParts[0])
                {
                    rootObject = root;
                    break;
                }
            }

            if (rootObject == null)
            {
                return rootObject;
            }

            // Navigate through the path
            var current = rootObject.transform;
            for (var i = 1; i < pathParts.Length; i++)
            {
                var child = current.Find(n: pathParts[i]);
                if (child == null)
                {
                    return null;
                }

                current = child;
            }

            return current.gameObject;
        }

        /// <summary>
        /// Retrieves a component of a specified type from a GameObject located at the given path
        /// in the scene hierarchy. This is a debugging utility that combines path finding with
        /// component retrieval in a single call.
        /// </summary>
        /// <typeparam name="T">
        /// The type of component to retrieve. Must inherit from UnityEngine.Component.
        /// </typeparam>
        /// <param name="path">
        /// Slash-separated path string starting from root GameObject (e.g., "Canvas/MainMenu/PlayButton").
        /// Path matching is case-sensitive and must exactly match GameObject names.
        /// </param>
        /// <returns>
        /// The component of type T attached to the GameObject at the specified path.
        /// Returns null if the path is invalid, the GameObject doesn't exist, or the component is not found.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is intended for debugging and development use only.
        /// It's equivalent to calling FindGameObjectAtPath() followed by GetComponent&lt;T&gt;().
        /// </para>
        /// <para>
        /// WARNING: This method is inherently fragile and should not be used in production code.
        /// Any hierarchy changes or component removal will cause this to return null.
        /// </para>
        /// <para>
        /// For production code, use proper component references or FindObjectOfType&lt;T&gt;() instead.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Find and get component in one call - great for debugging
        /// var button = SceneNavigator.FindComponentAtPath&lt;Button&gt;("Canvas/MainMenu/PlayButton");
        /// if (button != null)
        /// {
        ///     button.onClick.AddListener(() =&gt; Debug.Log("Button clicked!"));
        /// }
        /// 
        /// // Debug UI elements quickly
        /// var statusText = SceneNavigator.FindComponentAtPath&lt;Text&gt;("HUD/StatusPanel/StatusText");
        /// if (statusText != null)
        /// {
        ///     statusText.text = "DEBUG: Found status text component";
        /// }
        /// 
        /// // Emergency component access during development
        /// var playerHealth = SceneNavigator.FindComponentAtPath&lt;PlayerHealth&gt;("Player/HealthSystem");
        /// if (playerHealth != null)
        /// {
        ///     Debug.Log($"Player health: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}");
        /// }
        /// 
        /// // Returns null when component doesn't exist
        /// var missingComponent = SceneNavigator.FindComponentAtPath&lt;Rigidbody&gt;("Canvas/UIElement"); // null - UI doesn't have Rigidbody
        /// var wrongPath = SceneNavigator.FindComponentAtPath&lt;Button&gt;("Wrong/Path");                // null - path doesn't exist
        /// </code>
        /// </example>
        /// <seealso cref="FindGameObjectAtPath(string)"/>
        /// <seealso cref="RetrievePath(GameObject)"/>
        /// <seealso cref="UnityEngine.Component.GetComponent{T}()"/>
        /// <seealso cref="UnityEngine.Object.FindObjectOfType{T}()"/>
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
        /// Retrieves the full hierarchy path of a GameObject within the scene as a slash-separated string.
        /// This is extremely useful for debugging object locations and logging hierarchy information.
        /// </summary>
        /// <param name="obj">
        /// The GameObject whose path should be retrieved. Can be null.
        /// </param>
        /// <returns>
        /// A string representing the GameObject's full path within the scene hierarchy, 
        /// prefixed with the scene name (e.g., "SampleScene/Canvas/MainMenu/PlayButton").
        /// Returns "[null gameObject]" if obj is null.
        /// Returns a path with the "[invalid scene]/" prefix if the GameObject's scene is invalid.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is perfect for debugging - it tells you exactly where an object
        /// is located in the scene hierarchy, which is invaluable for debugging complex scenes.
        /// </para>
        /// <para>
        /// The returned path can be used with FindGameObjectAtPath() to locate the same object,
        /// making this useful for generating debug information and logs.
        /// </para>
        /// <para>
        /// This method handles edge cases gracefully and never throws exceptions.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Debug object locations during collision events
        /// void OnTriggerEnter(Collider other)
        /// {
        ///     string otherPath = other.gameObject.RetrievePath();
        ///     Debug.Log($"Collision with object at: {otherPath}");
        ///     // Output: "SampleScene/Level/Player/PlayerController"
        /// }
        /// 
        /// // Log current object hierarchy for debugging
        /// void DebugCurrentLocation()
        /// {
        ///     string myPath = this.gameObject.RetrievePath();
        ///     Debug.Log($"I am located at: {myPath}");
        ///     // Output: "SampleScene/Canvas/MainMenu/SettingsButton"
        /// }
        /// 
        /// // Generate debug info for all objects with a component
        /// void DebugAllEnemies()
        /// {
        ///     var enemies = FindObjectsOfType&lt;EnemyController&gt;();
        ///     foreach (var enemy in enemies)
        ///     {
        ///         Debug.Log($"Enemy at: {enemy.gameObject.RetrievePath()}");
        ///     }
        ///     // Output: "BattleScene/Level/Enemies/Goblin_01"
        ///     //         "BattleScene/Level/Enemies/Orc_03"
        /// }
        /// 
        /// // Handles edge cases gracefully
        /// GameObject nullObject = null;
        /// Debug.Log(nullObject.RetrievePath()); // Output: "[null gameObject]"
        /// 
        /// // Use with FindGameObjectAtPath for round-trip debugging
        /// string originalPath = myObject.RetrievePath();
        /// GameObject foundObject = SceneNavigator.FindGameObjectAtPath(originalPath.Split('/')[1..].Join("/"));
        /// Debug.Assert(foundObject == myObject); // Should be the same object
        /// </code>
        /// </example>
        /// <seealso cref="FindGameObjectAtPath(string)"/>
        /// <seealso cref="FindComponentAtPath{T}(string)"/>
        /// <seealso cref="UnityEngine.GameObject.name"/>
        /// <seealso cref="UnityEngine.SceneManagement.Scene.name"/>
        public static string RetrievePath(this GameObject obj)
        {
            if (obj == null)
            {
                return "[null gameObject]";
            }

            var pathParts = new List<string>();
            var current = obj.transform;

            while (current != null)
            {
                pathParts.Add(item: current.name);
                current = current.parent;
            }

            pathParts.Reverse();

            var sceneName = obj.scene.IsValid()
                ? obj.scene.name
                : "[invalid scene]";
            return $"{sceneName}/{String.Join(separator: "/", values: pathParts)}";
        }
    }
}