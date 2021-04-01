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
using System.Linq;
using System.Linq.Expressions;
using FluentSpecification.MemberTraversals.MemberNodes.Visitors;
using FluentSpecification.MemberTraversals.ValueNodes;

namespace FluentSpecification.MemberTraversals.MemberNodes
{
	public class ContinuationManyMemberNode<TRoot, TPrev, T> : ContinuationMemberNodeBase<TRoot, T>
	{
		public ContinuationManyMemberNode(IMemberNode<TRoot, TPrev> prevNode,
			Expression<Func<TPrev, IEnumerable<T>>> member) : base(member) =>
			(PrevNode, Member, CompiledMember) = (prevNode, member, member.Compile());

		public IMemberNode<TRoot, TPrev> PrevNode { get; }

		public Expression<Func<TPrev, IEnumerable<T>>> Member { get; }

		public Func<TPrev, IEnumerable<T>> CompiledMember { get; }

		public override IMemberNode<TNewRoot, T> Rebase<TNewRoot>(IMemberNode<TNewRoot, TRoot> memberNode) =>
			new ContinuationManyMemberNode<TNewRoot, TPrev, T>(PrevNode.Rebase(memberNode), Member);

		public override IEnumerable<IValueNode<T>> ToValueNodes(TRoot root)
		{
			foreach (var prevNode in PrevNode.ToValueNodes(root).Where(n => n.Value != null))
			{
				var enumerable = CompiledMember.Invoke(prevNode.Value);

				if (enumerable != null)
				{
					var array = enumerable.ToArray();
					var memberedValueNode = new MemberedValueNode<TPrev, IEnumerable<T>>(prevNode, Member, array);

					for (var i = 0; i < array.Length; i++)
						yield return new IndexedValueNode<IEnumerable<T>, T>(memberedValueNode, i, array[i]);
				}
			}
		}

		public override void Accept(MemberNodeVisitorBase memberNodeVisitor) => memberNodeVisitor.Visit(this);
	}
}