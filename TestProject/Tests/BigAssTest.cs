using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TestTypes;

namespace TestProject
{
  [TestClass]
  class BigAssTest
  {
    [TestInitialize]
    public void Init()
    {

    }

    // comment that should be ignored
    [TestMethod] // should be parsed
    [TestComponent(Component.Alpha)] // add AL_ prefix
    [LocalTest(SmokeTest = true, Production = true)] // generate local test, local smoke test, add ignore production flag
    [RemoteTest(SmokeTest = true, Production = true)]
    public void TestCase1()
    {
      int a = 5;
      Debug.Assert(a == 5);
    }
    [TestMethod]
    [TestComponent(Component.Beta)]
    [LocalTest(SmokeTest = true, Production = true)]
    public void TestCase2()
    {
      int a = 10;
      Debug.Assert(a == 5);
    }
  }
}