using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldDataAnalyzer
{
	public class FieldDescription
	{
		public double Csm; //удельная теплоемкость смеси, дж/кг/К
		public double V; //кинематическая вязкость смеси, м2/c
		public double Ro; //плотность смеси, кг/м3
		public double Gg; //относительная доля газа в массовом расходе смеси
		public double Pr; //число Прандтля газа
		public double Tsb; //температура на узле сбора (экспер), К
		public double Thickness; //толщина стенки трубы (средняя), м
		public double ThicknessIsol; //толщина изоляции (средняя), м
		public double ThicknessShow; //толщина снежного покрова (средняя), м
		public double Depth; //глубина залегания трубопровода (средняя), м
		public double LyambdaTr; //к-т теплопроводности материала трубы, Вт/м/К
		public double LyambdaIs; //к-т теплопроводности изоляции, Вт/м/К
		public double LyambdaSn; //к-т теплопроводности снега, Вт/м/К
		public double LyambdaGr; //к-т теплопроводности грунта, Вт/м/К
	}
}
