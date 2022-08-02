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
using System.Linq;

namespace CSharpLua.LuaAst {
  public class LuaBlockSyntax : LuaStatementSyntax {
    public string OpenToken { get; set; }
    public string CloseToken { get; set; }
    public readonly LuaSyntaxList<LuaStatementSyntax> Statements = new();
    private LuaLocalAreaSyntax headVariables_;
    public int TempCount;
    public int ReleaseCount;
    public List<int> UsingDeclarations;
    public bool HasUsingDeclarations => UsingDeclarations != null;

    internal void AddHeadVariable(LuaIdentifierNameSyntax name) {
      if (headVariables_ == null) {
        headVariables_ = new LuaLocalAreaSyntax();
        Statements.Insert(0, headVariables_);
      }
      headVariables_.Variables.Add(name);
    }

    internal void AddStatement(LuaStatementSyntax statement) {
      Statements.Add(statement);
    }

    internal void AddStatements(IEnumerable<LuaExpressionSyntax> expressions) {
      Statements.AddRange(expressions.Select(i => new LuaExpressionStatementSyntax(i)));
    }

    internal void AddStatements(IEnumerable<LuaStatementSyntax> statements) {
      Statements.AddRange(statements);
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }

  public sealed class LuaBlockStatementSyntax : LuaBlockSyntax {
    public LuaBlockStatementSyntax() {
      OpenToken = Keyword.Do;
      CloseToken = Keyword.End;
    }

    internal override void Render(LuaRenderer renderer) {
      renderer.Render(this);
    }
  }
}
