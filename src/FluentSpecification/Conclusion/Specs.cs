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

using System.Linq;
using FluentSpecification.Configuration;
using FluentSpecification.Configuration.Builders.ManualMapping;

namespace FluentSpecification.Conclusion
{
	public static class Specs
	{
		public static ISpecConclusions<T> For<T>() => For<T>(SpecGlobalConfig.DefaultSpecNodeMapBuilder);

		public static ISpecConclusions<T> For<T>(ISpecNodeMapBuilder specNodeMapBuilder) =>
			new SpecConclusions<T>(specNodeMapBuilder);

		public static void ThrowIfNotSatisfied<T>(this ISpecConclusions<T> specConclusions, T value)
		{
			var specSatisfactions = specConclusions
				.SelectMany(c => c.GetSatisfactions(value))
				.Where(s => !s.IsSatisfied)
				.ToArray();

			if (specSatisfactions.Length > 0) throw new SpecSatisfactionException(typeof(T), specSatisfactions);
		}
	}
}