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

using System.Collections.Generic;
using FluentSpecification.AspNet.Presentations;
using FluentSpecification.Conclusion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FluentSpecification.AspNet.Filters
{
	internal class SpecExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			switch (context.Exception)
			{
				case null: return;
				case SpecSatisfactionException specSatisfactionException:
				{
					var specSatisfactionEntries = new List<SpecSatisfactionEntryRead>();

					foreach (var specSatisfaction in specSatisfactionException.Satisfactions)
					{
						var pathValueNodeVisitor = new PathValueNodeVisitor();
						pathValueNodeVisitor.Visit(specSatisfaction.VisitableValueNode);
						specSatisfactionEntries.Add(new SpecSatisfactionEntryRead(specSatisfaction.SpecNodeId,
							specSatisfaction.IsSatisfiedOn, specSatisfaction.InfluenceOn, specSatisfaction.InfluenceValue,
							pathValueNodeVisitor.Path));
					}

					var specSatisfactionRead =
						new SpecSatisfactionRead(specSatisfactionException.Type.Name, specSatisfactionEntries);
					var jsonSerializerSettings = new JsonSerializerSettings
					{
						ContractResolver = new DefaultContractResolver {NamingStrategy = new CamelCaseNamingStrategy()},
						Formatting = Formatting.Indented,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore
					};
					var content = JsonConvert.SerializeObject(specSatisfactionRead, jsonSerializerSettings);
					context.Result = new ContentResult {Content = content, StatusCode = 400, ContentType = "application/json"};
					context.ExceptionHandled = true;
					break;
				}
			}
		}
	}
}