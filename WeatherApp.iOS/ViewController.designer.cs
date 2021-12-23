// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace WeatherApp.iOS
{
	[Register("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UIButton btnOK { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UIButton btnGetWeather { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UIButton btnResetData { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UIButton btnResetLoc { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UITextView txtCity { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UITextView txtState { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UITextView txtCountry { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UISwitch switchCurrentLoc { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UISwitch switchSave { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UIActivityIndicatorView spinProgress { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UIView viewResults { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblSky { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblTemp { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblTempMax { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblTempMin { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblPressure { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblHumid { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblSunrise { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblSunset { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblSpeed { get; set; }

		[Outlet]
		[GeneratedCode("iOS Designer", "1.0")]
		UILabel lblDegrees { get; set; }

		void ReleaseDesignerOutlets()
		{
			if (btnOK != null)
			{
				btnOK.Dispose();
				btnOK = null;
			}
			if (btnGetWeather != null)
			{
				btnGetWeather.Dispose();
				btnGetWeather = null;
			}
			if (btnResetData != null)
			{
				btnResetData.Dispose();
				btnResetData = null;
			}
			if (btnResetLoc != null)
			{
				btnResetLoc.Dispose();
				btnResetLoc = null;
			}
			if (txtCity != null)
			{
				txtCity.Dispose();
				txtCity = null;
			}
			if (txtCountry != null)
			{
				txtCountry.Dispose();
				txtCountry = null;
			}
			if (txtState != null)
			{
				txtState.Dispose();
				txtState = null;
			}
			if (switchCurrentLoc != null)
			{
				switchCurrentLoc.Dispose();
				switchCurrentLoc = null;
			}
			if (switchSave != null)
			{
				switchSave.Dispose();
				switchSave = null;
			}
			if (spinProgress != null)
			{
				spinProgress.Dispose();
				spinProgress = null;
			}
			if (viewResults != null)
			{
				viewResults.Dispose();
				viewResults = null;
			}

			if (lblSky != null)
			{
				lblSky.Dispose();
				lblSky = null;
			}
			if (lblTemp != null)
			{
				lblTemp.Dispose();
				lblTemp = null;
			}
			if (lblTempMin != null)
			{
				lblTempMin.Dispose();
				lblTempMin = null;
			}
			if (lblTempMax != null)
			{
				lblTempMax.Dispose();
				lblTempMax = null;
			}
			if (lblPressure != null)
			{
				lblPressure.Dispose();
				lblPressure = null;
			}
			if (lblHumid != null)
			{
				lblHumid.Dispose();
				lblHumid = null;
			}
			if (lblSunrise != null)
			{
				lblSunrise.Dispose();
				lblSunrise = null;
			}
			if (lblSunset != null)
			{
				lblSunset.Dispose();
				lblSunset = null;
			}
			if (lblDegrees != null)
			{
				lblDegrees.Dispose();
				lblDegrees = null;
			}
			if (lblSpeed != null)
			{
				lblSpeed.Dispose();
				lblSpeed = null;
			}
		}
	}
}