using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGenerator.Generator
{
	class RewriterHelper
	{
		public static SyntaxNode ProcessTestMethod(MethodDeclarationSyntax node)
		{


			return node;
		}

		public static SyntaxNode ProcessNamespace(NamespaceDeclarationSyntax node)
		{

			return node;
		}
	}
}
