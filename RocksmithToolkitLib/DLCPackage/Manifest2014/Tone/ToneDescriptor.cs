using System.Collections.Generic;

namespace RocksmithToolkitLib.DLCPackage.Manifest2014.Tone
{
    public class ToneDescriptor
    {
        public string Name;
        public string ShortName;
        public string Descriptor;

        public override string ToString()
        {
            return Name;
        }

        public static List<ToneDescriptor> AllDescriptors { get; } = new List<ToneDescriptor>
        {
            new ToneDescriptor { Name = "Acoustic", ShortName = "acoustic", Descriptor = "$[35721]ACOUSTIC" },
            new ToneDescriptor { Name = "Bass", ShortName = "bass", Descriptor = "$[35715]BASS" },
            new ToneDescriptor { Name = "Chorus", ShortName = "chorus", Descriptor = "$[35723]CHORUS" },
            new ToneDescriptor { Name = "Clean", ShortName = "clean", Descriptor = "$[35720]CLEAN" },
            new ToneDescriptor { Name = "Delay", ShortName = "delay", Descriptor = "$[35753]DELAY" },
            new ToneDescriptor { Name = "Distortion", ShortName = "dist", Descriptor = "$[35722]DISTORTION" },
            new ToneDescriptor { Name = "Echo", ShortName = "echo", Descriptor = "$[35754]ECHO" },
            new ToneDescriptor { Name = "Filter", ShortName = "filter", Descriptor = "$[35729]FILTER" },
            new ToneDescriptor { Name = "Flanger", ShortName = "flanger", Descriptor = "$[35731]FLANGER" },
            new ToneDescriptor { Name = "Fuzz", ShortName = "fuzz", Descriptor = "$[35756]FUZZ" },
            new ToneDescriptor { Name = "High Gain", ShortName = "high", Descriptor = "$[35755]HIGH GAIN" },
            new ToneDescriptor { Name = "Lead", ShortName = "lead", Descriptor = "$[35724]LEAD" },
            new ToneDescriptor { Name = "Low Output", ShortName = "low", Descriptor = "$[35732]LOW OUTPUT" },
            new ToneDescriptor { Name = "Multi Effect", ShortName = "multi", Descriptor = "$[35751]MULTI-EFFECT" },
            new ToneDescriptor { Name = "Octave", ShortName = "8va", Descriptor = "$[35719]OCTAVE" },
            new ToneDescriptor { Name = "Overdrive", ShortName = "od", Descriptor = "$[35716]OVERDRIVE" },
            new ToneDescriptor { Name = "Phaser", ShortName = "phaser", Descriptor = "$[35730]PHASER" },
            new ToneDescriptor { Name = "Processed", ShortName = "comp", Descriptor = "$[35734]PROCESSED" },
            new ToneDescriptor { Name = "Reverb", ShortName = "verb", Descriptor = "$[35726]REVERB" },
            new ToneDescriptor { Name = "Rotary", ShortName = "roto", Descriptor = "$[35725]ROTARY" },
            new ToneDescriptor { Name = "Special Effect", ShortName = "sitar", Descriptor = "$[35750]SPECIAL EFFECT" },
            new ToneDescriptor { Name = "Tremolo", ShortName = "trem", Descriptor = "$[35727]TREMOLO" },
            new ToneDescriptor { Name = "Vibrato", ShortName = "vib", Descriptor = "$[35728]VIBRATO" },

            new ToneDescriptor { Name = "* Vocal", ShortName = "vocal", Descriptor = "$[35718]VOCAL" },
            new ToneDescriptor { Name = "* Direct", ShortName = "direct", Descriptor = "$[35752]DIRECT" },
            new ToneDescriptor { Name = "* Effect", ShortName = "effect", Descriptor = "$[35733]EFFECT" },
            new ToneDescriptor { Name = "** Crunch", ShortName = "crunch", Descriptor = "$[27156]CRUNCH" },

            new ToneDescriptor { Name = "** Slap Bass", ShortName = "slap", Descriptor = "$[27151]SLAP_BASS" },

            new ToneDescriptor { Name = "** Emulated", ShortName = "emu", Descriptor = "$[27119]EMULATED" },

            new ToneDescriptor { Name = "** Banjo", ShortName = "banjo", Descriptor = "$[27201]BANJO" },
            new ToneDescriptor { Name = "** Mandolin", ShortName = "mandolin", Descriptor = "$[27202]MANDOLIN" },
            new ToneDescriptor { Name = "** Ukulele", ShortName = "uke", Descriptor = "$[27204]UKULELE" },
            new ToneDescriptor { Name = "** Piano", ShortName = "piano", Descriptor = "$[29495]PIANO" }
        };
    }
}

