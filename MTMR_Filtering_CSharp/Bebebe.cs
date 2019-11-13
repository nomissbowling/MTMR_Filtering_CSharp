/*==============================================*/
/*		D1レポート用サンプルプログラム				*/
/*		作成者氏名　岡部蒼太						*/
/*==============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using OpenCvSharp.Utilities;
using System.Text.RegularExpressions;

namespace MTMR_Filtering_CSharp {
	class Bebebe {
		/// <summary>
		/// エッジ検出したいPPM画像(rgb3チャネル)
		/// </summary>
		private Mat PpmImage;

		/// <summary>
		/// PpmImageをグレースケール変換したやつ(1チャネル)
		/// </summary>
		private Mat GrayImage;

		/// <summary>
		/// 出力画像用(エッジ検出画像保存用 1チャネル．．．グレースケール)
		/// </summary>
		private Mat[] OutputImage;

		/// <summary>
		///	PpmImageにかけたいフィルタ(複数できるように)
		/// </summary>
		private List<float[][]> Filters;

		/// <summary>
		/// 平滑化フィルタ(ガウシアンフィルタ)
		/// </summary>
		private float[][] SmoothFilter;

		/// <summary>
		/// ラプラシアンフィルタ
		/// </summary>
		private float[][] LaplacianFilter;

		/// <summary>
		/// 各種画像の保存用の名前
		/// </summary>
		private string[] SaveNames;
		
		/// <summary>
		/// フィルタ形成用
		/// </summary>
		private float[][] StrToIntArr2d(string str) {
			return Regex.Split(str, "; ").Select(a => a.Split(' ').Select(b => float.Parse(b)).ToArray()).ToArray();
			//return str.Split(';').Select(a => a.Split(' ').Select(b => float.Parse(b)).ToArray()).ToArray();
		} //End_Method

		/// <summary>
		/// フィルタの初期化とか
		/// </summary>
		private void Initialize() {
			//微分フィルタ(縦横2*2種類)，プリューウィットフィルタ(縦横2*2種類)，ソーベルフィルタ(縦横2*2種類)
			int filterNum = 12;
			//フィルタサイズ(3x3で固定)
            int size = 3;

			//変数初期化
            this.Filters = new List<float[][]>();
            for(int i = 0; i < filterNum; ++i) {
                this.Filters.Add(new float[size][]);
                for(int j = 0; j < size; ++j) {
                    this.Filters[i][j] = new float[size];
                } //End_For
            } //End_For
			this.SmoothFilter = new float[size][];
			for(int i = 0; i < size; ++i) { this.SmoothFilter[i] = new float[size]; }

			var filters = new string[filterNum];
			//微分フィルタ横 右から左
			filters[0] = "0 0 0; 0 1 -1; 0 0 0";
			//微分フィルタ横 左から右
			filters[1] = "0 0 0; -1 1 0; 0 0 0";
			//微分フィルタ縦 上から下
			filters[2] = "0 -1 0; 0 1 0; 0 0 0";
			//微分フィルタ縦 下から上
			filters[3] = "0 0 0; 0 1 0; 0 -1 0";
			//プリューウィットフィルタ横 右から左
			filters[4] = "-1 0 1; -1 0 1; -1 0 1";
			//プリューウィットフィルタ横 左から右
			filters[5] = "1 0 -1; 1 0 -1; 1 0 -1";
			//プリューウィットフィルタ縦 上から下
			filters[6] = "1 1 1; 0 0 0; -1 -1 -1";
			//プリューウィットフィルタ縦 下から上
			filters[7] = "-1 -1 -1; 0 0 0; 1 1 1";
			//ソーベルフィルタ横 右から左
			filters[8] = "-1 0 1; -2 0 2; -1 0 1";
			//ソーベルフィルタ横 左から右
			filters[9] = "1 0 -1; 2 0 -2; 1 0 -1";
			//ソーベルフィルタ縦 上から下
			filters[10] = "1 2 1; 0 0 0; -1 -2 -1";
			//ソーベルフィルタ縦 下から上
			filters[11] = "-1 -2 -1; 0 0 0; 1 2 1";

			//ガウシアンフィルタ
			string gauss = "1 2 1; 2 4 2; 1 2 1";

			//ラプラシアンフィルタ
			string lap = "0 1 0; 1 -4 1; 0 1 0";

			//各種フィルタを値に変換
			for (int i = 0; i < filterNum; ++i) {
				this.Filters[i] = this.StrToIntArr2d(filters[i]);
			} //End_For
			this.SmoothFilter = this.StrToIntArr2d(gauss);
			this.SmoothFilter.Select(a => a.Select(b => b / 16).ToArray()).ToArray();
			this.LaplacianFilter = this.StrToIntArr2d(lap);


			//保存用の名前
			this.SaveNames = new string[filterNum + 1];
			this.SaveNames[0] = "DifferentialFilter_X_RightToLeft";
			this.SaveNames[1] = "DifferentialFilter_X_LeftToRight";
			this.SaveNames[2] = "DifferentialFilter_Y_TopToBottom";
			this.SaveNames[3] = "DifferentialFilter_Y_BottomToTop";
			this.SaveNames[4] = "PrewittFilter_X_RightToLeft";
			this.SaveNames[5] = "PrewittFilter_X_LeftToRight";
			this.SaveNames[6] = "PrewittFilter_Y_TopToBottom";
			this.SaveNames[7] = "PrewittFilter_Y_BottomToTop";
			this.SaveNames[8] = "SobelFilter_X_RightToLeft";
			this.SaveNames[9] = "SobelFilter_X_LeftToRight";
			this.SaveNames[10] = "SobelFilter_Y_TopToBottom";
			this.SaveNames[11] = "SobelFilter_Y_BottomToTop";
			this.SaveNames[this.SaveNames.Length - 1] = "LogFilter";
		} //End_Method

		/// <summary>
		/// Main文は簡潔に書きたい
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args) {
			//使う変数とかここで宣言(下でもいいけど)
			var bebebe = new Bebebe();
			var ext = ".png";

			//初期化
			bebebe.Initialize();

			//入力画像Open
			Console.Write("入力画像ファイル名を入力（拡張子はなくて良い） : ");
			var inputPath = Console.ReadLine() + ".ppm";
			Console.WriteLine("入力画像 : " + inputPath);
			try {
				bebebe.PpmImage = new Mat(inputPath);
			}catch (Exception e) {
				Console.WriteLine("入力画像のパスが死んでるかもね");
				Console.WriteLine(e.ToString());
			} //End_TryCatch

			//先に出力先聞いといたほうがストレスレス(実行時間を待たなくてよいので)
			Console.WriteLine("出力先パスを入力(ファイル名以下はなくて良い(つけると死ぬ)) : ");
			var outputPath = Console.ReadLine() + "\\";
			Console.WriteLine("保存先 : " + outputPath);

			//グレースケール化
			bebebe.GrayImage = PPMPGM_Utility.MakeGrayImage(bebebe.PpmImage);
			bebebe.GrayImage.SaveImage(outputPath + "GrayScale"+ext);

			//出力用画像変数初期化
			bebebe.OutputImage = new Mat[bebebe.Filters.Count + 1];

			//フィルタリング処理(微分フィルタ，プリューウィットフィルタ，ソーベルフィルタ)
			for(int i = 0; i < bebebe.Filters.Count; ++i) {
				Console.WriteLine("処理始めますよ : " + bebebe.SaveNames[i] + ext);
				bebebe.OutputImage[i] = PPMPGM_Utility.FilteringGrayScale(bebebe.GrayImage, bebebe.Filters[i]);
			} //End_For

			//フィルタリング処理(ガウシアンフィルタ → ラプラシアンフィルタ ≡ ログフィルタ)
			Console.WriteLine("処理始めますよ : " + "LogFilter" + ext);Console.WriteLine();
			bebebe.OutputImage[bebebe.OutputImage.Length - 1] = PPMPGM_Utility.FilteringGrayScale(bebebe.GrayImage, bebebe.SmoothFilter);
			bebebe.OutputImage[bebebe.OutputImage.Length - 1] = PPMPGM_Utility.FilteringGrayScale(bebebe.OutputImage[bebebe.OutputImage.Length - 1], bebebe.LaplacianFilter);

			//保存していく
			for(int i = 0; i < bebebe.OutputImage.Length; ++i) {
				var path = outputPath + bebebe.SaveNames[i] + ext;
				Console.WriteLine("出力パス : " + path);
				bebebe.OutputImage[i].SaveImage(path);
			} //End_For
		} //End_Method
	} //End_Class
} //End_Namespace
