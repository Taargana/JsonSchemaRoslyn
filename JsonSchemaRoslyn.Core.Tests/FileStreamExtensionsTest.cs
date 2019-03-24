using System.IO;
using System.Text;
using JsonSchemaRoslyn.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonSchemaRoslyn.Core.Tests
{
    [TestClass]
    public class FileStreamExtensionsTest
    {
        [TestMethod]
        public void Utf8WithBom()
        {
            EncodingInfos encodingInfos = new FileStream("./TestJsonFiles/JsonUtf8WithBom.json", FileMode.Open, FileAccess.Read).GetEncoding();
            Assert.IsTrue(encodingInfos.WithBom);
            Assert.AreEqual(Encoding.UTF8, encodingInfos.Encoding);
        }

        [TestMethod]
        public void Utf8NoBom()
        {
            EncodingInfos encodingInfos = new FileStream("./TestJsonFiles/JsonUtf8NoBom.json", FileMode.Open, FileAccess.Read).GetEncoding();
            Assert.IsFalse(encodingInfos.WithBom);
            Assert.AreEqual(Encoding.Default, encodingInfos.Encoding);
        }
    }
}