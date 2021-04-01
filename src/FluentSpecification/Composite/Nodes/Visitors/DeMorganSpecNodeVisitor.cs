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

namespace FluentSpecification.Composite.Nodes.Visitors
{
	//todo сделать mediator, чтобы лишь 1 раз обходить дерево другими визиторами
	internal class DeMorganSpecNodeVisitor<T> : SpecNodeVisitorBase<T>
	{
		private readonly Stack<bool> _notNodes = new Stack<bool>(new[] {false});
		private readonly Stack<ISpecNode<T>> _nodes = new Stack<ISpecNode<T>>();

		public override void Visit(AndSpecNode<T> andSpecNode)
		{
			var takenNot = _notNodes.Peek();
			_notNodes.Push(takenNot);
			base.Visit(andSpecNode);
			var rightNode = _nodes.Pop();
			var leftNode = _nodes.Pop();
			_nodes.Push(takenNot
				? (ISpecNode<T>) new OrSpecNode<T>(leftNode, rightNode)
				: rightNode != andSpecNode.RightNode || leftNode != andSpecNode.LeftNode
					? new AndSpecNode<T>(leftNode, rightNode)
					: andSpecNode);
		}

		public override void Visit(OrSpecNode<T> orSpecNode)
		{
			var takenNot = _notNodes.Peek();
			_notNodes.Push(takenNot);
			base.Visit(orSpecNode);
			var rightNode = _nodes.Pop();
			var leftNode = _nodes.Pop();
			_nodes.Push(takenNot
				? (ISpecNode<T>) new AndSpecNode<T>(leftNode, rightNode)
				: rightNode != orSpecNode.RightNode || leftNode != orSpecNode.LeftNode
					? new OrSpecNode<T>(leftNode, rightNode)
					: orSpecNode);
		}

		public override void Visit(NotSpecNode<T> notSpecNode)
		{
			_notNodes.Push(!_notNodes.Pop());
			base.Visit(notSpecNode);
		}

		public override void Visit(WrappedSpecNode<T> wrappedSpecNode)
		{
			var takenNot = _notNodes.Pop();
			base.Visit(wrappedSpecNode);
			_nodes.Push(takenNot ? (ISpecNode<T>) new NotSpecNode<T>(wrappedSpecNode) : wrappedSpecNode);
		}

		public ISpecNode<T> NewRoot => _nodes.Peek();
	}
}