using BlazorLocalizerSample.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorLocalizerSample.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LocalizationController : ControllerBase
    {

        [HttpGet]
        public JsonResult GetLocalizedCategory([FromQuery] string category, [FromQuery] string culture)
        {
            // Fetch all localized values from a database for the given category and culture

            // For this sample, we simply mock the data and split it into some categories.

            var localizedValues = new List<KeyValue>();

            if (category == "BlazorLocalizerSample.GlobalTexts")
            {
                localizedValues.Add(new KeyValue { Key = "About", Value = "About" });
                localizedValues.Add(new KeyValue { Key = "Home", Value = "Home" });
                localizedValues.Add(new KeyValue { Key = "Loading...", Value = "Loading..." });
            }

            if (category == "BlazorLocalizerSample.Client.Pages.Index")
            {
                localizedValues.Add(new KeyValue { Key = "Hello, world!", Value = "Hello, world!" });
                localizedValues.Add(new KeyValue { Key = "Welcome to your new app.", Value = "Welcome to your new app." });
            }

            if (category == "BlazorLocalizerSample.Client.Pages.Counter")
            {
                localizedValues.Add(new KeyValue { Key = "Counter", Value = "Counter" });
                localizedValues.Add(new KeyValue { Key = "Current count:", Value = "Current count:" });
                localizedValues.Add(new KeyValue { Key = "Click me", Value = "Click me" });
                localizedValues.Add(new KeyValue { Key = "Example of text being localized on a html attribute", Value = "Example of text being localized on a html attribute" });
            }

            if(category == "BlazorLocalizerSample.Client.Pages.FetchData")
            {
                localizedValues.Add(new KeyValue { Key = "Fetch data", Value = "Fetch data" });
                localizedValues.Add(new KeyValue { Key = "This component demonstrates fetching data from the server.", Value = "This component demonstrates fetching data from the server." });
                localizedValues.Add(new KeyValue { Key = "Weather forecast", Value = "Weather forecast" });
                localizedValues.Add(new KeyValue { Key = "Date", Value = "Date" });
                localizedValues.Add(new KeyValue { Key = "Temp. (C)", Value = "Temp. (C)" });
                localizedValues.Add(new KeyValue { Key = "Temp. (F)", Value = "Temp. (F)" });
                localizedValues.Add(new KeyValue { Key = "Summary", Value = "Summary" });
            }

            // In order to see that text has actually been localized, we append the culture information for all localized texts.
            localizedValues.ForEach(x => x.Value += $" ({culture})");

            return new JsonResult(localizedValues);
        }

        [HttpPost]
        public IActionResult AddMissingKey([FromBody] MissingKeyValue model)
        {
            // Save the missing key for a given category + culture. 
            // Please consider the safety implications of doing this! You are handing the user the control to create keys in your database/resource management system!

            return Ok();
        }
    }
}
