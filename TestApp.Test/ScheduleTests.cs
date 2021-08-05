using System;
using System.Linq;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestApp.Test
{
	[TestClass]
	public class ScheduleTests
	{
		[TestMethod]
		public void Construction ()
		{
			// "*.*.* * *:*:*.*" (раз в 1 мс)
			var schedule = new Schedule ();
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 101).ToList (), schedule._years);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 12).ToList (), schedule._months);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 32).ToList (), schedule._days);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 7).ToList (), schedule._weekDays);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 24).ToList (), schedule._hours);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 60).ToList (), schedule._minutes);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 60).ToList (), schedule._seconds);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 1000).ToList (), schedule._milliseconds);

			// 1,2,3-5,10-20/3 означает список 1,2,3,4,5,10,13,16,19
			schedule = new Schedule ("*.*.* * *:*:*.1,2,3-5,10-20/3");
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 101).ToList (), schedule._years);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 12).ToList (), schedule._months);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 32).ToList (), schedule._days);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 7).ToList (), schedule._weekDays);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 24).ToList (), schedule._hours);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 60).ToList (), schedule._minutes);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 60).ToList (), schedule._seconds);
			var ms = Enumerable.Repeat<byte> (0, 1000).ToList ();
			ms[1] = 1; ms[2] = 1; ms[3] = 1; ms[4] = 1; ms[5] = 1; ms[10] = 1; ms[13] = 1; ms[16] = 1; ms[19] = 1;
			CollectionAssert.AreEquivalent (ms, schedule._milliseconds);

			// (для часов) */4 означает 0,4,8,12,16,20
			schedule = new Schedule ("*.*.* * */4:*:*");
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 101).ToList (), schedule._years);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 12).ToList (), schedule._months);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 32).ToList (), schedule._days);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 7).ToList (), schedule._weekDays);
			var hours = Enumerable.Repeat<byte> (0, 24).ToList ();
			hours[0] = 1; hours[4] = 1; hours[8] = 1; hours[12] = 1; hours[16] = 1; hours[20] = 1;
			CollectionAssert.AreEquivalent (hours, schedule._hours);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 60).ToList (), schedule._minutes);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 60).ToList (), schedule._seconds);
			var ms2 = Enumerable.Repeat<byte> (0, 1000).ToList ();
			ms2[0] = 1;
			CollectionAssert.AreEquivalent (ms2, schedule._milliseconds);

			//  *.9.*/2 1-5 10:00:00.000 означает 10:00 во все дни с пн. по пт. по нечетным числам в сентябре
			schedule = new Schedule ("*.9.*/2 1-5 10:00:00.000");
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 101).ToList (), schedule._years);
			var months = Enumerable.Repeat<byte> (0, 12).ToList ();
			months[8] = 1;
			CollectionAssert.AreEquivalent (months.ToList (), schedule._months);
			var days = Enumerable.Repeat<byte> (0, 32).ToList ();
			days[0] = 1; days[2] = 1; days[4] = 1; days[6] = 1; days[8] = 1; days[10] = 1; days[12] = 1; days[14] = 1; days[16] = 1; days[18] = 1; days[20] = 1; days[22] = 1; days[24] = 1; days[26] = 1; days[28] = 1; days[30] = 1;
			CollectionAssert.AreEquivalent (days, schedule._days);
			var weekDays = Enumerable.Repeat<byte> (0, 7).ToList ();
			weekDays[1] = 1; weekDays[2] = 1; weekDays[3] = 1; weekDays[4] = 1; weekDays[5] = 1;
			CollectionAssert.AreEquivalent (weekDays, schedule._weekDays);
			hours = Enumerable.Repeat<byte> (0, 24).ToList ();
			hours[10] = 1;
			CollectionAssert.AreEquivalent (hours, schedule._hours);
			var minutes = Enumerable.Repeat<byte> (0, 60).ToList ();
			minutes[0] = 1;
			CollectionAssert.AreEquivalent (minutes, schedule._minutes);
			var seconds = Enumerable.Repeat<byte> (0, 60).ToList ();
			seconds[0] = 1;
			CollectionAssert.AreEquivalent (seconds, schedule._seconds);
			ms = Enumerable.Repeat<byte> (0, 1000).ToList ();
			ms[0] = 1;
			CollectionAssert.AreEquivalent (ms, schedule._milliseconds);

			// *:00:00 означает начало любого часа
			schedule = new Schedule ("*:00:00");
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 101).ToList (), schedule._years);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 12).ToList (), schedule._months);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 32).ToList (), schedule._days);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 7).ToList (), schedule._weekDays);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 24).ToList (), schedule._hours);
			minutes = Enumerable.Repeat<byte> (0, 60).ToList ();
			minutes[0] = 1;
			CollectionAssert.AreEquivalent (minutes, schedule._minutes);
			seconds = Enumerable.Repeat<byte> (0, 60).ToList ();
			seconds[0] = 1;
			CollectionAssert.AreEquivalent (seconds, schedule._seconds);
			ms = Enumerable.Repeat<byte> (0, 1000).ToList ();
			ms[0] = 1;
			CollectionAssert.AreEquivalent (ms, schedule._milliseconds);

			// *.*.01 01:30:00 означает 01:30 по первым числам каждого месяца
			schedule = new Schedule ("*.*.01 01:30:00");
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 101).ToList (), schedule._years);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 12).ToList (), schedule._months);
			days = Enumerable.Repeat<byte> (0, 32).ToList ();
			days[0] = 1;
			CollectionAssert.AreEquivalent (days, schedule._days);
			CollectionAssert.AreEquivalent (Enumerable.Repeat<byte> (1, 7).ToList (), schedule._weekDays);
			hours = Enumerable.Repeat<byte> (0, 24).ToList ();
			hours[1] = 1;
			CollectionAssert.AreEquivalent (hours, schedule._hours);
			minutes = Enumerable.Repeat<byte> (0, 60).ToList ();
			minutes[30] = 1;
			CollectionAssert.AreEquivalent (minutes, schedule._minutes);
			seconds = Enumerable.Repeat<byte> (0, 60).ToList ();
			seconds[0] = 1;
			CollectionAssert.AreEquivalent (seconds, schedule._seconds);
			ms = Enumerable.Repeat<byte> (0, 1000).ToList ();
			ms[0] = 1;
			CollectionAssert.AreEquivalent (ms, schedule._milliseconds);
		}

		[DataTestMethod]
		[DataRow ("2100.12.31 23:59:59.999", "2000-01-01 00:00:00.000", "2100-12-31 23:59:59.999")] // макс. количество итераций при проверке
		[DataRow ("*.*.* * *:*:*.*", "2000-01-01 00:00:00.001", "2000-01-01 00:00:00.001")] // "*.*.* * *:*:*.*" (раз в 1 мс)
		[DataRow ("*.*.* * *:*:*.*", "2100-12-31 23:59:59.999", "2100-12-31 23:59:59.999")]
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-02 00:00:00.500", "2021-08-02 00:00:00.500")] // 1,2,3-5,10-20/3 означает список 1,2,3,4,5,10,13,16,19
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-05 00:00:00.500", "2021-08-06 00:00:00.000")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2020-01-01 00:00:00.011", "2020-01-01 00:00:00.013")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2020-12-31 23:59:59.020", "2021-01-01 00:00:00.001")]
		[DataRow ("*.*.* * */4:*:*", "2020-01-01 00:00:00.000", "2020-01-01 00:00:00.000")] // (для часов) */4 означает 0,4,8,12,16,20
		[DataRow ("*.*.* * */4:*:*", "2020-12-31 21:00:00.000", "2021-01-01 00:00:00.000")]
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2020-09-03 10:00:00.000", "2020-09-03 10:00:00.000")] // *.9.*/2 1-5 10:00:00.000 означает 10:00 во все дни с пн. по пт. по нечетным числам в сентябре
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2020-09-30 12:00:00.000", "2021-09-01 10:00:00.000")]
		[DataRow ("*:00:00", "2020-01-01 00:00:00.000", "2020-01-01 00:00:00.000")] // *:00:00 означает начало любого часа
		[DataRow ("*:00:00", "2020-12-31 23:59:59.999", "2021-01-01 00:00:00.000")]
		[DataRow ("*.*.01 01:30:00", "2020-01-01 01:30:00.000", "2020-01-01 01:30:00.000")] // *.*.01 01:30:00 означает 01:30 по первым числам каждого месяца
		[DataRow ("*.*.01 01:30:00", "2020-12-31 01:30:00.001", "2021-01-01 01:30:00.000")]
		[DataRow ("*.*.32 12:00:00", "2020-01-31 12:00:00.000", "2020-01-31 12:00:00.000")] // 32-й день означает последнее число месяца
		[DataRow ("*.*.32 12:00:00", "2020-01-31 12:00:00.001", "2020-02-29 12:00:00.000")]
		public void NearestEvent (string scheduleString, string time, string expectedResult)
		{
			var schedule = new Schedule (scheduleString);
			Assert.AreEqual (ParseTime (expectedResult), schedule.NearestEvent (ParseTime (time)));
		}

		[DataTestMethod]
		[DataRow ("2100.12.31 23:59:59.999", "2000-01-01 00:00:00.000", "2100-12-31 23:59:59.999")] // макс. количество итераций при проверке
		[DataRow ("*.*.* * *:*:*.*", "2000-01-01 00:00:00.001", "2000-01-01 00:00:00.002")] // "*.*.* * *:*:*.*" (раз в 1 мс)
		[DataRow ("*.*.* * *:*:*.*", "2100-12-31 23:59:59.998", "2100-12-31 23:59:59.999")]
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-02 23:59:59.999", "2021-08-04 00:00:00.000")] // 1,2,3-5,10-20/3 означает список 1,2,3,4,5,10,13,16,19
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-05 00:00:00.500", "2021-08-06 00:00:00.000")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2020-01-01 00:00:00.011", "2020-01-01 00:00:00.013")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2020-12-31 23:59:59.020", "2021-01-01 00:00:00.001")]
		[DataRow ("*.*.* * */4:*:*", "2020-01-01 00:00:00.000", "2020-01-01 00:00:01.000")] // (для часов) */4 означает 0,4,8,12,16,20
		[DataRow ("*.*.* * */4:*:*", "2020-12-31 21:00:00.000", "2021-01-01 00:00:00.000")]
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2020-09-03 00:00:00.000", "2020-09-03 10:00:00.000")] // *.9.*/2 1-5 10:00:00.000 означает 10:00 во все дни с пн. по пт. по нечетным числам в сентябре
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2020-09-30 12:00:00.000", "2021-09-01 10:00:00.000")]
		[DataRow ("*:00:00", "2020-01-01 00:00:00.000", "2020-01-01 01:00:00.000")] // *:00:00 означает начало любого часа
		[DataRow ("*:00:00", "2020-12-31 23:59:59.999", "2021-01-01 00:00:00.000")]
		[DataRow ("*.*.01 01:30:00", "2020-01-01 01:00:00.000", "2020-01-01 01:30:00.000")] // *.*.01 01:30:00 означает 01:30 по первым числам каждого месяца
		[DataRow ("*.*.01 01:30:00", "2020-12-31 01:30:00.000", "2021-01-01 01:30:00.000")]
		[DataRow ("*.*.32 12:00:00", "2020-01-31 11:00:00.000", "2020-01-31 12:00:00.000")] // 32-й день означает последнее число месяца
		[DataRow ("*.*.32 12:00:00", "2020-01-31 12:00:00.000", "2020-02-29 12:00:00.000")]
		public void NextEvent (string scheduleString, string time, string expectedResult)
		{
			var schedule = new Schedule (scheduleString);
			Assert.AreEqual (ParseTime (expectedResult), schedule.NextEvent (ParseTime (time)));
		}

		[DataTestMethod]
		[DataRow ("2000.01.01 00:00:00.000", "2100-12-31 23:59:59.999", "2000-01-01 00:00:00.000")] // макс. количество итераций при проверке
		[DataRow ("*.*.* * *:*:*.*", "2000-01-01 00:00:00.001", "2000-01-01 00:00:00.001")] // "*.*.* * *:*:*.*" (раз в 1 мс)
		[DataRow ("*.*.* * *:*:*.*", "2100-12-31 23:59:59.999", "2100-12-31 23:59:59.999")]
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-02 00:00:00.500", "2021-08-02 00:00:00.500")] // 1,2,3-5,10-20/3 означает список 1,2,3,4,5,10,13,16,19
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-05 00:00:00.500", "2021-08-04 23:59:59.999")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2020-01-01 00:00:00.013", "2020-01-01 00:00:00.013")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2021-01-01 00:00:00.000", "2020-12-31 23:59:59.019")]
		[DataRow ("*.*.* * */4:*:*", "2020-01-01 00:00:00.000", "2020-01-01 00:00:00.000")] // (для часов) */4 означает 0,4,8,12,16,20
		[DataRow ("*.*.* * */4:*:*", "2021-01-01 05:50:00.000", "2021-01-01 04:59:59.000")]
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2020-09-03 10:00:00.000", "2020-09-03 10:00:00.000")] // *.9.*/2 1-5 10:00:00.000 означает 10:00 во все дни с пн. по пт. по нечетным числам в сентябре
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2021-09-01 09:59:59.999", "2020-09-29 10:00:00.000")]
		[DataRow ("*:00:00", "2020-01-01 00:00:00.000", "2020-01-01 00:00:00.000")] // *:00:00 означает начало любого часа
		[DataRow ("*:00:00", "2021-01-01 23:59:59.999", "2021-01-01 23:00:00.000")]
		[DataRow ("*.*.01 01:30:00", "2020-01-01 01:30:00.000", "2020-01-01 01:30:00.000")] // *.*.01 01:30:00 означает 01:30 по первым числам каждого месяца
		[DataRow ("*.*.01 01:30:00", "2021-01-01 01:29:59.999", "2020-12-01 01:30:00.000")]
		[DataRow ("*.*.32 12:00:00", "2020-01-31 12:00:00.000", "2020-01-31 12:00:00.000")] // 32-й день означает последнее число месяца
		[DataRow ("*.*.32 12:00:00", "2020-03-29 00:00:00.000", "2020-02-29 12:00:00.000")]
		public void NearestPrevEvent (string scheduleString, string time, string expectedResult)
		{
			var schedule = new Schedule (scheduleString);
			Assert.AreEqual (ParseTime (expectedResult), schedule.NearestPrevEvent (ParseTime (time)));
		}

		[DataTestMethod]
		[DataRow ("2000.01.01 00:00:00.000", "2100-12-31 23:59:59.999", "2000-01-01 00:00:00.000")] // макс. количество итераций при проверке
		[DataRow ("*.*.* * *:*:*.*", "2000-01-01 00:00:00.001", "2000-01-01 00:00:00.000")] // "*.*.* * *:*:*.*" (раз в 1 мс)
		[DataRow ("*.*.* * *:*:*.*", "2021-01-01 00:00:00.000", "2020-12-31 23:59:59.999")]
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-04 00:00:00.000", "2021-08-02 23:59:59.999")] // 1,2,3-5,10-20/3 означает список 1,2,3,4,5,10,13,16,19
		[DataRow ("*.*.* 1,3,5 *:*:*.*", "2021-08-05 00:00:00.500", "2021-08-04 23:59:59.999")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2020-01-01 00:00:00.013", "2020-01-01 00:00:00.010")]
		[DataRow ("*.*.* * *:*:*.1,2,3-5,10-20/3", "2021-01-01 00:00:00.000", "2020-12-31 23:59:59.019")]
		[DataRow ("*.*.* * */4:*:*", "2021-01-01 00:00:00.000", "2020-12-31 20:59:59.000")] // (для часов) */4 означает 0,4,8,12,16,20
		[DataRow ("*.*.* * */4:*:*", "2021-01-01 05:50:00.000", "2021-01-01 04:59:59.000")]
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2020-09-01 10:00:00.000", "2019-09-27 10:00:00.000")] // *.9.*/2 1-5 10:00:00.000 означает 10:00 во все дни с пн. по пт. по нечетным числам в сентябре
		[DataRow ("*.9.*/2 1-5 10:00:00.000", "2021-09-01 09:59:59.999", "2020-09-29 10:00:00.000")]
		[DataRow ("*:00:00", "2020-01-01 00:00:00.000", "2019-12-31 23:00:00.000")] // *:00:00 означает начало любого часа
		[DataRow ("*:00:00", "2021-01-01 23:59:59.999", "2021-01-01 23:00:00.000")]
		[DataRow ("*.*.01 01:30:00", "2020-01-01 01:30:00.000", "2019-12-01 01:30:00.000")] // *.*.01 01:30:00 означает 01:30 по первым числам каждого месяца
		[DataRow ("*.*.01 01:30:00", "2021-01-01 01:29:59.999", "2020-12-01 01:30:00.000")]
		[DataRow ("*.*.32 12:00:00", "2021-03-31 12:00:00.000", "2021-02-28 12:00:00.000")] // 32-й день означает последнее число месяца
		[DataRow ("*.*.32 12:00:00", "2020-03-29 00:00:00.000", "2020-02-29 12:00:00.000")]
		public void PrevEvent (string scheduleString, string time, string expectedResult)
		{
			var schedule = new Schedule (scheduleString);
			Assert.AreEqual (ParseTime (expectedResult), schedule.PrevEvent (ParseTime (time)));
		}

		private static DateTime ParseTime (string timeStr)
		{
			return DateTime.ParseExact (timeStr, "yyyy-MM-dd H:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None);
		}
	}
}
