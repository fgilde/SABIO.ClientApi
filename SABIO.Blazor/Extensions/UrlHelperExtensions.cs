using Microsoft.AspNetCore.Components;

namespace SABIO.Blazor.Extensions
{
    public static class UrlHelperExtensions
    {
        public static NavigationManager NavigateWhen(this NavigationManager helper, string url, bool condition)
        {
            if(condition)
                helper.NavigateTo(url);
            return helper;
        }
    }
}