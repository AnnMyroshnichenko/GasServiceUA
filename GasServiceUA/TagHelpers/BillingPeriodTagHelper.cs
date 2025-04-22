using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace GasServiceUA.TagHelpers
{
    public class BillingPeriodTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            output.TagName = "span";
            output.Content.SetContent($"{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
        }
    }
}
