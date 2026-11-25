using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace mk.helpers.tests
{
    [TestClass]
    public class PathHelperTests
    {
        [TestMethod]
        public void IsInside_ChecksWholeFolderNames()
        {
            var assets = Path.Combine(Path.GetTempPath(), "Assets");

            Assert.IsTrue(PathHelper.IsInside(Path.Combine(assets, "a.pix"), assets));
            Assert.IsTrue(PathHelper.IsInside(Path.Combine(assets, "sub", "a.pix"), assets + Path.DirectorySeparatorChar));
            Assert.IsTrue(PathHelper.IsInside(assets, assets));
            Assert.IsFalse(PathHelper.IsInside(assets + "2", assets));
            Assert.IsFalse(PathHelper.IsInside(Path.Combine(assets + "2", "a.pix"), assets));
        }

        [TestMethod]
        public void SafeCombine_ReturnsNullWhenLeavingFolder()
        {
            var assets = Path.Combine(Path.GetTempPath(), "Assets");

            Assert.AreEqual(Path.Combine(assets, "maps", "1.map"), PathHelper.SafeCombine(assets, "maps/1.map"));
            Assert.IsNull(PathHelper.SafeCombine(assets, "../../secret.txt"));
            Assert.IsNull(PathHelper.SafeCombine(assets, Path.GetTempPath()));
        }

        [TestMethod]
        public void FindUp_ReturnsFirstMatchingFolder()
        {
            var root = Path.Combine(Path.GetTempPath(), "mk-findup-" + Path.GetRandomFileName());
            var deep = Path.Combine(root, "a", "b");
            Directory.CreateDirectory(deep);
            File.WriteAllText(Path.Combine(root, "x.slnx"), "");
            try
            {
                Assert.AreEqual(root, PathHelper.FindUp(deep, d => d.GetFiles("*.slnx").Any()));
                Assert.IsNull(PathHelper.FindUp(deep, d => d.Name == "no-such-folder-name"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        [TestMethod]
        public void ToForwardSlashes_ReplacesBackslashes()
        {
            Assert.AreEqual("Assets/maps/1.map", PathHelper.ToForwardSlashes("Assets\\maps\\1.map"));
        }

        [TestMethod]
        public void ShortHash_IsShortAndIgnoresCaseAndTrailingSlash()
        {
            var folder = Path.Combine(Path.GetTempPath(), "MyProject");

            var key = PathHelper.ShortHash(folder);

            Assert.AreEqual(8, key.Length);
            Assert.AreEqual(key, PathHelper.ShortHash(folder.ToLowerInvariant() + Path.DirectorySeparatorChar));
            Assert.AreNotEqual(key, PathHelper.ShortHash(folder + "2"));
        }
    }
}
