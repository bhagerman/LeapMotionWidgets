﻿/************************************************************************************

Filename    :   OVRScreenFade.cs
Content     :   An example of how to fade in a scene.
				In the future this feature could be added to OVRCamera
Created     :   June 27, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;					// required for Coroutines

public class OVRScreenFade : MonoBehaviour {

	public float			fadeTime = 2.0f;
	public Color			fadeColor = new Color( 0.01f, 0.01f, 0.01f, 1.0f );
	public Shader			fadeShader = null;

	private Material		fadeMaterial = null;
	private bool			isFading = false;

	/// <summary>
	/// Initialize.
	/// </summary>
	void Awake () {
		// create the fade material
		fadeMaterial = ( fadeShader != null ) ? new Material( fadeShader ) : new Material( Shader.Find ( "Transparent/Diffuse" ) );
	}

	/// <summary>
	/// Starts the fade in
	/// </summary>
	void OnEnable() {
		StartCoroutine( FadeIn() );

		// Add a listener to the OVRCamera for custom postrender work
		OVRCamera.OnCustomPostRender += OnCustomPostRender;
	}

	void OnDisable() {
		// Remove listener to the OVRCamera for custom postrender work
		OVRCamera.OnCustomPostRender -= OnCustomPostRender;
	}

	/// <summary>
	/// Starts a fade in when a new level is loaded
	/// </summary>
	void OnLevelWasLoaded( int level ) {
		StartCoroutine( FadeIn() );
	}

	/// <summary>
	/// Cleans up the fade material
	/// </summary>
	void OnDestroy() {
		if ( fadeMaterial != null ) {
			Destroy ( fadeMaterial );
		}
	}

	/// <summary>
	/// Fades alpha from 1.0 to 0.0
	/// </summary>
	IEnumerator FadeIn() {
		float elapsedTime = 0.0f;
		Color color = fadeMaterial.color = fadeColor;
		isFading = true;
		while ( elapsedTime < fadeTime ) {
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
			color.a = 1.0f - Mathf.Clamp01( elapsedTime / fadeTime );
			fadeMaterial.color = color;
		}
		isFading = false;
	}

	/// <summary>
	/// Renders the fade overlay when attached to a camera object
	/// </summary>
	void OnCustomPostRender() {
		if ( isFading ) {
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color( fadeMaterial.color );
			GL.Begin( GL.QUADS );
			GL.Vertex3( 0f, 0f, -12f );
			GL.Vertex3( 0f, 1f, -12f );
			GL.Vertex3( 1f, 1f, -12f );
			GL.Vertex3( 1f, 0f, -12f );
			GL.End();
			GL.PopMatrix();
		}
	}

}
