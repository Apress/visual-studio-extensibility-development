using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System.Runtime.CompilerServices;

namespace ConfigureAwaitAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigureAwaitAnalyzerCodeFixProvider)), Shared]
    public class ConfigureAwaitAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Use ConfigureAwait";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ConfigureAwaitAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AwaitExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ConfigureAwaitFalseAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> ConfigureAwaitFalseAsync(Document document, ExpressionSyntax expression, CancellationToken cancellationToken)
        {
            return await this.ConfigureAwaitAsync(document, expression, SyntaxKind.FalseLiteralExpression, cancellationToken).ConfigureAwait(false);
        }

        private async Task<Document> ConfigureAwaitAsync(Document document, ExpressionSyntax invocationExpression, SyntaxKind configureAwaitLiteral, CancellationToken cancellationToken)
        {
            MemberAccessExpressionSyntax memberAccessExpressionSyntax = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, invocationExpression, SyntaxFactory.IdentifierName("ConfigureAwait"));
            SyntaxToken syntaxToken = SyntaxFactory.Token(SyntaxKind.OpenParenToken);
            List<ArgumentSyntax> argumentSyntaxes = new List<ArgumentSyntax>()
            {
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(configureAwaitLiteral))
            };

            InvocationExpressionSyntax invocationExpressionSyntax = SyntaxFactory.InvocationExpression(memberAccessExpressionSyntax, SyntaxFactory.ArgumentList(syntaxToken, SyntaxFactory.SeparatedList<ArgumentSyntax>(argumentSyntaxes), SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode syntaxNode = SyntaxNodeExtensions.ReplaceNode<SyntaxNode>(root, invocationExpression, invocationExpressionSyntax);
            return document.WithSyntaxRoot(syntaxNode);
        }
    }
}
