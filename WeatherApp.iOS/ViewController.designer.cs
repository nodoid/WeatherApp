// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WeatherApp.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton btnGetWeather { get; set; }

		[Outlet]
		UIKit.UIButton btnOK { get; set; }

		[Outlet]
		UIKit.UIButton btnReset { get; set; }

		[Outlet]
		UIKit.UIButton btnResetLocation { get; set; }

		[Outlet]
		UIKit.UILabel lblDegrees { get; set; }

		[Outlet]
		UIKit.UILabel lblGeolocation { get; set; }

		[Outlet]
		UIKit.UILabel lblHumid { get; set; }

		[Outlet]
		UIKit.UILabel lblPlace { get; set; }

		[Outlet]
		UIKit.UILabel lblPressure { get; set; }

		[Outlet]
		UIKit.UILabel lblSky { get; set; }

		[Outlet]
		UIKit.UILabel lblSpeed { get; set; }

		[Outlet]
		UIKit.UILabel lblSunset { get; set; }

		[Outlet]
		UIKit.UILabel lblSunUp { get; set; }

		[Outlet]
		UIKit.UILabel lblTemp { get; set; }

		[Outlet]
		UIKit.UILabel lblTempMax { get; set; }

		[Outlet]
		UIKit.UILabel lblTempMin { get; set; }

		[Outlet]
		UIKit.UILabel lblWind { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView spinProgress { get; set; }

		[Outlet]
		UIKit.UISwitch switchSaveLocation { get; set; }

		[Outlet]
		UIKit.UISwitch switchUseLocation { get; set; }

		[Outlet]
		UIKit.UITextField txtCity { get; set; }

		[Outlet]
		UIKit.UITextField txtCountry { get; set; }

		[Outlet]
		UIKit.UITextField txtState { get; set; }

		[Outlet]
		UIKit.UIView viewResults { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnGetWeather != null) {
				btnGetWeather.Dispose ();
				btnGetWeather = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}

			if (btnResetLocation != null) {
				btnResetLocation.Dispose ();
				btnResetLocation = null;
			}

			if (lblDegrees != null) {
				lblDegrees.Dispose ();
				lblDegrees = null;
			}

			if (lblGeolocation != null) {
				lblGeolocation.Dispose ();
				lblGeolocation = null;
			}

			if (lblHumid != null) {
				lblHumid.Dispose ();
				lblHumid = null;
			}

			if (lblPlace != null) {
				lblPlace.Dispose ();
				lblPlace = null;
			}

			if (lblPressure != null) {
				lblPressure.Dispose ();
				lblPressure = null;
			}

			if (lblSky != null) {
				lblSky.Dispose ();
				lblSky = null;
			}

			if (lblSpeed != null) {
				lblSpeed.Dispose ();
				lblSpeed = null;
			}

			if (lblSunset != null) {
				lblSunset.Dispose ();
				lblSunset = null;
			}

			if (lblSunUp != null) {
				lblSunUp.Dispose ();
				lblSunUp = null;
			}

			if (lblTemp != null) {
				lblTemp.Dispose ();
				lblTemp = null;
			}

			if (lblTempMax != null) {
				lblTempMax.Dispose ();
				lblTempMax = null;
			}

			if (lblTempMin != null) {
				lblTempMin.Dispose ();
				lblTempMin = null;
			}

			if (lblWind != null) {
				lblWind.Dispose ();
				lblWind = null;
			}

			if (spinProgress != null) {
				spinProgress.Dispose ();
				spinProgress = null;
			}

			if (switchSaveLocation != null) {
				switchSaveLocation.Dispose ();
				switchSaveLocation = null;
			}

			if (switchUseLocation != null) {
				switchUseLocation.Dispose ();
				switchUseLocation = null;
			}

			if (txtCity != null) {
				txtCity.Dispose ();
				txtCity = null;
			}

			if (txtCountry != null) {
				txtCountry.Dispose ();
				txtCountry = null;
			}

			if (txtState != null) {
				txtState.Dispose ();
				txtState = null;
			}

			if (viewResults != null) {
				viewResults.Dispose ();
				viewResults = null;
			}
		}
	}
}
