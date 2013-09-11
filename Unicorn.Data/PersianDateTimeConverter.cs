using System;

namespace Unicorn
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class PersianDateTimeConverter
	{
		private PersianDateTimeConverter()
		{
		}

		public static string ToShamsiString( PersianDateTime dt )
		{
			return dt.ToShortDateString();
		}

		public static string ToShamsiStringFull( PersianDateTime dt )
		{
            return dt.ToShortDateTimeString();
		}

		public static string ToSQLPersianDateTimeString( PersianDateTime dt )
		{
			return "'" + dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString() +  " " +
				dt.Hour.ToString() + ":" + dt.Minute.ToString() + ":" + dt.Second.ToString() + "." + dt.Millisecond.ToString() + "'";
		}

		// "jalali.php"	is convertor to	and	from Gregorian and Jalali calendars. 
		// Copyright (C) 2000  Roozbeh Pournader and Mohammad Toossi 
		// 
		// This	program	is free	software; you can redistribute it and/or 
		// modify it under the terms of	the	GNU	General	Public License 
		// as published	by the Free	Software Foundation; either	version	2 
		// of the License, or (at your option) any later version. 
		// 
		// This	program	is distributed in the hope that	it will	be useful, 
		// but WITHOUT ANY WARRANTY; without even the implied warranty of 
		// MERCHANTABILITY or FITNESS FOR A	PARTICULAR PURPOSE.	 See the 
		// GNU General Public License for more details.	
		// 
		// A copy of the GNU General Public	License	is available from: 
		// 
		//	  http://www.gnu.org/copyleft/gpl.html 
		// 

		private static double div( double a, double b ) 
		{
			return Math.Floor(a	/ b);
		}

		//**************************************************************************************
		//******************************    Shamsi To Miladi   *********************************
		//**************************************************************************************

		/// <summary>
		/// Converts given time to Shamsi
		/// </summary>
		/// <param name="year">year, also used as output</param>
		/// <param name="month">month, also used as output</param>
		/// <param name="day">day, also used as output</param>
		public static void MiladiToShamsi ( ref int year, ref int month, ref int day ) 
		{
			if ( year<1 || year >9999 )
				throw new ArgumentOutOfRangeException( "year", year, "The year must be between 1 and 9999 ." );
			if ( month<1 || month>12 )
				throw new ArgumentOutOfRangeException( "month", month, "The month must be between 1 and 12 ." );
			if ( day<1 || day>31 )
				throw new ArgumentOutOfRangeException( "day", day, "The day must be between 1 and 31 ." );

			int[] g_days_in_month = new int[]{31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
			int[] j_days_in_month = new int[]{31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29};

			int gy = year-1600; 
			int gm = month-1; 
			int gd = day-1; 

			double g_day_no = 365.0*gy+div(gy+3,4)-div(gy+99,100)+div(gy+399,400); 

			int i;
			for ( i=0; i < gm; ++i) 
				g_day_no += g_days_in_month[i]; 
			if (gm>1	&& ((gy%4==0 &&	gy%100!=0) || (gy%400==0)))	
				/* leap and after	Feb	*/ 
				g_day_no++; 
			g_day_no += gd; 

			int j_day_no = (int)g_day_no-79; 

			double j_np = div(j_day_no, 12053);	/* 12053 = 365*33 +	32/4 */	
			j_day_no = j_day_no % 12053;	

			double jy = 979+33*j_np+4*div(j_day_no,1461); /* 1461 =	365*4 +	4/4	*/ 

			j_day_no %= 1461; 

			if (j_day_no >= 366)	
			{ 
				jy += div(j_day_no-1,	365); 
				j_day_no = (j_day_no-1)%365; 
			} 

			for ( i = 0; i < 11 && j_day_no >= j_days_in_month[i]; ++i) 
				j_day_no -= j_days_in_month[i]; 
			int jm = i+1; 
			int jd = j_day_no+1;	


			year = (int)jy;
			month = jm;
			day = jd; 
		}

		public static PersianDateTime MiladiToShamsi( DateTime mi )
		{
			int d = mi.Day;
			int m = mi.Month;
			int y = mi.Year;
			MiladiToShamsi( ref y, ref m, ref d );
			return new PersianDateTime( y, m, d, mi.Hour, mi.Minute, mi.Second, mi.Millisecond );
		}

		public static PersianDateTime MiladiToShamsi( int year, int month, int day )
		{
			MiladiToShamsi( ref year, ref month, ref day );
			return new PersianDateTime( year, month, day );
		}
		

		//**************************************************************************************
		//******************************    Shamsi To Miladi   *********************************
		//**************************************************************************************

		public static void ShamsiToMiladi(ref int year, ref int month, ref int day )//j_y, j_m, j_d)	
		{
			if ( year<1 || year >9999 )
				throw new ArgumentOutOfRangeException( "year", year, "The year must be between 1 and 9999 ." );
			if ( month<1 || month>12 )
				throw new ArgumentOutOfRangeException( "month", month, "The month must be between 1 and 12 ." );
			if ( day<1 || day>31 )
				throw new ArgumentOutOfRangeException( "day", day, "The day must be between 1 and 31 ." );

			int[] g_days_in_month = new int[]{ 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }; 
			int[] j_days_in_month = new int[]{ 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29 };
	
			int jy = year - 979;
			int jm = month - 1;
			int jd = day - 1;

			double j_day_no	= 365 * jy + div(jy, 33) * 8 + div( jy%33+3, 4 );	
			int i;
			for ( i=0 ; i < jm ; ++i ) 
				j_day_no += j_days_in_month[i]; 

			j_day_no += jd; 
			int g_day_no = (int)j_day_no + 79; 

			double gy = 1600 + 400 * div( g_day_no, 146097 ); /* 146097	= 365*400 +	400/4 -	400/100	+ 400/400 */ 
			g_day_no = g_day_no % 146097; 

			bool leap = true;	
			if ( g_day_no >= 36525 ) /* 36525 = 365*100 + 100/4 */	
			{ 
				g_day_no--; 
				gy += 100*div(g_day_no,  36524); /* 36524 = 365*100 + 100/4 - 100/100 */ 
				g_day_no = g_day_no % 36524; 

				if (g_day_no >= 365) 
					g_day_no++; 
				else 
					leap = false;
			}

			gy += 4 * div( g_day_no, 1461); /* 1461 = 365*4 + 4/4 */ 
			g_day_no %= 1461; 

			if (g_day_no >= 366)	
			{ 
				leap = false;	
				g_day_no--; 
				gy += div(g_day_no, 365);	
				g_day_no = g_day_no % 365; 
			}

			for (i = 0; g_day_no >= g_days_in_month[i] + ( i == 1 && leap ? 1 : 0 ); i++) 
				g_day_no -= g_days_in_month[i] + ( i == 1 && leap ? 1 : 0 ); 
			month = i+1;
			day = g_day_no+1;
			year = (int)gy;
		}

		public static DateTime ShamsiToMiladi( PersianDateTime sh )
		{
			int year = sh.Year;
			int month = sh.Month;
			int day = sh.Day;
			ShamsiToMiladi ( ref year, ref month, ref day );
            try
            {
                return new DateTime(year, month, day, sh.Hour, sh.Minute, sh.Second, sh.Millisecond);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}/{1}/{2} {3}:{4}:{5}",year, month, day, sh.Hour, sh.Minute, sh.Second ), ex);
            }
		}

		public static DateTime ShamsiToMiladi( int year, int month, int day )
		{
			ShamsiToMiladi( ref year, ref month, ref day );
			return new DateTime ( year, month, day );
		}

	}
}