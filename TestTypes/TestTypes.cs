using System;
using System.Collections.Generic;
using System.Text;

namespace TestTypes
{
	public enum Component
	{
		Alpha, Beta
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

	public enum TestType
	{
		InMemory, Local, Remote
	}
}