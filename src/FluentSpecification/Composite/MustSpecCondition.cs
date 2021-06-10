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
using FluentSpecification.Configuration;

namespace FluentSpecification.Composite
{
	internal class MustSpecCondition : SpecCondition
	{
		private readonly string _origin;
		private readonly string _human;

		public MustSpecCondition(string isSatisfiedOn)
		{
			_origin = isSatisfiedOn;

			if (!TryReplaceSingle(isSatisfiedOn, SpecSpecialKeyword.must, SpecGlobalConfig.MustReplacement,
				    out _human) &&
			    !TryReplaceSingle(isSatisfiedOn, SpecSpecialKeyword.must_not, SpecGlobalConfig.MustNotReplacement,
				    out _human))
				throw new ArgumentException(
					$"{nameof(isSatisfiedOn)} must be contains single " +
					$"{nameof(SpecSpecialKeyword.must)} or {nameof(SpecSpecialKeyword.must_not)} special keyword");
		}

		public override SpecCondition Not()
		{
			var _ = TryReplaceSingle(_origin, SpecSpecialKeyword.must, SpecSpecialKeyword.must_not, out var not) ||
			        TryReplaceSingle(_origin, SpecSpecialKeyword.must_not, SpecSpecialKeyword.must, out not);
			return new MustSpecCondition(not);
		}

		public override string ToString() => _human;

		private static bool TryReplaceSingle(string origin, string oldValue, string newValue, out string result)
		{
			var oldValueStartIndex = origin.IndexOf(oldValue, StringComparison.Ordinal);

			if (oldValueStartIndex >= 0 && oldValueStartIndex + oldValue.Length is var oldValueEndIndex &&
			    origin.IndexOf(oldValue, oldValueEndIndex, StringComparison.Ordinal) < 0)
			{
				result = origin.Substring(0, oldValueStartIndex) + newValue +
				         origin.Substring(oldValueEndIndex, origin.Length - oldValueEndIndex);
				return true;
			}

			result = origin;
			return false;
		}
	}
}