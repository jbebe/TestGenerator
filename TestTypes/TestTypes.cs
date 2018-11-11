using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
	public enum Component
	{
		Alpha, Beta
	}

  public class AlphaComponentAttribute: Attribute
  {

  }

  public class BetaComponentAttribute : AlphaComponentAttribute
  {

  }

  public class TestComponentAttribute : Attribute
	{
		public TestComponentAttribute(Component _)
		{

		}
	}

	public class TestInitializerAttribute : Attribute
	{
	}

	public class LocalTest : Attribute
	{
		public bool SmokeTest;
		public bool Production;

		public LocalTest()
		{

		}
	}

	public class RemoteTest: LocalTest
	{
	}

  public class InMemoryTest : LocalTest
  {
  }

  public enum TestType
	{
		InMemory, Local, Remote
	}

  public enum Type
  {
    Alpha, Beta
  }

  public class SmokeTest : Attribute
  {

  }

  public class ProductionTest : Attribute
  {

  }

  public class DevelTest : Attribute
  {

  }
}