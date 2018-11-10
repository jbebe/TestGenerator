using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace TestGenerator.Generator
{
	public static class SyntaxNodeExtension
	{
		public static bool IsTestMethod(this SyntaxNode method)
			=> method
			.QueryNodesAtPath(new[] {
				SyntaxKind.AttributeList,
				SyntaxKind.Attribute,
				SyntaxKind.IdentifierName
			})
			.First()
			.GetText().ToString() == "TestMethod";

		public static IEnumerable<SyntaxNode> GetMethods(this SyntaxNode @class)
			=> @class.ChildNodes().Where((x) => x.IsKind(SyntaxKind.MethodDeclaration));

		public static IEnumerable<SyntaxNode> GetTestMethods(this SyntaxNode @class)
			=> @class.GetMethods().Where((x) => x.IsTestMethod());

		public static List<SyntaxNode> GetClasses(this SyntaxNode root)
		{
			var classes = new List<SyntaxNode>();

			void findClassesRecursive(SyntaxNode currentRoot)
			{
				foreach (var node in currentRoot.ChildNodes())
				{
					if (node.IsKind(SyntaxKind.NamespaceDeclaration))
					{
						findClassesRecursive(node);
					}
					else if (node.IsKind(SyntaxKind.ClassDeclaration))
					{
						classes.Add(node);
					}
				}
			}

			findClassesRecursive(root);

			return classes;
		}

		public static List<SyntaxNode> QueryNodesAtPath(this SyntaxNode root, IEnumerable<SyntaxKind> path)
		{
			Debug.Assert(path.Count() > 0);

			var result = new List<SyntaxNode>();

			void getNodesRecursive(SyntaxNode _root, ImmutableArray<SyntaxKind> _path)
			{
				var currentKind = _path.First();
				var reducedPath = _path.RemoveAt(0);
				var currentKindChildren = _root.ChildNodes().Where((x) => x.IsKind(currentKind));

				foreach (var node in currentKindChildren)
				{
					if (reducedPath.IsEmpty)
					{
						result.Add(_root);
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
