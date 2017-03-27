using System;

namespace Shevastream.Extensions
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Returns the int value of the enum
		/// </summary>
		/// <returns>The int value of the enum</returns>
		public static int AsInt(this Enum value)
		{
			return Convert.ToInt32(value);
		}

	}
}
