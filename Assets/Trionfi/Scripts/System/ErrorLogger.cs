using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
	public class ErrorLogger {
		private static List<string> errorMessage = new List<string>();
		private static List<string> warningMessage = new List<string>();

		public static void Clear() {
			errorMessage.Clear();
			warningMessage.Clear();
		}

		public static void addLog(string message, string file, int line, bool stop) {
			if (stop) {
				string str = "<color=green>Novel</color>[" + file + "]:<color=red>Error:" + line + "行目 " + message + "</color>";
				errorMessage.Add(str);
			}
			else {
				string str = "<color=green>Novel</color>[" + file + "]:<color=yellow>Warning:" + line + "行目 " + message + "</color>";
				warningMessage.Add(str);
			}
		}

		public static bool showAll() {
			foreach (string message in errorMessage) {
				Debug.LogError(message);
			}
			foreach (string message in warningMessage) {
				Debug.LogWarning(message);
			}
			return errorMessage.Count > 0 ? true : false;
		}

		public static void stopError(string message) {
			Debug.LogError(message);
			throw new UnityException(message);		
		}
	};
}
