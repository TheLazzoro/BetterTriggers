// ------------------------------------------------------------------------------
// <copyright file="GlobalDeclarationParser.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using Pidgin;

using War3Net.CodeAnalysis.Jass.Syntax;

using static Pidgin.Parser;

namespace War3Net.CodeAnalysis.Jass
{
    internal partial class JassParser
    {
        internal static Parser<char, IGlobalDeclarationSyntax> GetGlobalDeclarationParser(
            Parser<char, JassEmptySyntax> emptyParser,
            Parser<char, JassCommentSyntax> commentParser,
            Parser<char, JassGlobalDeclarationSyntax> constantDeclarationParser,
            Parser<char, JassGlobalDeclarationSyntax> variableDeclarationParser)
        {
            return OneOf(
                emptyParser.Cast<IGlobalDeclarationSyntax>(),
                commentParser.Cast<IGlobalDeclarationSyntax>(),
                constantDeclarationParser.Cast<IGlobalDeclarationSyntax>(),
                variableDeclarationParser.Cast<IGlobalDeclarationSyntax>());
        }
    }
}