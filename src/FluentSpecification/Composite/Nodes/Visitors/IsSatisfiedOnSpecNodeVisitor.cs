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

using System.Collections.Generic;
using FluentSpecification.Configuration;

namespace FluentSpecification.Composite.Nodes.Visitors
{
	internal class IsSatisfiedOnSpecNodeVisitor<T> : SpecNodeVisitorBase<T>
	{
		private readonly Stack<string> _isSatisfiedOns = new Stack<string>();
		private bool _not;

		public override void Visit(AndSpecNode<T> andSpecNode)
		{
			if (_not) Visit(MoveNotDown(new NotSpecNode<T>(andSpecNode)));
			else
			{
				static string WrapOrBracket(ISpecNode<T> node, string isSatisfiedOn) =>
					node is OrSpecNode<T> ? $"({isSatisfiedOn})" : isSatisfiedOn;

				base.Visit(andSpecNode);
				var rightIsSatisfiedOn = WrapOrBracket(andSpecNode.RightNode, _isSatisfiedOns.Pop());
				var leftIsSatisfiedOn = WrapOrBracket(andSpecNode.LeftNode, _isSatisfiedOns.Pop());
				_isSatisfiedOns.Push($"{leftIsSatisfiedOn} {SpecGlobalConfig.AndReplacement} {rightIsSatisfiedOn}");
			}
		}

		public override void Visit(OrSpecNode<T> orSpecNode)
		{
			if (_not) Visit(MoveNotDown(new NotSpecNode<T>(orSpecNode)));
			else
			{
				base.Visit(orSpecNode);
				var rightIsSatisfiedOn = _isSatisfiedOns.Pop();
				var leftIsSatisfiedOn = _isSatisfiedOns.Pop();
				_isSatisfiedOns.Push($"{leftIsSatisfiedOn} {SpecGlobalConfig.OrReplacement} {rightIsSatisfiedOn}");
			}
		}

		public override void Visit(NotSpecNode<T> notSpecNode)
		{
			_not = !_not;
			base.Visit(notSpecNode);
		}

		public override void Visit(WrappedSpecNode<T> wrappedSpecNode)
		{
			var takenNot = _not;
			_not = false;
			base.Visit(wrappedSpecNode);
			_isSatisfiedOns.Push(takenNot
				? wrappedSpecNode.Spec.IsSatisfiedOn.Not().ToString()
				: wrappedSpecNode.Spec.IsSatisfiedOn.ToString());
		}

		public string IsSatisfiedOn => _isSatisfiedOns.Peek();

		private static ISpecNode<T> MoveNotDown(ISpecNode<T> specNode)
		{
			var deMorganSpecNodeVisitor = new DeMorganSpecNodeVisitor<T>();
			deMorganSpecNodeVisitor.Visit(new NotSpecNode<T>(specNode));
			return deMorganSpecNodeVisitor.NewRoot;
		}
	}
}