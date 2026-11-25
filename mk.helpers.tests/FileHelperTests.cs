using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace mk.helpers.tests
{
    [TestClass]
    public class FileHelperTests
    {
        [TestMethod]
        public void WriteAllTextAtomic_CreatesThenReplaces()
        {
            var path = Path.Combine(Path.GetTempPath(), "mk-atomic-" + Path.GetRandomFileName() + ".json");
            try
            {
                FileHelper.WriteAllTextAtomic(path, "first");
                Assert.AreEqual("first", File.ReadAllText(path));

                FileHelper.WriteAllTextAtomic(path, "second");
                Assert.AreEqual("second", File.ReadAllText(path));
                Assert.IsFalse(File.Exists(path + ".tmp"));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void WriteAllTextAtomic_CreatesMissingFolder()
        {
            var folder = Path.Combine(Path.GetTempPath(), "mk-atomic-" + Path.GetRandomFileName());
            var path = Path.Combine(folder, "sub", "settings.json");
            try
            {
                FileHelper.WriteAllTextAtomic(path, "x");
                Assert.AreEqual("x", File.ReadAllText(path));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void ReadAllTextWithRetry_WaitsForLock()
        {
            var path = Path.Combine(Path.GetTempPath(), "mk-retry-" + Path.GetRandomFileName());
            File.WriteAllText(path, "hello");
            try
            {
                var locked = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                System.Threading.Tasks.Task.Delay(150).ContinueWith(_ => locked.Dispose());

                Assert.AreEqual("hello", FileHelper.ReadAllTextWithRetry(path, 10, 50));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void ReadAllTextWithRetry_MissingFileFailsAtOnce()
        {
            Assert.ThrowsException<FileNotFoundException>(() => FileHelper.ReadAllTextWithRetry(Path.Combine(Path.GetTempPath(), "mk-missing-" + Path.GetRandomFileName())));
        }
    }
}
