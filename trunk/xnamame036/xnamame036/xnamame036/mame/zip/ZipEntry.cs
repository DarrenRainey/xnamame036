using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class ZipEntry : ZipConstants
    {
        private static byte KNOWN_SIZE = 1;
        private static byte KNOWN_CSIZE = 2;
        private static byte KNOWN_CRC = 4;
        private static byte KNOWN_TIME = 8;
        private static byte KNOWN_DOSTIME = 16;
        private static byte KNOWN_EXTRA = 32;

        /** Immutable name of the entry */
        private String name;
        /** Uncompressed size */
        private int size;
        /** Compressed size */
        private long compressedSize = -1;
        /** CRC of uncompressed data */
        private int crc;
        /** Comment or null if none */
        private String comment = null;
        /** The compression method. Either DEFLATED or STORED, by default -1. */
        private byte method = unchecked((byte)-1);
        /** Flags specifying what we know about this entry */
        private byte known = 0;
        /**
         * The 32bit DOS encoded format for the time of this entry. Only valid if
         * KNOWN_DOSTIME is set in known.
         */
        private int dostime;
        /**
         * The 64bit Java encoded millisecond time since the beginning of the epoch.
         * Only valid if KNOWN_TIME is set in known.
         */
        private long time;
        /** Extra data */
        private byte[] extra = null;

        int flags;              /* used by ZipOutputStream */
        public int offset;             /* used by ZipFile and ZipOutputStream */

        /**
         * Compression method.  This method doesn't compress at all.
         */
        public static int STORED = 0;
        /**
         * Compression method.  This method uses the Deflater.
         */
        public static int DEFLATED = 8;

        /**
         * Creates a zip entry with the given name.
         * @param name the name. May include directory components separated
         * by '/'.
         *
         * @exception NullPointerException when name is null.
         * @exception IllegalArgumentException when name is bigger then 65535 chars.
         */
        public ZipEntry(String name)
        {
            int length = name.Length;
            if (length > 65535)
                throw new System.Exception("name length is " + length);
            this.name = name;
        }

        /**
         * Creates a copy of the given zip entry.
         * @param e the entry to copy.
         */
        public ZipEntry(ZipEntry e) : this(e, e.name) { }

        public ZipEntry(ZipEntry e, String name)
        {
            this.name = name;
            known = e.known;
            size = e.size;
            compressedSize = e.compressedSize;
            crc = e.crc;
            dostime = e.dostime;
            time = e.time;
            method = e.method;
            extra = e.extra;
            comment = e.comment;
        }

        public void setDOSTime(int dostime)
        {
            this.dostime = dostime;
            known |= KNOWN_DOSTIME;
            known &= (byte)~KNOWN_TIME;
        }

        public int getDOSTime()
        {
            if ((known & KNOWN_DOSTIME) != 0)
                return dostime;
            else if ((known & KNOWN_TIME) != 0)
            {
                DateTime dt = new DateTime(time);
                dostime = (dt.Year - 1980 & 0x7f) << 25
                   | (dt.Month + 1) << 21
                   | (dt.Day) << 16
                   | (dt.Hour) << 11
                   | (dt.Minute) << 5
                   | (dt.Second) >> 1;
                known |= KNOWN_DOSTIME;
                return dostime;
            }
            else
                return 0;
        }

        /**
         * Creates a copy of this zip entry.
         */
        //public Object clone()
        //{
        //  // JCL defines this as being the same as the copy constructor above,
        //  // except that value of the "extra" field is also copied. Take care
        //  // that in the case of a subclass we use clone() rather than the copy
        //  // constructor.
        //  ZipEntry clone;
        //  if (this.getClass() == ZipEntry.class)
        //    clone = new ZipEntry(this);
        //  else
        //    {
        //     try
        //       {
        //        clone = (ZipEntry) super.clone();
        //       }
        //     catch (CloneNotSupportedException e)
        //       {
        //        throw new InternalError();
        //       }
        //    }
        //  if (extra != null)
        //    {
        //     clone.extra = new byte[extra.length];
        //     System.arraycopy(extra, 0, clone.extra, 0, extra.length);
        //    }
        //  return clone;
        //}

        /**
         * Returns the entry name.  The path components in the entry are
         * always separated by slashes ('/').  
         */
        public String getName()
        {
            return name;
        }

        /**
         * Sets the time of last modification of the entry.
         * @time the time of last modification of the entry.
         */
        public void setTime(long time)
        {
            this.time = time;
            this.known |= KNOWN_TIME;
            this.known &= (byte)~KNOWN_DOSTIME;
        }

        /**
         * Gets the time of last modification of the entry.
         * @return the time of last modification of the entry, or -1 if unknown.
         */
        public long getTime()
        {
            // The extra bytes might contain the time (posix/unix extension)
            parseExtra();

            if ((known & KNOWN_TIME) != 0)
                return time;
            else if ((known & KNOWN_DOSTIME) != 0)
            {
                int sec = 2 * (dostime & 0x1f);
                int min = (dostime >> 5) & 0x3f;
                int hrs = (dostime >> 11) & 0x1f;
                int day = (dostime >> 16) & 0x1f;
                int mon = ((dostime >> 21) & 0xf) - 1;
                int year = ((dostime >> 25) & 0x7f) + 1980; /* since 1900 */

                DateTime dt = new DateTime(year, mon, day, hrs, min, sec);
                time = dt.Ticks;
                known |= KNOWN_TIME;
                return time;
            }
            else
                return -1;
        }

        /**
         * Sets the size of the uncompressed data.
         * @exception IllegalArgumentException if size is not in 0..0xffffffffL
         */
        public void setSize(long size)
        {
            if ((size & unchecked((long)0xffffffff00000000L)) != 0)
                throw new System.Exception();
            this.size = (int)size;
            this.known |= KNOWN_SIZE;
        }

        /**
         * Gets the size of the uncompressed data.
         * @return the size or -1 if unknown.
         */
        public long getSize()
        {
            return (known & KNOWN_SIZE) != 0 ? size & 0xffffffffL : -1L;
        }

        /**
         * Sets the size of the compressed data.
         */
        public void setCompressedSize(long csize)
        {
            this.compressedSize = csize;
        }

        /**
         * Gets the size of the compressed data.
         * @return the size or -1 if unknown.
         */
        public long getCompressedSize()
        {
            return compressedSize;
        }

        /**
         * Sets the crc of the uncompressed data.
         * @exception IllegalArgumentException if crc is not in 0..0xffffffffL
         */
        public void setCrc(long crc)
        {
            if ((crc & unchecked((long)0xffffffff00000000L)) != 0)
                throw new System.Exception();
            this.crc = (int)crc;
            this.known |= KNOWN_CRC;
        }

        /**
         * Gets the crc of the uncompressed data.
         * @return the crc or -1 if unknown.
         */
        public long getCrc()
        {
            return (known & KNOWN_CRC) != 0 ? crc & 0xffffffffL : -1L;
        }

        /**
         * Sets the compression method.  Only DEFLATED and STORED are
         * supported.
         * @exception IllegalArgumentException if method is not supported.
         * @see ZipOutputStream#DEFLATED
         * @see ZipOutputStream#STORED 
         */
        public void setMethod(int method)
        {
            if (method != ZipOutputStream.STORED
            && method != ZipOutputStream.DEFLATED)
                throw new System.Exception();
            this.method = (byte)method;
        }

        /**
         * Gets the compression method.  
         * @return the compression method or -1 if unknown.
         */
        public int getMethod()
        {
            return method;
        }

        /**
         * Sets the extra data.
         * @exception IllegalArgumentException if extra is longer than 0xffff bytes.
         */
        public void setExtra(byte[] extra)
        {
            if (extra == null)
            {
                this.extra = null;
                return;
            }
            if (extra.Length > 0xffff)
                throw new System.Exception();
            this.extra = extra;
        }

        private void parseExtra()
        {
            // Already parsed?
            if ((known & KNOWN_EXTRA) != 0)
                return;

            if (extra == null)
            {
                known |= KNOWN_EXTRA;
                return;
            }

            //try
            {
                int pos = 0;
                while (pos < extra.Length)
                {
                    int sig = (extra[pos++] & 0xff)
                      | (extra[pos++] & 0xff) << 8;
                    int len = (extra[pos++] & 0xff)
                      | (extra[pos++] & 0xff) << 8;
                    if (sig == 0x5455)
                    {
                        /* extended time stamp */
                        int flags = extra[pos];
                        if ((flags & 1) != 0)
                        {
                            long time = ((extra[pos + 1] & 0xff)
                                | (extra[pos + 2] & 0xff) << 8
                                | (extra[pos + 3] & 0xff) << 16
                                | (extra[pos + 4] & 0xff) << 24);
                            setTime(time * 1000);
                        }
                    }
                    pos += len;
                }
            }
            //catch (ArrayIndexOutOfBoundsException ex)
            //  {
            ///* be lenient */
            //  }

            known |= KNOWN_EXTRA;
            return;
        }

        /**
         * Gets the extra data.
         * @return the extra data or null if not set.
         */
        public byte[] getExtra()
        {
            return extra;
        }

        /**
         * Sets the entry comment.
         * @exception IllegalArgumentException if comment is longer than 0xffff.
         */
        public void setComment(String comment)
        {
            if (comment != null && comment.Length > 0xffff)
                throw new System.Exception();
            this.comment = comment;
        }

        /**
         * Gets the comment.
         * @return the comment or null if not set.
         */
        public String getComment()
        {
            return comment;
        }

        /**
         * Gets true, if the entry is a directory.  This is solely
         * determined by the name, a trailing slash '/' marks a directory.  
         */
        public bool isDirectory()
        {
            int nlen = name.Length;
            return nlen > 0 && name[nlen - 1] == '/';
        }

        /**
         * Gets the string representation of this ZipEntry.  This is just
         * the name as returned by getName().
         */
        public String toString()
        {
            return name;
        }

        /**
         * Gets the hashCode of this ZipEntry.  This is just the hashCode
         * of the name.  Note that the equals method isn't changed, though.
         */
        public int hashCode()
        {
            return name.GetHashCode();
        }
    }
}
