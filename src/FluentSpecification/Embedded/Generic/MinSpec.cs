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
using FluentSpecification.Composite;

namespace FluentSpecification.Embedded
{
	public class MinSpec<T> : CompositeSpecLeaf<T>
		where T : IComparable<T>
	{
		private readonly T _threshold;
		private readonly IComparer<T> _comparer;

		public MinSpec(T threshold) : this(threshold, Comparer<T>.Default)
		{
		}

		public MinSpec(T threshold, IComparer<T> comparer) => (_threshold, _comparer) = (threshold, comparer);

		public override bool IsSatisfiedBy(T value)
		{
			var compareTo = _comparer.Compare(value, _threshold);
			return compareTo == 0 || compareTo == 1;
		}

		public override SpecCondition IsSatisfiedOn => $"Value {must_not} inferior {_threshold}";
	}
}