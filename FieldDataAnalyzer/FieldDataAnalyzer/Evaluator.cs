using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace FieldDataAnalyzer
{
	internal class Evaluator
	{
		public double K_t = 1, K_p = 1;
		private Graph _graph;
		private Interpolator interpolator = new Interpolator("PVT.txt");
		private FieldDescription _fieldData;
		private DateTime _dateEvaluation;
		public ToolStripProgressBar toolStripProgressBar;
		private FieldDataAnalyzerDBEntities db = new FieldDataAnalyzerDBEntities();

		public Evaluator(Graph graph, FieldDescription fieldData, ToolStripProgressBar _toolStripProgressBar)
		{
			_graph = graph;
			_fieldData = fieldData;
			toolStripProgressBar = _toolStripProgressBar;
		}

		private void CalcPipe(Pipe pipe)
		{
			if (pipe.StartNode.P == null)
			{
				CalcNode(pipe.StartNode);
			}

			pipe.EndNode.P = 0;
			pipe.EndNode.T = 0;
			pipe.EndNode.G = 0;
			if (pipe.StartNode.P == 0) return;
			if (pipe.StartNode.T - 273 < 8)
			{
				string aaa = "f";
			}
			ZInterpolationData interpolatedParams = interpolator.FindValue((double) pipe.StartNode.T - 273,
				(double) pipe.StartNode.P*1e-5);
			double Gi = (double) pipe.StartNode.G;

			// 1. Расчёт скорости смеси
			double V = Gi/(interpolatedParams.Ro*Math.PI*Math.Pow(pipe.Data.inner_d, 2)/4);
			// 2. Расчёт числа Рейнольса
			double Re = V * pipe.Data.inner_d / interpolatedParams.V;
			// 3. Коэффициент теплоотдачи от газожидкостной смеси к трубе
			double alpha = 0.021 * interpolatedParams.C / pipe.Data.inner_d * Math.Pow(Re, 0.8) * Math.Pow(_fieldData.Pr, 0.43);
			// 4.	Условный коэффициент теплоотдачи от теплоизоляции в грунт
			double alpha_gr = 2 * _fieldData.LyambdaGr / (pipe.Data.inner_d + 2 * _fieldData.ThicknessIsol) / Math.Log(2 * _fieldData.Depth
			                                                                                                  /pipe.Data.outer_d +
			                                                                                                  Math.Sqrt(
				                                                                                                  Math.Pow(
					                                                                                                  2*
					                                                                                                  _fieldData.Depth/
																													  pipe.Data.outer_d,
					                                                                                                  2) - 1));
			// 5.	Коэффициент теплопередачи (расчётный)
			double lyambdaOkr = 5; //коэффициент теплоотдачи откружающей среды
			double K_tp = 1/(1/alpha + _fieldData.Thickness/_fieldData.LyambdaTr + _fieldData.ThicknessIsol
			                 /_fieldData.LyambdaIs + _fieldData.ThicknessShow/_fieldData.LyambdaSn + 1/lyambdaOkr);

			// 6.	Температура на выходе
			double alpha_tr = 1.0*K_t*K_tp*Math.PI*pipe.Data.inner_d /Gi/_fieldData.Csm; //коэффициент Шухова

			double dt = (1 - Math.Exp(-alpha_tr * pipe.Data.length)) * ((double)pipe.StartNode.T - pipe.Data.temper);
			pipe.EndNode.T += (pipe.StartNode.T - dt);

			// 6а.	Давление на выходе
			double lyambda_0_tr = 0.067*Math.Pow(158/Re + 2*pipe.Data.roughness/pipe.Data.inner_d, 0.2);
			//double big_unkown_coef = lyambda_0_tr * pipe.Data.Length / pipe.Data.InnerD * interpolatedParams.Ro / Math.Pow(interpolatedParams.Ro * Math.PI * Math.Pow(pipe.Data.InnerD, 2) / 4, 2) / 2;
			double dp = lyambda_0_tr*pipe.Data.length/pipe.Data.inner_d*interpolatedParams.Ro*V*V/2;
			if (pipe.StartNode.P > dp) pipe.EndNode.P += pipe.StartNode.P - K_p*dp;
			pipe.EndNode.G = pipe.StartNode.G;
		}

		private void CalcNode(Node node)
		{
			if (node.wells.Count != 0)
			{
				double TG = 0, PG = 0, G = 0;
				foreach (var well in node.wells)
				{
					TG += well.T_shl*well.G_gas;
					PG += well.P_shl*well.G_gas;
					G += well.G_gas;
				}
				if (G == 0)
				{
					node.T = 0;
					node.P = 0;
				}
				else
				{
					node.T = TG/G;
					node.P = PG/G;
				}
				node.G = G;
			}
			else
			{
				var _pipes = _graph.pipes.Where(x => x.EndNode == node).ToList();
				if (_pipes.Count == 0)
				{
					node.P = 0;
					node.T = 0;
					node.G = 0;
				}
				else
					foreach (var pipe in _pipes)
					{
						CalcPipe(pipe);
					}
			}
		}

		public LearningResult Calc(DateTime fromDate, DateTime toDate)
		{
			double[] Ks = { 0.5, 1, 2 }; //массив коэффициентов для интерполирования
			LearningResult result = new LearningResult();
			result.GPResults = new List<GPLearningResult>();
			var datesRange = db.wells_measurements.Where(x => (x.measure_date >= fromDate) && (x.measure_date <= toDate)).Select(y => new { Date = y.measure_date }).Distinct().ToList();

			var dates = (from date in datesRange
						 join meas in db.final_gather_point_measurements on date.Date equals meas.measure_date
						 select new { Date = date.Date }).ToList();
			toolStripProgressBar.Maximum = dates.Count;
			toolStripProgressBar.Minimum = 0;
			toolStripProgressBar.Value = 0;
			foreach (var date in dates)
			{
				toolStripProgressBar.Value++;
				double[,] _PTk = new double[Ks.Length, 2];
				foreach (var wellMeasurement in db.wells_measurements.Where(x => x.measure_date == date.Date).ToList()) //заполнение данных о скважинах за данный день
				{
					var well = _graph.wells.First(x => x.Id == wellMeasurement.well_id);
					well.G_condensat = wellMeasurement.cond_output;
					well.G_gas = wellMeasurement.gas_output;
					well.P_shl = wellMeasurement.inlet_P;
					well.P_ust = wellMeasurement.wellhead_P;
					well.T_shl = wellMeasurement.inlet_T;
					well.T_ust = wellMeasurement.wellhead_T;
					well.a = new double[] { -4f / 5, -1f / 5, 1f };
					well.b = new double[] { -12f / 5, 11f / 5, 1f / 5 };
				}

				for (int i = 0; i < Ks.Length; i++) //расчет P и T для каждого из значений коэффициентов
				{
					K_p = Ks[i];
					K_t = Ks[i];
					CalcNode(_graph.endNode);
					_PTk[i, 0] = (double) _graph.endNode.P;
					_PTk[i, 1] = (double)_graph.endNode.T;
					_graph.Clear();

					foreach (var wellMeasurement in db.wells_measurements.Where(x => x.measure_date == date.Date).ToList()) //заполнение данных о скважинах за данный день
					{
						var well = _graph.wells.First(x => x.Id == wellMeasurement.well_id);
						well.G_condensat = wellMeasurement.cond_output;
						well.G_gas = wellMeasurement.gas_output;
						well.P_shl = wellMeasurement.inlet_P;
						well.P_ust = wellMeasurement.wellhead_P;
						well.T_shl = wellMeasurement.inlet_T;
						well.T_ust = wellMeasurement.wellhead_T;
						well.a = new double[] { -4f / 5, -1f / 5, 1f };
						well.b = new double[] { -12f / 5, 11f / 5, 1f / 5 };
					}
				}

				var realResults = db.final_gather_point_measurements.FirstOrDefault(x => x.measure_date == date.Date);
				double _Kt = GetInterpolatedValue(realResults.Texper, new double[] {_PTk[0, 1], _PTk[1, 1], _PTk[2, 1]}, Ks);
				double _Kp = GetInterpolatedValue(realResults.Pexper, new double[] {_PTk[0, 0], _PTk[1, 0], _PTk[2, 0]}, Ks);
				K_p = _Kp;
				K_t = _Kt;
				CalcNode(_graph.endNode);
				result.GPResults.Add(new GPLearningResult()
				{
					Date = date.Date,
					Pexpr = realResults.Pexper,
					Texpr = realResults.Texper,
					Pf = _PTk[1, 0],
					Tf = _PTk[1, 1],
					Coef_T = _Kt,
					Coef_P = _Kp,
					Pcoef = (double) _graph.endNode.P,
					Tcoef = (double)_graph.endNode.T,
					G = (double) _graph.endNode.G,
					WellMeasurements = db.wells_measurements.Where(x => EntityFunctions.TruncateTime(x.measure_date) == date.Date).ToList()
				});
				_graph.Clear();
				//break;
			}
			result.Coef_P = result.GPResults.Average(x => x.Coef_P);
			result.Coef_T = result.GPResults.Average(x => x.Coef_T);
			return result;
		}

		double GetInterpolatedValue(double value, double[] y, double[] x)
		{
			double a = ((y[0] - y[1]) * (x[0] - x[2]) - (y[0] - y[2]) * (x[0] - x[1])) / ((x[0] - x[1]) * (x[0] - x[2]) * (x[1] - x[2]));
			double b = ((y[0] - y[1]) - a * (x[0] - x[1]) * (x[0] + x[1])) / (x[0] - x[1]);
			double c = y[0] - (a * x[0] + b) * x[0] - value;
			double d = b * b - 4 * a * c;
			double x1 = (-b - Math.Sqrt(d)) / 2 / a;
			double x2 = (-b + Math.Sqrt(d)) / 2 / a;
			if (x1 > 0) return x1;
			if (x2 > 0) return x2;
			return 0;
		}
	}

	internal class LearningResult
	{
		public List<GPLearningResult> GPResults;
		public double Coef_T, Coef_P;
	}

	//class WellCharacterResult

	class GPLearningResult
	{
		public DateTime Date;
		public double Pexpr, Texpr, Coef_P, Coef_T, Pcoef, Tcoef, Pf, Tf, G;
		public List<wells_measurements> WellMeasurements;
	}
	
}
