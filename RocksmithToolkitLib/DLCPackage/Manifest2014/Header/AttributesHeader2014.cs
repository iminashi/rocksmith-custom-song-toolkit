﻿using System;
using System.Reflection;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.AggregateGraph2014;
using RocksmithToolkitLib.DLCPackage.Manifest.Functions;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;
using System.Xml.Serialization;
using System.ComponentModel;

namespace RocksmithToolkitLib.DLCPackage.Manifest2014.Header
{
    [Serializable]
    public class AttributesHeader2014
    {
        public static readonly string URN_TEMPLATE = "urn:{0}:{1}:{2}";
        public static readonly string URN_TEMPLATE_SHORT = "urn:{0}:{1}";

        internal bool IsVocal = false;
        internal Song2014 song2014 = null;

        public string AlbumArt { get; set; }
        public string AlbumName { get; set; }
        public string AlbumNameSort { get; set; }
        public string ArrangementName { get; set; }
        public string ArtistName { get; set; }
        public string ArtistNameSort { get; set; }
        // apply conditional serialization to BassPick - see comments at bottom of page
        public int BassPick { get; set; } // added to resolve issue #272, header only
        public decimal CapoFret { get; set; }
        public double? CentOffset { get; set; } // tuning frequency, see Cents2Frequency method
        public bool DLC { get; set; } // tags DLC in setlist
        // usually DLCKey = SongKey, except that songs.psarc does not contain a DLCKey
        // in compatiblity packs DLCKey is always equal to RS1CompatibilityDisc
        // in ODLC/CDLC DLCKey is always equal to SongKey which is SongName with all spaces removed
        public string DLCKey { get; set; }
        public double? DNA_Chords { get; set; }
        public double? DNA_Riffs { get; set; }
        public double? DNA_Solo { get; set; }
        public double? EasyMastery { get; set; }
        // REM strings are not serialized if the value has not been initialized
        // new optional attributes
        public string JapaneseArtistName { get; set; } // Unicode string, be cautious
        public string JapaneseSongName { get; set; } // Unicode string, be cautious
        public bool JapaneseVocal { get; set; }
        //
        public int LeaderboardChallengeRating { get; set; }
        public string ManifestUrn { get; set; }
        public int MasterID_RDV { get; set; }
        public int? Metronome { get; set; } // see comments at bottom
        public double? MediumMastery { get; set; }
        public double? NotesEasy { get; set; }
        public double? NotesHard { get; set; }
        public double? NotesMedium { get; set; }
        public int? Representative { get; set; } // Header only
        public int? RouteMask { get; set; } // Header only
        public bool Shipping { get; set; } // false = hidden arrangement
        public string SKU { get; set; } // determines tag text in setlist
        public double? SongDiffEasy { get; set; }
        public double? SongDiffHard { get; set; }
        public double? SongDiffMed { get; set; }
        public double? SongDifficulty { get; set; }
        // usually SongKey = DLCKey, except that songs.psarc does not contain a DLCKey
        // usually SongKey is the SongName with all spaces removed
        public string SongKey { get; set; }
        public double? SongLength { get; set; }
        public string SongName { get; set; }
        public string SongNameSort { get; set; }
        public int? SongYear { get; set; }
        public TuningStrings Tuning { get; set; }
        public string PersistentID { get; set; }

        public AttributesHeader2014() { }

        public AttributesHeader2014(Attributes2014 attributes)
        {
            foreach (PropertyInfo prop in attributes.GetType().GetProperties())
                if (GetType().GetProperty(prop.Name) != null)
                    GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(attributes, null), null);
        }

        public AttributesHeader2014(string arrangementFileName, Arrangement arrangement, DLCPackageData info, Platform platform)
        {
            if (arrangement.ArrangementType == ArrangementType.ShowLight)
                return;

            IsVocal = arrangement.ArrangementType == ArrangementType.Vocal;
            song2014 = (IsVocal) ? null : Song2014.LoadFromFile(arrangement.SongXml.File);
            var dlcName = info.Name.ToLower();

            var albumUrn = String.Format(URN_TEMPLATE, TagValue.Image.GetDescription(), TagValue.DDS.GetDescription(), String.Format("album_{0}", dlcName));
            var jsonUrn = String.Format(URN_TEMPLATE, TagValue.Database.GetDescription(), TagValue.JsonDB.GetDescription(), String.Format("{0}_{1}", dlcName, arrangementFileName));

            //FILL ATTRIBUTES
            this.AlbumArt = albumUrn;
            JapaneseVocal |= arrangement.ArrangementName == Sng.ArrangementName.JVocals;
            ArrangementName = IsVocal ? Sng.ArrangementName.Vocals.ToString() : arrangement.ArrangementName.ToString(); //HACK: weird vocals stuff
            DLCKey = info.Name; // in RS2 DLCKey = SongKey, in RS1 they are different
            SongKey = info.Name;
            LeaderboardChallengeRating = 0;
            ManifestUrn = jsonUrn;
            MasterID_RDV = arrangement.MasterId; // must be unique else in-game hang occures
            PersistentID = arrangement.Id.ToString().Replace("-", "").ToUpper();
            Shipping = true; // false = hides the arrangement in-game

            // DLC property controls whether album artwork marker is shown in-game setlist, but
            // has side effect that 'Alternate Arrangements' are locked for newer player profiles
            DLC = true; // false = requires UPlay to unlock

            // TODO: monitor this change
            //if (info.ToolkitInfo == null || info.ToolkitInfo.PackageAuthor == "Ubisoft")
                SKU = "RS2"; // shows purple marker w/ "DLC" text overlay
            //else
            //    SKU = "RS2"; // hides album artwork marker in-game setlist

            // this SKU and DLCKey combination shows black marker w/ "RS1" text overlay on album artwork in-game setlist
            // SKU = "RS1";
            // DLCKey = "RS1CompatibilityDisc";

            if (IsVocal)
                return;

            // added better AlbumNameSort feature
            AlbumName = info.SongInfo.Album;
            AlbumNameSort = info.SongInfo.AlbumSort;
            ArtistName = info.SongInfo.Artist;
            CentOffset = (!arrangement.TuningPitch.Equals(0)) ? TuningFrequency.Frequency2Cents(arrangement.TuningPitch) : 0.0;
            ArtistNameSort = info.SongInfo.ArtistSort;
            BassPick = arrangement.ArrangementType == ArrangementType.Bass ? (int)arrangement.PluckedType : 0;
            CapoFret = (arrangement.Sng2014.Metadata.CapoFretId == 0xFF) ? CapoFret = 0 : Convert.ToDecimal(arrangement.Sng2014.Metadata.CapoFretId);
            DNA_Chords = arrangement.Sng2014.DNACount[(int)DNAId.Chord];
            DNA_Riffs = arrangement.Sng2014.DNACount[(int)DNAId.Riff];
            DNA_Solo = arrangement.Sng2014.DNACount[(int)DNAId.Solo];
            NotesEasy = arrangement.Sng2014.NoteCount[0];
            NotesMedium = arrangement.Sng2014.NoteCount[1];
            NotesHard = arrangement.Sng2014.NoteCount[2];
            EasyMastery = Math.Round((double)(NotesEasy / NotesHard), 9);
            MediumMastery = Math.Round((double)(NotesMedium / NotesHard), 9);
            Metronome = arrangement.Metronome == Sng.Metronome.None ? null : (int?)arrangement.Metronome;

            // TODO: monitor this change
            Representative = arrangement.ArrangementPropeties.Represent;

            RouteMask = (int)arrangement.RouteMask;

            ManifestFunctions.GetSongDifficulty(this, song2014);

            SongLength = Math.Round(song2014.SongLength, 3, MidpointRounding.AwayFromZero);
            SongName = info.SongInfo.SongDisplayName;
            SongNameSort = info.SongInfo.SongDisplayNameSort;
            SongYear = info.SongInfo.SongYear;

            //Detect tuning
            var tuning = TuningDefinitionRepository.Instance.Detect(song2014.Tuning, platform.version, arrangement.ArrangementType == ArrangementType.Bass);
            Tuning = tuning.Tuning; //can we just use SongContent.Tuning

            // optional properties
            JapaneseSongName = string.IsNullOrEmpty(info.SongInfo.JapaneseSongName) ? null : info.SongInfo.JapaneseSongName;
            JapaneseArtistName = string.IsNullOrEmpty(info.SongInfo.JapaneseArtistName) ? null : info.SongInfo.JapaneseArtistName;            
        }

        // Conditionally serialized properties
        // see www.geekytidbits.com/conditional-serialization-with-json-net
        // Newtonsoft.Json will look for these methods at runtime

        // Only serialize if picked and header ("BassPick: 0" never found in ODLC headers)
        public bool ShouldSerializeBassPick()
        {
            return BassPick == 1 && !(this is Attributes2014);
        }
        // Only serialize if header
        public bool ShouldSerializeRepresentative()
        {
            return !(this is Attributes2014);
        }
        public bool ShouldSerializeRouteMask()
        {
            return !(this is Attributes2014);
        }
        // Only serialize if true
        public bool ShouldSerializeJapaneseVocal()
        {
            return JapaneseVocal;
        }
    }
}
