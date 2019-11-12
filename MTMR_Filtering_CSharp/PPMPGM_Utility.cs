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
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using OpenCvSharp.Utilities;

namespace MTMR_Filtering_CSharp {
	static class PPMPGM_Utility {
		/// <summary>
		/// フィルタリングに使う，フィルタが画像からはみ出るサイズ
		/// </summary>
		private static int FilSize;

		/// <summary>
		/// グレースケール化(OpenCVの関数使ったら負けな気がしたのでMTMR先生のやつ)
		/// </summary>
		/// <param name="color">PpmImageとかのカラー画像</param>
		/// <returns></returns>
		public static Mat MakeGrayImage(Mat color) {
			var gray = new Mat(color.Height, color.Width, MatType.CV_32S);

			for (int i = 0; i < color.Height; ++i) {
				for (int j = 0; j < color.Width; ++j) {
					var pix = color.At<Vec3b>(i, j);
					int intensity = (int)(0.298839 * pix[0] + 0.586811 * pix[1] + 0.114350 * pix[2]);
					gray.Set<int>(i, j, intensity);
				} //End_For
			} //End_For

			return gray;
		} //End_Method

		/// <summary>
		/// フィルタリング処理を行う
		/// </summary>
		/// <param name="img">対象画像(グレースケール)</param>
		/// <param name="filter">かけたいフィルタ(任意のサイズ)</param>
		/// <returns></returns>
		public static Mat FilteringGrayScale(Mat img, float[][] filter) {
			int filSize = (filter.Length - 1) / 2;  //はみ出るサイズ
			var mat = new Mat(img.Height, img.Width, MatType.CV_32S);
			//各画素にフィルタリングしていく
			for(int i = 0; i < img.Height; ++i) {
				for(int j = 0; j < img.Width; ++j) {
					mat.Set<int>(i, j, MultiplyAddGrayScale(img, filter, j, i));
				} //End_For
			} //End_For
			return mat;
		} //End_Method

		/// <summary>
		/// 1画素に対してフィルタリング(積和演算)する(グレースケール)
		/// </summary>
		/// <param name="img">対象画像</param>
		/// <param name="filter">フィルタ</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <returns></returns>
		private static int MultiplyAddGrayScale(Mat img, float[][] filter, int x, int y) {
			float value = 0;
			int i = 0, j = 0; //filterの現在値
			for(int row = y - FilSize; row <= y + FilSize; ++row) {
				j = 0;
				for(int col = x - FilSize; col <= x + FilSize; ++col) {
					//画素値(はみ出てたら0)
					int pix = 0;
					if (0 <= row && row < img.Height && 0 <= col && col < img.Width) { pix = img.At<int>(row, col); }
					//積和
					value += pix * filter[i][j];
					//アクセッサ更新
					j += 1;
				} //End_For
				//アクセッサ更新
				i += 1;
			} //End_For
			//マイナスの値が出てくるとうんち(どれくらいエッジになってるかわかればいいので方向情報は削除だ)
			return Math.Abs((int)value);
		} //End_Method
	} //End_Class
} //End_NameSpace
