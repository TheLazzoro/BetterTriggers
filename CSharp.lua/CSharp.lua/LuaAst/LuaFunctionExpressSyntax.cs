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

using System.Collections.Generic;

namespace CSharpLua.LuaAst {
  public sealed class LuaParameterListSyntax : LuaSyntaxNode {
    public string OpenParenToken => Tokens.OpenParentheses;
    public string CloseParenToken => Tokens.CloseParentheses;
    public readonly LuaSyntaxList<LuaIdentifierNameSyntax> Parameters = new();
    public int Count => Parameters.Count;

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public class LuaFunctionExpressionSyntax : LuaExpressionSyntax {
    public readonly LuaParameterListSyntax ParameterList = new();
    public string FunctionKeyword => Keyword.Function;
    public bool RenderAsFunctionDefinition;
    public int TempCount;

    public readonly LuaBlockSyntax Body = new() {
      OpenToken = Tokens.Empty,
      CloseToken = Keyword.End,
    };

    public void AddParameter(LuaIdentifierNameSyntax parameter) {
      ParameterList.Parameters.Add(parameter);
    }

    public void AddParameters(IEnumerable<LuaIdentifierNameSyntax> parameters) {
      ParameterList.Parameters.AddRange(parameters);
    }

    public void AddStatement(LuaStatementSyntax statement) {
      Body.Statements.Add(statement);
    }

    public void AddStatements(IEnumerable<LuaStatementSyntax> statements) {
      Body.Statements.AddRange(statements);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaConstructorAdapterExpressionSyntax : LuaFunctionExpressionSyntax {
    public bool IsInvokeThisCtor { get; set; }
    public bool IsStatic { get; set; }
  }

  public abstract class LuaCheckLoopControlExpressionSyntax : LuaFunctionExpressionSyntax {
    public bool HasReturn { get; set; }
    public bool HasBreak { get; set; }
    public bool HasContinue { get; set; }
  }

  public sealed class LuaTryAdapterExpressionSyntax : LuaCheckLoopControlExpressionSyntax {
    public LuaIdentifierNameSyntax CatchTemp { get; set; }
  }

  public sealed class LuaUsingAdapterExpressionSyntax : LuaCheckLoopControlExpressionSyntax {
  }
}
