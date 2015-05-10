using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FieldDataAnalyzer
{
	class FileParser
	{

		public FileParser()
		{

		}

		public FieldDescription ParseGeneralData(string filename)
		{			
			var sr = new StreamReader(filename);
			return new FieldDescription()
			{
				Csm = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				V = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",", ".")),
				Ro = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",", ".")),
				Gg = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",", ".")),
				Pr = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",", ".")),
				Tsb = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",", ".")),
				Thickness = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				ThicknessIsol = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				ThicknessShow = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				Depth = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				LyambdaTr = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				LyambdaIs = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				LyambdaSn = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",",".")),
				LyambdaGr = Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ', '\t' })[0].Replace(",","."))
			};
		}

		public List<string[]> ParseSchema(string filename)
		{
			var result = new List<string[]>();
			var sr = new StreamReader(filename);
			string[] buf;
			while (!sr.EndOfStream)
			{
				buf = sr.ReadLine().Split(new char[] {' ', '\t'});
				if (buf.Count() != 0)
					result.Add(new string[]{buf[0], buf[1]});
			}
			return result;
		}

		public List<PipeData> ParsePipes(string filename)
		{
			Converter converter = new Converter();
			var result = new List<PipeData>();
			var sr = new StreamReader(filename);
			string[] buf;
			sr.ReadLine();
			while (!sr.EndOfStream)
			{
				buf = sr.ReadLine().Split(new char[] { ' ', '\t' });
				if (buf.Count() != 0)
					result.Add(new PipeData()
					{
						Num = Convert.ToInt32(buf[0].Replace(",", ".")),
						Length = Convert.ToDouble(buf[1].Replace(",", ".")),
						OuterD = converter.ToM(Convert.ToDouble(buf[2].Replace(",", ".")), Units.Mm),
						Width = converter.ToM(Convert.ToDouble(buf[3].Replace(",", ".")), Units.Mm),
						InnerD = converter.ToM(Convert.ToDouble(buf[4].Replace(",", ".")), Units.Mm),
						Roughness = converter.ToM(Convert.ToDouble(buf[5].Replace(",", ".")), Units.Mm),
						StartNode = buf[6],
						EndNode = buf[7],
						OuterT = converter.ToK(Convert.ToDouble(buf[8].Replace(",", ".")), Units.C)
					});
			}
			return result;
		}

		public List<string[]> ParseWells(string filename)
		{
			var result = new List<string[]>();
			var sr = new StreamReader(filename);
			string[] buf;
			while (!sr.EndOfStream)
			{
				buf = sr.ReadLine().Split(new char[] { ' ', '\t' });
				if (buf.Count() != 0)
					result.Add(new string[] { buf[0], buf[1] });
			}
			return result;
		}

		public List<WellData> ParseSkv(string filename)
		{
			Converter converter = new Converter();

			var result = new List<WellData>();
			var sr = new StreamReader(filename);
			string[] buf;
			sr.ReadLine();
			sr.ReadLine();
			sr.ReadLine();
			while (!sr.EndOfStream)
			{
				buf = sr.ReadLine().Split(new char[] { ' ', '\t' });
				if ((buf.Count() == 8)&&(buf.Count(x => x == "") == 0))
					result.Add(new WellData
					{
						Name = buf[0],
						Date = Convert.ToDateTime(buf[1]),
						G_gas = Convert.ToDouble(buf[2].Replace(",", ".")) * 1000 / converter.ToSec(1, Units.Day),
						G_condensat = converter.ToKG(Convert.ToDouble(buf[3].Replace(",", ".")), Units.T) / converter.ToSec(1, Units.Day),
						P_ust = converter.ToPascal(Convert.ToDouble(buf[4].Replace(",", "."))),
						T_ust = converter.ToK(Convert.ToDouble(buf[5].Replace(",", ".")), Units.C),
						P_shl = converter.ToPascal(Convert.ToDouble(buf[6].Replace(",", "."))),
						T_shl = converter.ToK(Convert.ToDouble(buf[7].Replace(",", ".")), Units.C)
					});
			}
			return result;
		}

		public List<SborData> ParseSbor(string filename)
		{
			Converter converter = new Converter();

			var result = new List<SborData>();
			var sr = new StreamReader(filename);
			string[] buf;
			sr.ReadLine();
			sr.ReadLine();
			sr.ReadLine();
			while (!sr.EndOfStream)
			{
				buf = sr.ReadLine().Split(new char[] { ' ', '\t' });
				if ((buf.Count() == 3)&&(buf.Count(x => x == "") == 0))
					result.Add(new SborData
					{
						Date = Convert.ToDateTime(buf[0]),
						P = converter.ToPascal(Convert.ToDouble(buf[1].Replace(",", "."))),
						T = converter.ToK(Convert.ToDouble(buf[2].Replace(",", ".")), Units.C)
					});
			}
			return result;
		}
	}

	public class PipeData
	{
		public int Num;
		public double Length; //длина участка трубопровода, м
		public double OuterD; //внешний диаметр по ГОСТ, мм
		public double Width; // Толщина стенки, мм
		public double InnerD; //Внутренний диаметр, мм
		public double Roughness; //Шероховатость, мм
		public string StartNode; //начальный узел
		public string EndNode; //конечный узел
		public double OuterT; //температура окружающей среды, градусы Цельсия
	}

	public class WellData
	{
		public int Id;
		public string Name; 
		public DateTime Date; 
		public double G_gas; //дебит газа, тыс. м3/сут (ст.)
		public double G_condensat; //дебит конденсата, т/сут (ст.)
		public double P_ust; //давление устьевое, кг/см2
		public double T_ust; //температура устьевая, градусы Цельсия
		public double P_shl; //давление (шл.), кг/см2 
		public double T_shl; //температура (шл.), градусы Цельсия
		public double[] a, b; //коэффициенты первой и второй параболы
		public double[] G, P;//значения массового расхода и давления (точки на графике)
	}

	public class SborData
	{
		public DateTime Date;
		public double P; //давление, кг/см2
		public double T; //температура, , градусы Цельсия
	}
}
