# FluentSpecification

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
	.ThrowIfNotSatisfied(matrix);

```

![Result](https://i.imgur.com/oPbCRpT.jpg)
