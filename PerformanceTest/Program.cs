using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Okanshi;

namespace PerformanceTest
{
	/*
                                     Method |     Mean |     Error |   StdDev | Rank |
------------------------------------------- |---------:|----------:|---------:|-----:|
 RegisterFuncUsingTimerFromRegistryWithTags | 724.9 ns | 10.122 ns | 9.468 ns |    5 |
         RegisterFuncUsingTimerFromRegistry | 534.7 ns |  7.731 ns | 7.232 ns |    4 |
                               RegisterFunc | 269.4 ns |  3.278 ns | 3.066 ns |    3 |
                             RegisterAction | 249.3 ns |  3.262 ns | 3.051 ns |    2 |
               Register2Action_ResetClosure | 241.2 ns |  3.461 ns | 3.237 ns |    1 |
                            Register2Action | 239.3 ns |  2.865 ns | 2.680 ns |    1 |

And performance when two timers exist. Interestingly, the timer with tags is faster than the one without tags.


                                     Method |       Mean |     Error |    StdDev | Rank |
------------------------------------------- |-----------:|----------:|----------:|-----:|
         RegisterFuncUsingTimerFromRegistry | 1,159.8 ns | 12.973 ns | 12.135 ns |    6 |
 RegisterFuncUsingTimerFromRegistryWithTags |   733.9 ns |  8.320 ns |  7.783 ns |    5 |
                               RegisterFunc |   268.3 ns |  3.180 ns |  2.974 ns |    4 |
                             RegisterAction |   250.5 ns |  3.699 ns |  3.460 ns |    3 |
               Register2Action_ResetClosure |   242.4 ns |  2.973 ns |  2.781 ns |    2 |
                            Register2Action |   237.4 ns |  3.399 ns |  3.179 ns |    1 |



// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  Rank   : Relative position of current benchmark mean among all benchmarks (Arabic style)
  1 ns   : 1 Nanosecond (0.000000001 sec)
  */
	[ClrJob]
		[RPlotExporter, RankColumn]
		public class Md5VsSha256
		{
			private int x = 0;
			Timer timer = OkanshiMonitor.Timer("timer");
	
			[GlobalSetup]
			public void Setup()
			{
				OkanshiMonitor.Timer("RegisterFuncUsingTimerFromRegistry");
				OkanshiMonitor.Timer("RegisterFuncUsingTimerFromRegistryWithTags",
					new[] {new Tag("key", "val"), new Tag("Environment", "production"), new Tag("Tic", "Tac")});
			}

			[Benchmark]
			public int RegisterFunc() => timer.Record(() => 1);

			[Benchmark]
			public int RegisterFuncUsingTimerFromRegistry() => OkanshiMonitor.Timer("RegisterFuncUsingTimerFromRegistry").Record(() => 1);

			[Benchmark]
			public int RegisterFuncUsingTimerFromRegistryWithTags() => OkanshiMonitor
				.Timer("RegisterFuncUsingTimerFromRegistryWithTags", new[]{new Tag("key","val"), new Tag("Environment","production"), new Tag("Tic","Tac")})
				.Record(() => 1);

			[Benchmark]
			public void RegisterAction() => timer.Record(() => { int i = 0; });

			[Benchmark]
			public void Register2Action() => timer.Record2(() => { int i = 0; });

			[Benchmark]
			public void Register2Action_ResetClosure() => timer.Record2(() => { x = 0; });

		}

	public class Program
		{
			public static void Main(string[] args)
			{
				var summary = BenchmarkRunner.Run<Md5VsSha256>();
			}
		}
	}
