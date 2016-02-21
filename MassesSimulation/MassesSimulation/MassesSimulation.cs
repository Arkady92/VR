using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace MassesSimulation
{
    public partial class MassesSimulation : Form
    {
        private Random rnd = new Random();
        private Bitmap backBuffer;
        private float massRadius = 20.0f;
        private double time;
        private double x1;
        private double y1;
        private double x2;
        private double y2;
        private double a0;
        private double av0;
        private double delta;
        private double m1;
        private double m2;
        private double l;
        private double angle = 0;
        private double angleVelocity = 0;
        private double angleAcceleration = 0;
        private double newAngle;
        private double newAngleVelocity;
        private float visualizationFactor = 150;

        public MassesSimulation()
        {
            InitializeComponent();
            InitializeValues();
            backBuffer = new Bitmap(VisualizationPanel.Width, VisualizationPanel.Height);
            SetDoubleBuffered(VisualizationPanel);
            UpdateVisualization();
            StartButton.Focus();
        }

        private void InitializeValues()
        {
            a0 = Math.PI / 4;
            av0 = Math.PI;
            delta = 0.02;
            m1 = 1;
            m2 = 2;

            l = Math.Sqrt(2);
            y1 = 0.0;
            x2 = 0.0;
            x1 = -Math.Cos(Math.PI - a0) * l;
            y2 = Math.Sin(Math.PI - a0) * l;

            angle = a0;
            angleVelocity = av0;
            angleAcceleration = 0;

            UpdateTextBoxes();
        }

        private void ReinitializeValues()
        {
            y1 = 0.0;
            x2 = 0.0;
            x1 = -Math.Cos(Math.PI - a0) * l;
            y2 = Math.Sin(Math.PI - a0) * l;

            angle = a0;
            angleVelocity = av0;
            angleAcceleration = 0;

            UpdateTextBoxes();
        }

        private void UpdateTextBoxes()
        {
            X0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", a0);
            V0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", av0);
            DeltaTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", delta);
            MTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m1);
            KTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m2);
            CTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", l);

            positionLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", angle);
            velocityLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", angleVelocity);
            accelerationLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", angleAcceleration);

            X1Label.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", x1);
            Y1Label.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", y1);
            X2Label.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", x2);
            Y2Label.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", y2);
        }

        private void UpdateVisualization()
        {
            var graphics = Graphics.FromImage(backBuffer);
            graphics.Clear(Color.White);
            graphics.TranslateTransform((float)(VisualizationPanel.Size.Width * 0.5), (float)(VisualizationPanel.Size.Height * 0.5));
            graphics.DrawLine(Pens.Black, 0.0f, -500f, 0.0f, 500f);
            graphics.DrawLine(Pens.Black, -500f, 0.0f, 500f, 0);
            graphics.DrawLine(new Pen(Color.Red, 3), (float)x1 * visualizationFactor, -((float)y1 * visualizationFactor),
                (float)x2 * visualizationFactor, -((float)y2 * visualizationFactor));
            graphics.FillEllipse(new SolidBrush(Color.Orange), (float)x1 * visualizationFactor - massRadius,
                -((float)y1 * visualizationFactor) - massRadius, massRadius * 2, massRadius * 2);
            graphics.DrawEllipse(new Pen(Color.Black, 2), (float)x1 * visualizationFactor - massRadius,
                -((float)y1 * visualizationFactor) - massRadius, massRadius * 2, massRadius * 2);
            graphics.FillEllipse(new SolidBrush(Color.Orange), (float)x2 * visualizationFactor - massRadius,
                -((float)y2 * visualizationFactor) - massRadius, massRadius * 2, massRadius * 2);
            graphics.DrawEllipse(new Pen(Color.Black, 2), (float)x2 * visualizationFactor - massRadius,
                -((float)y2 * visualizationFactor) - massRadius, massRadius * 2, massRadius * 2);
            VisualizationPanel.Image = backBuffer;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);
            angleAcceleration = (sin * cos * (m2 - m1) * angleVelocity * angleVelocity) / (m1 * sin * sin + m2 * cos * cos);
            newAngleVelocity = angleAcceleration * delta + angleVelocity;
            newAngle = newAngleVelocity * delta + angle;

            x1 = -Math.Cos(Math.PI - angle) * l;
            y2 = Math.Sin(Math.PI - angle) * l;

            time += delta;
            UpdateVisualization();
            UpdateTextBoxes();

            angle = newAngle;
            angleVelocity = newAngleVelocity;
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
            angle = a0;
            angleVelocity = av0;
            angleAcceleration = 0;
            ReinitializeValues();
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
                a0 = result;
        }

        private void V0TextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                av0 = result;
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
                m1 = result;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                m2 = result;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            double result;
            if (textBox == null) return;
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                l = result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            a0 += 0.1;
            X0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", a0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            a0 -= 0.1;
            X0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", a0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            av0 += 0.1;
            V0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", av0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            av0 -= 0.1;
            V0TextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", av0);
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
            m1 += 0.5;
            MTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            m1 -= 0.5;
            MTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m1);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            m2 += 0.5;
            KTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m2);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            m2 -= 0.5;
            KTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", m2);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            l += 0.1;
            CTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", l);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            l -= 0.1;
            CTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0:F2}", l);
        }
    }
}
