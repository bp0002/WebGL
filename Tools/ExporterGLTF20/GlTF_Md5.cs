#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.Kanau.Editor.Utils;

public class GlTF_Md5 {
	public static string getMd5(int[] data){
		return byteToMd5 (getByte (data));
	}
	public static string getMd5(float[] data){
		return byteToMd5 (getByte (data));
	}

	public static string getMd5(Vector2[] data){
		return byteToMd5 (getByte (data));
	}

	public static string getMd5(Vector3[] data) {
		return byteToMd5 (getByte (data));
	}

	public static string getMd5(Vector4[] data) {
		return byteToMd5 (getByte (data));
	}

	public static string getMd5(Color[] data) {
		return byteToMd5 (getByte (data));
	}

	public static string byteToMd5(float[] data ){
		byte[] byteArray = new byte[4 * data.Length];
		Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);

		MD5 md5Hash = MD5.Create();
		byte[] md5bt = md5Hash.ComputeHash(byteArray);
		//return System.Convert.ToBase64String(md5bt);
		return Base58Encoding.Encode(md5bt);
	}
	public static string byteToMd5(byte[] data ){
		MD5 md5Hash = MD5.Create();
		byte[] md5bt = md5Hash.ComputeHash(data);
		//return System.Convert.ToBase64String(md5bt);
		return Base58Encoding.Encode(md5bt);
	}
	//数组合并
	public static byte[] combine(byte[][] data){
		byte[] result = data[0];
		for(var i = 0;i < data.Length; i++){
			if(i != 0){
				result = combine (result, data[i]);
			}
		}
		return result;
	}
	//数组合并
	public static byte[] combine(byte[] defArr,byte[] md5Arr){
		//resArr为合并后数组
		byte[] resArr = new byte[ defArr.Length + md5Arr.Length];
		md5Arr.CopyTo(resArr,0);
		defArr.CopyTo(resArr, md5Arr.Length);
		return resArr;
	}

	public static byte[] getByte(int[] data){
		byte[] byteArray = new byte[4 * data.Length];
		Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
		return byteArray;
	}
	public static byte[]  getByte(float[] data){
		byte[] byteArray = new byte[4 * data.Length];
		Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
		return byteArray;
	}

	public static byte[]  getByte(Vector2[] data){
		var arr = new float[2 * data.Length];
		for (var i = 0; i < data.Length; ++i) {
			arr [2 * i + 0] = data [i].x;
			arr [2 * i + 1] = data [i].y;
		}
		byte[] byteArray = new byte[4 * arr.Length];
		Buffer.BlockCopy(arr, 0, byteArray, 0, byteArray.Length);
		return byteArray;
	}

	public static byte[]  getByte(Vector3[] data) {
		var arr = new float[3 * data.Length];
		for (var i = 0; i < data.Length; ++i) {
			arr [3 * i + 0] = data [i].x;
			arr [3 * i + 1] = data [i].y;
			arr [3 * i + 2] = data [i].z;
		}
		byte[] byteArray = new byte[4 * arr.Length];
		Buffer.BlockCopy(arr, 0, byteArray, 0, byteArray.Length);
		return byteArray;
	}

	public static byte[]  getByte(Vector4[] data) {
		var arr = new float[4 * data.Length];
		for (var i = 0; i < data.Length; ++i) {
			arr [4 * i + 0] = data [i].x;
			arr [4 * i + 1] = data [i].y;
			arr [4 * i + 2] = data [i].z;
			arr [4 * i + 3] = data [i].w;
		}
		byte[] byteArray = new byte[4 * arr.Length];
		Buffer.BlockCopy(arr, 0, byteArray, 0, byteArray.Length);
		return byteArray;
	}

	public static byte[]  getByte(Color[] data) {
		var arr = new float[4 * data.Length];
		for (var i = 0; i < data.Length; ++i) {
			arr [4 * i + 0] = data [i].r;
			arr [4 * i + 1] = data [i].g;
			arr [4 * i + 2] = data [i].b;
			arr [4 * i + 3] = data [i].a;
		}
		byte[] byteArray = new byte[4 * arr.Length];
		Buffer.BlockCopy(arr, 0, byteArray, 0, byteArray.Length);
		return byteArray;
	}
}
#endif