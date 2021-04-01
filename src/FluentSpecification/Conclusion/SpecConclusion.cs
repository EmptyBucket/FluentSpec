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
using FluentSpecification.Composite;
using FluentSpecification.Composite.Nodes.Visitors;
using FluentSpecification.Configuration.Builders.ManualMapping;
using FluentSpecification.MemberTraversals.MemberNodes;

namespace FluentSpecification.Conclusion
{
	internal class SpecConclusion<TRoot, T> : ISpecConclusion<TRoot>
	{
		private readonly IMemberNode<TRoot, T> _memberNode;
		private readonly ICompositeSpec<T> _spec;
		private readonly ISpecNodeMapBuilder _specNodeMapBuilder;

		public SpecConclusion(IMemberNode<TRoot, T> memberNode, ICompositeSpec<T> spec,
			ISpecNodeMapBuilder specNodeMapBuilder) =>
			(_memberNode, _spec, _specNodeMapBuilder) = (memberNode, spec, specNodeMapBuilder);

		public ISpecConclusion<TNewRoot> Rebase<TNewRoot>(IMemberNode<TNewRoot, TRoot> memberNode) =>
			new SpecConclusion<TNewRoot, T>(_memberNode.Rebase(memberNode), _spec, _specNodeMapBuilder);

		public SpecDeclaration GetDeclaration() => new SpecDeclaration(_spec.IsSatisfiedOn, _memberNode);

		public IEnumerable<SpecSatisfaction> GetSatisfactions(TRoot root)
		{
			var deMorganSpecNodeVisitor = new DeMorganSpecNodeVisitor<T>();
			deMorganSpecNodeVisitor.Visit(_spec);
			var simplifiedSpecNode = deMorganSpecNodeVisitor.NewRoot;
			var isSatisfiedBySpecNodeVisitor = new IsSatisfiedBySpecNodeVisitor<T>();
			isSatisfiedBySpecNodeVisitor.Visit(simplifiedSpecNode);
			var isSatisfiedOn = new Lazy<string>(() =>
			{
				var isSatisfiedOnSpecNodeVisitor = new IsSatisfiedOnSpecNodeVisitor<T>();
				isSatisfiedOnSpecNodeVisitor.Visit(simplifiedSpecNode);
				return isSatisfiedOnSpecNodeVisitor.IsSatisfiedOn;
			});
			return _memberNode.ToValueNodes(root)
				.Select(n =>
				{
					var isSatisfiedBy = isSatisfiedBySpecNodeVisitor.IsSatisfiedBy(n.Value, out var influenceNode);
					var specNodeId = new Lazy<int>(() => _specNodeMapBuilder.Build<T>().Invoke(influenceNode));
					var influenceOn = new Lazy<string>(() => new CompositeSpec<T>(influenceNode).IsSatisfiedOn);
					return new SpecSatisfaction(isSatisfiedBy, specNodeId, isSatisfiedOn, influenceOn, n.Value, n);
				});
		}
	}
}