# FluentSpecification

## The tasks that were set for the library:
1) Save the specifications in the most canonical form, which was described in DDD
2) Ability to combine specifications using unions and, or and apply negation not to them (as a result, a tree is formed in which we can descend not to leaves, and then apply not to the string literal of the specification, which is why you must use the implicit MustSpecCondition, which obliges to use {must} or {must_not} to invert it later, or explicit PredefinedSpecCondition, where you explicitly negate the string literal)
3) Provide fluent api to describe aggregate rules
4) Obtaining a complete list of rules for the aggregate in the form of text (we can do this in view of the fact that we define all the rules declaratively)
5) Integration with frameworks
6) Getting a detailed description of the specification violation

## Defining your own specifications
```
public class FormulaMaxDepthSpec : CompositeSpecLeaf<ParseTreeNode>
{
	public override bool IsSatisfiedBy(ParseTreeNode value) =>
		new FormulaAnalyzer(value).OperatorDepth(ExcelFormula.AllowedNamedFunctions) <= 65;

	public override SpecCondition IsSatisfiedOn => $"Depth {must_not} exceed 64";
}

```
## Combining specifications
```
new NullSpec<decimal>()
	.Or(new MinSpec<decimal>(0)
		.And(new MaxSpec<decimal>(100))
		.And(new EqualsSpec<decimal>(3).Not()))
	.Not()

```
As a result, you will receive a specification that will comply with:

Value must not be null and (Value must inferior 0 or Value must exceed 100 or Value must be equals 3)
## Description of aggregate rules
#### Consider some entity "Matrix" with the following internal structure:
* Matrix
	* SmartTasks
		* Weight
		* Name
		* TargetResult
* Something other
#### Declarative description of the rules for the "Matrix" entity:
```
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
## Getting a declaration - a list of rules
```
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
## ASP.NET MVC and Swagger integration
#### Connecting integration in your startup
```
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
#### Comparison of errors and their codes (All specs built into the library are already mapped into native codes)
```
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
#### Result of the broken rule for "Matrix.SmartTasks.Weight":
![Result](https://i.imgur.com/oPbCRpT.jpg)

* specNodeId - error identifier
* isSatisfiedOn - all the rules associated with the property
* influenceOn - is a broken rule
* influenceValue - the value that the rule did not pass
* path - the path to the property inside the aggregate
