using UnityEditor;
using UnityEngine;
using ChanibaL;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class PlotGenerator : EditorWindow
{


	static void Scatter2D(string name, Func<RandomGenerator, Vector2> gen, int count = 1000, float xmin = -1.5f, float xmax = 1.5f, float ymin = -1.5f, float ymax = 1.5f)
	{
		string pathPlot = Application.dataPath + "/../Docs/Plots/" + name + ".plot";
		StringBuilder sb = new StringBuilder();


		sb.AppendLine("set terminal pngcairo size 315, 250 enhanced font 'Verdana,10'");
		sb.AppendLine("set output '" + name + ".png'");

		sb.AppendLine("set grid");
		sb.AppendLine("set nokey");
		sb.AppendLine("set size ratio 1");
		sb.AppendLine("set style fill transparent solid 0.05 noborder");
		sb.AppendLine("set style circle radius 0.05");
		sb.AppendLine("set xtics nomirror; set ytics nomirror; set ztics nomirror;");

		sb.AppendLine(string.Format("plot[{0:0.00}:{1:0.00}] [{2:0.00}:{3:0.00}] '{4}.data2d' with circles lt rgb \"#0050BE\"", xmin, xmax, ymin, ymax, name));
		sb.AppendLine("quit");

		File.WriteAllText(pathPlot, sb.ToString());

		List<Vector2> points = new List<Vector2>();
		RandomGenerator rng = new RandomGenerator(0);
		for (; count-- > 0;)
			points.Add(gen(rng));
		string pathData = Application.dataPath + "/../Docs/Plots/" + name + ".data2d";
		File.WriteAllLines(pathData, points.ConvertAll(v => string.Format("{0:00.0000}, {1:00.0000}", v.x, v.y)).ToArray());
	}

	static void Scatter3D(string name, Func<RandomGenerator, Vector3> gen, int count = 1000, float rmin = -1f, float rmax = 1f)
	{
		string pathPlot = Application.dataPath + "/../Docs/Plots/" + name + ".plot";
		StringBuilder sb = new StringBuilder();

		sb.AppendLine("set terminal pngcairo enhanced font \"arial, 10\" fontscale 1.0 size 400, 300");
		sb.AppendLine("set zeroaxis");
		sb.AppendLine("set grid");
		sb.AppendLine("set nokey");

		sb.AppendLine("set size ratio 1");

		sb.AppendLine("rlow = " + rmin);
		sb.AppendLine("rhigh = " + rmax);
		sb.AppendLine("set xrange [rlow:rhigh]; set yrange [rlow:rhigh]; set zrange [rlow:rhigh]");
		sb.AppendLine("set xtics axis nomirror; set ytics axis nomirror; set ztics axis nomirror;");
		sb.AppendLine("set border 0");
		sb.AppendLine("set xyplane at 0");
		sb.AppendLine("set xzeroaxis lt - 1");
		sb.AppendLine("set yzeroaxis lt - 1");
		sb.AppendLine("set zzeroaxis lt - 1");

		sb.AppendLine("set view equal xyz");
		sb.AppendLine("set pointsize");
		sb.AppendLine("set datafile separator \",\"");


		sb.AppendLine("do for [i = 0:100] {");
		sb.AppendLine("	set output sprintf('__frame%04i.png', i)");

		sb.AppendLine("	set view 60, 45 + 30 * cos(3.14 * i / 50.0)");

		sb.AppendLine("	splot '" + name + ".data3d' with points lt rgb \"#0050BE\"");
		sb.AppendLine("}");

		sb.AppendLine("system('ffmpeg -y -r 25 -i __frame%04d.png -c:v libx264 -vf fps=25 -pix_fmt yuv420p \"" + name + ".mp4\"')");
		sb.AppendLine("system('convert -gravity center -crop 315x250+0+0 -delay 4 __frame*.png \"" + name + ".gif\"');");
		sb.AppendLine("system('rm __frame*png');");
		sb.AppendLine("quit");

		File.WriteAllText(pathPlot, sb.ToString());

		List<Vector3> points = new List<Vector3>();
		RandomGenerator rng = new RandomGenerator(0);
		for (; count-- > 0;)
			points.Add(gen(rng));
		string pathData = Application.dataPath + "/../Docs/Plots/" + name + ".data3d";
		File.WriteAllLines(pathData, points.ConvertAll(v => string.Format("{0:00.0000}, {1:00.0000}, {2:00.0000}", v.x, v.y, v.z)).ToArray());
	}


	static void ScatterQuaternions(string name, Func<RandomGenerator, Quaternion> gen, int count = 1000, float rmin = -1f, float rmax = 1f)
	{
		string pathPlot = Application.dataPath + "/../Docs/Plots/" + name + ".plot";
		StringBuilder sb = new StringBuilder();


		//sb.AppendLine("set terminal pngcairo size 400, 400 enhanced font 'Verdana,10'");
		//sb.AppendLine("set output '" + name + ".png'");

		//sb.AppendLine("set grid");
		//sb.AppendLine("set nokey");

		//sb.AppendLine("set view equal xyz");
		//sb.AppendLine("set pointsize");

		//sb.AppendLine(string.Format("splot '{6}.data3d'", xmin, xmax, ymin, ymax, zmin, zmax, name));
		//sb.AppendLine("quit");



		sb.AppendLine("set terminal pngcairo enhanced font \"arial, 10\" fontscale 1.0 size 400, 300");
		sb.AppendLine("set zeroaxis");
		sb.AppendLine("set grid");
		sb.AppendLine("set nokey");

		sb.AppendLine("set size ratio 1");
		// sb.AppendLine("set style fill transparent solid 0.05 noborder");
		// sb.AppendLine("set style circle radius 0.05");

		sb.AppendLine("rlow = " + rmin);
		sb.AppendLine("rhigh = " + rmax);
		sb.AppendLine("set xrange [rlow:rhigh]; set yrange [rlow:rhigh]; set zrange [rlow:rhigh]");
		sb.AppendLine("set xtics axis nomirror; set ytics axis nomirror; set ztics axis nomirror;");
		sb.AppendLine("set border 0");
		sb.AppendLine("set xyplane at 0");
		sb.AppendLine("set xzeroaxis lt - 1");
		sb.AppendLine("set yzeroaxis lt - 1");
		sb.AppendLine("set zzeroaxis lt - 1");

		sb.AppendLine("set view equal xyz");
		sb.AppendLine("set pointsize");
		sb.AppendLine("set datafile separator \",\"");


		sb.AppendLine("do for [i = 0:100] {");
		sb.AppendLine("	set output sprintf('__frame%04i.png', i)");

		sb.AppendLine("	set view 60, 45 + 30 * cos(3.14 * i / 50.0)");

		sb.AppendLine("	splot '" + name + ".data3d' with lines lt rgb \"#0050BE\"");
		sb.AppendLine("}");

		sb.AppendLine("system('ffmpeg -y -r 25 -i __frame%04d.png -c:v libx264 -vf fps=25 -pix_fmt yuv420p \"" + name + ".mp4\"')");
		sb.AppendLine("system('convert -gravity center -crop 315x250+0+0 -delay 4 __frame*.png \"" + name + ".gif\"');");
		sb.AppendLine("system('rm __frame*png');");
		sb.AppendLine("quit");

		File.WriteAllText(pathPlot, sb.ToString());

		List<Quaternion> points = new List<Quaternion>();
		RandomGenerator rng = new RandomGenerator(0);
		for (; count-- > 0;)
			points.Add(gen(rng));
		string pathData = Application.dataPath + "/../Docs/Plots/" + name + ".data3d";
		File.WriteAllLines(pathData, points.ConvertAll(q => {
			Vector3 v = q * Vector3.forward;
			Vector3 u = q * Vector3.up;
			return string.Format("{0:00.0000}, {1:00.0000}, {2:00.0000}, {3:00.0000}, {4:00.0000}, {5:00.0000}, ", v.x, v.y, v.z, u.x, u.y, u.z);
		}).ToArray());
	}


	[MenuItem("Tools/Generate plots for RandomGenerator")]
	public static void Generate()
	{
		Scatter2D("Float01", rng => rng.GetFloat01() * Vector2.right, 50);
		Scatter2D("Float-0.5,0.75", rng => rng.GetFloatRange(-0.5f, 0.75f) * Vector2.right, 50);
		Scatter2D("IntRange0,32", rng => rng.GetIntRange(1, 3) * Vector2.right, 100, 0, 4, -1, 1);
		Scatter2D("InUnitCircle", rng => rng.GetInUnitCircle());
		Scatter2D("OnUnitCircle", rng => rng.GetOnUnitCircle(), 200);
		Scatter2D("InUnitSquare", rng => rng.GetInUnitSquare());
		Scatter2D("OnUnitSquare", rng => rng.GetOnUnitSquare(), 200);
		Scatter3D("InUnitSphere", rng => rng.GetInUnitSphere(), 1000);
		Scatter3D("OnUnitSphere", rng => rng.GetOnUnitSphere(), 1000);
		ScatterQuaternions("Quaternions", rng => rng.GetQuaternion());
		Scatter3D("InUnitCube", rng => rng.GetInUnitCube(), 2000, 0, 1);
		Scatter3D("OnUnitCube", rng => rng.GetOnUnitCube(), 2000, 0, 1);
	}


}
