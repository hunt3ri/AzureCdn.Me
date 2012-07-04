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
		public static string CdnEndpoint { get; set; }

		public static int RandomNum { get; set; }

		public static string DebugMode { get; set; }

		static AzureCDNMeExtensions()
		{
			CdnEndpoint = ConfigurationManager.AppSettings["AzureCDNEndpoint"];
			DebugMode = ConfigurationManager.AppSettings["AzureCDNDebug"];
		}


		/// <summary>
		/// Creates a URL pointing to the appropriate Azure CDN, allows user to specify querystring value
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="contentLocation">The resource.</param>
        /// <param name="queryStringValue">The value you want to append to the querystring</param>
		/// <returns></returns>
		public static string AzureCdnContent(this UrlHelper helper, string contentLocation, string queryStringValue)
		{
			var queryString = "?cachebuster=" + queryStringValue;
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
		public static string AzureCdnContent(this UrlHelper helper, string contentLocation)
		{
			// Only get a new number the first time the app loads.
			if (RandomNum == 0)
				RandomNum = new Random().Next();

            var queryString = "?cachebuster=" + RandomNum;
			var newLocation = FormatContentLocation(contentLocation, queryString);

			return helper.Content(newLocation);
		}

		public static string FormatContentLocation(string contentLocation, string queryString)
		{
			var token = new char[] { '/' };

			var locationArray = contentLocation.Split(token);
			var locationArrayList = new ArrayList(locationArray);

			// Create CDN Address as array so we can insert it into url array at appropriate point
			var cdnAddressArray = new List<string> { CdnEndpoint };

			// Replace root element with http address for CDN
			if (CdnEndpoint.StartsWith("http") && ((locationArrayList[0].ToString() == "~") || (locationArrayList[0].ToString() == ".")))
			{
				locationArrayList[0] = CdnEndpoint;
			}
			else if (locationArrayList[0].ToString() == "~" || locationArrayList[0].ToString() == ".")
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
		/// Determines whether we're in debug mode, so we can load the appropriate stylesheets etc
		/// </summary>
		public static bool IsInDebugMode(this HtmlHelper helper)
		{
			return Convert.ToBoolean(DebugMode);

		}
	}
}