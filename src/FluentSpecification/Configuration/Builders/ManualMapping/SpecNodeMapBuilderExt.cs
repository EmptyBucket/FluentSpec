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
using FluentSpecification.Configuration.Builders.AutoMapping;

namespace FluentSpecification.Configuration.Builders.ManualMapping
{
	public static class SpecNodeMapBuilderExt
	{
		public static ISpecNodeMapBuilder AddBoth(this ISpecNodeMapBuilder specNodeMapBuilder, Type specType,
			int id, int notId)
		{
			specNodeMapBuilder.Add(specType, id);
			specNodeMapBuilder.AddNot(specType, notId);
			return specNodeMapBuilder;
		}

		private const int ReservedThreshold = -1;

		internal static ISpecNodeMapBuilder StartFromReserved(this ISpecNodeMapBuilder specNodeMapBuilder,
			Action<IAutoSpecNodeMapBuilder> action) =>
			StartFromImpl(specNodeMapBuilder, ReservedThreshold, id => id - 1, action);

		public static ISpecNodeMapBuilder StartFrom(this ISpecNodeMapBuilder specNodeMapBuilder, int startId,
			Action<IAutoSpecNodeMapBuilder> action)
		{
			if (startId <= ReservedThreshold || startId == SpecNodeMapBuilder.UndefinedId)
				throw new ArgumentException($"{startId} must be more than reserved ids({ReservedThreshold}) and " +
				                            $"not equal undefined id({SpecNodeMapBuilder.UndefinedId})");

			return StartFromImpl(specNodeMapBuilder, startId, id => id + 1, action);
		}

		private static ISpecNodeMapBuilder StartFromImpl(ISpecNodeMapBuilder specNodeMapBuilder, int startId,
			Func<int, int> func, Action<IAutoSpecNodeMapBuilder> action)
		{
			action(new AutoSpecNodeMapBuilder(specNodeMapBuilder, startId, func));
			return specNodeMapBuilder;
		}
	}
}