/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;

namespace CSharpLua.LuaAst {
  public sealed class LuaLocalVariablesSyntax : LuaVariableDeclarationSyntax {
    public string LocalKeyword => Keyword.Local;
    public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new();
    public LuaEqualsValueClauseListSyntax Initializer { get; set; }

    public LuaLocalVariablesSyntax() {
    }

    public LuaLocalVariablesSyntax(IEnumerable<LuaIdentifierNameSyntax> variables, IEnumerable<LuaExpressionSyntax> values = null) {
      Variables.AddRange(variables);
      if (values != null) {
        Initializer = new LuaEqualsValueClauseListSyntax(values);
      }
    }

    public override bool IsEmpty => Variables.Count == 0;

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaEqualsValueClauseListSyntax : LuaSyntaxNode {
    public string EqualsToken => Tokens.Equals;
    public readonly LuaSyntaxList<LuaExpressionSyntax> Values = new();

    public LuaEqualsValueClauseListSyntax() {

    }

    public LuaEqualsValueClauseListSyntax(LuaExpressionSyntax value) {
      Values.Add(value);
    }

    public LuaEqualsValueClauseListSyntax(IEnumerable<LuaExpressionSyntax> values) {
      Values.AddRange(values);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public abstract class LuaVariableDeclarationSyntax : LuaSyntaxNode {
    public abstract bool IsEmpty { get; }

    public static implicit operator LuaStatementSyntax(LuaVariableDeclarationSyntax node) {
      return new LuaLocalDeclarationStatementSyntax(node);
    }
  }

  public sealed class LuaVariableListDeclarationSyntax : LuaVariableDeclarationSyntax {
    public readonly LuaSyntaxList<LuaVariableDeclaratorSyntax> Variables = new();

    public override bool IsEmpty => Variables.Count == 0;

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalDeclarationStatementSyntax : LuaStatementSyntax {
    public LuaVariableDeclarationSyntax Declaration { get; }

    public LuaLocalDeclarationStatementSyntax(LuaVariableDeclarationSyntax declaration) {
      Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaVariableDeclaratorSyntax : LuaSyntaxNode {
    public string LocalKeyword => Keyword.Local;
    public LuaIdentifierNameSyntax Identifier { get; }
    public LuaEqualsValueClauseSyntax Initializer { get; set; }
    public bool IsLocalDeclaration { get; set; }

    public LuaVariableDeclaratorSyntax(LuaIdentifierNameSyntax identifier, LuaExpressionSyntax expression = null) {
      Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
      if (expression != null) {
        Initializer = new LuaEqualsValueClauseSyntax(expression);
      }
      IsLocalDeclaration = true;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalVariableDeclaratorSyntax : LuaStatementSyntax {
    public LuaVariableDeclaratorSyntax Declarator { get; }

    public LuaLocalVariableDeclaratorSyntax(LuaVariableDeclaratorSyntax declarator) {
      Declarator = declarator ?? throw new ArgumentNullException(nameof(declarator));
    }

    public LuaLocalVariableDeclaratorSyntax(LuaIdentifierNameSyntax identifier, LuaExpressionSyntax expression = null) {
      Declarator = new LuaVariableDeclaratorSyntax(identifier, expression);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaEqualsValueClauseSyntax : LuaSyntaxNode {
    public string EqualsToken => Tokens.Equals;
    public LuaExpressionSyntax Value { get; }

    public LuaEqualsValueClauseSyntax(LuaExpressionSyntax value) {
      Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalAreaSyntax : LuaStatementSyntax {
    public string LocalKeyword => Keyword.Local;
    public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalFunctionSyntax : LuaStatementSyntax {
    public readonly LuaStatementListSyntax Comments = new();
    public string LocalKeyword => Keyword.Local;
    public LuaIdentifierNameSyntax IdentifierName { get; }
    public LuaFunctionExpressionSyntax FunctionExpression { get; }

    public LuaLocalFunctionSyntax(LuaIdentifierNameSyntax identifierName, LuaFunctionExpressionSyntax functionExpression, LuaDocumentStatement documentation = null) {
      IdentifierName = identifierName ?? throw new ArgumentNullException(nameof(identifierName));
      FunctionExpression = functionExpression ?? throw new ArgumentNullException(nameof(functionExpression));
      if (documentation != null) {
        Comments.Statements.Add(documentation);
      }
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaLocalTupleVariableExpression : LuaExpressionSyntax {
    public string LocalKeyword => Keyword.Local;
    public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Variables = new();

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }

    public LuaLocalTupleVariableExpression() {
    }

    public LuaLocalTupleVariableExpression(IEnumerable<LuaIdentifierNameSyntax> variables) {
      Variables.AddRange(variables);
    }
  }
}
