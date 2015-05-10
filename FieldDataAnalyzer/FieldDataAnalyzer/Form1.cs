using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FieldDataAnalyzer
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			FileParser parser = new FileParser();
			fieldDescription = parser.ParseGeneralData("UKPG2_gen.txt");
		}

		private Graph graph = new Graph();
		private LearningResult results;
		private FieldDescription fieldDescription;

		private void button1_Click(object sender, EventArgs e)
		{

			FieldDataAnalyzerDBEntities db = new FieldDataAnalyzerDBEntities();
			//foreach (var well in db.wells)
			//{
			//	well.a1 = -4f/5;
			//	well.a2 = -1f/5;
			//	well.a3 = 1f;
			//	well.b1 = -12f/5;
			//	well.b2 = 11f/5;
			//	well.b3 = 1f/5;
			//}
			//db.SaveChanges();
			Evaluator evaluator = new Evaluator(graph, fieldDescription, toolStripProgressBar1);

			DateTime dateFrom = db.wells_measurements.Min(x => x.measure_date), dateTo = db.wells_measurements.Max(x => x.measure_date);

			//EvaluatorUnknownG evaluator1 = new EvaluatorUnknownG(graph, fieldDescription, toolStripProgressBar1);
			//evaluator1.Calc(dateFrom, dateTo);

			//foreach (var well in graph.wells)
			//{
			//	dataGridView4.RowCount++;
			//	dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[0].Value = well.Name;
			//	dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[1].Value = (well.a != null) ? String.Format("{0}; {1}; {2}", well.a[0], well.a[1], well.a[2]) : "";
			//	dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[2].Value = (well.b != null) ?String.Format("{0}; {1}; {2}", well.b[0], well.b[1], well.b[2]) : "";
			//	dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[3].Value = (well.P != null) ?String.Format("{0}; {1}; {2}", well.P[0], well.P[1], well.P[2]) : "";
			//	dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[4].Value = (well.G != null) ? String.Format("{0}; {1}; {2}", well.G[0], well.G[1], well.G[2]) : "";
			//}

			//foreach (var node in graph.nodes)
			//{
			//	dataGridView3.RowCount++;
			//	dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[0].Value = node.Name;
			//	dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[1].Value = (node.a != null) ? String.Format("{0}; {1}; {2}", node.a[0], node.a[1], node.a[2]) : "";
			//	dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[2].Value = (node.b != null) ? String.Format("{0}; {1}; {2}", node.b[0], node.b[1], node.b[2]) : "";
			//	dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[3].Value = (node.P_ != null) ? String.Format("{0}; {1}; {2}", node.P_[0], node.P_[1], node.P_[2]) : "";
			//	dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[4].Value = (node.G_ != null) ? String.Format("{0}; {1}; {2}", node.G_[0], node.G_[1], node.G_[2]) : "";
			//}

			results = evaluator.Calc(dateFrom, dateTo);
			foreach (var well in graph.wells)
			{
				dataGridView2.RowCount++;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = well.Name;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[1].Value = well.G_gas;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = well.G_condensat;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[3].Value = well.T_shl;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[4].Value = well.T_ust;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[5].Value = well.P_shl;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[6].Value = well.P_ust;
			}
			foreach (var node in graph.nodes)
			{
				dataGridView1.RowCount++;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = node.Name;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = (node.NextNode == null) ? "NULL" : node.NextNode.Name;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = node.G;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = node.T;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[4].Value = node.P;
			}
			foreach (var result in results.GPResults)
			{
				dataGridView1.RowCount++;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = result.Date.Date;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = result.Pexpr;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = result.Texpr;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = result.Coef_P;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[4].Value = result.Coef_T;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[5].Value = result.Pcoef;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[6].Value = result.Tcoef;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[7].Value = result.Pf;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[8].Value = result.Tf;
				dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[9].Value = result.G;
			}
			label4.Text = String.Format("Давление (Kp): {0}", results.Coef_P);
			label5.Text = String.Format("Давление (Kt): {0}", results.Coef_T);
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			DateTime selectedDate = Convert.ToDateTime(dataGridView1.CurrentRow.Cells[0].Value);
			if (selectedDate == DateTime.Parse("01.01.0001")) return;
			label7.Text = String.Format("Дaта {0}", selectedDate.Date);
			dataGridView2.RowCount = 0;
			foreach (var well in results.GPResults.Where(x => x.Date.Date == selectedDate.Date).FirstOrDefault().WellMeasurements
				)
			{
				dataGridView2.RowCount++;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = well.well_id;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[1].Value = well.gas_output;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = well.cond_output;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[3].Value = well.wellhead_P;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[4].Value = well.wellhead_T;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[5].Value = well.inlet_P;
				dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[6].Value = well.inlet_T;
			}

		}

		private void button2_Click(object sender, EventArgs e)
		{
			FieldDataAnalyzerDBEntities db = new FieldDataAnalyzerDBEntities();
			DateTime dateFrom = db.wells_measurements.Min(x => x.measure_date), dateTo = db.wells_measurements.Max(x => x.measure_date);
			EvaluatorUnknownG evaluator1 = new EvaluatorUnknownG(graph, fieldDescription, toolStripProgressBar1);
			evaluator1.Calc(dateFrom, dateTo);

			foreach (var well in graph.wells)
			{
				dataGridView4.RowCount++;
				dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[0].Value = well.Name;
				dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[1].Value = (well.a != null) ? String.Format("{0}; {1}; {2}", well.a[0], well.a[1], well.a[2]) : "";
				dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[2].Value = (well.b != null) ? String.Format("{0}; {1}; {2}", well.b[0], well.b[1], well.b[2]) : "";
				dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[3].Value = (well.P != null) ? String.Format("{0}; {1}; {2}", well.P[0], well.P[1], well.P[2]) : "";
				dataGridView4.Rows[dataGridView4.RowCount - 1].Cells[4].Value = (well.G != null) ? String.Format("{0}; {1}; {2}", well.G[0], well.G[1], well.G[2]) : "";
			}

			foreach (var node in graph.nodes)
			{
				dataGridView3.RowCount++;
				dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[0].Value = node.Name;
				dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[1].Value = (node.a != null) ? String.Format("{0}; {1}; {2}", node.a[0], node.a[1], node.a[2]) : "";
				dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[2].Value = (node.b != null) ? String.Format("{0}; {1}; {2}", node.b[0], node.b[1], node.b[2]) : "";
				dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[3].Value = (node.P_ != null) ? String.Format("{0}; {1}; {2}", node.P_[0], node.P_[1], node.P_[2]) : "";
				dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[4].Value = (node.G_ != null) ? String.Format("{0}; {1}; {2}", node.G_[0], node.G_[1], node.G_[2]) : "";
			}
		}

		private void dataGridView4_SelectionChanged(object sender, EventArgs e)
		{
			double Gkr = 15, Pkr = 35000000, beta_kr = 0.2;
			//DateTime selectedDate = Convert.ToDateTime(dataGridView1.CurrentRow.Cells[0].Value);
			if (dataGridView4.CurrentRow.Cells[0].Value == null) return;
			var well = graph.wells.First(x => x.Name == dataGridView4.CurrentRow.Cells[0].Value.ToString());;

			if ((well == null)||(well.a == null)) return;

			chart3.Series["Series1"].Points.Clear();
			chart3.Series["Series1"].Points.AddXY(0, well.G[0]);
			chart3.Series["Series1"].Points.AddXY(well.P[0], well.G[0]);
			chart3.Series["Series1"].Points.AddXY(well.P[1], well.G[1]);
			chart3.Series["Series1"].Points.AddXY(well.P[2], well.G[2]);
			chart3.Series["Series1"].Points.AddXY(well.P[3], well.G[3]);
			chart3.Series["Series1"].Points.AddXY(well.P[4], well.G[4]);
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
	}
}
