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
		private List<int[][]> Filters;

		/// <summary>
		/// フィルタの初期化とか
		/// </summary>
		private void Initialize() {
			//微分フィルタ(縦横2*2種類)，プリューウィットフィルタ(縦横2*2種類)，ソーベルフィルタ(縦横2*2種類)，ラプラシアンフィルタそれぞれ縦横(3x3)
			int filterNum = 13;
            int size = 3;
            this.Filters = new List<int[][]>(filterNum);
            for(int i = 0; i < this.Filters.Count; ++i) {
                this.Filters[i] = new int[size][];
                for(int j = 0; j < size; ++j) {
                    this.Filters[i][j] = new int[size];
                } //End_For
            } //End_For

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
			//ラプラシアンフィルタ
			filters[12] = "0 1 0; 1 -4 1; 0 1 0";


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
