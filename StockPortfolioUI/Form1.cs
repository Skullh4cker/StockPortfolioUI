using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockPortfolioUI
{
    public partial class StockPortfolio : Form
    {
        private Population population;

        public StockPortfolio()
        {
            InitializeComponent();
            population = new Population();
            textBox1.Text = GlobalParametrs.E1.ToString();
            textBox2.Text = GlobalParametrs.E2.ToString();
            textBox3.Text = GlobalParametrs.E3.ToString();
            generationBar.Value = GlobalParametrs.POPULATION_SIZE;
            parentpoolBar.Value = GlobalParametrs.PARENTPOOL_SIZE;
            genSizeLabel.Text = generationBar.Value.ToString();
            poolSizeLabel.Text = parentpoolBar.Value.ToString();
            comboBox1.SelectedIndex = 0;
            button2.Enabled = false;
            button2.Visible = false;
            PrepareChart();
            UpdateChart();
        }                   
        private void button1_Click(object sender, EventArgs e)
        {
            string strE1 = textBox1.Text.Replace('.', ',');
            string strE2 = textBox2.Text.Replace('.', ',');
            string strE3 = textBox3.Text.Replace('.', ',');
            if (double.TryParse(strE1, out double e1) && double.TryParse(strE2, out double e2) && double.TryParse(strE3, out double e3) && generationBar.Value > parentpoolBar.Value)
            {
                GlobalParametrs.E1 = e1;
                GlobalParametrs.E2 = e2;
                GlobalParametrs.E3 = e3;
                PrepareChart();
                population.InitializePopulation(comboBox1.SelectedIndex);
                functionChart.Series["Average Fitness"].Points.Clear();
                functionChart.Series["Best Fitness"].Points.Clear();
                timer1.Start();
            }
            else
                MessageBox.Show("Пожалуйста, введите корректные значения", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }                                          
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (population.SimulateOneIteration()) UnlockAll();
            else BlockAll();

            UpdateLabels();
            UpdateChart();
        }
        private void PrepareChart()
        {
            var objChart = functionChart.ChartAreas[0];
            objChart.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            objChart.AxisX.Minimum = 1;
            objChart.AxisX.Maximum = 30;

            objChart.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            objChart.AxisY.Minimum = 1.2;
            objChart.AxisY.Maximum = 2;

            functionChart.Series["Average Fitness"].Legend = "Legend1";
            functionChart.Series["Average Fitness"].ChartArea = "ChartArea1";
            functionChart.Series["Average Fitness"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            functionChart.Series["Best Fitness"].Legend = "Legend1";
            functionChart.Series["Best Fitness"].ChartArea = "ChartArea1";
            functionChart.Series["Best Fitness"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
        }
        private void UpdateLabels()
        {
            generationLabel.Text = population.generationNumber.ToString();
            avgLabel.Text = Math.Round(population.avgFitness, 3).ToString();
            bestLabel.Text = Math.Round(population.bestFitness, 3).ToString();
        }
        private void UpdateChart()
        {
            var objChart = functionChart.ChartAreas[0];
            var maxValue = Math.Max(population.avgFitness, population.bestFitness);
            if (population.generationNumber > objChart.AxisX.Maximum) objChart.AxisX.Maximum = population.generationNumber;
            if (population.bestFitness > objChart.AxisY.Maximum || population.avgFitness > objChart.AxisY.Maximum) objChart.AxisY.Maximum = maxValue + maxValue * 0.1;
            functionChart.Series["Average Fitness"].Points.AddXY(population.generationNumber, population.avgFitness);
            functionChart.Series["Best Fitness"].Points.AddXY(population.generationNumber, population.bestFitness);
        }
        private void BlockAll()
        {
            solutionLabel.Text = "Решение не найдено";
            solutionLabel.ForeColor = Color.Crimson;
            generationBar.Enabled = false;
            parentpoolBar.Enabled = false;
            button2.Enabled = false;
            button2.Visible = false;
        }
        private void UnlockAll()
        {
            solutionLabel.Text = "Решение найдено!";
            solutionLabel.ForeColor = Color.LawnGreen;
            timer1.Stop();
            generationBar.Enabled = true;
            parentpoolBar.Enabled = true;
            button2.Visible = true;
            button2.Enabled = true;
        }
        private void generationBar_Scroll(object sender, EventArgs e)
        {
            GlobalParametrs.POPULATION_SIZE = generationBar.Value;
            genSizeLabel.Text = generationBar.Value.ToString();
        }

        private void parentpoolBar_Scroll(object sender, EventArgs e)
        {
            GlobalParametrs.PARENTPOOL_SIZE = parentpoolBar.Value;
            poolSizeLabel.Text = parentpoolBar.Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DetailedPopulation detailedPopulation = new DetailedPopulation();
            detailedPopulation.ShowPopulation(population.Individuals);
            detailedPopulation.ShowDialog();
            detailedPopulation.Dispose();
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            int index = e.Index >= 0 ? e.Index : 0;
            var brush = Brushes.WhiteSmoke;
            e.DrawBackground();
            e.Graphics.DrawString(comboBox1.Items[index].ToString(), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }
    }
}
