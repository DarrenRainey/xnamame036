using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public interface Checksum
    {
        /**
         * Returns the data checksum computed so far.
         */
        long getValue();

        /**
         * Resets the data checksum as if no update was ever called.
         */
        void reset();

        /**
         * Adds one byte to the data checksum.
         *
         * @param bval the data value to add. The high byte of the int is ignored.
         */
        void update(int bval);

        /**
         * Adds the byte array to the data checksum.
         *
         * @param buf the buffer which contains the data
         * @param off the offset in the buffer where the data starts
         * @param len the length of the data
         */
        void update(byte[] buf, int off, int len);
    }

}
