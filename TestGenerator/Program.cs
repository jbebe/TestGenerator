using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using TestGenerator.Generator;
using TestTypes;

namespace TestGenerator
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Debug.Assert(args.Length > 0);

			string[] testPaths = args;

			foreach (string testPath in testPaths)
			{
				var testFiles = Directory.GetFiles(testPath);

				foreach (var testFile in testFiles.Where((x) => !Common.IsGeneratedTestFile(x)))
				{
					ProcessTestFile(testFile);
				}
			}
		}

		private static void ProcessTestFile(string testFilePath)
		{
			string testPath = Path.GetDirectoryName(testFilePath);
			string testFileName = Path.GetFileName(testFilePath);
			string testText = File.ReadAllText(testFilePath);

			var testTypes = new[] { TestType.InMemory, TestType.Local, TestType.Remote };
			foreach (var testType in testTypes)
			{
				var syntaxTree = CSharpSyntaxTree.ParseText(testText);
				var root = syntaxTree.GetRoot();

				// rewrite tests
				var tsr = new TestSyntaxRewriter(TestType.Remote);
				root = tsr.Visit(root);

				// save file
				string generatedTestFilePath = Path.Join(testPath, Common.GetGeneratedTestName(testFileName, testType));
				File.WriteAllText(generatedTestFilePath, root.ToFullString());
			}

		}

		private static void ProcessTestMethods(SyntaxNode originalRoot, Dictionary<TestType, SyntaxTree> testTrees)
		{
			IEnumerable<SyntaxNode> getTestMethods() => originalRoot.GetClasses().SelectMany((x) => x.GetTestMethods());

			var testMethods = getTestMethods();
			foreach (var testMethod in testMethods)
			{
				originalRoot = ProcessTestMethod(originalRoot, testMethod);
			}
		}

		private static SyntaxNode ProcessTestMethod(SyntaxNode root, SyntaxNode testMethod)
		{
			var attributes = testMethod.QueryNodesAtPath(new[] { SyntaxKind.AttributeList, SyntaxKind.Attribute });

			// remove TestMethod attribute
			var testAttributeNode = attributes.First();
			var testAttributeNodeParentOld = testAttributeNode.Parent;
			Console.WriteLine($"Removing {testAttributeNode.GetText()}");

			return root.RemoveNodes(new[] { testAttributeNode }, SyntaxRemoveOptions.KeepNoTrivia);
		}

		private static Compilation CreateTestCompilation()
		{
			// creation of the syntax tree for every file
			String programPath = @"Program.cs";
			String programText = File.ReadAllText(programPath);
			SyntaxTree programTree = CSharpSyntaxTree.ParseText(programText)
						.WithFilePath(programPath);

			String rewriterPath = @"InitializerRewriter.cs";
			String rewriterText = File.ReadAllText(rewriterPath);
			SyntaxTree rewriterTree = CSharpSyntaxTree.ParseText(rewriterText)
						 .WithFilePath(rewriterPath);

			SyntaxTree[] sourceTrees = { programTree, rewriterTree };

			// gathering the assemblies
			MetadataReference mscorlib = MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
			MetadataReference codeAnalysis = MetadataReference.CreateFromFile(typeof(SyntaxTree).GetTypeInfo().Assembly.Location);
			MetadataReference csharpCodeAnalysis = MetadataReference.CreateFromFile(typeof(CSharpSyntaxTree).GetTypeInfo().Assembly.Location);

			MetadataReference[] references = { mscorlib, codeAnalysis, csharpCodeAnalysis };

			// compilation
			return CSharpCompilation.Create("ConsoleApplication",
								 sourceTrees,
								 references,
								 new CSharpCompilationOptions(OutputKind.ConsoleApplication));
		}
	}
}