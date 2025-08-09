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
        public void TestFindGameObjectAtPath_ValidPath()
        {
            // Create test hierarchy
            var parent = new GameObject(name: "TestParent");
            var child = new GameObject(name: "TestChild");
            child.transform.SetParent(p: parent.transform);

            try
            {
                // Test finding parent
                var foundParent = SceneNavigator.FindGameObjectAtPath(path: "TestParent");
                Assert.AreEqual(expected: parent, actual: foundParent);

                // Test finding child
                var foundChild = SceneNavigator.FindGameObjectAtPath(path: "TestParent/TestChild");
                Assert.AreEqual(expected: child, actual: foundChild);
            }
            finally
            {
                Object.DestroyImmediate(obj: child);
                Object.DestroyImmediate(obj: parent);
            }
        }

        [Test]
        public void TestFindGameObjectAtPath_InvalidPath()
        {
            var result = SceneNavigator.FindGameObjectAtPath(path: "NonExistent/Path");
            Assert.IsNull(anObject: result);
        }

        [Test]
        public void TestFindGameObjectAtPath_NullOrEmptyPath()
        {
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectAtPath(path: null));
            Assert.IsNull(anObject: SceneNavigator.FindGameObjectAtPath(path: ""));
        }

        [Test]
        public void TestFindComponentAtPath_ValidComponent()
        {
            var gameObject = new GameObject(name: "TestComponent");
            var transform = gameObject.transform;

            try
            {
                var foundTransform =
                    SceneNavigator.FindComponentAtPath<Transform>(path: "TestComponent");
                Assert.AreEqual(expected: transform, actual: foundTransform);
            }
            finally
            {
                Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        public void TestFindComponentAtPath_InvalidComponent()
        {
            var gameObject = new GameObject(name: "TestComponent");

            try
            {
                var notFound =
                    SceneNavigator.FindComponentAtPath<Rigidbody>(path: "TestComponent");
                Assert.IsNull(anObject: notFound);
            }
            finally
            {
                Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        public void TestRetrievePath_ValidGameObject()
        {
            var parent = new GameObject(name: "Parent");
            var child = new GameObject(name: "Child");
            child.transform.SetParent(p: parent.transform);

            try
            {
                var parentPath = parent.RetrievePath();
                var childPath = child.RetrievePath();

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
        public void TestRetrievePath_NullGameObject()
        {
            GameObject nullObject = null;
            var result = nullObject.RetrievePath();
            Assert.AreEqual(expected: "[null gameObject]", actual: result);
        }

        [Test]
        public void TestFindGameObjectAtPath_WithIndices()
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
                var foundParent1 = SceneNavigator.FindGameObjectAtPath(path: "Parent");
                Assert.AreEqual(expected: parent1, actual: foundParent1);

                // Test finding second parent with explicit index
                var foundParent2 = SceneNavigator.FindGameObjectAtPath(path: "Parent[1]");
                Assert.AreEqual(expected: parent2, actual: foundParent2);

                // Test finding first child (default index [0])
                var foundChild1 = SceneNavigator.FindGameObjectAtPath(path: "Parent/Child");
                Assert.AreEqual(expected: child1, actual: foundChild1);

                // Test finding second child with explicit index
                var foundChild2 = SceneNavigator.FindGameObjectAtPath(path: "Parent/Child[1]");
                Assert.AreEqual(expected: child2, actual: foundChild2);

                // Test finding second child with explicit parent index
                var foundChild2Alt =
                    SceneNavigator.FindGameObjectAtPath(path: "Parent[0]/Child[1]");
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
        public void TestFindGameObjectAtPath_InvalidIndex()
        {
            var parent = new GameObject(name: "TestParent");

            try
            {
                // Test index out of range
                var result = SceneNavigator.FindGameObjectAtPath(path: "TestParent[5]");
                Assert.IsNull(anObject: result);
            }
            finally
            {
                Object.DestroyImmediate(obj: parent);
            }
        }

        [Test]
        public void TestFindGameObjectAtPath_NegativeIndex()
        {
            // Create multiple objects with the same name
            var obj1 = new GameObject(name: "TestObj");
            var obj2 = new GameObject(name: "TestObj");
            var obj3 = new GameObject(name: "TestObj");

            try
            {
                // Test negative index (from end)
                var lastObj = SceneNavigator.FindGameObjectAtPath(path: "TestObj[^1]");
                Assert.AreEqual(expected: obj3, actual: lastObj);

                var secondToLastObj = SceneNavigator.FindGameObjectAtPath(path: "TestObj[^2]");
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
                var explicitPath = child.RetrievePath();

                // Find an object using that path
                var foundObject = SceneNavigator.FindGameObjectAtPath(path: explicitPath);

                // Should be the same object
                Assert.AreEqual(expected: child, actual: foundObject);
            }
            finally
            {
                Object.DestroyImmediate(obj: child);
                Object.DestroyImmediate(obj: parent);
            }
        }
    }
}