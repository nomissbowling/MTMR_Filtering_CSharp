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
		private Mat OutputImage;

		/// <summary>
		///	PpmImageにかけたいフィルタ(複数できるように)
		/// </summary>
		private List<int[][]> Filter;

		/// <summary>
		/// フィルタの初期化とか
		/// </summary>
		private void Initialize() {
			Filter = new List<int[][]>()
		} //End_Method

		/// <summary>
		/// Main文は簡潔に書きたい
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args) {
			//使う変数とかここで宣言(下でもいいけど)
			var bebebe = new Bebebe();

			//初期化
			bebebe.Initialize();

			//入力画像Open
			Console.Write("入力画像ファイル名を入力（拡張子はなくて良い） : ");
			var inputPath = Console.ReadLine();
			bebebe.PpmImage = new Mat(inputPath);

			//グレースケール化
			bebebe.GrayImage = PPMPGM_Utility.MakeGrayImage(bebebe.PpmImage);

			//フィルタリング処理


		} //End_Function
	} //End_Class
} //End_Namespace
