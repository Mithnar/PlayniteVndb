﻿using System;

namespace VndbSharp.Models.Common
{
	/// <summary>
	///		<para>A simple DateTime object that can represent a Year, Year-Month, and Year-Month-Day value</para>
	///		<para>This is a very brittle class</para>
	/// </summary>
	public class SimpleDate
	{
		/// <summary>
		///		Represents a "TBA" date
		/// </summary>
		public SimpleDate()
		{ }

		/// <summary>
		///		Represents a date with only a year
		/// </summary>
		public SimpleDate(UInt32 year)
		{
			this.Year = year;
		}

		/// <summary>
		///		Represents a date with a year and month
		/// </summary>
		public SimpleDate(UInt32 year, Byte month)
			: this(year)
		{
			if (month > 12)
				throw new ArgumentOutOfRangeException(nameof(month), month, "Month is Greater then 12? What calender are you using?");
			this.Month = month;
		}

		/// <summary>
		///		Represents a date with a year, month and day
		/// </summary>
		public SimpleDate(UInt32 year, Byte month, Byte day)
			: this(year, month)
		{
			if (day > 31)
				throw new ArgumentOutOfRangeException(nameof(day), day, "Day is larger then 31? What planet you living on?");
			this.Day = day;
		}

		/// <summary>
		///		Represents a date with a month and day
		/// </summary>
		public SimpleDate(Byte month, Byte day)
		{
			if (month > 12)
				throw new ArgumentOutOfRangeException(nameof(month), month, "Month is Greater then 12? What calender are you using?");
			if (day > 31)
				throw new ArgumentOutOfRangeException(nameof(day), day, "Day is larger then 31? What planet you living on?");
			this.Month = month;
			this.Day = day;
		}

		public override String ToString()
		{
			switch (this.Month)
			{
				case null when this.Day == null && this.Year == null:
					return "tba";
				case null:
					return $"{this.Year:0000}";
			}

			if (this.Day == null)
				return $"{this.Year:0000}-{this.Month:00}";

			if (this.Year == null && (this.Month != null && this.Day != null))
				return $"{this.Month:00}-{this.Day:00}"; // This is unintiutive

			return $"{this.Year:0000}-{this.Month:00}-{this.Day:00}";
		}

		public UInt32? Year { get; set; }
		public Byte? Month { get; set; }
		public Byte? Day { get; set; }
	}
}
