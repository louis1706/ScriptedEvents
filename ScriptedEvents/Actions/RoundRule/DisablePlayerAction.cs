﻿namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DisablePlayerAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DISABLEPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Disables a feature for the entire round, but only for certain player(s).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to disable for.", true),
            new Argument("key", typeof(string), "The key of the feature to disable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection players, script))
                return new(false, players.Message);

            string key = Arguments[1].ToUpper();
            var rule = MainPlugin.Handlers.GetPlayerDisableRule(key);

            if (rule.HasValue)
            {
                rule.Value.Players.AddRange(players.GetInnerList());
            }
            else
            {
                MainPlugin.Handlers.DisabledPlayerKeys.Add(new(key, players.GetInnerList()));
            }

            return new(true);
        }
    }
}
