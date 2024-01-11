using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers.Store;
using System;


namespace mk.helpers.tests
{
    [TestClass]
    public class InMemoryStoreTests
    {
        [TestMethod]
        public void Set_WithValue_ShouldAddToStore()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "TestKey";
            string value = "TestValue";

            // Act
            InMemoryStore.Set(key, value);

            // Assert
            Assert.IsTrue(InMemoryStore.Get<string>(key) == value);
        }

        [TestMethod]
        public void Set_WithExpiry_ShouldExpireValue()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "TestKey";
            string value = "TestValue";
            TimeSpan expiry = TimeSpan.FromSeconds(1);

            // Act
            InMemoryStore.Set(key, value, expiry);
            System.Threading.Thread.Sleep(2000); // Wait for the value to expire

            // Assert
            Assert.IsNull(InMemoryStore.Get<string>(key));
        }

        [TestMethod]
        public void Get_WithExistingKey_ShouldReturnValue()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "TestKey";
            string value = "TestValue";
            InMemoryStore.Set(key, value);

            // Act
            var result = InMemoryStore.Get<string>(key);

            // Assert
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Get_WithNonExistingKey_ShouldReturnDefault()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "NonExistingKey1";
            string defaultValue = "DefaultValue";

            // Act
            var result = InMemoryStore.Get<string>(key, defaultValue);

            // Assert
            Assert.AreEqual(defaultValue, result);
        }

        [TestMethod]
        public void Get_WithSetOnNull_ShouldGenerateAndSetValue()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "TestKey";
            string generatedValue = "GeneratedValue";

            // Act
            var result = InMemoryStore.Get<string>(key, () => generatedValue);

            // Assert
            Assert.AreEqual(generatedValue, result);

            var data = InMemoryStore.Get<string>(key);

            Assert.AreEqual(generatedValue, data);
        }

        [TestMethod]
        public void GetExpiry_WithExistingKey_ShouldReturnExpiryTimeSpan()
        {
            InMemoryStore.InvalidateAll();

            // Arrange
            string key = "TestKey";
            string value = "TestValue";
            TimeSpan expiry = TimeSpan.FromSeconds(10);
            InMemoryStore.Set(key, value, expiry);

            // Act
            var result = InMemoryStore.GetExpiry(key);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetExpiry_WithNonExistingKey_ShouldReturnNull()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "NonExistingKey2";

            // Act
            var result = InMemoryStore.GetExpiry(key);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Invalidate_WithExistingKey_ShouldRemoveFromStore()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "TestKey";
            string value = "TestValue";
            InMemoryStore.Set(key, value);

            // Act
            var result = InMemoryStore.Invalidate(key);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(InMemoryStore.Get<string>(key));
        }

        [TestMethod]
        public void Invalidate_WithNonExistingKey_ShouldNotRemoveFromStore()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "NonExistingKey";

            // Act
            var result = InMemoryStore.Invalidate(key);

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Set_WithDecimal_ShouldAddToStore()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "TestKey";
            decimal value = 1.23m;

            // Act
            InMemoryStore.Set(key, value);

            // Assert
            Assert.IsTrue(InMemoryStore.Get<decimal>(key) == value);
        }


        // set get decimal?
        [TestMethod]
        public void Set_WithNullableDecimal_ShouldAddToStore()
        {
            InMemoryStore.InvalidateAll();
            // Arrange
            string key = "TestKey";
            decimal? value = 1.23m;

            // Act
            InMemoryStore.Set(key, value);

            // Assert
            Assert.IsTrue(InMemoryStore.Get<decimal?>(key) == value);
        }
    }
}