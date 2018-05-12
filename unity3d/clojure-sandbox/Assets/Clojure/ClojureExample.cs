using System;
using System.IO;
using System.Collections.Concurrent;
using UnityEngine;
using clojure.lang;
using clojure.clr.api;

namespace MarbleWorks {
	public class ClojureExample : MonoBehaviour {

		public Transform cubePrefab;

		private ConcurrentQueue<IFn> _cmdQueue;

		void Start () {
			Environment.SetEnvironmentVariable ("UNITY_CLJ_LOAD_PATH", Path.Combine (Directory.GetCurrentDirectory (), "Assets", "Clojure", "clojure"));
			Environment.SetEnvironmentVariable ("clojure.server.UnitySocketServer", "{:port 5555 :accept clojure.core.server/repl}");
			Debug.Log(Environment.GetEnvironmentVariable("UNITY_CLJ_LOAD_PATH"));

	        IFn REQUIRE = Clojure.var("clojure.core", "require");
			Symbol mwCore = Symbol.create ("marble-works.core");
			REQUIRE.invoke (mwCore);

			var prefabMap = PersistentHashMap.create ("cube", cubePrefab);
			IFn mwInit = Clojure.var ("marble-works.core", "init");
			_cmdQueue = new ConcurrentQueue<IFn> ();
			mwInit.invoke (prefabMap, _cmdQueue);

			Debug.Log ("Clojure socket REPL initialized!");
		}

		void Update() {
			IFn fn;
			while (_cmdQueue.TryDequeue (out fn)) {
				fn.invoke ();
			}
		}
	}
}