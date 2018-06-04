using System;
using System.Globalization;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using XamarinShared;

namespace iOS
{
	public partial class ViewController : UIViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		partial void myButtonAction(UIButton sender)
		{
    		string localNodeServerURL = "http://127.0.0.1:3000/";
			var versionsData = new System.Net.WebClient().DownloadString(localNodeServerURL);
 		   	if (!string.IsNullOrEmpty(versionsData))
    		{
        		myTextView.Text = versionsData;
    		}
		}
	}
}
