// ------------------------------------------------------------------------------
// <copyright file="GlobalDeclarationRenderer.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;

using War3Net.CodeAnalysis.Jass.Syntax;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassRenderer
    {
        public void Render(JassGlobalDeclarationSyntax globalDeclaration)
        {
            Render(globalDeclaration.Declarator);
        }

        public void Render(IGlobalDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case JassEmptySyntax empty: Render(empty); break;
                case JassCommentSyntax comment: Render(comment); break;
                case JassGlobalDeclarationSyntax globalDeclaration: Render(globalDeclaration); break;

                default: throw new NotSupportedException();
            }
        }
    }
}