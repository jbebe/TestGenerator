using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using TestTypes;

namespace TestProject
{
  partial class BigAssTest
  {

    
    [TestMethod] 
    [TestComponent(Component.Alpha)] 
    [LocalTest(SmokeTest = true)] 
    [RemoteTest(SmokeTest = true, Production = true)]
    public void AL_L_TestCase1()
    {
      TestCase1();
    }
    [TestMethod]
    [TestComponent(Component.Beta)]
    [LocalTest(SmokeTest = true, Production = true)]
    public async Task BE_L_TestCase2()
    {
      await TestCase2();
    }
  }
}