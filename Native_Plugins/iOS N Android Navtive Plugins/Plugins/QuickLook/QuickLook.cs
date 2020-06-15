using UnityEngine;
using System.IO;
using System.Collections.Generic;

#pragma warning disable 0414
public class QuickLook
{
#if !UNITY_EDITOR && UNITY_IOS
	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void _QuickLook( string file );
#endif

	private string file;

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

	public void StartQuickLook()
	{
		if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file))
		{
			Debug.LogWarning( "QuickLook Error: attempting to show nothing!" );
			return;
		}

        if (!File.Exists(file))
        {
			Debug.Log(" **** file not present  !!!!  ");
			return;
		}

#if UNITY_EDITOR
		Debug.Log( "Quicklook started !!!!" );
#elif UNITY_IOS
		_QuickLook( file);
#else
		Debug.Log( "No quicklook set up for this platform." );
#endif
	}

	#region Utility Functions
	
	#endregion
}
#pragma warning restore 0414
