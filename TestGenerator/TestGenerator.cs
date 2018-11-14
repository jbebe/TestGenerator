using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using TestGenerator.Generator;
using System;
using TestGenerator.Resource;
using TestCommon;

namespace TestGenerator
{
  class TestGenerator
  {
    private SanityChecker Checker { get; } = new SanityChecker();
    private ILogger Logger { get; } = new ConsoleLogger(LogLevel.Debug);
    public string[] TestPaths { get; }

    public TestGenerator(string[] testPaths)
    {
      CheckTestPaths(testPaths);
      TestPaths = testPaths;
    }

    private void CheckTestPaths(string[] testPaths)
    {
      Checker.Assert(testPaths.Length > 0, ErrorMessages.InsufficientArgs);
      Checker.Assert(testPaths.All((x) => Directory.Exists(x)), ErrorMessages.DirectoryNotExist);
    }

    private void OnPerformanceMeasureEnd(TimeSpan elapsed)
      => Logger.Log($"Processing test files ended. Speed: {elapsed.TotalMilliseconds} ms", LogLevel.Info);

    public void Execute()
    {
      Logger.Log("Test generation started.", LogLevel.Info);
      using (var measure = Logger.Measure(OnPerformanceMeasureEnd))
      {
        foreach (string testPath in TestPaths)
        {
          var filesInDirectory = Directory.GetFiles(testPath, Constants.CSharpFilePattern);
          var filesWithoutAlreadyGenerated = filesInDirectory.Where((x) => !TestHelper.IsGeneratedTestFile(x));
          if (filesWithoutAlreadyGenerated.Count() == 0)
          {
            Logger.Log($"No test file found in directory. ({testPath})", LogLevel.Warning);
          }
          foreach (var testFile in filesWithoutAlreadyGenerated)
          {
            Logger.Log($"Processing file: {Path.GetFileName(testFile)}", LogLevel.Info);
            ProcessTestFile(testFile);
          }
        }
      }
      Logger.Log("Test generation ended.", LogLevel.Info);
    }

    private void ProcessTestFile(string testFilePath)
    {
      string testDirPath = Path.GetDirectoryName(testFilePath);
      string fileName = Path.GetFileName(testFilePath);
      string fileContent = File.ReadAllText(testFilePath);

      var testTypes = Enum.GetValues(typeof(TestClassName)).Cast<TestClassName>();

      void processFileWithTestType(TestClassName testType)
      {
        // Skip file generation if it is already up to date
        string generatedTestName = TestHelper.GenerateTestName(fileName, testType);
        string generatedTestFilePath = Path.Join(testDirPath, generatedTestName);
        if (IsAlreadyGenerated(fileContent, generatedTestFilePath))
        {
          Logger.Log($"Skipping test type {testType:G} of {fileName} as it is already generated.", LogLevel.Info);
          return;
        }

        // Get root
        var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
        var root = syntaxTree.GetRoot();

        // Rewrite tests
        // This is where the actual processing happens
        var tsr = new TestSyntaxRewriter(testType);
        root = tsr.Visit(root);

        // Save file
        string metadata = TestHelper.GetMetaData(fileContent);
        string formattedOutput = metadata + root.ToFullString();
        File.WriteAllText(generatedTestFilePath, formattedOutput);
      }

      // Run generation
      //testTypes.AsParallel().ForAll(processFileWithTestType);
      processFileWithTestType(TestClassName.Local);
    }

    private bool IsAlreadyGenerated(string testFileContent, string generatedTestFilePath)
    {
      // If the generated file does not exist it cannot be unchanged
      if (!File.Exists(generatedTestFilePath))
      {
        return false;
      }

      string testFileHash = TestHelper.GetHashOfContent(testFileContent);
      int hashLength = testFileHash.Length;
      string generatedFileContent = File.ReadAllText(generatedTestFilePath);
      int metadataLength = 2 + 1 + hashLength;
      int metadataWithNewLineLength = metadataLength + 2;

      // If the file does not start with the length of "// <md5hash>\r\n" it is probably not valid
      if (generatedFileContent.Length < metadataLength)
      {
        return false;
      }

      // If the first line is longer than the metadata, still not valid
      int firstLineEndOffset = generatedFileContent.IndexOf(Environment.NewLine);
      if (firstLineEndOffset > metadataLength)
      {
        return false;
      }

      // Get the original test file hash from the metadata
      string oldTestFileHash = generatedFileContent.Substring(3, hashLength);

      return testFileHash == oldTestFileHash;
    }
  }
}
