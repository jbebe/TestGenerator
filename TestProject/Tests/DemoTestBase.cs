using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCommon;

namespace TestProject
{

	public class DemoTestBase
	{
    protected virtual bool IsLocal { get { return true; } }
  }
}
