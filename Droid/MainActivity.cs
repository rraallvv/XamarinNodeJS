using System.Globalization;
using Android.App;
using Android.Widget;
using Android.OS;
using XamarinShared;
using System;
using Android.Content;
using Java.IO;
using XamarinShared;
using Android.Views;

namespace Droid
{
	[Activity(Label = "Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		public static bool _startedNodeAlready = false;
		public static TextView _textViewVersions;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			if (!_startedNodeAlready)
			{
				_startedNodeAlready = true;
				new Java.Lang.Thread(new MyRunnable(this)).Start();
			}

			var buttonVersions = (Button)FindViewById(Resource.Id.btVersions);
			_textViewVersions = (TextView)FindViewById(Resource.Id.tvVersions);

			buttonVersions.SetOnClickListener(new MyClickListener());
		}
	}

	class MyClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener
	{
		public void OnClick(View v)
		{
			//Network operations should be done in the background.
			new MyAsyncTask(MainActivity._textViewVersions).Execute();
		}
	}

	public class MyAsyncTask : Android.OS.AsyncTask
	{
		TextView textViewVersions;

		public MyAsyncTask(TextView textView)
		{
			textViewVersions = textView;
		}

		protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
		{
			string nodeResponse = "";
			try
			{
				var localNodeServer = new Java.Net.URL("http://localhost:3000/");
				var input = new BufferedReader(
					new InputStreamReader(localNodeServer.OpenStream()));
				String inputLine;
				while ((inputLine = input.ReadLine()) != null)
					nodeResponse = nodeResponse + inputLine;
				input.Close();
			}
			catch (Exception ex)
			{
				nodeResponse = ex.ToString();
			}
			return nodeResponse;
		}

		protected override void OnPostExecute(Java.Lang.Object result)
		{
			textViewVersions.Text = result.ToString();
		}
	}

	class MyRunnable : Java.Lang.Object, Java.Lang.IRunnable
	{
		Context context;

		public MyRunnable(Context context)
		{
			this.context = context;
		}

		public void Run()
		{
			//The path where we expect the node project to be at runtime.
			var nodeDir = context.FilesDir.AbsolutePath + "/nodejs-project";
			if (WasAPKUpdated())
			{
				//Recursively delete any existing nodejs-project.
				var nodeDirReference = new Java.IO.File(nodeDir);
				if (nodeDirReference.Exists())
				{
					DeleteFolderRecursively(new File(nodeDir));
				}
				//Copy the node project from assets into the application's data path.
				copyAssetFolder(context.ApplicationContext.Assets, "nodejs-project", nodeDir);

				saveLastUpdateTime();
			}

			CLib.StartNodeWithArguments(2, new String[] { "node", nodeDir + "/main.js" });
		}

		private bool WasAPKUpdated()
		{
			var prefs = context.ApplicationContext.GetSharedPreferences("NODEJS_MOBILE_PREFS", FileCreationMode.Private);
			long previousLastUpdateTime = prefs.GetLong("NODEJS_MOBILE_APK_LastUpdateTime", 0);
			long lastUpdateTime = 1;
			try
			{
				var packageInfo = context.ApplicationContext.PackageManager.GetPackageInfo(context.ApplicationContext.PackageName, 0);
				lastUpdateTime = packageInfo.LastUpdateTime;
			}
			catch (Android.Content.PM.PackageManager.NameNotFoundException e)
			{
				e.PrintStackTrace();
			}
			return (lastUpdateTime != previousLastUpdateTime);
		}

		private void saveLastUpdateTime()
		{
			long lastUpdateTime = 1;
			try
			{
				var packageInfo = context.ApplicationContext.PackageManager.GetPackageInfo(context.ApplicationContext.PackageName, 0);
				lastUpdateTime = packageInfo.LastUpdateTime;
			}
			catch (Android.Content.PM.PackageManager.NameNotFoundException e)
			{
				e.PrintStackTrace();
			}
			var prefs = context.ApplicationContext.GetSharedPreferences("NODEJS_MOBILE_PREFS", FileCreationMode.Private);
			var editor = prefs.Edit();
			editor.PutLong("NODEJS_MOBILE_APK_LastUpdateTime", lastUpdateTime);
			editor.Commit();
		}

		private static bool DeleteFolderRecursively(Java.IO.File file)
		{
			try
			{
				bool res = true;
				foreach (Java.IO.File childFile in file.ListFiles())
				{
					if (childFile.IsDirectory)
					{
						res &= DeleteFolderRecursively(childFile);
					}
					else
					{
						res &= childFile.Delete();
					}
				}
				res &= file.Delete();
				return res;
			}
			catch (Java.Lang.Exception e)
			{
				e.PrintStackTrace();
				return false;
			}
		}

		private static bool copyAssetFolder(Android.Content.Res.AssetManager assetManager, String fromAssetPath, String toPath)
		{
			try
			{
				String[] files = assetManager.List(fromAssetPath);
				bool res = true;

				if (files.Length == 0)
				{
					//If it's a file, it won't have any assets "inside" it.
					res &= copyAsset(assetManager,
							fromAssetPath,
							toPath);
				}
				else
				{
					new File(toPath).Mkdirs();
					foreach (string file in files)
						res &= copyAssetFolder(assetManager,
								fromAssetPath + "/" + file,
								toPath + "/" + file);
				}
				return res;
			}
			catch (Java.Lang.Exception e)
			{
				e.PrintStackTrace();
				return false;
			}
		}

		private static bool copyAsset(Android.Content.Res.AssetManager assetManager, String fromAssetPath, String toPath)
		{
			try
			{
				using (var input = assetManager.Open(fromAssetPath))
				using (var output = new System.IO.FileStream(toPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					input.CopyTo(output);
				}
				return true;
			}
			catch (Java.Lang.Exception e)
			{
				e.PrintStackTrace();
				return false;
			}
		}
	}
}
