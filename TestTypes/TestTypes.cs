using System;

namespace TestCommon
{
  // Test to generate

  [AttributeUsage(AttributeTargets.Method)]
  public class TestToGenerateAttribute : Attribute { }

  // Test class name

  public enum TestClassName
  {
    Local, Remote, Memory
  }

  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class TestClassNameAttribute : Attribute
  {
    public bool IsSmokeTest { get; set; }
    public bool IgnoreTest { get; set; }
    public bool IgnoreProduction { get; set; }
    public bool IgnoreDevel { get; set; }

    public TestClassNameAttribute(TestClassName _) { }
  }

  // Test prefix

  public enum TestPrefix
  {
    AL, BE, GA
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class TestPrefixAttribute : Attribute
  {

    public TestPrefixAttribute(TestPrefix _) { }
  }

  public static class TestedServer
  {
    public const string AlphaServer = nameof(TestedServer) + "." + nameof(AlphaServer);
    public const string BetaServer  = nameof(TestedServer) + "." + nameof(BetaServer);
    public const string GammaServer = nameof(TestedServer) + "." + nameof(GammaServer);
  }



}