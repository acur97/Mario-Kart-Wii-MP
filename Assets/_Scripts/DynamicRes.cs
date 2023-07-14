using UnityEngine;
using Cleverous.Shared;

[RequireComponent(typeof(Camera))]
public class DynamicRes : MonoBehaviour
{
	public FpsToText fps;
	public Camera UIcam;

	[Space]
	public float steps = 0.1f;
	public float timings = 5;

	[Space]
	public float min = 0.5f;
	public float max = 1.2f;

	[Space]
	public int min2FPS = 25;
	public int minFPS = 55;
	public int maxFPS = 70;

	[Space]
	public bool subidaMitad = true;

	[Space]
	[Range(0.12f, 2)]
	public float myFactor = 1;

	private float cachedFactor;	
	private float factor;
	private float lastFactor;
	private int drHeight;
	private int drWith;
	private int irH;
	private int irW;
	private bool optionSet;
	private Camera drCam;
	private RenderTexture dynFlip;
	private RenderTexture dynSample;
	private RenderTextureFormat drTextureFormat;
	private readonly bool mitad = true;

	private void Start()
	{
		UIcam.forceIntoRenderTexture = false;
		UIcam.gameObject.SetActive(false);

		drCam = GetComponent<Camera>();

		if (factor <= 0.12f)
		{
			factor = 1.0f;
		}

		SetTFormat(RenderTextureFormat.DefaultHDR);

		irH = Screen.height;
		irW = Screen.width;

		optionSet = true;

		UIcam.gameObject.SetActive(true);
		InvokeRepeating(nameof(ResChange), 0, 1);
		InvokeRepeating(nameof(Change), 0.25f, timings);
	}

	private void ResChange()
    {
		if (Screen.width != irW || Screen.height != irH)
        {
			Instantiate(gameObject, transform.parent).name =gameObject.name;
			Destroy(gameObject);
        }
    }

    private void Change()
	{
		if (myFactor > min && fps.Framerate <= min2FPS)
		{
			myFactor -= steps * 2;
			subidaMitad = true;
			if (!cachedFactor.Equals(myFactor))
			{
				ChangeRes(myFactor);
			}
		}
		else if (myFactor > min && fps.Framerate <= minFPS)
		{
			myFactor -= steps;
			subidaMitad = true;
			if (!cachedFactor.Equals(myFactor))
			{
				ChangeRes(myFactor);
			}
		}
		//else if (myFactor < max && fps.Framerate >= maxFPS)
		else if (myFactor < max && fps.Framerate >= (QualitySettings.vSyncCount == 0 ? maxFPS : Screen.currentResolution.refreshRate - 1))
		{
			if (mitad)
			{
				if (subidaMitad)
				{
					subidaMitad = false;
					myFactor += steps;
					if (!cachedFactor.Equals(myFactor))
					{
						ChangeRes(myFactor);
					}
				}
				else
				{
					subidaMitad = true;
				}
			}
			else
			{
				myFactor += steps;
				if (!cachedFactor.Equals(myFactor))
				{
					ChangeRes(myFactor);
				}
			}
		}
	}

	private void ChangeRes(float factor)
	{
		myFactor = Mathf.Clamp(factor, min, max);
		SetFactor(myFactor);
		cachedFactor = myFactor;
		//Debug.Log("change res");
	}

	// Sample the buffer according to setup
	public void OnPreCull()
	{
		if (optionSet)
		{
			if (!factor.Equals(lastFactor))
			{
				lastFactor = Mathf.Max(factor, 0.01f);
				factor = lastFactor;
				drWith = Mathf.RoundToInt(irW * lastFactor);
				drHeight = Mathf.RoundToInt(irH * lastFactor);
			}

			dynSample = RenderTexture.GetTemporary(drWith, drHeight, 24, drTextureFormat, RenderTextureReadWrite.Default, 1);
			dynSample.wrapMode = TextureWrapMode.Clamp;

			//if (drCam != null)
			drCam.SetTargetBuffers(dynSample.colorBuffer, dynSample.depthBuffer);
		}
	}

	// Scale the buffer content back to screen size
	public void OnPostRender()
	{
		if (optionSet)
		{
			dynFlip = RenderTexture.GetTemporary(Screen.width, Screen.height);
			dynFlip.wrapMode = TextureWrapMode.Clamp;

			//if (drCam != null)
			drCam.SetTargetBuffers(dynFlip.colorBuffer, dynFlip.depthBuffer);
			RenderTexture.ReleaseTemporary(dynFlip);
			RenderTexture.ReleaseTemporary(dynSample);
		}
	}

	// We're using RenderTextures so we need to Blit;
	// this can be safely removed if image effects are used
	// because the need to Blit as well
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
	}

	// Takes:	float value that specifies the factor of the sampling;
	//			1.0 equals 100% and the value will be clamped between
	//			0.01 (1%) and 4.0 (400%)
	public void SetFactor(float sFactor)
	{
		factor = Mathf.Clamp(sFactor, 0.12f, 2.0f);
	}

	// Takes:	two int values specifying pixel with and height values
	//			to sample to; they will always be changed to meet the
	//			real screen width to height ratio
	public void SetRes(int sWidth, int sHeight)
	{
		SetRes(sWidth, sHeight, false);
	}

	// Takes:	two int values specifying pixel with and height values
	//			to sample to as well as a boolean value; use a boolean
	//			value of true to force exact sampling even if its width
	//			to height ratio differs from the screen width to height
	//			ratio; consider using the SetRes method above if your're
	//			not using a boolean value of true ;-)
	public void SetRes(int sWidth, int sHeight, bool forceRes)
	{
		if (forceRes)
		{
			irW = sWidth;
			irH = sHeight;
			drWith = Mathf.RoundToInt(irW * lastFactor);
			drHeight = Mathf.RoundToInt(irH * lastFactor);
		}
		else
		{
			float __wFactor = sWidth / (1.0f * Screen.width);
			float __hFactor = sHeight / (1.0f * Screen.height);

			if (__wFactor < __hFactor)
			{
				SetFactor(__wFactor);
			}
			else
			{
				SetFactor(__hFactor);
			}
		}
	}

	// Takes:	RenderTextureFormat to use internally; use
	//			Unity's documentation for a complete list
	//			of formats
	public void SetTFormat(RenderTextureFormat tFormat)
	{
		if (SystemInfo.SupportsRenderTextureFormat(tFormat))
		{
			drTextureFormat = tFormat;
		}
		else
		{
			drTextureFormat = RenderTextureFormat.Default;
		}
	}
}