using Adsophic.CodeGen.API;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adsophic.CodeGen
{
    public class CodeFormatter : ICodeFormatter
    {
        public string Format(string unformatted)
        {
            var root = CSharpSyntaxTree.ParseText(unformatted).GetRoot();
            var namespaceDeclaration = root
                  .DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                  .FirstOrDefault();

            root = root.ReplaceNode(namespaceDeclaration, 
                namespaceDeclaration.NormalizeWhitespace(elasticTrivia: true));

            return root.ToFullString();
        }
    }
}
