using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class MachineSound
        {
            public int sound_type;
            public object sound_interface;
            public MachineSound(int sound_type, object sound_interface) { this.sound_type = sound_type; this.sound_interface = sound_interface; }
        }
        static Timer.timer_entry sound_update_timer;
        double snd_refresh_period;

        public abstract class snd_interface
        {
            public uint sound_num;
            public string name;
            public abstract int chips_num(MachineSound msound);
            public abstract int chips_clock(MachineSound msound);
            public abstract int start(MachineSound msound);
            public abstract void stop();
            public abstract void update();
            public abstract void reset();
        }
        class SOUND_DUMMY : snd_interface
        {
            public override int chips_clock(MachineSound msound)
            {
                throw new NotImplementedException();
            }
            public override int chips_num(MachineSound msound)
            {
                throw new NotImplementedException();
            }
            public override int start(MachineSound msound)
            {
                throw new NotImplementedException();
            }
            public override void stop()
            {
                throw new NotImplementedException();
            }
            public override void reset()
            {
                throw new NotImplementedException();
            }
            public override void update()
            {
                throw new NotImplementedException();
            }
        }

        const byte SOUND_COUNT = 10;
        public const byte SOUND_NAMCO = 1;
        public const byte SOUND_SAMPLES = 2;
        public const byte SOUND_AY8910 = 3;
        public const byte SOUND_YM2203 = 4;
        public const byte SOUND_DAC = 5;
        public const byte SOUND_SN76496 = 6;
        public const byte SOUND_OKIM6295 = 7;
        public const byte SOUND_YM2413 = 8;
        public const byte SOUND_YM2151 = 9;
        public const byte SOUND_K007232 = 10;
        public const byte SOUND_POKEY = 11;
        public const byte SOUND_YM3812 = 12;
        public const byte SOUND_CUSTOM = 13;

        public const byte SOUND_SEGAPCM = 14;

        public delegate int CustomSoundStart(MachineSound msound);
        public delegate void CustomSoundHandler();

        public class CustomSoundInterface : snd_interface
        {
            public CustomSoundStart SoundStart;
            public CustomSoundHandler SoundStop, SoundUpdate;
            public override int chips_clock(MachineSound msound)
            {
                throw new NotImplementedException();
            }
            public override int chips_num(MachineSound msound)
            {
                throw new NotImplementedException();
            }
            public override void reset()
            {
                //nothing
            }
            public override int start(MachineSound msound)
            {
                if (SoundStart != null) return SoundStart(msound); else return 0;                
            }
            public override void stop()
            {
                if (SoundStop != null)SoundStop();
            }
            public override void update()
            {
                if (SoundUpdate != null)SoundUpdate();
            }
        }
        
        public static snd_interface[] sndintf = {
                                      new SOUND_DUMMY(),
                                      new Namco(),
                                      new Samples(),
                                      new ay8910(),                                      
                                      new ym2203(),
                                      new DAC(),
                                      new SN76496(),
                                      new okim6295(),
                                      new ym2413(),
                                      new YM2151(),
                                      new K007232(),
                                      new Pokey(),
                                      new YM3812(),
                                      new CustomSoundInterface(),
                                  };
        int sound_start()
        {
            int totalsound = 0;

            /* Verify the order of entries in the sndintf[] array */
            for (int i = 0; i < SOUND_COUNT; i++)
            {
                if (sndintf[i].sound_num != i)
                {
                    printf("Sound #%d wrong ID %d: check enum SOUND_... in src/sndintrf.h!\n", i, sndintf[i].sound_num);
                    return 1;
                }
            }


            /* samples will be read later if needed */
            Machine.samples = null;

            snd_refresh_period = Timer.TIME_IN_HZ(Machine.drv.frames_per_second);
            refresh_period_inv = 1.0 / refresh_period;
            sound_update_timer = Timer.timer_set(Timer.TIME_NEVER, 0, null);

            if (mixer_sh_start() != 0)
                return 1;

            if (streams_sh_start() != 0)
                return 1;

            while (totalsound < Machine.drv.sound.Count && Machine.drv.sound[totalsound].sound_type != 0 && totalsound < MAX_SOUND)
            {
                if (Machine.drv.sound[totalsound].sound_type == SOUND_CUSTOM)
                {
                    ((CustomSoundInterface)sndintf[SOUND_CUSTOM]).SoundStart = ((snd_interface)Machine.drv.sound[totalsound].sound_interface).start;
                    ((CustomSoundInterface)sndintf[SOUND_CUSTOM]).SoundStop = ((snd_interface)Machine.drv.sound[totalsound].sound_interface).stop;
                    ((CustomSoundInterface)sndintf[SOUND_CUSTOM]).SoundUpdate = ((snd_interface)Machine.drv.sound[totalsound].sound_interface).update;
                }
                if ((sndintf[Machine.drv.sound[totalsound].sound_type].start)(Machine.drv.sound[totalsound]) != 0)
                    goto getout;

                totalsound++;
            }

            return 0;

        getout:
            /* TODO: should also free the resources allocated before */
            return 1;
        }
        void sound_stop()
        {
            int totalsound = 0;

            while (totalsound < Machine.drv.sound.Count && Machine.drv.sound[totalsound].sound_type != 0 && totalsound < MAX_SOUND)
            {
                sndintf[Machine.drv.sound[totalsound].sound_type].stop();

                totalsound++;
            }

            streams_sh_stop();
            mixer_sh_stop();

            if (sound_update_timer != null)
            {
                Timer.timer_remove(sound_update_timer);
                sound_update_timer = null;
            }

            /* free audio samples */
            freesamples(ref Machine.samples);
            Machine.samples = null;
        }
        public static string sound_name(MachineSound msound)
        {
            if (msound.sound_type < SOUND_COUNT)
                return sndintf[msound.sound_type].name;
            else
                return "";
        }
        int sound_num(MachineSound msound)
        {
            if (msound.sound_type < SOUND_COUNT)// && sndintf[msound.sound_type].chips_num)
                return sndintf[msound.sound_type].chips_num(msound);
            else
                return 0;
        }
        int sound_clock(MachineSound msound)
        {
            if (msound.sound_type < SOUND_COUNT)// && sndintf[msound.sound_type].chips_clock)
                return sndintf[msound.sound_type].chips_clock(msound);
            else
                return 0;
        }
        void sound_reset()
        {
            int totalsound = 0;
            while (totalsound < Machine.drv.sound.Count && Machine.drv.sound[totalsound].sound_type != 0 && totalsound < MAX_SOUND && totalsound < Machine.drv.sound.Count)
            {
                sndintf[Machine.drv.sound[totalsound].sound_type].reset();
                totalsound++;
            }
        }
        void sound_update()
        {
            int totalsound = 0;

            while (totalsound < Machine.drv.sound.Count && Machine.drv.sound[totalsound].sound_type != 0 && totalsound < MAX_SOUND)
            {
                sndintf[Machine.drv.sound[totalsound].sound_type].update();

                totalsound++;
            }

            streams_sh_update();
            mixer_sh_update();

            Timer.timer_reset(sound_update_timer, Timer.TIME_NEVER);
        }
        public static int sound_scalebufferpos(int value)
        {
            int result = (int)((double)value * Timer.timer_timeelapsed(sound_update_timer) * refresh_period_inv);
            if (value >= 0) return (result < value) ? result : value;
            else return (result > value) ? result : value;
        }
        static int cleared_value = 0x00;
        static int latch;
        static void soundlatch_callback(int param)
        {
            latch = param;
        }
        public static void soundlatch_w(int offset, int data)
        {
            /* make all the CPUs synchronize, and only AFTER that write the new command to the latch */
            Timer.timer_set(Timer.TIME_NOW, data, soundlatch_callback);
        }
        public static void soundlatch2_w(int offset, int data)
        {
            /* make all the CPUs synchronize, and only AFTER that write the new command to the latch */
            Timer.timer_set(Timer.TIME_NOW, data, soundlatch2_callback);
        }
        static int latch2;

        static void soundlatch2_callback(int param)
        {
            latch2 = param;
        }
        public static int soundlatch_r(int offset)
        {
            return latch;
        }
        static void soundlatch_clear_w(int offset, int data)
        {
            latch = cleared_value;
        }
        public static int soundlatch2_r(int offset)
        {
            return latch2;
        }

    }
}
