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
using System.Diagnostics.Contracts;
using System.IO;

using CSharpLua.LuaAst;

namespace CSharpLua {
  public sealed class LuaRenderer {
    // private readonly LuaSyntaxGenerator generator_;
    private readonly LuaSyntaxGenerator.SettingInfo settings_;
    private readonly TextWriter writer_;
    private bool isNewLine_;
    private int indentLevel_;
    private int singleLineCounter_;
    private bool IsSingleLine => singleLineCounter_ > 0;

    public LuaRenderer(LuaSyntaxGenerator generator, TextWriter writer) {
      // generator_ = generator;
      settings_ = generator.Setting;
      writer_ = writer;
    }

    public LuaRenderer(LuaSyntaxGenerator.SettingInfo settings, TextWriter writer) {
      // generator_ = generator;
      settings_ = settings;
      writer_ = writer;
    }

    public LuaSyntaxGenerator.SettingInfo Setting {
      get {
        // return generator_.Setting;
        return settings_;
      }
    }

    public void RenderCompilationUnit(LuaCompilationUnitSyntax luaCompilationUnit) {
      luaCompilationUnit.Render(this);
    }

    private void AddIndent() {
      if (IsSingleLine) {
        WriteSpace();
      } else {
        ++indentLevel_;
      }
    }

    private void Outdent() {
      if (IsSingleLine) {
        WriteSpace();
      } else {
        if (indentLevel_ == 0) {
          throw new InvalidOperationException();
        }
        --indentLevel_;
      }
    }

    private void WriteNewLine() {
      if (IsSingleLine) {
        return;
      }

      writer_.Write(writer_.NewLine);
      isNewLine_ = true;
    }

    private void WriteComma() {
      Write(", ");
    }

    private void WriteCommaWithoutSpace() {
      if (IsSingleLine) {
        WriteComma();
      } else {
        Write(",");
      }
    }

    private void WriteSpace() {
      Write(" ");
    }

    private void Write(string value) {
      if (isNewLine_) {
        for (int i = 0; i < indentLevel_; i++) {
          writer_.Write(Setting.IndentString);
        }
        isNewLine_ = false;
      }
      writer_.Write(value);
    }

    private void WriteSemicolon(LuaSyntaxNode.Semicolon semicolonToken) {
      if (Setting.HasSemicolon) {
        Write(semicolonToken.ToString());
      }
    }

    private void WriteSemicolon(LuaStatementSyntax statement) {
      if (Setting.HasSemicolon || statement.ForceSemicolon) {
        Write(statement.SemicolonToken.ToString());
      }
    }

    private void WriteNodes(IEnumerable<LuaSyntaxNode> nodes) {
      foreach (var node in nodes) {
        node.Render(this);
      }
    }

    internal void Render(LuaCompilationUnitSyntax node) {
      WriteNodes(node.Statements);
    }

    internal void Render(LuaWrapFunctionStatementSyntax node) {
      node.Statement.Render(this);
    }

    public void Render(LuaExpressionStatementSyntax node) {
      node.Expression.Render(this);
      WriteSemicolon(node);
      WriteNewLine();
    }

    internal void Render(LuaMemberAccessExpressionSyntax node) {
      node.Expression.Render(this);
      Write(node.OperatorToken);
      node.Name.Render(this);
    }

    internal void Render(LuaInvocationExpressionSyntax node) {
      node.Expression.Render(this);
      node.ArgumentList.Render(this);
    }

    internal void Render(LuaIdentifierNameSyntax node) {
      Write(node.ValueText);
    }

    internal void Render(LuaPropertyOrEventIdentifierNameSyntax node) {
      Write(node.PrefixToken);
      node.Name.Render(this);
    }

    private void WriteSeparatedSyntaxList(IEnumerable<LuaSyntaxNode> list) {
      bool isFirst = true;
      foreach (LuaSyntaxNode node in list) {
        if (isFirst) {
          isFirst = false;
        } else {
          WriteComma();
        }
        node.Render(this);
      }
    }

    private void WriteArgumentList(string openParenToken, IEnumerable<LuaSyntaxNode> list, string closeParenToken) {
      Write(openParenToken);
      WriteSeparatedSyntaxList(list);
      Write(closeParenToken);
    }

    internal void Render(LuaArgumentListSyntax node) {
      if (node.IsCallSingleTable) {
        Contract.Assert(node.Arguments.Count == 1);
        WriteSpace();
        node.Arguments.First().Render(this);
      } else {
        WriteArgumentList(node.OpenParenToken, node.Arguments, node.CloseParenToken);
      }
    }

    internal void Render(LuaFunctionExpressionSyntax node) {
      Write(node.FunctionKeyword);
      WriteSpace();
      node.ParameterList.Render(this);
      node.Body.Render(this);
    }

    internal void Render(LuaParameterListSyntax node) {
      WriteArgumentList(node.OpenParenToken, node.Parameters, node.CloseParenToken);
    }

    internal void Render(LuaBlockSyntax node) {
      Write(node.OpenToken);
      WriteNewLine();
      AddIndent();
      WriteNodes(node.Statements);
      Outdent();
      Write(node.CloseToken);
    }

    internal void Render(LuaBlockStatementSyntax node) {
      Render((LuaBlockSyntax)node);
      WriteNewLine();
    }

    internal void Render(LuaIdentifierLiteralExpressionSyntax node) {
      node.Identifier.Render(this);
    }

    internal void Render(LuaStringLiteralExpressionSyntax node) {
      Write(node.OpenParenToken);
      node.Identifier.Render(this);
      Write(node.CloseParenToken);
    }

    private void WriteEquals(int count) {
      for (int i = 0; i < count; ++i) {
        Write(LuaSyntaxNode.Tokens.Equals);
      }
    }

    internal void Render(LuaVerbatimStringLiteralExpressionSyntax node) {
      Write(node.OpenBracket);
      WriteEquals(node.EqualsCount);
      Write(node.OpenBracket);
      Write(node.Text);
      Write(node.CloseBracket);
      WriteEquals(node.EqualsCount);
      Write(node.CloseBracket);
    }

    internal void Render(LuaConstLiteralExpression node) {
      node.Value.Render(this);
      if (!settings_.IsCommentsDisabled) {
        WriteSpace();
        Write(node.OpenComment);
        Write(node.IdentifierToken);
        Write(node.CloseComment);
      }
    }

    internal void Render(LuaNumberLiteralExpressionSyntax node) {
      Write(node.Text);
    }

    internal void Render(LuaStatementListSyntax node) {
      WriteNodes(node.Statements);
    }

    internal void Render(LuaLocalVariablesSyntax node) {
      if (node.Variables.Count > 0) {
        Write(node.LocalKeyword);
        WriteSpace();
        WriteSeparatedSyntaxList(node.Variables);
        node.Initializer?.Render(this);
      }
    }

    internal void Render(LuaEqualsValueClauseListSyntax node) {
      WriteSpace();
      Write(node.EqualsToken);
      WriteSpace();
      WriteSeparatedSyntaxList(node.Values);
    }

    internal void Render(LuaAssignmentExpressionSyntax node) {
      node.Left.Render(this);
      WriteSpace();
      Write(node.OperatorToken);
      WriteSpace();
      node.Right.Render(this);
    }

    internal void Render(LuaMultipleAssignmentExpressionSyntax node) {
      Contract.Assert(node.Lefts.Count > 0 && node.Rights.Count > 0);
      WriteSeparatedSyntaxList(node.Lefts);
      WriteSpace();
      Write(node.OperatorToken);
      WriteSpace();
      WriteSeparatedSyntaxList(node.Rights);
    }

    internal void Render(LuaLineMultipleExpressionSyntax node) {
      bool isFirst = true;
      foreach (var assignment in node.Assignments) {
        if (isFirst) {
          isFirst = false;
        } else {
          WriteSemicolon(LuaSyntaxNode.Tokens.Semicolon);
          WriteSpace();
        }
        assignment.Render(this);
      }
    }

    public void Render(LuaReturnStatementSyntax node) {
      Write(node.ReturnKeyword);
      if (node.Expressions.Count > 0) {
        WriteSpace();
        WriteSeparatedSyntaxList(node.Expressions);
      }
      WriteSemicolon(node.SemicolonToken);
      WriteNewLine();
    }

    internal void Render(LuaTableExpression node) {
      if (node.IsSingleLine) {
        ++singleLineCounter_;
      }

      Write(node.OpenBraceToken);
      if (node.Items.Count > 0) {
        WriteNewLine();
        AddIndent();

        bool isFirst = true;
        foreach (var itemNode in node.Items) {
          if (isFirst) {
            isFirst = false;
          } else {
            WriteCommaWithoutSpace();
            WriteNewLine();
          }
          itemNode.Render(this);
        }

        Outdent();
        WriteNewLine();
      }
      Write(node.CloseBraceToken);

      if (node.IsSingleLine) {
        --singleLineCounter_;
      }
    }

    internal void Render(LuaSingleTableItemSyntax node) {
      node.Expression.Render(this);
    }

    internal void Render(LuaKeyValueTableItemSyntax node) {
      node.Key.Render(this);
      WriteSpace();
      Write(node.OperatorToken);
      WriteSpace();
      node.Value.Render(this);
    }

    internal void Render(LuaTableExpressionKeySyntax node) {
      Write(node.OpenBracketToken);
      node.Expression.Render(this);
      Write(node.CloseBracketToken);
    }

    internal void Render(LuaTableLiteralKeySyntax node) {
      node.Identifier.Render(this);
    }

    internal void Render(LuaTableIndexAccessExpressionSyntax node) {
      node.Expression.Render(this);
      Write(node.OpenBracketToken);
      node.Index.Render(this);
      Write(node.CloseBracketToken);
    }

    internal void Render(LuaEqualsValueClauseSyntax node) {
      WriteSpace();
      Write(node.EqualsToken);
      WriteSpace();
      node.Value.Render(this);
    }

    public void Render(LuaLocalDeclarationStatementSyntax node) {
      if (!node.Declaration.IsEmpty) {
        node.Declaration.Render(this);
        WriteSemicolon(node);
        WriteNewLine();
      }
    }

    internal void Render(LuaVariableListDeclarationSyntax node) {
      bool isFirst = true;
      foreach (var variable in node.Variables) {
        if (isFirst) {
          isFirst = false;
        } else {
          WriteSpace();
        }
        variable.Render(this);
      }
    }

    internal void Render(LuaVariableDeclaratorSyntax node) {
      if (node.IsLocalDeclaration) {
        Write(node.LocalKeyword);
        WriteSpace();
      } else if (node.Initializer?.Value is LuaFunctionExpressionSyntax functionExpression && functionExpression.RenderAsFunctionDefinition) {
        Write(functionExpression.FunctionKeyword);
        WriteSpace();
        node.Identifier.Render(this);
        functionExpression.ParameterList.Render(this);
        functionExpression.Body.Render(this);
        return;
      }
      node.Identifier.Render(this);
      node.Initializer?.Render(this);
    }

    internal void Render(LuaLocalVariableDeclaratorSyntax node) {
      node.Declarator.Render(this);
      WriteSemicolon(node);
      WriteNewLine();
    }

    internal void Render(LuaLocalAreaSyntax node) {
      const int kPerLineCount = 8;
      if (node.Variables.Count > 0) {
        Write(node.LocalKeyword);
        WriteSpace();
        int count = 0;
        foreach (var item in node.Variables) {
          if (count > 0) {
            WriteComma();
            if (count % kPerLineCount == 0) {
              WriteNewLine();
            }
          }
          item.Render(this);
          ++count;
        }
        WriteSemicolon(node);
        WriteNewLine();
      }
    }

    internal void Render(LuaLocalFunctionSyntax node) {
      node.Comments.Render(this);
      Write(node.LocalKeyword);
      WriteSpace();
      Write(node.FunctionExpression.FunctionKeyword);
      WriteSpace();
      node.IdentifierName.Render(this);
      node.FunctionExpression.ParameterList.Render(this);
      WriteSpace();
      node.FunctionExpression.Body.Render(this);
      WriteNewLine();
    }


    internal void Render(LuaLocalTupleVariableExpression node) {
      Write(node.LocalKeyword);
      WriteSpace();
      WriteSeparatedSyntaxList(node.Variables);
    }

    internal void Render(LuaBinaryExpressionSyntax node) {
      node.Left.Render(this);
      WriteSpace();
      Write(node.OperatorToken);
      WriteSpace();
      node.Right.Render(this);
    }

    public void Render(LuaIfStatementSyntax node) {
      Write(node.IfKeyword);
      WriteSpace();
      node.Condition.Render(this);
      WriteSpace();
      Write(node.OpenParenToken);
      node.Body.Render(this);
      WriteNodes(node.ElseIfStatements);
      node.Else?.Render(this);
      Write(node.CloseParenToken);
      WriteNewLine();
    }

    internal void Render(LuaElseIfStatementSyntax node) {
      Write(node.ElseIfKeyword);
      WriteSpace();
      node.Condition.Render(this);
      WriteSpace();
      Write(node.OpenParenToken);
      node.Body.Render(this);
    }

    internal void Render(LuaElseClauseSyntax node) {
      Write(node.ElseKeyword);
      node.Body.Render(this);
    }

    public void RenderIf(LuaExpressionSyntax condition) {
      Write(LuaSyntaxNode.Tokens.If);
      WriteSpace();
      condition.Render(this);
      WriteSpace();
      Write(LuaSyntaxNode.Tokens.Then);
    }

    public void RenderElseIf(LuaExpressionSyntax condition) {
      Write(LuaSyntaxNode.Tokens.ElseIf);
      WriteSpace();
      condition.Render(this);
      WriteSpace();
      Write(LuaSyntaxNode.Tokens.Then);
    }

    public void RenderElse() {
      Write(LuaSyntaxNode.Tokens.Else);
    }

    public void RenderEnd() {
      Outdent();
      Write(LuaSyntaxNode.Tokens.End);
      WriteNewLine();
    }

    public void RenderFunctionDeclarator(LuaVariableDeclaratorSyntax functionDeclarator) {
      Write(LuaSyntaxNode.Tokens.Function);
      WriteSpace();
      functionDeclarator.Identifier.Render(this);
      ((LuaFunctionExpressionSyntax)functionDeclarator.Initializer.Value).ParameterList.Render(this);
      Write(LuaSyntaxNode.Tokens.Empty);
      WriteNewLine();
      AddIndent();
    }

    public void RenderLoop() {
      Write(LuaSyntaxNode.Tokens.While);
      WriteSpace();
      LuaIdentifierLiteralExpressionSyntax.True.Render(this);
      WriteSpace();
      Write(LuaSyntaxNode.Tokens.Do);
      WriteNewLine();
      AddIndent();
    }

    internal void Render(LuaPrefixUnaryExpressionSyntax node) {
      Write(node.OperatorToken);
      if (node.ForceWhitespaceAfterOperator || char.IsLetter(node.OperatorToken[^1])) {
        WriteSpace();
      }
      node.Operand.Render(this);
    }

    internal void Render(LuaForInStatementSyntax node) {
      Write(node.ForKeyword);
      WriteSpace();
      node.Placeholder.Render(this);
      WriteComma();
      node.Identifier.Render(this);
      WriteSpace();
      Write(node.InKeyword);
      WriteSpace();
      node.Expression.Render(this);
      WriteSpace();
      node.Body.Render(this);
      WriteNewLine();
    }

    internal void Render(LuaNumericalForStatementSyntax node) {
      Write(node.ForKeyword);
      WriteSpace();
      node.Identifier.Render(this);
      WriteSpace();
      Write(node.EqualsToken);
      WriteSpace();
      node.StartExpression.Render(this);
      WriteComma();
      node.LimitExpression.Render(this);
      if (node.StepExpression != null) {
        WriteComma();
        node.StepExpression.Render(this);
      }
      WriteSpace();
      node.Body.Render(this);
      WriteNewLine();
    }

    internal void Render(LuaWhileStatementSyntax node) {
      Write(node.WhileKeyword);
      WriteSpace();
      node.Condition.Render(this);
      WriteSpace();
      node.Body.Render(this);
      WriteNewLine();
    }

    internal void Render(LuaRepeatStatementSyntax node) {
      Write(node.RepeatKeyword);
      node.Body.Render(this);
      Write(node.UntilKeyword);
      WriteSpace();
      node.Condition.Render(this);
      WriteSemicolon(node);
      WriteNewLine();
    }

    internal void Render(LuaSwitchAdapterStatementSyntax node) {
      node.RepeatStatement.Render(this);
    }

    internal void Render(LuaBreakStatementSyntax node) {
      Write(node.BreakKeyword);
      WriteSemicolon(node.SemicolonToken);
      WriteNewLine();
    }

    internal void Render(LuaContinueAdapterStatementSyntax node) {
      node.Assignment.Render(this);
      node.Statement.Render(this);
    }

    internal void Render(LuaBlankLinesStatement node) {
      for (int i = 0; i < node.Count; ++i) {
        WriteNewLine();
      }
    }

    public void Render(LuaShortCommentStatement node) {
      if (!settings_.IsCommentsDisabled) {
        Write(node.SingleCommentToken);
        Write(node.Comment);
      }
      WriteNewLine();
    }

    internal void Render(LuaShortCommentExpressionStatement node) {
      if (!settings_.IsCommentsDisabled) {
        Write(node.SingleCommentToken);
        node.Expression.Render(this);
      }
      WriteNewLine();
    }

    internal void Render(LuaParenthesizedExpressionSyntax node) {
      Write(node.OpenParenToken);
      node.Expression.Render(this);
      Write(node.CloseParenToken);
    }

    internal void Render(LuaGotoStatement node) {
      Write(node.GotoKeyword);
      WriteSpace();
      node.Identifier.Render(this);
      WriteSemicolon(node.SemicolonToken);
      WriteNewLine();
    }

    internal void Render(LuaLabeledStatement node) {
      Write(node.PrefixToken);
      node.Identifier.Render(this);
      Write(node.SuffixToken);
      WriteSemicolon(node.SemicolonToken);
      WriteNewLine();
      node.Statement?.Render(this);
    }

    internal void Render(LuaGotoCaseAdapterStatement node) {
      node.Assignment.Render(this);
      node.GotoStatement.Render(this);
    }

    private void WriteWithShortComment(string text) {
      if (!settings_.IsCommentsDisabled) {
        Write(LuaSyntaxNode.Tokens.ShortComment);
        WriteSpace();
        Write(text);
      }
      WriteNewLine();
    }

    internal void Render(LuaDocumentStatement node) {
      WriteNodes(node.Statements);
    }

    internal void Render(LuaSummaryDocumentStatement node) {
      foreach (string text in node.Texts) {
        WriteWithShortComment(text);
      }
    }

    internal void Render(LuaLineDocumentStatement node) {
      WriteWithShortComment(node.Text);
    }

    internal void Render(LuaCodeTemplateExpressionSyntax node) {
      WriteNodes(node.Expressions);
    }

    internal void Render(LuaPropertyAdapterExpressionSyntax node) {
      if (node.Expression != null) {
        node.Expression.Render(this);
        Write(node.OperatorToken);
      }
      node.Name.Render(this);
      node.ArgumentList.Render(this);
    }

    internal void Render(LuaSequenceListExpressionSyntax node) {
      WriteSeparatedSyntaxList(node.Expressions);
    }
  }
}
