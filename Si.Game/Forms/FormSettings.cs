using Si.GameEngine.Engine;
using System;
using System.Windows.Forms;

namespace Si.Game
{
    public partial class FormSettings : Form
    {
        const int MAX_RESOLUTIONS = 16;

        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            trackBarResolution.Scroll += trackBarResolution_Scroll;

            var settings = EngineCore.LoadSettings();

            checkBoxAutoZoomWhenMoving.Checked = settings.AutoZoomWhenMoving;
            checkBoxHighlightAllSprites.Checked = settings.HighlightAllSprites;
            checkBoxHighlightNatrualBounds.Checked = settings.HighlightNatrualBounds;
            checkBoxEnableSpriteInterrogation.Checked = settings.EnableSpriteInterrogation;
            textBoxFrameLimiter.Text = $"{settings.FrameLimiter:n0}";
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

                if (trackBarResolution.Minimum == 1 && baseX > 640 && baseY > 640)
                {
                    trackBarResolution.Minimum = i;
                }

                if (baseX >= settings.Resolution.Width && baseY >= settings.Resolution.Height)
                {
                    trackBarResolution.Value = i;
                    trackBarResolution_Scroll(this, new EventArgs());
                    break;
                }
            }
        }

        private void trackBarResolution_Scroll(object sender, EventArgs e)
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
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
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = EngineCore.LoadSettings();

                settings.AutoZoomWhenMoving = checkBoxAutoZoomWhenMoving.Checked;
                settings.HighlightAllSprites = checkBoxHighlightAllSprites.Checked;
                settings.HighlightNatrualBounds = checkBoxHighlightNatrualBounds.Checked;
                settings.EnableSpriteInterrogation = checkBoxEnableSpriteInterrogation.Checked;

                settings.FrameLimiter = GetAndValidate(textBoxFrameLimiter, 30, 1000, "Frame Limiter");
                settings.OverdrawScale = GetAndValidate(textBoxOverdrawScale, 1.0, 10.0, "Overdraw scale");
                settings.InitialFrameStarCount = GetAndValidate(textBoxInitialFrameStarCount, 0, 1000, "Initial frame star count");
                settings.DeltaFrameTargetStarCount = GetAndValidate(textBoxDeltaFrameTargetStarCount, 0, 1000, "Delta-frame target star count");

                int baseX = Screen.PrimaryScreen.Bounds.Width - (int)((double)Screen.PrimaryScreen.Bounds.Width * (1.0 - ((double)trackBarResolution.Value / (double)MAX_RESOLUTIONS)));
                int baseY = Screen.PrimaryScreen.Bounds.Height - (int)((double)Screen.PrimaryScreen.Bounds.Height * (1.0 - ((double)trackBarResolution.Value / (double)MAX_RESOLUTIONS)));
                settings.Resolution = new System.Drawing.Size(baseX, baseY);

                settings.FullScreen = (trackBarResolution.Value == MAX_RESOLUTIONS);

                EngineCore.SaveSettings(settings);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Strikeforce Infinity", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
