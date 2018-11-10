using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
	[TestClass]
	public partial class UnitTest
	{
    // random comment

    public static int a = 5;

    public static int B { get; set; }

    [TestInitialize]
    public void Init()
    {
      a = 6;
    }

    [TestMethod]
		protected void TestMethod()
		{
      Debug.Assert(a == 6);
		}
	}
}
