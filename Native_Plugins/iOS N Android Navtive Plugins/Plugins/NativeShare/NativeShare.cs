using UnityEngine;
using System.IO;
using System.Collections.Generic;

public enum SharedActivityType
{
	OTHERS,
	FACEBOOK,
	TWITTER,
	INSTAGRAM,
	CANCEL
};

public class NativeShare
{
	private delegate void shareResultActivityCallBack(bool shareComplete, string shareActivity);

#if !UNITY_EDITOR && UNITY_IOS
	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void IOSNative_Share( string file, string caption, shareResultActivityCallBack shareActivityCallback);
#endif

	private string caption;

	private string file;

	public NativeShare()
	{
		caption = string.Empty;

		file = string.Empty;
	}

	public NativeShare SetCaption(string specialMsg = null , string hashtags = null, string urlLink = null)
	{
		if (caption != null)
        {
            if(!string.IsNullOrEmpty(specialMsg) && !string.IsNullOrWhiteSpace(specialMsg))
			    this.caption += specialMsg + "\n";
			if (!string.IsNullOrEmpty(hashtags) && !string.IsNullOrWhiteSpace(hashtags))
				this.caption += hashtags + "\n";
			if (!string.IsNullOrEmpty(urlLink) && !string.IsNullOrWhiteSpace(urlLink))
				this.caption += urlLink + "\n";
		}

		return this;
	}

	public NativeShare AddFile(string filePath)
	{
		if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
		{
			file = filePath;
		}
		else
			Debug.LogError("!!!! File does not exist at path or permission denied: " + filePath);

		return this;
	}

	static System.Action onShareCompleteAction;
	static System.Action onShareCancelAction;
	static System.Action<SharedActivityType> onShareActivityAction;

	static bool isSharing;

	[AOT.MonoPInvokeCallback(typeof(shareResultActivityCallBack))]
	public static void shareResultCallBack(bool shareComplete ,string shareActivity)
	{
		isSharing = false;

		Debug.Log("shareResultCallBack called with shareComplete : " + shareComplete + "\n shareActivity : " + shareActivity);

		SharedActivityType sharedActivityType = ReturnSharedActivity(shareActivity);

		if (shareComplete)
		{
			onShareCompleteAction?.Invoke();
		}
		else if (!shareComplete && sharedActivityType == SharedActivityType.CANCEL)
		{
			onShareCancelAction?.Invoke();
		}
		onShareActivityAction?.Invoke(sharedActivityType);
		
		
	}

    private static SharedActivityType ReturnSharedActivity(string activtyString)
    {
		SharedActivityType sharedActivityType = SharedActivityType.OTHERS;
#if UNITY_EDITOR
		sharedActivityType = SharedActivityType.OTHERS;
#elif UNITY_IOS
       if (string.IsNullOrEmpty(activtyString) || string.IsNullOrWhiteSpace(activtyString))
		{
			sharedActivityType = SharedActivityType.CANCEL;
		}
		else if (activtyString.Equals("com.apple.UIKit.activity.PostToFacebook"))
		{
			sharedActivityType = SharedActivityType.FACEBOOK;
		}
		else if (activtyString.Equals("com.apple.UIKit.activity.PostToTwitter"))
		{
			sharedActivityType = SharedActivityType.TWITTER;
		}
		else if (activtyString.Equals("com.burbn.instagram.shareextension"))
		{
			sharedActivityType = SharedActivityType.INSTAGRAM;
		}
		else
		{
			sharedActivityType = SharedActivityType.OTHERS;
		} 
#endif
		return sharedActivityType;
	}

	public void Share(System.Action shareComplete, System.Action shareCancel, System.Action<SharedActivityType> activitySharedWith)
	{
		Debug.Log("!!!! Share is called");
        if (isSharing)
        {
			Debug.LogError("!!!! Already sharing going on can not share now aborting !!!!");
			return;
        }
		if (file.Length == 0 && caption.Length == 0)
		{
			Debug.LogWarning("!!!! Share Error: attempting to share nothing!");
			return;
		}

		onShareCompleteAction = shareComplete;
		onShareCancelAction = shareCancel;
		onShareActivityAction = activitySharedWith;

		isSharing = true;

#if UNITY_EDITOR
		Debug.Log("Shared On Unity Editor !!!");
		shareResultCallBack(true,"others");
#elif UNITY_IOS
		IOSNative_Share( file, caption, shareResultCallBack);
#else
		Debug.Log( "No sharing set up for this platform." );
#endif
	}
}
