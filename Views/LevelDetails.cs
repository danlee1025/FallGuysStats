﻿using System.Collections.Generic;
using System.Windows.Forms;
namespace FallGuysStats {
    public partial class LevelDetails : Form {
        public string LevelName { get; set; }
        public List<RoundInfo> RoundDetails { get; set; }
        private bool ShowStats = false;
        public LevelDetails() {
            InitializeComponent();
        }

        private void LevelDetails_Load(object sender, System.EventArgs e) {
            if (LevelName == "Shows") {
                Text = $"Show Stats";
                ShowStats = true;
                ClientSize = new System.Drawing.Size(Width + 85, Height);
            } else {
                Text = $"Level Stats - {LevelName}";
            }

            gridDetails.DataSource = RoundDetails;
        }
        private void gridDetails_DataSourceChanged(object sender, System.EventArgs e) {
            if (gridDetails.Columns.Count == 0) { return; }

            int pos = 0;
            gridDetails.Columns["Tier"].Visible = false;
            gridDetails.Columns["Qualified"].Visible = false;
            gridDetails.Columns.Add(new DataGridViewImageColumn() { Name = "Medal", ImageLayout = DataGridViewImageCellLayout.Zoom, ToolTipText = "Medal" });
            gridDetails.Setup("Medal", pos++, 24, "", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("ShowID", pos++, 0, "Show", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Round", pos++, 50, "Round", DataGridViewContentAlignment.MiddleRight);
            if (ShowStats) {
                gridDetails.Setup("Name", pos++, 95, "Level", DataGridViewContentAlignment.MiddleLeft);
            } else {
                gridDetails.Columns["Name"].Visible = false;
            }
            gridDetails.Setup("Players", pos++, 60, "Players", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Start", pos++, 115, "Start", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("End", pos++, 60, "Duration", DataGridViewContentAlignment.MiddleCenter);
            gridDetails.Setup("Position", pos++, 60, "Position", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Score", pos++, 60, "Score", DataGridViewContentAlignment.MiddleRight);
            gridDetails.Setup("Kudos", pos++, 60, "Kudos", DataGridViewContentAlignment.MiddleRight);
        }
        private void gridDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex < 0) { return; }

            if (gridDetails.Columns[e.ColumnIndex].Name == "End") {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                e.Value = (info.End - info.Start).ToString("m\\:ss");
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Medal" && e.Value == null) {
                RoundInfo info = gridDetails.Rows[e.RowIndex].DataBoundItem as RoundInfo;
                if (info.Qualified) {
                    switch (info.Tier) {
                        case 0: e.Value = Properties.Resources.medal_pink; break;
                        case 1: e.Value = Properties.Resources.medal_gold; break;
                        case 2: e.Value = Properties.Resources.medal_silver; break;
                        case 3: e.Value = Properties.Resources.medal_bronze; break;
                    }
                } else {
                    e.Value = Properties.Resources.medal_eliminated;
                }
            } else if (gridDetails.Columns[e.ColumnIndex].Name == "Name") {
                string name = null;
                if (LevelStats.DisplayNameLookup.TryGetValue((string)e.Value, out name)) {
                    e.Value = name;
                }
            }
        }
        private void gridDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            string columnName = gridDetails.Columns[e.ColumnIndex].Name;
            SortOrder sortOrder = gridDetails.GetSortOrder(columnName);

            RoundDetails.Sort(delegate (RoundInfo one, RoundInfo two) {
                if (sortOrder == SortOrder.Descending) {
                    RoundInfo temp = one;
                    one = two;
                    two = temp;
                }
                switch (columnName) {
                    case "ShowID": return one.ShowID.CompareTo(two.ShowID);
                    case "Round": return one.Round.CompareTo(two.Round);
                    case "Players": return one.Players.CompareTo(two.Players);
                    case "Start": return one.Start.CompareTo(two.Start);
                    case "End": return (one.End - one.Start).CompareTo(two.End - two.Start);
                    case "Qualified": return one.Qualified.CompareTo(two.Qualified);
                    case "Position": return one.Position.CompareTo(two.Position);
                    case "Score": return one.Score.GetValueOrDefault(0).CompareTo(two.Score.GetValueOrDefault(0));
                    case "Medal":
                        int tierOne = one.Qualified ? one.Tier == 0 ? 4 : one.Tier : 5;
                        int tierTwo = two.Qualified ? two.Tier == 0 ? 4 : two.Tier : 5;
                        return tierOne.CompareTo(tierTwo);
                    default: return one.Kudos.CompareTo(two.Kudos);
                }
            });

            gridDetails.DataSource = null;
            gridDetails.DataSource = RoundDetails;
            gridDetails.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;
        }
    }
}