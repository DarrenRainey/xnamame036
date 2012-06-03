using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class Deflater
    {
         /**
   * The best and slowest compression level.  This tries to find very
   * long and distant string repetitions.  
   */
  public static  int BEST_COMPRESSION = 9;
  /**
   * The worst but fastest compression level.  
   */
  public static  int BEST_SPEED = 1;
  /**
   * The default compression level.
   */
  public static  int DEFAULT_COMPRESSION = -1;
  /**
   * This level won't compress at all but output uncompressed blocks.
   */
  public static  int NO_COMPRESSION = 0;

  /**
   * The default strategy.
   */
  public static  int DEFAULT_STRATEGY = 0;
  /**
   * This strategy will only allow longer string repetitions.  It is
   * useful for random data with a small character set.
   */
  public static  int FILTERED = 1;

  /** 
   * This strategy will not look for string repetitions at all.  It
   * only encodes with Huffman trees (which means, that more common
   * characters get a smaller encoding.  
   */
  public static  int HUFFMAN_ONLY = 2;

  /**
   * The compression method.  This is the only method supported so far.
   * There is no need to use this constant at all.
   */
  public static  int DEFLATED = 8;

  /*
   * The Deflater can do the following state transitions:
   *
   * (1) -> INIT_STATE   ----> INIT_FINISHING_STATE ---.
   *        /  | (2)      (5)                         |
   *       /   v          (5)                         |
   *   (3)| SETDICT_STATE ---> SETDICT_FINISHING_STATE |(3)
   *       \   | (3)                 |        ,-------'
   *        |  |                     | (3)   /
   *        v  v          (5)        v      v
   * (1) -> BUSY_STATE   ----> FINISHING_STATE
   *                                | (6)
   *                                v
   *                           FINISHED_STATE
   *    \_____________________________________/
   *          | (7)
   *          v
   *        CLOSED_STATE
   *
   * (1) If we should produce a header we start in INIT_STATE, otherwise
   *     we start in BUSY_STATE.
   * (2) A dictionary may be set only when we are in INIT_STATE, then
   *     we change the state as indicated.
   * (3) Whether a dictionary is set or not, on the first call of deflate
   *     we change to BUSY_STATE.
   * (4) -- intentionally left blank -- :)
   * (5) FINISHING_STATE is entered, when flush() is called to indicate that
   *     there is no more INPUT.  There are also states indicating, that
   *     the header wasn't written yet.
   * (6) FINISHED_STATE is entered, when everything has been flushed to the
   *     internal pending output buffer.
   * (7) At any time (7)
   * 
   */

  private const int IS_SETDICT              = 0x01;
  private const int IS_FLUSHING             = 0x04;
  private const int IS_FINISHING            = 0x08;
          
  private const int INIT_STATE              = 0x00;
  private const int SETDICT_STATE           = 0x01;
  private const int INIT_FINISHING_STATE    = 0x08;
  private const int SETDICT_FINISHING_STATE = 0x09;
  private const int BUSY_STATE              = 0x10;
  private const int FLUSHING_STATE          = 0x14;
  private const int FINISHING_STATE         = 0x1c;
  private const int FINISHED_STATE          = 0x1e;
  private const int CLOSED_STATE            = 0x7f;

  /** Compression level. */
  private int level;

  /** should we include a header. */
  private bool noHeader;

  /** The current state. */
  private int state;

  /** The total bytes of output written. */
  private long totalOut;
 


    }
}
