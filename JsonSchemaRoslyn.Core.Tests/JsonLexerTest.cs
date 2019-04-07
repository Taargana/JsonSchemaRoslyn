using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JsonSchemaRoslyn.Core.Exceptions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
                new JsonLexer((string)null);
            });
        }

        [TestMethod]
        public void LexFirstCurlyBracket()
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (JsonLexer jsonLexer = new JsonLexer(new FileInfo(@"C:\Program Files (x86)\Audiokinetic\Wwise 2018.1.6.6858\Authoring\Data\Schemas\WwiseAuthoringAPI.json")))
            {

                tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();
                Assert.IsTrue(!jsonLexer.Diagnostics.Any());
                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
                Assert.IsTrue(elapsed < TimeSpan.FromMilliseconds(250));
            }
        }

        [TestMethod]
        public void Lex_Property_Colon_String()
        {
            using (JsonLexer jsonLexer = new JsonLexer("\"pattern\": \"^\\\\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\\}$\""))
            {
                List<SyntaxToken> tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();

                Assert.AreEqual("pattern", tokens[0].Text);
                Assert.AreEqual(":", tokens[1].Text);
                Assert.AreEqual(" ", tokens[2].Text);
                Assert.AreEqual("^\\\\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\\}$", tokens[3].Text);
                Assert.AreEqual(tokens[4].Kind, SyntaxKind.EndOfFile);
            }
        }

        [TestMethod]
        [DataRow("\"pattern\": \"^\\\\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\\}$")]
        public void Lex_Property_Colon_String_Diagnostics_MissingEndOfString(string testCase)
        {
            using (JsonLexer jsonLexer = new JsonLexer(testCase))
            {
                List<SyntaxToken> tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();
                Assert.IsTrue(jsonLexer.Diagnostics.Any());
                Assert.IsTrue(jsonLexer.Diagnostics.All(d=>d.Exception is EndOfFileExtractLiteralException));

            }
        }

        [TestMethod]
        [DataRow("\"pattern: \"^\\\\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\\}$\"")] // excepted is ^ unknown character because of the missing " after pattern and before :
                                                                                                                          // endoffile exception also expected because the ": \"" qotue after whitespace is interpreted like the end of the sentence
        public void Lex_Property_Colon_String_Diagnostics_UnknownCharacter(string testCase)
        {
            using (JsonLexer jsonLexer = new JsonLexer(testCase))
            {
                List<SyntaxToken> tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();
                Assert.IsTrue(jsonLexer.Diagnostics.Any());
                Assert.IsTrue(jsonLexer.Diagnostics.First().Exception is UnknownCharacterException);
                Assert.IsTrue(jsonLexer.Diagnostics.Last().Exception is EndOfFileExtractLiteralException);

            }
        }

        [TestMethod]
        public void Lex_Property_Colon_String_Diagnostics_WhiteSpaceAndEndOfFile2()
        {
            string testCase =
@"""propertyName"": {
    ""type"": ""string"",
    ""pattern"": ""^[a-zA-Z0-9 _]+$""
},
""objectPathArg"": {
    ""type"": ""string"",
    ""pattern"": ""^\\\\""
}";
            using (JsonLexer jsonLexer = new JsonLexer(testCase))
            {
                List<SyntaxToken> tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();
                Assert.IsTrue(!jsonLexer.Diagnostics.Any());
            }
        }

        [TestMethod]
        public void Lex_Property_Colon_String_Diagnostics_WhiteSpaceAndEndOfFile()
        {
            string testCase = new String(new[] { ' ', ' ', ' ', ' ', ' ' });
            using (JsonLexer jsonLexer = new JsonLexer(testCase))
            {
                List<SyntaxToken> tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();
                Assert.IsTrue(!jsonLexer.Diagnostics.Any());
                Assert.AreEqual(tokens.Count, 2);
                Assert.AreEqual(tokens[0].Kind, SyntaxKind.Whitespace);
                Assert.AreEqual(tokens[0].Text.Length, 5);
                Assert.AreEqual(tokens[1].Kind, SyntaxKind.EndOfFile);
            }
        }

        [TestMethod]
        [DataRow("\"Test\"     ")]
        [DataRow("\"Test\"     \"A\"")]
        public void Lex_Property_Colon_String_Diagnostics_LettersWhiteSpaceAndEndOfFile(string testCase)
        {
            using (JsonLexer jsonLexer = new JsonLexer(testCase))
            {
                List<SyntaxToken> tokens = jsonLexer.Lex().AsParallel().AsOrdered().ToList();
                Assert.IsTrue(!jsonLexer.Diagnostics.Any());
                Assert.AreEqual(tokens[0].Kind, SyntaxKind.Literal);
                Assert.AreEqual(tokens[1].Kind, SyntaxKind.Whitespace);
                Assert.AreEqual(tokens.Last().Kind, SyntaxKind.EndOfFile);
            }
        }
    }
}
