//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using TeumesseCastle.Data;
using TeumesseCastle.Healthcheck;
using TeumesseCastle.template;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TeumesseCastle.Report
{
    public abstract class ReportBase
    {

        protected StringBuilder sb = new StringBuilder();
        public delegate bool HasDomainAmbigousNameDelegate(DomainKey domainKey);
        protected string ReportID;
        protected string TeumesseCastleEnterpriseBaseUrl = "https://github.com/Teumesse0/TeumesseCastle";

        protected GetUrlDelegateDomain GetUrlCallbackDomain;
        protected GetUrlDelegateAzureAD GetUrlCallbackAzureAD;
        protected GetAdditionInfoDelegate AdditionInfoDelegate;
        protected AddHtmlToTabSection TabDelegate;

        public HasDomainAmbigousNameDelegate HasDomainAmbigousName { get; set; }

        public void SetUrlDisplayDelegate(GetUrlDelegateDomain uRLDelegate)
        {
            GetUrlCallbackDomain = uRLDelegate;
        }

        public void SetUrlDisplayDelegate(GetUrlDelegateAzureAD uRLDelegate)
        {
            GetUrlCallbackAzureAD = uRLDelegate;
        }

        public void SetAdditionalInfoDelegate(GetAdditionInfoDelegate additionInfoDelegate)
        {
            AdditionInfoDelegate = additionInfoDelegate;
        }

        public void SetHtmlToTabSectionDelegate(AddHtmlToTabSection tabDelegate)
        {
            TabDelegate = tabDelegate;
        }

        private List<string> CSSToAdd = new List<string> { 
            TemplateManager.LoadBootstrapCss(),
            TemplateManager.LoadBootstrapTableCss(),
            TemplateManager.LoadReportBaseCss(),
        };

        private List<string> JSToAdd = new List<string> { 
            TemplateManager.LoadJqueryJs(), 
            TemplateManager.LoadPopperJs(), 
            TemplateManager.LoadBootstrapJs(),
            TemplateManager.LoadBootstrapTableJs(),
            TemplateManager.LoadReportBaseJs(),
        };

        public string GenerateReportFile(string filename)
        {
            ReferenceJSAndCSS();

            var reportSB = new StringBuilder(TemplateManager.LoadResponsiveTemplate());

            Hook(reportSB);

            sb.Length = 0;

            sb.Length = 0;
            GenerateCspMeta();
            AddLine();
            Add("<title>");
            GenerateTitleInformation();
            Add("</title>");
            /*Add("<base href=\"");
            Add(TeumesseCastleEnterpriseBaseUrl);
            if (!string.IsNullOrEmpty(ReportID))
            {
                AddEncoded(ReportID);
                Add("/");
            }
            Add("\">");*/
            Add(favicon);
            GenerateCss();
            reportSB = reportSB.Replace("<%=Header%>", sb.ToString());

            sb.Length = 0;
            GenerateBodyInformation();
            reportSB = reportSB.Replace("<%=Body%>", sb.ToString());

            sb.Length = 0;
            GenerateFooterInformation();
            GenerateJavascript();
            reportSB = reportSB.Replace("<%=Footer%>", sb.ToString());

            var html = reportSB.ToString();
            if (!String.IsNullOrEmpty(filename))
            {
                File.WriteAllText(filename, html);
            }
            return html;
        }

        private void GenerateJavascript()
        {
            foreach (var script in JSToAdd)
            {
                AddLine(@"<script type=""text/javascript"">");
                AddLine(script);
                AddLine("</script>");
            }
        }

        private void GenerateCss()
        {
            foreach (var css in CSSToAdd)
            {
                AddLine(@"<style type=""text/css"">");
                AddLine(css);
                AddLine("</style>");
            }
        }

        private void GenerateCspMeta()
        {
            Add(@"<meta http-equiv=""Content-Security-Policy"" content=""default-src 'self'; script-src ");
            foreach (var script in JSToAdd)
            {
                ComputeCSPHash(script);
            }
            Add(@" 'unsafe-inline'; style-src ");
            foreach (var css in CSSToAdd)
            {
                ComputeCSPHash(css);
            }
            Add(@" 'unsafe-inline'; object-src 'none'; base-uri 'self'; img-src data:;""/>");
        }

        private void ComputeCSPHash(string css)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes("\n" + css.Replace("\r", "") + "\n"));
                Add("'sha256-");
                Add(Convert.ToBase64String(bytes));
                Add("' ");
            }
        }

        protected void AddStyle(string style)
        {
            if (!CSSToAdd.Contains(style))
                CSSToAdd.Add(style);
        }

        protected void AddScript(string script)
        {
            if (!JSToAdd.Contains(script))
                JSToAdd.Add(script);
        }


        protected virtual void Hook(StringBuilder sbHtml)
        {

        }

        protected void AddLink(string link)
        {
            sb.Append("<a href=\"");
            sb.Append(link);
            sb.Append("\">");
            sb.Append(link);
            sb.Append("</a>");
        }

        protected void AddListStart()
        {
            Add(@"<ul><li>");
        }

        protected void AddListContinue()
        {
            Add(@"</li><li>");
        }

        protected void AddListEnd()
        {
            Add(@"</li></ul>");
        }

        protected void AddLine(string text)
        {
            sb.AppendLine(text);
        }

        protected void AddLine()
        {
            sb.AppendLine();
        }

        protected void Add(int value)
        {
            sb.Append(value);
        }

        protected void Add(ulong value)
        {
            sb.Append(value);
        }

        protected void Add(bool value)
        {
            sb.Append(value);
        }

        protected void Add(string text)
        {
            sb.Append(text);
        }

        protected void AddEncoded(string text)
        {
            sb.Append(ReportHelper.Encode(text));
        }

        protected void AddJsonEncoded(string text)
        {
            sb.Append(ReportHelper.EscapeJsonString(text));
        }

        protected void Add(DateTime date)
        {
            sb.Append(date.ToString("u"));
        }
        protected void DescribeBegin()
        {
            Add(@"<dl class='row'>");
        }

        protected void DescribeLabel(string label, string id = null)
        {
            Add(@"<dt class='col-sm-3'");
            if (!string.IsNullOrEmpty(id))
            {
                Add(" id='label_");
                Add(id);
                Add("'");
            }
            Add(">");
            AddEncoded(label);
            Add("</dt>");
        }

        protected void DescribeValue(string value, string id)
        {
            Add(@"<dd class='col-sm-9'");
            if (!string.IsNullOrEmpty(id))
            {
                Add(" id='label_");
                Add(id);
                Add("'");
            }
            Add(">");
            AddEncoded(value);
            Add("</dd>");
        }

        protected void DescribeEnd()
        {
            Add(@"</dl>");
        }


        protected void AddAnchor(string label)
        {
            Add(@"<a name=""");
            Add(label);
            Add(@"""></a>");
        }

        protected void AddParagraph(string content)
        {
            Add("<div class='row'><div class='col-lg-12'><p>");
            Add(content);
            Add("</p></div></div>");
        }

        protected void AddBeginTable(string ariaLabel, bool SimpleTable = false, string id = null)
        {
            Add(@"
		<div class=""row"">
			<div class=""col-md-12 table-responsive"">
				<table class=""table table-striped table-bordered");
            if (SimpleTable)
            {
                Add(" nopaging");
            }
            else
            {
                Add(@""" data-toggle=""table"" data-pagination=""true"" data-search=""true"" data-show-export=""true"" data-page-list=""[10,25,50,100,200,All]""");
            }
            if (!string.IsNullOrEmpty(id))
            {
                Add(" id=\"");
                AddEncoded(id);
                Add("\"");
            }
            if (!string.IsNullOrEmpty(ariaLabel))
            {
                Add(" aria-label=\"");
                AddEncoded(ariaLabel);
                Add("\"");
            }
            Add(@">
					<thead>
					<tr>");
        }

        protected void AddBeginTableData()
        {
            Add(@"</tr>
					</thead>
					<tbody>");
        }

        private bool footerMode = false;
        protected void AddEndTable(GenerateContentDelegate footer = null)
        {
            Add(@"</tbody>");
            if (footer != null)
            {
                Add("<tfoot>");
                AddBeginRow();
                footerMode = true;
                footer();
                footerMode = false;
                AddEndRow();
                Add("</tfoot>");
            }
            else
            {
                Add("<tfoot></tfoot>");
            }
            Add(@"
				</table>
			</div>
		</div>
");
        }

        protected void AddBeginRow()
        {
            Add(@"<tr>");
        }

        protected void AddEndRow()
        {
            Add(@"</tr>");
        }

        protected void AddBeginTooltip(bool wide = false, bool html = false)
        {
            Add(@"&nbsp;<i class=""info-mark d-print-none has-tooltip"" data-bs-placement=""bottom"" data1-bs-toggle=""tooltip""");
            if (wide)
            {
                Add(@" data-bs-template=""<div class='tooltip' role='tooltip'><div class='tooltip-arrow'></div><div class='tooltip-inner tooltip-wide'></div></div>"" ");
            }
            if (html)
            {
                Add(@" data-bs-html=""true"" ");
            }
            Add(@" title="""" data-bs-original-title=""");
        }

        protected void AddEndTooltip()
        {
            Add(@""">?</i>");
        }

        protected void AddHeaderText(string text, string tooltip, bool widetooltip, bool html = false)
        {
            AddHeaderText(text, tooltip, 0, 0, widetooltip, html);
        }

        protected void AddHeaderText(string text,
            string tooltip = null,
            int rowspan = 0,
            int colspan = 0,
            bool widetooltip = false,
            bool html = false,
            string datafield = null,
            string dataformatter = null
            )
        {
            Add(@"<th  data-sortable=""true"" ");
            if (rowspan != 0)
            {
                Add(@" rowspan=""");
                Add(rowspan);
                Add(@"""");
            }
            if (colspan != 0)
            {
                Add(@" colspan=""");
                Add(colspan);
                Add(@"""");
            }
            if (!string.IsNullOrEmpty(datafield))
            {
                Add(" data-field='");
                Add(datafield);
                Add("'");
            }
            if (!string.IsNullOrEmpty(dataformatter))
            {
                Add(" data-formatter='");
                Add(dataformatter);
                Add("'");
            }
            Add(@">");
            AddEncoded(text);
            if (!string.IsNullOrEmpty(tooltip))
            {
                AddBeginTooltip(widetooltip, html);
                // important: not encoded to pass html formatting
                Add(tooltip);
                AddEndTooltip();
            }
            Add("</th>");
        }

        protected void AddCellText(string text = null, bool highlight = false, bool IsGood = false)
        {
            Add("<td class='text'>");
            if (footerMode)
                Add("<b>");
            if (highlight)
            {
                if (IsGood)
                    Add("<span class=\"ticked\">");
                else
                    Add("<span class=\"unticked\">");
            }
            AddEncoded(text);
            if (highlight)
                Add("</span>");
            if (footerMode)
                Add("</b>");
            Add(@"</td>");
        }

        protected void AddCellTextNoWrap(string text)
        {
            Add(@"<td class='text text-nowrap'>");
            if (footerMode)
                Add("<b>");
            Add(text);
            if (footerMode)
                Add("</b>");
            Add(@"</td>");
        }

        protected void AddCellDateNoWrap(DateTime text)
        {
            Add(@"<td class='text text-nowrap'>");
            if (footerMode)
                Add("<b>");
            Add(text);
            if (footerMode)
                Add("</b>");
            Add(@"</td>");
        }

        protected void AddCellNumScore(int num)
        {
            Add("<td class='num ");
            if (num == 100)
            {
                Add("score_100");
            }
            else if (num >= 75)
            {
                Add("score_over75");
            }
            else if (num >= 50)
            {
                Add("score_over50");
            }
            else if (num >= 25)
            {
                Add("score_over25");
            }
            else
            {
                Add("score_below25");
            }
            Add("'>");
            Add(num);
            Add(@"</td>");
        }

        protected void AddCellNum(int num, bool HideIfZero = false)
        {
            Add("<td class='num'>");
            if (footerMode)
                Add("<b>");
            if (!(HideIfZero && num == 0))
            {
                Add(num);
            }
            if (footerMode)
                Add("</b>");
            Add(@"</td>");
        }

        protected void AddCellDate(DateTime date)
        {
            Add("<td>");
            if (footerMode)
                Add("<b>");
            if (date == DateTime.MinValue)
                Add("Not set");
            else
                Add(date);
            if (footerMode)
                Add("</b>");
            Add(@"</td>");
        }

        protected void AddCellBool(bool? value, bool highlight = false, bool IsGood = false)
        {
            if (value != null)
            {
                AddCellText(value.Value.ToString(), highlight, IsGood);
            }
            else
            {
                AddCellText();
            }
        }

        protected enum ShowModalType
        {
            Normal,
            XL,
            FullScreen
        }

        protected void AddBeginModal(string id, string title, ShowModalType modalType = ShowModalType.Normal)
        {
            Add(@"
<!--TAB dependancy -->
<div class=""modal");
            if (modalType == ShowModalType.FullScreen)
            {
                Add(" modal-full-screen");
            }
            Add(@""" id=""");
            Add(id);
            Add(@""" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
	<div class=""modal-dialog");
            if (modalType == ShowModalType.XL)
            {
                Add(" modal-xl");
            }
            else if (modalType == ShowModalType.FullScreen)
            {
                Add(" modal-full-screen-dialog");
            }
            Add(@""" role=""dialog"">
		<div class=""modal-content");
            if (modalType == ShowModalType.FullScreen)
            {
                Add(" modal-full-screen-content");
            }
            Add(@""">
			<div class=""modal-header"">
				<h4 class=""modal-title"">");
            AddEncoded(title);
            Add(@"</h4>
					<button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
			</div>
			<div class=""modal-body");
            if (modalType == ShowModalType.FullScreen)
            {
                Add(" modal-full-screen-body");
            }
            Add(@""">
");
        }

        protected void AddEndModal(ShowModalType modalType = ShowModalType.Normal)
        {
            Add(@"
			</div>
			<div class=""modal-footer");
            if (modalType == ShowModalType.FullScreen)
            {
                Add(" modal-full-screen-footer");
            }
            Add(@""">
				<button type=""button"" class=""btn btn-secondary"" data-bs-dismiss=""modal"">Close</button>
			</div>
		</div>
	</div>
</div>");
        }

        protected delegate void GenerateContentDelegate();

        protected abstract void GenerateFooterInformation();

        protected abstract void GenerateTitleInformation();

        protected abstract void ReferenceJSAndCSS();

        protected abstract void GenerateBodyInformation();

        protected static string favicon = @"<link href=""data:image/x-icon;base64,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"" rel=""icon"" type=""image/png"" />";
        protected static string brandLogo = @"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";

        protected void Brand(ADHealthCheckingLicense license)
        {
            if (!string.IsNullOrEmpty(license.Edition))
            {
                try
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BrandLogo"]))
                        brandLogo = ConfigurationManager.AppSettings["BrandLogo"];
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BrandCss"]))
                        AddStyle(ConfigurationManager.AppSettings["BrandCss"]);
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BrandJs"]))
                        AddScript(ConfigurationManager.AppSettings["BrandJs"]);
                }
                catch (Exception)
                {

                }
            }
        }

        protected void GenerateNavigation(string title, string domain, DateTime generationDate)
        {
            Add(@"
<nav class=""navbar py-0 navbar-expand-lg navbar-custom fixed-top border-bottom"">
	<div class=""container"">
		<a href=""#"" class=""navbar-brand"">
			<img src=""data:image/png;base64,");
            Add(brandLogo);
            Add(@""" />
		</a>
		<button class=""navbar-toggler"" type=""button"" data-bs-toggle=""collapse"" data-bs-target=""#navbarToggler"" aria-controls=""navbarToggler"" aria-expanded=""false"" aria-label=""Toggle navigation"">
			<span class=""navbar-toggler-icon""></span>
		</button>

		<div class=""collapse navbar-collapse"" id=""navbarToggler"">
			<ul class=""navbar-nav mr-auto"">
				<li class=""nav-item active"">
					<a class=""nav-link p-3"" href=""#"" role=""button"">
");
            if (String.IsNullOrEmpty(domain))
                Add(title);
            else
                Add(domain);
            Add(@"
					</a>
				</li>
				<li class=""nav-item"">
					<a class=""nav-link p-3"" href=""#"" role=""button"">
");
            Add(generationDate.ToString("yyyy-MM-dd"));
            Add(@"
					</a>
				</li>
				<li class=""nav-item"">
					<a class=""nav-link p-3"" role=""button"" href=""#modalAbout"" data-bs-toggle=""modal"">About</a>
				</li>
			</ul>
		</div>
	</div>
</nav>
");
        }

        protected string GenerateUniqueID(string input, long seed)
        {
            var d = new Dictionary<char, KeyValuePair<int, int>>()
            {
                { '.', new KeyValuePair<int, int>(0x3, 3 ) },
                { 'A', new KeyValuePair<int, int>(0xF, 4 ) },
                { 'L', new KeyValuePair<int, int>(0xE, 4 ) },
                { 'O', new KeyValuePair<int, int>(0xD, 4 ) },
                { 'C', new KeyValuePair<int, int>(0xB, 4 ) },
                { 'E', new KeyValuePair<int, int>(0xA, 4 ) },
                { 'R', new KeyValuePair<int, int>(0x5, 4 ) },
                { 'S', new KeyValuePair<int, int>(0x4, 4 ) },
                { 'T', new KeyValuePair<int, int>(0x2, 4 ) },
                { 'I', new KeyValuePair<int, int>(0x1, 4 ) },
                { 'N', new KeyValuePair<int, int>(0x0, 4 ) },
                { 'M', new KeyValuePair<int, int>(0x18, 5 ) },
                { 'D', new KeyValuePair<int, int>(0x12, 5 ) },
                { 'P', new KeyValuePair<int, int>(0x7, 5 ) },
                { 'F', new KeyValuePair<int, int>(0x6, 5 ) },
                { 'G', new KeyValuePair<int, int>(0x32, 6 ) },
                { 'U', new KeyValuePair<int, int>(0x23, 6 ) },
                { '-', new KeyValuePair<int, int>(0x21, 6 ) },
                { 'B', new KeyValuePair<int, int>(0x4F, 7 ) },
                { 'Y', new KeyValuePair<int, int>(0x4C, 7 ) },
                { 'H', new KeyValuePair<int, int>(0x45, 7 ) },
                { 'Z', new KeyValuePair<int, int>(0x44, 7 ) },
                { '0', new KeyValuePair<int, int>(0xCF, 8 ) },
                { '1', new KeyValuePair<int, int>(0xCE, 8 ) },
                { 'K', new KeyValuePair<int, int>(0xCC, 8 ) },
                { 'X', new KeyValuePair<int, int>(0x9D, 8 ) },
                { 'W', new KeyValuePair<int, int>(0x9C, 8 ) },
                { 'V', new KeyValuePair<int, int>(0x9B, 8 ) },
                { '6', new KeyValuePair<int, int>(0x83, 8 ) },
                { '2', new KeyValuePair<int, int>(0x82, 8 ) },
                { '3', new KeyValuePair<int, int>(0x80, 8 ) },
                { '5', new KeyValuePair<int, int>(0x19B, 9 ) },
                { '9', new KeyValuePair<int, int>(0x19A, 9 ) },
                { '7', new KeyValuePair<int, int>(0x135, 9 ) },
                { '8', new KeyValuePair<int, int>(0x103, 9 ) },
                { '4', new KeyValuePair<int, int>(0x269, 10 ) },
                { 'Q', new KeyValuePair<int, int>(0x268, 10 ) },
                { 'J', new KeyValuePair<int, int>(0x205, 10 ) },
                { '_', new KeyValuePair<int, int>(0x204, 10 ) },
            };
            string o = null;
            foreach (char c in input.ToUpperInvariant())
            {
                if (!d.ContainsKey(c))
                    continue;
                var t = d[c];
                var v = Convert.ToString(t.Key, 2);
                if (t.Value > v.Length)
                    o += new string('0', t.Value - v.Length) + v;
                else
                    o += v.Substring(v.Length - t.Value, t.Value);
            }
            o += Convert.ToString(0x80, 2);
            byte[] bytes = new byte[o.Length / 8 + 1];
            for (int i = 0; i < bytes.Length - 1; ++i)
            {
                bytes[i] = Convert.ToByte(o.Substring(8 * i, 8), 2);
            }
            bytes[bytes.Length - 1] = (byte)(seed % 256);
            return Convert.ToBase64String(bytes).Replace("=", "");
        }

        protected void GenerateAbout()
        {
            GenerateAbout(@"<p><strong>Generated by <a href=""https://github.com/Teumesse0/TeumesseCastle"">TeumesseCastle</a></strong></p>
<p>Based on PingCastle 3.0.0.0 by Vincent Le Toux (NPOSL-3.0)</p>
<p>Options:</p>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='optionWideScreen'>
  <label class='form-check-label' for='optionWideScreen'>
    Enable wide mode
  </label>
</div>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='optionPagination'>
  <label class='form-check-label' for='optionPagination'>
    Remove pagination in tables
  </label>
</div>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='optionExpand'>
  <label class='form-check-label' for='optionExpand'>
    Expand all collpsed items
  </label>
</div>
<p>Open source components:</p>
<ul>
<li><a href=""https://getbootstrap.com/"">Bootstrap</a> licensed under the <a href=""https://tldrlegal.com/license/mit-license"">MIT license</a></li>
<li><a href=""https://bootstrap-table.com/"">Bootstrap Table </a> licensed under the <a href=""https://tldrlegal.com/license/mit-license"">MIT license</a></li>
<li><a href=""https://popper.js.org/"">Popper.js</a> licensed under the <a href=""https://tldrlegal.com/license/mit-license"">MIT license</a></li>
<li><a href=""https://jquery.org"">JQuery</a> licensed under the <a href=""https://tldrlegal.com/license/mit-license"">MIT license</a></li>
</ul>");
        }
        protected void GenerateAbout(string aboutString)
        {
            Add(@"
<!-- Modal -->
<div class=""modal"" id=""modalAbout"" role=""dialog"">
    <div class=""modal-dialog modal-lg"">
        <!-- Modal content-->
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h4 class=""modal-title"">About</h4>
                <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
            </div>
            <div class=""modal-body"">
                <div class=""row"">
                     <div class=""col-lg-12"">
");
            Add(aboutString);
            Add(@"
                     </div>
                </div>
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-secondary"" data-bs-dismiss=""modal"">Close</button>
            </div>
        </div>

    </div>
</div>
");
        }

        protected virtual void GenerateSection(string title, GenerateContentDelegate generateContent)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9-]");
            string id = "section" + rgx.Replace(title, "");
            Add(@"
<!-- Section " + title + @" -->
<div id=""" + id + @""">
	<div class=""row"">
		<div class=""col-lg-12"">
			<div class=""starter-template"">
				<div class=""card border-pc-orange mb-4 mt-4 lead fs-6"">
					<div class=""card-header h3 text-white bg-pc-orange fw-bold"">
						<a class=""sectionheader text-white""data-bs-toggle=""collapse"" aria-expanded=""true"" href=""#panel" + id + @""">" + title + @"</a>
					</div>
					<div class=""card-body collapse show"" id=""panel" + id + @""">
");
            generateContent();
            Add(@"
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
<div class=""pagebreak""> </div>
<!-- Section " + title + @" end -->
");
        }

        protected void GenerateSubSection(string title, string section = null)
        {
            Add(@"
		<!-- SubSection ");
            AddEncoded(title);
            Add(@" -->");
            if (!string.IsNullOrEmpty(section))
            {
                AddAnchor(section);
            }
            Add(@"
		<div class=""row""><div class=""col-lg-12 mt-2"">
			<h2>");
            AddEncoded(title);
            Add(@"</h2>
		</div></div>
        <!-- SubSection ");
            AddEncoded(title);
            Add(@" end -->");
        }

        protected void GenerateAccordion(string accordion, GenerateContentDelegate content)
        {
            Add(@"
		<!-- Accordion ");
            AddEncoded(accordion);
            Add(@" -->
<div id=""");
            Add(accordion);
            Add(@""">
<div class=""card"">
    ");
            content();
            Add(@"</div></div>
			<!-- Accordion ");
            Add(accordion);
            Add(@" end -->
");

        }

        protected void GenerateAccordionDetailForDetail(string id, string dataParent, string title, int itemCount, GenerateContentDelegate content)
        {
            GenerateAccordionDetail(id, dataParent, title,
                () =>
                {
                    Add(@"<i class=""float-end"">[");
                    Add((int)itemCount);
                    Add(@"]</i>");
                }, content);
        }

        protected void GenerateAccordionDetail(string id, string dataParent, string title, GenerateContentDelegate header, GenerateContentDelegate content)
        {
            Add(@"
    <div class=""card-header"" id=""heading");
            Add(id);
            Add("\">");
            AddAnchor("anchor" + id);
            Add(@"
      <span class=""card-title mb-0"">
        <button class=""btn btn-link"" data-bs-toggle=""collapse"" data-bs-target=""#");
            Add(id);
            Add(@""" aria-expanded=""false"" aria-controls=""");
            Add(id);
            Add(@""">
          ");
            Add(title);
            Add(@"
        </button>
      </span>
");
            if (header != null)
            {
                header();
            }
            Add(@"
    </div>

    <div id=""");
            Add(id);
            Add(@""" class=""collapse"" aria-labelledby=""heading");
            Add(id);
            Add(@""" data-parent=""#");
            Add(dataParent);
            Add(@""">
      <div class=""card-body"">
        ");
            content();
            Add(@"
    </div>
  </div>");
        }

        private static string GenerateId(string title)
        {
            return "section" + title.Replace(" ", "");
        }

        protected void GenerateTabHeader(string title, string selectedTab, bool defaultIfTabEmpty = false)
        {
            string id = GenerateId(title);
            bool isActive = (String.IsNullOrEmpty(selectedTab) ? defaultIfTabEmpty : selectedTab == id);
            Add(@"<li class=""nav-item""><a href=""#");
            Add(id);
            Add(@""" class=""nav-link ");
            if (isActive)
                Add(@"active");
            Add(@""" role=""tab"" data-bs-toggle=""tab""");
            Add(@" id=""bs-");
            Add(id);
            Add(@"""");
            Add(@">");
            Add(title);
            Add("</a></li>");
        }

        protected void GenerateSectionFluid(string title, GenerateContentDelegate generateContent, string selectedTab, bool defaultIfTabEmpty = false)
        {
            string id = GenerateId(title);
            bool isActive = (String.IsNullOrEmpty(selectedTab) ? defaultIfTabEmpty : selectedTab == id);
            Add(@"
<!-- Section ");
            Add(title);
            Add(@" -->
<div class=""tab-pane");
            if (isActive)
                Add(" active");
            Add(@""" id=""");
            Add(id);
            Add(@""">
<div class=""row""><div class=""col-lg-12"">
	<div class=""starter-template"">
		<div class=""row""><div class=""col-lg-12"">
			<h1>");
            Add(title);
            Add(@"</h1>
		</div></div>
");
            if (TabDelegate != null)
            {
                Add(TabDelegate(title));
            }
            generateContent();
            Add(@"
			</div>
		</div>
	</div>
</div>
<!-- Section " + title + @" end -->
");
        }

        protected int OrderOS(string os1, string os2)
        {
            int ios1 = OSToInt(os1);
            int ios2 = OSToInt(os2);
            if (ios1 > 0 && ios2 > 0)
            {
                if (ios1 > ios2)
                    return 1;
                else if (ios1 < ios2)
                    return -1;
                return 0;
            }
            else if (ios1 > 0)
                return -1;
            else if (ios2 > 0)
                return 1;
            return String.Compare(os1, os2);
        }

        // this function is used to sort operating system based not on name but on its order
        // the value returned doesn't have a meaning at all. It is used for a comparison & sort
        protected int OSToInt(string os)
        {
            switch (os)
            {
                case "Windows XP":
                    return 1;
                case "Windows Vista":
                    return 2;
                case "Windows 7":
                    return 3;
                case "Windows 8":
                    return 4;
                case "Windows 10":
                    return 5;
                case "Windows 11":
                    return 6;
                case "Windows NT":
                    return 7;
                case "Windows 2000":
                    return 8;
                case "Windows 2003":
                    return 9;
                case "Windows 2008":
                    return 10;
                case "Windows 2012":
                    return 11;
                case "Windows 2016":
                    return 12;
                case "Windows 2019":
                    return 13;
                case "Windows 2022":
                    return 14;
                case "Windows Embedded":
                    return 15;
                case "OperatingSystem not set":
                    return 16;
            }
            return 100;
        }

        // ref: https://social.technet.microsoft.com/wiki/contents/articles/22615.how-to-get-the-number-of-computers-per-windows-operating-system-in-an-active-directory-domain-using-powershell.aspx
        // https://docs.microsoft.com/en-us/windows-server/get-started/windows-server-release-info
        // https://docs.microsoft.com/en-us/windows/release-health/release-information
        public static string GetOSVersionString(HealthcheckOSVersionData osVersion)
        {
            // ex: 10.0 (18362)
            Regex re = new Regex("(?<major>\\d+).(?<minor>\\d+)( \\((?<release>\\d+)\\))?");
            if (osVersion == null || string.IsNullOrEmpty(osVersion.OSVersion))
                return "Error";
            var m = re.Match(osVersion.OSVersion);
            if (!m.Success)
                return "Unknown (" + osVersion.OSVersion + (osVersion.IsServer ? " (Server)" : null) + ")";
            int major = int.Parse(m.Groups["major"].Value);
            int minor = int.Parse(m.Groups["minor"].Value);
            int release = 0;
            if (!string.IsNullOrEmpty(m.Groups["release"].Value))
                release = int.Parse(m.Groups["release"].Value);
            if (osVersion.IsServer)
            {
                if (major == 3)
                {
                    return "Windows NT 3.51 Server";
                }
                if (major == 4)
                {
                    return "Windows NT 4.0 Server";
                }
                if (major == 5)
                {
                    if (minor == 0)
                    {
                        return "Windows 2000 Server";
                    }
                    if (minor == 1)
                    {
                        return "Windows Server 2003";
                    }
                    if (minor == 2)
                    {
                        return "Windows Server 2003 SP2";
                    }
                }
                if (major == 6)
                {
                    if (minor == 0)
                        return "Windows Server 2008";
                    if (minor == 1)
                        return "Windows Server 2008 R2";
                    if (minor == 2)
                        return "Windows Server 2012";
                    if (minor == 3)
                        return "Windows Server 2012 R2";
                }
                if (major == 10)
                {
                    if (minor == 0)
                    {
                        switch (release)
                        {
                            case 14393: return "Windows Server 2016 1607";
                            //case 16299:
                            case 17763: return "Windows Server 2019 1809";
                            case 18362: return "Windows Server 2019 1903";
                            case 18363: return "Windows Server 2019 1909";
                            case 19041: return "Windows Server 2019 2004";
                            case 19042: return "Windows Server 2019 20H2";
                            // https://learn.microsoft.com/en-us/windows-server/get-started/windows-server-release-info
                            case 20348: return "Windows Server 2022";
                            default: return "Windows Server 2022 (Build " + release + ")";
                        }
                    }
                }
            }
            else
            {
                if (major == 3)
                {
                    return "Windows NT 3.51 Workstation";
                }
                if (major == 4)
                {
                    return "Windows NT 4.0 Workstation";
                }
                if (major == 5)
                {
                    if (minor == 0)
                        return "Windows 2000";
                    if (minor == 1)
                        return "Windows XP";
                    if (minor == 2)
                        return "Windows XP 64-Bit Edition";
                }
                if (major == 6)
                {
                    if (minor == 0)
                        return "Windows Vista";
                    if (minor == 1)
                        return "Windows 7";
                    if (minor == 2)
                        return "Windows 8";
                    if (minor == 3)
                        return "Windows 8.1";
                }
                if (major == 10 && minor == 0)
                {
                    string w;
                    switch (release)
                    {
                        case 22621: w = "Windows 11 22H2"; break;
                        case 22000: w = "Windows 11 21H2"; break;
                        case 22449:
                        case 22518:
                        case 21996: w = "Windows 11 Dev (" + release + ")"; break;
                        case 20185: w = "Windows 10 21H1 Dev"; break;
                        case 19045: w = "Windows 10 22H2"; break;
                        case 19044: w = "Windows 10 21H2"; break;
                        case 19043: w = "Windows 10 21H1"; break;
                        case 19042: w = "Windows 10 20H2"; break;
                        case 18908: w = "Windows 10 20H1"; break;
                        case 18356: w = "Windows 10 19H1"; break;
                        case 19041: w = "Windows 10 2004"; break;
                        case 18363: w = "Windows 10 1909"; break;
                        case 18362: w = "Windows 10 1903"; break;
                        case 17763: w = "Windows 10 1809"; break;
                        case 17134: w = "Windows 10 1803"; break;
                        case 16299: w = "Windows 10 1709"; break;
                        case 15063: w = "Windows 10 1703"; break;
                        case 14393: w = "Windows 10 1607"; break;
                        case 10586: w = "Windows 10 1511"; break;
                        case 10240: w = "Windows 10 1507"; break;
                        default:
                            if (release >= 22000)
                            {
                                w = "Windows 11 (Build " + release + ")"; break;
                            }
                            else
                            {
                                w = "Windows 10 (Build " + release + ")"; break;
                            }
                    }
                    return w + (osVersion.IsLTSC ? " (LTSC)" : null);
                }
            }
            return "Unknown (" + osVersion.OSVersion + (osVersion.IsServer ? " (Server)" : null) + ")";
        }

        protected void AddPSOStringValue(GPPSecurityPolicy policy, string propertyName)
        {
            foreach (var property in policy.Properties)
            {
                if (property.Property == propertyName)
                {
                    if (property.Value == 0)
                    {
                        if (propertyName == "PasswordComplexity")
                        {
                            Add("<td><span class=\"unticked\">False</span></td>");
                            return;
                        }
                        if (propertyName == "ClearTextPassword"
                            || propertyName == "ScreenSaveActive" || propertyName == "ScreenSaverIsSecure")
                        {
                            AddCellText("False");
                            return;
                        }
                    }
                    if (property.Value == -1 && propertyName == "MaximumPasswordAge")
                    {
                        Add("<td><span class=\"unticked\">Never expires</span></td>");
                        return;
                    }
                    if (property.Value == 1)
                    {
                        if (propertyName == "ClearTextPassword")
                        {
                            Add("<td><span class=\"unticked\">True</span></td>");
                            return;
                        }
                        if (propertyName == "PasswordComplexity"
                            || propertyName == "ScreenSaveActive" || propertyName == "ScreenSaverIsSecure")
                        {
                            AddCellText("True");
                            return;
                        }
                    }
                    if (propertyName == "MinimumPasswordLength")
                    {
                        if (property.Value < 8)
                        {
                            Add("<td><span class=\"unticked\">" + property.Value.ToString() + "</span></td>");
                            return;
                        }
                    }
                    if (propertyName == "MinimumPasswordAge")
                    {
                        if (property.Value == 0)
                        {
                            Add("<td><span class=\"unticked\">0 day</span></td>");
                            return;
                        }
                        AddCellText(property.Value + " day(s)");
                        return;
                    }
                    if (propertyName == "MaximumPasswordAge")
                    {
                        AddCellText(property.Value + " day(s)");
                        return;
                    }
                    if (propertyName == "ResetLockoutCount" || propertyName == "LockoutDuration")
                    {
                        if (property.Value <= 0)
                        {
                            AddCellText("Infinite");
                            return;
                        }
                        AddCellText(property.Value + " minute(s)");
                        return;
                    }
                    AddCellNum(property.Value);
                    return;
                }
            }
            AddCellText("Not Set");
        }

        protected string GetLinkForLsaSetting(string property)
        {
            switch (property.ToLowerInvariant())
            {
                case "enableguestaccount":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/accounts-guest-account-status"">Guest account</a> (<a href=""https://msdn.microsoft.com/en-us/library/hh128296.aspx"">Technical details</a>)";
                case "lsaanonymousnamelookup":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-access-allow-anonymous-sidname-translation"">Allow anonymous SID/Name translation</a> (<a href=""https://msdn.microsoft.com/en-us/library/hh128296.aspx"">Technical details</a>)";
                case "everyoneincludesanonymous":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-access-let-everyone-permissions-apply-to-anonymous-users"">Let Everyone permissions apply to anonymous users</a> (<a href=""https://support.microsoft.com/en-us/kb/278259"">Technical details</a>)";
                case "limitblankpassworduse":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/accounts-limit-local-account-use-of-blank-passwords-to-console-logon-only"">Limit local account use of blank passwords to console logon only</a> (<a href=""https://technet.microsoft.com/en-us/library/jj852174.aspx"">Technical details</a>)";
                case "forceguest":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-access-sharing-and-security-model-for-local-accounts"">Sharing and security model for local accounts</a> (<a href=""https://technet.microsoft.com/en-us/library/jj852219%28v=ws.11%29.aspx"">Technical details</a>)";
                case "lmcompatibilitylevel":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-security-lan-manager-authentication-level"">LAN Manager authentication level</a> (<a href=""https://technet.microsoft.com/en-us/library/cc960646.aspx"">Technical details</a>)";
                case "nolmhash":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-security-do-not-store-lan-manager-hash-value-on-next-password-change"">Do not store LAN Manager hash value on next password change</a> (<a href=""https://technet.microsoft.com/en-us/library/cc736342%28v=ws.10%29.aspx"">Technical details</a>)";
                case "restrictanonymous":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-access-do-not-allow-anonymous-enumeration-of-sam-accounts-and-shares"">Do not allow anonymous enumeration of SAM accounts and shares</a> (<a href=""https://technet.microsoft.com/en-us/library/cc963223.aspx"">Technical details</a>)";
                case "restrictanonymoussam":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-access-do-not-allow-anonymous-enumeration-of-sam-accounts"">Do not allow anonymous enumeration of SAM accounts</a> (<a href=""https://technet.microsoft.com/en-us/library/jj852184.aspx"">Technical details</a>)";
                case "ldapclientintegrity":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-security-ldap-client-signing-requirements"">LDAP client signing requirements</a> (<a href=""https://support.microsoft.com/en-us/help/935834/how-to-enable-ldap-signing-in-windows-server-2008"">Technical details</a>)";
                case "recoveryconsole_securitylevel":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/recovery-console-allow-automatic-administrative-logon"">Recovery console: Allow automatic administrative logon</a>";
                case "refusepasswordchange":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/domain-controller-refuse-machine-account-password-changes"">Refuse machine account password changes</a> (<a href=""https://support.microsoft.com/en-us/help/154501/how-to-disable-automatic-machine-account-password-changes"">Technical details</a>)";
                case "enablemulticast":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/client-management/mdm/policy-csp-admx-dnsclient#admx-dnsclient-turn-off-multicast"">Turn off multicast name resolution</a> (<a href=""https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-llmnrp/02b1d227-d7a2-4026-9fd6-27ea5651fe85"">Technical details</a>)";
                case "enablesecuritysignature":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/smbv1-microsoft-network-server-digitally-sign-communications-if-client-agrees"">Microsoft network server: Digitally sign communications (if client agrees)</a> (<a href=""https://www.stigviewer.com/stig/windows_server_2016/2017-11-20/finding/V-73663"">Technical details</a>)";
                case "enablemodulelogging":
                    return @"<a href=""https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_group_policy_settings?view=powershell-6"">Powershell: Turn on Module logging</a>";
                case "enablescriptblocklogging":
                    return @"<a href=""https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_group_policy_settings?view=powershell-6"">Powershell: Turn on Powershell Script Block logging</a>";
                case "srvsvcsessioninfo":
                    return @"<a href=""https://github.com/p0w3rsh3ll/NetCease"">Hardening Net Session Enumeration</a>";
                case "supportedencryptiontypes":
                    return @"<a href=""https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/network-security-configure-encryption-types-allowed-for-kerberos"">Network security: Configure encryption types allowed for Kerberos</a>";
                case "enablecbacandarmor":
                    return @"<a href=""https://learn.microsoft.com/en-us/windows/client-management/mdm/policy-csp-kerberos"">Kerberos client support for claims, compound authentication and Kerberos armoring</a>";
                case "cbacandarmorlevel":
                    return @"<a href=""https://learn.microsoft.com/en-us/windows-server/identity/ad-fs/operations/ad-fs-compound-authentication-and-ad-ds-claims"">KDC support for claims compound authentication and Kerberos armoring</a>";
            }
            return property;
        }

        protected void AddLsaSettingsValue(string property, int value)
        {
            Add("<td class='text'>");
            switch (property.ToLowerInvariant())
            {
                case "enablemulticast":
                    if (value == 0)
                    {
                        Add(@"<span class=""ticked"">LLMNR disabled</span>");
                    }
                    else
                    {
                        Add(@"<span class=""unticked"">LLMNR Enabled</span>");
                    }
                    break;
                case "lmcompatibilitylevel":
                    if (value == 0)
                    {
                        Add(@"<span class=""unticked"">Send LM & NTLM responses</span>");
                    }
                    else if (value == 1)
                    {
                        Add(@"<span class=""unticked"">Send LM & NTLM</span>");
                    }
                    else if (value == 2)
                    {
                        Add(@"<span class=""unticked"">Send NTLM response only</span>");
                    }
                    else if (value == 3)
                    {
                        Add("Send NTLMv2 response only");
                    }
                    else if (value == 4)
                    {
                        Add("Send NTLMv2 response only. Refuse LM Client devices");
                    }
                    else if (value == 5)
                    {
                        Add("Send NTLMv2 response only. Refuse LM & NTLM");
                    }
                    break;
                case "ldapclientintegrity":
                    if (value == 0)
                    {
                        Add(@"<span class=""unticked"">None</span> (Do not request signature)");
                    }
                    else
                    {
                        Add(value);
                    }
                    break;
                case "srvsvcsessioninfo":
                case "enablemodulelogging":
                case "enablescriptblocklogging":
                    if (value == 0)
                    {
                        Add(@"<span class=""unticked"">Disabled</span>");
                    }
                    else
                    {
                        Add(@"<span class=""ticked"">Enabled</span>");
                    }
                    break;
                case "supportedencryptiontypes":
                    Add(SupportedEncryptionTypeToString(value));
                    break;
                default:
                    if (value == 0)
                    {
                        Add(@"<span class=""unticked"">Disabled</span>");
                    }
                    else
                    {
                        Add(@"<span class=""unticked"">Enabled</span>");
                    }
                    break;
            }
            Add("</td>");
        }

        protected string SupportedEncryptionTypeToString(int msDSSupportedEncryptionTypes)
        {
            // see https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-kile/6cfc7b50-11ed-4b4d-846d-6f08f0812919
            List<string> algs = new List<string>();
            if ((msDSSupportedEncryptionTypes & 1) != 0)
            {
                algs.Add(@"<span class=""unticked"">DES-CBC-CRC</span>");
            }
            if ((msDSSupportedEncryptionTypes & 2) != 0)
            {
                algs.Add(@"<span class=""unticked"">DES-CBC-MD5</span>");
            }
            if ((msDSSupportedEncryptionTypes & 4) != 0)
            {
                algs.Add("RC4-HMAC");
            }
            if ((msDSSupportedEncryptionTypes & 8) != 0)
            {
                algs.Add(@"<span class=""ticked"">AES128-CTS-HMAC-SHA1-96</span>");
            }
            if ((msDSSupportedEncryptionTypes & 16) != 0)
            {
                algs.Add(@"<span class=""ticked"">AES256-CTS-HMAC-SHA1-96</span>");
            }
            if ((msDSSupportedEncryptionTypes & ~(1 + 2 + 4 + 8 + 16)) != 0)
            {
                algs.Add(@"Future encryption");
            }
            return string.Join(", ", algs.ToArray());
        }

        protected void GenerateGauge(int percentage)
        {
            Add(@"<svg viewBox=""0 0 400 230"" ><g transform=""translate(190, 190)""><path class=""arc chart-first"" d=""M-170,2.0818995585505004e-14A170,170 0 0,1 -120.20815280171311,-120.20815280171307L-86.26702730475881,-86.26702730475878A122,122 0 0,0 -122,1.494069094959771e-14Z""></path><path class=""arc chart-second"" d=""M-117.165698922674,-123.17548049819537A170,170 0 0,1 -3.122849337825751e-14,-170L-2.2411036424396564e-14,-122A122,122 0 0,0 -84.08361922686016,-88.39652129870491Z""></path><path class=""arc chart-third"" d=""M4.249557305501125,-169.94687776686945A170,170 0 0,1 120.20815280171306,-120.20815280171311L86.26702730475877,-86.26702730475881A122,122 0 0,0 3.0496823015949253,-121.96187698563573Z""></path><path class=""arc chart-quart"" d=""M123.17548049819526,-117.16569892267411A170,170 0 0,1 170,-1.9262832252003132e-13L122,-1.382391491026107e-13A122,122 0 0,0 88.39652129870484,-84.08361922686025Z""></path><circle class=""needle-center"" cx=""0"" cy=""0"" r=""18""></circle><path class=""needle"" d=""");
            GenerateChartNeedle(percentage);
            Add(@"""></path></g><text transform=""translate(3.8, 190)"" font-size=""15"">0</text><text transform=""translate(176.74418604651163, 12.666666666666666)"" font-size=""15"">50</text><text transform=""translate(368.93203883495147, 190)"" font-size=""15"">100</text></svg>");
        }

        protected void GenerateChartNeedle(int percentage)
        {
            double leftX, leftY, rightX, rightY, thetaRad, topX, topY;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            thetaRad = percentage * Math.PI / 100;
            topX = -144 * Math.Cos(thetaRad);
            topY = -144 * Math.Sin(thetaRad);
            leftX = -18 * Math.Cos(thetaRad - Math.PI / 2);
            leftY = -18 * Math.Sin(thetaRad - Math.PI / 2);
            rightX = -18 * Math.Cos(thetaRad + Math.PI / 2);
            rightY = -18 * Math.Sin(thetaRad + Math.PI / 2);
            Add("M ");
            Add(leftX.ToString(nfi));
            Add(" ");
            Add(leftY.ToString(nfi));
            Add(" L ");
            Add(topX.ToString(nfi));
            Add(" ");
            Add(topY.ToString(nfi));
            Add(" L ");
            Add(rightX.ToString(nfi));
            Add(" ");
            Add(rightY.ToString(nfi));
        }

        protected string PrintDomain(DomainKey key, string risk)
        {
            string label = key.DomainName;
            if (String.IsNullOrEmpty(label))
            {
                if (!String.IsNullOrEmpty(key.DomainNetBIOS))
                    label = key.DomainNetBIOS;
                else
                    label = key.DomainSID;
            }
            if (GetUrlCallbackDomain == null)
                return label;
            string htmlData = GetUrlCallbackDomain(key, label, risk);
            if (String.IsNullOrEmpty(htmlData))
                return label;
            return htmlData;
        }

        protected string NewLineToBR(string data)
        {
            if (String.IsNullOrEmpty(data))
                return data;
            return data.Replace("\r\n", "<br>\r\n");
        }
    }
}
