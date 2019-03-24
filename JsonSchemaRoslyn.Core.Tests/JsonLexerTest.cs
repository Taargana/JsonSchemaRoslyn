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
            //using (JsonLexer jsonLexer = new JsonLexer(new FileInfo("./TestJsonFiles/JsonUtf8WithBom.json")))
            using (JsonLexer jsonLexer = new JsonLexer(new FileInfo(@"C:\Program Files (x86)\Audiokinetic\Wwise 2017.2.9.6726\Authoring\Data\Schemas\WwiseAuthoringAPI.json")))
            {
                SyntaxToken token;
                do
                {
                    tokens.Add(jsonLexer.Lex());
                } while (tokens.Last().Kind != SyntaxKind.EndOfFile);
                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
            }
        }
    }
}