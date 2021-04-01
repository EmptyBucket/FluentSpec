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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentSpecification.Composite;
using FluentSpecification.Configuration.Builders.ManualMapping;
using FluentSpecification.MemberTraversals.Builder;
using FluentSpecification.MemberTraversals.MemberNodes;

namespace FluentSpecification.Conclusion
{
	internal class SpecConclusions<TRoot> : ISpecConclusions<TRoot>
	{
		private readonly IEnumerable<ISpecConclusion<TRoot>> _specConclusions;
		private readonly ISpecNodeMapBuilder _specNodeMapBuilder;

		public SpecConclusions(ISpecNodeMapBuilder specNodeMapBuilder)
			: this(Enumerable.Empty<ISpecConclusion<TRoot>>(), specNodeMapBuilder)
		{
		}

		public SpecConclusions(IEnumerable<ISpecConclusion<TRoot>> specConclusions, ISpecNodeMapBuilder specNodeMapBuilder) =>
			(_specConclusions, _specNodeMapBuilder) = (specConclusions, specNodeMapBuilder);

		public IEnumerator<ISpecConclusion<TRoot>> GetEnumerator() => _specConclusions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public ISpecConclusions<TRoot> Me(ICompositeSpec<TRoot> spec)
		{
			var specConclusion = new SpecConclusion<TRoot, TRoot>(new RootMemberNode<TRoot>(), spec, _specNodeMapBuilder);
			return new SpecConclusions<TRoot>(_specConclusions.Append(specConclusion), _specNodeMapBuilder);
		}

		public ISpecConclusions<TRoot> Member<TMember>(Expression<Func<TRoot, TMember>> member,
			ICompositeSpec<TMember> spec)
		{
			var memberNode = MemberTraversal.Of<TRoot>().To(member).UndefinedNode.ToOneMemberNode();
			var specConclusion = new SpecConclusion<TRoot, TMember>(memberNode, spec, _specNodeMapBuilder);
			return new SpecConclusions<TRoot>(_specConclusions.Append(specConclusion), _specNodeMapBuilder);
		}

		public ISpecConclusions<TRoot> Member<TMemberItem>(Expression<Func<TRoot, IEnumerable<TMemberItem>>> member,
			ICompositeSpec<TMemberItem> spec)
		{
			var memberNode = MemberTraversal.Of<TRoot>().To(member).UndefinedNode.ToManyMemberNode<TMemberItem>();
			var specConclusion = new SpecConclusion<TRoot, TMemberItem>(memberNode, spec, _specNodeMapBuilder);
			return new SpecConclusions<TRoot>(_specConclusions.Append(specConclusion), _specNodeMapBuilder);
		}

		public ISpecConclusions<TRoot> Nested<TPrev, T>(
			Func<IRootChainNode<TRoot>, IContinuationChainNode<TRoot, TPrev, T>> chainNodeBuilder,
			IEnumerable<ISpecConclusion<T>> specConclusionsNested)
		{
			var memberNode = chainNodeBuilder(MemberTraversal.Of<TRoot>()).UndefinedNode.ToOneMemberNode();
			var specConclusions = specConclusionsNested.Select(c => c.Rebase(memberNode));
			return new SpecConclusions<TRoot>(_specConclusions.Concat(specConclusions), _specNodeMapBuilder);
		}

		public ISpecConclusions<TRoot> Nested<TPrev, TItem>(
			Func<IRootChainNode<TRoot>, IContinuationChainNode<TRoot, TPrev, IEnumerable<TItem>>> chainNodeBuilder,
			IEnumerable<ISpecConclusion<TItem>> specConclusionsNested)
		{
			var memberNode = chainNodeBuilder(MemberTraversal.Of<TRoot>()).UndefinedNode.ToManyMemberNode<TItem>();
			var specConclusions = specConclusionsNested.Select(c => c.Rebase(memberNode));
			return new SpecConclusions<TRoot>(_specConclusions.Concat(specConclusions), _specNodeMapBuilder);
		}
	}
}