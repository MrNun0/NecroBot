﻿using System;
using System.Linq;
using System.Threading;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;

namespace PoGo.NecroBot.Logic.Tasks
{
    public static class CatchLurePokemonsTask
    {

        public static void Execute(Context ctx, StateMachine machine, FortData currentFortData)
        {
            Logger.Write("Looking for lure pokemon..", LogLevel.Debug);

            var fortId = currentFortData.Id;

            var pokemonId = currentFortData.LureInfo.ActivePokemonId;

            if (ctx.LogicSettings.UsePokemonToNotCatchFilter &&
                ctx.LogicSettings.PokemonsNotToCatch.Contains(pokemonId))
            {
                machine.Fire(new NoticeEvent() { Message = $"Skipped {pokemonId}" });
            }
            else
            {
                var encounterId = currentFortData.LureInfo.EncounterId;
                var encounter = ctx.Client.Encounter.EncounterLurePokemon(encounterId, fortId).Result;

                if (encounter.Result == DiskEncounterResponse.Types.Result.Success)
                {
                    CatchPokemonTask.Execute(ctx, machine, encounter, null, currentFortData, encounterId);
                }
                else
                {
                    machine.Fire(new WarnEvent { Message = $"Encounter problem: Lure Pokemon {encounter.Result}" });
                }
            }
        }
    }
}
