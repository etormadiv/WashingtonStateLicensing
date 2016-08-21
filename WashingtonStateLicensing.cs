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
			
			var details = licensing.GetInfo(args[0]);
			
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
		
		private CookieContainer cookieContainer;
		
		private string __VIEWSTATE;
		
		private string __VIEWSTATEGENERATOR;
		
		private string __EVENTVALIDATION;
		
		public WashingtonStateLicensing()
		{
			Initialize();
		}
		
		private void Initialize()
		{
			var hwr = (HttpWebRequest) WebRequest.Create("https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/");
			hwr.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
			
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
			
			using( var writer = new StreamWriter("wa_licensing_1.htm") )
			{
				writer.WriteLine(response);
			}
			
			GetCurrentState(response);
		}
		
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
		
		private void SimulateSearchClick(string licenseNumber)
		{
			var hwr = (HttpWebRequest) WebRequest.Create("https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/default.aspx");
			hwr.Method      = "POST";
			hwr.ContentType = "application/x-www-form-urlencoded";
			hwr.UserAgent   = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
			hwr.Referer     = "https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/";
			hwr.Accept      = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			
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
			
			using( var writer = new StreamWriter("wa_licensing_2.htm") )
			{
				writer.WriteLine(response);
			}
			
			GetCurrentState(response);
		}
		
		private string SimulateUpperContinue()
		{
			var hwr = (HttpWebRequest) WebRequest.Create("https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/lqsNarrow.aspx?NSL=299BAI&Narrow=1");
			hwr.Method      = "POST";
			hwr.ContentType = "application/x-www-form-urlencoded";
			hwr.UserAgent   = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
			hwr.Referer     = "https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/lqsNarrow.aspx?NSL=299BAI&Narrow=1";
			hwr.Accept      = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			
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
			
			using( var writer = new StreamWriter("wa_licensing_3.htm") )
			{
				writer.WriteLine(response);
			}
			
			int index = response.IndexOf("lqsLicenseDetail.aspx?RefID") + 28;
			int length = response.IndexOf("\"", index) - index;
			
			return response.Substring(index, length);
		}
		
		private LicenseDetails GetInfoByReferenceId(string refId)
		{
			var hwr = (HttpWebRequest) WebRequest.Create("https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/lqsLicenseDetail.aspx?RefID=" + refId);
			hwr.UserAgent   = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
			hwr.Referer     = "https://fortress.wa.gov/dol/dolprod/bpdLicenseQuery/lqsSearchResults.aspx";
			hwr.Accept      = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			
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
			
			using( var writer = new StreamWriter("wa_licensing_4.htm") )
			{
				writer.WriteLine(response);
			}
			
			LicenseDetails details = new LicenseDetails();
			
			int index = response.IndexOf("Name:");
			details.Name = GetText(response, ref index);
			
			index = response.IndexOf("License Type:", index);
			details.LicenseType = GetText(response, ref index);
			
			index = response.IndexOf("License Number:", index);
			details.LicenseNumber = GetText(response, ref index);
			
			index = response.IndexOf("License Status:", index);
			details.LicenseStatus = GetText(response, ref index);
			
			index = response.IndexOf("First Issued Date:", index);
			details.FirstIssuedDate = GetText(response, ref index);
			
			index = response.IndexOf("License Issued:", index);
			details.LicenseIssued = GetText(response, ref index);
			
			index = response.IndexOf("Expiration Date:", index);
			details.ExpirationDate = GetText(response, ref index);
			
			index = response.IndexOf("Address:", index);
			details.Address = GetText(response, ref index, true);
			
			return details;
		}
		
		private string GetText(string response, ref int index, bool isAddress = false)
		{
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