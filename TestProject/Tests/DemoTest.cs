using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCommon;

namespace TestProject
{

	public abstract class DemoTest: DemoTestBase
	{
    // random comment

    private static int a = 5;

    private static int B { get; set; }

    /// <summary>
    /// Test1 won't show up anywhere in the generated tests
    /// </summary>
    [TestToGenerate]
    [TestCategory(TestedServer.AlphaServer)]
    [TestPrefix(TestPrefix.AL)]
    //[TestClassName(TestClassName.Memory)]
    //[TestClassName(TestClassName.Local)]
    //[TestClassName(TestClassName.Remote)]
    protected void Test1()
    {
      Debug.Assert(a == 6);
    }

    /// <summary>
    /// Test2 will only show up in local with AL_ prefix and BetaServer category
    /// </summary>
    [TestToGenerate]
    [TestCategory(TestedServer.BetaServer)]
    [TestPrefix(TestPrefix.AL)]
    //[TestClassName(TestClassName.Memory)]
    [TestClassName(TestClassName.Local)]
    //[TestClassName(TestClassName.Remote)]
    protected void Test2()
		{
      Debug.Assert(a == 6);
		}

    /// <summary>
    /// Test3 will only show up in local with AL_ prefix and BetaServer category with [Ignore] attribute
    /// </summary>
    [TestToGenerate]
    [TestCategory(TestedServer.BetaServer)]
    [TestPrefix(TestPrefix.AL)]
    //[TestClassName(TestClassName.Memory)]
    [TestClassName(TestClassName.Local, IgnoreTest = true)]
    //[TestClassName(TestClassName.Remote)]
    protected void Test3()
    {
      Debug.Assert(a == 6);
    }

    /// <summary>
    /// Test4 will only show up both in local and remote, with AL_ prefix and BetaServer category 
    /// </summary>
    [TestToGenerate]
    [TestCategory(TestedServer.BetaServer)]
    [TestPrefix(TestPrefix.AL)]
    //[TestClassName(TestClassName.Memory)]
    [TestClassName(TestClassName.Local)]
    [TestClassName(TestClassName.Remote)]
    protected void Test4()
    {
      Debug.Assert(a == 6);
    }

    /// <summary>
    /// Same as Test4
    /// </summary>
    [TestToGenerate]
    [TestCategory(TestedServer.BetaServer)]
    [TestPrefix(TestPrefix.AL)]
    //[TestClassName(TestClassName.Memory)]
    //[TestClassName(TestClassName.Local)]
    [TestClassName(TestClassName.Remote, IgnoreDevel = false, IgnoreProduction = false, IgnoreTest = false, IsSmokeTest = false)]
    protected void Test5()
    {
      Debug.Assert(a == 6);
    }

    /// <summary>
    /// Test6 will only show up both in local and remote, with AL_ prefix and BetaServer category 
    /// but it will have all the flags from Remote
    /// </summary>
    [TestToGenerate]
    [TestCategory(TestedServer.BetaServer)]
    [TestPrefix(TestPrefix.AL)]
    //[TestClassName(TestClassName.Memory)]
    //[TestClassName(TestClassName.Local)]
    [TestClassName(TestClassName.Remote, IgnoreDevel = true, IgnoreProduction = true, IgnoreTest = true, IsSmokeTest = true)]
    protected void Test6()
    {
      Debug.Assert(a == 6);
    }

    /// <summary>
    /// Test7 will only show up both in local and remote, with AL_ prefix and BetaServer category 
    /// but it will have all the flags from Remote and also it is an awaited task
    /// </summary>
    [TestToGenerate]
    [TestCategory(TestedServer.BetaServer)]
    [TestPrefix(TestPrefix.AL)]
    //[TestClassName(TestClassName.Memory)]
    //[TestClassName(TestClassName.Local)]
    [TestClassName(TestClassName.Remote, IgnoreDevel = true, IgnoreProduction = true, IgnoreTest = true, IsSmokeTest = true)]
    protected async Task Test7()
    {
      Debug.Assert(a == 6);
      await Task.Yield();
    }
  }
}
