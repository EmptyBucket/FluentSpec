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
using System.Linq.Expressions;
using FluentSpecification.Composite;
using FluentSpecification.MemberTraversals.Builder;

namespace FluentSpecification.Conclusion
{
	public interface ISpecConclusions<TRoot> : IEnumerable<ISpecConclusion<TRoot>>
	{
		ISpecConclusions<TRoot> Me(ICompositeSpec<TRoot> spec);

		ISpecConclusions<TRoot> Member<TMember>(Expression<Func<TRoot, TMember>> member,
			ICompositeSpec<TMember> spec);

		ISpecConclusions<TRoot> Member<TMemberItem>(Expression<Func<TRoot, IEnumerable<TMemberItem>>> member,
			ICompositeSpec<TMemberItem> spec);

		ISpecConclusions<TRoot> Nested<TPrev, T>(
			Func<IRootChainNode<TRoot>, IContinuationChainNode<TRoot, TPrev, T>> chainNodeBuilder,
			IEnumerable<ISpecConclusion<T>> specConclusionsNested);

		ISpecConclusions<TRoot> Nested<TPrev, TItem>(
			Func<IRootChainNode<TRoot>, IContinuationChainNode<TRoot, TPrev, IEnumerable<TItem>>> chainNodeBuilder,
			IEnumerable<ISpecConclusion<TItem>> specConclusionsNested);
	}
}