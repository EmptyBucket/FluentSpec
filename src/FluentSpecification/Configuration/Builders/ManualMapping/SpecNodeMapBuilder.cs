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
using FluentSpecification.Composite.Nodes;
using FluentSpecification.Composite.Nodes.Visitors;

namespace FluentSpecification.Configuration.Builders.ManualMapping
{
	internal class SpecNodeMapBuilder : ISpecNodeMapBuilder
	{
		public const int UndefinedId = 0;

		private readonly Dictionary<(Type SpecType, bool HasNot), int> _map =
			new Dictionary<(Type SpecType, bool HasNot), int>();

		public ISpecNodeMapBuilder Add(Type specType, int id)
		{
			AssertSpecType(specType);
			_map.Add((specType, false), id);
			return this;
		}

		public ISpecNodeMapBuilder AddNot(Type specType, int id)
		{
			AssertSpecType(specType);
			_map.Add((specType, true), id);
			return this;
		}

		public SpecNodeMap<T> Build<T>() => Map<T>;

		private int Map<T>(ISpecNode<T> specNode)
		{
			var specTypeSpecNodeVisitor = new SpecTypeSpecNodeVisitor<T>();
			specTypeSpecNodeVisitor.Visit(specNode);
			return _map.TryGetValue((specTypeSpecNodeVisitor.SpecType!, specTypeSpecNodeVisitor.HasNot), out var specNodeId)
				? specNodeId
				: UndefinedId;
		}

		private static void AssertSpecType(Type specType)
		{
			var specInterface = typeof(ISpec<>);

			if (specType.IsInterface || specType.IsAbstract || !specType.GetInterfaces()
				.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == specInterface))
				throw new ArgumentException($"{specType.FullName} must be implementation of the {nameof(ISpec<object>)}");
		}

		private class SpecTypeSpecNodeVisitor<T> : SpecNodeVisitorBase<T>
		{
			private bool _hasVisitedLeaf;

			public override void Visit(AndSpecNode<T> andSpecNode)
			{
				if (HasNot) Visit(MoveNotDown(new NotSpecNode<T>(andSpecNode)));
				else base.Visit(andSpecNode);
			}

			public override void Visit(OrSpecNode<T> orSpecNode)
			{
				if (HasNot) Visit(MoveNotDown(new NotSpecNode<T>(orSpecNode)));
				else base.Visit(orSpecNode);
			}

			public override void Visit(NotSpecNode<T> notSpecNode)
			{
				HasNot = !HasNot;
				base.Visit(notSpecNode);
			}

			public override void Visit(WrappedSpecNode<T> wrappedSpecNode)
			{
				if (_hasVisitedLeaf) throw new Exception("Subtree must be tree with one leaf");

				_hasVisitedLeaf = true;
				var takenNot = HasNot;
				HasNot = false;
				base.Visit(wrappedSpecNode);
				HasNot = takenNot;
				var specType = wrappedSpecNode.Spec.GetType();

				SpecType = specType.IsGenericType && specType.GetGenericTypeDefinition() == typeof(ReplacementSpec<,>)
					? specType.GetProperty(nameof(ReplacementSpec<object, object>.OriginSpec))!.GetValue(wrappedSpecNode.Spec)
						.GetType()
					: specType;
			}

			public bool HasNot { get; private set; }

			public Type? SpecType { get; private set; }

			private static ISpecNode<T> MoveNotDown(ISpecNode<T> specNode)
			{
				var deMorganSpecNodeVisitor = new DeMorganSpecNodeVisitor<T>();
				deMorganSpecNodeVisitor.Visit(new NotSpecNode<T>(specNode));
				return deMorganSpecNodeVisitor.NewRoot;
			}
		}
	}
}