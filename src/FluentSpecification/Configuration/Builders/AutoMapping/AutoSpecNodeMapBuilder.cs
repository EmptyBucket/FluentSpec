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
using FluentSpecification.Configuration.Builders.ManualMapping;

namespace FluentSpecification.Configuration.Builders.AutoMapping
{
	internal class AutoSpecNodeMapBuilder : IAutoSpecNodeMapBuilder
	{
		private readonly ISpecNodeMapBuilder _specNodeMapBuilder;
		private int _currentId;
		private readonly Func<int, int> _changeId;

		public AutoSpecNodeMapBuilder(ISpecNodeMapBuilder specNodeMapBuilder, int startId, Func<int, int> changeId) =>
			(_specNodeMapBuilder, _currentId, _changeId) = (specNodeMapBuilder, startId, changeId);

		public IAutoSpecNodeMapBuilder Add(Type specType)
		{
			_specNodeMapBuilder.Add(specType, _currentId = _changeId(_currentId));
			return this;
		}

		public IAutoSpecNodeMapBuilder AddNot(Type specType)
		{
			_specNodeMapBuilder.AddNot(specType, _currentId = _changeId(_currentId));
			return this;
		}

		public SpecNodeMap<T> Build<T>() => _specNodeMapBuilder.Build<T>();
	}
}