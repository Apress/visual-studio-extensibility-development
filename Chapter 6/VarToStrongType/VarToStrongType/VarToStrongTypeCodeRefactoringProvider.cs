using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace VarToStrongType
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(VarToStrongTypeCodeRefactoringProvider)), Shared]
    internal class VarToStrongTypeCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            IEnumerable<LocalDeclarationStatementSyntax> nodes = root.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();

            foreach (var node in nodes)
            {
                if (!node.Declaration.Type.IsVar)
                {
                    continue;
                }

                // For any type declaration node, create a code action to replace with type
                CodeAction action = CodeAction.Create("Replace var with Type", c => this.ReplaceVarWithTypeAsync(context.Document, node, c));

                // Register this code action.
                context.RegisterRefactoring(action);
            }
        }

        private async Task<Document> ReplaceVarWithTypeAsync
            (Document document, 
            LocalDeclarationStatementSyntax varDeclaration, 
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            // Get the symbol representing the type to be renamed.
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            SymbolInfo typeSymbol = semanticModel.GetSymbolInfo(varDeclaration.Declaration.Type);
            var newIdentifier = SyntaxFactory.IdentifierName(typeSymbol.Symbol.ToDisplayString());

            IdentifierNameSyntax varTypeName = varDeclaration.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            LocalDeclarationStatementSyntax newDeclaration = varDeclaration.ReplaceNode(varTypeName, newIdentifier);
            SyntaxNode newRoot = root.ReplaceNode(varDeclaration, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
