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
using FluentSpecification.Composite;

namespace FluentSpecification.Embedded
{
	public class UniqueSpec<T> : CompositeSpecLeaf<IEnumerable<T>>
	{
		private readonly Func<T, object> _valueSelector;

		public UniqueSpec(Expression<Func<T, object>> expression)
		{
			switch (expression.Body)
			{
				case UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression:
					IsSatisfiedOn = $"Value {must} be unique by {memberExpression.Member.Name}";
					break;
				case NewExpression newExpression:
				{
					var members = new List<string>();

					foreach (var argument in newExpression.Arguments)
						if (argument is MemberExpression memberExpression) members.Add(memberExpression.Member.Name);
						else goto default;

					IsSatisfiedOn = $"Value {must} be unique by {string.Join(", ", members)}";
					break;
				}
				default:
					IsSatisfiedOn = $"Value {must} be unique";
					break;
			}

			_valueSelector = expression.Compile();
		}

		public override bool IsSatisfiedBy(IEnumerable<T> value) => value.GroupBy(_valueSelector).All(g => g.Count() == 1);

		public override SpecCondition IsSatisfiedOn { get; }
	}
}