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
            PrepareChart();
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

            //functionChart.Series.Clear();

            //functionChart.Series.Add("Average Fitness");
            //functionChart.Series["Average Fitness"].Color = Color.Green;
            functionChart.Series["Average Fitness"].Legend = "Legend1";
            functionChart.Series["Average Fitness"].ChartArea = "ChartArea1";
            functionChart.Series["Average Fitness"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            //functionChart.Series.Add("Best Fitness");
            //functionChart.Series["Best Fitness"].Color = Color.Blue;
            functionChart.Series["Best Fitness"].Legend = "Legend1";
            functionChart.Series["Best Fitness"].ChartArea = "ChartArea1";
            functionChart.Series["Best Fitness"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strE1 = textBox1.Text.Replace('.', ',');
            string strE2 = textBox2.Text.Replace('.', ',');
            string strE3 = textBox3.Text.Replace('.', ',');
            if (double.TryParse(strE1, out double e1) && double.TryParse(strE2, out double e2) && double.TryParse(strE3, out double e3))
            {
                GlobalParametrs.E1 = e1;
                GlobalParametrs.E2 = e2;
                GlobalParametrs.E3 = e3;
                PrepareChart();
                population.InitializePopulation();
                functionChart.Series["Average Fitness"].Points.Clear();
                functionChart.Series["Best Fitness"].Points.Clear();
                timer1.Start();
            }
            else
                MessageBox.Show("Пожалуйста, введите корректные значения", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
      
        }                        
                                 
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (population.SimulateOneIteration()) 
            {
                solutionLabel.Text = "Решение найдено!";
                solutionLabel.ForeColor = Color.Green;
                timer1.Stop();
            }
            else
            {
                solutionLabel.Text = "Решение не найдено";
                solutionLabel.ForeColor = Color.Red;
            }
            this.generationLabel.Text = population.generationNumber.ToString();
            this.avgLabel.Text = population.avgFitness.ToString();
            this.bestLabel.Text = population.bestFitness.ToString();

            var objChart = functionChart.ChartAreas[0];

            if (population.generationNumber > objChart.AxisX.Maximum)  objChart.AxisX.Maximum = population.generationNumber;
            if (population.bestFitness > objChart.AxisY.Maximum || population.avgFitness > objChart.AxisY.Maximum) objChart.AxisY.Maximum = Math.Max(population.avgFitness, population.bestFitness);
            functionChart.Series["Average Fitness"].Points.AddXY(population.generationNumber, population.avgFitness);
            functionChart.Series["Best Fitness"].Points.AddXY(population.generationNumber, population.bestFitness);
        }
    }
}
