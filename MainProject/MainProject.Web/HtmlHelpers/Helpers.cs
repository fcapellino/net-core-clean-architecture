namespace MainProject.Web.Helpers
{
    using System.Reflection;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class Helpers
    {
        public static HtmlString RawHtmlString(this IHtmlHelper html, object value)
        {
            var rawContent = html.Raw(value);
            return new HtmlString($"'{rawContent}'");
        }

        public static HtmlString AntiForgeryTokenTag(this IHtmlHelper html)
        {
            var htmlToken = html.AntiForgeryToken();
            var property = htmlToken.GetType().GetField("_requestToken", BindingFlags.NonPublic | BindingFlags.Instance);
            var token = property?.GetValue(htmlToken);
            return new HtmlString($"<script type='text/javascript'>const aftoken='{token}';</script>");
        }

        public static HtmlString DeclareJavaScriptConstant(this IHtmlHelper html, string name, object value)
        {
            return new HtmlString($"<script type='text/javascript'>const {name}='{value}';</script>");
        }
    }
}
