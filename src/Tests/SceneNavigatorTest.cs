using NUnit.Framework;
using UnityEngine;

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
                anObject: SceneNavigator.FindGameObjectByPath(((GameObject)null).GetScenePath()));
        }

        [Test]
        public void TestPathParsingErrors_CharacterPositions()
        {
            // Test multiple scene separators
            var ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Scene1:/Object:/Scene2"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character"));
            Assert.That(ex.Message, Does.Contain("multiple scene separators"));

            // Test malformed brackets - empty object name
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("[0]/Object"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 0"));
            Assert.That(ex.Message, Does.Contain("Empty object name before '['"));

            // Test too many brackets
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[0][1]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character"));
            Assert.That(ex.Message, Does.Contain("Too many brackets"));

            // Test malformed brackets - missing closing bracket
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[0/Child"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character"));
            Assert.That(ex.Message, Does.Contain("malformed brackets"));

            // Test invalid index - non-numeric
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[abc]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character"));
            Assert.That(ex.Message, Does.Contain("invalid index 'abc'"));
            Assert.That(ex.Message, Does.Contain("Expected non-negative integer"));

            // Test invalid index - negative
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[-1]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character"));
            Assert.That(ex.Message, Does.Contain("invalid index '-1'"));

            // Test invalid backwards index - ^0
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[^0]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character"));
            Assert.That(ex.Message, Does.Contain("invalid index '^0'"));
        }

        [Test]
        public void TestPathParsingErrors_ExactCharacterPositions()
        {
            var ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Scene1:/Object:/Scene2"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 14"));

            // Test malformed brackets - empty object name
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("[0]/Object"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 0"));

            // Test too many brackets
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[0][1]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 9"));
            // Test malformed brackets - missing closing bracket
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[0/Child"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 8"));

            // Test invalid index - non-numeric
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[abc]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 7"));

            // Test invalid index - negative
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[-1]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 7"));

            // Test invalid backwards index - ^0
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[^0]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 7"));
            
            // Test invalid backwards index - ^-1
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Object[^-1]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 7"));
            Assert.That(ex.Message, Does.Contain("invalid index '^-1'"));
            
            // Test character position calculation for nested errors
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Parent/Child[xyz]/GrandChild"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 13"));
            Assert.That(ex.Message, Does.Contain("invalid index 'xyz'"));

            // Test position for empty object name in middle of path
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("Parent/[2]"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 7"));
            Assert.That(ex.Message, Does.Contain("Empty object name before '['"));

            // Test position for too many brackets in middle
            ex = Assert.Throws<System.ArgumentException>(() =>
                SceneNavigator.FindGameObjectByPath("A/B[1][2]/C"));
            Assert.That(ex.Message, Does.Contain("Syntax Error at character 6"));
            Assert.That(ex.Message, Does.Contain("Too many brackets"));
        }
    }
}