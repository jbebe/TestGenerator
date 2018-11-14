using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCommon;

namespace TestProject
{

  [TestClass]
  public partial class DemoTestMemory : DemoTest
  {
    protected override bool IsLocal { get { return true; } }
  }
}
