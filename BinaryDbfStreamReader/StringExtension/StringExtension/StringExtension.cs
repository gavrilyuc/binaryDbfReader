using System.Text;
using System.Text.RegularExpressions;

namespace System.ExtensionString
{
    /// <summary>
    /// Тип форматируемой строки
    /// </summary>
    [Flags]
    public enum StringFormattable
    {
        /// <summary>
        /// ничего не делать.
        /// </summary>
        None = 0,
        /// <summary>
        /// Без пробелов. (символы \r \n \t тоже считаются как пробелы)
        /// </summary>
        NotWhiteSpace = 1 << 0,
        /// <summary>
        /// Максимальное кол-во провебов 1 на каждое совокупление пробелов. (символы \r \n \t тоже считаются как пробелы)
        /// </summary>
        ToOneWhiteSpace = 1 << 1,
        /// <summary>
        /// Без Неотображаемых символов
        /// </summary>
        NonDisplayable = 1 << 2,
        /// <summary>
        /// Без пробелов в начале
        /// </summary>
        TrimStart = 1 << 3,
        /// <summary>
        /// Без пробелов в конце
        /// </summary>
        TrimEnd = 1 << 4,
        /// <summary>
        /// Без пробелов в начале и в конце
        /// </summary>
        Trim = TrimStart | TrimEnd,

        /// <summary>
        /// Без условно подобности букв (например в рус. и англ. алфивите есть буквы: A, отображаются они одинаково, но это разные символы)
        /// перевод в английский вид.
        /// </summary>
        AssociableToEng = 1 << 5,
        /// <summary>
        /// Без условно подобности букв (например в рус. и англ. алфивите есть буквы: A, отображаются они одинаково, но это разные символы)
        /// перевод в Русский вид.
        /// </summary>
        AssociableToRu = 1 << 6,

        /// <summary>
        /// В верхнем регистре
        /// </summary>
        UpperSimbols = 1 << 7,
        /// <summary>
        /// В нижнем регистре
        /// </summary>
        LowwerSimbols = 1 << 8,

        /// <summary>
        /// По Умолчанию. содержит: NonDisplayable | Trim
        /// </summary>
        Any = NonDisplayable | Trim,
        /// <summary>
        /// По умолчанию и без пробелов. содержит: NotWhiteSpace | Any
        /// </summary>
        CompressAny = NotWhiteSpace | Any,
        /// <summary>
        /// По умолчанию, без пробелов и в верхнем регистре. содержит: CompressAny | UpperSimbols
        /// </summary>
        CompressAnyUpper = CompressAny | UpperSimbols,
        /// <summary>
        /// По умолчанию, без пробелов и в нижнем регистре. содержит: CompressAny | LowwerSimbols
        /// </summary>
        CompressAnyLower = CompressAny | LowwerSimbols,
        /// <summary>
        /// По Умолчанию в верхнем регистре. содержит: Any | UpperSimbols
        /// </summary>
        AnyUpper = Any | UpperSimbols,
        /// <summary>
        /// По Умолчанию в нижнем регистре. содержит: Any | LowwerSimbols
        /// </summary>
        AnyLower = Any | LowwerSimbols,

        /// <summary>
        /// По Умолчанию с одиночными пробелами
        /// </summary>
        AnyWhiteSpace = ToOneWhiteSpace | Any
    }

    /// <summary>
    /// Объект расширения строки
    /// </summary>
    public static class ExtensionString
    {
        #region private fields
        private static readonly Regex CorrectStringForematter =
            new Regex(@"([^\'\.\>\<\,\[\]\""\№\;\%\:\?\`\~\|\/\!\@\#\$\%\^\&\*\(\)\-+=\w\s]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex WhiteSpace = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Multiline);
        #endregion

        #region private methods
        private static string ToDenotationEng(string input)
        {
            if (input == null)
                return null;

            string value = input.ToUpper();
            char[] chars = new char[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case 'А':
                        chars[i] = 'A';
                        break;
                    case 'В':
                        chars[i] = 'B';
                        break;
                    case 'Е':
                        chars[i] = 'E';
                        break;
                    case 'Ё':
                        chars[i] = 'E';
                        break;
                    case 'К':
                        chars[i] = 'K';
                        break;
                    case 'М':
                        chars[i] = 'M';
                        break;
                    case 'Н':
                        chars[i] = 'H';
                        break;
                    case 'О':
                        chars[i] = 'O';
                        break;
                    case 'Р':
                        chars[i] = 'P';
                        break;
                    case 'С':
                        chars[i] = 'C';
                        break;
                    case 'Т':
                        chars[i] = 'T';
                        break;
                    case 'Х':
                        chars[i] = 'X';
                        break;
                    case 'І':
                        chars[i] = 'I';
                        break;
                    case 'Ї':
                        chars[i] = 'I';
                        break;

                    case 'а':
                        chars[i] = 'a';
                        break;
                    case 'е':
                        chars[i] = 'e';
                        break;
                    case 'к':
                        chars[i] = 'k';
                        break;
                    case 'о':
                        chars[i] = 'o';
                        break;
                    case 'р':
                        chars[i] = 'p';
                        break;
                    case 'с':
                        chars[i] = 'c';
                        break;
                    case 'х':
                        chars[i] = 'x';
                        break;
                    case 'і':
                        chars[i] = 'i';
                        break;
                    case 'ї':
                        chars[i] = 'i';
                        break;

                    default:
                        chars[i] = input[i];
                        break;
                }
            }
            return new string(chars);
        }
        private static string ToDenotationRus(string input)
        {
            if (input == null)
                return null;

            string value = input.ToUpper();
            char[] chars = new char[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case 'A':
                        chars[i] = 'А';
                        break;
                    case 'B':
                        chars[i] = 'В';
                        break;
                    case 'E':
                        chars[i] = 'Е';
                        break;
                    case 'K':
                        chars[i] = 'К';
                        break;
                    case 'M':
                        chars[i] = 'М';
                        break;
                    case 'H':
                        chars[i] = 'Н';
                        break;
                    case 'O':
                        chars[i] = 'О';
                        break;
                    case 'P':
                        chars[i] = 'Р';
                        break;
                    case 'C':
                        chars[i] = 'С';
                        break;
                    case 'T':
                        chars[i] = 'Т';
                        break;
                    case 'X':
                        chars[i] = 'Х';
                        break;
                    case 'I':
                        chars[i] = 'І';
                        break;

                    case 'a':
                        chars[i] = 'а';
                        break;
                    case 'e':
                        chars[i] = 'е';
                        break;
                    case 'k':
                        chars[i] = 'к';
                        break;
                    case 'o':
                        chars[i] = 'о';
                        break;
                    case 'p':
                        chars[i] = 'р';
                        break;
                    case 'c':
                        chars[i] = 'с';
                        break;
                    case 'x':
                        chars[i] = 'х';
                        break;
                    case 'i':
                        chars[i] = 'і';
                        break;

                    default:
                        chars[i] = input[i];
                        break;
                }
            }
            return new string(chars);
        }

        #endregion

        /// <summary>
        /// Провести форматирование строки по указаным критериям
        /// </summary>
        /// <param name="current">Строка</param>
        /// <param name="formates">Форматируемые критерии</param>
        /// <returns>Строка</returns>
        public static string FormatString(this string current, StringFormattable formates)
        {
            if ((formates & StringFormattable.NonDisplayable) == StringFormattable.NonDisplayable)
                current = CorrectStringForematter.Replace(current, string.Empty);

            if ((formates & StringFormattable.ToOneWhiteSpace) == StringFormattable.ToOneWhiteSpace)
                current = WhiteSpace.Replace(current, " ");

            if ((formates & StringFormattable.NotWhiteSpace) == StringFormattable.ToOneWhiteSpace)
                current = WhiteSpace.Replace(current, string.Empty);

            if ((formates & StringFormattable.NotWhiteSpace) == StringFormattable.ToOneWhiteSpace)
                current = WhiteSpace.Replace(current, string.Empty);

            if ((formates & StringFormattable.AssociableToEng) == StringFormattable.AssociableToEng)
                current = ToDenotationEng(current);
            else if ((formates & StringFormattable.AssociableToRu) == StringFormattable.AssociableToRu)
                current = ToDenotationRus(current);

            if ((formates & StringFormattable.Trim) == StringFormattable.Trim)
                current = current.Trim();
            else
            {
                if ((formates & StringFormattable.TrimStart) == StringFormattable.TrimStart)
                    current = current.TrimStart();

                if ((formates & StringFormattable.TrimEnd) == StringFormattable.TrimEnd)
                    current = current.TrimEnd();
            }

            if ((formates & StringFormattable.UpperSimbols) == StringFormattable.UpperSimbols)
                current = current.ToUpper();
            else if ((formates & StringFormattable.LowwerSimbols) == StringFormattable.LowwerSimbols)
                current = current.ToLower();

            return current;
        }

        /// <summary>
        /// Преобразовует строку из указаной кодировки в указаную кодировку
        /// </summary>
        /// <param name="current">Строка</param>
        /// <param name="from">Кодировка, из которой будет преобразование</param>
        /// <param name="to">Кодировка, в которую нужно преобразовать строку</param>
        /// <returns>Строка</returns>
        public static string Encoding(this string current, Encoding from, Encoding to)
        {
            return to.GetString(System.Text.Encoding.Convert(from, to, from.GetBytes(ToDenotationEng(current))));
        }
    }
}
