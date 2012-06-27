using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;

namespace AzureCdn.Me.Code.Extensions
{
	public static class AzureCDNMeExtensions
	{
		// Expose as public property to ease unit testing without going to trouble of DI
		public static string CdnAddress { get; set; }

		public static int RandomNum { get; set; }

		static AzureCDNMeExtensions()
		{
			CdnAddress = ConfigurationManager.AppSettings["AzureCDNEndpoint"];
		}


		/// <summary>
		/// Creates a URL pointing to the appropriate Azure CDN, includes a version number on query string to invalidate cache.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="contentLocation">The resource.</param>
		/// <returns></returns>
		public static string AzureCdnContent(this UrlHelper helper, string contentLocation)
		{
			var queryString = "?version=" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			var newLocation = FormatContentLocation(contentLocation, queryString);

			return helper.Content(newLocation);
		}

		/// <summary>
		/// Creates a URL pointing to the appropriate Azure CDN, includes a random number on querystring to invalidate cache
		/// This may reset randomly (ironically) if server resets etc, so you should ideally use the version number approach
		/// if you can
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="contentLocation"></param>
		/// <param name="randomQueryString"></param>
		/// <returns></returns>
		public static string AzureCdnContent(this UrlHelper helper, string contentLocation, bool randomQueryString)
		{
			// Only get a new number the first time the app loads.
			if (RandomNum == 0)
				RandomNum = new Random().Next();

			var queryString = "?version=" + RandomNum.ToString();
			var newLocation = FormatContentLocation(contentLocation, queryString);

			return helper.Content(newLocation);
		}

		public static string FormatContentLocation(string contentLocation, string queryString)
		{
			var token = new char[] { '/' };

			var locationArray = contentLocation.Split(token);
			var locationArrayList = new ArrayList(locationArray);

			// Create CDN Address as array so we can insert it into url array at appropriate point
			var cdnAddressArray = new List<string> { CdnAddress };

			// Replace root element with http address for CDN
			if (CdnAddress.StartsWith("http") && (locationArrayList[0].ToString() == "~"))
			{
				locationArrayList[0] = CdnAddress;
			}
			else if (locationArrayList[0].ToString() == "~")
			{

				locationArrayList.InsertRange(1, cdnAddressArray);
			}
			else
			{
				locationArrayList.InsertRange(0, cdnAddressArray);
			}

			var newUrl = String.Join("/", locationArrayList.ToArray());
			newUrl = newUrl + queryString;

			return newUrl;
		}


		/// <summary>
		/// Renders the version number.
		/// </summary>
		/// <returns></returns>
		public static MvcHtmlString RenderVersionNumber(this HtmlHelper helper)
		{
			return new MvcHtmlString(Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}

		public static bool IsInDebugMode(this HtmlHelper helper)
		{
#if DEBUG
			return true;
#else
            return false;
#endif
		}
	}
}