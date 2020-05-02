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

        public static IEnumerable<ToneDescriptor> List()
        {
            yield return new ToneDescriptor { Name = "BASS", ShortName = "bass", Descriptor = "$[35715]BASS" };
            yield return new ToneDescriptor { Name = "OVERDRIVE", ShortName = "od", Descriptor = "$[35716]OVERDRIVE" };
            yield return new ToneDescriptor { Name = "OCTAVE", ShortName = "8va", Descriptor = "$[35719]OCTAVE" };
            yield return new ToneDescriptor { Name = "CLEAN", ShortName = "clean", Descriptor = "$[35720]CLEAN" };
            yield return new ToneDescriptor { Name = "ACOUSTIC", ShortName = "acoustic", Descriptor = "$[35721]ACOUSTIC" };
            yield return new ToneDescriptor { Name = "DISTORTION", ShortName = "dist", Descriptor = "$[35722]DISTORTION" };
            yield return new ToneDescriptor { Name = "CHORUS", ShortName = "chorus", Descriptor = "$[35723]CHORUS" };
            yield return new ToneDescriptor { Name = "LEAD", ShortName = "lead", Descriptor = "$[35724]LEAD" };
            yield return new ToneDescriptor { Name = "ROTARY", ShortName = "roto", Descriptor = "$[35725]ROTARY" };
            yield return new ToneDescriptor { Name = "REVERB", ShortName = "verb", Descriptor = "$[35726]REVERB" };
            yield return new ToneDescriptor { Name = "TREMOLO", ShortName = "trem", Descriptor = "$[35727]TREMOLO" };
            yield return new ToneDescriptor { Name = "VIBRATO", ShortName = "vib", Descriptor = "$[35728]VIBRATO" };
            yield return new ToneDescriptor { Name = "FILTER", ShortName = "filter", Descriptor = "$[35729]FILTER" };
            yield return new ToneDescriptor { Name = "PHASER", ShortName = "phaser", Descriptor = "$[35730]PHASER" };
            yield return new ToneDescriptor { Name = "FLANGER", ShortName = "flanger", Descriptor = "$[35731]FLANGER" };
            yield return new ToneDescriptor { Name = "LOW_OUTPUT", ShortName = "low", Descriptor = "$[35732]LOW OUTPUT" };
            yield return new ToneDescriptor { Name = "PROCESSED", ShortName = "comp", Descriptor = "$[35734]PROCESSED" };
            yield return new ToneDescriptor { Name = "SPECIAL_EFFECT", ShortName = "sitar", Descriptor = "$[35750]SPECIAL EFFECT" };
            yield return new ToneDescriptor { Name = "MULTI_EFFECT", ShortName = "multi", Descriptor = "$[35751]MULTI-EFFECT" };
            yield return new ToneDescriptor { Name = "DELAY", ShortName = "delay", Descriptor = "$[35753]DELAY" };
            yield return new ToneDescriptor { Name = "ECHO", ShortName = "echo", Descriptor = "$[35754]ECHO" };
            yield return new ToneDescriptor { Name = "HIGH_GAIN", ShortName = "high", Descriptor = "$[35755]HIGH GAIN" };
            yield return new ToneDescriptor { Name = "FUZZ", ShortName = "fuzz", Descriptor = "$[35756]FUZZ" };

            yield return new ToneDescriptor { Name = "* Vocal", ShortName = "vocal", Descriptor = "$[35718]VOCAL" };
            yield return new ToneDescriptor { Name = "* Direct", ShortName = "direct", Descriptor = "$[35752]DIRECT" };
            yield return new ToneDescriptor { Name = "* Effect", ShortName = "effect", Descriptor = "$[35733]EFFECT" };
            yield return new ToneDescriptor { Name = "** Crunch", ShortName = "crunch", Descriptor = "$[27156]CRUNCH" };

            yield return new ToneDescriptor { Name = "** Slap Bass", ShortName = "slap", Descriptor = "$[27151]SLAP_BASS" };

            yield return new ToneDescriptor { Name = "** Emulated", ShortName = "emu", Descriptor = "$[27119]EMULATED" };

            yield return new ToneDescriptor { Name = "** Banjo", ShortName = "banjo", Descriptor = "$[27201]BANJO" };
            yield return new ToneDescriptor { Name = "** Mandolin", ShortName = "mandolin", Descriptor = "$[27202]MANDOLIN" };
            yield return new ToneDescriptor { Name = "** Ukulele", ShortName = "uke", Descriptor = "$[27204]UKULELE" };
            yield return new ToneDescriptor { Name = "** Piano", ShortName = "piano", Descriptor = "$[29495]PIANO" };
        }
    }
}

