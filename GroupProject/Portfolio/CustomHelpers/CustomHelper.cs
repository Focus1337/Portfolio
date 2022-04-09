using System.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Portfolio.CustomHelpers;

public static class CustomHelper
{
    // Indian version
    public static HtmlString SubmitButton(this IHtmlHelper htmlHelper, string name, object value, string attributes)
    {
        var str = $"<button type='submit' class='{attributes}' name='{name}'>{value}</button>";
        return new HtmlString(str);
    }
    
}