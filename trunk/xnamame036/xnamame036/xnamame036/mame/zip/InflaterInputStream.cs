using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace xnamame036.mame
{
    class InflaterInputStream : FilterInputStream
    {
        /**
       * Decompressor for this filter 
       */
        protected Inflater inf;

        /**
         * Byte array used as a buffer 
         */
        protected byte[] buf;

        /**
         * Size of buffer   
         */
        protected int len;

        // We just use this if we are decoding one byte at a time with the
        // read() call.
        private byte[] onebytebuffer = new byte[1];

        /**
         * Create an InflaterInputStream with the default decompresseor
         * and a default buffer size.
         *
         * @param in the InputStream to read bytes from
         */
        public InflaterInputStream(Stream _in) : this(_in, new Inflater(), 4096) { }

        /**
         * Create an InflaterInputStream with the specified decompresseor
         * and a default buffer size.
         *
         * @param in the InputStream to read bytes from
         * @param inf the decompressor used to decompress data read from in
         */
        public InflaterInputStream(Stream _in, Inflater inf) : this(_in, inf, 4096) { }

        /**
         * Create an InflaterInputStream with the specified decompresseor
         * and a specified buffer size.
         *
         * @param in the InputStream to read bytes from
         * @param inf the decompressor used to decompress data read from in
         * @param size size of the buffer to use
         */
        public InflaterInputStream(Stream _in, Inflater inf, int size)
            : base(_in)
        {

            if (_in == null)
                throw new System.Exception("in may not be null");
            if (inf == null)
                throw new System.Exception("inf may not be null");
            if (size < 0)
                throw new System.Exception("size may not be negative");

            this.inf = inf;
            this.buf = new byte[size];
        }

        /**
         * Returns 0 once the end of the stream (EOF) has been reached.
         * Otherwise returns 1.
         */
        public int available()
        {
            // According to the JDK 1.2 docs, this should only ever return 0
            // or 1 and should not be relied upon by Java programs.
            if (inf == null)
                throw new IOException("stream closed");
            return inf.finished() ? 0 : 1;
        }

        /**
         * Closes the input stream
         */
        public void close()
        {
            if (_in != null)
                _in.Close();
            _in = null;
        }

        /**
         * Fills the buffer with more data to decompress.
         */
        protected void fill()
        {
            if (_in == null)
                throw new System.Exception("InflaterInputStream is closed");

            len = _in.Read(buf, 0, buf.Length);

            if (len < 0)
                throw new System.Exception("Deflated stream ends early.");

            inf.setInput(buf, 0, len);
        }

        /**
         * Reads one byte of decompressed data.
         *
         * The byte is in the lower 8 bits of the int.
         */
        public int read()
        {
            int nread = read(onebytebuffer, 0, 1);
            if (nread > 0)
                return onebytebuffer[0] & 0xff;
            return -1;
        }

        /**
         * Decompresses data into the byte array
         *
         * @param b the array to read and decompress data into
         * @param off the offset indicating where the data should be placed
         * @param len the number of bytes to decompress
         */
        public int read(byte[] b, int off, int len)
        {
            if (inf == null)
                throw new IOException("stream closed");
            if (len == 0)
                return 0;

            int count = 0;
            for (; ; )
            {

                try
                {
                    count = inf.inflate(b, off, len);
                }
                catch (System.Exception dfe)
                {
                    throw new System.Exception(dfe.Message);
                }

                if (count > 0)
                    return count;

                if (inf.needsDictionary()
                    | inf.finished())
                    return -1;
                else if (inf.needsInput())
                    fill();
                else
                    throw new System.Exception("Don't know what to do");
            }
        }

        /**
         * Skip specified number of bytes of uncompressed data
         *
         * @param n number of bytes to skip
         */
        public long skip(long n)
        {
            if (inf == null)
                throw new IOException("stream closed");
            if (n < 0)
                throw new System.Exception();

            if (n == 0)
                return 0;

            int buflen = (int)Math.Min(n, 2048);
            byte[] tmpbuf = new byte[buflen];

            long skipped = 0L;
            while (n > 0L)
            {
                int numread = read(tmpbuf, 0, buflen);
                if (numread <= 0)
                    break;
                n -= numread;
                skipped += numread;
                buflen = (int)Math.Min(n, 2048);
            }

            return skipped;
        }

        public bool markSupported()
        {
            return false;
        }

        public void mark(int readLimit)
        {
        }

        public void reset()
        {
            throw new IOException("reset not supported");
        }
    }
}
