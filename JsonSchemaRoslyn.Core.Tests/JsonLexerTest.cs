using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonSchemaRoslyn.Core.Tests
{
    [TestClass]
    public class JsonLexerTest
    {
        [TestMethod]
        public void Constructor_Throws_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new JsonLexer(Stream.Null);
            });
        }
        [TestMethod]
        public void Constructor_Throws_ArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                new JsonLexer(String.Empty);
            });
        }

        [TestMethod]
        public void LexFirstCurlyBracket()
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (JsonLexer jsonLexer = new JsonLexer(new FileInfo(@"C:\Program Files (x86)\Audiokinetic\Wwise 2017.2.0.6500\Authoring\Data\Schemas\WwiseAuthoringAPI.json")))
            {

                tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();

                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
            }
        }

        [TestMethod]
        public void Lex_Property_Colon_String()
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (JsonLexer jsonLexer = new JsonLexer("\"pattern\": \"^\\\\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\\}$\""))
            {

                tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();

                Assert.AreEqual("pattern", tokens[0].Text);
                Assert.AreEqual(":", tokens[1].Text);
                Assert.AreEqual(" ", tokens[2].Text);
                Assert.AreEqual("^\\\\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\\}$", tokens[3].Text);
                Assert.AreEqual(tokens[4].Kind, SyntaxKind.EndOfFile);

                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
            }
        }
    }
}
