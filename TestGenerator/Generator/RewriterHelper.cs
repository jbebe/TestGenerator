using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static TestGenerator.Generator.SyntaxNodeExtension;

namespace TestGenerator.Generator
{
  class RewriterHelper
  {
    public static SyntaxNode ProcessTestMethod(MethodDeclarationSyntax node)
    {
      return node.Parent.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
    }

    public static SyntaxNode ProcessNamespace(NamespaceDeclarationSyntax node)
    {
      var namespaceNode = node.QueryNodesAtPath(new[] { SyntaxKind.IdentifierName }).First();

      var oldIdentifierToken = namespaceNode.GetFirstToken();
      var identifierToken = SyntaxFactory.Identifier($"{oldIdentifierToken.ValueText}.Generated");

      return node.ReplaceToken(oldIdentifierToken, identifierToken);
    }
  }
}
