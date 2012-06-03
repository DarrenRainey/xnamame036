using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace xnamame036.mame
{

    public class ZipFile : ZipConstants
    {  /**
   * Mode flag to open a zip file for reading.
   */
        public const int OPEN_READ = 0x1;

        /**
         * Mode flag to delete a zip file after reading.
         */
        public const int OPEN_DELETE = 0x4;

        /**
         * This field isn't defined in the JDK's ZipConstants, but should be.
         */
        const int ENDNRD = 4;

        // Name of this zip file.
        private string name;

        // File from which zip entries are read.
        private Stream raf;

        // The entries of this zip file when initialized and not yet closed.
        public Dictionary<String, ZipEntry> entries;

        private bool closed = false;



        private Stream openFile(String name)
        {
            return new FileStream(name, FileMode.Open, FileAccess.Read);
        }


        /**
         * Opens a Zip file with the given name for reading.
         * @exception IOException if a i/o error occured.
         * @exception ZipException if the file doesn't contain a valid zip
         * archive.  
         */
        public ZipFile(String name)
        {
            this.raf = openFile(name);
            this.name = name;
            checkZipFile();
        }


        private void checkZipFile()
        {
            bool valid = false;

            try
            {
                byte[] buf = new byte[4];
                raf.Read(buf, 0, buf.Length);
                int sig = buf[0] & 0xFF
                        | ((buf[1] & 0xFF) << 8)
                        | ((buf[2] & 0xFF) << 16)
                        | ((buf[3] & 0xFF) << 24);
                valid = sig == LOCSIG;
            }
            catch (IOException _)
            {
            }

            if (!valid)
            {
                try
                {
                    raf.Close();
                }
                catch (IOException _)
                {
                }
                throw new System.Exception("Not a valid zip file");
            }
        }

        /**
         * Checks if file is closed and throws an exception.
         */
        private void checkClosed()
        {
            if (closed)
                throw new System.Exception("ZipFile has closed: " + name);
        }

        /**
         * Read the central directory of a zip file and fill the entries
         * array.  This is called exactly once when first needed. It is called
         * while holding the lock on <code>raf</code>.
         *
         * @exception IOException if a i/o error occured.
         * @exception ZipException if the central directory is malformed 
         */
        private void readEntries()
        {
            /* Search for the End Of Central Directory.  When a zip comment is 
             * present the directory may start earlier.
             * Note that a comment has a maximum length of 64K, so that is the
             * maximum we search backwards.
             */
            PartialInputStream inp = new PartialInputStream(raf, 4096);
            long pos = raf.Length - ENDHDR;
            long top = Math.Max(0, pos - 65536);
            do
            {
                if (pos < top)
                    throw new System.Exception
                      ("central directory not found, probably not a zip file: " + name);
                inp.seek(pos--);
            }
            while (inp.readLeInt() != ENDSIG);

            if (inp.skip(ENDTOT - ENDNRD) != ENDTOT - ENDNRD)
                throw new System.Exception(name);
            int count = inp.readLeShort();
            if (inp.skip(ENDOFF - ENDSIZ) != ENDOFF - ENDSIZ)
                throw new System.Exception(name);
            int centralOffset = inp.readLeInt();

            entries = new Dictionary<String, ZipEntry>(count + count / 2);
            inp.seek(centralOffset);

            for (int i = 0; i < count; i++)
            {
                if (inp.readLeInt() != CENSIG)
                    throw new System.Exception("Wrong Central Directory signature: " + name);

                inp.skip(6);
                int method = inp.readLeShort();
                int dostime = inp.readLeInt();
                int crc = inp.readLeInt();
                int csize = inp.readLeInt();
                int size = inp.readLeInt();
                int nameLen = inp.readLeShort();
                int extraLen = inp.readLeShort();
                int commentLen = inp.readLeShort();
                inp.skip(8);
                int offset = inp.readLeInt();
                String _name = inp.readString(nameLen);

                ZipEntry entry = new ZipEntry(_name);
                entry.setMethod(method);
                entry.setCrc(crc & 0xffffffffL);
                entry.setSize(size & 0xffffffffL);
                entry.setCompressedSize(csize & 0xffffffffL);
                entry.setDOSTime(dostime);
                if (extraLen > 0)
                {
                    byte[] extra = new byte[extraLen];
                    inp.readFully(extra);
                    entry.setExtra(extra);
                }
                if (commentLen > 0)
                {
                    entry.setComment(inp.readString(commentLen));
                }
                entry.offset = offset;
                entries[_name] = entry;
            }
        }

        /**
         * Closes the ZipFile.  This also closes all input streams given by
         * this class.  After this is called, no further method should be
         * called.
         * 
         * @exception IOException if a i/o error occured.
         */
        public void close()
        {
            Stream raf = this.raf;
            if (raf == null)
                return;

            lock (raf)
            {
                closed = true;
                entries = null;
                raf.Close();
            }
        }

        /**
         * Calls the <code>close()</code> method when this ZipFile has not yet
         * been explicitly closed.
         */
        protected void finalize()
        {
            if (!closed && raf != null) close();
        }

        /**
         * Returns an enumeration of all Zip entries in this Zip file.
         *
         * @exception IllegalStateException when the ZipFile has already been closed
         */
        //public Enumeration<? extends ZipEntry> entries()
        //{
        //  checkClosed();

        //  try
        //    {
        //  return new ZipEntryEnumeration(getEntries().values().iterator());
        //    }
        //  catch (IOException ioe)
        //    {
        //  return new EmptyEnumeration<ZipEntry>();
        //    }
        //}

        /**
         * Checks that the ZipFile is still open and reads entries when necessary.
         *
         * @exception IllegalStateException when the ZipFile has already been closed.
         * @exception IOException when the entries could not be read.
         */
        public Dictionary<String, ZipEntry> getEntries()
        {
            lock (raf)
            {
                checkClosed();

                if (entries == null)
                    readEntries();

                return entries;
            }
        }

        /**
         * Searches for a zip entry in this archive with the given name.
         *
         * @param name the name. May contain directory components separated by
         * slashes ('/').
         * @return the zip entry, or null if no entry with that name exists.
         *
         * @exception IllegalStateException when the ZipFile has already been closed
         */
        public ZipEntry getEntry(String name)
        {
            checkClosed();

            try
            {
                Dictionary<String, ZipEntry> entries = getEntries();
                ZipEntry entry = entries[name];
                // If we didn't find it, maybe it's a directory.
                if (entry == null && !name.EndsWith("/"))
                    entry = entries[name + '/'];
                return entry != null ? new ZipEntry(entry, name) : null;
            }
            catch (IOException ioe)
            {
                return null;
            }
        }

        /**
         * Creates an input stream reading the given zip entry as
         * uncompressed data.  Normally zip entry should be an entry
         * returned by getEntry() or entries().
         *
         * This implementation returns null if the requested entry does not
         * exist.  This decision is not obviously correct, however, it does
         * appear to mirror Sun's implementation, and it is consistant with
         * their javadoc.  On the other hand, the old JCL book, 2nd Edition,
         * claims that this should return a "non-null ZIP entry".  We have
         * chosen for now ignore the old book, as modern versions of Ant (an
         * important application) depend on this behaviour.  See discussion
         * in this thread:
         * http://gcc.gnu.org/ml/java-patches/2004-q2/msg00602.html
         *
         * @param entry the entry to create an InputStream for.
         * @return the input stream, or null if the requested entry does not exist.
         *
         * @exception IllegalStateException when the ZipFile has already been closed
         * @exception IOException if a i/o error occured.
         * @exception ZipException if the Zip archive is malformed.  
         */
        public Stream getInputStream(ZipEntry entry)
        {
            checkClosed();

            Dictionary<String, ZipEntry> entries = getEntries();
            String name = entry.getName();
            ZipEntry zipEntry = entries[name];
            if (zipEntry == null)
                return null;

            PartialInputStream inp = new PartialInputStream(raf, 1024);
            inp.seek(zipEntry.offset);

            if (inp.readLeInt() != LOCSIG)
                throw new System.Exception("Wrong Local header signature: " + name);

            inp.skip(4);

            if (zipEntry.getMethod() != inp.readLeShort())
                throw new System.Exception("Compression method mismatch: " + name);

            inp.skip(16);

            int nameLen = inp.readLeShort();
            int extraLen = inp.readLeShort();
            inp.skip(nameLen + extraLen);

            inp.setLength(zipEntry.getCompressedSize());

            int method = zipEntry.getMethod();
            switch (method)
            {
                case ZipOutputStream.STORED:
                    return inp;
                case ZipOutputStream.DEFLATED:
                    inp.addDummyByte();
                    return new System.IO.Compression.DeflateStream(inp, System.IO.Compression.CompressionMode.Decompress);
//                    Inflater inf = new Inflater(true);
                    //int sz = (int)entry.getSize();
                //return new InflateStream(sz,inp,inf);
                //return new InflaterInputStream(inp, inf)
                //{
                //  public int available() 
                //  {
                //    if (sz == -1)
                //      return super.available();
                //    if (super.available() != 0)
                //      return sz - inf.getTotalOut();
                //    return 0;
                //  }
                //};
                default:
                throw new System.Exception("Unknown compression method " + method);
            }
        }
        class InflateStream : InflaterInputStream
        {
            public InflateStream(int sz, Stream inp, Inflater inf):base(inp,inf)
            {
            }
        }
        /**
         * Returns the (path) name of this zip file.
         */
        public String getName()
        {
            return name;
        }

        /**
         * Returns the number of entries in this zip file.
         *
         * @exception IllegalStateException when the ZipFile has already been closed
         */
        public int size()
        {
            checkClosed();

            try
            {
                return getEntries().Count();
            }
            catch (IOException ioe)
            {
                return 0;
            }
        }

        //private static class ZipEntryEnumeration implements Enumeration<ZipEntry>
        //{
        //  private final Iterator<ZipEntry> elements;

        //  public ZipEntryEnumeration(Iterator<ZipEntry> elements)
        //  {
        //    this.elements = elements;
        //  }

        //  public boolean hasMoreElements()
        //  {
        //    return elements.hasNext();
        //  }

        //  public ZipEntry nextElement()
        //  {
        //    /* We return a clone, just to be safe that the user doesn't
        //     * change the entry.  
        //     */
        //    return (ZipEntry) (elements.next().clone());
        //  }
        //}

        public class PartialInputStream : Stream
        {
            /**
             * The UTF-8 charset use for decoding the filenames.
             */
            //private static final Charset UTF8CHARSET = Charset.forName("UTF-8");

            /**
             * The actual UTF-8 decoder. Created on demand. 
             */
            //private CharsetDecoder utf8Decoder;

            private Stream raf;
            private byte[] buffer;
            private long bufferOffset;
            private int pos;
            private long end;
            // We may need to supply an extra dummy byte to our reader.
            // See Inflater.  We use a count here to simplify the logic
            // elsewhere in this class.  Note that we ignore the dummy
            // byte in methods where we know it is not needed.
            private int dummyByteCount;

            public override bool CanRead
            {
                get { return true; }
            }
            public override bool CanSeek
            {
                get { throw new NotImplementedException(); }
            }
            public override bool CanWrite
            {
                get { throw new NotImplementedException(); }
            }
            public override void Flush()
            {
                throw new NotImplementedException();
            }
            public override long Length
            {
                get { throw new NotImplementedException(); }
            }
            public override long Position
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
            public override int Read(byte[] buffer, int offset, int count)
            {
                for (int i = 0; i < count; i++)
                    buffer[offset + i] = (byte)read();
                return count;
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }
            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }
            public PartialInputStream(Stream raf, int bufferSize)
            {
                this.raf = raf;
                buffer = new byte[bufferSize];
                bufferOffset = -buffer.Length;
                pos = buffer.Length;
                end = raf.Length;
            }

            public void setLength(long length)
            {
                end = bufferOffset + pos + length;
            }

            private void fillBuffer()
            {
                lock (raf)
                {
                    long len = end - bufferOffset;
                    if (len == 0 && dummyByteCount > 0)
                    {
                        buffer[0] = 0;
                        dummyByteCount = 0;
                    }
                    else
                    {
                        raf.Seek(bufferOffset, SeekOrigin.Begin);
                        raf.Read(buffer, 0, (int)Math.Min(buffer.Length, len));
                    }
                }
            }

            public int available()
            {
                long amount = end - (bufferOffset + pos);
                if (amount > int.MaxValue)
                    return int.MaxValue;
                return (int)amount;
            }

            public int read()
            {
                if (bufferOffset + pos >= end + dummyByteCount)
                    return -1;
                if (pos == buffer.Length)
                {
                    bufferOffset += buffer.Length;
                    pos = 0;
                    fillBuffer();
                }

                return buffer[pos++] & 0xFF;
            }

            public int read(byte[] b, int off, int len)
            {
                if (len > end + dummyByteCount - (bufferOffset + pos))
                {
                    len = (int)(end + dummyByteCount - (bufferOffset + pos));
                    if (len == 0)
                        return -1;
                }

                int totalBytesRead = Math.Min(buffer.Length - pos, len);
                Buffer.BlockCopy(buffer, pos, b, off, totalBytesRead);
                pos += totalBytesRead;
                off += totalBytesRead;
                len -= totalBytesRead;

                while (len > 0)
                {
                    bufferOffset += buffer.Length;
                    pos = 0;
                    fillBuffer();
                    int remain = Math.Min(buffer.Length, len);
                    Buffer.BlockCopy(buffer, pos, b, off, remain);
                    pos += remain;
                    off += remain;
                    len -= remain;
                    totalBytesRead += remain;
                }

                return totalBytesRead;
            }

            public long skip(long amount)
            {
                if (amount < 0)
                    return 0;
                if (amount > end - (bufferOffset + pos))
                    amount = end - (bufferOffset + pos);
                seek(bufferOffset + pos + amount);
                return amount;
            }

            public void seek(long newpos)
            {
                long offset = newpos - bufferOffset;
                if (offset >= 0 && offset <= buffer.Length)
                {
                    pos = (int)offset;
                }
                else
                {
                    bufferOffset = newpos;
                    pos = 0;
                    fillBuffer();
                }
            }

            public void readFully(byte[] buf)
            {
                if (read(buf, 0, buf.Length) != buf.Length)
                    throw new System.Exception();
            }

            public void readFully(byte[] buf, int off, int len)
            {
                if (read(buf, off, len) != len)
                    throw new System.Exception();
            }

            public int readLeShort()
            {
                int result;
                if (pos + 1 < buffer.Length)
                {
                    result = ((buffer[pos + 0] & 0xff) | (buffer[pos + 1] & 0xff) << 8);
                    pos += 2;
                }
                else
                {
                    int b0 = read();
                    int b1 = read();
                    if (b1 == -1)
                        throw new System.Exception();
                    result = (b0 & 0xff) | (b1 & 0xff) << 8;
                }
                return result;
            }

            public int readLeInt()
            {
                int result;
                if (pos + 3 < buffer.Length)
                {
                    result = (((buffer[pos + 0] & 0xff) | (buffer[pos + 1] & 0xff) << 8)
                             | ((buffer[pos + 2] & 0xff)
                                 | (buffer[pos + 3] & 0xff) << 8) << 16);
                    pos += 4;
                }
                else
                {
                    int b0 = read();
                    int b1 = read();
                    int b2 = read();
                    int b3 = read();
                    if (b3 == -1)
                        throw new System.Exception();
                    result = (((b0 & 0xff) | (b1 & 0xff) << 8) | ((b2 & 0xff)
                              | (b3 & 0xff) << 8) << 16);
                }
                return result;
            }

            /**
             * Decode chars from byte buffer using UTF8 encoding.  This
             * operation is performance-critical since a jar file contains a
             * large number of strings for the name of each file in the
             * archive.  This routine therefore avoids using the expensive
             * utf8Decoder when decoding is straightforward.
             *
             * @param buffer the buffer that contains the encoded character
             *        data
             * @param pos the index in buffer of the first byte of the encoded
             *        data
             * @param length the length of the encoded data in number of
             *        bytes.
             *
             * @return a String that contains the decoded characters.
             */
            private String decodeChars(byte[] buffer, int pos, int length)
            {
                String result;
                int i = length - 1;
                while ((i >= 0) && (buffer[i] <= 0x7f))
                {
                    i--;
                }
                if (i < 0)
                {
                    result = Encoding.Default.GetString(buffer, pos, length);
                }
                else
                {
                    //ByteBuffer bufferBuffer = ByteBuffer.wrap(buffer, pos, length);
                    //if (utf8Decoder == null)
                    //  utf8Decoder = UTF8CHARSET.newDecoder();
                    //utf8Decoder.reset();
                    //char [] characters = utf8Decoder.decode(bufferBuffer).array();
                    //result = String.valueOf(characters);
                    result = Encoding.Default.GetString(buffer, pos, length);
                    
                }
                return result;
            }

            public String readString(int length)
            {
                if (length > end - (bufferOffset + pos))
                    throw new System.Exception();

                String result = null;
                //try
                {
                    if (buffer.Length - pos >= length)
                    {
                        result = decodeChars(buffer, pos, length);
                        pos += length;
                    }
                    else
                    {
                        byte[] b = new byte[length];
                        readFully(b);
                        result = decodeChars(b, 0, length);
                    }
                }
                //catch (UnsupportedEncodingException uee)
                //  {
                //    throw new AssertionError(uee);
                //  }
                return result;
            }

            public void addDummyByte()
            {
                dummyByteCount = 1;
            }
        }

    }
}
