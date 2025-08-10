using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DebugUtils.Unity.Tests
{
    /// <summary>
    /// Unit tests for SceneNavigator functionality
    /// </summary>
    public class SceneNavigatorTest
    {
        [Test]
        public void TestFindGameObjectByPath_ValidPath()
        {
            // Create a test hierarchy
            var parent = new GameObject(name: "TestParent");
            var child = new GameObject(name: "TestChild");
            child.transform.SetParent(p: parent.transform);

            try
            {
                // Test finding parent
                var foundParent = SceneNavigator.FindGameObjectByPath(path: "TestParent");
                Assert.AreEqual(expected: parent, actual: foundParent);

                // Test finding child
                var foundChild = SceneNavigator.FindGameObjectByPath(path: "TestParent/TestChild");
                Assert.AreEqual(expected: child, actual: foundChild);
            }
            finally
            {
                Object.DestroyImmediate(obj: child);
                Object.DestroyImmediate(obj: parent);
            }
        }

        [Test]
        public void TestFindGameObjectByPath_InvalidPath()
        {
            var result = SceneNavigator.FindGameObjectByPath(path: "NonExistent/Path");
            Assert.IsNull(anObject: result);
        }

        [Test]
        public void TestFindGameObjectByPath_NullOrEmptyPath()
        {
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: null));
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: ""));
        }

        [Test]
        public void TestFindComponentByPath_ValidComponent()
        {
            var gameObject = new GameObject(name: "TestComponent");
            var transform = gameObject.transform;

            try
            {
                var foundTransform =
                    SceneNavigator.FindComponentByPath<Transform>(path: "TestComponent");
                Assert.AreEqual(expected: transform, actual: foundTransform);
            }
            finally
            {
                Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        public void TestFindComponentByPath_InvalidComponent()
        {
            var gameObject = new GameObject(name: "TestComponent");

            try
            {
                var notFound =
                    SceneNavigator.FindComponentByPath<Rigidbody>(path: "TestComponent");
                Assert.IsNull(anObject: notFound);
            }
            finally
            {
                Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        public void TestGetPath_ValidGameObject()
        {
            var parent = new GameObject(name: "Parent");
            var child = new GameObject(name: "Child");
            child.transform.SetParent(p: parent.transform);

            try
            {
                var parentPath = parent.GetScenePath();
                var childPath = child.GetScenePath();

                Assert.IsTrue(condition: parentPath.Contains(value: "Parent[0]"));
                Assert.IsTrue(condition: childPath.Contains(value: "Parent[0]/Child[0]"));
                Assert.IsTrue(condition: parentPath.Contains(value: ":/"));
                Assert.IsTrue(condition: childPath.Contains(value: ":/"));
            }
            finally
            {
                Object.DestroyImmediate(obj: child);
                Object.DestroyImmediate(obj: parent);
            }
        }

        [Test]
        public void TestGetPath_NullGameObject()
        {
            GameObject nullObject = null;
            var result = nullObject.GetScenePath();
            Assert.AreEqual(expected: "[null gameObject]", actual: result);
        }

        [Test]
        public void TestFindGameObjectByPath_WithIndices()
        {
            // Create a test hierarchy with duplicate names
            var parent1 = new GameObject(name: "Parent");
            var parent2 = new GameObject(name: "Parent");
            var child1 = new GameObject(name: "Child");
            var child2 = new GameObject(name: "Child");
            child1.transform.SetParent(p: parent1.transform);
            child2.transform.SetParent(p: parent1.transform);

            try
            {
                // Test finding first parent (default index [0])
                var foundParent1 = SceneNavigator.FindGameObjectByPath(path: "Parent");
                Assert.AreEqual(expected: parent1, actual: foundParent1);

                // Test finding second parent with explicit index
                var foundParent2 = SceneNavigator.FindGameObjectByPath(path: "Parent[1]");
                Assert.AreEqual(expected: parent2, actual: foundParent2);

                // Test finding first child (default index [0])
                var foundChild1 = SceneNavigator.FindGameObjectByPath(path: "Parent/Child");
                Assert.AreEqual(expected: child1, actual: foundChild1);

                // Test finding second child with explicit index
                var foundChild2 = SceneNavigator.FindGameObjectByPath(path: "Parent/Child[1]");
                Assert.AreEqual(expected: child2, actual: foundChild2);

                // Test finding second child with explicit parent index
                var foundChild2Alt =
                    SceneNavigator.FindGameObjectByPath(path: "Parent[0]/Child[1]");
                Assert.AreEqual(expected: child2, actual: foundChild2Alt);
            }
            finally
            {
                Object.DestroyImmediate(obj: child2);
                Object.DestroyImmediate(obj: child1);
                Object.DestroyImmediate(obj: parent2);
                Object.DestroyImmediate(obj: parent1);
            }
        }

        [Test]
        public void TestFindGameObjectByPath_InvalidIndex()
        {
            var parent = new GameObject(name: "TestParent");

            try
            {
                // Test index out of range
                var result = SceneNavigator.FindGameObjectByPath(path: "TestParent[5]");
                Assert.IsNull(anObject: result);
            }
            finally
            {
                Object.DestroyImmediate(obj: parent);
            }
        }

        [Test]
        public void TestFindGameObjectByPath_NegativeIndex()
        {
            // Create multiple objects with the same name
            var obj1 = new GameObject(name: "TestObj");
            var obj2 = new GameObject(name: "TestObj");
            var obj3 = new GameObject(name: "TestObj");

            try
            {
                // Test negative index (from end)
                var lastObj = SceneNavigator.FindGameObjectByPath(path: "TestObj[^1]");
                Assert.AreEqual(expected: obj3, actual: lastObj);

                var secondToLastObj = SceneNavigator.FindGameObjectByPath(path: "TestObj[^2]");
                Assert.AreEqual(expected: obj2, actual: secondToLastObj);
            }
            finally
            {
                Object.DestroyImmediate(obj: obj3);
                Object.DestroyImmediate(obj: obj2);
                Object.DestroyImmediate(obj: obj1);
            }
        }

        [Test]
        public void TestRoundTripPathFinding()
        {
            // Create a test hierarchy
            var parent = new GameObject(name: "RoundTripParent");
            var child = new GameObject(name: "RoundTripChild");
            child.transform.SetParent(p: parent.transform);

            try
            {
                // Get an explicit path from an object
                var explicitPath = child.GetScenePath();

                // Find an object using that path
                var foundObject = SceneNavigator.FindGameObjectByPath(path: explicitPath);

                // Should be the same object
                Assert.AreEqual(expected: child, actual: foundObject);
            }
            finally
            {
                Object.DestroyImmediate(obj: child);
                Object.DestroyImmediate(obj: parent);
            }
        }

        [Test]
        public void TestFindGameObjectByPath_WithIndices_NullOrEmptyPath()
        {
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: null));
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: ""));
        }

        [Test]
        public void TestRoundTripPathFinding_NullOrEmptyPath()
        {
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: null));
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: ""));
            Assert.IsNull(
                anObject: SceneNavigator.FindGameObjectByPath(
                    path: ((GameObject)null).GetScenePath()));
        }

        [Test]
        public void TestPathParsingErrors_CharacterPositions()
        {
            // Test multiple scene separators
            var ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Scene1:/Object:/Scene2"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(
                condition: ex.Message.Contains(value: "Path contains multiple scene separators"));

            // Test malformed brackets - empty object name
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "[0]/Object"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Empty object name before '['"));

            // Test too many brackets
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[0][1]"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Too many brackets"));

            // Test malformed brackets - missing opening bracket
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object0]/Child"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Reserved character"));

            // Test malformed brackets - missing closing bracket
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[0/Child"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Malformed brackets"));

            // Test invalid index - non-numeric
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[abc]"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Invalid index"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Expected non-negative integer"));

            // Test invalid index - negative
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[-1]"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Invalid index"));

            // Test invalid backwards index - ^0
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[^0]"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Syntax Error at character"));
            Assert.IsTrue(condition: ex.Message.Contains(value: "Invalid index"));
        }

        [Test]
        public void TestPathParsingErrors_ExactCharacterPositions()
        {
            var ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Scene1:/Object:/Scene2"));
            var characterPosition = Int32.Parse(s: ex.Message
                                                     .Split(separator: "\n")[2]
                                                     .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 14, actual: characterPosition);

            // Test malformed brackets - empty object name
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "[0]/Object"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 0, actual: characterPosition);

            // Test too many brackets
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[0][1]"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 9, actual: characterPosition);

            // Test malformed bracket - missing opening bracket
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object0]/Child"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 7, actual: characterPosition);

            // Test malformed brackets - missing closing bracket
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[0/Child"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 8, actual: characterPosition);

            // Test invalid index - non-numeric
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[abc]"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 7, actual: characterPosition);

            // Test invalid index - negative
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[-1]"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 7, actual: characterPosition);

            // Test invalid backwards index - ^0
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[^0]"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 7, actual: characterPosition);

            // Test invalid backwards index - ^-1
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Object[^-1]"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 7, actual: characterPosition);

            // Test character position calculation for nested errors
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Parent/Child[xyz]/GrandChild"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 13, actual: characterPosition);

            // Test position for empty object name in middle of path
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "Parent/[2]"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 7, actual: characterPosition);

            // Test position for too many brackets in middle
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "A/B[1][2]/C"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 6, actual: characterPosition);

            // Test position for too many brackets in middle
            ex = Assert.Throws<ArgumentException>(code: () =>
                SceneNavigator.FindGameObjectByPath(path: "A/B[1][234]/C"));
            characterPosition = Int32.Parse(s: ex.Message
                                                 .Split(separator: "\n")[2]
                                                 .Split(separator: " ")[4][..^1]);
            Assert.AreEqual(expected: 6, actual: characterPosition);
        }

        [Test]
        public void TestSceneNavigator_FindGameObjectByPath()
        {
            // Create a test hierarchy: Parent -> Child -> GrandChild
            var parent = new GameObject(name: "TestParent");
            var child = new GameObject(name: "TestChild");
            var grandChild = new GameObject(name: "TestGrandChild");

            child.transform.SetParent(p: parent.transform);
            grandChild.transform.SetParent(p: child.transform);

            try
            {
                // Test finding objects at different levels
                var foundParent = SceneNavigator.FindGameObjectByPath(path: "TestParent");
                var foundChild = SceneNavigator.FindGameObjectByPath(path: "TestParent/TestChild");
                var foundGrandChild =
                    SceneNavigator.FindGameObjectByPath(
                        path: "TestParent/TestChild/TestGrandChild");

                Assert.AreEqual(expected: parent, actual: foundParent);
                Assert.AreEqual(expected: child, actual: foundChild);
                Assert.AreEqual(expected: grandChild, actual: foundGrandChild);

                // Test with non-existent path
                var notFound = SceneNavigator.FindGameObjectByPath(path: "TestParent/NonExistent");
                Assert.IsNull(anObject: notFound);

                // Test with null/empty path
                Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: null));
                Assert.IsNull(anObject: SceneNavigator.FindGameObjectByPath(path: ""));
            }
            finally
            {
                Object.DestroyImmediate(obj: grandChild);
                Object.DestroyImmediate(obj: child);
                Object.DestroyImmediate(obj: parent);
            }
        }

        [Test]
        public void TestSceneNavigator_FindComponentByPath()
        {
            var gameObject = new GameObject(name: "TestComponent");
            var testTransform = gameObject.transform; // Transform is always present

            try
            {
                var foundTransform =
                    SceneNavigator.FindComponentByPath<Transform>(path: "TestComponent");
                Assert.AreEqual(expected: testTransform, actual: foundTransform);

                // Test with non-existent component
                var notFound =
                    SceneNavigator.FindComponentByPath<Rigidbody>(path: "TestComponent");
                Assert.IsNull(anObject: notFound);

                // Test with non-existent path
                var pathNotFound =
                    SceneNavigator.FindComponentByPath<Transform>(path: "NonExistent");
                Assert.IsNull(anObject: pathNotFound);
            }
            finally
            {
                Object.DestroyImmediate(obj: gameObject);
            }
        }
    }
}