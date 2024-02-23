using Si.GameEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Si.Game
{
    public partial class FormSettings : Form
    {
        private const int MAX_RESOLUTIONS = 32;

        class GraphicsAdapter
        {
            public int DeviceId { get; set; }
            public string Description { get; set; }

            public double VideoMemoryMb { get; set; }

            public GraphicsAdapter(int deviceId, string description)
            {
                DeviceId = deviceId;
                Description = description;
            }

            public override string ToString()
            {
                return Description;
            }
        }

        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            trackBarResolution.Scroll += TrackBarResolution_Scroll;

            var settings = GameEngineCore.LoadSettings();

            checkBoxPlayMusic.Checked = settings.PlayMusic;
            checkBoxEnableAntiAliasing.Checked = settings.EnableSpeedScaleFactoring;
            checkBoxEnableVerticalSync.Checked = settings.VerticalSync;
            checkBoxAutoZoomWhenMoving.Checked = settings.EnableSpeedScaleFactoring;
            checkBoxHighlightAllSprites.Checked = settings.HighlightAllSprites;
            checkBoxHighlightNatrualBounds.Checked = settings.HighlightNatrualBounds;
            checkBoxEnableSpriteInterrogation.Checked = settings.EnableSpriteInterrogation;
            checkBoxPreCacheAllAssets.Checked = settings.PreCacheAllAssets;
            textBoxFramePerSecondLimit.Text = $"{settings.FramePerSecondLimit:n0}";
            textBoxOverdrawScale.Text = $"{settings.OverdrawScale:n0}";
            textBoxInitialFrameStarCount.Text = $"{settings.InitialFrameStarCount:n0}";
            textBoxDeltaFrameTargetStarCount.Text = $"{settings.DeltaFrameTargetStarCount:n0}";

            trackBarResolution.Minimum = 1;
            trackBarResolution.Maximum = MAX_RESOLUTIONS;

            for (int i = 0; i < MAX_RESOLUTIONS + 1; i++)
            {
                int baseX = Screen.PrimaryScreen.Bounds.Width - (int)((double)Screen.PrimaryScreen.Bounds.Width * (1.0 - ((double)i / (double)MAX_RESOLUTIONS)));
                int baseY = Screen.PrimaryScreen.Bounds.Height - (int)((double)Screen.PrimaryScreen.Bounds.Height * (1.0 - ((double)i / (double)MAX_RESOLUTIONS)));

                if ((baseX % 2) != 0) baseX++;
                if ((baseY % 2) != 0) baseY++;

                if (trackBarResolution.Minimum == 1 && baseX >= 540 && baseY >= 540)
                {
                    trackBarResolution.Minimum = i;
                }

                if (baseX >= settings.Resolution.Width && baseY >= settings.Resolution.Height)
                {
                    trackBarResolution.Value = i;
                    TrackBarResolution_Scroll(this, new EventArgs());
                    break;
                }
            }

            var adapters = GetGraphicsAdaptersInfo();
            foreach (var item in adapters)
            {
                comboBoxGraphicsAdapter.Items.Add(item);
                if (settings.GraphicsAdapterId == item.DeviceId)
                {
                    comboBoxGraphicsAdapter.SelectedItem = item;
                }
            }

            if (comboBoxGraphicsAdapter.SelectedItem == null)
            {
                comboBoxGraphicsAdapter.SelectedItem = adapters.OrderByDescending(o => o.VideoMemoryMb).First();
            }
        }

        private void TrackBarResolution_Scroll(object sender, EventArgs e)
        {
            int baseX = Screen.PrimaryScreen.Bounds.Width;
            int baseY = Screen.PrimaryScreen.Bounds.Height;

            if (trackBarResolution.Value < MAX_RESOLUTIONS)
            {
                baseX = Screen.PrimaryScreen.Bounds.Width - (int)((double)Screen.PrimaryScreen.Bounds.Width * (1.0 - ((double)trackBarResolution.Value / (double)MAX_RESOLUTIONS)));
                baseY = Screen.PrimaryScreen.Bounds.Height - (int)((double)Screen.PrimaryScreen.Bounds.Height * (1.0 - ((double)trackBarResolution.Value / (double)MAX_RESOLUTIONS)));

                if ((baseX % 2) != 0) baseX++;
                if ((baseY % 2) != 0) baseY++;

                labelResolution.Text = $"{baseX} x {baseY}";
            }
            else
            {
                labelResolution.Text = $"{baseX} x {baseY} [Full Screen]";
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private List<GraphicsAdapter> GetGraphicsAdaptersInfo()
        {
            var result = new List<GraphicsAdapter>();
            using (var factory = new SharpDX.DXGI.Factory1())
            {
                foreach (var adapter in factory.Adapters)
                {
                    result.Add(new GraphicsAdapter(adapter.Description.DeviceId, adapter.Description.Description)
                    {
                        VideoMemoryMb = adapter.Description.DedicatedVideoMemory / 1024.0 / 1024.0
                    });
                }
            }

            return result;
        }

        private double GetAndValidate(TextBox textbox, double min, double max, string fieldNameForError)
        {
            if (double.TryParse(textbox.Text, out var value) == false || value < min || value > max)
            {
                throw new Exception($"Can invalid value has been specified for: {fieldNameForError}. Enter a whole or decimal numeric value between {min} and {max}.");
            }
            return value;
        }

        private int GetAndValidate(TextBox textbox, int min, int max, string fieldNameForError)
        {
            if (int.TryParse(textbox.Text, out var value) == false || value < min || value > max)
            {
                throw new Exception($"Can invalid value has been specified for: {fieldNameForError}. Enter a while numeric value between {min} and {max}.");
            }
            return value;
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = GameEngineCore.LoadSettings();

                settings.PlayMusic = checkBoxPlayMusic.Checked;
                settings.EnableSpeedScaleFactoring = checkBoxEnableAntiAliasing.Checked;
                settings.VerticalSync = checkBoxEnableVerticalSync.Checked;
                settings.EnableSpeedScaleFactoring = checkBoxAutoZoomWhenMoving.Checked;
                settings.HighlightAllSprites = checkBoxHighlightAllSprites.Checked;
                settings.HighlightNatrualBounds = checkBoxHighlightNatrualBounds.Checked;
                settings.EnableSpriteInterrogation = checkBoxEnableSpriteInterrogation.Checked;
                settings.PreCacheAllAssets = checkBoxPreCacheAllAssets.Checked;

                settings.FramePerSecondLimit = GetAndValidate(textBoxFramePerSecondLimit, 30, 1000, "Frame Limiter");
                settings.OverdrawScale = GetAndValidate(textBoxOverdrawScale, 1.0, 10.0, "Overdraw scale");
                settings.InitialFrameStarCount = GetAndValidate(textBoxInitialFrameStarCount, 0, 1000, "Initial frame star count");
                settings.DeltaFrameTargetStarCount = GetAndValidate(textBoxDeltaFrameTargetStarCount, 0, 1000, "Delta-frame target star count");

                int baseX = Screen.PrimaryScreen.Bounds.Width - (int)((double)Screen.PrimaryScreen.Bounds.Width * (1.0 - ((double)trackBarResolution.Value / (double)MAX_RESOLUTIONS)));
                int baseY = Screen.PrimaryScreen.Bounds.Height - (int)((double)Screen.PrimaryScreen.Bounds.Height * (1.0 - ((double)trackBarResolution.Value / (double)MAX_RESOLUTIONS)));
                settings.Resolution = new System.Drawing.Size(baseX, baseY);

                settings.FullScreen = (trackBarResolution.Value == MAX_RESOLUTIONS);

                var graphicsAdapter = comboBoxGraphicsAdapter.SelectedItem as GraphicsAdapter;
                if (graphicsAdapter == null)
                {
                    throw new Exception("You must select a graphics adapter.");
                }

                settings.GraphicsAdapterId = graphicsAdapter.DeviceId;

                GameEngineCore.SaveSettings(settings);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Strikeforce Infinity", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
