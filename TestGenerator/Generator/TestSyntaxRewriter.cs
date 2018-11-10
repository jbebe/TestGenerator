using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestGenerator.Generator;
using TestTypes;

namespace TestGenerator.Generator
{
	class TestSyntaxRewriter : CSharpSyntaxRewriter
	{
		public readonly TestType testType;

		public TestSyntaxRewriter(TestType testType): base()
		{
			this.testType = testType;
		}

		public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			return RewriterHelper.ProcessNamespace(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			if (node.IsTestMethod())
			{
				return RewriterHelper.ProcessTestMethod(node);
			}

			return node;
		}
	}
}
