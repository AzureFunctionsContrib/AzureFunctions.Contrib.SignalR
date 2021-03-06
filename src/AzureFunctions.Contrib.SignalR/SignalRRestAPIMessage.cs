﻿using Newtonsoft.Json;

namespace AzureFunctions.Contrib.SignalR
{
    /// <summary>
    /// SignalR rest api message
    /// </summary>
    internal class SignalRRestAPIMessage
    {
        [JsonProperty("target")]
        internal string Target { get; set; }

        [JsonProperty("arguments")]
        internal object[] Arguments { get; set; }
    }
}