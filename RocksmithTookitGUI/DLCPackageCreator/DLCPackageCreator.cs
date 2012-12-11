﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Tone;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public partial class DLCPackageCreator : UserControl
    {
        public DLCPackageCreator()
        {
            InitializeComponent();
            var dammit = new SongAppId { Name = "Blink 182 - Dammit", AppId = "206113" };
            cmbAppIds.Items.Add(new SongAppId { Name = "Avenged Sevenfold - Afterlife", AppId = "206175" });
            cmbAppIds.Items.Add(new SongAppId { Name = "Avenged Sevenfold - Beast and the Harlot", AppId = "206174" });
            cmbAppIds.Items.Add(new SongAppId { Name = "Avenged Sevenfold - Nightmare", AppId = "206176" });
            cmbAppIds.Items.Add(dammit);
            cmbAppIds.Items.Add(new SongAppId { Name = "Blue Oyster Cult - Godzilla", AppId = "206149" });
            cmbAppIds.Items.Add(new SongAppId { Name = "Pantera - Cowboys From Hell", AppId = "222054" });
            cmbAppIds.Items.Add(new SongAppId { Name = "Pantera - Domination", AppId = "222055" });
            cmbAppIds.Items.Add(new SongAppId { Name = "Pantera - Walk", AppId = "222056" });
            cmbAppIds.SelectedItem = dammit;
            AppIdTB.Text = dammit.AppId;
        }

        private string OggPath
        {
            get { return OggPathTB.Text; }
            set { OggPathTB.Text = value; }
        }
        private string AlbumArtPath
        {
            get { return AlbumArtPathTB.Text; }
            set { AlbumArtPathTB.Text = value; }
        }

        private void arrangementAddButton_Click(object sender, EventArgs e)
        {
            Arrangement arrangement;
            using (var form = new ArrangementForm())
            {
                form.ShowDialog();
                arrangement = form.Arrangement;
            }
            if (arrangement == null)
                return;
            ArrangementLB.Items.Add(arrangement);
        }

        private void arrangementRemoveButton_Click(object sender, EventArgs e)
        {
            if (ArrangementLB.SelectedItem != null)
                ArrangementLB.Items.Remove(ArrangementLB.SelectedItem);
        }

        private void openOggButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Fixed WWise Files|*.ogg";
                if (ofd.ShowDialog() == DialogResult.OK)
                    OggPath = ofd.FileName;
            }
        }

        private void dlcGenerateButton_Click(object sender, EventArgs e)
        {
            var packageData = GetPackageData();
            if (packageData == null)
            {
                MessageBox.Show("One or more fields are missing information.", "DLC Package Creator");
                return;
            }
            string dlcSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = "Rocksmith DLC|*.dat";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }
            RocksmithToolkitLib.DLCPackage.DLCPackageCreator.Generate(dlcSavePath, packageData);

            MessageBox.Show("Package was generated.", "DLC Package Creator");
        }

        private void albumArtButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "dds Files|*.dds";
                if (ofd.ShowDialog() == DialogResult.OK)
                    AlbumArtPath = ofd.FileName;
            }
        }

        private void dlcSaveButton_Click(object sender, EventArgs e)
        {
            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            string dlcSavePath;
            using (var ofd = new SaveFileDialog())
            {
                ofd.Filter = "Rocksmith DLC Template|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }
            var path = new Uri(Path.GetDirectoryName(dlcSavePath) + Path.DirectorySeparatorChar);

            var packageData = GetPackageData();
            if (packageData == null)
            {
                MessageBox.Show("One or more fields are missing information.", "DLC Package Creator");
                return;
            }

            //Make the paths relative
            if (!string.IsNullOrEmpty(packageData.AlbumArtPath))
            {
                packageData.AlbumArtPath = path.MakeRelativeUri(new Uri(packageData.AlbumArtPath)).ToString();
            }
            packageData.OggPath = path.MakeRelativeUri(new Uri(packageData.OggPath)).ToString();
            foreach (var arr in packageData.Arrangements)
            {
                arr.SongFile.File = path.MakeRelativeUri(new Uri(arr.SongFile.File)).ToString();
                arr.SongXml.File = path.MakeRelativeUri(new Uri(arr.SongXml.File)).ToString();
            }
            var serializer = new DataContractSerializer(typeof(DLCPackageData));
            using (var stm = new XmlTextWriter(dlcSavePath, Encoding.Default))
            {
                serializer.WriteObject(stm, packageData);
            }
            //Re-absolutize the paths
            foreach (var arr in packageData.Arrangements)
            {
                arr.SongFile.File = MakeAbsolute(path, arr.SongFile.File);
                arr.SongXml.File = MakeAbsolute(path, arr.SongXml.File);
            }
            MessageBox.Show("DLC Package template was saved.", "DLC Package Creator");
        }

        private void dlcLoadButton_Click(object sender, EventArgs e)
        {

            string dlcSavePath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Rocksmith DLC Template|*.dlc.xml";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dlcSavePath = ofd.FileName;
            }

            DLCPackageData info;

            var serializer = new DataContractSerializer(typeof(DLCPackageData));
            using (var stm = new XmlTextReader(dlcSavePath))
            {
                info = (DLCPackageData)serializer.ReadObject(stm);
            }

            var path = new Uri(Path.GetDirectoryName(dlcSavePath) + Path.DirectorySeparatorChar);

            DlcNameTB.Text = info.Name;
            AppIdTB.Text = info.AppId;
            AlbumTB.Text = info.SongInfo.Album;
            SongDisplayNameTB.Text = info.SongInfo.SongDisplayName;
            YearTB.Text = info.SongInfo.SongYear.ToString();
            ArtistTB.Text = info.SongInfo.Artist;
            AverageTempo.Text = info.SongInfo.AverageTempo.ToString();

            AlbumArtPath = MakeAbsolute(path, info.AlbumArtPath);
            OggPath = MakeAbsolute(path, info.OggPath);
            ArrangementLB.Items.Clear();
            foreach (var arrangement in info.Arrangements)
            {
                arrangement.SongFile.File = MakeAbsolute(path, arrangement.SongFile.File);
                arrangement.SongXml.File = MakeAbsolute(path, arrangement.SongXml.File);
                ArrangementLB.Items.Add(arrangement);
            }
            toneControl.Tone.PedalList.Clear();
            foreach (var pedal in info.Tone.PedalList)
            {
                toneControl.Tone.PedalList[pedal.Key] = pedal.Value;
            }
            toneControl.Tone.Name = info.Tone.Name;
            toneControl.Tone.Volume = info.Tone.Volume;
            toneControl.RefreshControls();

            MessageBox.Show("DLC Package template was loaded.", "DLC Package Creator");
        }

        private string MakeAbsolute(Uri baseUri, string path) {
            return new Uri(baseUri, path).AbsolutePath.Replace("%25", "%").Replace("%20", " ");
        }

        private DLCPackageData GetPackageData()
        {
            int year, tempo;
            if(string.IsNullOrEmpty(DlcNameTB.Text)) {
                DlcNameTB.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(SongDisplayNameTB.Text))
            {
                SongDisplayNameTB.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(AlbumTB.Text))
            {
                AlbumTB.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(ArtistTB.Text))
            {
                ArtistTB.Focus();
                return null;
            }
            if (!int.TryParse(YearTB.Text, out year))
            {
                YearTB.Focus();
                return null;
            }
            if (!int.TryParse(AverageTempo.Text, out tempo))
            {
                AverageTempo.Focus();
                return null;
            }
            if (string.IsNullOrEmpty(AppIdTB.Text))
            {
                AppIdTB.Focus();
                return null;
            }
            if (!File.Exists(OggPath))
            {
                OggPathTB.Focus();
                return null;
            }
            var arrangements = ArrangementLB.Items.OfType<Arrangement>().ToList();
            if (arrangements.Count(x => x.IsVocal) > 1)
            {
                MessageBox.Show("Error: Multiple Vocals Found");
                return null;
            }
            var data = new DLCPackageData
            {
                Name = DlcNameTB.Text.Replace(" ", "_"),
                AppId = AppIdTB.Text,
                SongInfo = new SongInfo
                {
                    SongDisplayName = SongDisplayNameTB.Text,
                    Album = AlbumTB.Text,
                    SongYear = year,
                    Artist = ArtistTB.Text,
                    AverageTempo = tempo                    
                },
                AlbumArtPath = AlbumArtPath,
                OggPath = OggPath,
                Arrangements = arrangements,
                Tone = toneControl.Tone
            };

            return data;
        }

        public class SongAppId
        {
            public string Name;
            public string AppId;
            public override string ToString()
            {
                return string.Format("{0} - {1}", Name, AppId);
            }
        }

        private void cmbAppIds_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbAppIds.SelectedItem != null)
            {
                AppIdTB.Text = ((SongAppId)cmbAppIds.SelectedItem).AppId;
            }
        }
    }
}