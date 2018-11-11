using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Diagnostics;
using TestGenerator.Generator;
using TestCommon;
using System;
using TestGenerator.Resource;

namespace TestGenerator
{
	public class Program
	{
		public static void Main(string[] args)
		{
      var testGenerator = new TestGenerator(args);
      testGenerator.Execute();
    }
	}
}