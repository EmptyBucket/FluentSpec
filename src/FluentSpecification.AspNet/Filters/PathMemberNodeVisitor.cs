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
using FluentSpecification.AspNet.Presentations;
using FluentSpecification.MemberTraversals.MemberNodes;
using FluentSpecification.MemberTraversals.MemberNodes.Visitors;

namespace FluentSpecification.AspNet.Filters
{
	internal class PathMemberNodeVisitor : MemberNodeVisitorBase
	{
		private readonly Queue<IPathEntryRead> _path = new Queue<IPathEntryRead>();

		public override void Visit<TRoot, TPrev, T>(ContinuationOneMemberNode<TRoot, TPrev, T> memberNode)
		{
			base.Visit(memberNode);
			_path.Enqueue(new MemberedPathEntryRead(memberNode.Member.GetMemberName()));
		}

		public override void Visit<TRoot, TPrev, T>(ContinuationManyMemberNode<TRoot, TPrev, T> memberNode)
		{
			base.Visit(memberNode);
			_path.Enqueue(new MemberedPathEntryRead(memberNode.Member.GetMemberName()));
		}

		public IEnumerable<IPathEntryRead> Path => _path;
	}
}