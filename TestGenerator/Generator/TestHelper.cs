using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TestCommon;

namespace TestGenerator.Generator
{
  public class TestTypeArguments
  {
    public bool SmokeTest { get; set; }
    public bool Devel { get; set; }
    public bool Production { get; set; }
  }

  public class TestHelper
	{
    public static string GenerateTestName(string fileName, TestType testType)
			=> Regex.Replace(fileName, @"(\.cs)$", $".{testType:G}.Generated$1");

		public static bool IsGeneratedTestFile(string fileName)
			=> fileName.Contains(".Generated.");

    public static readonly SyntaxTrivia Space = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");

    public static readonly SyntaxTrivia Empty = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, "");

    public static readonly SyntaxTrivia NewLine = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\r\n");

    public static readonly SyntaxToken Semicolon = SyntaxFactory.Token(Empty.AsList(), SyntaxKind.SemicolonToken, NewLine.AsList());

    public static string GetHashOfContent(string text)
    {
      using (var md5 = MD5.Create())
      {
        byte[] bytes = Encoding.ASCII.GetBytes(text);
        byte[] hash = md5.ComputeHash(bytes);
        string hashStr = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return hashStr;
      }
    }

    public static string GenerateMetaData(string fileContent)
    {
      string fileHash = GetHashOfContent(fileContent);
      return $"// {fileHash}" + Environment.NewLine;
    }
  }
}
