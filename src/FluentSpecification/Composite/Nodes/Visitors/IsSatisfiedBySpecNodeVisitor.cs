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
	internal class IsSatisfiedBySpecNodeVisitor<T> : SpecNodeVisitorBase<T>
	{
		private readonly Stack<Func<T, (bool Success, ISpecNode<T> InfluenceSpecNode)>> _isSatisfiedBys =
			new Stack<Func<T, (bool Success, ISpecNode<T> InfluenceSpecNode)>>();

		public override void Visit(AndSpecNode<T> andSpecNode)
		{
			base.Visit(andSpecNode);
			var rightIsSatisfiedBy = _isSatisfiedBys.Pop();
			var leftIsSatisfiedBy = _isSatisfiedBys.Pop();
			_isSatisfiedBys.Push(a => leftIsSatisfiedBy(a) is var (leftSuccess, leftInfluenceSpecNode) && leftSuccess
				? rightIsSatisfiedBy(a)
				: (leftSuccess, leftInfluenceSpecNode));
		}

		public override void Visit(OrSpecNode<T> orSpecNode)
		{
			base.Visit(orSpecNode);
			var rightIsSatisfiedBy = _isSatisfiedBys.Pop();
			var leftIsSatisfiedBy = _isSatisfiedBys.Pop();
			_isSatisfiedBys.Push(a => leftIsSatisfiedBy(a) is var (leftSuccess, leftInfluenceSpecNode) && leftSuccess
				? (leftSuccess, leftInfluenceSpecNode)
				: rightIsSatisfiedBy(a));
		}

		public override void Visit(NotSpecNode<T> notSpecNode)
		{
			if (notSpecNode.NestedNode is WrappedSpecNode<T>)
			{
				base.Visit(notSpecNode);
				var isSatisfiedBy = _isSatisfiedBys.Pop();
				_isSatisfiedBys.Push(a => (!isSatisfiedBy(a).Success, notSpecNode));
			}
			else Visit(MoveNotDown(notSpecNode));
		}

		public override void Visit(WrappedSpecNode<T> wrappedSpecNode)
		{
			base.Visit(wrappedSpecNode);
			_isSatisfiedBys.Push(a => (wrappedSpecNode.Spec.IsSatisfiedBy(a), wrappedSpecNode));
		}

		public bool IsSatisfiedBy(T value) => _isSatisfiedBys.Peek().Invoke(value).Success;

		public bool IsSatisfiedBy(T value, out ISpecNode<T> influenceSpecNode)
		{
			var (success, infSpecNode) = _isSatisfiedBys.Peek().Invoke(value);
			influenceSpecNode = infSpecNode;
			return success;
		}

		private static ISpecNode<T> MoveNotDown(ISpecNode<T> specNode)
		{
			var deMorganSpecNodeVisitor = new DeMorganSpecNodeVisitor<T>();
			deMorganSpecNodeVisitor.Visit(new NotSpecNode<T>(specNode));
			return deMorganSpecNodeVisitor.NewRoot;
		}
	}
}