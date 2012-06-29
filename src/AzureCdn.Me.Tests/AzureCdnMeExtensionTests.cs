using System.Web.Mvc;
using AzureCdn.Me.Code.Extensions;
using Moq;
using NUnit.Framework;

namespace AzureCdn.Me.Tests
{
	[TestFixture]
	public class AzureCdnMeExtensionTests
	{
		[Test]
		public void If_CDN_EndPoint_Does_Not_Start_Http_Then_Insert_Folder_Appropriately()
		{
			// Arrange
			AzureCDNMeExtensions.CdnEndpoint = "CDN";

			// Act
			var cdnUrl = AzureCDNMeExtensions.FormatContentLocation("~/Content/bootstrap.css", "?version=1.0.0.0");

			//Assert
			Assert.That(cdnUrl, Is.EqualTo("~/CDN/Content/bootstrap.css?version=1.0.0.0"));
		}

		[Test]
		public void If_CDN_EndPoint_Starts_With_Http_Then_Insert_Folder_Appropriately()
		{
			// Arrange
			AzureCDNMeExtensions.CdnEndpoint = "http://az12345.vo.msecnd.net";

			// Act
			var cdnUrl = AzureCDNMeExtensions.FormatContentLocation("~/Content/bootstrap.css", "?version=1.0.0.0");

			//Assert
			Assert.That(cdnUrl, Is.EqualTo("http://az12345.vo.msecnd.net/Content/bootstrap.css?version=1.0.0.0"));
		}

		[Test]
		public void If_CDN_EndPoint_Starts_With_Https_Then_Insert_Folder_Appropriately()
		{
			// Arrange
			AzureCDNMeExtensions.CdnEndpoint = "https://az12345.vo.msecnd.net";

			// Act
			var cdnUrl = AzureCDNMeExtensions.FormatContentLocation("~/Content/bootstrap.css", "?version=1.0.0.0");

			//Assert
			Assert.That(cdnUrl, Is.EqualTo("https://az12345.vo.msecnd.net/Content/bootstrap.css?version=1.0.0.0"));
		}

		[Test]
		public void If_Address_Not_Specified_With_Tilda_Insert_CDN_Endpoint_Correctly()
		{
			// Arrange
			AzureCDNMeExtensions.CdnEndpoint = "https://az12345.vo.msecnd.net";

			// Act
			var cdnUrl = AzureCDNMeExtensions.FormatContentLocation("Content/bootstrap.css", "?version=1.0.0.0");

			//Assert
			Assert.That(cdnUrl, Is.EqualTo("https://az12345.vo.msecnd.net/Content/bootstrap.css?version=1.0.0.0"));
		}

		[Test]
		public void If_Address_Specified_With_DotSlash_Insert_CDN_Endpoint_Correctly()
		{
			// Arrange
			AzureCDNMeExtensions.CdnEndpoint = "https://az12345.vo.msecnd.net";

			// Act
			var cdnUrl = AzureCDNMeExtensions.FormatContentLocation("./Content/bootstrap.css", "?version=1.0.0.0");

			//Assert
			Assert.That(cdnUrl, Is.EqualTo("https://az12345.vo.msecnd.net/Content/bootstrap.css?version=1.0.0.0"));
		}

		[Test]
		public void If_Address_Specified_With_DotSlash_Insert_CDN_Endpoint_Correctly2()
		{
			// Arrange
			AzureCDNMeExtensions.CdnEndpoint = "CDN";

			// Act
			var cdnUrl = AzureCDNMeExtensions.FormatContentLocation("./Content/bootstrap.css", "?version=1.0.0.0");

			//Assert
			Assert.That(cdnUrl, Is.EqualTo("./CDN/Content/bootstrap.css?version=1.0.0.0"));
		}

		[Test]
		public void Test_IsInDebugMode_Handles_String_Conversion()
		{

			// Arrange
			AzureCDNMeExtensions.DebugMode = "true";

			// Setup mock html helper
			var mockViewContext = new Mock<ViewContext>();
			var mockViewDataContainer = new Mock<IViewDataContainer>();
			var mockHtmlHelper = new HtmlHelper(mockViewContext.Object, mockViewDataContainer.Object);

			// Act
			var result = AzureCDNMeExtensions.IsInDebugMode(mockHtmlHelper);

			// Assert
			Assert.That(result, Is.EqualTo(true));
		}
	}
}
