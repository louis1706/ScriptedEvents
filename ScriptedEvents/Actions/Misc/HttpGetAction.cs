﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using MEC;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine.Networking;

    public class HttpGetAction : IHelpInfo, ITimingAction
    {
        /// <inheritdoc/>
        public string Name => "HTTPGET";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Sends an HTTP GET request to a website.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("url", typeof(string), "The URL to send a GET request to.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                return null;
            }

            string coroutineKey = $"HTTPGET_COROUTINE_{DateTime.UtcNow.Ticks}";
            CoroutineHandle handle = Timing.RunCoroutine(InternalSendHTTP(script, VariableSystem.ReplaceVariable(Arguments[0], script)), coroutineKey);
            CoroutineHelper.AddCoroutine("HTTPGET", handle, script);
            message = new(true);
            return Timing.WaitUntilDone(handle);
        }

        private IEnumerator<float> InternalSendHTTP(Script script, string input)
        {
            UnityWebRequest discordWWW = UnityWebRequest.Get(input);
            yield return Timing.WaitUntilDone(discordWWW.SendWebRequest());

            string result;
            if (discordWWW.responseCode != 200)
                result = $"ERROR {discordWWW.responseCode}";
            else
                result = discordWWW.downloadHandler.text;

            script.AddVariable("{HTTPSUCCESS}", "Whether or not the result of an HTTP request was successful.", (discordWWW.responseCode == 200).ToString().ToUpper());
            script.AddVariable("{HTTPRESULT}", "The result of a request through the HTTPGET or HTTPPOST actions.", result);
        }
    }
}