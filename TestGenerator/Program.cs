using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Diagnostics;
using TestGenerator.Generator;
using TestTypes;
using System;

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

      var testTypes = Enum.GetValues(typeof(TestType)).Cast<TestType>();

      void processFileWithTestType(TestType testType)
      {
        // Get root
        var syntaxTree = CSharpSyntaxTree.ParseText(testText);
        var root = syntaxTree.GetRoot();

        // Rewrite tests
        var tsr = new TestSyntaxRewriter(testType);
        root = tsr.Visit(root);

        // Save file
        string generatedTestName = Common.GetGeneratedTestName(testFileName, testType);
        string generatedTestFilePath = Path.Join(testPath, generatedTestName);
        File.WriteAllText(generatedTestFilePath, root.ToFullString());
      }

      testTypes.AsParallel().ForAll(processFileWithTestType);
      //processFileWithTestType(TestType.Local);

    }
	}
}