namespace HannibalUI.Editor.CodeGen
{
    using System.Text;

    /// <summary>
    /// Turns free-form config names into valid C# identifiers for generated code.
    /// </summary>
    public static class IdentifierUtility
    {
        /// <summary>
        /// PascalCase identifier from a display name — used for type/enum members like CanvasType.
        /// "Main Menu" -> "MainMenu", "shop" -> "Shop", "2p" -> "_2p".
        /// </summary>
        public static string ToPascalCase(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "_";
            }

            var sb = new StringBuilder(raw.Length);
            bool startWord = true;

            foreach (var c in raw)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(startWord ? char.ToUpperInvariant(c) : c);
                    startWord = false;
                }
                else
                {
                    startWord = true;
                }
            }

            if (sb.Length == 0)
            {
                return "_";
            }

            if (char.IsDigit(sb[0]))
            {
                sb.Insert(0, '_');
            }

            return sb.ToString();
        }

        /// <summary>
        /// A valid identifier that preserves the caller's casing and single underscores — used for
        /// UIEvents members. "ON_MARKET_BUTTON_CLICK" is preserved; "market  click" -> "market_click".
        /// </summary>
        public static string ToValidIdentifier(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "_";
            }

            var sb = new StringBuilder(raw.Length);
            bool lastWasSeparator = false;

            foreach (var c in raw.Trim())
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                    lastWasSeparator = false;
                }
                else if (!lastWasSeparator)
                {
                    sb.Append('_');
                    lastWasSeparator = true;
                }
            }

            var result = sb.ToString().Trim('_');

            if (result.Length == 0)
            {
                return "_";
            }

            if (char.IsDigit(result[0]))
            {
                result = "_" + result;
            }

            return result;
        }
    }
}
