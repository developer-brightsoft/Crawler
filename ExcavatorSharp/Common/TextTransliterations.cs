// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.TextTransliterations
/// <summary>
/// Structure for transliteration to standart column names
/// </summary>

namespace ExcavatorSharp.Common
{
	public class TextTransliterations
	{
		private static readonly string[] lat_up = new string[33]
		{
			"A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I",
			"Y", "K", "L", "M", "N", "O", "P", "R", "S", "T",
			"U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'",
			"E", "Yu", "Ya"
		};

		private static readonly string[] lat_low = new string[33]
		{
			"a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i",
			"y", "k", "l", "m", "n", "o", "p", "r", "s", "t",
			"u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'",
			"e", "yu", "ya"
		};

		private static readonly string[] rus_up = new string[33]
		{
			"А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И",
			"Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т",
			"У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь",
			"Э", "Ю", "Я"
		};

		private static readonly string[] rus_low = new string[33]
		{
			"а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и",
			"й", "к", "л", "м", "н", "о", "п", "р", "с", "т",
			"у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь",
			"э", "ю", "я"
		};

		/// <summary>
		/// Transliterates some text from cyrillyc to latin
		/// </summary>
		/// <param name="OriginalString"></param>
		/// <returns></returns>
		public static string TransliterateFromCyryllicToLatin(string OriginalString)
		{
			for (int i = 0; i <= 32; i++)
			{
				OriginalString = OriginalString.Replace(rus_up[i], lat_up[i]);
				OriginalString = OriginalString.Replace(rus_low[i], lat_low[i]);
			}
			return OriginalString;
		}
	}
}