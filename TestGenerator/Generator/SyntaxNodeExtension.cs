using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGenerator.Generator
{
	public static class SyntaxNodeExtension
	{
		public static bool IsTestMethod(this MethodDeclarationSyntax method)
    {
      var attributes = method.AttributeLists;
      var isTestMethod = attributes.Any((x) => x.Attributes.Any((y) => y.ToString() == "TestMethod"));
      return isTestMethod;
    }

    public static SyntaxTriviaList AsList(this SyntaxTrivia trivia)
      => new SyntaxTriviaList(trivia);

    public static List<SyntaxNodeOrToken> QueryNodesOrTokensAtPath(this SyntaxNode root, params SyntaxKind[] path)
		{
			Debug.Assert(path.Count() > 0);

      var result = new List<SyntaxNodeOrToken>();

			void getNodesRecursive(SyntaxNodeOrToken _root, ImmutableArray<SyntaxKind> _path)
			{
				var currentKind = _path.First();
				var reducedPath = _path.RemoveAt(0);
				var currentKindChildren = _root.ChildNodesAndTokens().Where((x) => x.IsKind(currentKind));

				foreach (var node in currentKindChildren)
				{
					if (reducedPath.IsEmpty)
					{
						result.Add(node);
					}
					else
					{
						getNodesRecursive(node, reducedPath);
					}
				}
			}

			getNodesRecursive(root, ImmutableArray.CreateRange(path));

			return result;
		}
	}
}
