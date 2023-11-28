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
    public partial class DetailedPopulation : Form
    {
        public DetailedPopulation()
        {
            InitializeComponent();
        }
        public void ShowPopulation(List<Individual> individuals)
        {
            dataGridView1.Rows.Clear();

            foreach (var obj in individuals)
            {
                int rowIndex = dataGridView1.Rows.Add();

                dataGridView1.Rows[rowIndex].Cells["W1"].Value = obj.Weight1;
                dataGridView1.Rows[rowIndex].Cells["W2"].Value = obj.Weight2;
                dataGridView1.Rows[rowIndex].Cells["W3"].Value = obj.Weight3;
                dataGridView1.Rows[rowIndex].Cells["Fitness"].Value = obj.Fitness;
            }
        }
    }
}
