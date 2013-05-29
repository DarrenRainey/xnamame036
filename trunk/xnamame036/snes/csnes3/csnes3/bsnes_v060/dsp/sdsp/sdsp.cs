using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class sDSP : DSP
        {
            const int echo_hist_size = 8, brr_buf_size = 12, brr_block_size = 9;
            enum env_mode_t { env_release, env_attack, env_decay, env_sustain };

            uint phase_index;
            class State
            {
                public byte[] regs = new byte[128];

                public ModuloArray[] echo_hist = new ModuloArray[2]; //echo history keeps most recent 8 samples
                public int echo_hist_pos;

                public bool every_other_sample; //toggles every sample
                public int kon;                 //KON value when last checked
                public int noise;
                public int counter;
                public int echo_offset;         //offset from ESA in echo buffer
                public int echo_length;         //number of bytes that echo_offset will stop at

                //hidden registers also written to when main register is written to
                public int new_kon;
                public int endx_buf;
                public int envx_buf;
                public int outx_buf;

                //temporary state between clocks

                //read once per sample
                public int t_pmon;
                public int t_non;
                public int t_eon;
                public int t_dir;
                public int t_koff;

                //read a few clocks ahead before used
                public int t_brr_next_addr;
                public int t_adsr0;
                public int t_brr_header;
                public int t_brr_byte;
                public int t_srcn;
                public int t_esa;
                public int t_echo_disabled;

                //internal state that is recalculated every sample
                public int t_dir_addr;
                public int t_pitch;
                public int t_output;
                public int t_looped;
                public int t_echo_ptr;

                //left/right sums
                public int[] t_main_out = new int[2];
                public int[] t_echo_out = new int[2];
                public int[] t_echo_in = new int[2];
            }
            State state = new State();
            class Voice
            {
                public ModuloArray buffer = new ModuloArray(brr_buf_size); //decoded samples
                public int buf_pos;         //place in buffer where next samples will be decoded
                public int interp_pos;      //relative fractional position in sample (0x1000 = 1.0)
                public int brr_addr;        //address of current BRR block
                public int brr_offset;      //current decoding offset in BRR block
                public int vbit;            //bitmask for voice: 0x01 for voice 0, 0x02 for voice 1, etc
                public int vidx;            //voice channel register index: 0x00 for voice 0, 0x10 for voice 1, etc
                public int kon_delay;       //KON delay/current setup phase
                public int env_mode;
                public int env;             //current envelope level
                public int t_envx_out;
                public int hidden_env;      //used by GAIN mode 7, very obscure quirk
            }
            Voice[] voice = new Voice[8];
            public void serialize(Serializer s)
            {
                base.serialize(s);

                s.integer(phase_index);

                s.array(state.regs, 128);
                state.echo_hist[0].serialize(s);
                state.echo_hist[1].serialize(s);
                s.integer(state.echo_hist_pos);

                s.integer(state.every_other_sample);
                s.integer(state.kon);
                s.integer(state.noise);
                s.integer(state.counter);
                s.integer(state.echo_offset);
                s.integer(state.echo_length);

                s.integer(state.new_kon);
                s.integer(state.endx_buf);
                s.integer(state.envx_buf);
                s.integer(state.outx_buf);

                s.integer(state.t_pmon);
                s.integer(state.t_non);
                s.integer(state.t_eon);
                s.integer(state.t_dir);
                s.integer(state.t_koff);

                s.integer(state.t_brr_next_addr);
                s.integer(state.t_adsr0);
                s.integer(state.t_brr_header);
                s.integer(state.t_brr_byte);
                s.integer(state.t_srcn);
                s.integer(state.t_esa);
                s.integer(state.t_echo_disabled);

                s.integer(state.t_dir_addr);
                s.integer(state.t_pitch);
                s.integer(state.t_output);
                s.integer(state.t_looped);
                s.integer(state.t_echo_ptr);

                s.integer(state.t_main_out[0]);
                s.integer(state.t_main_out[1]);
                s.integer(state.t_echo_out[0]);
                s.integer(state.t_echo_out[1]);
                s.integer(state.t_echo_in[0]);
                s.integer(state.t_echo_in[1]);

                for (uint n = 0; n < 8; n++)
                {
                    voice[n].buffer.serialize(s);
                    s.integer(voice[n].buf_pos);
                    s.integer(voice[n].interp_pos);
                    s.integer(voice[n].brr_addr);
                    s.integer(voice[n].brr_offset);
                    s.integer(voice[n].vbit);
                    s.integer(voice[n].vidx);
                    s.integer(voice[n].kon_delay);
                    s.integer(voice[n].env_mode);
                    s.integer(voice[n].env);
                    s.integer(voice[n].t_envx_out);
                    s.integer(voice[n].hidden_env);
                }
            }
        }
    }
}
