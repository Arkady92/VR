using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.WindowsForms;
using OxyPlot;
using OxyPlot.Series;
using System.Globalization;
using System.IO;

namespace SpringSimulation
{
    public partial class SpringSimulation : Form
    {
        private PlotView forcesPlot;
        private PlotView kinematicsPlot;
        private PlotView trajectoryPlot;
        private PlotView displacementPlot;
        private OxyPlot.Series.LineSeries fSeries;
        private OxyPlot.Series.LineSeries gSeries;
        private OxyPlot.Series.LineSeries hSeries;
        private OxyPlot.Series.LineSeries wSeries;
        private OxyPlot.Series.LineSeries xSeries;
        private OxyPlot.Series.LineSeries wxSeries;
        private OxyPlot.Series.LineSeries positionSeries;
        private OxyPlot.Series.LineSeries velocitySeries;
        private OxyPlot.Series.LineSeries accelerationSeries;
        private OxyPlot.Series.LineSeries trajectorySeries;
        private OxyPlot.Series.LineSeries errorSeries;
        private Random rnd = new Random();
        private Bitmap backBuffer;
        private float springRadius = 30.0f;
        private double time;
        private double x0 = -7.0;
        private double v0 = 0;
        private double delta = 0.05;
        private double m = 1;
        private double k = 0.1;
        private double c = 5;
        private double amplitude = 0;
        private double omega = 1;
        private double fi = 0;
        private int htSelect = 0;
        private int wtSelect = 0;
        private double f;
        private double g;
        private double h;
        private double w;
        private double position = 0;
        private double velocity = 0;
        private double acceleration = 0;
        private Func<double, double>[] functions;
        private bool firstIteration = true;
        private bool secondIteration = true;
        private double prePositon;
        private double correctPosition;
        private float visualizationFactor = 25;

        public SpringSimulation()
        {
            InitializeComponent();
            InitializePlots();
            InitializeFunctions();
            backBuffer = new Bitmap(VisualizationPanel.Width, VisualizationPanel.Height);
            SetDoubleBuffered(VisualizationPanel);
            UpdateVisualization();
            StartButton.Focus();
        }

        private void InitializeFunctions()
        {
            functions = new Func<double, double>[5];
            functions[0] = (t) => { return 0; };
            functions[1] = (t) => { return amplitude; };
            functions[2] = (t) => { return (t >= 0) ? amplitude : 0; };
            functions[3] = (t) => { return Math.Sign(amplitude * Math.Sin(omega * t + fi)); };
            functions[4] = (t) => { return amplitude * Math.Sin(omega * t + fi); };
        }

        private void InitializePlots()
        {
            InitializeForcesPlot();
            InitializeKinematicsPlot();
            InitializeTrajectoryPlot();
            InitializeDisplacementPlot();
        }

        private void UpdateVisualization()
        {
            var graphics = Graphics.FromImage(backBuffer);
            graphics.Clear(Color.White);
            graphics.TranslateTransform((float)(VisualizationPanel.Size.Width * 0.5), (float)(VisualizationPanel.Size.Height * 0.5));
            graphics.DrawLine(Pens.Black, 0.0f + (float)w * visualizationFactor, -50f, 0.0f + (float)w * visualizationFactor, 50f);
            graphics.DrawLine(Pens.Red, -500.0f, -0.0f, (float)position * visualizationFactor, 0.0f);
            graphics.FillEllipse(new SolidBrush(Color.Orange), (float)position * visualizationFactor - springRadius, -springRadius, springRadius * 2, springRadius * 2);
            graphics.DrawEllipse(new Pen(Color.Black, 2), (float)position * visualizationFactor - springRadius, -springRadius, springRadius * 2, springRadius * 2);
            VisualizationPanel.Image = backBuffer;
        }

        private void InitializeDisplacementPlot()
        {
            this.displacementPlot = new OxyPlot.WindowsForms.PlotView();
            this.SuspendLayout();
            this.displacementPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displacementPlot.Location = new System.Drawing.Point(0, 0);
            this.displacementPlot.Name = "displacementPlot";
            this.displacementPlot.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.displacementPlot.Size = new System.Drawing.Size(200, 200);
            this.displacementPlot.TabIndex = 0;
            this.displacementPlot.Text = "displacement";
            this.displacementPlot.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.displacementPlot.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.displacementPlot.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;

            DisplacementPlotPanel.Controls.Add(this.displacementPlot);
            this.ResumeLayout(false);

            wSeries = new OxyPlot.Series.LineSeries();
            wSeries.Title = "w(t)";
            xSeries = new OxyPlot.Series.LineSeries();
            xSeries.Title = "x(t)";
            wxSeries = new OxyPlot.Series.LineSeries();
            wxSeries.Title = "w(t) - x(t)";
            var myModel = new PlotModel();
            myModel.Series.Add(wSeries);
            myModel.Series.Add(xSeries);
            myModel.Series.Add(wxSeries);
            this.displacementPlot.Model = myModel;
        }

        private void InitializeTrajectoryPlot()
        {
            this.trajectoryPlot = new OxyPlot.WindowsForms.PlotView();
            this.SuspendLayout();
            this.trajectoryPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trajectoryPlot.Location = new System.Drawing.Point(0, 0);
            this.trajectoryPlot.Name = "trajectoryPlot";
            this.trajectoryPlot.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.trajectoryPlot.Size = new System.Drawing.Size(200, 200);
            this.trajectoryPlot.TabIndex = 0;
            this.trajectoryPlot.Text = "trajectory";
            this.trajectoryPlot.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.trajectoryPlot.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.trajectoryPlot.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;

            TrajectoryPlotPanel.Controls.Add(this.trajectoryPlot);
            this.ResumeLayout(false);

            trajectorySeries = new OxyPlot.Series.LineSeries();
            var myModel = new PlotModel { Title = "Trajectory" };
            myModel.Series.Add(trajectorySeries);
            this.trajectoryPlot.Model = myModel;

            using (StreamWriter outputFile = new StreamWriter("Pages.txt"))
            {
                int start = 21;
                var line = "";
                while (start <= 120)
                {
                    line = line + start + ",";
                    start++;
                    line = line + start + ",";
                    start+=3;
                }
                line.Remove(line.Length - 1);
                outputFile.WriteLine(line);

                start = 23;
                line = "";
                while (start <= 120)
                {
                    line = line + start + ",";
                    start++;
                    line = line + start + ",";
                    start+=3;
                }
                line.Remove(line.Length - 1);
                outputFile.WriteLine(line);
            }
        }

        private void InitializeKinematicsPlot()
        {
            this.kinematicsPlot = new OxyPlot.WindowsForms.PlotView();
            this.SuspendLayout();
            this.kinematicsPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kinematicsPlot.Location = new System.Drawing.Point(0, 0);
            this.kinematicsPlot.Name = "KinematicsPlot";
            this.kinematicsPlot.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.kinematicsPlot.Size = new System.Drawing.Size(200, 200);
            this.kinematicsPlot.TabIndex = 0;
            this.kinematicsPlot.Text = "Kinematics";
            this.kinematicsPlot.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.kinematicsPlot.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.kinematicsPlot.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;

            KinematicsPlotPanel.Controls.Add(this.kinematicsPlot);
            this.ResumeLayout(false);

            positionSeries = new OxyPlot.Series.LineSeries();
            positionSeries.Title = "x(t)";
            velocitySeries = new OxyPlot.Series.LineSeries();
            velocitySeries.Title = "v(t)";
            accelerationSeries = new OxyPlot.Series.LineSeries();
            accelerationSeries.Title = "a(t)";
            errorSeries = new OxyPlot.Series.LineSeries();
            errorSeries.Title = "err(t)";
            var myModel = new PlotModel { Title = "Kinematics" };
            myModel.Series.Add(positionSeries);
            myModel.Series.Add(velocitySeries);
            myModel.Series.Add(accelerationSeries);
            myModel.Series.Add(errorSeries);
            this.kinematicsPlot.Model = myModel;
        }

        private void InitializeForcesPlot()
        {
            this.forcesPlot = new OxyPlot.WindowsForms.PlotView();
            this.SuspendLayout();
            this.forcesPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.forcesPlot.Location = new System.Drawing.Point(0, 0);
            this.forcesPlot.Name = "ForcesPlot";
            this.forcesPlot.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.forcesPlot.Size = new System.Drawing.Size(200, 200);
            this.forcesPlot.TabIndex = 0;
            this.forcesPlot.Text = "Forces";
            this.forcesPlot.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.forcesPlot.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.forcesPlot.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;

            ForcesPlotPanel.Controls.Add(this.forcesPlot);
            this.ResumeLayout(false);

            fSeries = new OxyPlot.Series.LineSeries();
            fSeries.Title = "f(t)";
            gSeries = new OxyPlot.Series.LineSeries();
            gSeries.Title = "g(t)";
            hSeries = new OxyPlot.Series.LineSeries();
            hSeries.Title = "h(t)";
            var myModel = new PlotModel { Title = "Forces" };
            myModel.Series.Add(fSeries);
            myModel.Series.Add(gSeries);
            myModel.Series.Add(hSeries);
            this.forcesPlot.Model = myModel;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            w = functions[wtSelect](time);
            h = functions[htSelect](time);
            if (firstIteration)
            {
                position = x0;
                velocity = v0;
                acceleration = (c * (w - position) - k * velocity + h) / m;
                firstIteration = false;
            }
            else if (secondIteration)
            {
                prePositon = position;
                //position = x0 + delta * velocity + acceleration * delta * delta / 2;
                double delta_2 = delta * delta;
                //position = delta_2 * (c * (w - position) - k * v0 + h) / m + 2 * position - prePositon;
                position = delta_2 / (2 * m) * (c * (w - position) - k * v0 + h) + delta * velocity - position;
                velocity = (position - prePositon) / delta;
                acceleration = (c * (w - position) - k * velocity + h) / m;
                secondIteration = false;
            }
            else
            {
                var prePrePosition = prePositon;
                prePositon = position;
                var x_prev = prePrePosition;
                var x_curr = prePositon;
                double delta_2 = delta * delta;
                //position = (2 * delta * delta * c * (w - x_curr) + 2 * delta * k * x_prev + h * 2 * delta * delta +
                //    4 * m * x_curr - 2 * m * x_prev) / (2 * m + delta * k);

                position = (4 * delta_2) / (double)(-2 * delta * k - 4 * m) *
                         ((-2 * m * x_curr + m * x_prev) / (double)delta_2 - h - c * (w - x_curr) + (-k * x_prev) / (double)(2 * delta));
                velocity = (position - prePrePosition) / (2 * delta);
                acceleration = (position - 2 * prePositon + prePrePosition) / (delta * delta);
            }
            f = c * (w - position);
            g = -k * velocity;
            correctPosition = x0 * Math.Exp(-k / (2 * m) * time) * Math.Cos(Math.Sqrt(c / m) * time + fi);

            fSeries.Points.Add(new DataPoint(time, f));
            gSeries.Points.Add(new DataPoint(time, g));
            hSeries.Points.Add(new DataPoint(time, h));
            wSeries.Points.Add(new DataPoint(time, w));
            xSeries.Points.Add(new DataPoint(time, position));
            wxSeries.Points.Add(new DataPoint(time, w - position));

            positionSeries.Points.Add(new DataPoint(time, position));
            velocitySeries.Points.Add(new DataPoint(time, velocity));
            accelerationSeries.Points.Add(new DataPoint(time, acceleration));
            trajectorySeries.Points.Add(new DataPoint(position, velocity));
            errorSeries.Points.Add(new DataPoint(time, position - correctPosition));
            forcesPlot.InvalidatePlot(true);
            kinematicsPlot.InvalidatePlot(true);
            trajectoryPlot.InvalidatePlot(true);
            displacementPlot.InvalidatePlot(true);

            wLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", w);
            fLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", f);
            gLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", g);
            hLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", h);
            positionLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", position);
            velocityLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", velocity);
            accelerationLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", acceleration);

            time += delta;
            UpdateVisualization();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Timer.Interval = (int)(delta * 1000);
            Timer.Enabled = true;
            StartButton.Enabled = false;
            StopButton.Enabled = true;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Timer.Enabled = false;
            StartButton.Enabled = true;
            StopButton.Enabled = false;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            time = 0;
            firstIteration = true;
            secondIteration = true;
            position = 0;
            velocity = 0;
            w = 0;
            h = 0;
            positionLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", 0.00);
            velocityLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", 0.00);
            accelerationLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", 0.00);
            fLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", 0.00);
            gLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", 0.00);
            hLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", 0.00);
            wLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", 0.00);
            fSeries.Points.Clear();
            gSeries.Points.Clear();
            hSeries.Points.Clear();
            wSeries.Points.Clear();
            xSeries.Points.Clear();
            wxSeries.Points.Clear();
            positionSeries.Points.Clear();
            velocitySeries.Points.Clear();
            accelerationSeries.Points.Clear();
            trajectorySeries.Points.Clear();
            errorSeries.Points.Clear();
            forcesPlot.InvalidatePlot(true);
            kinematicsPlot.InvalidatePlot(true);
            trajectoryPlot.InvalidatePlot(true);
            displacementPlot.InvalidatePlot(true);
            Timer.Enabled = false;
            StartButton.Enabled = true;
            StopButton.Enabled = false;
            UpdateVisualization();
        }

        public static void SetDoubleBuffered(Control c)
        {
            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        private void X0TextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                x0 = result;
        }

        private void V0TextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                v0 = result;
        }

        private void DeltaTextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                delta = result;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                m = result;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                k = result;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                c = result;
        }

        private void HTListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listBox = sender as ListBox;
            htSelect = listBox.SelectedIndex;
        }
        private void WTListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listBox = sender as ListBox;
            wtSelect = listBox.SelectedIndex;
        }

        private void ATextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                amplitude = result;
        }

        private void OmegaTextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                omega = result;
        }

        private void FiTextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                fi = result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            x0 += 0.5;
            X0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", x0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            x0 -= 0.5;
            X0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", x0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            v0 += 0.5;
            V0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", v0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            v0 -= 0.5;
            V0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", v0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            delta += 0.01;
            DeltaTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", delta);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            delta -= 0.01;
            DeltaTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", delta);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            m += 0.5;
            MTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            m -= 0.5;
            MTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            k += 0.1;
            KTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", k);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            k -= 0.1;
            KTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", k);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            c += 0.5;
            CTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", c);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            c -= 0.5;
            CTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", c);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            amplitude += 0.1;
            ATextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", amplitude);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            amplitude -= 0.1;
            ATextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", amplitude);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            omega += 0.1;
            OmegaTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", omega);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            omega -= 0.1;
            OmegaTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", omega);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            fi += 0.1;
            FiTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", fi);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            fi -= 0.1;
            FiTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", fi);
        }

        private void ShowErrorsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            kinematicsPlot.Model.Series[3].IsVisible = (sender as CheckBox).Checked;
            kinematicsPlot.InvalidatePlot(true);
        }

        private void PositionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            kinematicsPlot.Model.Series[0].IsVisible = (sender as CheckBox).Checked;
            kinematicsPlot.InvalidatePlot(true);
        }

        private void VelocityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            kinematicsPlot.Model.Series[1].IsVisible = (sender as CheckBox).Checked;
            kinematicsPlot.InvalidatePlot(true);
        }

        private void AccelerationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            kinematicsPlot.Model.Series[2].IsVisible = (sender as CheckBox).Checked;
            kinematicsPlot.InvalidatePlot(true);
        }

        private void FtCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            forcesPlot.Model.Series[0].IsVisible = (sender as CheckBox).Checked;
            forcesPlot.InvalidatePlot(true);
        }

        private void GtCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            forcesPlot.Model.Series[1].IsVisible = (sender as CheckBox).Checked;
            forcesPlot.InvalidatePlot(true);
        }

        private void HtCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            forcesPlot.Model.Series[2].IsVisible = (sender as CheckBox).Checked;
            forcesPlot.InvalidatePlot(true);
        }
    }
}
