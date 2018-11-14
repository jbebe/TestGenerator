using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestCommon;

namespace TestGenerator.Generator
{
  class TestSyntaxRewriter : CSharpSyntaxRewriter
  {
    public readonly TestClassName testType;
    public readonly RewriterHelper helper;

    /// <summary>
    /// Initialize the rewriter instance by test type
    /// </summary>
    /// <param name="testType"></param>
    public TestSyntaxRewriter(TestClassName testType) : base()
    {
      this.testType = testType;
      this.helper = new RewriterHelper(testType);
    }

    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
      if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) || trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
      {
        return base.VisitTrivia(default(SyntaxTrivia));
      }
      return base.VisitTrivia(trivia);
    }

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
      node = helper.ProcessClassDeclaration(node);
      return base.VisitClassDeclaration(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
      node = helper.ProcessMethod(node);
      return base.VisitMethodDeclaration(node);
		}

  }
}
