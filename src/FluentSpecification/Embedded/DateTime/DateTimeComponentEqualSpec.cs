// // The MIT License (MIT)
// //
// // Copyright (c) 2021 Politov Alexey
// // https://github.com/EmptyBucket/FluentSpecification
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy of
// // this software and associated documentation files (the "Software"), to deal in
// // the Software without restriction, including without limitation the rights to
// // use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// // the Software, and to permit persons to whom the Software is furnished to do so,
// // subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// // FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// // COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// // IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// // CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using FluentSpecification.Composite;

namespace FluentSpecification.Embedded
{
	public class DateTimeComponentEqualSpec : CompositeSpecLeaf<DateTime>
	{
		private readonly long? _ticks;
		private readonly int? _year;
		private readonly int? _month;
		private readonly int? _day;
		private readonly DayOfWeek? _dayOfWeek;
		private readonly int? _hour;
		private readonly int? _minute;
		private readonly int? _second;
		private readonly int? _millisecond;

		public DateTimeComponentEqualSpec(long ticks)
		{
			_ticks = ticks;
			IsSatisfiedOn = $"Value {must} be with " + PrintNotEmptyComponent(nameof(DateTime.Ticks), _ticks);
		}

		public DateTimeComponentEqualSpec(int? year = null, int? month = null, int? day = null, DayOfWeek? dayOfWeek = null,
			int? hour = null, int? minute = null, int? second = null, int? millisecond = null)
		{
			_ticks = null;
			_year = year;
			_month = month;
			_day = day;
			_dayOfWeek = dayOfWeek;
			_hour = hour;
			_minute = minute;
			_second = second;
			_millisecond = millisecond;

			IsSatisfiedOn = $"Value {must} be with " + string.Join(", ",
				PrintNotEmptyComponent(nameof(DateTime.Year), _year),
				PrintNotEmptyComponent(nameof(DateTime.Month), _month),
				PrintNotEmptyComponent(nameof(DateTime.Day), _day),
				PrintNotEmptyComponent(nameof(DateTime.DayOfWeek), _dayOfWeek),
				PrintNotEmptyComponent(nameof(DateTime.Hour), _hour),
				PrintNotEmptyComponent(nameof(DateTime.Minute), _minute),
				PrintNotEmptyComponent(nameof(DateTime.Second), _second),
				PrintNotEmptyComponent(nameof(DateTime.Millisecond), _millisecond));
		}

		public DateTimeComponentEqualSpec(int? year = null, int? month = null, int? day = null, DayOfWeek? dayOfWeek = null,
			TimeSpan? timeOfDay = null)
			: this(year, month, day, dayOfWeek, timeOfDay?.Hours, timeOfDay?.Minutes, timeOfDay?.Seconds,
				timeOfDay?.Milliseconds)
		{
		}

		public override bool IsSatisfiedBy(DateTime value)
		{
			if (_ticks != null && value.Ticks != _ticks) return false;
			if (_year != null && value.Year != _year) return false;
			if (_month != null && value.Month != _month) return false;
			if (_day != null && value.Day != _day) return false;
			if (_dayOfWeek != null && value.DayOfWeek != _dayOfWeek) return false;
			if (_hour != null && value.Hour != _hour) return false;
			if (_minute != null && value.Minute != _minute) return false;
			if (_second != null && value.Second != _second) return false;
			if (_millisecond != null && value.Millisecond != _millisecond) return false;
			return true;
		}

		public override SpecCondition IsSatisfiedOn { get; }

		private static string PrintNotEmptyComponent<T>(string componentName, T? value) where T : struct =>
			value is null ? string.Empty : $"component of the {componentName} equal {value.Value}";
	}
}