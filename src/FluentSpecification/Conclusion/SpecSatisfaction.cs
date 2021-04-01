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
using FluentSpecification.MemberTraversals.ValueNodes;

namespace FluentSpecification.Conclusion
{
	public class SpecSatisfaction
	{
		private readonly Lazy<string> _isSatisfiedOn;
		private readonly Lazy<string> _influenceOn;
		private readonly Lazy<int> _specNodeId;

		public SpecSatisfaction(bool isSatisfied, Lazy<int> specNodeId, Lazy<string> isSatisfiedOn,
			Lazy<string> influenceOn, object? influenceValue, IVisitableValueNode visitableValueNode) =>
			(IsSatisfied, _specNodeId, _isSatisfiedOn, _influenceOn, InfluenceValue, VisitableValueNode) =
			(isSatisfied, specNodeId, isSatisfiedOn, influenceOn, influenceValue, visitableValueNode);

		public int SpecNodeId => _specNodeId.Value;

		public bool IsSatisfied { get; }

		public string IsSatisfiedOn => _isSatisfiedOn.Value;

		public string InfluenceOn => _influenceOn.Value;

		public object? InfluenceValue { get; }

		public IVisitableValueNode VisitableValueNode { get; }
	}
}