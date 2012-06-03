using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public partial class cpu_Z80 : cpu_interface
        {
            /* tmp1 value for ini/inir/outi/otir for [C.1-0][io.1-0] */
            static byte[][] irep_tmp1 = {
 new byte[]{0,0,1,0},new byte[]{0,1,0,1},new byte[]{1,0,1,1},new byte[]{0,1,1,0}
};

            /* tmp1 value for ind/indr/outd/otdr for [C.1-0][io.1-0] */
            static byte[][] drep_tmp1 = {
 new byte[]{0,1,0,0},new byte[]{1,0,0,1},new byte[]{0,0,1,0},new byte[]{0,1,0,1}
};

            /* tmp2 value for all in/out repeated opcodes for B.7-0 */
            static byte[] breg_tmp2 = {
 0,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,
 0,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,
 0,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,
 0,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,
 0,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,
 0,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,
 0,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1,
 0,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,1,0,0,1,0,1,1,0,0,1,1,0,1,0,0,
 1,0,1,1,0,1,0,0,1,1,0,0,1,0,1,1
};


            static byte[] cc_op = {
 4,10, 7, 6, 4, 4, 7, 4, 4,11, 7, 6, 4, 4, 7, 4,
 8,10, 7, 6, 4, 4, 7, 4,12,11, 7, 6, 4, 4, 7, 4,
 7,10,16, 6, 4, 4, 7, 4, 7,11,16, 6, 4, 4, 7, 4,
 7,10,13, 6,11,11,10, 4, 7,11,13, 6, 4, 4, 7, 4,
 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
 7, 7, 7, 7, 7, 7, 4, 7, 4, 4, 4, 4, 4, 4, 7, 4,
 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
 5,10,10,10,10,11, 7,11, 5, 4,10, 0,10,10, 7,11,
 5,10,10,11,10,11, 7,11, 5, 4,10,11,10, 0, 7,11,
 5,10,10,19,10,11, 7,11, 5, 4,10, 4,10, 0, 7,11,
 5,10,10, 4,10,11, 7,11, 5, 6,10, 4,10, 0, 7,11};


            static byte[] cc_cb = {
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
 8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
 8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
 8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
 8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8};

            static byte[] cc_dd = {
 4, 4, 4, 4, 4, 4, 4, 4, 4,15, 4, 4, 4, 4, 4, 4,
 4, 4, 4, 4, 4, 4, 4, 4, 4,15, 4, 4, 4, 4, 4, 4,
 4,14,20,10, 9, 9, 9, 4, 4,15,20,10, 9, 9, 9, 4,
 4, 4, 4, 4,23,23,19, 4, 4,15, 4, 4, 4, 4, 4, 4,
 4, 4, 4, 4, 9, 9,19, 4, 4, 4, 4, 4, 9, 9,19, 4,
 4, 4, 4, 4, 9, 9,19, 4, 4, 4, 4, 4, 9, 9,19, 4,
 9, 9, 9, 9, 9, 9,19, 9, 9, 9, 9, 9, 9, 9,19, 9,
19,19,19,19,19,19, 4,19, 4, 4, 4, 4, 9, 9,19, 4,
 4, 4, 4, 4, 9, 9,19, 4, 4, 4, 4, 4, 9, 9,19, 4,
 4, 4, 4, 4, 9, 9,19, 4, 4, 4, 4, 4, 9, 9,19, 4,
 4, 4, 4, 4, 9, 9,19, 4, 4, 4, 4, 4, 9, 9,19, 4,
 4, 4, 4, 4, 9, 9,19, 4, 4, 4, 4, 4, 9, 9,19, 4,
 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4,
 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
 4,14, 4,23, 4,15, 4, 4, 4, 8, 4, 4, 4, 4, 4, 4,
 4, 4, 4, 4, 4, 4, 4, 4, 4,10, 4, 4, 4, 4, 4, 4};

            // dd/fd cycles are identical


            static byte[] cc_xxcb = {
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23};

            static byte[] cc_ed = {
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
12,12,15,20, 8, 8, 8, 9,12,12,15,20, 8, 8, 8, 9,
12,12,15,20, 8, 8, 8, 9,12,12,15,20, 8, 8, 8, 9,
12,12,15,20, 8, 8, 8,18,12,12,15,20, 8, 8, 8,18,
12,12,15,20, 8, 8, 8, 8,12,12,15,20, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
16,16,16,16, 8, 8, 8, 8,16,16,16,16, 8, 8, 8, 8,
16,16,16,16, 8, 8, 8, 8,16,16,16,16, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8};

            public const int Z80_IGNORE_INT = -1, Z80_NMI_INT = -2, Z80_IRQ_INT = -1000;
            int[] z80_ICount = new int[1];
            public cpu_Z80()
            {
                cpu_num = CPU_Z80;
                num_irqs = 1;
                default_vector = 255;
                overclock = 1.0;
                no_int = Z80_IGNORE_INT;
                irq_int = Z80_IRQ_INT;
                nmi_int = Z80_NMI_INT;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_LE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = z80_ICount;

                SetupOpCodeLists();
            }


            void SetupOpCodeLists()
            {
                Setup_op();
                Setup_cb();
                Setup_ed();
                Setup_dd();
                Setup_fd();
                Setup_xxcb();
            }
            void Setup_op()
            {
                Z80op[0x00] = op_00;
                Z80op[0x01] = op_01;
                Z80op[0x02] = op_02;
                Z80op[0x03] = op_03;
                Z80op[0x04] = op_04;
                Z80op[0x05] = op_05;
                Z80op[0x06] = op_06;
                Z80op[0x07] = op_07;
                Z80op[0x08] = op_08;
                Z80op[0x09] = op_09;
                Z80op[0x0a] = op_0a;
                Z80op[0x0b] = op_0b;
                Z80op[0x0c] = op_0c;
                Z80op[0x0d] = op_0d;
                Z80op[0x0e] = op_0e;
                Z80op[0x0f] = op_0f;

                Z80op[0x10] = op_10;
                Z80op[0x11] = op_11;
                Z80op[0x12] = op_12;
                Z80op[0x13] = op_13;
                Z80op[0x14] = op_14;
                Z80op[0x15] = op_15;
                Z80op[0x16] = op_16;
                Z80op[0x17] = op_17;
                Z80op[0x18] = op_18;
                Z80op[0x19] = op_19;
                Z80op[0x1a] = op_1a;
                Z80op[0x1b] = op_1b;
                Z80op[0x1c] = op_1c;
                Z80op[0x1d] = op_1d;
                Z80op[0x1e] = op_1e;
                Z80op[0x1f] = op_1f;

                Z80op[0x20] = op_20;
                Z80op[0x21] = op_21;
                Z80op[0x22] = op_22;
                Z80op[0x23] = op_23;
                Z80op[0x24] = op_24;
                Z80op[0x25] = op_25;
                Z80op[0x26] = op_26;
                Z80op[0x27] = op_27;
                Z80op[0x28] = op_28;
                Z80op[0x29] = op_29;
                Z80op[0x2a] = op_2a;
                Z80op[0x2b] = op_2b;
                Z80op[0x2c] = op_2c;
                Z80op[0x2d] = op_2d;
                Z80op[0x2e] = op_2e;
                Z80op[0x2f] = op_2f;

                Z80op[0x30] = op_30;
                Z80op[0x31] = op_31;
                Z80op[0x32] = op_32;
                Z80op[0x33] = op_33;
                Z80op[0x34] = op_34;
                Z80op[0x35] = op_35;
                Z80op[0x36] = op_36;
                Z80op[0x37] = op_37;
                Z80op[0x38] = op_38;
                Z80op[0x39] = op_39;
                Z80op[0x3a] = op_3a;
                Z80op[0x3b] = op_3b;
                Z80op[0x3c] = op_3c;
                Z80op[0x3d] = op_3d;
                Z80op[0x3e] = op_3e;
                Z80op[0x3f] = op_3f;

                Z80op[0x40] = op_40;
                Z80op[0x41] = op_41;
                Z80op[0x42] = op_42;
                Z80op[0x43] = op_43;
                Z80op[0x44] = op_44;
                Z80op[0x45] = op_45;
                Z80op[0x46] = op_46;
                Z80op[0x47] = op_47;
                Z80op[0x48] = op_48;
                Z80op[0x49] = op_49;
                Z80op[0x4a] = op_4a;
                Z80op[0x4b] = op_4b;
                Z80op[0x4c] = op_4c;
                Z80op[0x4d] = op_4d;
                Z80op[0x4e] = op_4e;
                Z80op[0x4f] = op_4f;

                Z80op[0x50] = op_50;
                Z80op[0x51] = op_51;
                Z80op[0x52] = op_52;
                Z80op[0x53] = op_53;
                Z80op[0x54] = op_54;
                Z80op[0x55] = op_55;
                Z80op[0x56] = op_56;
                Z80op[0x57] = op_57;
                Z80op[0x58] = op_58;
                Z80op[0x59] = op_59;
                Z80op[0x5a] = op_5a;
                Z80op[0x5b] = op_5b;
                Z80op[0x5c] = op_5c;
                Z80op[0x5d] = op_5d;
                Z80op[0x5e] = op_5e;
                Z80op[0x5f] = op_5f;

                Z80op[0x60] = op_60;
                Z80op[0x61] = op_61;
                Z80op[0x62] = op_62;
                Z80op[0x63] = op_63;
                Z80op[0x64] = op_64;
                Z80op[0x65] = op_65;
                Z80op[0x66] = op_66;
                Z80op[0x67] = op_67;
                Z80op[0x68] = op_68;
                Z80op[0x69] = op_69;
                Z80op[0x6a] = op_6a;
                Z80op[0x6b] = op_6b;
                Z80op[0x6c] = op_6c;
                Z80op[0x6d] = op_6d;
                Z80op[0x6e] = op_6e;
                Z80op[0x6f] = op_6f;

                Z80op[0x70] = op_70;
                Z80op[0x71] = op_71;
                Z80op[0x72] = op_72;
                Z80op[0x73] = op_73;
                Z80op[0x74] = op_74;
                Z80op[0x75] = op_75;
                Z80op[0x76] = op_76;
                Z80op[0x77] = op_77;
                Z80op[0x78] = op_78;
                Z80op[0x79] = op_79;
                Z80op[0x7a] = op_7a;
                Z80op[0x7b] = op_7b;
                Z80op[0x7c] = op_7c;
                Z80op[0x7d] = op_7d;
                Z80op[0x7e] = op_7e;
                Z80op[0x7f] = op_7f;

                Z80op[0x80] = op_80;
                Z80op[0x81] = op_81;
                Z80op[0x82] = op_82;
                Z80op[0x83] = op_83;
                Z80op[0x84] = op_84;
                Z80op[0x85] = op_85;
                Z80op[0x86] = op_86;
                Z80op[0x87] = op_87;
                Z80op[0x88] = op_88;
                Z80op[0x89] = op_89;
                Z80op[0x8a] = op_8a;
                Z80op[0x8b] = op_8b;
                Z80op[0x8c] = op_8c;
                Z80op[0x8d] = op_8d;
                Z80op[0x8e] = op_8e;
                Z80op[0x8f] = op_8f;

                Z80op[0x90] = op_90;
                Z80op[0x91] = op_91;
                Z80op[0x92] = op_92;
                Z80op[0x93] = op_93;
                Z80op[0x94] = op_94;
                Z80op[0x95] = op_95;
                Z80op[0x96] = op_96;
                Z80op[0x97] = op_97;
                Z80op[0x98] = op_98;
                Z80op[0x99] = op_99;
                Z80op[0x9a] = op_9a;
                Z80op[0x9b] = op_9b;
                Z80op[0x9c] = op_9c;
                Z80op[0x9d] = op_9d;
                Z80op[0x9e] = op_9e;
                Z80op[0x9f] = op_9f;

                Z80op[0xa0] = op_a0;
                Z80op[0xa1] = op_a1;
                Z80op[0xa2] = op_a2;
                Z80op[0xa3] = op_a3;
                Z80op[0xa4] = op_a4;
                Z80op[0xa5] = op_a5;
                Z80op[0xa6] = op_a6;
                Z80op[0xa7] = op_a7;
                Z80op[0xa8] = op_a8;
                Z80op[0xa9] = op_a9;
                Z80op[0xaa] = op_aa;
                Z80op[0xab] = op_ab;
                Z80op[0xac] = op_ac;
                Z80op[0xad] = op_ad;
                Z80op[0xae] = op_ae;
                Z80op[0xaf] = op_af;

                Z80op[0xb0] = op_b0;
                Z80op[0xb1] = op_b1;
                Z80op[0xb2] = op_b2;
                Z80op[0xb3] = op_b3;
                Z80op[0xb4] = op_b4;
                Z80op[0xb5] = op_b5;
                Z80op[0xb6] = op_b6;
                Z80op[0xb7] = op_b7;
                Z80op[0xb8] = op_b8;
                Z80op[0xb9] = op_b9;
                Z80op[0xba] = op_ba;
                Z80op[0xbb] = op_bb;
                Z80op[0xbc] = op_bc;
                Z80op[0xbd] = op_bd;
                Z80op[0xbe] = op_be;
                Z80op[0xbf] = op_bf;

                Z80op[0xc0] = op_c0;
                Z80op[0xc1] = op_c1;
                Z80op[0xc2] = op_c2;
                Z80op[0xc3] = op_c3;
                Z80op[0xc4] = op_c4;
                Z80op[0xc5] = op_c5;
                Z80op[0xc6] = op_c6;
                Z80op[0xc7] = op_c7;
                Z80op[0xc8] = op_c8;
                Z80op[0xc9] = op_c9;
                Z80op[0xca] = op_ca;
                Z80op[0xcb] = op_cb;
                Z80op[0xcc] = op_cc;
                Z80op[0xcd] = op_cd;
                Z80op[0xce] = op_ce;
                Z80op[0xcf] = op_cf;

                Z80op[0xd0] = op_d0;
                Z80op[0xd1] = op_d1;
                Z80op[0xd2] = op_d2;
                Z80op[0xd3] = op_d3;
                Z80op[0xd4] = op_d4;
                Z80op[0xd5] = op_d5;
                Z80op[0xd6] = op_d6;
                Z80op[0xd7] = op_d7;
                Z80op[0xd8] = op_d8;
                Z80op[0xd9] = op_d9;
                Z80op[0xda] = op_da;
                Z80op[0xdb] = op_db;
                Z80op[0xdc] = op_dc;
                Z80op[0xdd] = op_dd;
                Z80op[0xde] = op_de;
                Z80op[0xdf] = op_df;

                Z80op[0xe0] = op_e0;
                Z80op[0xe1] = op_e1;
                Z80op[0xe2] = op_e2;
                Z80op[0xe3] = op_e3;
                Z80op[0xe4] = op_e4;
                Z80op[0xe5] = op_e5;
                Z80op[0xe6] = op_e6;
                Z80op[0xe7] = op_e7;
                Z80op[0xe8] = op_e8;
                Z80op[0xe9] = op_e9;
                Z80op[0xea] = op_ea;
                Z80op[0xeb] = op_eb;
                Z80op[0xec] = op_ec;
                Z80op[0xed] = op_ed;
                Z80op[0xee] = op_ee;
                Z80op[0xef] = op_ef;

                Z80op[0xf0] = op_f0;
                Z80op[0xf1] = op_f1;
                Z80op[0xf2] = op_f2;
                Z80op[0xf3] = op_f3;
                Z80op[0xf4] = op_f4;
                Z80op[0xf5] = op_f5;
                Z80op[0xf6] = op_f6;
                Z80op[0xf7] = op_f7;
                Z80op[0xf8] = op_f8;
                Z80op[0xf9] = op_f9;
                Z80op[0xfa] = op_fa;
                Z80op[0xfb] = op_fb;
                Z80op[0xfc] = op_fc;
                Z80op[0xfd] = op_fd;
                Z80op[0xfe] = op_fe;
                Z80op[0xff] = op_ff;

            }
            void Setup_cb()
            {
                Z80cb[0x00] = cb_00;
                Z80cb[0x01] = cb_01;
                Z80cb[0x02] = cb_02;
                Z80cb[0x03] = cb_03;
                Z80cb[0x04] = cb_04;
                Z80cb[0x05] = cb_05;
                Z80cb[0x06] = cb_06;
                Z80cb[0x07] = cb_07;
                Z80cb[0x08] = cb_08;
                Z80cb[0x09] = cb_09;
                Z80cb[0x0a] = cb_0a;
                Z80cb[0x0b] = cb_0b;
                Z80cb[0x0c] = cb_0c;
                Z80cb[0x0d] = cb_0d;
                Z80cb[0x0e] = cb_0e;
                Z80cb[0x0f] = cb_0f;

                Z80cb[0x10] = cb_10;
                Z80cb[0x11] = cb_11;
                Z80cb[0x12] = cb_12;
                Z80cb[0x13] = cb_13;
                Z80cb[0x14] = cb_14;
                Z80cb[0x15] = cb_15;
                Z80cb[0x16] = cb_16;
                Z80cb[0x17] = cb_17;
                Z80cb[0x18] = cb_18;
                Z80cb[0x19] = cb_19;
                Z80cb[0x1a] = cb_1a;
                Z80cb[0x1b] = cb_1b;
                Z80cb[0x1c] = cb_1c;
                Z80cb[0x1d] = cb_1d;
                Z80cb[0x1e] = cb_1e;
                Z80cb[0x1f] = cb_1f;

                Z80cb[0x20] = cb_20;
                Z80cb[0x21] = cb_21;
                Z80cb[0x22] = cb_22;
                Z80cb[0x23] = cb_23;
                Z80cb[0x24] = cb_24;
                Z80cb[0x25] = cb_25;
                Z80cb[0x26] = cb_26;
                Z80cb[0x27] = cb_27;
                Z80cb[0x28] = cb_28;
                Z80cb[0x29] = cb_29;
                Z80cb[0x2a] = cb_2a;
                Z80cb[0x2b] = cb_2b;
                Z80cb[0x2c] = cb_2c;
                Z80cb[0x2d] = cb_2d;
                Z80cb[0x2e] = cb_2e;
                Z80cb[0x2f] = cb_2f;

                Z80cb[0x30] = cb_30;
                Z80cb[0x31] = cb_31;
                Z80cb[0x32] = cb_32;
                Z80cb[0x33] = cb_33;
                Z80cb[0x34] = cb_34;
                Z80cb[0x35] = cb_35;
                Z80cb[0x36] = cb_36;
                Z80cb[0x37] = cb_37;
                Z80cb[0x38] = cb_38;
                Z80cb[0x39] = cb_39;
                Z80cb[0x3a] = cb_3a;
                Z80cb[0x3b] = cb_3b;
                Z80cb[0x3c] = cb_3c;
                Z80cb[0x3d] = cb_3d;
                Z80cb[0x3e] = cb_3e;
                Z80cb[0x3f] = cb_3f;

                Z80cb[0x40] = cb_40;
                Z80cb[0x41] = cb_41;
                Z80cb[0x42] = cb_42;
                Z80cb[0x43] = cb_43;
                Z80cb[0x44] = cb_44;
                Z80cb[0x45] = cb_45;
                Z80cb[0x46] = cb_46;
                Z80cb[0x47] = cb_47;
                Z80cb[0x48] = cb_48;
                Z80cb[0x49] = cb_49;
                Z80cb[0x4a] = cb_4a;
                Z80cb[0x4b] = cb_4b;
                Z80cb[0x4c] = cb_4c;
                Z80cb[0x4d] = cb_4d;
                Z80cb[0x4e] = cb_4e;
                Z80cb[0x4f] = cb_4f;

                Z80cb[0x50] = cb_50;
                Z80cb[0x51] = cb_51;
                Z80cb[0x52] = cb_52;
                Z80cb[0x53] = cb_53;
                Z80cb[0x54] = cb_54;
                Z80cb[0x55] = cb_55;
                Z80cb[0x56] = cb_56;
                Z80cb[0x57] = cb_57;
                Z80cb[0x58] = cb_58;
                Z80cb[0x59] = cb_59;
                Z80cb[0x5a] = cb_5a;
                Z80cb[0x5b] = cb_5b;
                Z80cb[0x5c] = cb_5c;
                Z80cb[0x5d] = cb_5d;
                Z80cb[0x5e] = cb_5e;
                Z80cb[0x5f] = cb_5f;

                Z80cb[0x60] = cb_60;
                Z80cb[0x61] = cb_61;
                Z80cb[0x62] = cb_62;
                Z80cb[0x63] = cb_63;
                Z80cb[0x64] = cb_64;
                Z80cb[0x65] = cb_65;
                Z80cb[0x66] = cb_66;
                Z80cb[0x67] = cb_67;
                Z80cb[0x68] = cb_68;
                Z80cb[0x69] = cb_69;
                Z80cb[0x6a] = cb_6a;
                Z80cb[0x6b] = cb_6b;
                Z80cb[0x6c] = cb_6c;
                Z80cb[0x6d] = cb_6d;
                Z80cb[0x6e] = cb_6e;
                Z80cb[0x6f] = cb_6f;

                Z80cb[0x70] = cb_70;
                Z80cb[0x71] = cb_71;
                Z80cb[0x72] = cb_72;
                Z80cb[0x73] = cb_73;
                Z80cb[0x74] = cb_74;
                Z80cb[0x75] = cb_75;
                Z80cb[0x76] = cb_76;
                Z80cb[0x77] = cb_77;
                Z80cb[0x78] = cb_78;
                Z80cb[0x79] = cb_79;
                Z80cb[0x7a] = cb_7a;
                Z80cb[0x7b] = cb_7b;
                Z80cb[0x7c] = cb_7c;
                Z80cb[0x7d] = cb_7d;
                Z80cb[0x7e] = cb_7e;
                Z80cb[0x7f] = cb_7f;

                Z80cb[0x80] = cb_80;
                Z80cb[0x81] = cb_81;
                Z80cb[0x82] = cb_82;
                Z80cb[0x83] = cb_83;
                Z80cb[0x84] = cb_84;
                Z80cb[0x85] = cb_85;
                Z80cb[0x86] = cb_86;
                Z80cb[0x87] = cb_87;
                Z80cb[0x88] = cb_88;
                Z80cb[0x89] = cb_89;
                Z80cb[0x8a] = cb_8a;
                Z80cb[0x8b] = cb_8b;
                Z80cb[0x8c] = cb_8c;
                Z80cb[0x8d] = cb_8d;
                Z80cb[0x8e] = cb_8e;
                Z80cb[0x8f] = cb_8f;

                Z80cb[0x90] = cb_90;
                Z80cb[0x91] = cb_91;
                Z80cb[0x92] = cb_92;
                Z80cb[0x93] = cb_93;
                Z80cb[0x94] = cb_94;
                Z80cb[0x95] = cb_95;
                Z80cb[0x96] = cb_96;
                Z80cb[0x97] = cb_97;
                Z80cb[0x98] = cb_98;
                Z80cb[0x99] = cb_99;
                Z80cb[0x9a] = cb_9a;
                Z80cb[0x9b] = cb_9b;
                Z80cb[0x9c] = cb_9c;
                Z80cb[0x9d] = cb_9d;
                Z80cb[0x9e] = cb_9e;
                Z80cb[0x9f] = cb_9f;

                Z80cb[0xa0] = cb_a0;
                Z80cb[0xa1] = cb_a1;
                Z80cb[0xa2] = cb_a2;
                Z80cb[0xa3] = cb_a3;
                Z80cb[0xa4] = cb_a4;
                Z80cb[0xa5] = cb_a5;
                Z80cb[0xa6] = cb_a6;
                Z80cb[0xa7] = cb_a7;
                Z80cb[0xa8] = cb_a8;
                Z80cb[0xa9] = cb_a9;
                Z80cb[0xaa] = cb_aa;
                Z80cb[0xab] = cb_ab;
                Z80cb[0xac] = cb_ac;
                Z80cb[0xad] = cb_ad;
                Z80cb[0xae] = cb_ae;
                Z80cb[0xaf] = cb_af;

                Z80cb[0xb0] = cb_b0;
                Z80cb[0xb1] = cb_b1;
                Z80cb[0xb2] = cb_b2;
                Z80cb[0xb3] = cb_b3;
                Z80cb[0xb4] = cb_b4;
                Z80cb[0xb5] = cb_b5;
                Z80cb[0xb6] = cb_b6;
                Z80cb[0xb7] = cb_b7;
                Z80cb[0xb8] = cb_b8;
                Z80cb[0xb9] = cb_b9;
                Z80cb[0xba] = cb_ba;
                Z80cb[0xbb] = cb_bb;
                Z80cb[0xbc] = cb_bc;
                Z80cb[0xbd] = cb_bd;
                Z80cb[0xbe] = cb_be;
                Z80cb[0xbf] = cb_bf;

                Z80cb[0xc0] = cb_c0;
                Z80cb[0xc1] = cb_c1;
                Z80cb[0xc2] = cb_c2;
                Z80cb[0xc3] = cb_c3;
                Z80cb[0xc4] = cb_c4;
                Z80cb[0xc5] = cb_c5;
                Z80cb[0xc6] = cb_c6;
                Z80cb[0xc7] = cb_c7;
                Z80cb[0xc8] = cb_c8;
                Z80cb[0xc9] = cb_c9;
                Z80cb[0xca] = cb_ca;
                Z80cb[0xcb] = cb_cb;
                Z80cb[0xcc] = cb_cc;
                Z80cb[0xcd] = cb_cd;
                Z80cb[0xce] = cb_ce;
                Z80cb[0xcf] = cb_cf;

                Z80cb[0xd0] = cb_d0;
                Z80cb[0xd1] = cb_d1;
                Z80cb[0xd2] = cb_d2;
                Z80cb[0xd3] = cb_d3;
                Z80cb[0xd4] = cb_d4;
                Z80cb[0xd5] = cb_d5;
                Z80cb[0xd6] = cb_d6;
                Z80cb[0xd7] = cb_d7;
                Z80cb[0xd8] = cb_d8;
                Z80cb[0xd9] = cb_d9;
                Z80cb[0xda] = cb_da;
                Z80cb[0xdb] = cb_db;
                Z80cb[0xdc] = cb_dc;
                Z80cb[0xdd] = cb_dd;
                Z80cb[0xde] = cb_de;
                Z80cb[0xdf] = cb_df;

                Z80cb[0xe0] = cb_e0;
                Z80cb[0xe1] = cb_e1;
                Z80cb[0xe2] = cb_e2;
                Z80cb[0xe3] = cb_e3;
                Z80cb[0xe4] = cb_e4;
                Z80cb[0xe5] = cb_e5;
                Z80cb[0xe6] = cb_e6;
                Z80cb[0xe7] = cb_e7;
                Z80cb[0xe8] = cb_e8;
                Z80cb[0xe9] = cb_e9;
                Z80cb[0xea] = cb_ea;
                Z80cb[0xeb] = cb_eb;
                Z80cb[0xec] = cb_ec;
                Z80cb[0xed] = cb_ed;
                Z80cb[0xee] = cb_ee;
                Z80cb[0xef] = cb_ef;

                Z80cb[0xf0] = cb_f0;
                Z80cb[0xf1] = cb_f1;
                Z80cb[0xf2] = cb_f2;
                Z80cb[0xf3] = cb_f3;
                Z80cb[0xf4] = cb_f4;
                Z80cb[0xf5] = cb_f5;
                Z80cb[0xf6] = cb_f6;
                Z80cb[0xf7] = cb_f7;
                Z80cb[0xf8] = cb_f8;
                Z80cb[0xf9] = cb_f9;
                Z80cb[0xfa] = cb_fa;
                Z80cb[0xfb] = cb_fb;
                Z80cb[0xfc] = cb_fc;
                Z80cb[0xfd] = cb_fd;
                Z80cb[0xfe] = cb_fe;
                Z80cb[0xff] = cb_ff;

            }
            void Setup_ed()
            {
                Z80ed[0x00] = ed_00;
                Z80ed[0x01] = ed_01;
                Z80ed[0x02] = ed_02;
                Z80ed[0x03] = ed_03;
                Z80ed[0x04] = ed_04;
                Z80ed[0x05] = ed_05;
                Z80ed[0x06] = ed_06;
                Z80ed[0x07] = ed_07;
                Z80ed[0x08] = ed_08;
                Z80ed[0x09] = ed_09;
                Z80ed[0x0a] = ed_0a;
                Z80ed[0x0b] = ed_0b;
                Z80ed[0x0c] = ed_0c;
                Z80ed[0x0d] = ed_0d;
                Z80ed[0x0e] = ed_0e;
                Z80ed[0x0f] = ed_0f;

                Z80ed[0x10] = ed_10;
                Z80ed[0x11] = ed_11;
                Z80ed[0x12] = ed_12;
                Z80ed[0x13] = ed_13;
                Z80ed[0x14] = ed_14;
                Z80ed[0x15] = ed_15;
                Z80ed[0x16] = ed_16;
                Z80ed[0x17] = ed_17;
                Z80ed[0x18] = ed_18;
                Z80ed[0x19] = ed_19;
                Z80ed[0x1a] = ed_1a;
                Z80ed[0x1b] = ed_1b;
                Z80ed[0x1c] = ed_1c;
                Z80ed[0x1d] = ed_1d;
                Z80ed[0x1e] = ed_1e;
                Z80ed[0x1f] = ed_1f;

                Z80ed[0x20] = ed_20;
                Z80ed[0x21] = ed_21;
                Z80ed[0x22] = ed_22;
                Z80ed[0x23] = ed_23;
                Z80ed[0x24] = ed_24;
                Z80ed[0x25] = ed_25;
                Z80ed[0x26] = ed_26;
                Z80ed[0x27] = ed_27;
                Z80ed[0x28] = ed_28;
                Z80ed[0x29] = ed_29;
                Z80ed[0x2a] = ed_2a;
                Z80ed[0x2b] = ed_2b;
                Z80ed[0x2c] = ed_2c;
                Z80ed[0x2d] = ed_2d;
                Z80ed[0x2e] = ed_2e;
                Z80ed[0x2f] = ed_2f;

                Z80ed[0x30] = ed_30;
                Z80ed[0x31] = ed_31;
                Z80ed[0x32] = ed_32;
                Z80ed[0x33] = ed_33;
                Z80ed[0x34] = ed_34;
                Z80ed[0x35] = ed_35;
                Z80ed[0x36] = ed_36;
                Z80ed[0x37] = ed_37;
                Z80ed[0x38] = ed_38;
                Z80ed[0x39] = ed_39;
                Z80ed[0x3a] = ed_3a;
                Z80ed[0x3b] = ed_3b;
                Z80ed[0x3c] = ed_3c;
                Z80ed[0x3d] = ed_3d;
                Z80ed[0x3e] = ed_3e;
                Z80ed[0x3f] = ed_3f;

                Z80ed[0x40] = ed_40;
                Z80ed[0x41] = ed_41;
                Z80ed[0x42] = ed_42;
                Z80ed[0x43] = ed_43;
                Z80ed[0x44] = ed_44;
                Z80ed[0x45] = ed_45;
                Z80ed[0x46] = ed_46;
                Z80ed[0x47] = ed_47;
                Z80ed[0x48] = ed_48;
                Z80ed[0x49] = ed_49;
                Z80ed[0x4a] = ed_4a;
                Z80ed[0x4b] = ed_4b;
                Z80ed[0x4c] = ed_4c;
                Z80ed[0x4d] = ed_4d;
                Z80ed[0x4e] = ed_4e;
                Z80ed[0x4f] = ed_4f;

                Z80ed[0x50] = ed_50;
                Z80ed[0x51] = ed_51;
                Z80ed[0x52] = ed_52;
                Z80ed[0x53] = ed_53;
                Z80ed[0x54] = ed_54;
                Z80ed[0x55] = ed_55;
                Z80ed[0x56] = ed_56;
                Z80ed[0x57] = ed_57;
                Z80ed[0x58] = ed_58;
                Z80ed[0x59] = ed_59;
                Z80ed[0x5a] = ed_5a;
                Z80ed[0x5b] = ed_5b;
                Z80ed[0x5c] = ed_5c;
                Z80ed[0x5d] = ed_5d;
                Z80ed[0x5e] = ed_5e;
                Z80ed[0x5f] = ed_5f;

                Z80ed[0x60] = ed_60;
                Z80ed[0x61] = ed_61;
                Z80ed[0x62] = ed_62;
                Z80ed[0x63] = ed_63;
                Z80ed[0x64] = ed_64;
                Z80ed[0x65] = ed_65;
                Z80ed[0x66] = ed_66;
                Z80ed[0x67] = ed_67;
                Z80ed[0x68] = ed_68;
                Z80ed[0x69] = ed_69;
                Z80ed[0x6a] = ed_6a;
                Z80ed[0x6b] = ed_6b;
                Z80ed[0x6c] = ed_6c;
                Z80ed[0x6d] = ed_6d;
                Z80ed[0x6e] = ed_6e;
                Z80ed[0x6f] = ed_6f;

                Z80ed[0x70] = ed_70;
                Z80ed[0x71] = ed_71;
                Z80ed[0x72] = ed_72;
                Z80ed[0x73] = ed_73;
                Z80ed[0x74] = ed_74;
                Z80ed[0x75] = ed_75;
                Z80ed[0x76] = ed_76;
                Z80ed[0x77] = ed_77;
                Z80ed[0x78] = ed_78;
                Z80ed[0x79] = ed_79;
                Z80ed[0x7a] = ed_7a;
                Z80ed[0x7b] = ed_7b;
                Z80ed[0x7c] = ed_7c;
                Z80ed[0x7d] = ed_7d;
                Z80ed[0x7e] = ed_7e;
                Z80ed[0x7f] = ed_7f;

                Z80ed[0x80] = ed_80;
                Z80ed[0x81] = ed_81;
                Z80ed[0x82] = ed_82;
                Z80ed[0x83] = ed_83;
                Z80ed[0x84] = ed_84;
                Z80ed[0x85] = ed_85;
                Z80ed[0x86] = ed_86;
                Z80ed[0x87] = ed_87;
                Z80ed[0x88] = ed_88;
                Z80ed[0x89] = ed_89;
                Z80ed[0x8a] = ed_8a;
                Z80ed[0x8b] = ed_8b;
                Z80ed[0x8c] = ed_8c;
                Z80ed[0x8d] = ed_8d;
                Z80ed[0x8e] = ed_8e;
                Z80ed[0x8f] = ed_8f;

                Z80ed[0x90] = ed_90;
                Z80ed[0x91] = ed_91;
                Z80ed[0x92] = ed_92;
                Z80ed[0x93] = ed_93;
                Z80ed[0x94] = ed_94;
                Z80ed[0x95] = ed_95;
                Z80ed[0x96] = ed_96;
                Z80ed[0x97] = ed_97;
                Z80ed[0x98] = ed_98;
                Z80ed[0x99] = ed_99;
                Z80ed[0x9a] = ed_9a;
                Z80ed[0x9b] = ed_9b;
                Z80ed[0x9c] = ed_9c;
                Z80ed[0x9d] = ed_9d;
                Z80ed[0x9e] = ed_9e;
                Z80ed[0x9f] = ed_9f;

                Z80ed[0xa0] = ed_a0;
                Z80ed[0xa1] = ed_a1;
                Z80ed[0xa2] = ed_a2;
                Z80ed[0xa3] = ed_a3;
                Z80ed[0xa4] = ed_a4;
                Z80ed[0xa5] = ed_a5;
                Z80ed[0xa6] = ed_a6;
                Z80ed[0xa7] = ed_a7;
                Z80ed[0xa8] = ed_a8;
                Z80ed[0xa9] = ed_a9;
                Z80ed[0xaa] = ed_aa;
                Z80ed[0xab] = ed_ab;
                Z80ed[0xac] = ed_ac;
                Z80ed[0xad] = ed_ad;
                Z80ed[0xae] = ed_ae;
                Z80ed[0xaf] = ed_af;

                Z80ed[0xb0] = ed_b0;
                Z80ed[0xb1] = ed_b1;
                Z80ed[0xb2] = ed_b2;
                Z80ed[0xb3] = ed_b3;
                Z80ed[0xb4] = ed_b4;
                Z80ed[0xb5] = ed_b5;
                Z80ed[0xb6] = ed_b6;
                Z80ed[0xb7] = ed_b7;
                Z80ed[0xb8] = ed_b8;
                Z80ed[0xb9] = ed_b9;
                Z80ed[0xba] = ed_ba;
                Z80ed[0xbb] = ed_bb;
                Z80ed[0xbc] = ed_bc;
                Z80ed[0xbd] = ed_bd;
                Z80ed[0xbe] = ed_be;
                Z80ed[0xbf] = ed_bf;

                Z80ed[0xc0] = ed_c0;
                Z80ed[0xc1] = ed_c1;
                Z80ed[0xc2] = ed_c2;
                Z80ed[0xc3] = ed_c3;
                Z80ed[0xc4] = ed_c4;
                Z80ed[0xc5] = ed_c5;
                Z80ed[0xc6] = ed_c6;
                Z80ed[0xc7] = ed_c7;
                Z80ed[0xc8] = ed_c8;
                Z80ed[0xc9] = ed_c9;
                Z80ed[0xca] = ed_ca;
                Z80ed[0xcb] = ed_cb;
                Z80ed[0xcc] = ed_cc;
                Z80ed[0xcd] = ed_cd;
                Z80ed[0xce] = ed_ce;
                Z80ed[0xcf] = ed_cf;

                Z80ed[0xd0] = ed_d0;
                Z80ed[0xd1] = ed_d1;
                Z80ed[0xd2] = ed_d2;
                Z80ed[0xd3] = ed_d3;
                Z80ed[0xd4] = ed_d4;
                Z80ed[0xd5] = ed_d5;
                Z80ed[0xd6] = ed_d6;
                Z80ed[0xd7] = ed_d7;
                Z80ed[0xd8] = ed_d8;
                Z80ed[0xd9] = ed_d9;
                Z80ed[0xda] = ed_da;
                Z80ed[0xdb] = ed_db;
                Z80ed[0xdc] = ed_dc;
                Z80ed[0xdd] = ed_dd;
                Z80ed[0xde] = ed_de;
                Z80ed[0xdf] = ed_df;

                Z80ed[0xe0] = ed_e0;
                Z80ed[0xe1] = ed_e1;
                Z80ed[0xe2] = ed_e2;
                Z80ed[0xe3] = ed_e3;
                Z80ed[0xe4] = ed_e4;
                Z80ed[0xe5] = ed_e5;
                Z80ed[0xe6] = ed_e6;
                Z80ed[0xe7] = ed_e7;
                Z80ed[0xe8] = ed_e8;
                Z80ed[0xe9] = ed_e9;
                Z80ed[0xea] = ed_ea;
                Z80ed[0xeb] = ed_eb;
                Z80ed[0xec] = ed_ec;
                Z80ed[0xed] = ed_ed;
                Z80ed[0xee] = ed_ee;
                Z80ed[0xef] = ed_ef;

                Z80ed[0xf0] = ed_f0;
                Z80ed[0xf1] = ed_f1;
                Z80ed[0xf2] = ed_f2;
                Z80ed[0xf3] = ed_f3;
                Z80ed[0xf4] = ed_f4;
                Z80ed[0xf5] = ed_f5;
                Z80ed[0xf6] = ed_f6;
                Z80ed[0xf7] = ed_f7;
                Z80ed[0xf8] = ed_f8;
                Z80ed[0xf9] = ed_f9;
                Z80ed[0xfa] = ed_fa;
                Z80ed[0xfb] = ed_fb;
                Z80ed[0xfc] = ed_fc;
                Z80ed[0xfd] = ed_fd;
                Z80ed[0xfe] = ed_fe;
                Z80ed[0xff] = ed_ff;

            }
            void Setup_dd()
            {
                Z80dd[0x00] = dd_00;
                Z80dd[0x01] = dd_01;
                Z80dd[0x02] = dd_02;
                Z80dd[0x03] = dd_03;
                Z80dd[0x04] = dd_04;
                Z80dd[0x05] = dd_05;
                Z80dd[0x06] = dd_06;
                Z80dd[0x07] = dd_07;
                Z80dd[0x08] = dd_08;
                Z80dd[0x09] = dd_09;
                Z80dd[0x0a] = dd_0a;
                Z80dd[0x0b] = dd_0b;
                Z80dd[0x0c] = dd_0c;
                Z80dd[0x0d] = dd_0d;
                Z80dd[0x0e] = dd_0e;
                Z80dd[0x0f] = dd_0f;

                Z80dd[0x10] = dd_10;
                Z80dd[0x11] = dd_11;
                Z80dd[0x12] = dd_12;
                Z80dd[0x13] = dd_13;
                Z80dd[0x14] = dd_14;
                Z80dd[0x15] = dd_15;
                Z80dd[0x16] = dd_16;
                Z80dd[0x17] = dd_17;
                Z80dd[0x18] = dd_18;
                Z80dd[0x19] = dd_19;
                Z80dd[0x1a] = dd_1a;
                Z80dd[0x1b] = dd_1b;
                Z80dd[0x1c] = dd_1c;
                Z80dd[0x1d] = dd_1d;
                Z80dd[0x1e] = dd_1e;
                Z80dd[0x1f] = dd_1f;

                Z80dd[0x20] = dd_20;
                Z80dd[0x21] = dd_21;
                Z80dd[0x22] = dd_22;
                Z80dd[0x23] = dd_23;
                Z80dd[0x24] = dd_24;
                Z80dd[0x25] = dd_25;
                Z80dd[0x26] = dd_26;
                Z80dd[0x27] = dd_27;
                Z80dd[0x28] = dd_28;
                Z80dd[0x29] = dd_29;
                Z80dd[0x2a] = dd_2a;
                Z80dd[0x2b] = dd_2b;
                Z80dd[0x2c] = dd_2c;
                Z80dd[0x2d] = dd_2d;
                Z80dd[0x2e] = dd_2e;
                Z80dd[0x2f] = dd_2f;

                Z80dd[0x30] = dd_30;
                Z80dd[0x31] = dd_31;
                Z80dd[0x32] = dd_32;
                Z80dd[0x33] = dd_33;
                Z80dd[0x34] = dd_34;
                Z80dd[0x35] = dd_35;
                Z80dd[0x36] = dd_36;
                Z80dd[0x37] = dd_37;
                Z80dd[0x38] = dd_38;
                Z80dd[0x39] = dd_39;
                Z80dd[0x3a] = dd_3a;
                Z80dd[0x3b] = dd_3b;
                Z80dd[0x3c] = dd_3c;
                Z80dd[0x3d] = dd_3d;
                Z80dd[0x3e] = dd_3e;
                Z80dd[0x3f] = dd_3f;

                Z80dd[0x40] = dd_40;
                Z80dd[0x41] = dd_41;
                Z80dd[0x42] = dd_42;
                Z80dd[0x43] = dd_43;
                Z80dd[0x44] = dd_44;
                Z80dd[0x45] = dd_45;
                Z80dd[0x46] = dd_46;
                Z80dd[0x47] = dd_47;
                Z80dd[0x48] = dd_48;
                Z80dd[0x49] = dd_49;
                Z80dd[0x4a] = dd_4a;
                Z80dd[0x4b] = dd_4b;
                Z80dd[0x4c] = dd_4c;
                Z80dd[0x4d] = dd_4d;
                Z80dd[0x4e] = dd_4e;
                Z80dd[0x4f] = dd_4f;

                Z80dd[0x50] = dd_50;
                Z80dd[0x51] = dd_51;
                Z80dd[0x52] = dd_52;
                Z80dd[0x53] = dd_53;
                Z80dd[0x54] = dd_54;
                Z80dd[0x55] = dd_55;
                Z80dd[0x56] = dd_56;
                Z80dd[0x57] = dd_57;
                Z80dd[0x58] = dd_58;
                Z80dd[0x59] = dd_59;
                Z80dd[0x5a] = dd_5a;
                Z80dd[0x5b] = dd_5b;
                Z80dd[0x5c] = dd_5c;
                Z80dd[0x5d] = dd_5d;
                Z80dd[0x5e] = dd_5e;
                Z80dd[0x5f] = dd_5f;

                Z80dd[0x60] = dd_60;
                Z80dd[0x61] = dd_61;
                Z80dd[0x62] = dd_62;
                Z80dd[0x63] = dd_63;
                Z80dd[0x64] = dd_64;
                Z80dd[0x65] = dd_65;
                Z80dd[0x66] = dd_66;
                Z80dd[0x67] = dd_67;
                Z80dd[0x68] = dd_68;
                Z80dd[0x69] = dd_69;
                Z80dd[0x6a] = dd_6a;
                Z80dd[0x6b] = dd_6b;
                Z80dd[0x6c] = dd_6c;
                Z80dd[0x6d] = dd_6d;
                Z80dd[0x6e] = dd_6e;
                Z80dd[0x6f] = dd_6f;

                Z80dd[0x70] = dd_70;
                Z80dd[0x71] = dd_71;
                Z80dd[0x72] = dd_72;
                Z80dd[0x73] = dd_73;
                Z80dd[0x74] = dd_74;
                Z80dd[0x75] = dd_75;
                Z80dd[0x76] = dd_76;
                Z80dd[0x77] = dd_77;
                Z80dd[0x78] = dd_78;
                Z80dd[0x79] = dd_79;
                Z80dd[0x7a] = dd_7a;
                Z80dd[0x7b] = dd_7b;
                Z80dd[0x7c] = dd_7c;
                Z80dd[0x7d] = dd_7d;
                Z80dd[0x7e] = dd_7e;
                Z80dd[0x7f] = dd_7f;

                Z80dd[0x80] = dd_80;
                Z80dd[0x81] = dd_81;
                Z80dd[0x82] = dd_82;
                Z80dd[0x83] = dd_83;
                Z80dd[0x84] = dd_84;
                Z80dd[0x85] = dd_85;
                Z80dd[0x86] = dd_86;
                Z80dd[0x87] = dd_87;
                Z80dd[0x88] = dd_88;
                Z80dd[0x89] = dd_89;
                Z80dd[0x8a] = dd_8a;
                Z80dd[0x8b] = dd_8b;
                Z80dd[0x8c] = dd_8c;
                Z80dd[0x8d] = dd_8d;
                Z80dd[0x8e] = dd_8e;
                Z80dd[0x8f] = dd_8f;

                Z80dd[0x90] = dd_90;
                Z80dd[0x91] = dd_91;
                Z80dd[0x92] = dd_92;
                Z80dd[0x93] = dd_93;
                Z80dd[0x94] = dd_94;
                Z80dd[0x95] = dd_95;
                Z80dd[0x96] = dd_96;
                Z80dd[0x97] = dd_97;
                Z80dd[0x98] = dd_98;
                Z80dd[0x99] = dd_99;
                Z80dd[0x9a] = dd_9a;
                Z80dd[0x9b] = dd_9b;
                Z80dd[0x9c] = dd_9c;
                Z80dd[0x9d] = dd_9d;
                Z80dd[0x9e] = dd_9e;
                Z80dd[0x9f] = dd_9f;

                Z80dd[0xa0] = dd_a0;
                Z80dd[0xa1] = dd_a1;
                Z80dd[0xa2] = dd_a2;
                Z80dd[0xa3] = dd_a3;
                Z80dd[0xa4] = dd_a4;
                Z80dd[0xa5] = dd_a5;
                Z80dd[0xa6] = dd_a6;
                Z80dd[0xa7] = dd_a7;
                Z80dd[0xa8] = dd_a8;
                Z80dd[0xa9] = dd_a9;
                Z80dd[0xaa] = dd_aa;
                Z80dd[0xab] = dd_ab;
                Z80dd[0xac] = dd_ac;
                Z80dd[0xad] = dd_ad;
                Z80dd[0xae] = dd_ae;
                Z80dd[0xaf] = dd_af;

                Z80dd[0xb0] = dd_b0;
                Z80dd[0xb1] = dd_b1;
                Z80dd[0xb2] = dd_b2;
                Z80dd[0xb3] = dd_b3;
                Z80dd[0xb4] = dd_b4;
                Z80dd[0xb5] = dd_b5;
                Z80dd[0xb6] = dd_b6;
                Z80dd[0xb7] = dd_b7;
                Z80dd[0xb8] = dd_b8;
                Z80dd[0xb9] = dd_b9;
                Z80dd[0xba] = dd_ba;
                Z80dd[0xbb] = dd_bb;
                Z80dd[0xbc] = dd_bc;
                Z80dd[0xbd] = dd_bd;
                Z80dd[0xbe] = dd_be;
                Z80dd[0xbf] = dd_bf;

                Z80dd[0xc0] = dd_c0;
                Z80dd[0xc1] = dd_c1;
                Z80dd[0xc2] = dd_c2;
                Z80dd[0xc3] = dd_c3;
                Z80dd[0xc4] = dd_c4;
                Z80dd[0xc5] = dd_c5;
                Z80dd[0xc6] = dd_c6;
                Z80dd[0xc7] = dd_c7;
                Z80dd[0xc8] = dd_c8;
                Z80dd[0xc9] = dd_c9;
                Z80dd[0xca] = dd_ca;
                Z80dd[0xcb] = dd_cb;
                Z80dd[0xcc] = dd_cc;
                Z80dd[0xcd] = dd_cd;
                Z80dd[0xce] = dd_ce;
                Z80dd[0xcf] = dd_cf;

                Z80dd[0xd0] = dd_d0;
                Z80dd[0xd1] = dd_d1;
                Z80dd[0xd2] = dd_d2;
                Z80dd[0xd3] = dd_d3;
                Z80dd[0xd4] = dd_d4;
                Z80dd[0xd5] = dd_d5;
                Z80dd[0xd6] = dd_d6;
                Z80dd[0xd7] = dd_d7;
                Z80dd[0xd8] = dd_d8;
                Z80dd[0xd9] = dd_d9;
                Z80dd[0xda] = dd_da;
                Z80dd[0xdb] = dd_db;
                Z80dd[0xdc] = dd_dc;
                Z80dd[0xdd] = dd_dd;
                Z80dd[0xde] = dd_de;
                Z80dd[0xdf] = dd_df;

                Z80dd[0xe0] = dd_e0;
                Z80dd[0xe1] = dd_e1;
                Z80dd[0xe2] = dd_e2;
                Z80dd[0xe3] = dd_e3;
                Z80dd[0xe4] = dd_e4;
                Z80dd[0xe5] = dd_e5;
                Z80dd[0xe6] = dd_e6;
                Z80dd[0xe7] = dd_e7;
                Z80dd[0xe8] = dd_e8;
                Z80dd[0xe9] = dd_e9;
                Z80dd[0xea] = dd_ea;
                Z80dd[0xeb] = dd_eb;
                Z80dd[0xec] = dd_ec;
                Z80dd[0xed] = dd_ed;
                Z80dd[0xee] = dd_ee;
                Z80dd[0xef] = dd_ef;

                Z80dd[0xf0] = dd_f0;
                Z80dd[0xf1] = dd_f1;
                Z80dd[0xf2] = dd_f2;
                Z80dd[0xf3] = dd_f3;
                Z80dd[0xf4] = dd_f4;
                Z80dd[0xf5] = dd_f5;
                Z80dd[0xf6] = dd_f6;
                Z80dd[0xf7] = dd_f7;
                Z80dd[0xf8] = dd_f8;
                Z80dd[0xf9] = dd_f9;
                Z80dd[0xfa] = dd_fa;
                Z80dd[0xfb] = dd_fb;
                Z80dd[0xfc] = dd_fc;
                Z80dd[0xfd] = dd_fd;
                Z80dd[0xfe] = dd_fe;
                Z80dd[0xff] = dd_ff;

            }
            void Setup_fd()
            {
                Z80fd[0x00] = fd_00;
                Z80fd[0x01] = fd_01;
                Z80fd[0x02] = fd_02;
                Z80fd[0x03] = fd_03;
                Z80fd[0x04] = fd_04;
                Z80fd[0x05] = fd_05;
                Z80fd[0x06] = fd_06;
                Z80fd[0x07] = fd_07;
                Z80fd[0x08] = fd_08;
                Z80fd[0x09] = fd_09;
                Z80fd[0x0a] = fd_0a;
                Z80fd[0x0b] = fd_0b;
                Z80fd[0x0c] = fd_0c;
                Z80fd[0x0d] = fd_0d;
                Z80fd[0x0e] = fd_0e;
                Z80fd[0x0f] = fd_0f;

                Z80fd[0x10] = fd_10;
                Z80fd[0x11] = fd_11;
                Z80fd[0x12] = fd_12;
                Z80fd[0x13] = fd_13;
                Z80fd[0x14] = fd_14;
                Z80fd[0x15] = fd_15;
                Z80fd[0x16] = fd_16;
                Z80fd[0x17] = fd_17;
                Z80fd[0x18] = fd_18;
                Z80fd[0x19] = fd_19;
                Z80fd[0x1a] = fd_1a;
                Z80fd[0x1b] = fd_1b;
                Z80fd[0x1c] = fd_1c;
                Z80fd[0x1d] = fd_1d;
                Z80fd[0x1e] = fd_1e;
                Z80fd[0x1f] = fd_1f;

                Z80fd[0x20] = fd_20;
                Z80fd[0x21] = fd_21;
                Z80fd[0x22] = fd_22;
                Z80fd[0x23] = fd_23;
                Z80fd[0x24] = fd_24;
                Z80fd[0x25] = fd_25;
                Z80fd[0x26] = fd_26;
                Z80fd[0x27] = fd_27;
                Z80fd[0x28] = fd_28;
                Z80fd[0x29] = fd_29;
                Z80fd[0x2a] = fd_2a;
                Z80fd[0x2b] = fd_2b;
                Z80fd[0x2c] = fd_2c;
                Z80fd[0x2d] = fd_2d;
                Z80fd[0x2e] = fd_2e;
                Z80fd[0x2f] = fd_2f;

                Z80fd[0x30] = fd_30;
                Z80fd[0x31] = fd_31;
                Z80fd[0x32] = fd_32;
                Z80fd[0x33] = fd_33;
                Z80fd[0x34] = fd_34;
                Z80fd[0x35] = fd_35;
                Z80fd[0x36] = fd_36;
                Z80fd[0x37] = fd_37;
                Z80fd[0x38] = fd_38;
                Z80fd[0x39] = fd_39;
                Z80fd[0x3a] = fd_3a;
                Z80fd[0x3b] = fd_3b;
                Z80fd[0x3c] = fd_3c;
                Z80fd[0x3d] = fd_3d;
                Z80fd[0x3e] = fd_3e;
                Z80fd[0x3f] = fd_3f;

                Z80fd[0x40] = fd_40;
                Z80fd[0x41] = fd_41;
                Z80fd[0x42] = fd_42;
                Z80fd[0x43] = fd_43;
                Z80fd[0x44] = fd_44;
                Z80fd[0x45] = fd_45;
                Z80fd[0x46] = fd_46;
                Z80fd[0x47] = fd_47;
                Z80fd[0x48] = fd_48;
                Z80fd[0x49] = fd_49;
                Z80fd[0x4a] = fd_4a;
                Z80fd[0x4b] = fd_4b;
                Z80fd[0x4c] = fd_4c;
                Z80fd[0x4d] = fd_4d;
                Z80fd[0x4e] = fd_4e;
                Z80fd[0x4f] = fd_4f;

                Z80fd[0x50] = fd_50;
                Z80fd[0x51] = fd_51;
                Z80fd[0x52] = fd_52;
                Z80fd[0x53] = fd_53;
                Z80fd[0x54] = fd_54;
                Z80fd[0x55] = fd_55;
                Z80fd[0x56] = fd_56;
                Z80fd[0x57] = fd_57;
                Z80fd[0x58] = fd_58;
                Z80fd[0x59] = fd_59;
                Z80fd[0x5a] = fd_5a;
                Z80fd[0x5b] = fd_5b;
                Z80fd[0x5c] = fd_5c;
                Z80fd[0x5d] = fd_5d;
                Z80fd[0x5e] = fd_5e;
                Z80fd[0x5f] = fd_5f;

                Z80fd[0x60] = fd_60;
                Z80fd[0x61] = fd_61;
                Z80fd[0x62] = fd_62;
                Z80fd[0x63] = fd_63;
                Z80fd[0x64] = fd_64;
                Z80fd[0x65] = fd_65;
                Z80fd[0x66] = fd_66;
                Z80fd[0x67] = fd_67;
                Z80fd[0x68] = fd_68;
                Z80fd[0x69] = fd_69;
                Z80fd[0x6a] = fd_6a;
                Z80fd[0x6b] = fd_6b;
                Z80fd[0x6c] = fd_6c;
                Z80fd[0x6d] = fd_6d;
                Z80fd[0x6e] = fd_6e;
                Z80fd[0x6f] = fd_6f;

                Z80fd[0x70] = fd_70;
                Z80fd[0x71] = fd_71;
                Z80fd[0x72] = fd_72;
                Z80fd[0x73] = fd_73;
                Z80fd[0x74] = fd_74;
                Z80fd[0x75] = fd_75;
                Z80fd[0x76] = fd_76;
                Z80fd[0x77] = fd_77;
                Z80fd[0x78] = fd_78;
                Z80fd[0x79] = fd_79;
                Z80fd[0x7a] = fd_7a;
                Z80fd[0x7b] = fd_7b;
                Z80fd[0x7c] = fd_7c;
                Z80fd[0x7d] = fd_7d;
                Z80fd[0x7e] = fd_7e;
                Z80fd[0x7f] = fd_7f;

                Z80fd[0x80] = fd_80;
                Z80fd[0x81] = fd_81;
                Z80fd[0x82] = fd_82;
                Z80fd[0x83] = fd_83;
                Z80fd[0x84] = fd_84;
                Z80fd[0x85] = fd_85;
                Z80fd[0x86] = fd_86;
                Z80fd[0x87] = fd_87;
                Z80fd[0x88] = fd_88;
                Z80fd[0x89] = fd_89;
                Z80fd[0x8a] = fd_8a;
                Z80fd[0x8b] = fd_8b;
                Z80fd[0x8c] = fd_8c;
                Z80fd[0x8d] = fd_8d;
                Z80fd[0x8e] = fd_8e;
                Z80fd[0x8f] = fd_8f;

                Z80fd[0x90] = fd_90;
                Z80fd[0x91] = fd_91;
                Z80fd[0x92] = fd_92;
                Z80fd[0x93] = fd_93;
                Z80fd[0x94] = fd_94;
                Z80fd[0x95] = fd_95;
                Z80fd[0x96] = fd_96;
                Z80fd[0x97] = fd_97;
                Z80fd[0x98] = fd_98;
                Z80fd[0x99] = fd_99;
                Z80fd[0x9a] = fd_9a;
                Z80fd[0x9b] = fd_9b;
                Z80fd[0x9c] = fd_9c;
                Z80fd[0x9d] = fd_9d;
                Z80fd[0x9e] = fd_9e;
                Z80fd[0x9f] = fd_9f;

                Z80fd[0xa0] = fd_a0;
                Z80fd[0xa1] = fd_a1;
                Z80fd[0xa2] = fd_a2;
                Z80fd[0xa3] = fd_a3;
                Z80fd[0xa4] = fd_a4;
                Z80fd[0xa5] = fd_a5;
                Z80fd[0xa6] = fd_a6;
                Z80fd[0xa7] = fd_a7;
                Z80fd[0xa8] = fd_a8;
                Z80fd[0xa9] = fd_a9;
                Z80fd[0xaa] = fd_aa;
                Z80fd[0xab] = fd_ab;
                Z80fd[0xac] = fd_ac;
                Z80fd[0xad] = fd_ad;
                Z80fd[0xae] = fd_ae;
                Z80fd[0xaf] = fd_af;

                Z80fd[0xb0] = fd_b0;
                Z80fd[0xb1] = fd_b1;
                Z80fd[0xb2] = fd_b2;
                Z80fd[0xb3] = fd_b3;
                Z80fd[0xb4] = fd_b4;
                Z80fd[0xb5] = fd_b5;
                Z80fd[0xb6] = fd_b6;
                Z80fd[0xb7] = fd_b7;
                Z80fd[0xb8] = fd_b8;
                Z80fd[0xb9] = fd_b9;
                Z80fd[0xba] = fd_ba;
                Z80fd[0xbb] = fd_bb;
                Z80fd[0xbc] = fd_bc;
                Z80fd[0xbd] = fd_bd;
                Z80fd[0xbe] = fd_be;
                Z80fd[0xbf] = fd_bf;

                Z80fd[0xc0] = fd_c0;
                Z80fd[0xc1] = fd_c1;
                Z80fd[0xc2] = fd_c2;
                Z80fd[0xc3] = fd_c3;
                Z80fd[0xc4] = fd_c4;
                Z80fd[0xc5] = fd_c5;
                Z80fd[0xc6] = fd_c6;
                Z80fd[0xc7] = fd_c7;
                Z80fd[0xc8] = fd_c8;
                Z80fd[0xc9] = fd_c9;
                Z80fd[0xca] = fd_ca;
                Z80fd[0xcb] = fd_cb;
                Z80fd[0xcc] = fd_cc;
                Z80fd[0xcd] = fd_cd;
                Z80fd[0xce] = fd_ce;
                Z80fd[0xcf] = fd_cf;

                Z80fd[0xd0] = fd_d0;
                Z80fd[0xd1] = fd_d1;
                Z80fd[0xd2] = fd_d2;
                Z80fd[0xd3] = fd_d3;
                Z80fd[0xd4] = fd_d4;
                Z80fd[0xd5] = fd_d5;
                Z80fd[0xd6] = fd_d6;
                Z80fd[0xd7] = fd_d7;
                Z80fd[0xd8] = fd_d8;
                Z80fd[0xd9] = fd_d9;
                Z80fd[0xda] = fd_da;
                Z80fd[0xdb] = fd_db;
                Z80fd[0xdc] = fd_dc;
                Z80fd[0xdd] = fd_dd;
                Z80fd[0xde] = fd_de;
                Z80fd[0xdf] = fd_df;

                Z80fd[0xe0] = fd_e0;
                Z80fd[0xe1] = fd_e1;
                Z80fd[0xe2] = fd_e2;
                Z80fd[0xe3] = fd_e3;
                Z80fd[0xe4] = fd_e4;
                Z80fd[0xe5] = fd_e5;
                Z80fd[0xe6] = fd_e6;
                Z80fd[0xe7] = fd_e7;
                Z80fd[0xe8] = fd_e8;
                Z80fd[0xe9] = fd_e9;
                Z80fd[0xea] = fd_ea;
                Z80fd[0xeb] = fd_eb;
                Z80fd[0xec] = fd_ec;
                Z80fd[0xed] = fd_ed;
                Z80fd[0xee] = fd_ee;
                Z80fd[0xef] = fd_ef;

                Z80fd[0xf0] = fd_f0;
                Z80fd[0xf1] = fd_f1;
                Z80fd[0xf2] = fd_f2;
                Z80fd[0xf3] = fd_f3;
                Z80fd[0xf4] = fd_f4;
                Z80fd[0xf5] = fd_f5;
                Z80fd[0xf6] = fd_f6;
                Z80fd[0xf7] = fd_f7;
                Z80fd[0xf8] = fd_f8;
                Z80fd[0xf9] = fd_f9;
                Z80fd[0xfa] = fd_fa;
                Z80fd[0xfb] = fd_fb;
                Z80fd[0xfc] = fd_fc;
                Z80fd[0xfd] = fd_fd;
                Z80fd[0xfe] = fd_fe;
                Z80fd[0xff] = fd_ff;

            }
            void Setup_xxcb()
            {
                Z80xxcb[0x00] = xxcb_00;
                Z80xxcb[0x01] = xxcb_01;
                Z80xxcb[0x02] = xxcb_02;
                Z80xxcb[0x03] = xxcb_03;
                Z80xxcb[0x04] = xxcb_04;
                Z80xxcb[0x05] = xxcb_05;
                Z80xxcb[0x06] = xxcb_06;
                Z80xxcb[0x07] = xxcb_07;
                Z80xxcb[0x08] = xxcb_08;
                Z80xxcb[0x09] = xxcb_09;
                Z80xxcb[0x0a] = xxcb_0a;
                Z80xxcb[0x0b] = xxcb_0b;
                Z80xxcb[0x0c] = xxcb_0c;
                Z80xxcb[0x0d] = xxcb_0d;
                Z80xxcb[0x0e] = xxcb_0e;
                Z80xxcb[0x0f] = xxcb_0f;

                Z80xxcb[0x10] = xxcb_10;
                Z80xxcb[0x11] = xxcb_11;
                Z80xxcb[0x12] = xxcb_12;
                Z80xxcb[0x13] = xxcb_13;
                Z80xxcb[0x14] = xxcb_14;
                Z80xxcb[0x15] = xxcb_15;
                Z80xxcb[0x16] = xxcb_16;
                Z80xxcb[0x17] = xxcb_17;
                Z80xxcb[0x18] = xxcb_18;
                Z80xxcb[0x19] = xxcb_19;
                Z80xxcb[0x1a] = xxcb_1a;
                Z80xxcb[0x1b] = xxcb_1b;
                Z80xxcb[0x1c] = xxcb_1c;
                Z80xxcb[0x1d] = xxcb_1d;
                Z80xxcb[0x1e] = xxcb_1e;
                Z80xxcb[0x1f] = xxcb_1f;

                Z80xxcb[0x20] = xxcb_20;
                Z80xxcb[0x21] = xxcb_21;
                Z80xxcb[0x22] = xxcb_22;
                Z80xxcb[0x23] = xxcb_23;
                Z80xxcb[0x24] = xxcb_24;
                Z80xxcb[0x25] = xxcb_25;
                Z80xxcb[0x26] = xxcb_26;
                Z80xxcb[0x27] = xxcb_27;
                Z80xxcb[0x28] = xxcb_28;
                Z80xxcb[0x29] = xxcb_29;
                Z80xxcb[0x2a] = xxcb_2a;
                Z80xxcb[0x2b] = xxcb_2b;
                Z80xxcb[0x2c] = xxcb_2c;
                Z80xxcb[0x2d] = xxcb_2d;
                Z80xxcb[0x2e] = xxcb_2e;
                Z80xxcb[0x2f] = xxcb_2f;

                Z80xxcb[0x30] = xxcb_30;
                Z80xxcb[0x31] = xxcb_31;
                Z80xxcb[0x32] = xxcb_32;
                Z80xxcb[0x33] = xxcb_33;
                Z80xxcb[0x34] = xxcb_34;
                Z80xxcb[0x35] = xxcb_35;
                Z80xxcb[0x36] = xxcb_36;
                Z80xxcb[0x37] = xxcb_37;
                Z80xxcb[0x38] = xxcb_38;
                Z80xxcb[0x39] = xxcb_39;
                Z80xxcb[0x3a] = xxcb_3a;
                Z80xxcb[0x3b] = xxcb_3b;
                Z80xxcb[0x3c] = xxcb_3c;
                Z80xxcb[0x3d] = xxcb_3d;
                Z80xxcb[0x3e] = xxcb_3e;
                Z80xxcb[0x3f] = xxcb_3f;

                Z80xxcb[0x40] = xxcb_40;
                Z80xxcb[0x41] = xxcb_41;
                Z80xxcb[0x42] = xxcb_42;
                Z80xxcb[0x43] = xxcb_43;
                Z80xxcb[0x44] = xxcb_44;
                Z80xxcb[0x45] = xxcb_45;
                Z80xxcb[0x46] = xxcb_46;
                Z80xxcb[0x47] = xxcb_47;
                Z80xxcb[0x48] = xxcb_48;
                Z80xxcb[0x49] = xxcb_49;
                Z80xxcb[0x4a] = xxcb_4a;
                Z80xxcb[0x4b] = xxcb_4b;
                Z80xxcb[0x4c] = xxcb_4c;
                Z80xxcb[0x4d] = xxcb_4d;
                Z80xxcb[0x4e] = xxcb_4e;
                Z80xxcb[0x4f] = xxcb_4f;

                Z80xxcb[0x50] = xxcb_50;
                Z80xxcb[0x51] = xxcb_51;
                Z80xxcb[0x52] = xxcb_52;
                Z80xxcb[0x53] = xxcb_53;
                Z80xxcb[0x54] = xxcb_54;
                Z80xxcb[0x55] = xxcb_55;
                Z80xxcb[0x56] = xxcb_56;
                Z80xxcb[0x57] = xxcb_57;
                Z80xxcb[0x58] = xxcb_58;
                Z80xxcb[0x59] = xxcb_59;
                Z80xxcb[0x5a] = xxcb_5a;
                Z80xxcb[0x5b] = xxcb_5b;
                Z80xxcb[0x5c] = xxcb_5c;
                Z80xxcb[0x5d] = xxcb_5d;
                Z80xxcb[0x5e] = xxcb_5e;
                Z80xxcb[0x5f] = xxcb_5f;

                Z80xxcb[0x60] = xxcb_60;
                Z80xxcb[0x61] = xxcb_61;
                Z80xxcb[0x62] = xxcb_62;
                Z80xxcb[0x63] = xxcb_63;
                Z80xxcb[0x64] = xxcb_64;
                Z80xxcb[0x65] = xxcb_65;
                Z80xxcb[0x66] = xxcb_66;
                Z80xxcb[0x67] = xxcb_67;
                Z80xxcb[0x68] = xxcb_68;
                Z80xxcb[0x69] = xxcb_69;
                Z80xxcb[0x6a] = xxcb_6a;
                Z80xxcb[0x6b] = xxcb_6b;
                Z80xxcb[0x6c] = xxcb_6c;
                Z80xxcb[0x6d] = xxcb_6d;
                Z80xxcb[0x6e] = xxcb_6e;
                Z80xxcb[0x6f] = xxcb_6f;

                Z80xxcb[0x70] = xxcb_70;
                Z80xxcb[0x71] = xxcb_71;
                Z80xxcb[0x72] = xxcb_72;
                Z80xxcb[0x73] = xxcb_73;
                Z80xxcb[0x74] = xxcb_74;
                Z80xxcb[0x75] = xxcb_75;
                Z80xxcb[0x76] = xxcb_76;
                Z80xxcb[0x77] = xxcb_77;
                Z80xxcb[0x78] = xxcb_78;
                Z80xxcb[0x79] = xxcb_79;
                Z80xxcb[0x7a] = xxcb_7a;
                Z80xxcb[0x7b] = xxcb_7b;
                Z80xxcb[0x7c] = xxcb_7c;
                Z80xxcb[0x7d] = xxcb_7d;
                Z80xxcb[0x7e] = xxcb_7e;
                Z80xxcb[0x7f] = xxcb_7f;

                Z80xxcb[0x80] = xxcb_80;
                Z80xxcb[0x81] = xxcb_81;
                Z80xxcb[0x82] = xxcb_82;
                Z80xxcb[0x83] = xxcb_83;
                Z80xxcb[0x84] = xxcb_84;
                Z80xxcb[0x85] = xxcb_85;
                Z80xxcb[0x86] = xxcb_86;
                Z80xxcb[0x87] = xxcb_87;
                Z80xxcb[0x88] = xxcb_88;
                Z80xxcb[0x89] = xxcb_89;
                Z80xxcb[0x8a] = xxcb_8a;
                Z80xxcb[0x8b] = xxcb_8b;
                Z80xxcb[0x8c] = xxcb_8c;
                Z80xxcb[0x8d] = xxcb_8d;
                Z80xxcb[0x8e] = xxcb_8e;
                Z80xxcb[0x8f] = xxcb_8f;

                Z80xxcb[0x90] = xxcb_90;
                Z80xxcb[0x91] = xxcb_91;
                Z80xxcb[0x92] = xxcb_92;
                Z80xxcb[0x93] = xxcb_93;
                Z80xxcb[0x94] = xxcb_94;
                Z80xxcb[0x95] = xxcb_95;
                Z80xxcb[0x96] = xxcb_96;
                Z80xxcb[0x97] = xxcb_97;
                Z80xxcb[0x98] = xxcb_98;
                Z80xxcb[0x99] = xxcb_99;
                Z80xxcb[0x9a] = xxcb_9a;
                Z80xxcb[0x9b] = xxcb_9b;
                Z80xxcb[0x9c] = xxcb_9c;
                Z80xxcb[0x9d] = xxcb_9d;
                Z80xxcb[0x9e] = xxcb_9e;
                Z80xxcb[0x9f] = xxcb_9f;

                Z80xxcb[0xa0] = xxcb_a0;
                Z80xxcb[0xa1] = xxcb_a1;
                Z80xxcb[0xa2] = xxcb_a2;
                Z80xxcb[0xa3] = xxcb_a3;
                Z80xxcb[0xa4] = xxcb_a4;
                Z80xxcb[0xa5] = xxcb_a5;
                Z80xxcb[0xa6] = xxcb_a6;
                Z80xxcb[0xa7] = xxcb_a7;
                Z80xxcb[0xa8] = xxcb_a8;
                Z80xxcb[0xa9] = xxcb_a9;
                Z80xxcb[0xaa] = xxcb_aa;
                Z80xxcb[0xab] = xxcb_ab;
                Z80xxcb[0xac] = xxcb_ac;
                Z80xxcb[0xad] = xxcb_ad;
                Z80xxcb[0xae] = xxcb_ae;
                Z80xxcb[0xaf] = xxcb_af;

                Z80xxcb[0xb0] = xxcb_b0;
                Z80xxcb[0xb1] = xxcb_b1;
                Z80xxcb[0xb2] = xxcb_b2;
                Z80xxcb[0xb3] = xxcb_b3;
                Z80xxcb[0xb4] = xxcb_b4;
                Z80xxcb[0xb5] = xxcb_b5;
                Z80xxcb[0xb6] = xxcb_b6;
                Z80xxcb[0xb7] = xxcb_b7;
                Z80xxcb[0xb8] = xxcb_b8;
                Z80xxcb[0xb9] = xxcb_b9;
                Z80xxcb[0xba] = xxcb_ba;
                Z80xxcb[0xbb] = xxcb_bb;
                Z80xxcb[0xbc] = xxcb_bc;
                Z80xxcb[0xbd] = xxcb_bd;
                Z80xxcb[0xbe] = xxcb_be;
                Z80xxcb[0xbf] = xxcb_bf;

                Z80xxcb[0xc0] = xxcb_c0;
                Z80xxcb[0xc1] = xxcb_c1;
                Z80xxcb[0xc2] = xxcb_c2;
                Z80xxcb[0xc3] = xxcb_c3;
                Z80xxcb[0xc4] = xxcb_c4;
                Z80xxcb[0xc5] = xxcb_c5;
                Z80xxcb[0xc6] = xxcb_c6;
                Z80xxcb[0xc7] = xxcb_c7;
                Z80xxcb[0xc8] = xxcb_c8;
                Z80xxcb[0xc9] = xxcb_c9;
                Z80xxcb[0xca] = xxcb_ca;
                Z80xxcb[0xcb] = xxcb_cb;
                Z80xxcb[0xcc] = xxcb_cc;
                Z80xxcb[0xcd] = xxcb_cd;
                Z80xxcb[0xce] = xxcb_ce;
                Z80xxcb[0xcf] = xxcb_cf;

                Z80xxcb[0xd0] = xxcb_d0;
                Z80xxcb[0xd1] = xxcb_d1;
                Z80xxcb[0xd2] = xxcb_d2;
                Z80xxcb[0xd3] = xxcb_d3;
                Z80xxcb[0xd4] = xxcb_d4;
                Z80xxcb[0xd5] = xxcb_d5;
                Z80xxcb[0xd6] = xxcb_d6;
                Z80xxcb[0xd7] = xxcb_d7;
                Z80xxcb[0xd8] = xxcb_d8;
                Z80xxcb[0xd9] = xxcb_d9;
                Z80xxcb[0xda] = xxcb_da;
                Z80xxcb[0xdb] = xxcb_db;
                Z80xxcb[0xdc] = xxcb_dc;
                Z80xxcb[0xdd] = xxcb_dd;
                Z80xxcb[0xde] = xxcb_de;
                Z80xxcb[0xdf] = xxcb_df;

                Z80xxcb[0xe0] = xxcb_e0;
                Z80xxcb[0xe1] = xxcb_e1;
                Z80xxcb[0xe2] = xxcb_e2;
                Z80xxcb[0xe3] = xxcb_e3;
                Z80xxcb[0xe4] = xxcb_e4;
                Z80xxcb[0xe5] = xxcb_e5;
                Z80xxcb[0xe6] = xxcb_e6;
                Z80xxcb[0xe7] = xxcb_e7;
                Z80xxcb[0xe8] = xxcb_e8;
                Z80xxcb[0xe9] = xxcb_e9;
                Z80xxcb[0xea] = xxcb_ea;
                Z80xxcb[0xeb] = xxcb_eb;
                Z80xxcb[0xec] = xxcb_ec;
                Z80xxcb[0xed] = xxcb_ed;
                Z80xxcb[0xee] = xxcb_ee;
                Z80xxcb[0xef] = xxcb_ef;

                Z80xxcb[0xf0] = xxcb_f0;
                Z80xxcb[0xf1] = xxcb_f1;
                Z80xxcb[0xf2] = xxcb_f2;
                Z80xxcb[0xf3] = xxcb_f3;
                Z80xxcb[0xf4] = xxcb_f4;
                Z80xxcb[0xf5] = xxcb_f5;
                Z80xxcb[0xf6] = xxcb_f6;
                Z80xxcb[0xf7] = xxcb_f7;
                Z80xxcb[0xf8] = xxcb_f8;
                Z80xxcb[0xf9] = xxcb_f9;
                Z80xxcb[0xfa] = xxcb_fa;
                Z80xxcb[0xfb] = xxcb_fb;
                Z80xxcb[0xfc] = xxcb_fc;
                Z80xxcb[0xfd] = xxcb_fd;
                Z80xxcb[0xfe] = xxcb_fe;
                Z80xxcb[0xff] = xxcb_ff;

            }
            public override void set_op_base(int pc)
            {
                cpu_setOPbase16(pc, 0);
            }
            public override void burn(int cycles)
            {
                if (cycles > 0)
                {
                    /* NOP takes 4 cycles per instruction */
                    int n = (cycles + 3) / 4;
                    Z80.R += (byte)n;
                    z80_ICount[0] -= 4 * n;
                }
            }
            public override uint cpu_dasm(ref string buffer, uint pc)
            {
                throw new NotImplementedException();
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "Z80";
                    case CPU_INFO_FAMILY: return "Zilog Z80";
                    case CPU_INFO_VERSION: return "2.7";
                    case CPU_INFO_FILE: return "cpu_z80.cs";
                    case CPU_INFO_CREDITS: return "Copyright (C) 1998,1999 Juergen Buchmueller, all rights reserved.";
                }
                throw new Exception();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                Z80.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int state)
            {
                //LOG((errorlog, "Z80#%d set_irq_line %d\n",cpu_getactivecpu() , state));
                Z80.irq_state = (byte)state;
                if (state == CLEAR_LINE) return;

                if (Z80.irq_max != 0)
                {
                    int daisychain, device, int_state;
                    daisychain = Z80.irq_callback(irqline);
                    device = daisychain >> 8;
                    int_state = daisychain & 0xff;
                    //LOG((errorlog, "Z80#%d daisy chain $%04x -> device %d, state $%02x",cpu_getactivecpu(), daisychain, device, int_state));

                    if (Z80.int_state[device] != int_state)
                    {
                        //LOG((errorlog, " change\n"));
                        /* set new interrupt status */
                        Z80.int_state[device] = (byte)int_state;
                        /* check interrupt status */
                        Z80.request_irq = Z80.service_irq = -1;

                        /* search higher IRQ or IEO */
                        for (device = 0; device < Z80.irq_max; device++)
                        {
                            /* IEO = disable ? */
                            if ((Z80.int_state[device] & Z80_INT_IEO) != 0)
                            {
                                Z80.request_irq = -1;		/* if IEO is disable , masking lower IRQ */
                                Z80.service_irq = (sbyte)device;	/* set highest interrupt service device */
                            }
                            /* IRQ = request ? */
                            if ((Z80.int_state[device] & Z80_INT_REQ) != 0)
                                Z80.request_irq = (sbyte)device;
                        }
                        // LOG((errorlog, "Z80#%d daisy chain service_irq $%02x, request_irq $%02x\n", cpu_getactivecpu(), Z80.service_irq, Z80.request_irq));
                        if (Z80.request_irq < 0) return;
                    }
                    else
                    {
                        //LOG((errorlog, " no change\n"));
                        return;
                    }
                }
                take_interrupt();
            }
            public override void internal_interrupt(int type)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new Z80_Regs();
            }
            public override uint get_context(ref object reg)
            {                
                reg = Z80;
                return 1;
            }
            public override uint get_pc()
            {
                return Z80.PC.d;
            }
            public override uint get_reg(int regnum)
            {
                switch (regnum)
                {
                    case Z80_PC: return Z80.PC.wl;
                    case Z80_SP: return Z80.SP.wl;
                    case Z80_AF: return Z80.AF.wl;
                    case Z80_BC: return Z80.BC.wl;
                    case Z80_DE: return Z80.DE.wl;
                    case Z80_HL: return Z80.HL.wl;
                    case Z80_IX: return Z80.IX.wl;
                    case Z80_IY: return Z80.IY.wl;
                    case Z80_R: return (uint)((Z80.R & 0x7f) | (Z80.R2 & 0x80));
                    case Z80_I: return Z80.I;
                    case Z80_AF2: return Z80.AF2.wl;
                    case Z80_BC2: return Z80.BC2.wl;
                    case Z80_DE2: return Z80.DE2.wl;
                    case Z80_HL2: return Z80.HL2.wl;
                    case Z80_IM: return Z80.IM;
                    case Z80_IFF1: return Z80.IFF1;
                    case Z80_IFF2: return Z80.IFF2;
                    case Z80_HALT: return Z80.HALT;
                    case Z80_NMI_STATE: return Z80.nmi_state;
                    case Z80_IRQ_STATE: return Z80.irq_state;
                    case Z80_DC0: return Z80.int_state[0];
                    case Z80_DC1: return Z80.int_state[1];
                    case Z80_DC2: return Z80.int_state[2];
                    case Z80_DC3: return Z80.int_state[3];
                    case REG_PREVIOUSPC: return Z80.PREPC.wl;
                    default:
                        if (regnum <= REG_SP_CONTENTS)
                        {
                            uint offset = (uint)(Z80.SP.d + 2 * (REG_SP_CONTENTS - regnum));
                            if (offset < 0xffff)
                                return (uint)(cpu_readmem16((int)offset) | (cpu_readmem16((int)offset + 1) << 8));
                        }
                        break;
                }
                return 0;
            }
            public override uint get_sp()
            {
                throw new NotImplementedException();
            }
            public override void set_nmi_line(int state)
            {
                if (Z80.nmi_state == state) return;

                //LOG((errorlog, "Z80#%d set_nmi_line %d\n", cpu_getactivecpu(), state));
                Z80.nmi_state = (byte)state;
                if (state == CLEAR_LINE) return;

                //LOG((errorlog, "Z80#%d take NMI\n", cpu_getactivecpu()));
                Z80.PREPC.d = unchecked((uint)-1);			/* there isn't a valid previous program counter */
                if (Z80.HALT != 0)
                {
                    Z80.HALT = 0;
                    Z80.PC.wl++;
                }	 		/* Check if processor was halted */

                Z80.IFF1 = 0;
                //PUSH( PC );
                Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC);
                Z80.PC.d = 0x0066;
                Z80.extra_cycles += 11;
            }
            public override void cpu_state_load(object file)
            {
                throw new NotImplementedException();
            }
            public override void cpu_state_save(object file)
            {
                throw new NotImplementedException();
            }
            public override int memory_read(int offset)
            {
                return cpu_readmem16(offset);
            }
            public override void memory_write(int offset, int data)
            {
                cpu_writemem16(offset, data);
            }
            public override void set_pc(uint val)
            {
                throw new NotImplementedException();
            }
            public override void set_reg(int regnum, uint val)
            {
                throw new NotImplementedException();
            }
            public override void set_sp(uint val)
            {
                throw new NotImplementedException();
            }
            public const byte Z80_PC = 1, Z80_SP = 2, Z80_AF = 3, Z80_BC = 4, Z80_DE = 5, Z80_HL = 6,
                Z80_IX = 7, Z80_IY = 8, Z80_AF2 = 9, Z80_BC2 = 10, Z80_DE2 = 11, Z80_HL2 = 12,
                Z80_R = 13, Z80_I = 14, Z80_IM = 15, Z80_IFF1 = 16, Z80_IFF2 = 17, Z80_HALT = 18,
                Z80_NMI_STATE = 19, Z80_IRQ_STATE = 20, Z80_DC0 = 21, Z80_DC1 = 22, Z80_DC2 = 23, Z80_DC3 = 24,
                Z80_NMI_NESTING = 25;

            static byte[] z80_reg_layout = {
    Z80_PC, Z80_SP, Z80_AF, Z80_BC, Z80_DE, Z80_HL, unchecked((byte)-1),
    Z80_IX, Z80_IY, Z80_AF2,Z80_BC2,Z80_DE2,Z80_HL2,unchecked((byte)-1),
    Z80_R,  Z80_I,  Z80_IM, Z80_IFF1,Z80_IFF2, unchecked((byte)-1),
	Z80_NMI_STATE,Z80_IRQ_STATE,Z80_DC0,Z80_DC1,Z80_DC2,Z80_DC3, 0
};
            public abstract class Z80_DaisyChain
            {
                public abstract void reset(int i);
                public abstract int interrupt_entry(int i);
                public abstract void interrupt_reti(int i);
                public int irq_param;
            }
            class Z80_Regs
            {
                public PAIR PREPC, PC, SP, AF, BC, DE, HL, IX, IY;
                public PAIR AF2, BC2, DE2, HL2;
                public byte R, R2, IFF1, IFF2, HALT, IM, I;
                public byte irq_max;
                public sbyte request_irq, service_irq;
                public byte nmi_state, irq_state;
                public irqcallback irq_callback;
                public int extra_cycles;
                public byte[] int_state = new byte[Z80_MAXDAISY];
                public Z80_DaisyChain[] irq = new Z80_DaisyChain[Z80_MAXDAISY];
                public string Flags
                {
                    get
                    {
                        return sprintf("%c%c%c%c%c%c%c%c",
                (AF.bl & 0x80) != 0 ? 'S' : '.',
                (AF.bl & 0x40) != 0 ? 'Z' : '.',
                (AF.bl & 0x20) != 0 ? '5' : '.',
                (AF.bl & 0x10) != 0 ? 'H' : '.',
                (AF.bl & 0x08) != 0 ? '3' : '.',
                (AF.bl & 0x04) != 0 ? 'P' : '.',
                (AF.bl & 0x02) != 0 ? 'N' : '.',
                (AF.bl & 0x01) != 0 ? 'C' : '.');
                    }
                }
            }
            uint EA;

            Z80_Regs Z80 = new Z80_Regs();
            uint ea;
            bool after_EI = false;
            byte[] SZ = new byte[256];
            byte[] SZ_BIT = new byte[256];
            byte[] SZP = new byte[256];
            byte[] SZHV_inc = new byte[256];
            byte[] SZHV_dec = new byte[256];
            _BytePtr SZHVC_add = null, SZHVC_sub = null;
            delegate void opcode();

            opcode[] Z80cb = new opcode[256];
            opcode[] Z80dd = new opcode[256];
            opcode[] Z80ed = new opcode[256];
            opcode[] Z80fd = new opcode[256];
            opcode[] Z80op = new opcode[256];
            opcode[] Z80xxcb = new opcode[256];

            public override int execute(int cycles)
            {
                z80_ICount[0] = cycles - Z80.extra_cycles;
                Z80.extra_cycles = 0;

                do
                {
                    if (Z80.PC.d == 0x3075)
                    {
                        int a = 0;
                    }
                    Z80.PREPC.d = Z80.PC.d;
                    Z80.R++;
                    uint op = ROP();
                    z80_ICount[0] -= cc_op[op];
                    Z80op[op]();
#if false
                    switch (op)
                    {
                        case 0x00: op_00(); break;
                        case 0x01: op_01(); break;
                        case 0x02: op_02(); break;
                        case 0x03: op_03(); break;
                        case 0x04: op_04(); break;
                        case 0x05: op_05(); break;
                        case 0x06: op_06(); break;
                        case 0x07: op_07(); break;
                        case 0x08: op_08(); break;
                        case 0x09: op_09(); break;
                        case 0x0a: op_0a(); break;
                        case 0x0b: op_0b(); break;
                        case 0x0c: op_0c(); break;
                        case 0x0d: op_0d(); break;
                        case 0x0e: op_0e(); break;
                        case 0x0f: op_0f(); break;
                        case 0x10: op_10(); break;
                        case 0x11: op_11(); break;
                        case 0x12: op_12(); break;
                        case 0x13: op_13(); break;
                        case 0x14: op_14(); break;
                        case 0x15: op_15(); break;
                        case 0x16: op_16(); break;
                        case 0x17: op_17(); break;
                        case 0x18: op_18(); break;
                        case 0x19: op_19(); break;
                        case 0x1a: op_1a(); break;
                        case 0x1b: op_1b(); break;
                        case 0x1c: op_1c(); break;
                        case 0x1d: op_1d(); break;
                        case 0x1e: op_1e(); break;
                        case 0x1f: op_1f(); break;
                        case 0x20: op_20(); break;
                        case 0x21: op_21(); break;
                        case 0x22: op_22(); break;
                        case 0x23: op_23(); break;
                        case 0x24: op_24(); break;
                        case 0x25: op_25(); break;
                        case 0x26: op_26(); break;
                        case 0x27: op_27(); break;
                        case 0x28: op_28(); break;
                        case 0x29: op_29(); break;
                        case 0x2a: op_2a(); break;
                        case 0x2b: op_2b(); break;
                        case 0x2c: op_2c(); break;
                        case 0x2d: op_2d(); break;
                        case 0x2e: op_2e(); break;
                        case 0x2f: op_2f(); break;
                        case 0x30: op_30(); break;
                        case 0x31: op_31(); break;
                        case 0x32: op_32(); break;
                        case 0x33: op_33(); break;
                        case 0x34: op_34(); break;
                        case 0x35: op_35(); break;
                        case 0x36: op_36(); break;
                        case 0x37: op_37(); break;
                        case 0x38: op_38(); break;
                        case 0x39: op_39(); break;
                        case 0x3a: op_3a(); break;
                        case 0x3b: op_3b(); break;
                        case 0x3c: op_3c(); break;
                        case 0x3d: op_3d(); break;
                        case 0x3e: op_3e(); break;
                        case 0x3f: op_3f(); break;
                        case 0x40: op_40(); break;
                        case 0x41: op_41(); break;
                        case 0x42: op_42(); break;
                        case 0x43: op_43(); break;
                        case 0x44: op_44(); break;
                        case 0x45: op_45(); break;
                        case 0x46: op_46(); break;
                        case 0x47: op_47(); break;
                        case 0x48: op_48(); break;
                        case 0x49: op_49(); break;
                        case 0x4a: op_4a(); break;
                        case 0x4b: op_4b(); break;
                        case 0x4c: op_4c(); break;
                        case 0x4d: op_4d(); break;
                        case 0x4e: op_4e(); break;
                        case 0x4f: op_4f(); break;
                        case 0x50: op_50(); break;
                        case 0x51: op_51(); break;
                        case 0x52: op_52(); break;
                        case 0x53: op_53(); break;
                        case 0x54: op_54(); break;
                        case 0x55: op_55(); break;
                        case 0x56: op_56(); break;
                        case 0x57: op_57(); break;
                        case 0x58: op_58(); break;
                        case 0x59: op_59(); break;
                        case 0x5a: op_5a(); break;
                        case 0x5b: op_5b(); break;
                        case 0x5c: op_5c(); break;
                        case 0x5d: op_5d(); break;
                        case 0x5e: op_5e(); break;
                        case 0x5f: op_5f(); break;
                        case 0x60: op_60(); break;
                        case 0x61: op_61(); break;
                        case 0x62: op_62(); break;
                        case 0x63: op_63(); break;
                        case 0x64: op_64(); break;
                        case 0x65: op_65(); break;
                        case 0x66: op_66(); break;
                        case 0x67: op_67(); break;
                        case 0x68: op_68(); break;
                        case 0x69: op_69(); break;
                        case 0x6a: op_6a(); break;
                        case 0x6b: op_6b(); break;
                        case 0x6c: op_6c(); break;
                        case 0x6d: op_6d(); break;
                        case 0x6e: op_6e(); break;
                        case 0x6f: op_6f(); break;
                        case 0x70: op_70(); break;
                        case 0x71: op_71(); break;
                        case 0x72: op_72(); break;
                        case 0x73: op_73(); break;
                        case 0x74: op_74(); break;
                        case 0x75: op_75(); break;
                        case 0x76: op_76(); break;
                        case 0x77: op_77(); break;
                        case 0x78: op_78(); break;
                        case 0x79: op_79(); break;
                        case 0x7a: op_7a(); break;
                        case 0x7b: op_7b(); break;
                        case 0x7c: op_7c(); break;
                        case 0x7d: op_7d(); break;
                        case 0x7e: op_7e(); break;
                        case 0x7f: op_7f(); break;
                        case 0x80: op_80(); break;
                        case 0x81: op_81(); break;
                        case 0x82: op_82(); break;
                        case 0x83: op_83(); break;
                        case 0x84: op_84(); break;
                        case 0x85: op_85(); break;
                        case 0x86: op_86(); break;
                        case 0x87: op_87(); break;
                        case 0x88: op_88(); break;
                        case 0x89: op_89(); break;
                        case 0x8a: op_8a(); break;
                        case 0x8b: op_8b(); break;
                        case 0x8c: op_8c(); break;
                        case 0x8d: op_8d(); break;
                        case 0x8e: op_8e(); break;
                        case 0x8f: op_8f(); break;
                        case 0x90: op_90(); break;
                        case 0x91: op_91(); break;
                        case 0x92: op_92(); break;
                        case 0x93: op_93(); break;
                        case 0x94: op_94(); break;
                        case 0x95: op_95(); break;
                        case 0x96: op_96(); break;
                        case 0x97: op_97(); break;
                        case 0x98: op_98(); break;
                        case 0x99: op_99(); break;
                        case 0x9a: op_9a(); break;
                        case 0x9b: op_9b(); break;
                        case 0x9c: op_9c(); break;
                        case 0x9d: op_9d(); break;
                        case 0x9e: op_9e(); break;
                        case 0x9f: op_9f(); break;
                        case 0xa0: op_a0(); break;
                        case 0xa1: op_a1(); break;
                        case 0xa2: op_a2(); break;
                        case 0xa3: op_a3(); break;
                        case 0xa4: op_a4(); break;
                        case 0xa5: op_a5(); break;
                        case 0xa6: op_a6(); break;
                        case 0xa7: op_a7(); break;
                        case 0xa8: op_a8(); break;
                        case 0xa9: op_a9(); break;
                        case 0xaa: op_aa(); break;
                        case 0xab: op_ab(); break;
                        case 0xac: op_ac(); break;
                        case 0xad: op_ad(); break;
                        case 0xae: op_ae(); break;
                        case 0xaf: op_af(); break;
                        case 0xb0: op_b0(); break;
                        case 0xb1: op_b1(); break;
                        case 0xb2: op_b2(); break;
                        case 0xb3: op_b3(); break;
                        case 0xb4: op_b4(); break;
                        case 0xb5: op_b5(); break;
                        case 0xb6: op_b6(); break;
                        case 0xb7: op_b7(); break;
                        case 0xb8: op_b8(); break;
                        case 0xb9: op_b9(); break;
                        case 0xba: op_ba(); break;
                        case 0xbb: op_bb(); break;
                        case 0xbc: op_bc(); break;
                        case 0xbd: op_bd(); break;
                        case 0xbe: op_be(); break;
                        case 0xbf: op_bf(); break;
                        case 0xc0: op_c0(); break;
                        case 0xc1: op_c1(); break;
                        case 0xc2: op_c2(); break;
                        case 0xc3: op_c3(); break;
                        case 0xc4: op_c4(); break;
                        case 0xc5: op_c5(); break;
                        case 0xc6: op_c6(); break;
                        case 0xc7: op_c7(); break;
                        case 0xc8: op_c8(); break;
                        case 0xc9: op_c9(); break;
                        case 0xca: op_ca(); break;
                        case 0xcb: op_cb(); break;
                        case 0xcc: op_cc(); break;
                        case 0xcd: op_cd(); break;
                        case 0xce: op_ce(); break;
                        case 0xcf: op_cf(); break;
                        case 0xd0: op_d0(); break;
                        case 0xd1: op_d1(); break;
                        case 0xd2: op_d2(); break;
                        case 0xd3: op_d3(); break;
                        case 0xd4: op_d4(); break;
                        case 0xd5: op_d5(); break;
                        case 0xd6: op_d6(); break;
                        case 0xd7: op_d7(); break;
                        case 0xd8: op_d8(); break;
                        case 0xd9: op_d9(); break;
                        case 0xda: op_da(); break;
                        case 0xdb: op_db(); break;
                        case 0xdc: op_dc(); break;
                        case 0xdd: op_dd(); break;
                        case 0xde: op_de(); break;
                        case 0xdf: op_df(); break;
                        case 0xe0: op_e0(); break;
                        case 0xe1: op_e1(); break;
                        case 0xe2: op_e2(); break;
                        case 0xe3: op_e3(); break;
                        case 0xe4: op_e4(); break;
                        case 0xe5: op_e5(); break;
                        case 0xe6: op_e6(); break;
                        case 0xe7: op_e7(); break;
                        case 0xe8: op_e8(); break;
                        case 0xe9: op_e9(); break;
                        case 0xea: op_ea(); break;
                        case 0xeb: op_eb(); break;
                        case 0xec: op_ec(); break;
                        case 0xed: op_ed(); break;
                        case 0xee: op_ee(); break;
                        case 0xef: op_ef(); break;
                        case 0xf0: op_f0(); break;
                        case 0xf1: op_f1(); break;
                        case 0xf2: op_f2(); break;
                        case 0xf3: op_f3(); break;
                        case 0xf4: op_f4(); break;
                        case 0xf5: op_f5(); break;
                        case 0xf6: op_f6(); break;
                        case 0xf7: op_f7(); break;
                        case 0xf8: op_f8(); break;
                        case 0xf9: op_f9(); break;
                        case 0xfa: op_fa(); break;
                        case 0xfb: op_fb(); break;
                        case 0xfc: op_fc(); break;
                        case 0xfd: op_fd(); break;
                        case 0xfe: op_fe(); break;
                        case 0xff: op_ff(); break;
                    }
#endif
                } while (z80_ICount[0] > 0);

                z80_ICount[0] -= Z80.extra_cycles;
                Z80.extra_cycles = 0;

                return cycles - z80_ICount[0];
            }
            public override void exit()
            {
                SZHVC_add = null;
                SZHVC_sub = null;
            }
            const byte CF = 0x01;
            const byte NF = 0x02;
            const byte PF = 0x04;
            const byte VF = PF;
            const byte XF = 0x08;
            const byte HF = 0x10;
            const byte YF = 0x20;
            const byte ZF = 0x40;
            const byte SF = 0x80;
            public override void reset(object param)
            {
                Z80_DaisyChain[] daisy_chain = (Z80_DaisyChain[])param;
                int i, p;

                if (SZHVC_add == null || SZHVC_sub == null)
                {
                    int oldval, newval, val;
                    _BytePtr padd, padc, psub, psbc;
                    /* allocate big flag arrays once */
                    SZHVC_add = new _BytePtr(2 * 256 * 256);
                    SZHVC_sub = new _BytePtr(2 * 256 * 256);

                    padd = new _BytePtr(SZHVC_add, 0 * 256);
                    padc = new _BytePtr(SZHVC_add, 256 * 256);
                    psub = new _BytePtr(SZHVC_sub, 0 * 256);
                    psbc = new _BytePtr(SZHVC_sub, 256 * 256);
                    for (oldval = 0; oldval < 256; oldval++)
                    {
                        for (newval = 0; newval < 256; newval++)
                        {
                            /* add or adc w/o carry set */
                            val = newval - oldval;
                            padd[0] = (newval) != 0 ? ((newval & 0x80) != 0 ? SF : (byte)0) : ZF;
                            padd[0] |= (byte)(newval & (YF | XF));	/* undocumented flag bits 5+3 */
                            if ((newval & 0x0f) < (oldval & 0x0f)) padd[0] |= HF;
                            if (newval < oldval) padd[0] |= CF;
                            if (((val ^ oldval ^ 0x80) & (val ^ newval) & 0x80) != 0) padd[0] |= VF;
                            padd.offset++;

                            /* adc with carry set */
                            val = newval - oldval - 1;
                            padc[0] = (newval) != 0 ? ((newval & 0x80) != 0 ? SF : (byte)0) : ZF;
                            padc[0] |= (byte)(newval & (YF | XF));	/* undocumented flag bits 5+3 */
                            if ((newval & 0x0f) <= (oldval & 0x0f)) padc[0] |= HF;
                            if (newval <= oldval) padc[0] |= CF;
                            if (((val ^ oldval ^ 0x80) & (val ^ newval) & 0x80) != 0) padc[0] |= VF;
                            padc.offset++;

                            /* cp, sub or sbc w/o carry set */
                            val = oldval - newval;
                            psub[0] = (byte)(NF | ((newval) != 0 ? ((newval & 0x80) != 0 ? SF : 0) : ZF));
                            psub[0] |= (byte)(newval & (YF | XF));	/* undocumented flag bits 5+3 */
                            if ((newval & 0x0f) > (oldval & 0x0f)) psub[0] |= HF;
                            if (newval > oldval) psub[0] |= CF;
                            if (((val ^ oldval) & (oldval ^ newval) & 0x80) != 0) psub[0] |= VF;
                            psub.offset++;

                            /* sbc with carry set */
                            val = oldval - newval - 1;
                            psbc[0] = (byte)(NF | ((newval) != 0 ? ((newval & 0x80) != 0 ? SF : 0) : ZF));
                            psbc[0] |= (byte)(newval & (YF | XF));	/* undocumented flag bits 5+3 */
                            if ((newval & 0x0f) >= (oldval & 0x0f)) psbc[0] |= HF;
                            if (newval >= oldval) psbc[0] |= CF;
                            if (((val ^ oldval) & (oldval ^ newval) & 0x80) != 0) psbc[0] |= VF;
                            psbc.offset++;
                        }
                    }
                }
                for (i = 0; i < 256; i++)
                {
                    p = 0;
                    if ((i & 0x01) != 0) ++p;
                    if ((i & 0x02) != 0) ++p;
                    if ((i & 0x04) != 0) ++p;
                    if ((i & 0x08) != 0) ++p;
                    if ((i & 0x10) != 0) ++p;
                    if ((i & 0x20) != 0) ++p;
                    if ((i & 0x40) != 0) ++p;
                    if ((i & 0x80) != 0) ++p;
                    SZ[i] = (byte)(i != 0 ? i & SF : ZF);
                    SZ[i] |= (byte)(i & (YF | XF));		/* undocumented flag bits 5+3 */
                    SZ_BIT[i] = (byte)(i != 0 ? i & SF : ZF | PF);
                    SZ_BIT[i] |= (byte)(i & (YF | XF));	/* undocumented flag bits 5+3 */
                    SZP[i] = (byte)(SZ[i] | ((p & 1) != 0 ? 0 : PF));
                    SZHV_inc[i] = SZ[i];
                    if (i == 0x80) SZHV_inc[i] |= VF;
                    if ((i & 0x0f) == 0x00) SZHV_inc[i] |= HF;
                    SZHV_dec[i] = (byte)(SZ[i] | NF);
                    if (i == 0x7f) SZHV_dec[i] |= VF;
                    if ((i & 0x0f) == 0x0f) SZHV_dec[i] |= HF;
                }

                //memset(ref Z80, 0, sizeof(Z80));
                Z80.IX.wl = Z80.IY.wl = 0xffff; /* IX and IY are FFFF after a reset! */
                Z80.AF.bl = ZF;			/* Zero flag is set */
                Z80.request_irq = -1;
                Z80.service_irq = -1;
                Z80.nmi_state = CLEAR_LINE;
                Z80.irq_state = CLEAR_LINE;

                int dci = 0;
                if (daisy_chain != null)
                {
                    while (daisy_chain[dci].irq_param != -1 && Z80.irq_max < Z80_MAXDAISY)
                    {
                        /* set callbackhandler after reti */
                        Z80.irq[Z80.irq_max] = daisy_chain[dci];
                        /* device reset */

                        Z80.irq[Z80.irq_max].reset(Z80.irq[Z80.irq_max].irq_param);
                        Z80.irq_max++;
                        dci++;
                    }
                }

                change_pc(Z80.PC.d);
            }
            public override void set_context(object reg)
            {
                Z80 = (Z80_Regs)reg;
                change_pc(Z80.PC.d);
            }
            byte ARG()
            {
                uint pc = Z80.PC.d;
                Z80.PC.wl++;
                return cpu_readop_arg(pc);
            }
            ushort ARG16()
            {
                uint pc = Z80.PC.d;
                Z80.PC.wl += 2;
                return (ushort)(cpu_readop_arg(pc) | (cpu_readop_arg((pc + 1) & 0xffff) << 8));
            }
            byte INC(byte value)
            {
                byte res = (byte)(value + 1);
                Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZHV_inc[res]);
                return (byte)res;
            }
            byte DEC(byte value)
            {
                byte res = (byte)(value - 1);
                Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZHV_dec[res]);
                return res;
            }
            void BURNODD(int cycles, int opcodes, int cyclesum)
            {
                if (cycles > 0)
                {
                    Z80.R += (byte)((cycles / cyclesum) * opcodes);
                    z80_ICount[0] -= (cycles / cyclesum) * cyclesum;
                }
            }
            void RM16(uint addr, ref PAIR r)
            {
                r.bl = (byte)cpu_readmem16((int)addr);
                r.bh = (byte)cpu_readmem16((int)(addr + 1) & 0xffff);
            }
            void WM16(uint addr, PAIR r)
            {
                cpu_writemem16((int)addr, r.bl);
                cpu_writemem16((int)(addr + 1) & 0xffff, r.bh);
            }
            byte ROP()
            {
                uint pc = Z80.PC.d;
                Z80.PC.wl++;
                return cpu_readop(pc);
            }
            byte RLC(byte value)
            {
                uint res = value;
                uint c = (res & 0x80) != 0 ? (byte)0x01 : (byte)0;
                res = ((res << 1) | (res >> 7)) & 0xff;
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte RRC(byte value)
            {
                uint res = value;
                uint c = (res & 0x01) != 0 ? (byte)0x01 : (byte)0;
                res = ((res >> 1) | (res << 7)) & 0xff;
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte RL(byte value)
            {
                uint res = value;
                uint c = (res & 0x80) != 0 ? (byte)0x01 : (byte)0;
                res = (uint)(((res << 1) | (Z80.AF.bl & 0x01)) & 0xff);
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte RR(byte value)
            {
                uint res = value;
                uint c = (res & 0x01) != 0 ? (byte)0x01 : (byte)0;
                res = (uint)(((res >> 1) | (Z80.AF.bl << 7)) & 0xff);
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte SLA(byte value)
            {
                uint res = value;
                uint c = (res & 0x80) != 0 ? (byte)0x01 : (byte)0;
                res = (res << 1) & 0xff;
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte SRA(byte value)
            {
                uint res = value;
                uint c = (res & 0x01) != 0 ? (byte)0x01 : (byte)0;
                res = ((res >> 1) | (res & 0x80)) & 0xff;
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte SLL(byte value)
            {
                uint res = value;
                uint c = (res & 0x80) != 0 ? (byte)0x01 : (byte)0;
                res = ((res << 1) | 0x01) & 0xff;
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte SRL(byte value)
            {
                uint res = value;
                uint c = (res & 0x01) != 0 ? (byte)0x01 : (byte)0;
                res = (res >> 1) & 0xff;
                Z80.AF.bl = (byte)(SZP[res] | c);
                return (byte)res;
            }
            byte RES(byte bit, byte value)
            {
                return (byte)((value & ~(1 << bit)));
            }
            byte SET(byte bit, byte value)
            {
                return (byte)(value | (1 << bit));
            }
            void illegal_1()
            {
                printf("Z80#%d ill. opcode $%02x $%02x\n",cpu_getactivecpu(), cpu_readop((Z80.PC.d - 1) & 0xffff), cpu_readop(Z80.PC.d));
            }
            void illegal_2()
            {
                printf("Z80#%d ill. opcode $ed $%02x\n",cpu_getactivecpu(), cpu_readop((Z80.PC.d - 1) & 0xffff));
            }

            #region op main opcodes
            void op_00() { } /* NOP			  */
            void op_01() { Z80.BC.wl = ARG16(); } /* LD   BC,w		  */
            void op_02() { cpu_writemem16(Z80.BC.wl, Z80.AF.bh); } /* LD   (BC),A	  */
            void op_03() { Z80.BC.wl++; } /* INC  BC		  */
            void op_04() { Z80.BC.bh = INC(Z80.BC.bh); } /* INC  B 		  */
            void op_05() { Z80.BC.bh = DEC(Z80.BC.bh); } /* DEC  B 		  */
            void op_06() { Z80.BC.bh = ARG(); } /* LD   B,n		  */
            void op_07()
            {
                Z80.AF.bh = (byte)((Z80.AF.bh << 1) | (Z80.AF.bh >> 7));
                Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (Z80.AF.bh & (0x20 | 0x08 | 0x01)));
            } /* RLCA			  */

            void op_08() { { PAIR tmp; tmp = Z80.AF; Z80.AF = Z80.AF2; Z80.AF2 = tmp; }; } /* EX   AF,AF'      */
            void op_09()
            {
                {
                    uint res = Z80.HL.d + Z80.BC.d;
                    Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.HL.d ^ res ^ Z80.BC.d) >> 8) & 0x10) | ((res >> 16) & 0x01));
                    Z80.HL.wl = (ushort)res;
                };
            } /* ADD  HL,BC 	  */
            void op_0a() { Z80.AF.bh = (byte)cpu_readmem16(Z80.BC.wl); } /* LD   A,(BC)	  */
            void op_0b()
            {
                Z80.BC.wl--; if (Z80.BC.wl > 1 && Z80.PC.d < 0xfffc)
                {
                    byte op1 = cpu_readop(Z80.PC.d);
                    byte op2 = cpu_readop(Z80.PC.d + 1); if ((op1 == 0x78 && op2 == 0xb1) || (op1 == 0x79 && op2 == 0xb0)) { byte op3 = cpu_readop(Z80.PC.d + 2); byte op4 = cpu_readop(Z80.PC.d + 3); if (op3 == 0x20 && op4 == 0xfb) { while (Z80.BC.wl > 0 && z80_ICount[0] > 4 + 4 + 12 + 6) { BURNODD(4 + 4 + 12 + 6, 4, 4 + 4 + 12 + 6); Z80.BC.wl--; } } else if (op3 == 0xc2) { byte ad1 = cpu_readop_arg(Z80.PC.d + 3); byte ad2 = cpu_readop_arg(Z80.PC.d + 4); if ((ad1 + 256 * ad2) == (Z80.PC.d - 1)) { while (Z80.BC.wl > 0 && z80_ICount[0] > 4 + 4 + 10 + 6) { BURNODD(4 + 4 + 10 + 6, 4, 4 + 4 + 10 + 6); Z80.BC.wl--; } } } }
                };
            } /* DEC  BC		  */
            void op_0c() { Z80.BC.bl = INC(Z80.BC.bl); } /* INC  C 		  */
            void op_0d() { Z80.BC.bl = DEC(Z80.BC.bl); } /* DEC  C 		  */
            void op_0e() { Z80.BC.bl = ARG(); } /* LD   C,n		  */
            void op_0f()
            {
                Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (Z80.AF.bh & (0x20 | 0x08 | 0x01)));
                Z80.AF.bh = (byte)((Z80.AF.bh >> 1) | (Z80.AF.bh << 7));
            } /* RRCA			  */

            void op_10() { Z80.BC.bh--; if (Z80.BC.bh != 0) { sbyte arg = (sbyte)ARG(); Z80.PC.wl += (ushort)(short)arg; z80_ICount[0] -= 5; change_pc16(Z80.PC.d); } else Z80.PC.wl++; ; } /* DJNZ o 		  */
            void op_11() { Z80.DE.wl = ARG16(); } /* LD   DE,w		  */
            void op_12() { cpu_writemem16(Z80.DE.wl, Z80.AF.bh); } /* LD   (DE),A	  */
            void op_13() { Z80.DE.wl++; } /* INC  DE		  */
            void op_14() { Z80.DE.bh = INC(Z80.DE.bh); } /* INC  D 		  */
            void op_15() { Z80.DE.bh = DEC(Z80.DE.bh); } /* DEC  D 		  */
            void op_16() { Z80.DE.bh = ARG(); } /* LD   D,n		  */
            void op_17()
            {
                {
                    byte res = (byte)((Z80.AF.bh << 1) | (Z80.AF.bl & 0x01));
                    byte c = (Z80.AF.bh & 0x80) != 0 ? (byte)0x01 : (byte)0;
                    Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | c | (res & (0x20 | 0x08))); Z80.AF.bh = res;
                };
            } /* RLA			  */

            void op_18()
            {
                {
                    uint oldpc = Z80.PC.d - 1; sbyte arg = (sbyte)ARG(); Z80.PC.wl += (ushort)(short)arg; change_pc16(Z80.PC.d);
                    if (Z80.PC.d == oldpc) { if (!after_EI) BURNODD(z80_ICount[0], 1, 12); } else { byte op = cpu_readop(Z80.PC.d); if (Z80.PC.d == oldpc - 1) { if (op == 0x00 || op == 0xfb) { if (!after_EI) BURNODD(z80_ICount[0] - 4, 2, 4 + 12); } } else if (Z80.PC.d == oldpc - 3 && op == 0x31) { if (!after_EI) BURNODD(z80_ICount[0] - 12, 2, 10 + 12); } }
                };
            } /* JR   o 		  */
            void op_19()
            {
                {
                    uint res = Z80.HL.d + Z80.DE.d;
                    Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.HL.d ^ res ^ Z80.DE.d) >> 8) & 0x10) | ((res >> 16) & 0x01));
                    Z80.HL.wl = (ushort)res;
                };
            } /* ADD  HL,DE 	  */
            void op_1a() { Z80.AF.bh = (byte)cpu_readmem16(Z80.DE.wl); } /* LD   A,(DE)	  */
            void op_1b() { Z80.DE.wl--; if (Z80.DE.wl > 1 && Z80.PC.d < 0xfffc) { byte op1 = cpu_readop(Z80.PC.d); byte op2 = cpu_readop(Z80.PC.d + 1); if ((op1 == 0x7a && op2 == 0xb3) || (op1 == 0x7b && op2 == 0xb2)) { byte op3 = cpu_readop(Z80.PC.d + 2); byte op4 = cpu_readop(Z80.PC.d + 3); if (op3 == 0x20 && op4 == 0xfb) { while (Z80.DE.wl > 0 && z80_ICount[0] > 4 + 4 + 12 + 6) { BURNODD(4 + 4 + 12 + 6, 4, 4 + 4 + 12 + 6); Z80.DE.wl--; } } else if (op3 == 0xc2) { byte ad1 = cpu_readop_arg(Z80.PC.d + 3); byte ad2 = cpu_readop_arg(Z80.PC.d + 4); if ((ad1 + 256 * ad2) == (Z80.PC.d - 1)) { while (Z80.DE.wl > 0 && z80_ICount[0] > 4 + 4 + 10 + 6) { BURNODD(4 + 4 + 10 + 6, 4, 4 + 4 + 10 + 6); Z80.DE.wl--; } } } } }; } /* DEC  DE		  */
            void op_1c() { Z80.DE.bl = INC(Z80.DE.bl); } /* INC  E 		  */
            void op_1d() { Z80.DE.bl = DEC(Z80.DE.bl); } /* DEC  E 		  */
            void op_1e() { Z80.DE.bl = ARG(); } /* LD   E,n		  */
            void op_1f()
            {
                {
                    byte res = (byte)((Z80.AF.bh >> 1) | (Z80.AF.bl << 7));
                    byte c = (Z80.AF.bh & 0x01) != 0 ? (byte)0x01 : (byte)0;
                    Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | c | (res & (0x20 | 0x08))); Z80.AF.bh = res;
                };
            } /* RRA			  */

            void op_20() { if ((Z80.AF.bl & 0x40) == 0) { sbyte arg = (sbyte)ARG(); Z80.PC.wl += (ushort)(short)arg; z80_ICount[0] -= 5; change_pc16(Z80.PC.d); } else Z80.PC.wl++; ; } /* JR   NZ,o		  */
            void op_21() { Z80.HL.wl = ARG16(); } /* LD   HL,w		  */
            void op_22() { EA = ARG16(); WM16(EA, Z80.HL); } /* LD   (w),HL	  */
            void op_23() { Z80.HL.wl++; } /* INC  HL		  */
            void op_24() { Z80.HL.bh = INC(Z80.HL.bh); } /* INC  H 		  */
            void op_25() { Z80.HL.bh = DEC(Z80.HL.bh); } /* DEC  H 		  */
            void op_26() { Z80.HL.bh = ARG(); } /* LD   H,n		  */
            void op_27()
            {
                {
                    int idx = Z80.AF.bh; if ((Z80.AF.bl & 0x01) != 0) idx |= 0x100;
                    if ((Z80.AF.bl & 0x10) != 0) idx |= 0x200;
                    if ((Z80.AF.bl & 0x02) != 0)
                        idx |= 0x400; Z80.AF.wl = DAATable[idx];
                };
            } /* DAA			  */

            void op_28() { if ((Z80.AF.bl & 0x40) != 0) { sbyte arg = (sbyte)ARG(); Z80.PC.wl += (ushort)(short)arg; z80_ICount[0] -= 5; change_pc16(Z80.PC.d); } else Z80.PC.wl++; ; } /* JR   Z,o		  */
            void op_29()
            {
                {
                    uint res = Z80.HL.d + Z80.HL.d;
                    Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.HL.d ^ res ^ Z80.HL.d) >> 8) & 0x10) | ((res >> 16) & 0x01));
                    Z80.HL.wl = (ushort)res;
                };
            } /* ADD  HL,HL 	  */
            void op_2a() { EA = ARG16(); RM16(EA, ref Z80.HL); } /* LD   HL,(w)	  */
            void op_2b() { Z80.HL.wl--; if (Z80.HL.wl > 1 && Z80.PC.d < 0xfffc) { byte op1 = cpu_readop(Z80.PC.d); byte op2 = cpu_readop(Z80.PC.d + 1); if ((op1 == 0x7c && op2 == 0xb5) || (op1 == 0x7d && op2 == 0xb4)) { byte op3 = cpu_readop(Z80.PC.d + 2); byte op4 = cpu_readop(Z80.PC.d + 3); if (op3 == 0x20 && op4 == 0xfb) { while (Z80.HL.wl > 0 && z80_ICount[0] > 4 + 4 + 12 + 6) { BURNODD(4 + 4 + 12 + 6, 4, 4 + 4 + 12 + 6); Z80.HL.wl--; } } else if (op3 == 0xc2) { byte ad1 = cpu_readop_arg(Z80.PC.d + 3); byte ad2 = cpu_readop_arg(Z80.PC.d + 4); if ((ad1 + 256 * ad2) == (Z80.PC.d - 1)) { while (Z80.HL.wl > 0 && z80_ICount[0] > 4 + 4 + 10 + 6) { BURNODD(4 + 4 + 10 + 6, 4, 4 + 4 + 10 + 6); Z80.HL.wl--; } } } } }; } /* DEC  HL		  */
            void op_2c() { Z80.HL.bl = INC(Z80.HL.bl); } /* INC  L 		  */
            void op_2d() { Z80.HL.bl = DEC(Z80.HL.bl); } /* DEC  L 		  */
            void op_2e() { Z80.HL.bl = ARG(); } /* LD   L,n		  */
            void op_2f()
            {
                Z80.AF.bh ^= 0xff;
                Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04 | 0x01)) | 0x10 | 0x02 | (Z80.AF.bh & (0x20 | 0x08)));
            } /* CPL			  */

            void op_30() { if ((Z80.AF.bl & 0x01) == 0) { sbyte arg = (sbyte)ARG(); Z80.PC.wl += (ushort)(short)arg; z80_ICount[0] -= 5; change_pc16(Z80.PC.d); } else Z80.PC.wl++; ; } /* JR   NC,o		  */
            void op_31() { Z80.SP.wl = ARG16(); } /* LD   SP,w		  */
            void op_32() { EA = ARG16(); cpu_writemem16((int)EA, Z80.AF.bh); } /* LD   (w),A 	  */
            void op_33() { Z80.SP.wl++; } /* INC  SP		  */
            void op_34() { cpu_writemem16(Z80.HL.wl, INC((byte)cpu_readmem16(Z80.HL.wl))); } /* INC  (HL)		  */
            void op_35() { cpu_writemem16(Z80.HL.wl, DEC((byte)cpu_readmem16(Z80.HL.wl))); } /* DEC  (HL)		  */
            void op_36() { cpu_writemem16(Z80.HL.wl, ARG()); } /* LD   (HL),n	  */
            void op_37() { Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | 0x01 | (Z80.AF.bh & (0x20 | 0x08))); } /* SCF			  */

            void op_38() { if ((Z80.AF.bl & 0x01) != 0) { sbyte arg = (sbyte)ARG(); Z80.PC.wl += (ushort)(short)arg; z80_ICount[0] -= 5; change_pc16(Z80.PC.d); } else Z80.PC.wl++; ; } /* JR   C,o		  */
            void op_39()
            {
                {
                    uint res = Z80.HL.d + Z80.SP.d;
                    Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.HL.d ^ res ^ Z80.SP.d) >> 8) & 0x10) | ((res >> 16) & 0x01));
                    Z80.HL.wl = (ushort)res;
                };
            } /* ADD  HL,SP 	  */
            void op_3a() { EA = ARG16(); Z80.AF.bh = (byte)cpu_readmem16((int)EA); } /* LD   A,(w) 	  */
            void op_3b() { Z80.SP.wl--; } /* DEC  SP		  */
            void op_3c() { Z80.AF.bh = INC(Z80.AF.bh); } /* INC  A 		  */
            void op_3d() { Z80.AF.bh = DEC(Z80.AF.bh); } /* DEC  A 		  */
            void op_3e() { Z80.AF.bh = ARG(); } /* LD   A,n		  */
            void op_3f() { Z80.AF.bl = (byte)(((Z80.AF.bl & (0x80 | 0x40 | 0x04 | 0x01)) | ((Z80.AF.bl & 0x01) << 4) | (Z80.AF.bh & (0x20 | 0x08))) ^ 0x01); } /* CCF			  */
            //OP(op,3f) { _F = ((_F & ~(HF|NF)) | ((_F & CF)<<4)) ^ CF; 		  } /* CCF				*/

            void op_40() { } /* LD   B,B		  */
            void op_41() { Z80.BC.bh = Z80.BC.bl; } /* LD   B,C		  */
            void op_42() { Z80.BC.bh = Z80.DE.bh; } /* LD   B,D		  */
            void op_43() { Z80.BC.bh = Z80.DE.bl; } /* LD   B,E		  */
            void op_44() { Z80.BC.bh = Z80.HL.bh; } /* LD   B,H		  */
            void op_45() { Z80.BC.bh = Z80.HL.bl; } /* LD   B,L		  */
            void op_46() { Z80.BC.bh = (byte)cpu_readmem16(Z80.HL.wl); } /* LD   B,(HL)	  */
            void op_47() { Z80.BC.bh = Z80.AF.bh; } /* LD   B,A		  */

            void op_48() { Z80.BC.bl = Z80.BC.bh; } /* LD   C,B		  */
            void op_49() { } /* LD   C,C		  */
            void op_4a() { Z80.BC.bl = Z80.DE.bh; } /* LD   C,D		  */
            void op_4b() { Z80.BC.bl = Z80.DE.bl; } /* LD   C,E		  */
            void op_4c() { Z80.BC.bl = Z80.HL.bh; } /* LD   C,H		  */
            void op_4d() { Z80.BC.bl = Z80.HL.bl; } /* LD   C,L		  */
            void op_4e() { Z80.BC.bl = (byte)cpu_readmem16(Z80.HL.wl); } /* LD   C,(HL)	  */
            void op_4f() { Z80.BC.bl = Z80.AF.bh; } /* LD   C,A		  */

            void op_50() { Z80.DE.bh = Z80.BC.bh; } /* LD   D,B		  */
            void op_51() { Z80.DE.bh = Z80.BC.bl; } /* LD   D,C		  */
            void op_52() { } /* LD   D,D		  */
            void op_53() { Z80.DE.bh = Z80.DE.bl; } /* LD   D,E		  */
            void op_54() { Z80.DE.bh = Z80.HL.bh; } /* LD   D,H		  */
            void op_55() { Z80.DE.bh = Z80.HL.bl; } /* LD   D,L		  */
            void op_56() { Z80.DE.bh = (byte)cpu_readmem16(Z80.HL.wl); } /* LD   D,(HL)	  */
            void op_57() { Z80.DE.bh = Z80.AF.bh; } /* LD   D,A		  */

            void op_58() { Z80.DE.bl = Z80.BC.bh; } /* LD   E,B		  */
            void op_59() { Z80.DE.bl = Z80.BC.bl; } /* LD   E,C		  */
            void op_5a() { Z80.DE.bl = Z80.DE.bh; } /* LD   E,D		  */
            void op_5b() { } /* LD   E,E		  */
            void op_5c() { Z80.DE.bl = Z80.HL.bh; } /* LD   E,H		  */
            void op_5d() { Z80.DE.bl = Z80.HL.bl; } /* LD   E,L		  */
            void op_5e() { Z80.DE.bl = (byte)cpu_readmem16(Z80.HL.wl); } /* LD   E,(HL)	  */
            void op_5f() { Z80.DE.bl = Z80.AF.bh; } /* LD   E,A		  */

            void op_60() { Z80.HL.bh = Z80.BC.bh; } /* LD   H,B		  */
            void op_61() { Z80.HL.bh = Z80.BC.bl; } /* LD   H,C		  */
            void op_62() { Z80.HL.bh = Z80.DE.bh; } /* LD   H,D		  */
            void op_63() { Z80.HL.bh = Z80.DE.bl; } /* LD   H,E		  */
            void op_64() { } /* LD   H,H		  */
            void op_65() { Z80.HL.bh = Z80.HL.bl; } /* LD   H,L		  */
            void op_66() { Z80.HL.bh = (byte)cpu_readmem16(Z80.HL.wl); } /* LD   H,(HL)	  */
            void op_67() { Z80.HL.bh = Z80.AF.bh; } /* LD   H,A		  */

            void op_68() { Z80.HL.bl = Z80.BC.bh; } /* LD   L,B		  */
            void op_69() { Z80.HL.bl = Z80.BC.bl; } /* LD   L,C		  */
            void op_6a() { Z80.HL.bl = Z80.DE.bh; } /* LD   L,D		  */
            void op_6b() { Z80.HL.bl = Z80.DE.bl; } /* LD   L,E		  */
            void op_6c() { Z80.HL.bl = Z80.HL.bh; } /* LD   L,H		  */
            void op_6d() { } /* LD   L,L		  */
            void op_6e() { Z80.HL.bl = (byte)cpu_readmem16(Z80.HL.wl); } /* LD   L,(HL)	  */
            void op_6f() { Z80.HL.bl = Z80.AF.bh; } /* LD   L,A		  */

            void op_70() { cpu_writemem16(Z80.HL.wl, Z80.BC.bh); } /* LD   (HL),B	  */
            void op_71() { cpu_writemem16(Z80.HL.wl, Z80.BC.bl); } /* LD   (HL),C	  */
            void op_72() { cpu_writemem16(Z80.HL.wl, Z80.DE.bh); } /* LD   (HL),D	  */
            void op_73() { cpu_writemem16(Z80.HL.wl, Z80.DE.bl); } /* LD   (HL),E	  */
            void op_74() { cpu_writemem16(Z80.HL.wl, Z80.HL.bh); } /* LD   (HL),H	  */
            void op_75() { cpu_writemem16(Z80.HL.wl, Z80.HL.bl); } /* LD   (HL),L	  */
            void op_76()
            {
                {
                    Z80.PC.wl--; Z80.HALT = 1; if (!after_EI)
                        if (z80_ICount[0] > 0)
                        {
                            /* NOP takes 4 cycles per instruction */
                            int n = (z80_ICount[0] + 3) / 4;
                            Z80.R += (byte)n;
                            z80_ICount[0] -= 4 * n;
                        }
                }
            } /* HALT			  */
            void op_77() { cpu_writemem16(Z80.HL.wl, Z80.AF.bh); } /* LD   (HL),A	  */

            void op_78() { Z80.AF.bh = Z80.BC.bh; } /* LD   A,B		  */
            void op_79() { Z80.AF.bh = Z80.BC.bl; } /* LD   A,C		  */
            void op_7a() { Z80.AF.bh = Z80.DE.bh; } /* LD   A,D		  */
            void op_7b() { Z80.AF.bh = Z80.DE.bl; } /* LD   A,E		  */
            void op_7c() { Z80.AF.bh = Z80.HL.bh; } /* LD   A,H		  */
            void op_7d() { Z80.AF.bh = Z80.HL.bl; } /* LD   A,L		  */
            void op_7e() { Z80.AF.bh = (byte)cpu_readmem16(Z80.HL.wl); } /* LD   A,(HL)	  */
            void op_7f() { } /* LD   A,A		  */

            void op_80() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.BC.bh); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,B		  */
            void op_81() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.BC.bl); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,C		  */
            void op_82() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.DE.bh); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,D		  */
            void op_83() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.DE.bl); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,E		  */
            void op_84() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.HL.bh); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,H		  */
            void op_85() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.HL.bl); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,L		  */
            void op_86() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + (byte)cpu_readmem16(Z80.HL.wl)); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,(HL)	  */
            void op_87() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.AF.bh); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,A		  */

            void op_88() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.BC.bh + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,B		  */
            void op_89() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.BC.bl + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,C		  */
            void op_8a() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.DE.bh + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,D		  */
            void op_8b() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.DE.bl + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,E		  */
            void op_8c() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.HL.bh + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,H		  */
            void op_8d() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.HL.bl + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,L		  */
            void op_8e() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + (byte)cpu_readmem16(Z80.HL.wl) + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,(HL)	  */
            void op_8f() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.AF.bh + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,A		  */

            void op_90() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.BC.bh); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  B 		  */
            void op_91() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.BC.bl); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  C 		  */
            void op_92() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.DE.bh); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  D 		  */
            void op_93() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.DE.bl); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  E 		  */
            void op_94() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.HL.bh); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  H 		  */
            void op_95() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.HL.bl); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  L 		  */
            void op_96() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16(Z80.HL.wl)); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  (HL)		  */
            void op_97() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.AF.bh); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  A 		  */

            void op_98() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.BC.bh - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,B		  */
            void op_99() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.BC.bl - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,C		  */
            void op_9a() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.DE.bh - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,D		  */
            void op_9b() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.DE.bl - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,E		  */
            void op_9c() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.HL.bh - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,H		  */
            void op_9d() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.HL.bl - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,L		  */
            void op_9e() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16(Z80.HL.wl) - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,(HL)	  */
            void op_9f() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.AF.bh - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,A		  */

            void op_a0() { Z80.AF.bh &= Z80.BC.bh; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  B 		  */
            void op_a1() { Z80.AF.bh &= Z80.BC.bl; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  C 		  */
            void op_a2() { Z80.AF.bh &= Z80.DE.bh; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  D 		  */
            void op_a3() { Z80.AF.bh &= Z80.DE.bl; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  E 		  */
            void op_a4() { Z80.AF.bh &= Z80.HL.bh; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  H 		  */
            void op_a5() { Z80.AF.bh &= Z80.HL.bl; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  L 		  */
            void op_a6() { Z80.AF.bh &= (byte)cpu_readmem16(Z80.HL.wl); Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  (HL)		  */
            void op_a7() { Z80.AF.bh &= Z80.AF.bh; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  A 		  */

            void op_a8() { Z80.AF.bh ^= Z80.BC.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  B 		  */
            void op_a9() { Z80.AF.bh ^= Z80.BC.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  C 		  */
            void op_aa() { Z80.AF.bh ^= Z80.DE.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  D 		  */
            void op_ab() { Z80.AF.bh ^= Z80.DE.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  E 		  */
            void op_ac() { Z80.AF.bh ^= Z80.HL.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  H 		  */
            void op_ad() { Z80.AF.bh ^= Z80.HL.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  L 		  */
            void op_ae() { Z80.AF.bh ^= (byte)cpu_readmem16(Z80.HL.wl); Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  (HL)		  */
            void op_af() { Z80.AF.bh ^= Z80.AF.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  A 		  */

            void op_b0() { Z80.AF.bh |= Z80.BC.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   B 		  */
            void op_b1() { Z80.AF.bh |= Z80.BC.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   C 		  */
            void op_b2() { Z80.AF.bh |= Z80.DE.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   D 		  */
            void op_b3() { Z80.AF.bh |= Z80.DE.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   E 		  */
            void op_b4() { Z80.AF.bh |= Z80.HL.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   H 		  */
            void op_b5() { Z80.AF.bh |= Z80.HL.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   L 		  */
            void op_b6() { Z80.AF.bh |= (byte)cpu_readmem16(Z80.HL.wl); Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   (HL)		  */
            void op_b7() { Z80.AF.bh |= Z80.AF.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   A 		  */

            void op_b8() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.BC.bh); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   B 		  */
            void op_b9() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.BC.bl); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   C 		  */
            void op_ba() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.DE.bh); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   D 		  */
            void op_bb() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.DE.bl); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   E 		  */
            void op_bc() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.HL.bh); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   H 		  */
            void op_bd() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.HL.bl); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   L 		  */
            void op_be() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16(Z80.HL.wl)); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   (HL)		  */
            void op_bf() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.AF.bh); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   A 		  */

            void op_c0() { if ((Z80.AF.bl & 0x40) == 0) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  NZ		  */
            void op_c1() { { RM16(Z80.SP.d, ref Z80.BC); Z80.SP.wl += 2; }; } /* POP  BC		  */
            void op_c2() { if ((Z80.AF.bl & 0x40) == 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   NZ,a		  */
            void op_c3()
            {
                uint oldpc = Z80.PC.d - 1;
                Z80.PC.d = ARG16(); 
                change_pc16(Z80.PC.d);
                //if (Z80.PC.d == oldpc)
                //{
                //    if (!after_EI)
                //        BURNODD(z80_ICount[0], 1, 10);
                //}
                //else
                //{
                //    byte op = cpu_readop(Z80.PC.d);
                //    if (Z80.PC.d == oldpc - 1)
                //    {
                //        if (op == 0x00 || op == 0xfb)/* NOP - JP $-1 or EI - JP $-1 */	
                //        {
                //            if (!after_EI) 
                //                BURNODD(z80_ICount[0] - 4, 2, 4 + 10);
                //        }
                //    }
                //    else if (Z80.PC.d == oldpc - 3 && op == 0x31)/* LD SP,#xxxx - JP $-3 (Galaga) */ 
                //    {
                //        if (!after_EI) 
                //            BURNODD(z80_ICount[0] - 10, 2, 10 + 10);
                //    }
                //}
            }  /* JP   a 		  */
            void op_c4() { if ((Z80.AF.bl & 0x40) == 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL NZ,a		  */
            void op_c5() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.BC); }; } /* PUSH BC		  */
            void op_c6() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + ARG()); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,n		  */
            void op_c7() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x00; change_pc16(Z80.PC.d); } /* RST  0 		  */

            void op_c8() { if ((Z80.AF.bl & 0x40) != 0) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  Z 		  */
            void op_c9() { if (true) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET			  */
            void op_ca() { if ((Z80.AF.bl & 0x40) != 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   Z,a		  */
            void op_cb() { Z80.R++; { uint op = ROP(); z80_ICount[0] -= cc_cb[op]; Z80cb[op](); }; } /* **** CB xx 	  */
            void op_cc() { if ((Z80.AF.bl & 0x40) != 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL Z,a		  */
            void op_cd() { if (true) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL a 		  */
            void op_ce() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + ARG() + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,n		  */
            void op_cf() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x08; change_pc16(Z80.PC.d); } /* RST  1 		  */

            void op_d0() { if ((Z80.AF.bl & 0x01) == 0) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  NC		  */
            void op_d1() { { RM16(Z80.SP.d, ref Z80.DE); Z80.SP.wl += 2; }; } /* POP  DE		  */
            void op_d2() { if ((Z80.AF.bl & 0x01) == 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   NC,a		  */
            void op_d3() { uint n = (uint)(ARG() | (Z80.AF.bh << 8)); cpu_writeport((int)n, Z80.AF.bh); } /* OUT  (n),A 	  */
            void op_d4() { if ((Z80.AF.bl & 0x01) == 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL NC,a		  */
            void op_d5() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.DE); }; } /* PUSH DE		  */
            void op_d6() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - ARG()); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  n 		  */
            void op_d7() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x10; change_pc16(Z80.PC.d); } /* RST  2 		  */

            void op_d8() { if ((Z80.AF.bl & 0x01) != 0) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  C 		  */
            void op_d9() { { PAIR tmp; tmp = Z80.BC; Z80.BC = Z80.BC2; Z80.BC2 = tmp; tmp = Z80.DE; Z80.DE = Z80.DE2; Z80.DE2 = tmp; tmp = Z80.HL; Z80.HL = Z80.HL2; Z80.HL2 = tmp; }; } /* EXX			  */
            void op_da() { if ((Z80.AF.bl & 0x01) != 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   C,a		  */
            void op_db() { uint n = (uint)(ARG() | (Z80.AF.bh << 8)); Z80.AF.bh = ((byte)cpu_readport((int)n)); } /* IN   A,(n) 	  */
            void op_dc() { if ((Z80.AF.bl & 0x01) != 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL C,a		  */
            void op_dd() { Z80.R++; { uint op = ROP(); z80_ICount[0] -= cc_dd[op]; Z80dd[op](); }; } /* **** DD xx 	  */
            void op_de() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - ARG() - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,n		  */
            void op_df() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x18; change_pc16(Z80.PC.d); } /* RST  3 		  */

            void op_e0() { if ((Z80.AF.bl & 0x04) == 0) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  PO		  */
            void op_e1() { { RM16(Z80.SP.d, ref Z80.HL); Z80.SP.wl += 2; }; } /* POP  HL		  */
            void op_e2() { if ((Z80.AF.bl & 0x04) == 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   PO,a		  */
            void op_e3() { { PAIR tmp = new PAIR(); RM16(Z80.SP.d, ref tmp); WM16(Z80.SP.d, Z80.HL); Z80.HL = tmp; }; } /* EX   HL,(SP)	  */
            void op_e4() { if ((Z80.AF.bl & 0x04) == 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL PO,a		  */
            void op_e5() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.HL); }; } /* PUSH HL		  */
            void op_e6() { Z80.AF.bh &= ARG(); Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  n 		  */
            void op_e7() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x20; change_pc16(Z80.PC.d); } /* RST  4 		  */

            void op_e8() { if ((Z80.AF.bl & 0x04) != 0) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  PE		  */
            void op_e9() { Z80.PC.wl = Z80.HL.wl; change_pc16(Z80.PC.d); } /* JP   (HL)		  */
            void op_ea() { if ((Z80.AF.bl & 0x04) != 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   PE,a		  */
            void op_eb() { { PAIR tmp; tmp = Z80.DE; Z80.DE = Z80.HL; Z80.HL = tmp; }; } /* EX   DE,HL 	  */
            void op_ec() { if ((Z80.AF.bl & 0x04) != 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL PE,a		  */
            void op_ed() { Z80.R++; { uint op = ROP(); z80_ICount[0] -= cc_ed[op]; Z80ed[op](); }; } /* **** ED xx 	  */
            void op_ee() { Z80.AF.bh ^= ARG(); Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  n 		  */
            void op_ef() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x28; change_pc16(Z80.PC.d); } /* RST  5 		  */

            void op_f0() { if ((Z80.AF.bl & 0x80) == 0) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  P 		  */
            void op_f1() { { RM16(Z80.SP.d, ref Z80.AF); Z80.SP.wl += 2; }; } /* POP  AF		  */
            void op_f2() { if ((Z80.AF.bl & 0x80) == 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   P,a		  */
            void op_f3() { Z80.IFF1 = Z80.IFF2 = 0; } /* DI 			  */
            void op_f4() { if ((Z80.AF.bl & 0x80) == 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL P,a		  */
            void op_f5() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.AF); }; } /* PUSH AF		  */
            void op_f6() { Z80.AF.bh |= ARG(); Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   n 		  */
            void op_f7() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x30; change_pc16(Z80.PC.d); } /* RST  6 		  */

            void op_f8() { if ((Z80.AF.bl & 0x80) != 0) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; } /* RET  M 		  */
            void op_f9() { Z80.SP.wl = Z80.HL.wl; } /* LD   SP,HL 	  */
            void op_fa() { if ((Z80.AF.bl & 0x80) != 0) { Z80.PC.d = ARG16(); change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* JP   M,a		  */
            void op_fb() { { if (Z80.IFF1 == 0) { Z80.IFF1 = Z80.IFF2 = 1; Z80.PREPC.d = Z80.PC.d; Z80.R++; if (Z80.irq_state != CLEAR_LINE || Z80.request_irq >= 0) { after_EI = true; { uint op = ROP(); z80_ICount[0] -= cc_op[op]; Z80op[op](); }; after_EI = false; take_interrupt(); } else { uint op = ROP(); z80_ICount[0] -= cc_op[op]; Z80op[op](); } } else Z80.IFF2 = 1; }; } /* EI 			  */
            void op_fc() { if ((Z80.AF.bl & 0x80) != 0) { EA = ARG16(); { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = EA; z80_ICount[0] -= 7; change_pc16(Z80.PC.d); } else { Z80.PC.wl += 2; }; } /* CALL M,a		  */
            void op_fd() { Z80.R++; { uint op = ROP(); z80_ICount[0] -= cc_dd[op]; Z80fd[op](); }; } /* **** FD xx 	  */
            void op_fe() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - ARG()); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   n 		  */
            void op_ff() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }; Z80.PC.d = 0x38; change_pc16(Z80.PC.d); } /* RST  7 		  */
            #endregion
            #region cb rotate, shift and bit operation opcodes
            void cb_00() { Z80.BC.bh = RLC(Z80.BC.bh); } /* RLC  B 		  */
            void cb_01() { Z80.BC.bl = RLC(Z80.BC.bl); } /* RLC  C 		  */
            void cb_02() { Z80.DE.bh = RLC(Z80.DE.bh); } /* RLC  D 		  */
            void cb_03() { Z80.DE.bl = RLC(Z80.DE.bl); } /* RLC  E 		  */
            void cb_04() { Z80.HL.bh = RLC(Z80.HL.bh); } /* RLC  H 		  */
            void cb_05() { Z80.HL.bl = RLC(Z80.HL.bl); } /* RLC  L 		  */
            void cb_06() { cpu_writemem16(Z80.HL.wl, RLC((byte)cpu_readmem16(Z80.HL.wl))); } /* RLC  (HL)		  */
            void cb_07() { Z80.AF.bh = RLC(Z80.AF.bh); } /* RLC  A 		  */

            void cb_08() { Z80.BC.bh = RRC(Z80.BC.bh); } /* RRC  B 		  */
            void cb_09() { Z80.BC.bl = RRC(Z80.BC.bl); } /* RRC  C 		  */
            void cb_0a() { Z80.DE.bh = RRC(Z80.DE.bh); } /* RRC  D 		  */
            void cb_0b() { Z80.DE.bl = RRC(Z80.DE.bl); } /* RRC  E 		  */
            void cb_0c() { Z80.HL.bh = RRC(Z80.HL.bh); } /* RRC  H 		  */
            void cb_0d() { Z80.HL.bl = RRC(Z80.HL.bl); } /* RRC  L 		  */
            void cb_0e() { cpu_writemem16(Z80.HL.wl, RRC((byte)cpu_readmem16(Z80.HL.wl))); } /* RRC  (HL)		  */
            void cb_0f() { Z80.AF.bh = RRC(Z80.AF.bh); } /* RRC  A 		  */

            void cb_10() { Z80.BC.bh = RL(Z80.BC.bh); } /* RL   B 		  */
            void cb_11() { Z80.BC.bl = RL(Z80.BC.bl); } /* RL   C 		  */
            void cb_12() { Z80.DE.bh = RL(Z80.DE.bh); } /* RL   D 		  */
            void cb_13() { Z80.DE.bl = RL(Z80.DE.bl); } /* RL   E 		  */
            void cb_14() { Z80.HL.bh = RL(Z80.HL.bh); } /* RL   H 		  */
            void cb_15() { Z80.HL.bl = RL(Z80.HL.bl); } /* RL   L 		  */
            void cb_16() { cpu_writemem16(Z80.HL.wl, RL((byte)cpu_readmem16(Z80.HL.wl))); } /* RL   (HL)		  */
            void cb_17() { Z80.AF.bh = RL(Z80.AF.bh); } /* RL   A 		  */

            void cb_18() { Z80.BC.bh = RR(Z80.BC.bh); } /* RR   B 		  */
            void cb_19() { Z80.BC.bl = RR(Z80.BC.bl); } /* RR   C 		  */
            void cb_1a() { Z80.DE.bh = RR(Z80.DE.bh); } /* RR   D 		  */
            void cb_1b() { Z80.DE.bl = RR(Z80.DE.bl); } /* RR   E 		  */
            void cb_1c() { Z80.HL.bh = RR(Z80.HL.bh); } /* RR   H 		  */
            void cb_1d() { Z80.HL.bl = RR(Z80.HL.bl); } /* RR   L 		  */
            void cb_1e() { cpu_writemem16(Z80.HL.wl, RR((byte)cpu_readmem16(Z80.HL.wl))); } /* RR   (HL)		  */
            void cb_1f() { Z80.AF.bh = RR(Z80.AF.bh); } /* RR   A 		  */

            void cb_20() { Z80.BC.bh = SLA(Z80.BC.bh); } /* SLA  B 		  */
            void cb_21() { Z80.BC.bl = SLA(Z80.BC.bl); } /* SLA  C 		  */
            void cb_22() { Z80.DE.bh = SLA(Z80.DE.bh); } /* SLA  D 		  */
            void cb_23() { Z80.DE.bl = SLA(Z80.DE.bl); } /* SLA  E 		  */
            void cb_24() { Z80.HL.bh = SLA(Z80.HL.bh); } /* SLA  H 		  */
            void cb_25() { Z80.HL.bl = SLA(Z80.HL.bl); } /* SLA  L 		  */
            void cb_26() { cpu_writemem16(Z80.HL.wl, SLA((byte)cpu_readmem16(Z80.HL.wl))); } /* SLA  (HL)		  */
            void cb_27() { Z80.AF.bh = SLA(Z80.AF.bh); } /* SLA  A 		  */

            void cb_28() { Z80.BC.bh = SRA(Z80.BC.bh); } /* SRA  B 		  */
            void cb_29() { Z80.BC.bl = SRA(Z80.BC.bl); } /* SRA  C 		  */
            void cb_2a() { Z80.DE.bh = SRA(Z80.DE.bh); } /* SRA  D 		  */
            void cb_2b() { Z80.DE.bl = SRA(Z80.DE.bl); } /* SRA  E 		  */
            void cb_2c() { Z80.HL.bh = SRA(Z80.HL.bh); } /* SRA  H 		  */
            void cb_2d() { Z80.HL.bl = SRA(Z80.HL.bl); } /* SRA  L 		  */
            void cb_2e() { cpu_writemem16(Z80.HL.wl, SRA((byte)cpu_readmem16(Z80.HL.wl))); } /* SRA  (HL)		  */
            void cb_2f() { Z80.AF.bh = SRA(Z80.AF.bh); } /* SRA  A 		  */

            void cb_30() { Z80.BC.bh = SLL(Z80.BC.bh); } /* SLL  B 		  */
            void cb_31() { Z80.BC.bl = SLL(Z80.BC.bl); } /* SLL  C 		  */
            void cb_32() { Z80.DE.bh = SLL(Z80.DE.bh); } /* SLL  D 		  */
            void cb_33() { Z80.DE.bl = SLL(Z80.DE.bl); } /* SLL  E 		  */
            void cb_34() { Z80.HL.bh = SLL(Z80.HL.bh); } /* SLL  H 		  */
            void cb_35() { Z80.HL.bl = SLL(Z80.HL.bl); } /* SLL  L 		  */
            void cb_36() { cpu_writemem16(Z80.HL.wl, SLL((byte)cpu_readmem16(Z80.HL.wl))); } /* SLL  (HL)		  */
            void cb_37() { Z80.AF.bh = SLL(Z80.AF.bh); } /* SLL  A 		  */

            void cb_38() { Z80.BC.bh = SRL(Z80.BC.bh); } /* SRL  B 		  */
            void cb_39() { Z80.BC.bl = SRL(Z80.BC.bl); } /* SRL  C 		  */
            void cb_3a() { Z80.DE.bh = SRL(Z80.DE.bh); } /* SRL  D 		  */
            void cb_3b() { Z80.DE.bl = SRL(Z80.DE.bl); } /* SRL  E 		  */
            void cb_3c() { Z80.HL.bh = SRL(Z80.HL.bh); } /* SRL  H 		  */
            void cb_3d() { Z80.HL.bl = SRL(Z80.HL.bl); } /* SRL  L 		  */
            void cb_3e() { cpu_writemem16(Z80.HL.wl, SRL((byte)cpu_readmem16(Z80.HL.wl))); } /* SRL  (HL)		  */
            void cb_3f() { Z80.AF.bh = SRL(Z80.AF.bh); } /* SRL  A 		  */

            void cb_40() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 0)]); } /* BIT  0,B		  */
            void cb_41() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 0)]); } /* BIT  0,C		  */
            void cb_42() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 0)]); } /* BIT  0,D		  */
            void cb_43() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 0)]); } /* BIT  0,E		  */
            void cb_44() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 0)]); } /* BIT  0,H		  */
            void cb_45() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 0)]); } /* BIT  0,L		  */
            void cb_46() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 0)]); } /* BIT  0,(HL)	  */
            void cb_47() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 0)]); } /* BIT  0,A		  */

            void cb_48() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 1)]); } /* BIT  1,B		  */
            void cb_49() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 1)]); } /* BIT  1,C		  */
            void cb_4a() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 1)]); } /* BIT  1,D		  */
            void cb_4b() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 1)]); } /* BIT  1,E		  */
            void cb_4c() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 1)]); } /* BIT  1,H		  */
            void cb_4d() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 1)]); } /* BIT  1,L		  */
            void cb_4e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 1)]); } /* BIT  1,(HL)	  */
            void cb_4f() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 1)]); } /* BIT  1,A		  */

            void cb_50() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 2)]); } /* BIT  2,B		  */
            void cb_51() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 2)]); } /* BIT  2,C		  */
            void cb_52() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 2)]); } /* BIT  2,D		  */
            void cb_53() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 2)]); } /* BIT  2,E		  */
            void cb_54() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 2)]); } /* BIT  2,H		  */
            void cb_55() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 2)]); } /* BIT  2,L		  */
            void cb_56() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 2)]); } /* BIT  2,(HL)	  */
            void cb_57() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 2)]); } /* BIT  2,A		  */

            void cb_58() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 3)]); } /* BIT  3,B		  */
            void cb_59() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 3)]); } /* BIT  3,C		  */
            void cb_5a() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 3)]); } /* BIT  3,D		  */
            void cb_5b() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 3)]); } /* BIT  3,E		  */
            void cb_5c() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 3)]); } /* BIT  3,H		  */
            void cb_5d() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 3)]); } /* BIT  3,L		  */
            void cb_5e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 3)]); } /* BIT  3,(HL)	  */
            void cb_5f() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 3)]); } /* BIT  3,A		  */

            void cb_60() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 4)]); } /* BIT  4,B		  */
            void cb_61() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 4)]); } /* BIT  4,C		  */
            void cb_62() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 4)]); } /* BIT  4,D		  */
            void cb_63() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 4)]); } /* BIT  4,E		  */
            void cb_64() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 4)]); } /* BIT  4,H		  */
            void cb_65() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 4)]); } /* BIT  4,L		  */
            void cb_66() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 4)]); } /* BIT  4,(HL)	  */
            void cb_67() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 4)]); } /* BIT  4,A		  */

            void cb_68() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 5)]); } /* BIT  5,B		  */
            void cb_69() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 5)]); } /* BIT  5,C		  */
            void cb_6a() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 5)]); } /* BIT  5,D		  */
            void cb_6b() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 5)]); } /* BIT  5,E		  */
            void cb_6c() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 5)]); } /* BIT  5,H		  */
            void cb_6d() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 5)]); } /* BIT  5,L		  */
            void cb_6e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 5)]); } /* BIT  5,(HL)	  */
            void cb_6f() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 5)]); } /* BIT  5,A		  */

            void cb_70() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 6)]); } /* BIT  6,B		  */
            void cb_71() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 6)]); } /* BIT  6,C		  */
            void cb_72() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 6)]); } /* BIT  6,D		  */
            void cb_73() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 6)]); } /* BIT  6,E		  */
            void cb_74() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 6)]); } /* BIT  6,H		  */
            void cb_75() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 6)]); } /* BIT  6,L		  */
            void cb_76() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 6)]); } /* BIT  6,(HL)	  */
            void cb_77() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 6)]); } /* BIT  6,A		  */

            void cb_78() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bh & (1 << 7)]); } /* BIT  7,B		  */
            void cb_79() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.BC.bl & (1 << 7)]); } /* BIT  7,C		  */
            void cb_7a() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bh & (1 << 7)]); } /* BIT  7,D		  */
            void cb_7b() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.DE.bl & (1 << 7)]); } /* BIT  7,E		  */
            void cb_7c() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bh & (1 << 7)]); } /* BIT  7,H		  */
            void cb_7d() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.HL.bl & (1 << 7)]); } /* BIT  7,L		  */
            void cb_7e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[(byte)cpu_readmem16(Z80.HL.wl) & (1 << 7)]); } /* BIT  7,(HL)	  */
            void cb_7f() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | SZ_BIT[Z80.AF.bh & (1 << 7)]); } /* BIT  7,A		  */

            void cb_80() { Z80.BC.bh = RES(0, Z80.BC.bh); } /* RES  0,B		  */
            void cb_81() { Z80.BC.bl = RES(0, Z80.BC.bl); } /* RES  0,C		  */
            void cb_82() { Z80.DE.bh = RES(0, Z80.DE.bh); } /* RES  0,D		  */
            void cb_83() { Z80.DE.bl = RES(0, Z80.DE.bl); } /* RES  0,E		  */
            void cb_84() { Z80.HL.bh = RES(0, Z80.HL.bh); } /* RES  0,H		  */
            void cb_85() { Z80.HL.bl = RES(0, Z80.HL.bl); } /* RES  0,L		  */
            void cb_86() { cpu_writemem16(Z80.HL.wl, RES(0, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  0,(HL)	  */
            void cb_87() { Z80.AF.bh = RES(0, Z80.AF.bh); } /* RES  0,A		  */

            void cb_88() { Z80.BC.bh = RES(1, Z80.BC.bh); } /* RES  1,B		  */
            void cb_89() { Z80.BC.bl = RES(1, Z80.BC.bl); } /* RES  1,C		  */
            void cb_8a() { Z80.DE.bh = RES(1, Z80.DE.bh); } /* RES  1,D		  */
            void cb_8b() { Z80.DE.bl = RES(1, Z80.DE.bl); } /* RES  1,E		  */
            void cb_8c() { Z80.HL.bh = RES(1, Z80.HL.bh); } /* RES  1,H		  */
            void cb_8d() { Z80.HL.bl = RES(1, Z80.HL.bl); } /* RES  1,L		  */
            void cb_8e() { cpu_writemem16(Z80.HL.wl, RES(1, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  1,(HL)	  */
            void cb_8f() { Z80.AF.bh = RES(1, Z80.AF.bh); } /* RES  1,A		  */

            void cb_90() { Z80.BC.bh = RES(2, Z80.BC.bh); } /* RES  2,B		  */
            void cb_91() { Z80.BC.bl = RES(2, Z80.BC.bl); } /* RES  2,C		  */
            void cb_92() { Z80.DE.bh = RES(2, Z80.DE.bh); } /* RES  2,D		  */
            void cb_93() { Z80.DE.bl = RES(2, Z80.DE.bl); } /* RES  2,E		  */
            void cb_94() { Z80.HL.bh = RES(2, Z80.HL.bh); } /* RES  2,H		  */
            void cb_95() { Z80.HL.bl = RES(2, Z80.HL.bl); } /* RES  2,L		  */
            void cb_96() { cpu_writemem16(Z80.HL.wl, RES(2, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  2,(HL)	  */
            void cb_97() { Z80.AF.bh = RES(2, Z80.AF.bh); } /* RES  2,A		  */

            void cb_98() { Z80.BC.bh = RES(3, Z80.BC.bh); } /* RES  3,B		  */
            void cb_99() { Z80.BC.bl = RES(3, Z80.BC.bl); } /* RES  3,C		  */
            void cb_9a() { Z80.DE.bh = RES(3, Z80.DE.bh); } /* RES  3,D		  */
            void cb_9b() { Z80.DE.bl = RES(3, Z80.DE.bl); } /* RES  3,E		  */
            void cb_9c() { Z80.HL.bh = RES(3, Z80.HL.bh); } /* RES  3,H		  */
            void cb_9d() { Z80.HL.bl = RES(3, Z80.HL.bl); } /* RES  3,L		  */
            void cb_9e() { cpu_writemem16(Z80.HL.wl, RES(3, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  3,(HL)	  */
            void cb_9f() { Z80.AF.bh = RES(3, Z80.AF.bh); } /* RES  3,A		  */

            void cb_a0() { Z80.BC.bh = RES(4, Z80.BC.bh); } /* RES  4,B		  */
            void cb_a1() { Z80.BC.bl = RES(4, Z80.BC.bl); } /* RES  4,C		  */
            void cb_a2() { Z80.DE.bh = RES(4, Z80.DE.bh); } /* RES  4,D		  */
            void cb_a3() { Z80.DE.bl = RES(4, Z80.DE.bl); } /* RES  4,E		  */
            void cb_a4() { Z80.HL.bh = RES(4, Z80.HL.bh); } /* RES  4,H		  */
            void cb_a5() { Z80.HL.bl = RES(4, Z80.HL.bl); } /* RES  4,L		  */
            void cb_a6() { cpu_writemem16(Z80.HL.wl, RES(4, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  4,(HL)	  */
            void cb_a7() { Z80.AF.bh = RES(4, Z80.AF.bh); } /* RES  4,A		  */

            void cb_a8() { Z80.BC.bh = RES(5, Z80.BC.bh); } /* RES  5,B		  */
            void cb_a9() { Z80.BC.bl = RES(5, Z80.BC.bl); } /* RES  5,C		  */
            void cb_aa() { Z80.DE.bh = RES(5, Z80.DE.bh); } /* RES  5,D		  */
            void cb_ab() { Z80.DE.bl = RES(5, Z80.DE.bl); } /* RES  5,E		  */
            void cb_ac() { Z80.HL.bh = RES(5, Z80.HL.bh); } /* RES  5,H		  */
            void cb_ad() { Z80.HL.bl = RES(5, Z80.HL.bl); } /* RES  5,L		  */
            void cb_ae() { cpu_writemem16(Z80.HL.wl, RES(5, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  5,(HL)	  */
            void cb_af() { Z80.AF.bh = RES(5, Z80.AF.bh); } /* RES  5,A		  */

            void cb_b0() { Z80.BC.bh = RES(6, Z80.BC.bh); } /* RES  6,B		  */
            void cb_b1() { Z80.BC.bl = RES(6, Z80.BC.bl); } /* RES  6,C		  */
            void cb_b2() { Z80.DE.bh = RES(6, Z80.DE.bh); } /* RES  6,D		  */
            void cb_b3() { Z80.DE.bl = RES(6, Z80.DE.bl); } /* RES  6,E		  */
            void cb_b4() { Z80.HL.bh = RES(6, Z80.HL.bh); } /* RES  6,H		  */
            void cb_b5() { Z80.HL.bl = RES(6, Z80.HL.bl); } /* RES  6,L		  */
            void cb_b6() { cpu_writemem16(Z80.HL.wl, RES(6, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  6,(HL)	  */
            void cb_b7() { Z80.AF.bh = RES(6, Z80.AF.bh); } /* RES  6,A		  */

            void cb_b8() { Z80.BC.bh = RES(7, Z80.BC.bh); } /* RES  7,B		  */
            void cb_b9() { Z80.BC.bl = RES(7, Z80.BC.bl); } /* RES  7,C		  */
            void cb_ba() { Z80.DE.bh = RES(7, Z80.DE.bh); } /* RES  7,D		  */
            void cb_bb() { Z80.DE.bl = RES(7, Z80.DE.bl); } /* RES  7,E		  */
            void cb_bc() { Z80.HL.bh = RES(7, Z80.HL.bh); } /* RES  7,H		  */
            void cb_bd() { Z80.HL.bl = RES(7, Z80.HL.bl); } /* RES  7,L		  */
            void cb_be() { cpu_writemem16(Z80.HL.wl, RES(7, (byte)cpu_readmem16(Z80.HL.wl))); } /* RES  7,(HL)	  */
            void cb_bf() { Z80.AF.bh = RES(7, Z80.AF.bh); } /* RES  7,A		  */

            void cb_c0() { Z80.BC.bh = SET(0, Z80.BC.bh); } /* SET  0,B		  */
            void cb_c1() { Z80.BC.bl = SET(0, Z80.BC.bl); } /* SET  0,C		  */
            void cb_c2() { Z80.DE.bh = SET(0, Z80.DE.bh); } /* SET  0,D		  */
            void cb_c3() { Z80.DE.bl = SET(0, Z80.DE.bl); } /* SET  0,E		  */
            void cb_c4() { Z80.HL.bh = SET(0, Z80.HL.bh); } /* SET  0,H		  */
            void cb_c5() { Z80.HL.bl = SET(0, Z80.HL.bl); } /* SET  0,L		  */
            void cb_c6() { cpu_writemem16(Z80.HL.wl, SET(0, (byte)cpu_readmem16(Z80.HL.wl))); } /* SET  0,(HL)	  */
            void cb_c7() { Z80.AF.bh = SET(0, Z80.AF.bh); } /* SET  0,A		  */

            void cb_c8() { Z80.BC.bh = SET(1, Z80.BC.bh); } /* SET  1,B		  */
            void cb_c9() { Z80.BC.bl = SET(1, Z80.BC.bl); } /* SET  1,C		  */
            void cb_ca() { Z80.DE.bh = SET(1, Z80.DE.bh); } /* SET  1,D		  */
            void cb_cb() { Z80.DE.bl = SET(1, Z80.DE.bl); } /* SET  1,E		  */
            void cb_cc() { Z80.HL.bh = SET(1, Z80.HL.bh); } /* SET  1,H		  */
            void cb_cd() { Z80.HL.bl = SET(1, Z80.HL.bl); } /* SET  1,L		  */
            void cb_ce() { cpu_writemem16(Z80.HL.wl, SET(1, (byte)cpu_readmem16(Z80.HL.wl))); } /* SET  1,(HL)	  */
            void cb_cf() { Z80.AF.bh = SET(1, Z80.AF.bh); } /* SET  1,A		  */

            void cb_d0() { Z80.BC.bh = SET(2, Z80.BC.bh); } /* SET  2,B		  */
            void cb_d1() { Z80.BC.bl = SET(2, Z80.BC.bl); } /* SET  2,C		  */
            void cb_d2() { Z80.DE.bh = SET(2, Z80.DE.bh); } /* SET  2,D		  */
            void cb_d3() { Z80.DE.bl = SET(2, Z80.DE.bl); } /* SET  2,E		  */
            void cb_d4() { Z80.HL.bh = SET(2, Z80.HL.bh); } /* SET  2,H		  */
            void cb_d5() { Z80.HL.bl = SET(2, Z80.HL.bl); } /* SET  2,L		  */
            void cb_d6() { cpu_writemem16(Z80.HL.wl, SET(2, (byte)cpu_readmem16(Z80.HL.wl))); }/* SET  2,(HL) 	 */
            void cb_d7() { Z80.AF.bh = SET(2, Z80.AF.bh); } /* SET  2,A		  */

            void cb_d8() { Z80.BC.bh = SET(3, Z80.BC.bh); } /* SET  3,B		  */
            void cb_d9() { Z80.BC.bl = SET(3, Z80.BC.bl); } /* SET  3,C		  */
            void cb_da() { Z80.DE.bh = SET(3, Z80.DE.bh); } /* SET  3,D		  */
            void cb_db() { Z80.DE.bl = SET(3, Z80.DE.bl); } /* SET  3,E		  */
            void cb_dc() { Z80.HL.bh = SET(3, Z80.HL.bh); } /* SET  3,H		  */
            void cb_dd() { Z80.HL.bl = SET(3, Z80.HL.bl); } /* SET  3,L		  */
            void cb_de() { cpu_writemem16(Z80.HL.wl, SET(3, (byte)cpu_readmem16(Z80.HL.wl))); } /* SET  3,(HL)	  */
            void cb_df() { Z80.AF.bh = SET(3, Z80.AF.bh); } /* SET  3,A		  */

            void cb_e0() { Z80.BC.bh = SET(4, Z80.BC.bh); } /* SET  4,B		  */
            void cb_e1() { Z80.BC.bl = SET(4, Z80.BC.bl); } /* SET  4,C		  */
            void cb_e2() { Z80.DE.bh = SET(4, Z80.DE.bh); } /* SET  4,D		  */
            void cb_e3() { Z80.DE.bl = SET(4, Z80.DE.bl); } /* SET  4,E		  */
            void cb_e4() { Z80.HL.bh = SET(4, Z80.HL.bh); } /* SET  4,H		  */
            void cb_e5() { Z80.HL.bl = SET(4, Z80.HL.bl); } /* SET  4,L		  */
            void cb_e6() { cpu_writemem16(Z80.HL.wl, SET(4, (byte)cpu_readmem16(Z80.HL.wl))); } /* SET  4,(HL)	  */
            void cb_e7() { Z80.AF.bh = SET(4, Z80.AF.bh); } /* SET  4,A		  */

            void cb_e8() { Z80.BC.bh = SET(5, Z80.BC.bh); } /* SET  5,B		  */
            void cb_e9() { Z80.BC.bl = SET(5, Z80.BC.bl); } /* SET  5,C		  */
            void cb_ea() { Z80.DE.bh = SET(5, Z80.DE.bh); } /* SET  5,D		  */
            void cb_eb() { Z80.DE.bl = SET(5, Z80.DE.bl); } /* SET  5,E		  */
            void cb_ec() { Z80.HL.bh = SET(5, Z80.HL.bh); } /* SET  5,H		  */
            void cb_ed() { Z80.HL.bl = SET(5, Z80.HL.bl); } /* SET  5,L		  */
            void cb_ee() { cpu_writemem16(Z80.HL.wl, SET(5, (byte)cpu_readmem16(Z80.HL.wl))); } /* SET  5,(HL)	  */
            void cb_ef() { Z80.AF.bh = SET(5, Z80.AF.bh); } /* SET  5,A		  */

            void cb_f0() { Z80.BC.bh = SET(6, Z80.BC.bh); } /* SET  6,B		  */
            void cb_f1() { Z80.BC.bl = SET(6, Z80.BC.bl); } /* SET  6,C		  */
            void cb_f2() { Z80.DE.bh = SET(6, Z80.DE.bh); } /* SET  6,D		  */
            void cb_f3() { Z80.DE.bl = SET(6, Z80.DE.bl); } /* SET  6,E		  */
            void cb_f4() { Z80.HL.bh = SET(6, Z80.HL.bh); } /* SET  6,H		  */
            void cb_f5() { Z80.HL.bl = SET(6, Z80.HL.bl); } /* SET  6,L		  */
            void cb_f6() { cpu_writemem16(Z80.HL.wl, SET(6, (byte)cpu_readmem16(Z80.HL.wl))); } /* SET  6,(HL)	  */
            void cb_f7() { Z80.AF.bh = SET(6, Z80.AF.bh); } /* SET  6,A		  */

            void cb_f8() { Z80.BC.bh = SET(7, Z80.BC.bh); } /* SET  7,B		  */
            void cb_f9() { Z80.BC.bl = SET(7, Z80.BC.bl); } /* SET  7,C		  */
            void cb_fa() { Z80.DE.bh = SET(7, Z80.DE.bh); } /* SET  7,D		  */
            void cb_fb() { Z80.DE.bl = SET(7, Z80.DE.bl); } /* SET  7,E		  */
            void cb_fc() { Z80.HL.bh = SET(7, Z80.HL.bh); } /* SET  7,H		  */
            void cb_fd() { Z80.HL.bl = SET(7, Z80.HL.bl); } /* SET  7,L		  */
            void cb_fe() { cpu_writemem16(Z80.HL.wl, SET(7, (byte)cpu_readmem16(Z80.HL.wl))); } /* SET  7,(HL)	  */
            void cb_ff() { Z80.AF.bh = SET(7, Z80.AF.bh); } /* SET  7,A		  */
            #endregion
            #region ed special opcodes
            void ed_00() { illegal_2(); } /* DB   ED		  */
            void ed_01() { illegal_2(); } /* DB   ED		  */
            void ed_02() { illegal_2(); } /* DB   ED		  */
            void ed_03() { illegal_2(); } /* DB   ED		  */
            void ed_04() { illegal_2(); } /* DB   ED		  */
            void ed_05() { illegal_2(); } /* DB   ED		  */
            void ed_06() { illegal_2(); } /* DB   ED		  */
            void ed_07() { illegal_2(); } /* DB   ED		  */

            void ed_08() { illegal_2(); } /* DB   ED		  */
            void ed_09() { illegal_2(); } /* DB   ED		  */
            void ed_0a() { illegal_2(); } /* DB   ED		  */
            void ed_0b() { illegal_2(); } /* DB   ED		  */
            void ed_0c() { illegal_2(); } /* DB   ED		  */
            void ed_0d() { illegal_2(); } /* DB   ED		  */
            void ed_0e() { illegal_2(); } /* DB   ED		  */
            void ed_0f() { illegal_2(); } /* DB   ED		  */

            void ed_10() { illegal_2(); } /* DB   ED		  */
            void ed_11() { illegal_2(); } /* DB   ED		  */
            void ed_12() { illegal_2(); } /* DB   ED		  */
            void ed_13() { illegal_2(); } /* DB   ED		  */
            void ed_14() { illegal_2(); } /* DB   ED		  */
            void ed_15() { illegal_2(); } /* DB   ED		  */
            void ed_16() { illegal_2(); } /* DB   ED		  */
            void ed_17() { illegal_2(); } /* DB   ED		  */

            void ed_18() { illegal_2(); } /* DB   ED		  */
            void ed_19() { illegal_2(); } /* DB   ED		  */
            void ed_1a() { illegal_2(); } /* DB   ED		  */
            void ed_1b() { illegal_2(); } /* DB   ED		  */
            void ed_1c() { illegal_2(); } /* DB   ED		  */
            void ed_1d() { illegal_2(); } /* DB   ED		  */
            void ed_1e() { illegal_2(); } /* DB   ED		  */
            void ed_1f() { illegal_2(); } /* DB   ED		  */

            void ed_20() { illegal_2(); } /* DB   ED		  */
            void ed_21() { illegal_2(); } /* DB   ED		  */
            void ed_22() { illegal_2(); } /* DB   ED		  */
            void ed_23() { illegal_2(); } /* DB   ED		  */
            void ed_24() { illegal_2(); } /* DB   ED		  */
            void ed_25() { illegal_2(); } /* DB   ED		  */
            void ed_26() { illegal_2(); } /* DB   ED		  */
            void ed_27() { illegal_2(); } /* DB   ED		  */

            void ed_28() { illegal_2(); } /* DB   ED		  */
            void ed_29() { illegal_2(); } /* DB   ED		  */
            void ed_2a() { illegal_2(); } /* DB   ED		  */
            void ed_2b() { illegal_2(); } /* DB   ED		  */
            void ed_2c() { illegal_2(); } /* DB   ED		  */
            void ed_2d() { illegal_2(); } /* DB   ED		  */
            void ed_2e() { illegal_2(); } /* DB   ED		  */
            void ed_2f() { illegal_2(); } /* DB   ED		  */

            void ed_30() { illegal_2(); } /* DB   ED		  */
            void ed_31() { illegal_2(); } /* DB   ED		  */
            void ed_32() { illegal_2(); } /* DB   ED		  */
            void ed_33() { illegal_2(); } /* DB   ED		  */
            void ed_34() { illegal_2(); } /* DB   ED		  */
            void ed_35() { illegal_2(); } /* DB   ED		  */
            void ed_36() { illegal_2(); } /* DB   ED		  */
            void ed_37() { illegal_2(); } /* DB   ED		  */

            void ed_38() { illegal_2(); } /* DB   ED		  */
            void ed_39() { illegal_2(); } /* DB   ED		  */
            void ed_3a() { illegal_2(); } /* DB   ED		  */
            void ed_3b() { illegal_2(); } /* DB   ED		  */
            void ed_3c() { illegal_2(); } /* DB   ED		  */
            void ed_3d() { illegal_2(); } /* DB   ED		  */
            void ed_3e() { illegal_2(); } /* DB   ED		  */
            void ed_3f() { illegal_2(); } /* DB   ED		  */

            void ed_40() { Z80.BC.bh = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.BC.bh]); } /* IN   B,(C) 	  */
            void ed_41() { cpu_writeport(Z80.BC.wl, Z80.BC.bh); } /* OUT  (C),B 	  */
            void ed_42() { { uint res = (uint)(Z80.HL.d - Z80.BC.d - (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.BC.d) >> 8) & 0x10) | 0x02 | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.BC.d ^ Z80.HL.d) & (Z80.HL.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* SBC  HL,BC 	  */
            void ed_43() { EA = ARG16(); WM16(EA, Z80.BC); } /* LD   (w),BC	  */
            void ed_44() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_45() { { ; if (true) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (Z80.IFF1 == 0 && Z80.IFF2 == 1) { Z80.IFF1 = 1; if (Z80.irq_state != CLEAR_LINE || Z80.request_irq >= 0) { ; take_interrupt(); } } else Z80.IFF1 = Z80.IFF2; }; } /* RETN;			  */
            void ed_46() { Z80.IM = 0; } /* IM   0 		  */
            void ed_47() { { Z80.I = Z80.AF.bh; }; } /* LD   I,A		  */

            void ed_48() { Z80.BC.bl = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.BC.bl]); } /* IN   C,(C) 	  */
            void ed_49() { cpu_writeport(Z80.BC.wl, Z80.BC.bl); } /* OUT  (C),C 	  */
            void ed_4a() { { uint res = (uint)(Z80.HL.d + Z80.BC.d + (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.BC.d) >> 8) & 0x10) | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.BC.d ^ Z80.HL.d ^ 0x8000) & (Z80.BC.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* ADC  HL,BC 	  */
            void ed_4b() { EA = ARG16(); RM16(EA, ref Z80.BC); } /* LD   BC,(w)	  */
            void ed_4c() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_4d() { { int device = Z80.service_irq; if (true) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (device >= 0) { ; Z80.irq[device].interrupt_reti(Z80.irq[device].irq_param); } }; } /* RETI			  */
            void ed_4e() { Z80.IM = 0; } /* IM   0 		  */
            void ed_4f() { { Z80.R = Z80.AF.bh; Z80.R2 = (byte)(Z80.AF.bh & 0x80); }; } /* LD   R,A		  */

            void ed_50() { Z80.DE.bh = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.DE.bh]); } /* IN   D,(C) 	  */
            void ed_51() { cpu_writeport(Z80.BC.wl, Z80.DE.bh); } /* OUT  (C),D 	  */
            void ed_52() { { uint res = (uint)(Z80.HL.d - Z80.DE.d - (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.DE.d) >> 8) & 0x10) | 0x02 | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.DE.d ^ Z80.HL.d) & (Z80.HL.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* SBC  HL,DE 	  */
            void ed_53() { EA = ARG16(); WM16(EA, Z80.DE); } /* LD   (w),DE	  */
            void ed_54() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_55() { { ; if (true) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (Z80.IFF1 == 0 && Z80.IFF2 == 1) { Z80.IFF1 = 1; if (Z80.irq_state != CLEAR_LINE || Z80.request_irq >= 0) { take_interrupt(); } } else Z80.IFF1 = Z80.IFF2; }; } /* RETN;			  */
            void ed_56() { Z80.IM = 1; } /* IM   1 		  */
            void ed_57() { { Z80.AF.bh = Z80.I; Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZ[Z80.AF.bh] | (Z80.IFF2 << 2)); }; } /* LD   A,I		  */

            void ed_58() { Z80.DE.bl = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.DE.bl]); } /* IN   E,(C) 	  */
            void ed_59() { cpu_writeport(Z80.BC.wl, Z80.DE.bl); } /* OUT  (C),E 	  */
            void ed_5a() { { uint res = (uint)(Z80.HL.d + Z80.DE.d + (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.DE.d) >> 8) & 0x10) | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.DE.d ^ Z80.HL.d ^ 0x8000) & (Z80.DE.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* ADC  HL,DE 	  */
            void ed_5b() { EA = ARG16(); RM16(EA, ref  Z80.DE); } /* LD   DE,(w)	  */
            void ed_5c() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_5d() { { int device = Z80.service_irq; if (true) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (device >= 0) { ; Z80.irq[device].interrupt_reti(Z80.irq[device].irq_param); } }; } /* RETI			  */
            void ed_5e() { Z80.IM = 2; } /* IM   2 		  */
            void ed_5f() { { Z80.AF.bh = (byte)((Z80.R & 0x7f) | Z80.R2); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZ[Z80.AF.bh] | (Z80.IFF2 << 2)); } } /* LD   A,R		  */

            void ed_60() { Z80.HL.bh = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.HL.bh]); } /* IN   H,(C) 	  */
            void ed_61() { cpu_writeport(Z80.BC.wl, Z80.HL.bh); } /* OUT  (C),H 	  */
            void ed_62() { { uint res = (uint)(Z80.HL.d - Z80.HL.d - (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.HL.d) >> 8) & 0x10) | 0x02 | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.HL.d ^ Z80.HL.d) & (Z80.HL.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* SBC  HL,HL 	  */
            void ed_63() { EA = ARG16(); WM16(EA, Z80.HL); } /* LD   (w),HL	  */
            void ed_64() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_65() { { ; if (true) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (Z80.IFF1 == 0 && Z80.IFF2 == 1) { Z80.IFF1 = 1; if (Z80.irq_state != CLEAR_LINE || Z80.request_irq >= 0) { take_interrupt(); } } else Z80.IFF1 = Z80.IFF2; }; } /* RETN;			  */
            void ed_66() { Z80.IM = 0; } /* IM   0 		  */
            void ed_67() { { byte n = (byte)cpu_readmem16(Z80.HL.wl); cpu_writemem16(Z80.HL.wl, (n >> 4) | (Z80.AF.bh << 4)); Z80.AF.bh = (byte)((Z80.AF.bh & 0xf0) | (n & 0x0f)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.AF.bh]); }; } /* RRD  (HL)		  */

            void ed_68() { Z80.HL.bl = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.HL.bl]); } /* IN   L,(C) 	  */
            void ed_69() { cpu_writeport(Z80.BC.wl, Z80.HL.bl); } /* OUT  (C),L 	  */
            void ed_6a() { { uint res = (uint)(Z80.HL.d + Z80.HL.d + (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.HL.d) >> 8) & 0x10) | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.HL.d ^ Z80.HL.d ^ 0x8000) & (Z80.HL.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* ADC  HL,HL 	  */
            void ed_6b() { EA = ARG16(); RM16(EA, ref Z80.HL); } /* LD   HL,(w)	  */
            void ed_6c() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_6d() { { int device = Z80.service_irq; if (true) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (device >= 0) { ; Z80.irq[device].interrupt_reti(Z80.irq[device].irq_param); } }; } /* RETI			  */
            void ed_6e() { Z80.IM = 0; } /* IM   0 		  */
            void ed_6f() { { byte n = (byte)cpu_readmem16(Z80.HL.wl); cpu_writemem16(Z80.HL.wl, (n << 4) | (Z80.AF.bh & 0x0f)); Z80.AF.bh = (byte)((Z80.AF.bh & 0xf0) | (n >> 4)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.AF.bh]); }; } /* RLD  (HL)		  */

            void ed_70() { byte res = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[res]); } /* IN   0,(C) 	  */
            void ed_71() { cpu_writeport(Z80.BC.wl, 0); } /* OUT  (C),0 	  */
            void ed_72() { { uint res = (uint)(Z80.HL.d - Z80.SP.d - (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.SP.d) >> 8) & 0x10) | 0x02 | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.SP.d ^ Z80.HL.d) & (Z80.HL.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* SBC  HL,SP 	  */
            void ed_73() { EA = ARG16(); WM16(EA, Z80.SP); } /* LD   (w),SP	  */
            void ed_74() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_75() { { ; if (true) { { RM16(Z80.SP.d, ref  Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (Z80.IFF1 == 0 && Z80.IFF2 == 1) { Z80.IFF1 = 1; if (Z80.irq_state != CLEAR_LINE || Z80.request_irq >= 0) { take_interrupt(); } } else Z80.IFF1 = Z80.IFF2; }; } /* RETN;			  */
            void ed_76() { Z80.IM = 1; } /* IM   1 		  */
            void ed_77() { illegal_2(); } /* DB   ED,77 	  */

            void ed_78() { Z80.AF.bh = ((byte)cpu_readport(Z80.BC.wl)); Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | SZP[Z80.AF.bh]); } /* IN   E,(C) 	  */
            void ed_79() { cpu_writeport(Z80.BC.wl, Z80.AF.bh); } /* OUT  (C),E 	  */
            void ed_7a() { { uint res = (uint)(Z80.HL.d + Z80.SP.d + (Z80.AF.bl & 0x01)); Z80.AF.bl = (byte)((((Z80.HL.d ^ res ^ Z80.SP.d) >> 8) & 0x10) | ((res >> 16) & 0x01) | ((res >> 8) & 0x80) | ((res & 0xffff) != 0 ? 0 : 0x40) | (((Z80.SP.d ^ Z80.HL.d ^ 0x8000) & (Z80.SP.d ^ res) & 0x8000) >> 13)); Z80.HL.wl = (ushort)res; }; } /* ADC  HL,SP 	  */
            void ed_7b() { EA = ARG16(); RM16(EA, ref Z80.SP); } /* LD   SP,(w)	  */
            void ed_7c() { { byte value = Z80.AF.bh; Z80.AF.bh = 0; { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - value); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; }; } /* NEG			  */
            void ed_7d() { { int device = Z80.service_irq; if (true) { { RM16(Z80.SP.d, ref Z80.PC); Z80.SP.wl += 2; }; change_pc16(Z80.PC.d); z80_ICount[0] -= 6; }; if (device >= 0) { ; Z80.irq[device].interrupt_reti(Z80.irq[device].irq_param); } }; } /* RETI			  */
            void ed_7e() { Z80.IM = 2; } /* IM   2 		  */
            void ed_7f() { illegal_2(); } /* DB   ED,7F 	  */

            void ed_80() { illegal_2(); } /* DB   ED		  */
            void ed_81() { illegal_2(); } /* DB   ED		  */
            void ed_82() { illegal_2(); } /* DB   ED		  */
            void ed_83() { illegal_2(); } /* DB   ED		  */
            void ed_84() { illegal_2(); } /* DB   ED		  */
            void ed_85() { illegal_2(); } /* DB   ED		  */
            void ed_86() { illegal_2(); } /* DB   ED		  */
            void ed_87() { illegal_2(); } /* DB   ED		  */

            void ed_88() { illegal_2(); } /* DB   ED		  */
            void ed_89() { illegal_2(); } /* DB   ED		  */
            void ed_8a() { illegal_2(); } /* DB   ED		  */
            void ed_8b() { illegal_2(); } /* DB   ED		  */
            void ed_8c() { illegal_2(); } /* DB   ED		  */
            void ed_8d() { illegal_2(); } /* DB   ED		  */
            void ed_8e() { illegal_2(); } /* DB   ED		  */
            void ed_8f() { illegal_2(); } /* DB   ED		  */

            void ed_90() { illegal_2(); } /* DB   ED		  */
            void ed_91() { illegal_2(); } /* DB   ED		  */
            void ed_92() { illegal_2(); } /* DB   ED		  */
            void ed_93() { illegal_2(); } /* DB   ED		  */
            void ed_94() { illegal_2(); } /* DB   ED		  */
            void ed_95() { illegal_2(); } /* DB   ED		  */
            void ed_96() { illegal_2(); } /* DB   ED		  */
            void ed_97() { illegal_2(); } /* DB   ED		  */

            void ed_98() { illegal_2(); } /* DB   ED		  */
            void ed_99() { illegal_2(); } /* DB   ED		  */
            void ed_9a() { illegal_2(); } /* DB   ED		  */
            void ed_9b() { illegal_2(); } /* DB   ED		  */
            void ed_9c() { illegal_2(); } /* DB   ED		  */
            void ed_9d() { illegal_2(); } /* DB   ED		  */
            void ed_9e() { illegal_2(); } /* DB   ED		  */
            void ed_9f() { illegal_2(); } /* DB   ED		  */

            void ed_a0()
            {
                {
                    byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writemem16(Z80.DE.wl, io); Z80.AF.bl &= 0x80 | 0x40 | 0x01;
                    if (((Z80.AF.bh + io) & 0x02) != 0) Z80.AF.bl |= 0x20;
                    if (((Z80.AF.bh + io) & 0x08) != 0) Z80.AF.bl |= 0x08; Z80.HL.wl++; Z80.DE.wl++; Z80.BC.wl--; if (Z80.BC.wl != 0) Z80.AF.bl |= 0x04;
                };
            } /* LDI			  */
            void ed_a1()
            {
                {
                    byte val = (byte)cpu_readmem16(Z80.HL.wl); byte res = (byte)(Z80.AF.bh - val); Z80.HL.wl++; Z80.BC.wl--;
                    Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | (SZ[res] & ~(0x20 | 0x08)) | ((Z80.AF.bh ^ val ^ res) & 0x10) | 0x02);
                    if ((Z80.AF.bl & 0x10) != 0) res -= 1; if ((res & 0x02) != 0) Z80.AF.bl |= 0x20;
                    if ((res & 0x08) != 0) Z80.AF.bl |= 0x08; if ((Z80.BC.wl) != 0) Z80.AF.bl |= 0x04;
                };
            } /* CPI			  */
            void ed_a2() { { byte io = ((byte)cpu_readport(Z80.BC.wl)); Z80.BC.bh--; cpu_writemem16(Z80.HL.wl, io); Z80.HL.wl++; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0) Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io + 1) & 0x100) != 0) Z80.AF.bl |= 0x10 | 0x01; if (((irep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0) Z80.AF.bl |= 0x04; }; } /* INI			  */
            void ed_a3() { { byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writeport(Z80.BC.wl, io); Z80.BC.bh--; Z80.HL.wl++; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0) Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io + 1) & 0x100) != 0) Z80.AF.bl |= 0x10 | 0x01; if (((irep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0) Z80.AF.bl |= 0x04; }; } /* OUTI			  */
            void ed_a4() { illegal_2(); } /* DB   ED		  */
            void ed_a5() { illegal_2(); } /* DB   ED		  */
            void ed_a6() { illegal_2(); } /* DB   ED		  */
            void ed_a7() { illegal_2(); } /* DB   ED		  */

            void ed_a8()
            {
                {
                    byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writemem16(Z80.DE.wl, io); Z80.AF.bl &= 0x80 | 0x40 | 0x01;
                    if (((Z80.AF.bh + io) & 0x02) != 0) Z80.AF.bl |= 0x20;
                    if (((Z80.AF.bh + io) & 0x08) != 0) Z80.AF.bl |= 0x08; Z80.HL.wl--; Z80.DE.wl--; Z80.BC.wl--; if (Z80.BC.wl != 0) Z80.AF.bl |= 0x04;
                };
            } /* LDD			  */
            void ed_a9()
            {
                {
                    byte val = (byte)cpu_readmem16(Z80.HL.wl); byte res = (byte)(Z80.AF.bh - val); Z80.HL.wl--; Z80.BC.wl--;
                    Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | (SZ[res] & ~(0x20 | 0x08)) | ((Z80.AF.bh ^ val ^ res) & 0x10) | 0x02); if ((Z80.AF.bl & 0x10) != 0) res -= 1; if ((res & 0x02) != 0) Z80.AF.bl |= 0x20; if ((res & 0x08) != 0) Z80.AF.bl |= 0x08; if (Z80.BC.wl != 0) Z80.AF.bl |= 0x04;
                };
            } /* CPD			  */
            void ed_aa() { { byte io = ((byte)cpu_readport(Z80.BC.wl)); Z80.BC.bh--; cpu_writemem16(Z80.HL.wl, io); Z80.HL.wl--; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0) Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io - 1) & 0x100) != 0) Z80.AF.bl |= 0x10 | 0x01; if (((drep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0)Z80.AF.bl |= 0x04; }; } /* IND			  */
            void ed_ab() { { byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writeport(Z80.BC.wl, io); Z80.BC.bh--; Z80.HL.wl--; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0) Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io - 1) & 0x100) != 0) Z80.AF.bl |= 0x10 | 0x01; if (((drep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0) Z80.AF.bl |= 0x04; }; } /* OUTD			  */
            void ed_ac() { illegal_2(); } /* DB   ED		  */
            void ed_ad() { illegal_2(); } /* DB   ED		  */
            void ed_ae() { illegal_2(); } /* DB   ED		  */
            void ed_af() { illegal_2(); } /* DB   ED		  */

            void ed_b0() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writemem16(Z80.DE.wl, io); Z80.AF.bl &= 0x80 | 0x40 | 0x01; if (((Z80.AF.bh + io) & 0x02) != 0) Z80.AF.bl |= 0x20; if (((Z80.AF.bh + io) & 0x08) != 0) Z80.AF.bl |= 0x08; Z80.HL.wl++; Z80.DE.wl++; Z80.BC.wl--; if (Z80.BC.wl != 0) Z80.AF.bl |= 0x04; }; if (Z80.BC.wl != 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* LDIR			  */
            void ed_b1() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte val = (byte)cpu_readmem16(Z80.HL.wl); byte res = (byte)(Z80.AF.bh - val); Z80.HL.wl++; Z80.BC.wl--; Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | (SZ[res] & ~(0x20 | 0x08)) | ((Z80.AF.bh ^ val ^ res) & 0x10) | 0x02); if ((Z80.AF.bl & 0x10) != 0) res -= 1; if ((res & 0x02) != 0)Z80.AF.bl |= 0x20; if ((res & 0x08) != 0)Z80.AF.bl |= 0x08; if (Z80.BC.wl != 0) Z80.AF.bl |= 0x04; }; if (Z80.BC.wl != 0 && (Z80.AF.bl & 0x40) == 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* CPIR			  */
            void ed_b2() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte io = ((byte)cpu_readport(Z80.BC.wl)); Z80.BC.bh--; cpu_writemem16(Z80.HL.wl, io); Z80.HL.wl++; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0) Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io + 1) & 0x100) != 0)Z80.AF.bl |= 0x10 | 0x01; if (((irep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0)Z80.AF.bl |= 0x04; }; if (Z80.BC.bh != 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* INIR			  */
            void ed_b3() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writeport(Z80.BC.wl, io); Z80.BC.bh--; Z80.HL.wl++; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0)Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io + 1) & 0x100) != 0) Z80.AF.bl |= 0x10 | 0x01; if (((irep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0)Z80.AF.bl |= 0x04; }; if (Z80.BC.bh != 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* OTIR			  */
            void ed_b4() { illegal_2(); } /* DB   ED		  */
            void ed_b5() { illegal_2(); } /* DB   ED		  */
            void ed_b6() { illegal_2(); } /* DB   ED		  */
            void ed_b7() { illegal_2(); } /* DB   ED		  */

            void ed_b8() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writemem16(Z80.DE.wl, io); Z80.AF.bl &= 0x80 | 0x40 | 0x01; if (((Z80.AF.bh + io) & 0x02) != 0) Z80.AF.bl |= 0x20; if (((Z80.AF.bh + io) & 0x08) != 0) Z80.AF.bl |= 0x08; Z80.HL.wl--; Z80.DE.wl--; Z80.BC.wl--; if (Z80.BC.wl != 0) Z80.AF.bl |= 0x04; }; if (Z80.BC.wl != 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* LDDR			  */
            void ed_b9() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte val = (byte)cpu_readmem16(Z80.HL.wl); byte res = (byte)(Z80.AF.bh - val); Z80.HL.wl--; Z80.BC.wl--; Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | (SZ[res] & ~(0x20 | 0x08)) | ((Z80.AF.bh ^ val ^ res) & 0x10) | 0x02); if ((Z80.AF.bl & 0x10) != 0)res -= 1; if ((res & 0x02) != 0) Z80.AF.bl |= 0x20; if ((res & 0x08) != 0) Z80.AF.bl |= 0x08; if (Z80.BC.wl != 0) Z80.AF.bl |= 0x04; }; if (Z80.BC.wl != 0 && (Z80.AF.bl & 0x40) == 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* CPDR			  */
            void ed_ba() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte io = ((byte)cpu_readport(Z80.BC.wl)); Z80.BC.bh--; cpu_writemem16(Z80.HL.wl, io); Z80.HL.wl--; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0) Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io - 1) & 0x100) != 0) Z80.AF.bl |= 0x10 | 0x01; if (((drep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0)Z80.AF.bl |= 0x04; }; if (Z80.BC.bh != 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* INDR			  */
            void ed_bb() { { z80_ICount[0] -= 5; Z80.PC.wl -= 2; do { { byte io = (byte)cpu_readmem16(Z80.HL.wl); cpu_writeport(Z80.BC.wl, io); Z80.BC.bh--; Z80.HL.wl--; Z80.AF.bl = SZ[Z80.BC.bh]; if ((io & 0x80) != 0) Z80.AF.bl |= 0x02; if (((Z80.BC.bl + io - 1) & 0x100) != 0) Z80.AF.bl |= 0x10 | 0x01; if (((drep_tmp1[Z80.BC.bl & 3][io & 3] ^ breg_tmp2[Z80.BC.bh] ^ (Z80.BC.bl >> 2) ^ (io >> 2)) & 1) != 0) Z80.AF.bl |= 0x04; }; if (Z80.BC.bh != 0) { if (z80_ICount[0] > 0) { Z80.R += 2; z80_ICount[0] -= 21; } else break; } else { Z80.PC.wl += 2; z80_ICount[0] += 5; break; } } while (z80_ICount[0] > 0); }; } /* OTDR			  */
            void ed_bc() { illegal_2(); } /* DB   ED		  */
            void ed_bd() { illegal_2(); } /* DB   ED		  */
            void ed_be() { illegal_2(); } /* DB   ED		  */
            void ed_bf() { illegal_2(); } /* DB   ED		  */

            void ed_c0() { illegal_2(); } /* DB   ED		  */
            void ed_c1() { illegal_2(); } /* DB   ED		  */
            void ed_c2() { illegal_2(); } /* DB   ED		  */
            void ed_c3() { illegal_2(); } /* DB   ED		  */
            void ed_c4() { illegal_2(); } /* DB   ED		  */
            void ed_c5() { illegal_2(); } /* DB   ED		  */
            void ed_c6() { illegal_2(); } /* DB   ED		  */
            void ed_c7() { illegal_2(); } /* DB   ED		  */

            void ed_c8() { illegal_2(); } /* DB   ED		  */
            void ed_c9() { illegal_2(); } /* DB   ED		  */
            void ed_ca() { illegal_2(); } /* DB   ED		  */
            void ed_cb() { illegal_2(); } /* DB   ED		  */
            void ed_cc() { illegal_2(); } /* DB   ED		  */
            void ed_cd() { illegal_2(); } /* DB   ED		  */
            void ed_ce() { illegal_2(); } /* DB   ED		  */
            void ed_cf() { illegal_2(); } /* DB   ED		  */

            void ed_d0() { illegal_2(); } /* DB   ED		  */
            void ed_d1() { illegal_2(); } /* DB   ED		  */
            void ed_d2() { illegal_2(); } /* DB   ED		  */
            void ed_d3() { illegal_2(); } /* DB   ED		  */
            void ed_d4() { illegal_2(); } /* DB   ED		  */
            void ed_d5() { illegal_2(); } /* DB   ED		  */
            void ed_d6() { illegal_2(); } /* DB   ED		  */
            void ed_d7() { illegal_2(); } /* DB   ED		  */

            void ed_d8() { illegal_2(); } /* DB   ED		  */
            void ed_d9() { illegal_2(); } /* DB   ED		  */
            void ed_da() { illegal_2(); } /* DB   ED		  */
            void ed_db() { illegal_2(); } /* DB   ED		  */
            void ed_dc() { illegal_2(); } /* DB   ED		  */
            void ed_dd() { illegal_2(); } /* DB   ED		  */
            void ed_de() { illegal_2(); } /* DB   ED		  */
            void ed_df() { illegal_2(); } /* DB   ED		  */

            void ed_e0() { illegal_2(); } /* DB   ED		  */
            void ed_e1() { illegal_2(); } /* DB   ED		  */
            void ed_e2() { illegal_2(); } /* DB   ED		  */
            void ed_e3() { illegal_2(); } /* DB   ED		  */
            void ed_e4() { illegal_2(); } /* DB   ED		  */
            void ed_e5() { illegal_2(); } /* DB   ED		  */
            void ed_e6() { illegal_2(); } /* DB   ED		  */
            void ed_e7() { illegal_2(); } /* DB   ED		  */

            void ed_e8() { illegal_2(); } /* DB   ED		  */
            void ed_e9() { illegal_2(); } /* DB   ED		  */
            void ed_ea() { illegal_2(); } /* DB   ED		  */
            void ed_eb() { illegal_2(); } /* DB   ED		  */
            void ed_ec() { illegal_2(); } /* DB   ED		  */
            void ed_ed() { illegal_2(); } /* DB   ED		  */
            void ed_ee() { illegal_2(); } /* DB   ED		  */
            void ed_ef() { illegal_2(); } /* DB   ED		  */

            void ed_f0() { illegal_2(); } /* DB   ED		  */
            void ed_f1() { illegal_2(); } /* DB   ED		  */
            void ed_f2() { illegal_2(); } /* DB   ED		  */
            void ed_f3() { illegal_2(); } /* DB   ED		  */
            void ed_f4() { illegal_2(); } /* DB   ED		  */
            void ed_f5() { illegal_2(); } /* DB   ED		  */
            void ed_f6() { illegal_2(); } /* DB   ED		  */
            void ed_f7() { illegal_2(); } /* DB   ED		  */

            void ed_f8() { illegal_2(); } /* DB   ED		  */
            void ed_f9() { illegal_2(); } /* DB   ED		  */
            void ed_fa() { illegal_2(); } /* DB   ED		  */
            void ed_fb() { illegal_2(); } /* DB   ED		  */
            void ed_fc() { illegal_2(); } /* DB   ED		  */
            void ed_fd() { illegal_2(); } /* DB   ED		  */
            void ed_fe() { illegal_2(); } /* DB   ED		  */
            void ed_ff() { illegal_2(); } /* DB   ED		  */
            #endregion special
            #region dd IX register opcodes
            void dd_00() { illegal_1(); } /* DB   DD		  */
            void dd_01() { illegal_1(); } /* DB   DD		  */
            void dd_02() { illegal_1(); } /* DB   DD		  */
            void dd_03() { illegal_1(); } /* DB   DD		  */
            void dd_04() { illegal_1(); } /* DB   DD		  */
            void dd_05() { illegal_1(); } /* DB   DD		  */
            void dd_06() { illegal_1(); } /* DB   DD		  */
            void dd_07() { illegal_1(); } /* DB   DD		  */

            void dd_08() { illegal_1(); } /* DB   DD		  */
            void dd_09() { { uint res = Z80.IX.d + Z80.BC.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IX.d ^ res ^ Z80.BC.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IX.wl = (ushort)res; }; } /* ADD  IX,BC 	  */
            void dd_0a() { illegal_1(); } /* DB   DD		  */
            void dd_0b() { illegal_1(); } /* DB   DD		  */
            void dd_0c() { illegal_1(); } /* DB   DD		  */
            void dd_0d() { illegal_1(); } /* DB   DD		  */
            void dd_0e() { illegal_1(); } /* DB   DD		  */
            void dd_0f() { illegal_1(); } /* DB   DD		  */

            void dd_10() { illegal_1(); } /* DB   DD		  */
            void dd_11() { illegal_1(); } /* DB   DD		  */
            void dd_12() { illegal_1(); } /* DB   DD		  */
            void dd_13() { illegal_1(); } /* DB   DD		  */
            void dd_14() { illegal_1(); } /* DB   DD		  */
            void dd_15() { illegal_1(); } /* DB   DD		  */
            void dd_16() { illegal_1(); } /* DB   DD		  */
            void dd_17() { illegal_1(); } /* DB   DD		  */

            void dd_18() { illegal_1(); } /* DB   DD		  */
            void dd_19() { { uint res = Z80.IX.d + Z80.DE.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IX.d ^ res ^ Z80.DE.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IX.wl = (ushort)res; }; } /* ADD  IX,DE 	  */
            void dd_1a() { illegal_1(); } /* DB   DD		  */
            void dd_1b() { illegal_1(); } /* DB   DD		  */
            void dd_1c() { illegal_1(); } /* DB   DD		  */
            void dd_1d() { illegal_1(); } /* DB   DD		  */
            void dd_1e() { illegal_1(); } /* DB   DD		  */
            void dd_1f() { illegal_1(); } /* DB   DD		  */

            void dd_20() { illegal_1(); } /* DB   DD		  */
            void dd_21() { Z80.IX.wl = ARG16(); } /* LD   IX,w		  */
            void dd_22() { EA = ARG16(); WM16(EA, Z80.IX); } /* LD   (w),IX	  */
            void dd_23() { Z80.IX.wl++; } /* INC  IX		  */
            void dd_24() { Z80.IX.bh = INC(Z80.IX.bh); } /* INC  HX		  */
            void dd_25() { Z80.IX.bh = DEC(Z80.IX.bh); } /* DEC  HX		  */
            void dd_26() { Z80.IX.bh = ARG(); } /* LD   HX,n		  */
            void dd_27() { illegal_1(); } /* DB   DD		  */

            void dd_28() { illegal_1(); } /* DB   DD		  */
            void dd_29() { { uint res = Z80.IX.d + Z80.IX.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IX.d ^ res ^ Z80.IX.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IX.wl = (ushort)res; }; } /* ADD  IX,IX 	  */
            void dd_2a() { EA = ARG16(); RM16(EA, ref Z80.IX); } /* LD   IX,(w)	  */
            void dd_2b() { Z80.IX.wl--; } /* DEC  IX		  */
            void dd_2c() { Z80.IX.bl = INC(Z80.IX.bl); } /* INC  LX		  */
            void dd_2d() { Z80.IX.bl = DEC(Z80.IX.bl); } /* DEC  LX		  */
            void dd_2e() { Z80.IX.bl = ARG(); } /* LD   LX,n		  */
            void dd_2f() { illegal_1(); } /* DB   DD		  */

            void dd_30() { illegal_1(); } /* DB   DD		  */
            void dd_31() { illegal_1(); } /* DB   DD		  */
            void dd_32() { illegal_1(); } /* DB   DD		  */
            void dd_33() { illegal_1(); } /* DB   DD		  */
            void dd_34() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, INC((byte)cpu_readmem16((int)EA))); } /* INC  (IX+o)	  */
            void dd_35() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, DEC((byte)cpu_readmem16((int)EA))); } /* DEC  (IX+o)	  */
            void dd_36() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, ARG()); } /* LD   (IX+o),n	  */
            void dd_37() { illegal_1(); } /* DB   DD		  */

            void dd_38() { illegal_1(); } /* DB   DD		  */
            void dd_39() { { uint res = Z80.IX.d + Z80.SP.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IX.d ^ res ^ Z80.SP.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IX.wl = (ushort)res; }; } /* ADD  IX,SP 	  */
            void dd_3a() { illegal_1(); } /* DB   DD		  */
            void dd_3b() { illegal_1(); } /* DB   DD		  */
            void dd_3c() { illegal_1(); } /* DB   DD		  */
            void dd_3d() { illegal_1(); } /* DB   DD		  */
            void dd_3e() { illegal_1(); } /* DB   DD		  */
            void dd_3f() { illegal_1(); } /* DB   DD		  */

            void dd_40() { illegal_1(); } /* DB   DD		  */
            void dd_41() { illegal_1(); } /* DB   DD		  */
            void dd_42() { illegal_1(); } /* DB   DD		  */
            void dd_43() { illegal_1(); } /* DB   DD		  */
            void dd_44() { Z80.BC.bh = Z80.IX.bh; } /* LD   B,HX		  */
            void dd_45() { Z80.BC.bh = Z80.IX.bl; } /* LD   B,LX		  */
            void dd_46() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.BC.bh = (byte)cpu_readmem16((int)EA); } /* LD   B,(IX+o)	  */
            void dd_47() { illegal_1(); } /* DB   DD		  */

            void dd_48() { illegal_1(); } /* DB   DD		  */
            void dd_49() { illegal_1(); } /* DB   DD		  */
            void dd_4a() { illegal_1(); } /* DB   DD		  */
            void dd_4b() { illegal_1(); } /* DB   DD		  */
            void dd_4c() { Z80.BC.bl = Z80.IX.bh; } /* LD   C,HX		  */
            void dd_4d() { Z80.BC.bl = Z80.IX.bl; } /* LD   C,LX		  */
            void dd_4e() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.BC.bl = (byte)cpu_readmem16((int)EA); } /* LD   C,(IX+o)	  */
            void dd_4f() { illegal_1(); } /* DB   DD		  */

            void dd_50() { illegal_1(); } /* DB   DD		  */
            void dd_51() { illegal_1(); } /* DB   DD		  */
            void dd_52() { illegal_1(); } /* DB   DD		  */
            void dd_53() { illegal_1(); } /* DB   DD		  */
            void dd_54() { Z80.DE.bh = Z80.IX.bh; } /* LD   D,HX		  */
            void dd_55() { Z80.DE.bh = Z80.IX.bl; } /* LD   D,LX		  */
            void dd_56() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.DE.bh = (byte)cpu_readmem16((int)EA); } /* LD   D,(IX+o)	  */
            void dd_57() { illegal_1(); } /* DB   DD		  */

            void dd_58() { illegal_1(); } /* DB   DD		  */
            void dd_59() { illegal_1(); } /* DB   DD		  */
            void dd_5a() { illegal_1(); } /* DB   DD		  */
            void dd_5b() { illegal_1(); } /* DB   DD		  */
            void dd_5c() { Z80.DE.bl = Z80.IX.bh; } /* LD   E,HX		  */
            void dd_5d() { Z80.DE.bl = Z80.IX.bl; } /* LD   E,LX		  */
            void dd_5e() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.DE.bl = (byte)cpu_readmem16((int)EA); } /* LD   E,(IX+o)	  */
            void dd_5f() { illegal_1(); } /* DB   DD		  */

            void dd_60() { Z80.IX.bh = Z80.BC.bh; } /* LD   HX,B		  */
            void dd_61() { Z80.IX.bh = Z80.BC.bl; } /* LD   HX,C		  */
            void dd_62() { Z80.IX.bh = Z80.DE.bh; } /* LD   HX,D		  */
            void dd_63() { Z80.IX.bh = Z80.DE.bl; } /* LD   HX,E		  */
            void dd_64() { } /* LD   HX,HX 	  */
            void dd_65() { Z80.IX.bh = Z80.IX.bl; } /* LD   HX,LX 	  */
            void dd_66() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.HL.bh = (byte)cpu_readmem16((int)EA); } /* LD   H,(IX+o)	  */
            void dd_67() { Z80.IX.bh = Z80.AF.bh; } /* LD   HX,A		  */

            void dd_68() { Z80.IX.bl = Z80.BC.bh; } /* LD   LX,B		  */
            void dd_69() { Z80.IX.bl = Z80.BC.bl; } /* LD   LX,C		  */
            void dd_6a() { Z80.IX.bl = Z80.DE.bh; } /* LD   LX,D		  */
            void dd_6b() { Z80.IX.bl = Z80.DE.bl; } /* LD   LX,E		  */
            void dd_6c() { Z80.IX.bl = Z80.IX.bh; } /* LD   LX,HX 	  */
            void dd_6d() { } /* LD   LX,LX 	  */
            void dd_6e() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.HL.bl = (byte)cpu_readmem16((int)EA); } /* LD   L,(IX+o)	  */
            void dd_6f() { Z80.IX.bl = Z80.AF.bh; } /* LD   LX,A		  */

            void dd_70() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.BC.bh); } /* LD   (IX+o),B	  */
            void dd_71() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.BC.bl); } /* LD   (IX+o),C	  */
            void dd_72() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.DE.bh); } /* LD   (IX+o),D	  */
            void dd_73() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.DE.bl); } /* LD   (IX+o),E	  */
            void dd_74() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.HL.bh); } /* LD   (IX+o),H	  */
            void dd_75() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.HL.bl); } /* LD   (IX+o),L	  */
            void dd_76() { illegal_1(); } /* DB   DD		  */
            void dd_77() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.AF.bh); } /* LD   (IX+o),A	  */

            void dd_78() { illegal_1(); } /* DB   DD		  */
            void dd_79() { illegal_1(); } /* DB   DD		  */
            void dd_7a() { illegal_1(); } /* DB   DD		  */
            void dd_7b() { illegal_1(); } /* DB   DD		  */
            void dd_7c() { Z80.AF.bh = Z80.IX.bh; } /* LD   A,HX		  */
            void dd_7d() { Z80.AF.bh = Z80.IX.bl; } /* LD   A,LX		  */
            void dd_7e() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.AF.bh = (byte)cpu_readmem16((int)EA); } /* LD   A,(IX+o)	  */
            void dd_7f() { illegal_1(); } /* DB   DD		  */

            void dd_80() { illegal_1(); } /* DB   DD		  */
            void dd_81() { illegal_1(); } /* DB   DD		  */
            void dd_82() { illegal_1(); } /* DB   DD		  */
            void dd_83() { illegal_1(); } /* DB   DD		  */
            void dd_84() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.IX.bh); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,HX		  */
            void dd_85() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.IX.bl); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,LX		  */
            void dd_86() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + (byte)cpu_readmem16((int)EA)); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,(IX+o)	  */
            void dd_87() { illegal_1(); } /* DB   DD		  */

            void dd_88() { illegal_1(); } /* DB   DD		  */
            void dd_89() { illegal_1(); } /* DB   DD		  */
            void dd_8a() { illegal_1(); } /* DB   DD		  */
            void dd_8b() { illegal_1(); } /* DB   DD		  */
            void dd_8c() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.IX.bh + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,HX		  */
            void dd_8d() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.IX.bl + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,LX		  */
            void dd_8e() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + (byte)cpu_readmem16((int)EA) + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,(IX+o)	  */
            void dd_8f() { illegal_1(); } /* DB   DD		  */

            void dd_90() { illegal_1(); } /* DB   DD		  */
            void dd_91() { illegal_1(); } /* DB   DD		  */
            void dd_92() { illegal_1(); } /* DB   DD		  */
            void dd_93() { illegal_1(); } /* DB   DD		  */
            void dd_94() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IX.bh); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  HX		  */
            void dd_95() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IX.bl); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  LX		  */
            void dd_96() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16((int)EA)); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  (IX+o)	  */
            void dd_97() { illegal_1(); } /* DB   DD		  */

            void dd_98() { illegal_1(); } /* DB   DD		  */
            void dd_99() { illegal_1(); } /* DB   DD		  */
            void dd_9a() { illegal_1(); } /* DB   DD		  */
            void dd_9b() { illegal_1(); } /* DB   DD		  */
            void dd_9c() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.IX.bh - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,HX		  */
            void dd_9d() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.IX.bl - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,LX		  */
            void dd_9e() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16((int)EA) - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,(IX+o)	  */
            void dd_9f() { illegal_1(); } /* DB   DD		  */

            void dd_a0() { illegal_1(); } /* DB   DD		  */
            void dd_a1() { illegal_1(); } /* DB   DD		  */
            void dd_a2() { illegal_1(); } /* DB   DD		  */
            void dd_a3() { illegal_1(); } /* DB   DD		  */
            void dd_a4() { Z80.AF.bh &= Z80.IX.bh; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  HX		  */
            void dd_a5() { Z80.AF.bh &= Z80.IX.bl; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  LX		  */
            void dd_a6() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.AF.bh &= (byte)cpu_readmem16((int)EA); Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  (IX+o)	  */
            void dd_a7() { illegal_1(); } /* DB   DD		  */

            void dd_a8() { illegal_1(); } /* DB   DD		  */
            void dd_a9() { illegal_1(); } /* DB   DD		  */
            void dd_aa() { illegal_1(); } /* DB   DD		  */
            void dd_ab() { illegal_1(); } /* DB   DD		  */
            void dd_ac() { Z80.AF.bh ^= Z80.IX.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  HX		  */
            void dd_ad() { Z80.AF.bh ^= Z80.IX.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  LX		  */
            void dd_ae() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.AF.bh ^= (byte)cpu_readmem16((int)EA); Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  (IX+o)	  */
            void dd_af() { illegal_1(); } /* DB   DD		  */

            void dd_b0() { illegal_1(); } /* DB   DD		  */
            void dd_b1() { illegal_1(); } /* DB   DD		  */
            void dd_b2() { illegal_1(); } /* DB   DD		  */
            void dd_b3() { illegal_1(); } /* DB   DD		  */
            void dd_b4() { Z80.AF.bh |= Z80.IX.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   HX		  */
            void dd_b5() { Z80.AF.bh |= Z80.IX.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   LX		  */
            void dd_b6() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); Z80.AF.bh |= (byte)cpu_readmem16((int)EA); Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   (IX+o)	  */
            void dd_b7() { illegal_1(); } /* DB   DD		  */

            void dd_b8() { illegal_1(); } /* DB   DD		  */
            void dd_b9() { illegal_1(); } /* DB   DD		  */
            void dd_ba() { illegal_1(); } /* DB   DD		  */
            void dd_bb() { illegal_1(); } /* DB   DD		  */
            void dd_bc() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IX.bh); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   HX		  */
            void dd_bd() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IX.bl); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   LX		  */
            void dd_be() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16((int)EA)); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   (IX+o)	  */
            void dd_bf() { illegal_1(); } /* DB   DD		  */

            void dd_c0() { illegal_1(); } /* DB   DD		  */
            void dd_c1() { illegal_1(); } /* DB   DD		  */
            void dd_c2() { illegal_1(); } /* DB   DD		  */
            void dd_c3() { illegal_1(); } /* DB   DD		  */
            void dd_c4() { illegal_1(); } /* DB   DD		  */
            void dd_c5() { illegal_1(); } /* DB   DD		  */
            void dd_c6() { illegal_1(); } /* DB   DD		  */
            void dd_c7() { illegal_1(); } /* DB   DD		  */

            void dd_c8() { illegal_1(); } /* DB   DD		  */
            void dd_c9() { illegal_1(); } /* DB   DD		  */
            void dd_ca() { illegal_1(); } /* DB   DD		  */
            void dd_cb() { EA = (uint)(ushort)(Z80.IX.wl + (sbyte)ARG()); { uint op = ARG(); z80_ICount[0] -= cc_xxcb[op]; Z80xxcb[op](); }; } /* **   DD CB xx	  */
            void dd_cc() { illegal_1(); } /* DB   DD		  */
            void dd_cd() { illegal_1(); } /* DB   DD		  */
            void dd_ce() { illegal_1(); } /* DB   DD		  */
            void dd_cf() { illegal_1(); } /* DB   DD		  */

            void dd_d0() { illegal_1(); } /* DB   DD		  */
            void dd_d1() { illegal_1(); } /* DB   DD		  */
            void dd_d2() { illegal_1(); } /* DB   DD		  */
            void dd_d3() { illegal_1(); } /* DB   DD		  */
            void dd_d4() { illegal_1(); } /* DB   DD		  */
            void dd_d5() { illegal_1(); } /* DB   DD		  */
            void dd_d6() { illegal_1(); } /* DB   DD		  */
            void dd_d7() { illegal_1(); } /* DB   DD		  */

            void dd_d8() { illegal_1(); } /* DB   DD		  */
            void dd_d9() { illegal_1(); } /* DB   DD		  */
            void dd_da() { illegal_1(); } /* DB   DD		  */
            void dd_db() { illegal_1(); } /* DB   DD		  */
            void dd_dc() { illegal_1(); } /* DB   DD		  */
            void dd_dd() { illegal_1(); } /* DB   DD		  */
            void dd_de() { illegal_1(); } /* DB   DD		  */
            void dd_df() { illegal_1(); } /* DB   DD		  */

            void dd_e0() { illegal_1(); } /* DB   DD		  */
            void dd_e1() { { RM16(Z80.SP.d, ref Z80.IX); Z80.SP.wl += 2; }; } /* POP  IX		  */
            void dd_e2() { illegal_1(); } /* DB   DD		  */
            void dd_e3() { { PAIR tmp = new PAIR(); RM16(Z80.SP.d, ref tmp); WM16(Z80.SP.d, Z80.IX); Z80.IX = tmp; }; } /* EX   (SP),IX	  */
            void dd_e4() { illegal_1(); } /* DB   DD		  */
            void dd_e5() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.IX); }; } /* PUSH IX		  */
            void dd_e6() { illegal_1(); } /* DB   DD		  */
            void dd_e7() { illegal_1(); } /* DB   DD		  */

            void dd_e8() { illegal_1(); } /* DB   DD		  */
            void dd_e9() { Z80.PC.wl = Z80.IX.wl; change_pc16(Z80.PC.d); } /* JP   (IX)		  */
            void dd_ea() { illegal_1(); } /* DB   DD		  */
            void dd_eb() { illegal_1(); } /* DB   DD		  */
            void dd_ec() { illegal_1(); } /* DB   DD		  */
            void dd_ed() { illegal_1(); } /* DB   DD		  */
            void dd_ee() { illegal_1(); } /* DB   DD		  */
            void dd_ef() { illegal_1(); } /* DB   DD		  */

            void dd_f0() { illegal_1(); } /* DB   DD		  */
            void dd_f1() { illegal_1(); } /* DB   DD		  */
            void dd_f2() { illegal_1(); } /* DB   DD		  */
            void dd_f3() { illegal_1(); } /* DB   DD		  */
            void dd_f4() { illegal_1(); } /* DB   DD		  */
            void dd_f5() { illegal_1(); } /* DB   DD		  */
            void dd_f6() { illegal_1(); } /* DB   DD		  */
            void dd_f7() { illegal_1(); } /* DB   DD		  */

            void dd_f8() { illegal_1(); } /* DB   DD		  */
            void dd_f9() { Z80.SP.wl = Z80.IX.wl; } /* LD   SP,IX 	  */
            void dd_fa() { illegal_1(); } /* DB   DD		  */
            void dd_fb() { illegal_1(); } /* DB   DD		  */
            void dd_fc() { illegal_1(); } /* DB   DD		  */
            void dd_fd() { illegal_1(); } /* DB   DD		  */
            void dd_fe() { illegal_1(); } /* DB   DD		  */
            void dd_ff() { illegal_1(); } /* DB   DD		  */
            #endregion
            #region fd IY register opcodes
            void fd_00() { illegal_1(); } /* DB   FD		  */
            void fd_01() { illegal_1(); } /* DB   FD		  */
            void fd_02() { illegal_1(); } /* DB   FD		  */
            void fd_03() { illegal_1(); } /* DB   FD		  */
            void fd_04() { illegal_1(); } /* DB   FD		  */
            void fd_05() { illegal_1(); } /* DB   FD		  */
            void fd_06() { illegal_1(); } /* DB   FD		  */
            void fd_07() { illegal_1(); } /* DB   FD		  */

            void fd_08() { illegal_1(); } /* DB   FD		  */
            void fd_09() { { uint res = Z80.IY.d + Z80.BC.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IY.d ^ res ^ Z80.BC.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IY.wl = (ushort)res; }; } /* ADD  IY,BC 	  */
            void fd_0a() { illegal_1(); } /* DB   FD		  */
            void fd_0b() { illegal_1(); } /* DB   FD		  */
            void fd_0c() { illegal_1(); } /* DB   FD		  */
            void fd_0d() { illegal_1(); } /* DB   FD		  */
            void fd_0e() { illegal_1(); } /* DB   FD		  */
            void fd_0f() { illegal_1(); } /* DB   FD		  */

            void fd_10() { illegal_1(); } /* DB   FD		  */
            void fd_11() { illegal_1(); } /* DB   FD		  */
            void fd_12() { illegal_1(); } /* DB   FD		  */
            void fd_13() { illegal_1(); } /* DB   FD		  */
            void fd_14() { illegal_1(); } /* DB   FD		  */
            void fd_15() { illegal_1(); } /* DB   FD		  */
            void fd_16() { illegal_1(); } /* DB   FD		  */
            void fd_17() { illegal_1(); } /* DB   FD		  */

            void fd_18() { illegal_1(); } /* DB   FD		  */
            void fd_19() { { uint res = Z80.IY.d + Z80.DE.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IY.d ^ res ^ Z80.DE.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IY.wl = (ushort)res; }; } /* ADD  IY,DE 	  */
            void fd_1a() { illegal_1(); } /* DB   FD		  */
            void fd_1b() { illegal_1(); } /* DB   FD		  */
            void fd_1c() { illegal_1(); } /* DB   FD		  */
            void fd_1d() { illegal_1(); } /* DB   FD		  */
            void fd_1e() { illegal_1(); } /* DB   FD		  */
            void fd_1f() { illegal_1(); } /* DB   FD		  */

            void fd_20() { illegal_1(); } /* DB   FD		  */
            void fd_21() { Z80.IY.wl = ARG16(); } /* LD   IY,w		  */
            void fd_22() { EA = ARG16(); WM16(EA, Z80.IY); } /* LD   (w),IY	  */
            void fd_23() { Z80.IY.wl++; } /* INC  IY		  */
            void fd_24() { Z80.IY.bh = INC(Z80.IY.bh); } /* INC  HY		  */
            void fd_25() { Z80.IY.bh = DEC(Z80.IY.bh); } /* DEC  HY		  */
            void fd_26() { Z80.IY.bh = ARG(); } /* LD   HY,n		  */
            void fd_27() { illegal_1(); } /* DB   FD		  */

            void fd_28() { illegal_1(); } /* DB   FD		  */
            void fd_29() { { uint res = Z80.IY.d + Z80.IY.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IY.d ^ res ^ Z80.IY.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IY.wl = (ushort)res; }; } /* ADD  IY,IY 	  */
            void fd_2a() { EA = ARG16(); RM16(EA, ref Z80.IY); } /* LD   IY,(w)	  */
            void fd_2b() { Z80.IY.wl--; } /* DEC  IY		  */
            void fd_2c() { Z80.IY.bl = INC(Z80.IY.bl); } /* INC  LY		  */
            void fd_2d() { Z80.IY.bl = DEC(Z80.IY.bl); } /* DEC  LY		  */
            void fd_2e() { Z80.IY.bl = ARG(); } /* LD   LY,n		  */
            void fd_2f() { illegal_1(); } /* DB   FD		  */

            void fd_30() { illegal_1(); } /* DB   FD		  */
            void fd_31() { illegal_1(); } /* DB   FD		  */
            void fd_32() { illegal_1(); } /* DB   FD		  */
            void fd_33() { illegal_1(); } /* DB   FD		  */
            void fd_34() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, INC((byte)cpu_readmem16((int)EA))); } /* INC  (IY+o)	  */
            void fd_35() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, DEC((byte)cpu_readmem16((int)EA))); } /* DEC  (IY+o)	  */
            void fd_36() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, ARG()); } /* LD   (IY+o),n	  */
            void fd_37() { illegal_1(); } /* DB   FD		  */

            void fd_38() { illegal_1(); } /* DB   FD		  */
            void fd_39() { { uint res = Z80.IY.d + Z80.SP.d; Z80.AF.bl = (byte)((Z80.AF.bl & (0x80 | 0x40 | 0x04)) | (((Z80.IY.d ^ res ^ Z80.SP.d) >> 8) & 0x10) | ((res >> 16) & 0x01)); Z80.IY.wl = (ushort)res; }; } /* ADD  IY,SP 	  */
            void fd_3a() { illegal_1(); } /* DB   FD		  */
            void fd_3b() { illegal_1(); } /* DB   FD		  */
            void fd_3c() { illegal_1(); } /* DB   FD		  */
            void fd_3d() { illegal_1(); } /* DB   FD		  */
            void fd_3e() { illegal_1(); } /* DB   FD		  */
            void fd_3f() { illegal_1(); } /* DB   FD		  */

            void fd_40() { illegal_1(); } /* DB   FD		  */
            void fd_41() { illegal_1(); } /* DB   FD		  */
            void fd_42() { illegal_1(); } /* DB   FD		  */
            void fd_43() { illegal_1(); } /* DB   FD		  */
            void fd_44() { Z80.BC.bh = Z80.IY.bh; } /* LD   B,HY		  */
            void fd_45() { Z80.BC.bh = Z80.IY.bl; } /* LD   B,LY		  */
            void fd_46() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.BC.bh = (byte)cpu_readmem16((int)EA); } /* LD   B,(IY+o)	  */
            void fd_47() { illegal_1(); } /* DB   FD		  */

            void fd_48() { illegal_1(); } /* DB   FD		  */
            void fd_49() { illegal_1(); } /* DB   FD		  */
            void fd_4a() { illegal_1(); } /* DB   FD		  */
            void fd_4b() { illegal_1(); } /* DB   FD		  */
            void fd_4c() { Z80.BC.bl = Z80.IY.bh; } /* LD   C,HY		  */
            void fd_4d() { Z80.BC.bl = Z80.IY.bl; } /* LD   C,LY		  */
            void fd_4e() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.BC.bl = (byte)cpu_readmem16((int)EA); } /* LD   C,(IY+o)	  */
            void fd_4f() { illegal_1(); } /* DB   FD		  */

            void fd_50() { illegal_1(); } /* DB   FD		  */
            void fd_51() { illegal_1(); } /* DB   FD		  */
            void fd_52() { illegal_1(); } /* DB   FD		  */
            void fd_53() { illegal_1(); } /* DB   FD		  */
            void fd_54() { Z80.DE.bh = Z80.IY.bh; } /* LD   D,HY		  */
            void fd_55() { Z80.DE.bh = Z80.IY.bl; } /* LD   D,LY		  */
            void fd_56() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.DE.bh = (byte)cpu_readmem16((int)EA); } /* LD   D,(IY+o)	  */
            void fd_57() { illegal_1(); } /* DB   FD		  */

            void fd_58() { illegal_1(); } /* DB   FD		  */
            void fd_59() { illegal_1(); } /* DB   FD		  */
            void fd_5a() { illegal_1(); } /* DB   FD		  */
            void fd_5b() { illegal_1(); } /* DB   FD		  */
            void fd_5c() { Z80.DE.bl = Z80.IY.bh; } /* LD   E,HY		  */
            void fd_5d() { Z80.DE.bl = Z80.IY.bl; } /* LD   E,LY		  */
            void fd_5e() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.DE.bl = (byte)cpu_readmem16((int)EA); } /* LD   E,(IY+o)	  */
            void fd_5f() { illegal_1(); } /* DB   FD		  */

            void fd_60() { Z80.IY.bh = Z80.BC.bh; } /* LD   HY,B		  */
            void fd_61() { Z80.IY.bh = Z80.BC.bl; } /* LD   HY,C		  */
            void fd_62() { Z80.IY.bh = Z80.DE.bh; } /* LD   HY,D		  */
            void fd_63() { Z80.IY.bh = Z80.DE.bl; } /* LD   HY,E		  */
            void fd_64() { } /* LD   HY,HY 	  */
            void fd_65() { Z80.IY.bh = Z80.IY.bl; } /* LD   HY,LY 	  */
            void fd_66() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.HL.bh = (byte)cpu_readmem16((int)EA); } /* LD   H,(IY+o)	  */
            void fd_67() { Z80.IY.bh = Z80.AF.bh; } /* LD   HY,A		  */

            void fd_68() { Z80.IY.bl = Z80.BC.bh; } /* LD   LY,B		  */
            void fd_69() { Z80.IY.bl = Z80.BC.bl; } /* LD   LY,C		  */
            void fd_6a() { Z80.IY.bl = Z80.DE.bh; } /* LD   LY,D		  */
            void fd_6b() { Z80.IY.bl = Z80.DE.bl; } /* LD   LY,E		  */
            void fd_6c() { Z80.IY.bl = Z80.IY.bh; } /* LD   LY,HY 	  */
            void fd_6d() { } /* LD   LY,LY 	  */
            void fd_6e() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.HL.bl = (byte)cpu_readmem16((int)EA); } /* LD   L,(IY+o)	  */
            void fd_6f() { Z80.IY.bl = Z80.AF.bh; } /* LD   LY,A		  */

            void fd_70() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.BC.bh); } /* LD   (IY+o),B	  */
            void fd_71() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.BC.bl); } /* LD   (IY+o),C	  */
            void fd_72() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.DE.bh); } /* LD   (IY+o),D	  */
            void fd_73() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.DE.bl); } /* LD   (IY+o),E	  */
            void fd_74() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.HL.bh); } /* LD   (IY+o),H	  */
            void fd_75() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.HL.bl); } /* LD   (IY+o),L	  */
            void fd_76() { illegal_1(); } /* DB   FD		  */
            void fd_77() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); cpu_writemem16((int)EA, Z80.AF.bh); } /* LD   (IY+o),A	  */

            void fd_78() { illegal_1(); } /* DB   FD		  */
            void fd_79() { illegal_1(); } /* DB   FD		  */
            void fd_7a() { illegal_1(); } /* DB   FD		  */
            void fd_7b() { illegal_1(); } /* DB   FD		  */
            void fd_7c() { Z80.AF.bh = Z80.IY.bh; } /* LD   A,HY		  */
            void fd_7d() { Z80.AF.bh = Z80.IY.bl; } /* LD   A,LY		  */
            void fd_7e() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.AF.bh = (byte)cpu_readmem16((int)EA); } /* LD   A,(IY+o)	  */
            void fd_7f() { illegal_1(); } /* DB   FD		  */

            void fd_80() { illegal_1(); } /* DB   FD		  */
            void fd_81() { illegal_1(); } /* DB   FD		  */
            void fd_82() { illegal_1(); } /* DB   FD		  */
            void fd_83() { illegal_1(); } /* DB   FD		  */
            void fd_84() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.IY.bh); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,HY		  */
            void fd_85() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + Z80.IY.bl); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,LY		  */
            void fd_86() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) + (byte)cpu_readmem16((int)EA)); Z80.AF.bl = SZHVC_add[ah | res]; Z80.AF.bh = (byte)res; }; } /* ADD  A,(IY+o)	  */
            void fd_87() { illegal_1(); } /* DB   FD		  */

            void fd_88() { illegal_1(); } /* DB   FD		  */
            void fd_89() { illegal_1(); } /* DB   FD		  */
            void fd_8a() { illegal_1(); } /* DB   FD		  */
            void fd_8b() { illegal_1(); } /* DB   FD		  */
            void fd_8c() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.IY.bh + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,HY		  */
            void fd_8d() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + Z80.IY.bl + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,LY		  */
            void fd_8e() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) + (byte)cpu_readmem16((int)EA) + c); Z80.AF.bl = SZHVC_add[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* ADC  A,(IY+o)	  */
            void fd_8f() { illegal_1(); } /* DB   FD		  */

            void fd_90() { illegal_1(); } /* DB   FD		  */
            void fd_91() { illegal_1(); } /* DB   FD		  */
            void fd_92() { illegal_1(); } /* DB   FD		  */
            void fd_93() { illegal_1(); } /* DB   FD		  */
            void fd_94() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IY.bh); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  HY		  */
            void fd_95() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IY.bl); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  LY		  */
            void fd_96() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16((int)EA)); Z80.AF.bl = SZHVC_sub[ah | res]; Z80.AF.bh = (byte)res; }; } /* SUB  (IY+o)	  */
            void fd_97() { illegal_1(); } /* DB   FD		  */

            void fd_98() { illegal_1(); } /* DB   FD		  */
            void fd_99() { illegal_1(); } /* DB   FD		  */
            void fd_9a() { illegal_1(); } /* DB   FD		  */
            void fd_9b() { illegal_1(); } /* DB   FD		  */
            void fd_9c() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.IY.bh - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,HY		  */
            void fd_9d() { { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - Z80.IY.bl - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,LY		  */
            void fd_9e() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00, c = Z80.AF.d & 1; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16((int)EA) - c); Z80.AF.bl = SZHVC_sub[(c << 16) | ah | res]; Z80.AF.bh = (byte)res; }; } /* SBC  A,(IY+o)	  */
            void fd_9f() { illegal_1(); } /* DB   FD		  */

            void fd_a0() { illegal_1(); } /* DB   FD		  */
            void fd_a1() { illegal_1(); } /* DB   FD		  */
            void fd_a2() { illegal_1(); } /* DB   FD		  */
            void fd_a3() { illegal_1(); } /* DB   FD		  */
            void fd_a4() { Z80.AF.bh &= Z80.IY.bh; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  HY		  */
            void fd_a5() { Z80.AF.bh &= Z80.IY.bl; Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  LY		  */
            void fd_a6() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.AF.bh &= (byte)cpu_readmem16((int)EA); Z80.AF.bl = (byte)(SZP[Z80.AF.bh] | 0x10); } /* AND  (IY+o)	  */
            void fd_a7() { illegal_1(); } /* DB   FD		  */

            void fd_a8() { illegal_1(); } /* DB   FD		  */
            void fd_a9() { illegal_1(); } /* DB   FD		  */
            void fd_aa() { illegal_1(); } /* DB   FD		  */
            void fd_ab() { illegal_1(); } /* DB   FD		  */
            void fd_ac() { Z80.AF.bh ^= Z80.IY.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  HY		  */
            void fd_ad() { Z80.AF.bh ^= Z80.IY.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  LY		  */
            void fd_ae() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.AF.bh ^= (byte)cpu_readmem16((int)EA); Z80.AF.bl = SZP[Z80.AF.bh]; } /* XOR  (IY+o)	  */
            void fd_af() { illegal_1(); } /* DB   FD		  */

            void fd_b0() { illegal_1(); } /* DB   FD		  */
            void fd_b1() { illegal_1(); } /* DB   FD		  */
            void fd_b2() { illegal_1(); } /* DB   FD		  */
            void fd_b3() { illegal_1(); } /* DB   FD		  */
            void fd_b4() { Z80.AF.bh |= Z80.IY.bh; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   HY		  */
            void fd_b5() { Z80.AF.bh |= Z80.IY.bl; Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   LY		  */
            void fd_b6() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); Z80.AF.bh |= (byte)cpu_readmem16((int)EA); Z80.AF.bl = SZP[Z80.AF.bh]; } /* OR   (IY+o)	  */
            void fd_b7() { illegal_1(); } /* DB   FD		  */

            void fd_b8() { illegal_1(); } /* DB   FD		  */
            void fd_b9() { illegal_1(); } /* DB   FD		  */
            void fd_ba() { illegal_1(); } /* DB   FD		  */
            void fd_bb() { illegal_1(); } /* DB   FD		  */
            void fd_bc() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IY.bh); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   HY		  */
            void fd_bd() { { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - Z80.IY.bl); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   LY		  */
            void fd_be() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); { uint ah = Z80.AF.d & 0xff00; uint res = (byte)((ah >> 8) - (byte)cpu_readmem16((int)EA)); Z80.AF.bl = SZHVC_sub[ah | res]; }; } /* CP   (IY+o)	  */
            void fd_bf() { illegal_1(); } /* DB   FD		  */

            void fd_c0() { illegal_1(); } /* DB   FD		  */
            void fd_c1() { illegal_1(); } /* DB   FD		  */
            void fd_c2() { illegal_1(); } /* DB   FD		  */
            void fd_c3() { illegal_1(); } /* DB   FD		  */
            void fd_c4() { illegal_1(); } /* DB   FD		  */
            void fd_c5() { illegal_1(); } /* DB   FD		  */
            void fd_c6() { illegal_1(); } /* DB   FD		  */
            void fd_c7() { illegal_1(); } /* DB   FD		  */

            void fd_c8() { illegal_1(); } /* DB   FD		  */
            void fd_c9() { illegal_1(); } /* DB   FD		  */
            void fd_ca() { illegal_1(); } /* DB   FD		  */
            void fd_cb() { EA = (uint)(ushort)(Z80.IY.wl + (sbyte)ARG()); { uint op = ARG(); z80_ICount[0] -= cc_xxcb[op]; Z80xxcb[op](); }; } /* **   FD CB xx	  */
            void fd_cc() { illegal_1(); } /* DB   FD		  */
            void fd_cd() { illegal_1(); } /* DB   FD		  */
            void fd_ce() { illegal_1(); } /* DB   FD		  */
            void fd_cf() { illegal_1(); } /* DB   FD		  */

            void fd_d0() { illegal_1(); } /* DB   FD		  */
            void fd_d1() { illegal_1(); } /* DB   FD		  */
            void fd_d2() { illegal_1(); } /* DB   FD		  */
            void fd_d3() { illegal_1(); } /* DB   FD		  */
            void fd_d4() { illegal_1(); } /* DB   FD		  */
            void fd_d5() { illegal_1(); } /* DB   FD		  */
            void fd_d6() { illegal_1(); } /* DB   FD		  */
            void fd_d7() { illegal_1(); } /* DB   FD		  */

            void fd_d8() { illegal_1(); } /* DB   FD		  */
            void fd_d9() { illegal_1(); } /* DB   FD		  */
            void fd_da() { illegal_1(); } /* DB   FD		  */
            void fd_db() { illegal_1(); } /* DB   FD		  */
            void fd_dc() { illegal_1(); } /* DB   FD		  */
            void fd_dd() { illegal_1(); } /* DB   FD		  */
            void fd_de() { illegal_1(); } /* DB   FD		  */
            void fd_df() { illegal_1(); } /* DB   FD		  */

            void fd_e0() { illegal_1(); } /* DB   FD		  */
            void fd_e1() { { RM16(Z80.SP.d, ref Z80.IY); Z80.SP.wl += 2; }; } /* POP  IY		  */
            void fd_e2() { illegal_1(); } /* DB   FD		  */
            void fd_e3() { { PAIR tmp = new PAIR(); RM16(Z80.SP.d, ref tmp); WM16(Z80.SP.d, Z80.IY); Z80.IY = tmp; }; } /* EX   (SP),IY	  */
            void fd_e4() { illegal_1(); } /* DB   FD		  */
            void fd_e5() { { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.IY); }; } /* PUSH IY		  */
            void fd_e6() { illegal_1(); } /* DB   FD		  */
            void fd_e7() { illegal_1(); } /* DB   FD		  */

            void fd_e8() { illegal_1(); } /* DB   FD		  */
            void fd_e9() { Z80.PC.wl = Z80.IY.wl; change_pc16(Z80.PC.d); } /* JP   (IY)		  */
            void fd_ea() { illegal_1(); } /* DB   FD		  */
            void fd_eb() { illegal_1(); } /* DB   FD		  */
            void fd_ec() { illegal_1(); } /* DB   FD		  */
            void fd_ed() { illegal_1(); } /* DB   FD		  */
            void fd_ee() { illegal_1(); } /* DB   FD		  */
            void fd_ef() { illegal_1(); } /* DB   FD		  */

            void fd_f0() { illegal_1(); } /* DB   FD		  */
            void fd_f1() { illegal_1(); } /* DB   FD		  */
            void fd_f2() { illegal_1(); } /* DB   FD		  */
            void fd_f3() { illegal_1(); } /* DB   FD		  */
            void fd_f4() { illegal_1(); } /* DB   FD		  */
            void fd_f5() { illegal_1(); } /* DB   FD		  */
            void fd_f6() { illegal_1(); } /* DB   FD		  */
            void fd_f7() { illegal_1(); } /* DB   FD		  */

            void fd_f8() { illegal_1(); } /* DB   FD		  */
            void fd_f9() { Z80.SP.wl = Z80.IY.wl; } /* LD   SP,IY 	  */
            void fd_fa() { illegal_1(); } /* DB   FD		  */
            void fd_fb() { illegal_1(); } /* DB   FD		  */
            void fd_fc() { illegal_1(); } /* DB   FD		  */
            void fd_fd() { illegal_1(); } /* DB   FD		  */
            void fd_fe() { illegal_1(); } /* DB   FD		  */
            void fd_ff() { illegal_1(); } /* DB   FD		  */

            #endregion

            #region xxcb rotate,shift and bit operations with (IX+o)
            void xxcb_00() { Z80.BC.bh = RLC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RLC  B=(XY+o)	  */
            void xxcb_01() { Z80.BC.bl = RLC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RLC  C=(XY+o)	  */
            void xxcb_02() { Z80.DE.bh = RLC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RLC  D=(XY+o)	  */
            void xxcb_03() { Z80.DE.bl = RLC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RLC  E=(XY+o)	  */
            void xxcb_04() { Z80.HL.bh = RLC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RLC  H=(XY+o)	  */
            void xxcb_05() { Z80.HL.bl = RLC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RLC  L=(XY+o)	  */
            void xxcb_06() { cpu_writemem16((int)EA, RLC((byte)cpu_readmem16((int)EA))); } /* RLC  (XY+o)	  */
            void xxcb_07() { Z80.AF.bh = RLC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RLC  A=(XY+o)	  */

            void xxcb_08() { Z80.BC.bh = RRC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RRC  B=(XY+o)	  */
            void xxcb_09() { Z80.BC.bl = RRC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RRC  C=(XY+o)	  */
            void xxcb_0a() { Z80.DE.bh = RRC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RRC  D=(XY+o)	  */
            void xxcb_0b() { Z80.DE.bl = RRC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RRC  E=(XY+o)	  */
            void xxcb_0c() { Z80.HL.bh = RRC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RRC  H=(XY+o)	  */
            void xxcb_0d() { Z80.HL.bl = RRC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RRC  L=(XY+o)	  */
            void xxcb_0e() { cpu_writemem16((int)EA, RRC((byte)cpu_readmem16((int)EA))); } /* RRC  (XY+o)	  */
            void xxcb_0f() { Z80.AF.bh = RRC((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RRC  A=(XY+o)	  */

            void xxcb_10() { Z80.BC.bh = RL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RL   B=(XY+o)	  */
            void xxcb_11() { Z80.BC.bl = RL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RL   C=(XY+o)	  */
            void xxcb_12() { Z80.DE.bh = RL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RL   D=(XY+o)	  */
            void xxcb_13() { Z80.DE.bl = RL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RL   E=(XY+o)	  */
            void xxcb_14() { Z80.HL.bh = RL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RL   H=(XY+o)	  */
            void xxcb_15() { Z80.HL.bl = RL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RL   L=(XY+o)	  */
            void xxcb_16() { cpu_writemem16((int)EA, RL((byte)cpu_readmem16((int)EA))); } /* RL   (XY+o)	  */
            void xxcb_17() { Z80.AF.bh = RL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RL   A=(XY+o)	  */

            void xxcb_18() { Z80.BC.bh = RR((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RR   B=(XY+o)	  */
            void xxcb_19() { Z80.BC.bl = RR((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RR   C=(XY+o)	  */
            void xxcb_1a() { Z80.DE.bh = RR((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RR   D=(XY+o)	  */
            void xxcb_1b() { Z80.DE.bl = RR((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RR   E=(XY+o)	  */
            void xxcb_1c() { Z80.HL.bh = RR((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RR   H=(XY+o)	  */
            void xxcb_1d() { Z80.HL.bl = RR((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RR   L=(XY+o)	  */
            void xxcb_1e() { cpu_writemem16((int)EA, RR((byte)cpu_readmem16((int)EA))); } /* RR   (XY+o)	  */
            void xxcb_1f() { Z80.AF.bh = RR((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RR   A=(XY+o)	  */

            void xxcb_20() { Z80.BC.bh = SLA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SLA  B=(XY+o)	  */
            void xxcb_21() { Z80.BC.bl = SLA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SLA  C=(XY+o)	  */
            void xxcb_22() { Z80.DE.bh = SLA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SLA  D=(XY+o)	  */
            void xxcb_23() { Z80.DE.bl = SLA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SLA  E=(XY+o)	  */
            void xxcb_24() { Z80.HL.bh = SLA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SLA  H=(XY+o)	  */
            void xxcb_25() { Z80.HL.bl = SLA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SLA  L=(XY+o)	  */
            void xxcb_26() { cpu_writemem16((int)EA, SLA((byte)cpu_readmem16((int)EA))); } /* SLA  (XY+o)	  */
            void xxcb_27() { Z80.AF.bh = SLA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SLA  A=(XY+o)	  */

            void xxcb_28() { Z80.BC.bh = SRA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SRA  B=(XY+o)	  */
            void xxcb_29() { Z80.BC.bl = SRA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SRA  C=(XY+o)	  */
            void xxcb_2a() { Z80.DE.bh = SRA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SRA  D=(XY+o)	  */
            void xxcb_2b() { Z80.DE.bl = SRA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SRA  E=(XY+o)	  */
            void xxcb_2c() { Z80.HL.bh = SRA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SRA  H=(XY+o)	  */
            void xxcb_2d() { Z80.HL.bl = SRA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SRA  L=(XY+o)	  */
            void xxcb_2e() { cpu_writemem16((int)EA, SRA((byte)cpu_readmem16((int)EA))); } /* SRA  (XY+o)	  */
            void xxcb_2f() { Z80.AF.bh = SRA((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SRA  A=(XY+o)	  */

            void xxcb_30() { Z80.BC.bh = SLL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SLL  B=(XY+o)	  */
            void xxcb_31() { Z80.BC.bl = SLL( (byte)cpu_readmem16((int)EA) ); cpu_writemem16((int)EA,Z80.BC.bl); } /* SLL  C=(XY+o)	  */
            void xxcb_32() { Z80.DE.bh = SLL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SLL  D=(XY+o)	  */
            void xxcb_33() { Z80.DE.bl = SLL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SLL  E=(XY+o)	  */
            void xxcb_34() { Z80.HL.bh = SLL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SLL  H=(XY+o)	  */
            void xxcb_35() { Z80.HL.bl = SLL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SLL  L=(XY+o)	  */
            void xxcb_36() { cpu_writemem16((int)EA, SLL((byte)cpu_readmem16((int)EA))); } /* SLL  (XY+o)	  */
            void xxcb_37() { Z80.AF.bh = SLL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SLL  A=(XY+o)	  */

            void xxcb_38() { Z80.BC.bh = SRL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SRL  B=(XY+o)	  */
            void xxcb_39() { Z80.BC.bl = SRL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SRL  C=(XY+o)	  */
            void xxcb_3a() { Z80.DE.bh = SRL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SRL  D=(XY+o)	  */
            void xxcb_3b() { Z80.DE.bl = SRL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SRL  E=(XY+o)	  */
            void xxcb_3c() { Z80.HL.bh = SRL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SRL  H=(XY+o)	  */
            void xxcb_3d() { Z80.HL.bl = SRL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SRL  L=(XY+o)	  */
            void xxcb_3e() { cpu_writemem16((int)EA, SRL((byte)cpu_readmem16((int)EA))); } /* SRL  (XY+o)	  */
            void xxcb_3f() { Z80.AF.bh = SRL((byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SRL  A=(XY+o)	  */

            void xxcb_40() { xxcb_46(); } /* BIT  0,B=(XY+o)  */
            void xxcb_41() { xxcb_46(); } /* BIT	0,C=(XY+o)	*/
            void xxcb_42() { xxcb_46(); } /* BIT  0,D=(XY+o)  */
            void xxcb_43() { xxcb_46(); } /* BIT  0,E=(XY+o)  */
            void xxcb_44() { xxcb_46(); } /* BIT  0,H=(XY+o)  */
            void xxcb_45() { xxcb_46(); } /* BIT  0,L=(XY+o)  */
            void xxcb_46() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 0)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  0,(XY+o)	  */
            void xxcb_47() { xxcb_46(); } /* BIT  0,A=(XY+o)  */

            void xxcb_48() { xxcb_4e(); } /* BIT  1,B=(XY+o)  */
            void xxcb_49() { xxcb_4e(); } /* BIT	1,C=(XY+o)	*/
            void xxcb_4a() { xxcb_4e(); } /* BIT  1,D=(XY+o)  */
            void xxcb_4b() { xxcb_4e(); } /* BIT  1,E=(XY+o)  */
            void xxcb_4c() { xxcb_4e(); } /* BIT  1,H=(XY+o)  */
            void xxcb_4d() { xxcb_4e(); } /* BIT  1,L=(XY+o)  */
            void xxcb_4e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 1)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  1,(XY+o)	  */
            void xxcb_4f() { xxcb_4e(); } /* BIT  1,A=(XY+o)  */

            void xxcb_50() { xxcb_56(); } /* BIT  2,B=(XY+o)  */
            void xxcb_51() { xxcb_56(); } /* BIT	2,C=(XY+o)	*/
            void xxcb_52() { xxcb_56(); } /* BIT  2,D=(XY+o)  */
            void xxcb_53() { xxcb_56(); } /* BIT  2,E=(XY+o)  */
            void xxcb_54() { xxcb_56(); } /* BIT  2,H=(XY+o)  */
            void xxcb_55() { xxcb_56(); } /* BIT  2,L=(XY+o)  */
            void xxcb_56() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 2)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  2,(XY+o)	  */
            void xxcb_57() { xxcb_56(); } /* BIT  2,A=(XY+o)  */

            void xxcb_58() { xxcb_5e(); } /* BIT  3,B=(XY+o)  */
            void xxcb_59() { xxcb_5e(); } /* BIT	3,C=(XY+o)	*/
            void xxcb_5a() { xxcb_5e(); } /* BIT  3,D=(XY+o)  */
            void xxcb_5b() { xxcb_5e(); } /* BIT  3,E=(XY+o)  */
            void xxcb_5c() { xxcb_5e(); } /* BIT  3,H=(XY+o)  */
            void xxcb_5d() { xxcb_5e(); } /* BIT  3,L=(XY+o)  */
            void xxcb_5e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 3)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  3,(XY+o)	  */
            void xxcb_5f() { xxcb_5e(); } /* BIT  3,A=(XY+o)  */

            void xxcb_60() { xxcb_66(); } /* BIT  4,B=(XY+o)  */
            void xxcb_61() { xxcb_66(); } /* BIT	4,C=(XY+o)	*/
            void xxcb_62() { xxcb_66(); } /* BIT  4,D=(XY+o)  */
            void xxcb_63() { xxcb_66(); } /* BIT  4,E=(XY+o)  */
            void xxcb_64() { xxcb_66(); } /* BIT  4,H=(XY+o)  */
            void xxcb_65() { xxcb_66(); } /* BIT  4,L=(XY+o)  */
            void xxcb_66() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 4)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  4,(XY+o)	  */
            void xxcb_67() { xxcb_66(); } /* BIT  4,A=(XY+o)  */

            void xxcb_68() { xxcb_6e(); } /* BIT  5,B=(XY+o)  */
            void xxcb_69() { xxcb_6e(); } /* BIT	5,C=(XY+o)	*/
            void xxcb_6a() { xxcb_6e(); } /* BIT  5,D=(XY+o)  */
            void xxcb_6b() { xxcb_6e(); } /* BIT  5,E=(XY+o)  */
            void xxcb_6c() { xxcb_6e(); } /* BIT  5,H=(XY+o)  */
            void xxcb_6d() { xxcb_6e(); } /* BIT  5,L=(XY+o)  */
            void xxcb_6e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 5)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  5,(XY+o)	  */
            void xxcb_6f() { xxcb_6e(); } /* BIT  5,A=(XY+o)  */

            void xxcb_70() { xxcb_76(); } /* BIT  6,B=(XY+o)  */
            void xxcb_71() { xxcb_76(); } /* BIT	6,C=(XY+o)	*/
            void xxcb_72() { xxcb_76(); } /* BIT  6,D=(XY+o)  */
            void xxcb_73() { xxcb_76(); } /* BIT  6,E=(XY+o)  */
            void xxcb_74() { xxcb_76(); } /* BIT  6,H=(XY+o)  */
            void xxcb_75() { xxcb_76(); } /* BIT  6,L=(XY+o)  */
            void xxcb_76() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 6)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  6,(XY+o)	  */
            void xxcb_77() { xxcb_76(); } /* BIT  6,A=(XY+o)  */

            void xxcb_78() { xxcb_7e(); } /* BIT  7,B=(XY+o)  */
            void xxcb_79() { xxcb_7e(); } /* BIT	7,C=(XY+o)	*/
            void xxcb_7a() { xxcb_7e(); } /* BIT  7,D=(XY+o)  */
            void xxcb_7b() { xxcb_7e(); } /* BIT  7,E=(XY+o)  */
            void xxcb_7c() { xxcb_7e(); } /* BIT  7,H=(XY+o)  */
            void xxcb_7d() { xxcb_7e(); } /* BIT  7,L=(XY+o)  */
            void xxcb_7e() { Z80.AF.bl = (byte)((Z80.AF.bl & 0x01) | 0x10 | (SZ_BIT[(byte)cpu_readmem16((int)EA) & (1 << 7)] & ~(0x20 | 0x08)) | ((EA >> 8) & (0x20 | 0x08))); } /* BIT  7,(XY+o)	  */
            void xxcb_7f() { xxcb_7e(); } /* BIT  7,A=(XY+o)  */

            void xxcb_80() { Z80.BC.bh = RES(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  0,B=(XY+o)  */
            void xxcb_81() { Z80.BC.bl = RES(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  0,C=(XY+o)  */
            void xxcb_82() { Z80.DE.bh = RES(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  0,D=(XY+o)  */
            void xxcb_83() { Z80.DE.bl = RES(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  0,E=(XY+o)  */
            void xxcb_84() { Z80.HL.bh = RES(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  0,H=(XY+o)  */
            void xxcb_85() { Z80.HL.bl = RES(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  0,L=(XY+o)  */
            void xxcb_86() { cpu_writemem16((int)EA, RES(0, (byte)cpu_readmem16((int)EA))); } /* RES  0,(XY+o)	  */
            void xxcb_87() { Z80.AF.bh = RES(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  0,A=(XY+o)  */

            void xxcb_88() { Z80.BC.bh = RES(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  1,B=(XY+o)  */
            void xxcb_89() { Z80.BC.bl = RES(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  1,C=(XY+o)  */
            void xxcb_8a() { Z80.DE.bh = RES(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  1,D=(XY+o)  */
            void xxcb_8b() { Z80.DE.bl = RES(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  1,E=(XY+o)  */
            void xxcb_8c() { Z80.HL.bh = RES(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  1,H=(XY+o)  */
            void xxcb_8d() { Z80.HL.bl = RES(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  1,L=(XY+o)  */
            void xxcb_8e() { cpu_writemem16((int)EA, RES(1, (byte)cpu_readmem16((int)EA))); } /* RES  1,(XY+o)	  */
            void xxcb_8f() { Z80.AF.bh = RES(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  1,A=(XY+o)  */

            void xxcb_90() { Z80.BC.bh = RES(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  2,B=(XY+o)  */
            void xxcb_91() { Z80.BC.bl = RES(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  2,C=(XY+o)  */
            void xxcb_92() { Z80.DE.bh = RES(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  2,D=(XY+o)  */
            void xxcb_93() { Z80.DE.bl = RES(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  2,E=(XY+o)  */
            void xxcb_94() { Z80.HL.bh = RES(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  2,H=(XY+o)  */
            void xxcb_95() { Z80.HL.bl = RES(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  2,L=(XY+o)  */
            void xxcb_96() { cpu_writemem16((int)EA, RES(2, (byte)cpu_readmem16((int)EA))); } /* RES  2,(XY+o)	  */
            void xxcb_97() { Z80.AF.bh = RES(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  2,A=(XY+o)  */

            void xxcb_98() { Z80.BC.bh = RES(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  3,B=(XY+o)  */
            void xxcb_99() { Z80.BC.bl = RES(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  3,C=(XY+o)  */
            void xxcb_9a() { Z80.DE.bh = RES(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  3,D=(XY+o)  */
            void xxcb_9b() { Z80.DE.bl = RES(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  3,E=(XY+o)  */
            void xxcb_9c() { Z80.HL.bh = RES(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  3,H=(XY+o)  */
            void xxcb_9d() { Z80.HL.bl = RES(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  3,L=(XY+o)  */
            void xxcb_9e() { cpu_writemem16((int)EA, RES(3, (byte)cpu_readmem16((int)EA))); } /* RES  3,(XY+o)	  */
            void xxcb_9f() { Z80.AF.bh = RES(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  3,A=(XY+o)  */

            void xxcb_a0() { Z80.BC.bh = RES(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  4,B=(XY+o)  */
            void xxcb_a1() { Z80.BC.bl = RES(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  4,C=(XY+o)  */
            void xxcb_a2() { Z80.DE.bh = RES(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  4,D=(XY+o)  */
            void xxcb_a3() { Z80.DE.bl = RES(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  4,E=(XY+o)  */
            void xxcb_a4() { Z80.HL.bh = RES(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  4,H=(XY+o)  */
            void xxcb_a5() { Z80.HL.bl = RES(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  4,L=(XY+o)  */
            void xxcb_a6() { cpu_writemem16((int)EA, RES(4, (byte)cpu_readmem16((int)EA))); } /* RES  4,(XY+o)	  */
            void xxcb_a7() { Z80.AF.bh = RES(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  4,A=(XY+o)  */

            void xxcb_a8() { Z80.BC.bh = RES(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  5,B=(XY+o)  */
            void xxcb_a9() { Z80.BC.bl = RES(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  5,C=(XY+o)  */
            void xxcb_aa() { Z80.DE.bh = RES(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  5,D=(XY+o)  */
            void xxcb_ab() { Z80.DE.bl = RES(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  5,E=(XY+o)  */
            void xxcb_ac() { Z80.HL.bh = RES(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  5,H=(XY+o)  */
            void xxcb_ad() { Z80.HL.bl = RES(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  5,L=(XY+o)  */
            void xxcb_ae() { cpu_writemem16((int)EA, RES(5, (byte)cpu_readmem16((int)EA))); } /* RES  5,(XY+o)	  */
            void xxcb_af() { Z80.AF.bh = RES(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  5,A=(XY+o)  */

            void xxcb_b0() { Z80.BC.bh = RES(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  6,B=(XY+o)  */
            void xxcb_b1() { Z80.BC.bl = RES(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  6,C=(XY+o)  */
            void xxcb_b2() { Z80.DE.bh = RES(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  6,D=(XY+o)  */
            void xxcb_b3() { Z80.DE.bl = RES(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  6,E=(XY+o)  */
            void xxcb_b4() { Z80.HL.bh = RES(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  6,H=(XY+o)  */
            void xxcb_b5() { Z80.HL.bl = RES(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  6,L=(XY+o)  */
            void xxcb_b6() { cpu_writemem16((int)EA, RES(6, (byte)cpu_readmem16((int)EA))); } /* RES  6,(XY+o)	  */
            void xxcb_b7() { Z80.AF.bh = RES(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  6,A=(XY+o)  */

            void xxcb_b8() { Z80.BC.bh = RES(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* RES  7,B=(XY+o)  */
            void xxcb_b9() { Z80.BC.bl = RES(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* RES  7,C=(XY+o)  */
            void xxcb_ba() { Z80.DE.bh = RES(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* RES  7,D=(XY+o)  */
            void xxcb_bb() { Z80.DE.bl = RES(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* RES  7,E=(XY+o)  */
            void xxcb_bc() { Z80.HL.bh = RES(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* RES  7,H=(XY+o)  */
            void xxcb_bd() { Z80.HL.bl = RES(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* RES  7,L=(XY+o)  */
            void xxcb_be() { cpu_writemem16((int)EA, RES(7, (byte)cpu_readmem16((int)EA))); } /* RES  7,(XY+o)	  */
            void xxcb_bf() { Z80.AF.bh = RES(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* RES  7,A=(XY+o)  */

            void xxcb_c0() { Z80.BC.bh = SET(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  0,B=(XY+o)  */
            void xxcb_c1() { Z80.BC.bl = SET(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  0,C=(XY+o)  */
            void xxcb_c2() { Z80.DE.bh = SET(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  0,D=(XY+o)  */
            void xxcb_c3() { Z80.DE.bl = SET(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  0,E=(XY+o)  */
            void xxcb_c4() { Z80.HL.bh = SET(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  0,H=(XY+o)  */
            void xxcb_c5() { Z80.HL.bl = SET(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  0,L=(XY+o)  */
            void xxcb_c6() { cpu_writemem16((int)EA, SET(0, (byte)cpu_readmem16((int)EA))); } /* SET  0,(XY+o)	  */
            void xxcb_c7() { Z80.AF.bh = SET(0, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  0,A=(XY+o)  */

            void xxcb_c8() { Z80.BC.bh = SET(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  1,B=(XY+o)  */
            void xxcb_c9() { Z80.BC.bl = SET(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  1,C=(XY+o)  */
            void xxcb_ca() { Z80.DE.bh = SET(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  1,D=(XY+o)  */
            void xxcb_cb() { Z80.DE.bl = SET(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  1,E=(XY+o)  */
            void xxcb_cc() { Z80.HL.bh = SET(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  1,H=(XY+o)  */
            void xxcb_cd() { Z80.HL.bl = SET(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  1,L=(XY+o)  */
            void xxcb_ce() { cpu_writemem16((int)EA, SET(1, (byte)cpu_readmem16((int)EA))); } /* SET  1,(XY+o)	  */
            void xxcb_cf() { Z80.AF.bh = SET(1, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  1,A=(XY+o)  */

            void xxcb_d0() { Z80.BC.bh = SET(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  2,B=(XY+o)  */
            void xxcb_d1() { Z80.BC.bl = SET(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  2,C=(XY+o)  */
            void xxcb_d2() { Z80.DE.bh = SET(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  2,D=(XY+o)  */
            void xxcb_d3() { Z80.DE.bl = SET(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  2,E=(XY+o)  */
            void xxcb_d4() { Z80.HL.bh = SET(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  2,H=(XY+o)  */
            void xxcb_d5() { Z80.HL.bl = SET(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  2,L=(XY+o)  */
            void xxcb_d6() { cpu_writemem16((int)EA, SET(2, (byte)cpu_readmem16((int)EA))); } /* SET  2,(XY+o)	  */
            void xxcb_d7() { Z80.AF.bh = SET(2, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  2,A=(XY+o)  */

            void xxcb_d8() { Z80.BC.bh = SET(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  3,B=(XY+o)  */
            void xxcb_d9() { Z80.BC.bl = SET(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  3,C=(XY+o)  */
            void xxcb_da() { Z80.DE.bh = SET(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  3,D=(XY+o)  */
            void xxcb_db() { Z80.DE.bl = SET(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  3,E=(XY+o)  */
            void xxcb_dc() { Z80.HL.bh = SET(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  3,H=(XY+o)  */
            void xxcb_dd() { Z80.HL.bl = SET(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  3,L=(XY+o)  */
            void xxcb_de() { cpu_writemem16((int)EA, SET(3, (byte)cpu_readmem16((int)EA))); } /* SET  3,(XY+o)	  */
            void xxcb_df() { Z80.AF.bh = SET(3, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  3,A=(XY+o)  */

            void xxcb_e0() { Z80.BC.bh = SET(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  4,B=(XY+o)  */
            void xxcb_e1() { Z80.BC.bl = SET(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  4,C=(XY+o)  */
            void xxcb_e2() { Z80.DE.bh = SET(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  4,D=(XY+o)  */
            void xxcb_e3() { Z80.DE.bl = SET(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  4,E=(XY+o)  */
            void xxcb_e4() { Z80.HL.bh = SET(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  4,H=(XY+o)  */
            void xxcb_e5() { Z80.HL.bl = SET(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  4,L=(XY+o)  */
            void xxcb_e6() { cpu_writemem16((int)EA, SET(4, (byte)cpu_readmem16((int)EA))); } /* SET  4,(XY+o)	  */
            void xxcb_e7() { Z80.AF.bh = SET(4, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  4,A=(XY+o)  */

            void xxcb_e8() { Z80.BC.bh = SET(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  5,B=(XY+o)  */
            void xxcb_e9() { Z80.BC.bl = SET(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  5,C=(XY+o)  */
            void xxcb_ea() { Z80.DE.bh = SET(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  5,D=(XY+o)  */
            void xxcb_eb() { Z80.DE.bl = SET(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  5,E=(XY+o)  */
            void xxcb_ec() { Z80.HL.bh = SET(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  5,H=(XY+o)  */
            void xxcb_ed() { Z80.HL.bl = SET(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  5,L=(XY+o)  */
            void xxcb_ee() { cpu_writemem16((int)EA, SET(5, (byte)cpu_readmem16((int)EA))); } /* SET  5,(XY+o)	  */
            void xxcb_ef() { Z80.AF.bh = SET(5, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  5,A=(XY+o)  */

            void xxcb_f0() { Z80.BC.bh = SET(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  6,B=(XY+o)  */
            void xxcb_f1() { Z80.BC.bl = SET(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  6,C=(XY+o)  */
            void xxcb_f2() { Z80.DE.bh = SET(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  6,D=(XY+o)  */
            void xxcb_f3() { Z80.DE.bl = SET(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  6,E=(XY+o)  */
            void xxcb_f4() { Z80.HL.bh = SET(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  6,H=(XY+o)  */
            void xxcb_f5() { Z80.HL.bl = SET(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  6,L=(XY+o)  */
            void xxcb_f6() { cpu_writemem16((int)EA, SET(6, (byte)cpu_readmem16((int)EA))); } /* SET  6,(XY+o)	  */
            void xxcb_f7() { Z80.AF.bh = SET(6, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  6,A=(XY+o)  */

            void xxcb_f8() { Z80.BC.bh = SET(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bh); } /* SET  7,B=(XY+o)  */
            void xxcb_f9() { Z80.BC.bl = SET(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.BC.bl); } /* SET  7,C=(XY+o)  */
            void xxcb_fa() { Z80.DE.bh = SET(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bh); } /* SET  7,D=(XY+o)  */
            void xxcb_fb() { Z80.DE.bl = SET(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.DE.bl); } /* SET  7,E=(XY+o)  */
            void xxcb_fc() { Z80.HL.bh = SET(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bh); } /* SET  7,H=(XY+o)  */
            void xxcb_fd() { Z80.HL.bl = SET(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.HL.bl); } /* SET  7,L=(XY+o)  */
            void xxcb_fe() { cpu_writemem16((int)EA, SET(7, (byte)cpu_readmem16((int)EA))); } /* SET  7,(XY+o)	  */
            void xxcb_ff() { Z80.AF.bh = SET(7, (byte)cpu_readmem16((int)EA)); cpu_writemem16((int)EA, Z80.AF.bh); } /* SET  7,A=(XY+o)  */

            #endregion
            void take_interrupt()
            {
                if (Z80.IFF1 != 0)
                {
                    int irq_vector;

                    /* there isn't a valid previous program counter */
                    Z80.PREPC.d = 0xffffffff;

                    /* Check if processor was halted */
                    if (Z80.HALT != 0)
                    {
                        Z80.HALT = 0;
                        Z80.PC.wl++;
                    }

                    if (Z80.irq_max != 0)           /* daisy chain mode */
                    {
                        if (Z80.request_irq >= 0)
                        {
                            /* Clear both interrupt flip flops */
                            Z80.IFF1 = Z80.IFF2 = 0;
                            irq_vector = Z80.irq[Z80.request_irq].interrupt_entry(Z80.irq[Z80.request_irq].irq_param);
                            //LOG((errorlog, "Z80#%d daisy chain irq_vector $%02x\n", cpu_getactivecpu(), irq_vector));
                            Z80.request_irq = -1;
                        }
                        else return;
                    }
                    else
                    {
                        /* Clear both interrupt flip flops */
                        Z80.IFF1 = Z80.IFF2 = 0;
                        /* call back the cpu interface to retrieve the vector */
                        irq_vector = Z80.irq_callback(0);
                        //LOG((errorlog, "Z80#%d single int. irq_vector $%02x\n", cpu_getactivecpu(), irq_vector));
                    }

                    /* Interrupt mode 2. Call [Z80.I:databyte] */
                    if (Z80.IM == 2)
                    {
                        irq_vector = (irq_vector & 0xff) | (Z80.I << 8);
                        //PUSH(PC);
                        { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }
                        RM16((uint)irq_vector, ref Z80.PC);
                        // LOG((errorlog, "Z80#%d IM2 [$%04x] = $%04x\n",cpu_getactivecpu() , irq_vector, _PCD));
                        Z80.extra_cycles += 19;
                    }
                    else
                        /* Interrupt mode 1. RST 38h */
                        if (Z80.IM == 1)
                        {
                            // LOG((errorlog, "Z80#%d IM1 $0038\n",cpu_getactivecpu() ));
                            { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }
                            Z80.PC.d = 0x0038;
                            Z80.extra_cycles += 11 + 2; /* RST $38 + 2 cycles */
                        }
                        else
                        {
                            /* Interrupt mode 0. We check for CALL and JP instructions, */
                            /* if neither of these were found we assume a 1 byte opcode */
                            /* was placed on the databus                                */
                            //LOG((errorlog, "Z80#%d IM0 $%04x\n",cpu_getactivecpu() , irq_vector));
                            switch (irq_vector & 0xff0000)
                            {
                                case 0xcd0000:  /* call */
                                    { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }
                                    Z80.extra_cycles += 5;  /* CALL $xxxx cycles (JP $xxxx follows)*/
                                    goto case 0xc30000;
                                case 0xc30000:  /* jump */
                                    Z80.PC.d = (uint)irq_vector & 0xffff;
                                    Z80.extra_cycles += 10 + 2; /* JP $xxxx + 2 cycles */
                                    break;
                                default:        /* rst */
                                    { Z80.SP.wl -= 2; WM16(Z80.SP.d, Z80.PC); }
                                    Z80.PC.d = (uint)(irq_vector & 0x0038);
                                    Z80.extra_cycles += 11 + 2; /* RST $xx + 2 cycles */
                                    break;
                            }
                        }
                    change_pc(Z80.PC.d);
                }
            }
        }
    }
}
