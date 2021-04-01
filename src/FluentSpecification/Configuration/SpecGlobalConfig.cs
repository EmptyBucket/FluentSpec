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
using FluentSpecification.Composite;
using FluentSpecification.Configuration.Builders.AutoMapping;
using FluentSpecification.Configuration.Builders.ManualMapping;
using FluentSpecification.Embedded;
using FluentSpecification.Embedded.Bool;

namespace FluentSpecification.Configuration
{
	public static class SpecGlobalConfig
	{
		public static Func<string, SpecCondition> DefaultSpecConditionFactory { get; set; } =
			s => new MustSpecCondition(s);

		public static ISpecNodeMapBuilder DefaultSpecNodeMapBuilder { get; set; } = new SpecNodeMapBuilder()
			.StartFromReserved(b => b
				.AddBoth(typeof(FalseSpec))
				.AddBoth(typeof(TrueSpec))
				.AddBoth(typeof(DateTimeZeroTimeSpec))
				.AddBoth(typeof(DecimalFractionMaxLengthSpec))
				.AddBoth(typeof(StringCharactersSpec))
				.AddBoth(typeof(StringMatchSpec))
				.AddBoth(typeof(StringMaxLengthSpec))
				.AddBoth(typeof(StringNotContinuousSpacesSpec))
				.AddBoth(typeof(StringNotEdgeSpaceSpec))
				.AddBoth(typeof(StringNotEmptySpec))
				.AddForAllEmbeddedTypes(typeof(EqualsSpec<>))
				.AddForAllEmbeddedTypes(typeof(ExistAmongSpec<>))
				.AddForAllEmbeddedTypes(typeof(LessSpec<>))
				.AddForAllEmbeddedTypes(typeof(MaxSpec<>))
				.AddForAllEmbeddedTypes(typeof(MinSpec<>))
				.AddForAllEmbeddedTypes(typeof(MoreSpec<>))
				.AddForRefEmbeddedTypes(typeof(NullSpec<>))
				.AddForAllEmbeddedTypes(typeof(UniqueSpec<>)));

		public static string MustReplacement { get; set; } = "must";

		public static string MustNotReplacement { get; set; } = "must not";

		public static string AndReplacement { get; set; } = "and";

		public static string OrReplacement { get; set; } = "or";

		public static string MustSpecialKeyword
		{
			get => SpecSpecialKeyword.must;
			set => SpecSpecialKeyword.must = value;
		}

		public static string MustNotSpecialKeyword
		{
			get => SpecSpecialKeyword.must_not;
			set => SpecSpecialKeyword.must_not = value;
		}
	}
}