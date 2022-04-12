using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System.Net.Http;
using PlayFab.Plugins.CloudScript;
using System.Collections.Generic;
using PlayFab.ServerModels;
using PlayFab;

namespace Company.Function
{
    public static class HelloWorld
    {
        [FunctionName("HelloWorld")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            var internalData = await Get();
            var message = internalData;//$"  internalData[Message] = {internalData["Message"]}";

            return new { messageValue = message };
        }

        [HttpGet]
        public static async Task<Dictionary<string, string>> Get()
        {
            PlayFabSettings.staticSettings.TitleId = "CB19E";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "MIEYF8EBBKUUXPQHKZ79UR5CQ9MJ54OOPKAANF494BMI1ZATQ9";

            var request = new GetTitleDataRequest();
            var titleInternalData = await PlayFabServerAPI.GetTitleInternalDataAsync(request);

            return titleInternalData.Result.Data;
        }
    }
}
