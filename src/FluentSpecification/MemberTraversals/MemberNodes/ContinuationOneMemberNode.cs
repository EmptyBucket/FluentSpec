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
	public class ContinuationOneMemberNode<TRoot, TPrev, T> : ContinuationMemberNodeBase<TRoot, T>
	{
		public ContinuationOneMemberNode(IMemberNode<TRoot, TPrev> prevNode,
			Expression<Func<TPrev, T>> member) : base(member) =>
			(PrevNode, Member, CompiledMember) = (prevNode, member, member.Compile());

		public IMemberNode<TRoot, TPrev> PrevNode { get; }

		public Expression<Func<TPrev, T>> Member { get; }

		public Func<TPrev, T> CompiledMember { get; }

		public override IMemberNode<TNewRoot, T> Rebase<TNewRoot>(IMemberNode<TNewRoot, TRoot> memberNode) =>
			new ContinuationOneMemberNode<TNewRoot, TPrev, T>(PrevNode.Rebase(memberNode), Member);

		public override IEnumerable<IValueNode<T>> ToValueNodes(TRoot root) =>
			PrevNode.ToValueNodes(root)
				.Where(n => n.Value != null)
				.Select(n => new MemberedValueNode<TPrev, T>(n, Member, CompiledMember.Invoke(n.Value)));

		public override void Accept(MemberNodeVisitorBase memberNodeVisitor) => memberNodeVisitor.Visit(this);
	}
}