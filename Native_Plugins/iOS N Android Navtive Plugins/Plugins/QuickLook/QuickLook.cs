using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

#pragma warning disable 0414
public class QuickLook
{
	public delegate void voidCallback();
	private static Action QUICKLOOKDISMISSCALLBACK;

#if !UNITY_EDITOR && UNITY_IOS
	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void _QuickLook( string file , voidCallback quickLookDismissCallBack);
#endif


	private string file;
	private static bool isProcessingQuickLook;

	public QuickLook()
	{
		file = null;
	}

	public QuickLook AddFile( string filePath)
	{
		if( !string.IsNullOrEmpty( filePath ) && File.Exists( filePath ) )
		{
			file = filePath;
		}
		else
			Debug.LogError( "File does not exist at path or permission denied: " + filePath );

		return this;
	}

	public void StartQuickLook(Action quickLookDismissCallBack, Action failToOpenQuickLook)
	{
        if (isProcessingQuickLook)
        {
            if (failToOpenQuickLook != null)
            {
				failToOpenQuickLook();
			}
			Debug.Log("Aborting the call cozn already running Quicklook !!!!");
			return;
        }

		if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file))
		{
			if (failToOpenQuickLook != null)
			{
				failToOpenQuickLook();
			}
			Debug.LogWarning( "QuickLook Error: attempting to show nothing!" );
			return;
		}

        if (!File.Exists(file))
        {
			if (failToOpenQuickLook != null)
			{
				failToOpenQuickLook();
			}
			Debug.Log(" **** file not present  !!!!  ");
			return;
		}

		isProcessingQuickLook = true;

		QUICKLOOKDISMISSCALLBACK = quickLookDismissCallBack;

#if UNITY_EDITOR
		Debug.Log( "Quicklook started !!!!" );
		QuickLookDismissCallBack();
#elif UNITY_IOS
		_QuickLook( file, QuickLookDismissCallBack);
#else
		Debug.Log( "No quicklook set up for this platform." );
#endif
	}

	#region Utility Functions

	[AOT.MonoPInvokeCallback(typeof (voidCallback))]
	public static void QuickLookDismissCallBack()
    {
		Debug.Log("@@@@ QuickLook dismissed From unitySS!!!!");
		if(QUICKLOOKDISMISSCALLBACK!= null)
        {
			QUICKLOOKDISMISSCALLBACK();
		}
		QUICKLOOKDISMISSCALLBACK = null;
		isProcessingQuickLook = false;

	}

	#endregion
}
#pragma warning restore 0414
