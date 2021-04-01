# FluentSpecification

#### The tasks that were set for the library:
1) [Defining your own specifications](https://github.com/EmptyBucket/FluentSpecification#defining-your-own-specifications)
2) [Combining specifications](https://github.com/EmptyBucket/FluentSpecification#combining-specifications)
3) [Description of aggregate rules](https://github.com/EmptyBucket/FluentSpecification#description-of-aggregate-rules)
4) [Getting a declaration - a list of rules](https://github.com/EmptyBucket/FluentSpecification#getting-a-declaration---a-list-of-rules)
5) [ASP.NET MVC and Swagger integration](https://github.com/EmptyBucket/FluentSpecification#aspnet-mvc-and-swagger-integration)
6) [Getting a detailed description of the specification violation](https://github.com/EmptyBucket/FluentSpecification#result-of-the-broken-rule-for-matrixsmarttasksweight)

## Defining your own specifications
#### Specifications in their most canonical form as described in DDD:
```csharp
public class FormulaMaxDepthSpec : CompositeSpecLeaf<ParseTreeNode>
{
	public override bool IsSatisfiedBy(ParseTreeNode value) =>
		new FormulaAnalyzer(value).OperatorDepth(ExcelFormula.AllowedNamedFunctions) <= 65;

	public override SpecCondition IsSatisfiedOn => $"Depth {must_not} exceed 64";
}
```
```must``` and ```must_not``` are special reserved words that you must use when implicitly using ```MustSpecConfition``` or use explicit ```PredefinedSpecCondition```, where you explicitly negate the string literal
## Combining specifications
#### Combine specifications using unions ```And```, ```Or``` and apply negation ```Not``` to them:
```csharp
new NullSpec<decimal>()
	.Or(new MinSpec<decimal>(0)
		.And(new MaxSpec<decimal>(100))
		.And(new EqualsSpec<decimal>(3).Not()))
	.Not()

```
As a result, a tree will be formed in which we can lower _not_ to leaves using de Morgan's law, and then apply negation to the specification, which is why we use ```SpecCondition``` implementations instead of ordinary strings - they know how to build their negation. You will receive a specification that will comply with:
>Value must not be null and (Value must inferior 0 or Value must exceed 100 or Value must be equals 3)
#### ```Replace``` the specification base if it does not suit you, or if you want to bring several specifications to the same base for the purpose of their further combination:
```csharp
new FormulaParsedSpec()
	.And(new FormulaMaxDepthSpec())
	.And(new FormulaAllowedSpec())
	.And(new FormulaHasExistVariablesSpec(excelVariables))
	.Replace<ParseTreeNode?, string>(ParseFormula);
```
In this example, we have replaced ```ISpec<ParseTreeNode>``` with ```ISpec<string>```
## Description of aggregate rules
#### Consider some root ```Matrix``` with the following internal structure:
* __Matrix__
	* __SmartTasks__
		* __Weight__
		* __Name__
		* __TargetResult__
* __Something other__
#### Use declarative fluent api builder for description of the rules for the ```Matrix``` root:
```csharp
Specs
	.For<Matrix>()
	.Nested(b => b.To(m => m.SmartTasks), Specs
		.For<SmartTask>()
		.Member(k => k.Weight, new MoreSpec<decimal>(0).And(new DecimalFractionMaxLengthSpec(2)))
		.Member(s => s.Name, new StringNotEmptySpec()
			.And(new StringMaxLengthSpec(100))
			.And(new StringNotContinuousSpacesSpec())
			.And(new StringNotEdgeSpaceSpec())
			.And(new StringMatchSpec("\n").Not()))
		.Member(s => s.TargetResult, new NullSpec<string>().Or(new StringMaxLengthSpec(200)
			.And(new StringNotContinuousSpacesSpec())
			.And(new StringNotEdgeSpaceSpec())
			.And(new StringMatchSpec("\n").Not()))!))
	.ThrowIfNotSatisfied(matrix);

```
* ```For``` - grab context for ```T``` root
* ```Me``` - define a rule for the captured root
* ```Member``` - define the rule for the member of the captured root
* ```Nested``` - traverse the members of the captured root up to a certain nesting level
* ```ThrowIfNotSatisfied``` - throw an exception if at least one rule was violated for an entity
## Getting a declaration - a list of rules
```csharp
Specs
	.For<Matrix>()
	.Nested(m => m.To(ma => ma.SmartTasks), Specs
		.For<SmartTask>()
		.Member(k => k.Weight, new MoreSpec<decimal>(0).And(new DecimalFractionMaxLengthSpec(2)))
		.Member(s => s.Name, new StringNotEmptySpec()
			.And(new StringMaxLengthSpec(100))
			.And(new StringNotContinuousSpacesSpec())
			.And(new StringNotEdgeSpaceSpec())
			.And(new StringMatchSpec("\n").Not()))
		.Member(s => s.TargetResult, new NullSpec<string>().Or(new StringMaxLengthSpec(200)
			.And(new StringNotContinuousSpacesSpec())
			.And(new StringNotEdgeSpaceSpec())
			.And(new StringMatchSpec("\n").Not()))!))
	.Select(i => i.GetDeclaration())
```
We can do this in view of the fact that we define all the rules declaratively
## ASP.NET MVC and Swagger integration
#### Connecting integration in your startup:
```csharp
public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services
			.AddMvc()
			// connecting asp.net mvc filters for errors handling
			.AddSpecHandling()
			.Services
			.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo());
				// connecing swagger models
				c.AddSpecModels();
			});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
		app
			.UseSwagger()
			.UseSwaggerUI(c => );
}
```
#### Comparison of errors and their codes (All specs built into the library are already mapped into native codes):
```csharp
SpecGlobalConfig.DefaultSpecNodeMapBuilder = SpecGlobalConfig.DefaultSpecNodeMapBuilder
	.StartFrom(10, b => b
		.Add(typeof(FalseSpec))
		.AddNot(typeof(TrueSpec))
		.AddBoth(typeof(DateTimeZeroTimeSpec))
		.AddForAllEmbeddedTypes(typeof(EqualsSpec<>)))
	.Add(typeof(DecimalFractionMaxLengthSpec), 1)
	.AddNot(typeof(StringCharactersSpec), 2)
	.AddBoth(typeof(StringMatchSpec), 3, 4);

```
#### Result of the broken rule for ```Matrix.SmartTasks.Weight```:
![Result](https://i.imgur.com/oPbCRpT.jpg)

* ```specNodeId``` - error identifier
* ```isSatisfiedOn``` - all the rules associated with the property
* ```influenceOn``` - is a broken rule
* ```influenceValue``` - the value that the rule did not pass
* ```path``` - the path to the property inside the aggregate
### You can change the behavior of fundamental things using ```SpecGlobalConfig```
## License

```
MIT License

Copyright (c) 2021 Alexey Politov
https://github.com/EmptyBucket/FluentSpecification

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
