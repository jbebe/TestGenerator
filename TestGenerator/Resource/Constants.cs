using System;
using System.Collections.Generic;
using System.Text;

namespace TestGenerator.Resource
{
  static class Constants
  {
    // File generation

    public const string CSharpFilePattern = "*.cs";
    public const string FileNamePostfix = ".Generated.cs";
    public static readonly string[] TestResourceType = new[] { "Local", "Remote", "Memory" };

    // Code generation
    public const string IgnoreAttribute = "Ignore";
  }
}
