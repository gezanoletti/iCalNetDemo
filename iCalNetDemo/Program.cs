using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace iCalNetDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			var start = DateTime.Now;
			var end = start.AddHours(1);
			var until = start.AddMonths(1);

			// Recurring every other Thursday until Dec 31
			/*
			 * El segundo parametro, es el intervalo, para lograr otras frecuencias que necesitamos.
			 * Por ejemplo:
			 * - quincenal => new RecurrencePattern(FrequencyType.Weekly, 2)
			 * - trimestral => new RecurrencePattern(FrequencyType.Monthly, 3)
			 * - bianual => new RecurrencePattern(FrequencyType.Monthly, 6)
			 */
			var rrule = new RecurrencePattern(FrequencyType.Daily, 1)
			{
				Until = until
			};

			// An event taking place between 07:00 and 08:00, beginning April 12 (Thursday)
			var vEvent = new CalendarEvent
			{
				Start = new CalDateTime(start),
				//End = new CalDateTime(end),
				Duration = TimeSpan.FromDays(1),
				RecurrenceRules = new List<RecurrencePattern> { rrule },
			};

			vEvent.ExceptionRules.Add(new RecurrencePattern(FrequencyType.Weekly, 1)
			{
				ByDay = new List<WeekDay> { new WeekDay(DayOfWeek.Saturday), new WeekDay(DayOfWeek.Sunday) }
			});

			// Create calendar obj
			var calendar = new Calendar();
			calendar.Events.Add(vEvent);

			Console.WriteLine(calendar);

			// Serialize calendar
			var calendarSerializer = new CalendarSerializer();
			var calendarSerialized = calendarSerializer.SerializeToString(calendar);
			vEvent = null;
			Console.WriteLine(calendarSerialized);

			var calendarLoaded = Calendar.Load(calendarSerialized);
			
			// Obtener las recurrencias
			foreach (var occurrence in calendarLoaded.Events[0].GetOccurrences(new CalDateTime(start), new CalDateTime(until)).ToList())
			{
				Console.WriteLine($"Next meeting at {occurrence.Period.StartTime} - {occurrence.Period.EndTime}, duration of {occurrence.Period.Duration}");
			}

			Console.ReadLine();
		}
	}
}
