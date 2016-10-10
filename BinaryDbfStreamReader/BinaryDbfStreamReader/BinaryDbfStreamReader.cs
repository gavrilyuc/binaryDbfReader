using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.ExtensionString;

namespace System.IO.DbfStream
{
    /// <summary>
    /// Представляет строку из dbf
    /// </summary>
    public class DbfRow : Dictionary<string, object>
    {
        /// <summary>ctor</summary>
        /// <param name="capacity"></param>
        public DbfRow(int capacity = 2) : base(capacity)
        {
        }

    }

    /// <summary>
    /// Представляет файловое чтение файла DBF
    /// </summary>
    public class BinaryDbfStreamReader : IDisposable
    {
        private BinaryReader _binaryFile;
        private Header _header;
        private Column[] _columns;
        private string[] _columnsName;
        private int _position;

        /// <summary>
        /// Список колонок
        /// </summary>
        public string[] Columns => _columnsName ?? (_columnsName = _columns.Select(e => e.Name).ToArray());
        
        /// <summary>
        /// Кодировка с которым будет открыт Dbf Файл
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Максимальное кол-во рядков в файле
        /// </summary>
        public int MaxRows => _header.CountRecords;

        /// <summary>
        /// Текущая позиция каретки чтения
        /// </summary>
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                
                if (_position < 0 || _position >= _header.CountRecords) return;

                _binaryFile.BaseStream.Seek(_header.HeaderSize + _position * _header.RowSize, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Показывает, дошли ли мы до конца файла (End of File)
        /// </summary>
        public bool Eof => Position >= _header.CountRecords;

        /// <summary>ctor</summary>
        public BinaryDbfStreamReader() { }
        /// <summary>ctor</summary>
        public BinaryDbfStreamReader(Encoding enc) : this()
        {
            if (enc != null)
                Encoding = enc;
        }
        
        /// <summary>
        /// Открыть поток чтения
        /// </summary>
        /// <param name="fileName"></param>
        public void Open(string fileName)
        {
            _binaryFile = new BinaryReader(File.OpenRead(fileName));
            GCHandle handle = GCHandle.Alloc(_binaryFile.ReadBytes(Marshal.SizeOf(typeof(Header))), GCHandleType.Pinned);
            _header = (Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Header));
            handle.Free();

            List<Column> columns = new List<Column>();
            while (_binaryFile.PeekChar() != 13)
            {
                handle = GCHandle.Alloc(_binaryFile.ReadBytes(Marshal.SizeOf(typeof(Column))), GCHandleType.Pinned);
                Column field = (Column)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Column));
                columns.Add(field);
                handle.Free();
            }
            _columns = columns.ToArray();

            if (Encoding == null)
                Encoding = GetEncoding(_header.Encoding);

            _binaryFile.BaseStream.Seek(_header.HeaderSize, SeekOrigin.Begin);
        }

        /// <summary>
        /// Закрыть Поток чтения
        /// </summary>
        public void Close()
        {
            _binaryFile.Close();
            _columns = null;
            Encoding = null;
            Position = 0;
        }

        /// <summary>
        /// считать строку из потока
        /// </summary>
        /// <returns></returns>
        public DbfRow ReadRow()
        {
            if (Eof) return null;

            using (MemoryStream streamRecord = new MemoryStream(_binaryFile.ReadBytes(_header.RowSize)))
            {
                using (BinaryReader record = new BinaryReader(streamRecord))
                {
                    if (record.ReadChar() == '*')
                    {
                        record.Close();
                        return ReadRow();// continue
                    }

                    DbfRow row = new DbfRow(_columns.Length);

                    #region Write data to current row
                    foreach (Column field in _columns)
                    {
                        string item;
                        switch (field.Type)
                        {
                            case 'D': // date ( Year.Mounth.day )
                            {
                                item = Encoding.GetString(record.ReadBytes(4)) + "."
                                    + Encoding.GetString(record.ReadBytes(2)) + "."
                                    + Encoding.GetString(record.ReadBytes(2));
                                item = item.FormatString(StringFormattable.CompressAny);
                                break;
                            }
                            case 'T': // dateTime (Julian time)
                            {
                                item =
                                    DbfTimeToDateTimeInvariant(record.ReadInt32())
                                        .AddTicks(record.ReadInt32() * 10000L).ToString(
                                            CultureInfo.InvariantCulture);
                                break;
                            }
                            case 'L': // boolean (Y- Yes / N - No)
                            {
                                item = record.ReadByte() == 'Y' ? "1" : "0";
                                break;
                            }
                            default:
                            {
                                item = Encoding.GetString(record.ReadBytes(field.Leght)).FormatString(StringFormattable.Any);

                                decimal number;
                                if (HasDecimal(item, out number))
                                    item = number.ToString(CultureInfo.InvariantCulture);
                                break;
                            }
                        }
                        row.Add(field.Name, item.FormatString(StringFormattable.Any));
                    }
                    #endregion

                    Position++;

                    return row;
                }
            }
        }

        /// <summary>
        /// Освободить рессурсы
        /// </summary>
        public void Dispose()
        {
            Close();
            
            GC.Collect(GC.GetGeneration(this) + 1, GCCollectionMode.Forced);
        }

        #region DbfStructure
        /// <summary>
        /// Header Bdf row (32 bytes + 4 url bytes)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct Header
        {
            public byte Version;
            public byte LastUpdateYear;
            public byte LastUpdateMonth;
            public byte LastUpdateDay;
            public int CountRecords;
            public short HeaderSize;
            public short RowSize;
            public long NotUsed;
            public long NotUsed2;
            public byte Mdx;
            public byte Encoding;
            public short NotUsed3;
        }

        /// <summary>
        /// Column dbf table
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct Column
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string Name;
            public char Type;
            public int Address;
            public byte Leght;
            public byte Count;
            public short NotUsed;
            public byte WorkArea;
            public short NotUsed2;
            public byte Flag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] NotUsed3;
            public byte IndexFlag;
        }

        /// <summary>
        /// http://en.wikipedia.org/wiki/Julian_day
        /// </summary>
        /// <param name="time">Julian Date to convert (days since 01/01/4713 BC)</param>
        /// <returns>DateTime</returns>
        private static DateTime DbfTimeToDateTimeInvariant(long time)
        {
            double s1 = Convert.ToDouble(time) + 68569;
            double n = Math.Floor(4 * s1 / 146097);
            double s2 = s1 - Math.Floor((146097 * n + 3) / 4);
            double i = Math.Floor(4000 * (s2 + 1) / 1461001);
            double s3 = s2 - Math.Floor(1461 * i / 4) + 31;
            double q = Math.Floor(80 * s3 / 2447);
            double s4 = Math.Floor(q / 11);
            return new DateTime(Convert.ToInt32(100 * (n - 49) + i + s4),
                                Convert.ToInt32(q + 2 - 12 * s4),
                                Convert.ToInt32(s3 - Math.Floor(2447 * q / 80)));
        }

        private static bool HasDecimal(string s, out decimal num)
        {
            return decimal.TryParse(s.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out num);
        }

        /// <summary>
        /// все зарегистрированые кодировки dbf.
        /// </summary>
        private static readonly Dictionary<byte, int> EncodingByte = new Dictionary<byte, int> {
            { 1, 437 }, { 2, 850 }, { 3, 1252 }, { 4, 10000 }, { 100, 852 },
            { 101, 866 }, { 102, 865 }, { 103, 861 }, { 104, 895 },
            { 105, 620 }, { 106, 737 }, { 107, 857 }, { 108, 863 },
            { 120, 950 }, { 121, 949 }, { 122, 936 }, { 123, 932 },
            { 124, 874 }, { 150, 10007 }, { 151, 10029 }, { 152, 10006 },
            { 200, 1250 }, { 201, 1251 }, { 203, 1253 }, { 202, 1254 },
            { 125, 1255 }, { 126, 1256 }, { 204, 1257 }, { 38, 28591 }, { 0, 0 }
        };
        private static Encoding GetEncoding(byte xorCode)
        {
            return
                Encoding.GetEncoding(EncodingByte.ContainsKey(xorCode)
                    ? EncodingByte[xorCode] : EncodingByte[38]);
                    // iso-8859-1 is default dbf encoding
        }
        #endregion
    }
}