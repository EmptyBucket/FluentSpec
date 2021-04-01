// MIT License
// 
// Copyright (c) 2021 Alexey Politov
// https://github.com/EmptyBucket/FluentSpecification
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;

namespace FluentSpecification.Composite.Nodes.Visitors
{
	internal class ReplacementSpecNodeVisitor<TReplaced, T> : SpecNodeVisitorBase<TReplaced>
	{
		private readonly Func<T, TReplaced> _replacement;
		private readonly Stack<ISpecNode<T>> _specNodes = new Stack<ISpecNode<T>>();

		public ReplacementSpecNodeVisitor(Func<T, TReplaced> replacement) => _replacement = replacement;

		public override void Visit(AndSpecNode<TReplaced> andSpecNode)
		{
			base.Visit(andSpecNode);
			var rightSpecNode = _specNodes.Pop();
			var leftSpecNode = _specNodes.Pop();
			_specNodes.Push(new AndSpecNode<T>(leftSpecNode, rightSpecNode));
		}

		public override void Visit(OrSpecNode<TReplaced> orSpecNode)
		{
			base.Visit(orSpecNode);
			var rightSpecNode = _specNodes.Pop();
			var leftSpecNode = _specNodes.Pop();
			_specNodes.Push(new OrSpecNode<T>(leftSpecNode, rightSpecNode));
		}

		public override void Visit(NotSpecNode<TReplaced> notSpecNode)
		{
			base.Visit(notSpecNode);
			_specNodes.Push(new NotSpecNode<T>(_specNodes.Pop()));
		}

		public override void Visit(WrappedSpecNode<TReplaced> wrappedSpecNode)
		{
			base.Visit(wrappedSpecNode);
			_specNodes.Push(new ReplacementSpec<TReplaced, T>(wrappedSpecNode.Spec, _replacement));
		}

		public ISpecNode<T> NewRoot => _specNodes.Peek();
	}
}