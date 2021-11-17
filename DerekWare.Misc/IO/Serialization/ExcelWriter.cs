using System;
using System.IO;
using System.Text;

namespace DerekWare.IO.Serialization
{
    public class ExcelWriter : IDisposable
    {
        BinaryWriter Writer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExcelWriter" /> class.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        public ExcelWriter(Stream stream)
        {
            Writer = new BinaryWriter(stream);
            Write(0x0809, 8, 0, 0x10, 0, 0);
        }

        ~ExcelWriter()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Writes a text cell value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="value">The string value.</param>
        public void WriteCell(int row, int col, string value)
        {
            Write(0x0204, 8 + value.Length, row, col, 0, value.Length);
            Writer.Write(Encoding.ASCII.GetBytes(value));
        }

        /// <summary>
        ///     Writes an integer cell value.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <param name="value">The value.</param>
        public void WriteCell(int row, int col, int value)
        {
            Write(0x027E, 10, row, col, 0);
            Writer.Write((value << 2) | 2);
        }

        /// <summary>
        ///     Writes a double cell value.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <param name="value">The value.</param>
        public void WriteCell(int row, int col, double value)
        {
            Write(0x0203, 14, row, col, 0);
            Writer.Write(value);
        }

        /// <summary>
        ///     Writes an empty cell.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        public void WriteCell(int row, int col)
        {
            Write(0x0201, 6, row, col, 0x17);
        }

        protected virtual void Dispose(bool disposing)
        {
            Extensions.Dispose(ref Writer,
                               v =>
                               {
                                   v.Write((ushort)0x0A);
                                   v.Write((ushort)0);
                                   v.Flush();
                               });
        }

        protected void Write(params int[] value)
        {
            foreach(var v in value)
            {
                if((v < 0) || (v > ushort.MaxValue))
                {
                    throw new ArgumentOutOfRangeException();
                }

                Writer.Write((ushort)v);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
