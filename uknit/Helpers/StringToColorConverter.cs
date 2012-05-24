using System.Windows.Media;

namespace uknit.Helpers.Converters
{
	public static class StringToColorConverter
	{
		public static Color HexString2Color(string hex)
		{
			byte a;
			byte r;
			byte g;
			byte b;

			a = (byte)(int.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			r = (byte)(int.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			g = (byte)(int.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			b = (byte)(int.Parse(hex.Substring(7, 2), System.Globalization.NumberStyles.AllowHexSpecifier));

			return Color.FromArgb(a, r, g, b);
		}
	}
}
