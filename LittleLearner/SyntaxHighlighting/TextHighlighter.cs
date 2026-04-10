using ColorfulCode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LittleLearner.SyntaxHighlighting
{
    public class TextHighlighter
    {
        public bool initialized = false;
        public static SyntaxSet syntaxSets = SyntaxSet.LoadDefaults();
        public static ThemeSet themeSets = ThemeSet.LoadDefaults();
        public string code = "";
        public int cursorPosition = 0;
        public Syntax syntax;
        public Theme theme;

        public TextHighlighter()
        {
            this.syntax = syntaxSets.FindByExtension("c");
            this.theme = themeSets["InspiredGitHub"];
        }

        public string initializeCodeHighligher(string? code) {
            if (initialized) throw new Exception("Text Highlighter has allready been initialized");
            if (code == null) code = "";

            initialized = true;
            code = this.updateCode(code);

            return
                $@"<body contenteditable=""true"" id=""TextContainer"" style=""background-color: lightblue;"">{code}</body>
                   <script>
                    let textContainer = document.querySelector(""#TextContainer"");
                    textContainer.addEventListener(""input"", codeEdited);
                    function codeEdited(){{window.location.href = (""https://google.com?newCode="" + textContainer.innerText)}}
                    function setInnerText(text){{ textContainer.innerHTML = text; }}
                   </script>";
        }

        public string updateCode(string newCode)
        {
            string coloredCode = syntax.HighlightToHtml(newCode, theme);
            coloredCode = new Regex("^<pre[^>]*>\\n").Replace(coloredCode, "");
            coloredCode = new Regex("</pre>\\n$").Replace(coloredCode, "");

            return coloredCode;
        }
    }
}
