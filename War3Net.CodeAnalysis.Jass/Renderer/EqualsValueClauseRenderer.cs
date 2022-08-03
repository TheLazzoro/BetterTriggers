﻿// ------------------------------------------------------------------------------
// <copyright file="EqualsValueClauseRenderer.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using War3Net.CodeAnalysis.Jass.Syntax;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassRenderer
    {
        public void Render(JassEqualsValueClauseSyntax equalsValueClause)
        {
            Write($"{JassSymbol.EqualsSign} ");
            Render(equalsValueClause.Expression);
        }
    }
}