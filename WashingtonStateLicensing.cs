using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Collections.Specialized;

/**
 *
 *	Author:     Etor Madiv
 *  FreeLancer: https://www.freelancer.com/u/etormadiv.html
 *
 */

namespace WashingtonStateLicensingClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if(args.Length != 1)
			{
				Console.WriteLine("Error: Please specify License Number");
				return;
			}
			
			WashingtonStateLicensing licensing = new WashingtonStateLicensing();
			
			LicenseDetails details = licensing.GetInfo(args[0]);
			
			Console.WriteLine("License Details:");
			
			Console.WriteLine("Name: "            + details.Name);
			Console.WriteLine("LicenseType: "     + details.LicenseType);
			Console.WriteLine("LicenseNumber: "   + details.LicenseNumber);
			Console.WriteLine("LicenseStatus: "   + details.LicenseStatus);
			Console.WriteLine("FirstIssuedDate: " + details.FirstIssuedDate);
			Console.WriteLine("LicenseIssued: "   + details.LicenseIssued);
			Console.WriteLine("ExpirationDate: "  + details.ExpirationDate);
			Console.WriteLine("Address: "         + details.Address);
		}
	}
	
	public class WashingtonStateLicensing
	{
		/// <summary>
		/// The base Url that the requests will be based on.
		/// </summary>
		private const string baseUrl = "https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/";
		
		/// <summary>
		/// The user agent that will be used to perform the requests.
		/// </summary>
		private const string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
		
		/// <summary>
		/// A Container that will hold the cookies.
		/// </summary>
		private CookieContainer cookieContainer;
		
		/// <summary>
		/// Holds the value of the __VIEWSTATE input.
		/// </summary>
		private string __VIEWSTATE;
		
		/// <summary>
		/// Holds the value of the __VIEWSTATEGENERATOR input.
		/// </summary>
		private string __VIEWSTATEGENERATOR;
		
		/// <summary>
		/// Holds the value of the __EVENTVALIDATION input.
		/// </summary>
		private string __EVENTVALIDATION;
		
		/// <summary>
		/// The default constructor.
		/// </summary>
		public WashingtonStateLicensing()
		{
			Initialize();
		}
		
		/// <summary>
		/// Initialize our object to the required values.
		/// It is not intended to be called from your code directly.
		/// </summary>
		private void Initialize()
		{
			var hwr = (HttpWebRequest) WebRequest.Create(baseUrl);
			hwr.UserAgent = userAgent;
			
			cookieContainer = new CookieContainer();
			hwr.CookieContainer = cookieContainer;
			
			string response = "";
			
			using( var hwResponse = hwr.GetResponse() )
			{
				using( var stream = hwResponse.GetResponseStream() )
				{
					using( var reader = new StreamReader(stream) )
					{
						response = reader.ReadToEnd();
					}
				}
			}
			
			GetCurrentState(response);
		}
		
		/// <summary>
		/// Allows to update the values VIEWSTATE, VIEWSTATEGENERATOR and EVENTVALIDATION that is used by ASP.NET.
		/// </summary>
		/// <param name="response"> A raw HTML response. </param>
		private void GetCurrentState(string response)
		{
			int index = response.IndexOf("id=\"__VIEWSTATE\" value=") + 24;
			int length = response.IndexOf("\"", index) - index;
			
			__VIEWSTATE = response.Substring(index, length);
			
			index = response.IndexOf("id=\"__VIEWSTATEGENERATOR\" value=", index + length) + 33;
			length = response.IndexOf("\"", index) - index;
			
			__VIEWSTATEGENERATOR = response.Substring(index, length);
			
			index = response.IndexOf("id=\"__EVENTVALIDATION\" value=", index + length) + 30;
			length = response.IndexOf("\"", index) - index;
			
			__EVENTVALIDATION = response.Substring(index, length);
		}
		
		/// <summary>
		/// Allows to simulate the Search button click.
		/// </summary>
		/// <param name="licenseNumber"> The License Number that we want to extract its details. </param>
		private void SimulateSearchClick(string licenseNumber)
		{
			var hwr = (HttpWebRequest) WebRequest.Create( baseUrl + "default.aspx");
			hwr.Method      = "POST";
			hwr.ContentType = "application/x-www-form-urlencoded";
			hwr.UserAgent   = userAgent;
			hwr.Referer     = baseUrl;
			
			hwr.CookieContainer = cookieContainer;
			
			NameValueCollection urlencodedValues = HttpUtility.ParseQueryString(String.Empty);
			
			urlencodedValues.Add("__VIEWSTATE"         , __VIEWSTATE);
			urlencodedValues.Add("__VIEWSTATEGENERATOR", __VIEWSTATEGENERATOR);
			urlencodedValues.Add("__EVENTVALIDATION"   , __EVENTVALIDATION);
			urlencodedValues.Add("ddLicType"           , "299BAI");
			urlencodedValues.Add("txtbLicUBI"          , licenseNumber);
			urlencodedValues.Add("ddCounty"            , "0");
			urlencodedValues.Add("btnSearch"           , "Search");
			
			byte[] data = Encoding.ASCII.GetBytes(
				urlencodedValues.ToString()
			);
			
			using( var stream = hwr.GetRequestStream() )
			{
				stream.Write(data, 0, data.Length);
			}
			
			string response = "";
			
			using( var hwResponse = (HttpWebResponse) hwr.GetResponse() )
			{
				using( var stream = hwResponse.GetResponseStream() )
				{
					using( var reader = new StreamReader(stream) )
					{
						response = reader.ReadToEnd();
					}
				}
			}
			
			GetCurrentState(response);
		}
		
		/// <summary>
		/// Allows to simulate the upper continue button click.
		/// </summary>
		/// <returns> Returns the Reference Id. </returns>
		private string SimulateUpperContinue()
		{
			var hwr = (HttpWebRequest) WebRequest.Create( baseUrl + "lqsNarrow.aspx?NSL=299BAI&Narrow=1");
			hwr.Method      = "POST";
			hwr.ContentType = "application/x-www-form-urlencoded";
			hwr.UserAgent   = userAgent;
			hwr.Referer     = baseUrl + "lqsNarrow.aspx?NSL=299BAI&Narrow=1";
			
			hwr.CookieContainer = cookieContainer;
			
			NameValueCollection urlencodedValues = HttpUtility.ParseQueryString(String.Empty);
			
			urlencodedValues.Add("__VIEWSTATE"         , __VIEWSTATE);
			urlencodedValues.Add("__VIEWSTATEGENERATOR", __VIEWSTATEGENERATOR);
			urlencodedValues.Add("__EVENTVALIDATION"   , __EVENTVALIDATION);
			urlencodedValues.Add("299BAI_A1AREALEVEL"  , "on");
			urlencodedValues.Add("btnCont1"            , "Continue");
			
			byte[] data = Encoding.ASCII.GetBytes(
				urlencodedValues.ToString()
			);
			
			using( var stream = hwr.GetRequestStream() )
			{
				stream.Write(data, 0, data.Length);
			}
			
			string response = "";
			
			using( var hwResponse = (HttpWebResponse) hwr.GetResponse() )
			{
				using( var stream = hwResponse.GetResponseStream() )
				{
					using( var reader = new StreamReader(stream) )
					{
						response = reader.ReadToEnd();
					}
				}
			}
			
			if(response.Contains("No matches were found for your search."))
			{
				throw new Exception("No matches were found for your search.");
			}
			
			int index = response.IndexOf("lqsLicenseDetail.aspx?RefID") + 28;
			int length = response.IndexOf("\"", index) - index;
			
			return response.Substring(index, length);
		}
		
		/// <summary>
		/// Allows to get the details using the Reference ID.
		/// </summary>
		/// <param name="refId"> The refernce Id that will be used to get the License Details. </param>
		/// <returns> Returns a LicenseDetails object containing the extracted License Details. </returns>
		private LicenseDetails GetInfoByReferenceId(string refId)
		{
			var hwr = (HttpWebRequest) WebRequest.Create( baseUrl + "lqsLicenseDetail.aspx?RefID=" + refId);
			hwr.UserAgent   = userAgent;
			hwr.Referer     = baseUrl + "lqsSearchResults.aspx";
			
			hwr.CookieContainer = cookieContainer;
			
			string response = "";
			
			using( var hwResponse = (HttpWebResponse) hwr.GetResponse() )
			{
				using( var stream = hwResponse.GetResponseStream() )
				{
					using( var reader = new StreamReader(stream) )
					{
						response = reader.ReadToEnd();
					}
				}
			}
			
			LicenseDetails details = new LicenseDetails();
			
			int index = 0;
			
			details.Name = GetText("Name:", response, ref index);
			
			details.LicenseType = GetText("License Type:", response, ref index);
			
			details.LicenseNumber = GetText("License Number:", response, ref index);
			
			details.LicenseStatus = GetText("License Status:", response, ref index);
			
			details.FirstIssuedDate = GetText("First Issued Date:", response, ref index);
			
			details.LicenseIssued = GetText("License Issued:", response, ref index);
			
			details.ExpirationDate = GetText("Expiration Date:", response, ref index);
			
			details.Address = GetText("Address:", response, ref index, true);
			
			return details;
		}
		
		/// <summary>
		/// Allows to get desired information from the HTML content.
		/// </summary>
		/// <param name="desiredInfo"> A string describing the information to extract. </param>
		/// <param name="response"> A raw HTML response. </param>
		/// <param name="index"> An index that is used to keep the operation of extraction fine. </param>
		/// <param name="isAddress"> Specify if we want to extract the address. </param>
		/// <returns> Returns the desired data. </returns>
		private string GetText(string desiredInfo, string response, ref int index, bool isAddress = false)
		{
			index = response.IndexOf(desiredInfo, index);
			
			index = response.IndexOf(">", index) + 1;
			index = response.IndexOf(">", index) + 1;
			
			if(isAddress)
			{
				index = response.IndexOf(">", index) + 1;
				index = response.IndexOf(">", index) + 1;
			}
			
			int length = isAddress ? (response.IndexOf("</td>", index) - index) : (response.IndexOf("<", index) - index);
			
			return isAddress ? response.Substring(index, length).Replace("<BR>", "\n") : response.Substring(index, length);
		}
		
		/// <summary>
		/// Allows to get the License Details for a specific License Number.
		/// </summary>
		/// <param name="licenseNumber"> The License Number that we want to extract its details. </param>
		/// <returns> Returns a LicenseDetails object that contains the desired License Details. </returns>
		public LicenseDetails GetInfo(string licenseNumber)
		{
			SimulateSearchClick(licenseNumber);
			string refId = SimulateUpperContinue();
			return GetInfoByReferenceId(refId);
		}
	}
	
	public class LicenseDetails
	{
		public string Name;
		public string LicenseType;
		public string LicenseNumber;
		public string LicenseStatus;
		public string FirstIssuedDate;
		public string LicenseIssued;
		public string ExpirationDate;
		public string Address;
	}
}