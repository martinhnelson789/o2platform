using System;
using System.Collections;
using System.Text;

/*
 
 NUnit Tests that will test correctness of the parser: for this you will need
 to get .NET 2.0 build of NUnit from http://www.nunit.org/
 
 You might need to update reference nunit.framework - even though it is copied locally and in theory
 should be self sufficient
 
 Adding new test is simple: you just create a function that passes some HTML to parse either in string
 or binary form and also HTML that you expect that should be generated from parser as it sees it, see
 below for examples.
  
 */

using NUnit.Framework;

using Majestic12;

namespace NUnitTests
{
	[TestFixture]	
	public class HTMLparserTest
	{
		/// <summary>
		/// Parser object that we will use
		/// </summary>
		HTMLparser oP=null;

		public HTMLparserTest()
		{
			CreateParser();
		}

		[Test]
		public void CreateParser()
		{
			if(oP!=null)
			{
				oP.Close();
				oP=null;
			}

			oP=new HTMLparser();

			oP.bDecodeEntities=true;

			// dummy assertion 
			Assert.IsNotNull(oP);
		}

		/// <summary>
		/// Tests parser by parsing chunk of data and then generating HTML on the basis of parsing
		/// and comparing this to expected HTML: in case of any discrepancies assertion will be fired
		/// </summary>
		/// <param name="bData">Data to parse</param>
		/// <param name="sExpectedHTML">Expected HTML as it gets generated by this very function</param>
		void TestParser(byte[] bData,string sExpectedHTML)
		{
			if(sExpectedHTML==null)
				return;

			StringBuilder oSB=new StringBuilder(512);

			bool bEncodingSet=false;

			oP.Init(bData);

			// ok lets parse HTML and save the HTML that we view back into string
			HTMLchunk oChunk;

			// we don't want to use hashes as they would change order in which params are made
			oP.SetChunkHashMode(false);

			// we parse until returned oChunk is null indicating we reached end of parsing
			while((oChunk=oP.ParseNext())!=null)
			{
				switch(oChunk.oType)
				{
					case HTMLchunkType.OpenTag:

					oSB.AppendFormat("<{0}",oChunk.sTag);

				PrintParams:

						if(oChunk.sTag.Length==4 && oChunk.sTag=="meta")
						{
							if(!bEncodingSet)
							{
								if(HTMLparser.HandleMetaEncoding(oP,oChunk,ref bEncodingSet))
								{
									if(bEncodingSet)
									{
										// possible Title re-encoding should happen here
									}
								}
							}
						}

						// commented out call to code that will do the job for you - long code below
						// is left to demonstrate how to access individual param values
						// Console.WriteLine(oChunk.GenerateParamsHTML());
						

						if(oChunk.bHashMode)
						{
							if(oChunk.oParams.Count>0)
							{
								foreach(string sParam in oChunk.oParams.Keys)
								{
									string sValue=oChunk.oParams[sParam].ToString();

									if(sValue.Length>0)
										oSB.AppendFormat(" {0}='{1}'",sParam,oP.ChangeToEntities(sValue));
									else
										oSB.AppendFormat(" {0}",sParam);
								}

							}
						}
						else
						{
							// this is alternative method of getting params -- it may look less convinient
							// but it saves a LOT of CPU ticks while parsing. It makes sense when you only need
							// params for a few
							if(oChunk.iParams>0)
							{
								for(int i=0; i<oChunk.iParams; i++)
								{
									// here we can use exactly the same single/double quotes as they
									// were used on params

									string sValue=oChunk.sValues[i];

									if(oChunk.bEntities)
										sValue=oP.ChangeToEntities(sValue);

									switch(oChunk.cParamChars[i])
									{
										case (byte)' ':
											if(oChunk.sValues[i].Length==0)
												oSB.AppendFormat(" {0}",oChunk.sParams[i]);
											else
												oSB.AppendFormat(" {0}={1}",oChunk.sParams[i],sValue);
											break;

										default:
											oSB.AppendFormat(" {0}={1}{2}{1}",oChunk.sParams[i],(char)oChunk.cParamChars[i],sValue);
											break;
									}

								}

							}

						}

						if(oChunk.bClosure && !oP.bAutoMarkClosedTagsWithParamsAsOpen)
							oSB.Append("/>");
						else
							oSB.Append(">");
						break;

					// matched close tag, ie </a>
					case HTMLchunkType.CloseTag:

						if(oChunk.iParams>0)
						{
							oSB.AppendFormat("<{0}",oChunk.sTag);
							goto PrintParams;
						}
						else
						{
							if(oChunk.bEndClosure)
								oSB.AppendFormat("<{0}/>",oChunk.sTag);
							else
								oSB.AppendFormat("</{0}>",oChunk.sTag);
						}

						break;

					// NOTE: you have to call finalisation because it is not done for Scripts or comments
					// Matched data between <script></script> tags
					case HTMLchunkType.Script:

						if(!oP.bAutoKeepScripts && !oP.bKeepRawHTML)
							oP.SetRawHTML(oChunk);

						oSB.AppendFormat(oChunk.oHTML);

						if(oChunk.iParams>0)
							goto PrintParams;

						break;

					// NOTE: you have to call finalisation because it is not done for Scripts or comments
					// matched HTML comment, that's stuff between <!-- and -->
					case HTMLchunkType.Comment:
						if(!oP.bAutoExtractBetweenTagsOnly)
							oSB.AppendFormat("{0}",oChunk.oHTML);
						else
							oSB.AppendFormat("<!--{0}-->",oChunk.oHTML);
						break;

					// matched normal text
					case HTMLchunkType.Text:

						// skip pure whitespace that we are not really interested in
						if(oP.bCompressWhiteSpaceBeforeTag && oChunk.oHTML.Trim().Length==0)
							continue;

						oSB.AppendFormat("{0}",oChunk.bEntities ? oP.ChangeToEntities(oChunk.oHTML) : oChunk.oHTML);
						break;

				};

			}

			// now compare parsed HTML with the one we expect
			Assert.AreEqual(sExpectedHTML,oSB.ToString());
		}

		/// <summary>
		/// Tests parser by parsing chunk of data and then generating HTML on the basis of parsing
		/// and comparing this to expected HTML: in case of any discrepancies assertion will be fired: using
		/// custom encoding here
		/// </summary>
		/// <param name="sData">Data as string</param>
		/// <param name="oEnc">Encoder to use to create bytestream</param>
		/// <param name="sExpectedHTML">Expected HTML</param>
		void TestParser(string sData,Encoding oEnc,string sExpectedHTML)
		{
			TestParser(oEnc.GetBytes(sData),sExpectedHTML);
		}

		/// <summary>
		/// Tests parser by parsing chunk of data and then generating HTML on the basis of parsing
		/// and comparing this to expected HTML: in case of any discrepancies assertion will be fired:
		/// using Default encoding - may only be valid for ASCII chars
		/// </summary>
		/// <param name="sData">Data as string</param>
		/// <param name="sExpectedHTML">Expected HTML</param>
		void TestParser(string sData,string sExpectedHTML)
		{
			TestParser(Encoding.Default.GetBytes(sData),sExpectedHTML);
		}

		/// <summary>
		/// Tests parser by parsing chunk of data and then generating HTML on the basis of parsing
		/// and comparing this to expected HTML: in case of any discrepancies assertion will be fired:
		/// using Default encoding - may only be valid for ASCII chars
		/// </summary>
		/// <param name="sData">Data as string - expected HTML should be exactly the same</param>
		void TestParser(string sData)
		{
			TestParser(Encoding.Default.GetBytes(sData),sData);
		}

		[Test]
		public void OpenTagsWithoutAttributes()
		{
			// intentionally broken tag
			TestParser("<","<>");

			TestParser("<p>");

			// add spacing 
			TestParser("<p >","<p>");
			TestParser("<p  >","<p>");
			TestParser("< p  >","<p>");
			TestParser("<  p  >","<p>");
			TestParser("  <  p  >  ","<p>");
			TestParser("  <p     "+"\n\r"+">  ","<p>");
			TestParser("  <p     "+"\r\n"+">  ","<p>");
			TestParser("  <p     "+"\n"+">  ","<p>");
			TestParser("  <p     "+"\r"+">  ","<p>");

			TestParser("<ho>");

			TestParser("<hr width=90%>");

			TestParser("<hr>");
			TestParser("<br>");

			TestParser("<brrr    name=","<brrr name>");
			TestParser("<brrr    name","<brrr name>");
			
			TestParser("<br style='whooo'>");
		}

		[Test]
		public void OpenTagsWithAttributes()
		{
			TestParser("<td x:num=\"38669\" abc=xl29 z:num=\"12345\">");

			TestParser("<a href=\"test\"title=\"some title\">","<a href=\"test\" title=\"some title\">");
			TestParser("<a href='test'title='some title'>","<a href='test' title='some title'>");

			TestParser("<brrr name","<brrr name>");

			TestParser("<brrr name=alex surname=chudnovsky","<brrr name=alex surname=chudnovsky>");

			TestParser("<font    color   = blue>","<font color=blue>");

			TestParser("<p class='nice;&amp;&amp;' c='n' calone lastattr=\"test\" more>");

			TestParser("<font   color   = blue><b>","<font color=blue><b>");

			TestParser("<a href=\"?url=&lt;\">");

			TestParser("<a href=''>");

			TestParser("<brrr name=","<brrr name>");
		}

		[Test]
		public void MixedTagsAndWhiteSpaces()
		{
		
			// by default whitespaces before tags will be ignored
			TestParser("  <p>  <br>some text <b>bold</b> nooo","<p><br>some text <b>bold</b> nooo");

			// same thing but we want full whitespace
			try
			{
				oP.bCompressWhiteSpaceBeforeTag=false;
				TestParser("  <p>  <br>some text <b>bold</b> nooo");
			}
			finally
			{
				oP.bCompressWhiteSpaceBeforeTag=true;
			}
		}

		[Test]
		public void Text()
		{
			// empty text
			TestParser("");

			TestParser("                    whitespace        ");

			TestParser("some text");
			TestParser("some text with entities &amp; blah blah &amp; Co ");

			TestParser("some text with wrong entities &aoped;");
			TestParser("some text with wrong entities &aopeddddddddddd;");
			TestParser("some text with wrong entities &");

			TestParser("some text with unicode entity  &Scaron;");
		}

		[Test]
		public void FontSizes()
		{
			// test font size calculation
			Assert.AreEqual(HTMLparser.FontSize.Large,HTMLparser.ParseFontSize("+1",HTMLparser.FontSize.Medium));
			Assert.AreEqual(HTMLparser.FontSize.Small,HTMLparser.ParseFontSize("-1",HTMLparser.FontSize.Medium));

			Assert.AreEqual(HTMLparser.FontSize.Unknown,HTMLparser.ParseFontSize("",HTMLparser.FontSize.Medium));
			Assert.AreEqual(HTMLparser.FontSize.Unknown,HTMLparser.ParseFontSize("ald",HTMLparser.FontSize.Medium));

			Assert.IsFalse(HTMLparser.IsBiggerFont(HTMLparser.FontSize.Small,HTMLparser.FontSize.Large));
			Assert.IsTrue(HTMLparser.IsBiggerFont(HTMLparser.FontSize.Medium,HTMLparser.FontSize.Small));
			Assert.IsTrue(HTMLparser.IsEqualOrBiggerFont(HTMLparser.FontSize.Medium,HTMLparser.FontSize.Medium));
			Assert.IsTrue(HTMLparser.IsEqualOrBiggerFont(HTMLparser.FontSize.Medium,HTMLparser.FontSize.Small));
		}

		[Test]
		public void Widths()
		{
			// try calculation of widths

			bool bRelative=false;
			Assert.AreEqual(500,HTMLparser.CalculateWidth("500",1000,ref bRelative));
			Assert.IsFalse(bRelative);

			Assert.AreEqual(1000,HTMLparser.CalculateWidth("",1000,ref bRelative));

			Assert.AreEqual(1000,HTMLparser.CalculateWidth("10000000000000",1000,ref bRelative));

			Assert.AreEqual(500,HTMLparser.CalculateWidth("50%",1000,ref bRelative));
			Assert.IsTrue(bRelative);
		}

		[Test]
		public void TextWithEncoding()
		{
			// Unicode text - do NOT be tempted to save file in ASCII format as you would mangle the data
			string sText="привет А.......я...";

			string sMeta="<META http-equiv='content-type' content='text/html; charset=utf-8'>";

			TestParser(Encoding.UTF8.GetBytes(sMeta+sText),sMeta.ToLower()+sText);

			// now try rubbish encoding that we know will fail
			sMeta="<META http-equiv='content-type' content='text/html; charset=utf-88'>";
			sText="bad encoding";
			TestParser(Encoding.UTF8.GetBytes(sMeta+sText),sMeta.ToLower()+sText);
		}

		[Test]
		public void Comments()
		{
			try
			{
				oP.bAutoExtractBetweenTagsOnly=false;

				TestParser("<!-- comments auto extract -->");
			}
			finally
			{
				oP.bAutoExtractBetweenTagsOnly=true;
			}

			TestParser("<!-- comments -->");

			// test how unending comments are handled
			TestParser("<!-- comments","<!-- comments-->");

			TestParser("<!-- \n\n\n comments \n\n ---->");
		}

		[Test]
		public void Script()
		{
			try
			{
				oP.bKeepRawHTML=true;
				oP.bAutoExtractBetweenTagsOnly=false;
				TestParser("<SCRipt> some </script>");
			}
			finally
			{
				oP.bAutoExtractBetweenTagsOnly=true;
				oP.bKeepRawHTML=false;
			}

			TestParser("<SCRipt> some javascript here</SCRI blah"," some javascript here</SCRI blah");

			// intentionally lack script at the end - parser should not fail
			TestParser("<script> some javascript here"," some javascript here");

			TestParser("<script> some javascript here</script>"," some javascript here");
			
			// FIXIT: this test case really shows that our current behavior is wrong...
			TestParser("<script><!-- some javascript here<b>--></script>","<!-- some javascript here<b>-->");
		}

		[Test]
		public void ClosedTagsWithoutAttributes()
		{
			// tag without closing bracket - this is to test whether code fails to note end of data
			TestParser("</p ","</p>");

			TestParser("</p>");
			TestParser("</b>");

			TestParser("</ho>");

			TestParser("</hr>");
			TestParser("</br>");

			TestParser("</p    >","</p>");
			TestParser("</  p    >","</p>");

			TestParser("</P    >","</p>");
			TestParser("</  P    >","</p>");

			TestParser("</clsss>","</clsss>");

			TestParser("</class>","</class>");
			TestParser("</CLASS>","</class>");
			TestParser("</CLass>","</class>");

			TestParser("<class />","<class/>");
			TestParser("<CLASS/>","<class/>");
			TestParser("<CLass / >","<class/>");
		}

		[Test]
		public void ClosedTagsWithAttributes()
		{
			try
			{
				oP.bAutoMarkClosedTagsWithParamsAsOpen=false;

				// intentionally not including last > to test if parser catches it
				TestParser("</class   blah=bloh","<class blah=bloh/>");
				TestParser("</class   blah=>	","<class blah/>");
									  
				TestParser("</clsss param='testing'    PARaM2='blAh'     >","<clsss param='testing' param2='blAh'/>");
				TestParser("<clsss   param='testing'   PARaM2 =    'blAh'    />","<clsss param='testing' param2='blAh'/>");
				TestParser("<cLSSS     param    =   'testing' PARaM2='blAh'  /   >","<clsss param='testing' param2='blAh'/>");

				TestParser("</br>","</br>");
				TestParser("<br/>","<br/>");
				TestParser("<brrr/>","<brrr/>");

				TestParser("<brrr name/>");

				TestParser("</brrr name","<brrr name/>");

				TestParser("</class   blah=bloh      >","<class blah=bloh/>");
				TestParser("</class   blah=bloh'      >","<class blah=bloh'/>");
				TestParser("</class   blah      >","<class blah/>");
				TestParser("</a   href      >","<a href/>");
				TestParser("</CLASS>","</class>");
				TestParser("</CLass>","</class>");
			}
			finally
			{
				oP.bAutoMarkClosedTagsWithParamsAsOpen=true;
			}

			// default behavior is to consider closed tags with attributes to be actually open
			TestParser("</clsss end>","<clsss end>");

		}


	}
}
