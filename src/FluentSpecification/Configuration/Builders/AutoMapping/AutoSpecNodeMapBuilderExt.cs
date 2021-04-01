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
using System.Reflection;

namespace FluentSpecification.Configuration.Builders.AutoMapping
{
	public static class AutoSpecNodeMapBuilderExt
	{
		public static IAutoSpecNodeMapBuilder AddBoth(this IAutoSpecNodeMapBuilder autoSpecNodeMapBuilder,
			Type specType)
		{
			autoSpecNodeMapBuilder.Add(specType);
			autoSpecNodeMapBuilder.AddNot(specType);
			return autoSpecNodeMapBuilder;
		}

		public static IAutoSpecNodeMapBuilder AddForValEmbeddedTypes(
			this IAutoSpecNodeMapBuilder autoSpecNodeMapBuilder, Type specOpenGenericType) =>
			autoSpecNodeMapBuilder.AddForTypes(specOpenGenericType, EmbeddedTypes.Where(t => t.IsValueType));

		public static IAutoSpecNodeMapBuilder AddForRefEmbeddedTypes(
			this IAutoSpecNodeMapBuilder autoSpecNodeMapBuilder, Type specOpenGenericType) =>
			autoSpecNodeMapBuilder.AddForTypes(specOpenGenericType, EmbeddedTypes.Where(t => !t.IsValueType));

		public static IAutoSpecNodeMapBuilder AddForAllEmbeddedTypes(
			this IAutoSpecNodeMapBuilder autoSpecNodeMapBuilder, Type specOpenGenericType) =>
			autoSpecNodeMapBuilder.AddForTypes(specOpenGenericType, EmbeddedTypes);

		public static IAutoSpecNodeMapBuilder AddForTypes(
			this IAutoSpecNodeMapBuilder autoSpecNodeMapBuilder, Type specOpenGenericType, IEnumerable<Type> types)
		{
			AssertOpenGenericSpecType(specOpenGenericType);
			var addBothMethodInfo =
				typeof(AutoSpecNodeMapBuilderExt).GetMethod(nameof(AddBoth), BindingFlags.Static | BindingFlags.Public);

			foreach (var type in types)
			{
				var specType = specOpenGenericType.MakeGenericType(type);
				addBothMethodInfo!.Invoke(autoSpecNodeMapBuilder, new object[] {autoSpecNodeMapBuilder, specType});
			}

			return autoSpecNodeMapBuilder;
		}

		private static readonly Type[] EmbeddedTypes =
		{
			typeof(bool),

			typeof(byte),
			typeof(sbyte),
			typeof(char),

			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),

			typeof(float),
			typeof(double),
			typeof(decimal),

			typeof(string),

			typeof(Guid)
		};

		private static void AssertOpenGenericSpecType(Type specOpenGenericType)
		{
			var specInterface = typeof(ISpec<>);

			if (specOpenGenericType.IsInterface || specOpenGenericType.IsAbstract || !specOpenGenericType.IsGenericType ||
			    !specOpenGenericType.IsGenericTypeDefinition || !specOpenGenericType.GetInterfaces()
				    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == specInterface))
				throw new ArgumentException(
					$"{specOpenGenericType.FullName} must be open generic implementation of the {nameof(ISpec<object>)}");
		}
	}
}