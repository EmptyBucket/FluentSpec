# FluentSpecification

### Description of the rules
##### Consider some entity "Matrix" with the following internal structure
* Matrix
	* SmartTasks
		* Weight
		* Name
		* TargetResult
* Something other
##### Declarative description of the rules for the "Matrix" entity
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
```
public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services
			.AddMvc()
			// asp net mvc integration
			.AddSpecHandling()
			.Services
			.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo());
				//swagger integration
				c.AddSpecModels();
			});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
		app
			.UseSwagger()
			.UseSwaggerUI(c => );
}
```
### Result of the broken rule for "Matrix.SmartTasks.Weight"
* specNodeId - error identifier
* isSatisfiedOn - all the rules associated with the property
* influenceOn is a broken rule
* influenceValue The value that the rule did not pass
* path - the path to the property inside the aggregate
![Result](https://i.imgur.com/oPbCRpT.jpg)
