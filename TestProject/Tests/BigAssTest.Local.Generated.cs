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
    [SmokeTest] 
    [AlphaComponent] 
    public void AL_L_TestCase1()
    {
      TestCase1();
    }

    [TestMethod]
    [SmokeTest]
    [ProductionTest]
    [BetaComponent]
    public async Task BE_L_TestCase2()
    {
      await TestCase2();
    }
  }
}