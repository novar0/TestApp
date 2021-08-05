using System;

namespace TestApp
{
	/// <summary>
	/// Класс для задания и расчета времени по расписанию.
	/// </summary>
	public class Schedule
	{
		private const string _placeholder = "*"; // Звездочка означает любое возможное значение.
		private const string _defaultMilliseconds = "0"; // fff - миллисекунды (0-999). Если не указаны, то 0

		internal readonly byte[] _years =        new byte[101]; // год (2000-2100)
		internal readonly byte[] _months =       new byte[12]; // месяц (1-12)
		internal readonly byte[] _days =         new byte[32]; // число месяца (1-31 или 32)
		internal readonly byte[] _weekDays =     new byte[7]; // день недели (0-6)
		internal readonly byte[] _hours =        new byte[24]; // часы (0-23)
		internal readonly byte[] _minutes =      new byte[60]; // минуты (0-59)
		internal readonly byte[] _seconds =      new byte[60]; // секунды (0-59)
		internal readonly byte[] _milliseconds = new byte[1000]; // миллисекунды (0-999)

		/// <summary>
		/// Создает пустой экземпляр, который будет соответствовать
		/// расписанию типа "*.*.* * *:*:*.*" (раз в 1 мс).
		/// </summary>
		public Schedule ()
			: this ("*.*.* * *:*:*.*")
		{
		}

		/// <summary>
		/// Создает экземпляр из строки с представлением расписания.
		/// </summary>
		/// <param name="scheduleString">Строка расписания.
		/// Формат строки:
		///     yyyy.MM.dd w HH:mm:ss.fff
		///     yyyy.MM.dd HH:mm:ss.fff
		///     HH:mm:ss.fff
		///     yyyy.MM.dd w HH:mm:ss
		///     yyyy.MM.dd HH:mm:ss
		///     HH:mm:ss
		/// Где yyyy - год (2000-2100)
		///     MM - месяц (1-12)
		///     dd - число месяца (1-31 или 32). 32 означает последнее число месяца
		///     w - день недели (0-6). 0 - воскресенье, 6 - суббота
		///     HH - часы (0-23)
		///     mm - минуты (0-59)
		///     ss - секунды (0-59)
		///     fff - миллисекунды (0-999). Если не указаны, то 0
		/// Каждую часть даты/времени можно задавать в виде списков и диапазонов.
		/// Например:
		///     1,2,3-5,10-20/3
		///     означает список 1,2,3,4,5,10,13,16,19
		/// Дробью задается шаг в списке.
		/// Звездочка означает любое возможное значение.
		/// Например (для часов):
		///     */4
		///     означает 0,4,8,12,16,20
		/// Вместо списка чисел месяца можно указать 32. Это означает последнее
		/// число любого месяца.
		/// Пример:
		///     *.9.*/2 1-5 10:00:00.000
		///     означает 10:00 во все дни с пн. по пт. по нечетным числам в сентябре
		///     *:00:00
		///     означает начало любого часа
		///     *.*.01 01:30:00
		///     означает 01:30 по первым числам каждого месяца
		/// </param>
		public Schedule (string scheduleString)
		{
			// конструктор большой по объёму исходника, но выполняет лишь простейшие действия и не выделяет память
			if (scheduleString == null)
			{
				throw new ArgumentNullException (nameof (scheduleString));
			}

			var dotPosition1 = scheduleString.IndexOf ('.');
			var dotPosition2 = (dotPosition1 < 0) ?
				-1 :
				scheduleString.IndexOf ('.', dotPosition1 + 1);
			var dotPosition3 = (dotPosition2 < 0) ?
				-1 :
				scheduleString.IndexOf ('.', dotPosition2 + 1);
			var colonPosition1 = scheduleString.IndexOf (':');
			var colonPosition2 = (colonPosition1 < 0) ?
				-1 :
				scheduleString.IndexOf (':', colonPosition1 + 1);
			if (colonPosition2 < 0)
			{
				// два двоеточия есть во всех форматах
				throw new FormatException ();
			}
			var spacePosition1 = scheduleString.IndexOf (' ');
			var spacePosition2 = (spacePosition1 < 0) ?
				-1 :
				scheduleString.IndexOf (' ', spacePosition1 + 1);

			ReadOnlySpan<char> yearPart;
			ReadOnlySpan<char> monthPart;
			ReadOnlySpan<char> dayPart;
			ReadOnlySpan<char> weekDayPart;
			ReadOnlySpan<char> hourPart;
			ReadOnlySpan<char> minutePart;
			ReadOnlySpan<char> secondPart;
			ReadOnlySpan<char> millisecondPart;
			if (
				(dotPosition1 >= 0) &&
				(dotPosition2 >= 0) &&
				(dotPosition3 >= 0) &&
				(colonPosition1 > dotPosition2) &&
				(dotPosition3 > colonPosition2))
			{
				// yyyy.MM.dd w HH:mm:ss.fff  или  yyyy.MM.dd HH:mm:ss.fff
				yearPart = scheduleString.AsSpan (0, dotPosition1);
				monthPart = scheduleString.AsSpan (dotPosition1 + 1, dotPosition2 - dotPosition1 - 1);
				dayPart = scheduleString.AsSpan (dotPosition2 + 1, spacePosition1 - dotPosition2 - 1);
				weekDayPart = (spacePosition2 < 0) ?
					_placeholder :
					scheduleString.AsSpan (spacePosition1 + 1, spacePosition2 - spacePosition1 - 1);
				hourPart = (spacePosition2 < 0) ?
					scheduleString.AsSpan (spacePosition1 + 1, colonPosition1 - spacePosition1 - 1) :
					scheduleString.AsSpan (spacePosition2 + 1, colonPosition1 - spacePosition2 - 1);
				minutePart = scheduleString.AsSpan (colonPosition1 + 1, colonPosition2 - colonPosition1 - 1);
				secondPart = scheduleString.AsSpan (colonPosition2 + 1, dotPosition3 - colonPosition2 - 1);
				millisecondPart = scheduleString.AsSpan (dotPosition3 + 1);
			}
			else
			{
				if (
					(dotPosition1 >= 0) &&
					(dotPosition2 >= 0) &&
					(dotPosition3 < 0) &&
					(colonPosition1 > dotPosition2))
				{
					// yyyy.MM.dd w HH:mm:ss  или  yyyy.MM.dd HH:mm:ss
					yearPart = scheduleString.AsSpan (0, dotPosition1);
					monthPart = scheduleString.AsSpan (dotPosition1 + 1, dotPosition2 - dotPosition1 - 1);
					dayPart = scheduleString.AsSpan (dotPosition2 + 1, spacePosition1 - dotPosition2 - 1);
					weekDayPart = (spacePosition2 < 0) ?
						_placeholder :
						scheduleString.AsSpan (spacePosition1 + 1, spacePosition2 - spacePosition1 - 1);
					hourPart = (spacePosition2 < 0) ?
						scheduleString.AsSpan (spacePosition1 + 1, colonPosition1 - spacePosition1 - 1) :
						scheduleString.AsSpan (spacePosition2 + 1, colonPosition1 - spacePosition2 - 1);
					minutePart = scheduleString.AsSpan (colonPosition1 + 1, colonPosition2 - colonPosition1 - 1);
					secondPart = scheduleString.AsSpan (colonPosition2 + 1);
					millisecondPart = _defaultMilliseconds;
				}
				else
				{
					if (
						(dotPosition1 >= 0) &&
						(dotPosition2 < 0) &&
						(colonPosition2 < dotPosition1))
					{
						// HH:mm:ss.fff
						yearPart = _placeholder;
						monthPart = _placeholder;
						dayPart = _placeholder;
						weekDayPart = _placeholder;
						hourPart = scheduleString.AsSpan (0, colonPosition1);
						minutePart = scheduleString.AsSpan (colonPosition1 + 1, colonPosition2 - colonPosition1 - 1);
						secondPart = scheduleString.AsSpan (colonPosition2 + 1, dotPosition1 - colonPosition2 - 1);
						millisecondPart = scheduleString.AsSpan (dotPosition1 + 1);
					}
					else
					{
						if (dotPosition1 < 0)
						{
							// HH:mm:ss
							yearPart = _placeholder;
							monthPart = _placeholder;
							dayPart = _placeholder;
							weekDayPart = _placeholder;
							hourPart = scheduleString.AsSpan (0, colonPosition1);
							minutePart = scheduleString.AsSpan (colonPosition1 + 1, colonPosition2 - colonPosition1 - 1);
							secondPart = scheduleString.AsSpan (colonPosition2 + 1);
							millisecondPart = _defaultMilliseconds;
						}
						else
						{
							throw new FormatException ("Unrecognized schedule.");
						}
					}
				}
			}

			ParsePart (yearPart,        _years,        2000);
			ParsePart (monthPart,       _months,       1);
			ParsePart (dayPart,         _days,         1);
			ParsePart (weekDayPart,     _weekDays,     0);
			ParsePart (hourPart,        _hours,        0);
			ParsePart (minutePart,      _minutes,      0);
			ParsePart (secondPart,      _seconds,      0);
			ParsePart (millisecondPart, _milliseconds, 0);
		}

		/// <summary>
		/// Возвращает следующий ближайший к заданному времени момент в расписании или
		/// само заданное время, если оно есть в расписании.
		/// </summary>
		/// <param name="t1">Заданное время</param>
		/// <returns>Ближайший момент времени в расписании</returns>
		public DateTime NearestEvent(DateTime t1)
		{
			// цикл проверки каждый раз после корректировки времени, макс. кол-во итераций: 100 + 11 + 31 + 23 + 59 + 59 = 283
			while (true)
			{
				var yearOffset = t1.Year - 2000;
				if ((yearOffset < 0) || (yearOffset >= _years.Length))
				{
					throw new InvalidOperationException ("Year out of range");
				}

				if (_years[yearOffset] > 0)
				{
					if (_months[t1.Month - 1] > 0)
					{
						var isLastDayInMonth = t1.Day == DateTime.DaysInMonth (t1.Year, t1.Month);
						// 32-й день означает последнее число месяца
						if (((_days[t1.Day - 1] > 0) || (isLastDayInMonth && (_days[31] > 0))) && (_weekDays[(int)t1.DayOfWeek] > 0))
						{
							if (_hours[t1.Hour] > 0)
							{
								if (_minutes[t1.Minute] > 0)
								{
									if (_seconds[t1.Second] > 0)
									{
										var millisecond = t1.Millisecond;
										do
										{
											if (_milliseconds[millisecond] > 0)
											{
												return new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second, millisecond);
											}

											millisecond++;
										} while (millisecond < _milliseconds.Length);
									}
									t1 = new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second).AddSeconds (1);
								}
								else
								{
									t1 = new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, 0).AddMinutes (1);
								}
							}
							else
							{
								t1 = new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, 0, 0).AddHours (1);
							}
						}
						else
						{
							t1 = new DateTime (t1.Year, t1.Month, t1.Day).AddDays (1);
						}
					}
					else
					{
						t1 = new DateTime (t1.Year, t1.Month, 1).AddMonths (1);
					}
				}
				else
				{
					t1 = new DateTime (t1.Year, 1, 1).AddYears (1);
				}
			}
		}

		/// <summary>
		/// Возвращает предыдущий ближайший к заданному времени момент в расписании или
		/// само заданное время, если оно есть в расписании.
		/// </summary>
		/// <param name="t1">Заданное время</param>
		/// <returns>Ближайший момент времени в расписании</returns>
		public DateTime NearestPrevEvent(DateTime t1)
		{
			// цикл проверки каждый раз после корректировки времени, макс. кол-во итераций: 100 + 11 + 31 + 23 + 59 + 59 = 283
			while (true)
			{
				var yearOffset = t1.Year - 2000;
				if ((yearOffset < 0) || (yearOffset >= _years.Length))
				{
					throw new InvalidOperationException ("Year out of range");
				}

				if (_years[yearOffset] > 0)
				{
					if (_months[t1.Month - 1] > 0)
					{
						var isLastDayInMonth = t1.Day == DateTime.DaysInMonth (t1.Year, t1.Month);
						// 32-й день означает последнее число месяца
						if (((_days[t1.Day - 1] > 0) || (isLastDayInMonth && (_days[31] > 0))) && (_weekDays[(int)t1.DayOfWeek] > 0))
						{
							if (_hours[t1.Hour] > 0)
							{
								if (_minutes[t1.Minute] > 0)
								{
									if (_seconds[t1.Second] > 0)
									{
										var millisecond = t1.Millisecond;
										do
										{
											if (_milliseconds[millisecond] > 0)
											{
												return new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second, millisecond);
											}

											millisecond--;
										} while (millisecond >= 0);
									}
									t1 = new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second).AddMilliseconds (-1);
								}
								else
								{
									t1 = new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, 0).AddMilliseconds (-1);
								}
							}
							else
							{
								t1 = new DateTime (t1.Year, t1.Month, t1.Day, t1.Hour, 0, 0).AddMilliseconds (-1);
							}
						}
						else
						{
							t1 = new DateTime (t1.Year, t1.Month, t1.Day).AddMilliseconds (-1);
						}
					}
					else
					{
						t1 = new DateTime (t1.Year, t1.Month, 1).AddMilliseconds (-1);
					}
				}
				else
				{
					t1 = new DateTime (t1.Year, 1, 1).AddMilliseconds (-1);
				}
			}
		}

		/// <summary>
		/// Возвращает следующий момент времени в расписании.
		/// </summary>
		/// <param name="t1">Время, от которого нужно отступить</param>
		/// <returns>Следующий момент времени в расписании</returns>
		public DateTime NextEvent(DateTime t1)
		{
			return NearestEvent (t1.AddMilliseconds (1));
		}

		/// <summary>
		/// Возвращает предыдущий момент времени в расписании.
		/// </summary>
		/// <param name="t1">Время, от которого нужно отступить</param>
		/// <returns>Предыдущий момент времени в расписании</returns>
		public DateTime PrevEvent(DateTime t1)
		{
			return NearestPrevEvent (t1.AddMilliseconds (-1));
		}

		/// <summary>
		/// Заполняет указанный массив ненулевыми значениями для тех индексов, которые указаны в partStr.
		/// </summary>
		/// <param name="partStr">Текстовый список диапазонов.</param>
		/// <param name="allowedValues">Массив, в котором будут отмечены ненулевым значением указанные в partStr числа.</param>
		/// <param name="offset">Смещение, которое будет вычитаться для всех чисел, указанных в partStr.</param>
		private static void ParsePart (ReadOnlySpan<char> partStr, byte[] allowedValues, int offset)
		{
			// Каждую часть даты/времени можно задавать в виде списков и диапазонов.
			// Например:
			//     1,2,3-5,10-20/3
			//     означает список 1,2,3,4,5,10,13,16,19
			// Дробью задается шаг в списке.
			// Звездочка означает любое возможное значение.

			if (partStr.Length < 1)
			{
				throw new FormatException ("Invalid part");
			}

			var idx = 0;
			while (idx < partStr.Length)
			{
				int number1;
				int number2;
				if (partStr[idx] == '*')
				{
					idx++;
					number1 = 0;
					number2 = allowedValues.Length - 1;
				}
				else
				{
					number1 = ParseNumber (partStr, ref idx) - offset;
					if ((number1 < 0) || (number1 >= allowedValues.Length))
					{
						throw new FormatException ("Number out of range.");
					}

					number2 = number1;
					if ((idx < partStr.Length) && (partStr[idx] == '-'))
					{
						idx++;
						number2 = ParseNumber (partStr, ref idx) - offset;
						if ((number2 < 0) || (number2 >= allowedValues.Length))
						{
							throw new FormatException ("Number out of range.");
						}
					}
				}

				var period = 1;
				if (idx < partStr.Length)
				{
					switch (partStr[idx])
					{
						case '/':
							idx++;
							period = ParseNumber (partStr, ref idx);
							break;
						case ',':
							idx++;
							break;
						default:
							throw new FormatException ("Invalid character.");
					}
				}

				for (var i = number1; i <= number2; i += period)
				{
					allowedValues[i] = 1;
				}
			}
		}

		// возвращает число, полученное из текстового представленая в partStr начиная с индекса в idx
		private static int ParseNumber (ReadOnlySpan<char> partStr, ref int idx)
		{
			var number = 0;
			while ((idx < partStr.Length) && (partStr[idx] >= '0') && (partStr[idx] <= '9'))
			{
				number = 10 * number + partStr[idx] - '0';
				idx++;
			}

			return number;
		}
	}

}