﻿namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DisableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DISABLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Disables a feature for the entire round.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("key", typeof(string), "The key of the feature to disable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string key = Arguments[0].ToUpper();

            if (!MainPlugin.Handlers.DisabledKeys.Contains(key))
                MainPlugin.Handlers.DisabledKeys.Add(key);

            return new(true);
        }
    }
}
