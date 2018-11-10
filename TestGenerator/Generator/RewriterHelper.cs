using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestTypes;
using static TestGenerator.Generator.SyntaxNodeExtension;

namespace TestGenerator.Generator
{
  class RewriterHelper
  {
    public readonly TestType testType;

    public RewriterHelper(TestType testType)
    {
      this.testType = testType;
    }

    public MethodDeclarationSyntax ProcessMethod(MethodDeclarationSyntax node)
    {
      if (node.IsTestMethod())
      {
        return ProcessTestMethod(node);
      }

      throw new NotSupportedException();
    }

    private MethodDeclarationSyntax ProcessTestMethod(MethodDeclarationSyntax node)
    {
      // Replace protected keyword with public
      SyntaxToken replaceToPublic(SyntaxToken token)
      {
        if (token.IsKind(SyntaxKind.ProtectedKeyword))
        {
          token = SyntaxFactory.Token(token.LeadingTrivia, SyntaxKind.PublicKeyword, token.TrailingTrivia);
        }
        return token;
      }
      var modifiers = node.Modifiers.Select(replaceToPublic);
      node = node.WithModifiers(new SyntaxTokenList(modifiers));

      // Replace method name with Component_Type_Methodname
      var methodName = node.ChildTokens().First((x) => x.IsKind(SyntaxKind.IdentifierToken));
      var testTypeName = testType.ToString("G").Substring(0, 1).ToUpperInvariant() + "_";
      var componentName = GetComponentName(node);
      var shortComponentName = string.IsNullOrEmpty(componentName) ? componentName : componentName.Substring(0, 2).ToUpperInvariant() + "_";
      var newMethodName = SyntaxFactory.Identifier(methodName.LeadingTrivia, $"{shortComponentName}{testTypeName}{methodName.ValueText}", methodName.TrailingTrivia);
      node = node.ReplaceToken(methodName, newMethodName);

      // Call original method instead of logic
      var isAsyncMethod = node.Modifiers.Any((x) => x.IsKind(SyntaxKind.AsyncKeyword));
      // hack for proper indentation: margin-to-method indentation + 1/2
      var indentTrivia = node.QueryNodesOrTokensAtPath(SyntaxKind.Block, SyntaxKind.OpenBraceToken).First().GetLeadingTrivia().First();
      var indentTriviaHalf = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, new string(' ', indentTrivia.ToString().Length / 2));
      ExpressionStatementSyntax body;

      if (isAsyncMethod)
      {
        body = SyntaxFactory.ExpressionStatement(
          SyntaxFactory.AwaitExpression(
            SyntaxFactory.Token(new SyntaxTriviaList(indentTrivia, indentTriviaHalf), SyntaxKind.AwaitKeyword, Common.Space.AsList()), 
            SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(methodName.ValueText))
          ),
          Common.Semicolon
        );
      }
      else
      {
        body = SyntaxFactory.ExpressionStatement(
          SyntaxFactory.InvocationExpression(
            SyntaxFactory.IdentifierName(
              SyntaxFactory.Identifier(
                new SyntaxTriviaList(indentTrivia, indentTriviaHalf),
                methodName.ValueText,
                Common.Empty.AsList()
              )
            )
          ),
          Common.Semicolon
        );
      }

      var bodyList = new SyntaxList<StatementSyntax>(body);
      Debug.Assert(indentTrivia.IsKind(SyntaxKind.WhitespaceTrivia));
      var openBrace = SyntaxFactory.Token(indentTrivia.AsList(), SyntaxKind.OpenBraceToken, Common.NewLine.AsList());
      var closeBrace = SyntaxFactory.Token(indentTrivia.AsList(), SyntaxKind.CloseBraceToken, Common.NewLine.AsList());
      node = node.WithBody(SyntaxFactory.Block(openBrace, bodyList, closeBrace));

      // Export TestType arguments and decorate the method with them individually
      var arguments = GetTestTypeArguments(node);
      MethodDeclarationSyntax createNewAttribute(string attributeName)
      {
        return node.AddAttributeLists(
          SyntaxFactory.AttributeList(
            SyntaxFactory.SeparatedList(new[] {
                SyntaxFactory.Attribute(
                  SyntaxFactory.IdentifierName(attributeName)
                )
              }
            )
          )
          .WithTrailingTrivia(node.AttributeLists.First().GetTrailingTrivia())
          .WithLeadingTrivia(indentTrivia)
        );
      }

      if (arguments.SmokeTest)
      {
        node = createNewAttribute("SmokeTest");
      }
      if (arguments.Devel)
      {
        node = createNewAttribute("DevelTest");
      }
      if (arguments.Production)
      {
        node = createNewAttribute("ProductionTest");
      }

      if (!string.IsNullOrEmpty(componentName))
      {
        node = createNewAttribute(componentName + "Component");
      }

      // Remove unused attributes
      var removableAttrLists = node.AttributeLists.Where((attribList) => attribList.Attributes.Any((attrib) =>
      {
        var testTypes = new[] { "TestComponent", "InMemoryTest", "LocalTest", "RemoteTest" }.ToList();
        var attributeName = attrib.Name.GetText().ToString();
        return testTypes.Contains(attributeName);
      }));

      node = node.RemoveNodes(removableAttrLists, SyntaxRemoveOptions.KeepNoTrivia);

      return node;
    }

    private string GetComponentName(MethodDeclarationSyntax node)
    {
      var attributes = node.AttributeLists.SelectMany((x) => x.Attributes);
      attributes = attributes.Where((x) => x.Name.ToString() == "TestComponent");
      if (attributes.Count() > 0)
      {
        var componentAttribute = attributes.First();
        foreach (var argument in componentAttribute.ArgumentList.Arguments)
        {
          var argumentTxt = argument.ToString();
          var componentName = argumentTxt.Substring(argumentTxt.LastIndexOf('.') + 1);
          return componentName;
        }
        throw new ArgumentException("Missing argument in TestComponent");
      }
      return "";
    }

    private TestTypeArguments GetTestTypeArguments(MethodDeclarationSyntax node)
    {
      var attributes = node.AttributeLists.SelectMany((x) => x.Attributes);
      attributes = attributes.Where((x) => x.Name.ToString() == testType.ToString("G") + "Test");
      var argumentsInfo = new TestTypeArguments();

      if (attributes.Count() > 0)
      {
        var testTypeAttribute = attributes.First();
        if (testTypeAttribute.ArgumentList != null)
        {
          foreach (var argument in testTypeAttribute.ArgumentList.Arguments)
          {
            var argumentName = argument.NameEquals.Name.ToString();
            var argumentValue = argument.ChildNodes().ElementAt(1).ChildTokens().First().ValueText;

            switch (argumentName)
            {
              case "SmokeTest":
                argumentsInfo.SmokeTest = bool.Parse(argumentValue);
                break;
              case "Production":
                argumentsInfo.Production = bool.Parse(argumentValue);
                break;
              case "Devel":
                argumentsInfo.Devel = bool.Parse(argumentValue);
                break;
            }
          }
        }
      }

      return argumentsInfo;
    }

    public ClassDeclarationSyntax ProcessClassDeclaration(ClassDeclarationSyntax node)
    {
      // Add partial keyword if not present
      var isPartialKeywordPresent = node.QueryNodesOrTokensAtPath(new[] { SyntaxKind.PartialKeyword }).Count != 0;
      var newClassDeclaration = node;

      if (!isPartialKeywordPresent)
      {
        var modifiers = node.Modifiers;

        var partialKeyword = SyntaxFactory.Token(Common.Empty.AsList(), SyntaxKind.PartialKeyword, Common.Space.AsList());
        var newModifiers = SyntaxFactory.TokenList(modifiers.Concat(new[] { partialKeyword }));
        newClassDeclaration = node.WithModifiers(newModifiers);
      }

      // Remove [TestClass]
      newClassDeclaration = newClassDeclaration.WithAttributeLists(new SyntaxList<AttributeListSyntax>());

      // Remove fields except test methods
      bool isTestMethod(MemberDeclarationSyntax member)
      {
        return member.GetType().IsAssignableFrom(typeof(MethodDeclarationSyntax)) 
          && ((MethodDeclarationSyntax)member).IsTestMethod();
      }
      var testMethods = newClassDeclaration.Members.Where((x) => isTestMethod(x));
      newClassDeclaration = newClassDeclaration.WithMembers(new SyntaxList<MemberDeclarationSyntax>(testMethods));

      return newClassDeclaration;
    }
  }
}
