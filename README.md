# FluentSpecification

### Description of the rules
##### Consider some entity "Matrix" with the following internal structure:
* Matrix
	* SmartTasks
		* Weight
		* Name
		* TargetResult
* Something other
##### Declarative description of the rules for the "Matrix" entity:
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
### ASP.NET MVC and Swagger integration
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
* influenceOn is a broken rule
* influenceValue The value that the rule did not pass
* path - the path to the property inside the aggregate
