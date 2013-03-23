using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_irobot : Mame.GameDriver
    {
        static object irmb_timer;

        /* Run mathbox */
        static void irmb_run()
        {
            int prevop = 0;
            int curop = 0;

            uint Q = 0;
            uint Y = 0;
            uint nflag = 0;
            uint vflag = 0;
            uint cflag = 0;
            uint zresult = 1;
            uint CI = 0;
            uint SP = 0;
            uint icount = 0;

            while ((mbops[prevop].flags & (FL_DPSEL | FL_carry)) != (FL_DPSEL | FL_carry))
            {
                uint result;
                uint fu;
                uint tmp;

                icount += mbops[curop].cycles;

                /* Get function code */
                fu = mbops[curop].func;

                /* Modify function for MULT */
                if ((mbops[prevop].flags & FL_MULT) == 0 || (Q & 1) != 0)
                    fu = fu ^ 0x02;
                else
                    fu = fu | 0x02;

                /* Modify function for DIV */
                if ((mbops[prevop].flags & FL_DIV) != 0 || nflag != 0)
                    fu = fu ^ 0x08;
                else
                    fu = fu | 0x08;

                /* Do source and operation */
                switch (fu & 0x03f)
                {
                    case 0x00: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = mbops[curop].areg[0] + Q + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].areg[0] & 0x7fff) + (Q & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x01: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = mbops[curop].areg[0] + mbops[curop].breg[0] + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].areg[0] & 0x7fff) + (mbops[curop].breg[0] & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x02: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = 0 + Q + CI; cflag = (result >> 16) & 1; vflag = (((0 & 0x7fff) + (Q & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x03: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = 0 + mbops[curop].breg[0] + CI; cflag = (result >> 16) & 1; vflag = (((0 & 0x7fff) + (mbops[curop].breg[0] & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x04: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = 0 + mbops[curop].areg[0] + CI; cflag = (result >> 16) & 1; vflag = (((0 & 0x7fff) + (mbops[curop].areg[0] & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x05: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = tmp + mbops[curop].areg[0] + CI; cflag = (result >> 16) & 1; vflag = (((tmp & 0x7fff) + (mbops[curop].areg[0] & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x06: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = tmp + Q + CI; cflag = (result >> 16) & 1; vflag = (((tmp & 0x7fff) + (Q & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x07: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = tmp + 0 + CI; cflag = (result >> 16) & 1; vflag = (((tmp & 0x7fff) + (0 & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x08: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (mbops[curop].areg[0] ^ 0xFFFF) + Q + CI; cflag = (result >> 16) & 1; vflag = (((Q & 0x7fff) + ((mbops[curop].areg[0] ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x09: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (mbops[curop].areg[0] ^ 0xFFFF) + mbops[curop].breg[0] + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].breg[0] & 0x7fff) + ((mbops[curop].areg[0] ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x0a: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (0 ^ 0xFFFF) + Q + CI; cflag = (result >> 16) & 1; vflag = (((Q & 0x7fff) + ((0 ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x0b: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (0 ^ 0xFFFF) + mbops[curop].breg[0] + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].breg[0] & 0x7fff) + ((0 ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x0c: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (0 ^ 0xFFFF) + mbops[curop].areg[0] + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].areg[0] & 0x7fff) + ((0 ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x0d: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (tmp ^ 0xFFFF) + mbops[curop].areg[0] + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].areg[0] & 0x7fff) + ((tmp ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x0e: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (tmp ^ 0xFFFF) + Q + CI; cflag = (result >> 16) & 1; vflag = (((Q & 0x7fff) + ((tmp ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x0f: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = (tmp ^ 0xFFFF) + 0 + CI; cflag = (result >> 16) & 1; vflag = (((0 & 0x7fff) + ((tmp ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x10: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = mbops[curop].areg[0] + (Q ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].areg[0] & 0x7fff) + ((Q ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x11: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = mbops[curop].areg[0] + (mbops[curop].breg[0] ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((mbops[curop].areg[0] & 0x7fff) + ((mbops[curop].breg[0] ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x12: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = 0 + (Q ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((0 & 0x7fff) + ((Q ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x13: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = 0 + (mbops[curop].breg[0] ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((0 & 0x7fff) + ((mbops[curop].breg[0] ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x14: CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = 0 + (mbops[curop].areg[0] ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((0 & 0x7fff) + ((mbops[curop].areg[0] ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x15: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = tmp + (mbops[curop].areg[0] ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((tmp & 0x7fff) + ((mbops[curop].areg[0] ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x16: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = tmp + (Q ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((tmp & 0x7fff) + ((Q ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x17: tmp = irmb_din(curop); CI = 0; if ((mbops[curop].flags & FL_DPSEL) != 0) CI = cflag; else { if ((mbops[curop].flags & FL_carry) != 0) CI = 1; if ((mbops[prevop].flags & FL_DIV) == 0 && nflag == 0) CI = 1; }; result = tmp + (0 ^ 0xFFFF) + CI; cflag = (result >> 16) & 1; vflag = (((tmp & 0x7fff) + ((0 ^ 0xffff) & 0x7fff) + CI) >> 15) ^ cflag; break;
                    case 0x18: result = mbops[curop].areg[0] | Q; vflag = cflag = 0; break;
                    case 0x19: result = mbops[curop].areg[0] | mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x1a: result = 0 | Q; vflag = cflag = 0; break;
                    case 0x1b: result = 0 | mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x1c: result = 0 | mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x1d: result = irmb_din(curop) | mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x1e: result = irmb_din(curop) | Q; vflag = cflag = 0; break;
                    case 0x1f: result = irmb_din(curop) | 0; vflag = cflag = 0; break;
                    case 0x20: result = mbops[curop].areg[0] & Q; vflag = cflag = 0; break;
                    case 0x21: result = mbops[curop].areg[0] & mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x22: result = 0 & Q; vflag = cflag = 0; break;
                    case 0x23: result = 0 & mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x24: result = 0 & mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x25: result = irmb_din(curop) & mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x26: result = irmb_din(curop) & Q; vflag = cflag = 0; break;
                    case 0x27: result = irmb_din(curop) & 0; vflag = cflag = 0; break;
                    case 0x28: result = (mbops[curop].areg[0] ^ 0xFFFF) & Q; vflag = cflag = 0; break;
                    case 0x29: result = (mbops[curop].areg[0] ^ 0xFFFF) & mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x2a: result = (0 ^ 0xFFFF) & Q; vflag = cflag = 0; break;
                    case 0x2b: result = (0 ^ 0xFFFF) & mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x2c: result = (0 ^ 0xFFFF) & mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x2d: result = (irmb_din(curop) ^ 0xFFFF) & mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x2e: result = (irmb_din(curop) ^ 0xFFFF) & Q; vflag = cflag = 0; break;
                    case 0x2f: result = (irmb_din(curop) ^ 0xFFFF) & 0; vflag = cflag = 0; break;
                    case 0x30: result = mbops[curop].areg[0] ^ Q; vflag = cflag = 0; break;
                    case 0x31: result = mbops[curop].areg[0] ^ mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x32: result = 0 ^ Q; vflag = cflag = 0; break;
                    case 0x33: result = 0 ^ mbops[curop].breg[0]; vflag = cflag = 0; break;
                    case 0x34: result = 0 ^ mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x35: result = irmb_din(curop) ^ mbops[curop].areg[0]; vflag = cflag = 0; break;
                    case 0x36: result = irmb_din(curop) ^ Q; vflag = cflag = 0; break;
                    case 0x37: result = irmb_din(curop) ^ 0; vflag = cflag = 0; break;
                    case 0x38: result = (mbops[curop].areg[0] ^ Q) ^ 0xFFFF; vflag = cflag = 0; break;
                    case 0x39: result = (mbops[curop].areg[0] ^ mbops[curop].breg[0]) ^ 0xFFFF; vflag = cflag = 0; break;
                    case 0x3a: result = (0 ^ Q) ^ 0xFFFF; vflag = cflag = 0; break;
                    case 0x3b: result = (0 ^ mbops[curop].breg[0]) ^ 0xFFFF; vflag = cflag = 0; break;
                    case 0x3c: result = (0 ^ mbops[curop].areg[0]) ^ 0xFFFF; vflag = cflag = 0; break;
                    case 0x3d: result = (irmb_din(curop) ^ mbops[curop].areg[0]) ^ 0xFFFF; vflag = cflag = 0; break;
                    case 0x3e: result = (irmb_din(curop) ^ Q) ^ 0xFFFF; vflag = cflag = 0; break;
                    default:
                    case 0x3f: result = (irmb_din(curop) ^ 0) ^ 0xFFFF; vflag = cflag = 0; break;
                }

                /* Evaluate flags */
                zresult = result & 0xFFFF;
                nflag = zresult >> 15;

                prevop = curop;

                /* Do destination and jump */
                switch (fu >> 6)
                {
                    case 0x00:
                    case 0x08: Q = Y = zresult; curop++; ; break;
                    case 0x01:
                    case 0x09: Y = zresult; curop++; ; break;
                    case 0x02:
                    case 0x0a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; curop++; ; break;
                    case 0x03:
                    case 0x0b: mbops[curop].breg[0] = zresult; Y = zresult; curop++; ; break;
                    case 0x04: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; curop++; ; break;
                    case 0x05: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; curop++; ; break;
                    case 0x06: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; curop++; ; break;
                    case 0x07: mbops[curop].breg[0] = zresult << 1; Y = zresult; curop++; ; break;
                    case 0x0c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; curop++; ; break;
                    case 0x0d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; curop++; ; break;
                    case 0x0e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; curop++; ; break;
                    case 0x0f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; curop++; ; break;

                    case 0x10:
                    case 0x18: Q = Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x11:
                    case 0x19: Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x12:
                    case 0x1a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x13:
                    case 0x1b: mbops[curop].breg[0] = zresult; Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x14: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x15: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x16: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x17: mbops[curop].breg[0] = zresult << 1; Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x1c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x1d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x1e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x1f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; if (cflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;

                    case 0x20:
                    case 0x28: Q = Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x21:
                    case 0x29: Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x22:
                    case 0x2a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x23:
                    case 0x2b: mbops[curop].breg[0] = zresult; Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x24: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x25: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x26: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x27: mbops[curop].breg[0] = zresult << 1; Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x2c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x2d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x2e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x2f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; if (zresult == 0) curop = mbops[curop].nxtop; else curop++; ; break;

                    case 0x30:
                    case 0x38: Q = Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x31:
                    case 0x39: Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x32:
                    case 0x3a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x33:
                    case 0x3b: mbops[curop].breg[0] = zresult; Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x34: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x35: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x36: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x37: mbops[curop].breg[0] = zresult << 1; Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x3c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x3d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x3e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x3f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; if (nflag == 0) curop = mbops[curop].nxtop; else curop++; ; break;

                    case 0x40:
                    case 0x48: Q = Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x41:
                    case 0x49: Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x42:
                    case 0x4a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x43:
                    case 0x4b: mbops[curop].breg[0] = zresult; Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x44: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x45: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x46: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x47: mbops[curop].breg[0] = zresult << 1; Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x4c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x4d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x4e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;
                    case 0x4f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; if (nflag != 0) curop = mbops[curop].nxtop; else curop++; ; break;

                    case 0x50:
                    case 0x58: Q = Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x51:
                    case 0x59: Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x52:
                    case 0x5a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x53:
                    case 0x5b: mbops[curop].breg[0] = zresult; Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x54: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x55: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x56: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x57: mbops[curop].breg[0] = zresult << 1; Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x5c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x5d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x5e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; curop = mbops[curop].nxtop; ; break;
                    case 0x5f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; curop = mbops[curop].nxtop; ; break;

                    case 0x60:
                    case 0x68: Q = Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x61:
                    case 0x69: Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x62:
                    case 0x6a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x63:
                    case 0x6b: mbops[curop].breg[0] = zresult; Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x64: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x65: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x66: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x67: mbops[curop].breg[0] = zresult << 1; Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x6c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x6d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x6e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;
                    case 0x6f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; irmb_stack[SP] = curop + 1; SP = (SP + 1) & 15; curop = mbops[curop].nxtop; ; break;

                    case 0x70:
                    case 0x78: Q = Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x71:
                    case 0x79: Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x72:
                    case 0x7a: Y = mbops[curop].areg[0]; mbops[curop].breg[0] = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x73:
                    case 0x7b: mbops[curop].breg[0] = zresult; Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x74: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Q = (uint)((Q >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x75: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((mbops[curop].flags & 0x20) << 10)); Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x76: mbops[curop].breg[0] = zresult << 1; Q = (uint)(((Q << 1) & 0xffff) | (nflag ^ 1)); Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x77: mbops[curop].breg[0] = zresult << 1; Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x7c: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Q = (uint)((Q >> 1) | ((zresult & 0x01) << 15)); Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x7d: mbops[curop].breg[0] = (uint)((zresult >> 1) | ((nflag ^ vflag) << 15)); Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x7e: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Q = (Q << 1) & 0xffff; Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                    case 0x7f: mbops[curop].breg[0] = (uint)((zresult << 1) | ((Q & 0x8000) >> 15)); Y = zresult; SP = (SP - 1) & 15; curop = irmb_stack[SP]; ; break;
                }

                /* Do write */
                if ((mbops[prevop].flags & FL_MBRW) == 0)
                    irmb_dout(prevop, Y);

                /* ADDEN */
                if ((mbops[prevop].flags & FL_ADDEN) == 0)
                {
                    if ((mbops[prevop].flags & FL_MBRW) != 0)
                        irmb_latch = irmb_din(prevop);
                    else
                        irmb_latch = Y;
                }
            }

            if (!irmb_running)
            {
                irmb_timer = Mame.Timer.timer_set(Mame.Timer.TIME_IN_HZ(12000000) * icount, 0, irmb_done_callback);

            }
            else
            {

                Mame.Timer.timer_reset(irmb_timer, Mame.Timer.TIME_IN_NSEC(200) * icount);
            }
            irmb_running = true;
        }
        static void irmb_done_callback(int param)
        {
            irmb_running = false;
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_FIRQ_LINE, Mame.ASSERT_LINE);
        }


    }
}
