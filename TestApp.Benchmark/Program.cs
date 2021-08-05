using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace TestApp.Benchmark
{
	class Program
	{
		static void Main (string[] args)
		{
			var summary = BenchmarkRunner.Run<MyTest> ();
		}
	}

	public class MyTest
	{
		[Benchmark]
		public double AnyMoment ()
		{
			return TestMethod ("*.*.* * *:*:*.*");
		}

		[Benchmark]
		public double HourStart ()
		{
			return TestMethod ("*.*.* *:0:0");
		}

		[Benchmark]
		public double DayStart ()
		{
			return TestMethod ("*.*.* 0:0:0");
		}

		[Benchmark]
		public double MonthStart ()
		{
			return TestMethod ("*.*.1 0:0:0");
		}

		private double TestMethod (string scheduleStr)
		{
			var schedule = new Schedule (scheduleStr);
			var minDate = new DateTime (2020, 12, 15);
			var maxDate = new DateTime (2021, 1, 15);
			double result = 0;
			for (var time = minDate; time < maxDate; time = time.AddMilliseconds (12345))
			{
				var nextTime = schedule.NextEvent (time);
				var lastTime = schedule.PrevEvent (time);
				result += (nextTime - lastTime).TotalMilliseconds;
			}
			return result;
		}
	}
}
