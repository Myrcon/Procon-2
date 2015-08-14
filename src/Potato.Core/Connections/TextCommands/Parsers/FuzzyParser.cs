﻿#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Potato.Core.Connections.TextCommands.Parsers.Fuzzy;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Fuzzy;
using Potato.Fuzzy.Tokens.Object;
using Potato.Fuzzy.Tokens.Primitive;
using Potato.Fuzzy.Tokens.Primitive.Numeric;
using Potato.Fuzzy.Tokens.Primitive.Temporal;
using Potato.Fuzzy.Utils;
using Potato.Net.Shared.Models;

namespace Potato.Core.Connections.TextCommands.Parsers {
    /// <summary>
    /// Finds matches agaisnt text with no structure. Extracts various information from the text
    /// </summary>
    public class FuzzyParser : Parser, IFuzzyState {
        /// <summary>
        /// The document to use for localization purposes. This is a raw format to be used
        /// by Potato.Fuzzy
        /// </summary>
        public JObject Document { get; set; }

        /// <summary>
        /// Dictionary of cached property info fetches. Minor optimization.
        /// </summary>
        protected Dictionary<string, PropertyInfo> PropertyInfoCache = new Dictionary<string, PropertyInfo>();

        protected int MinimumSimilarity(int lower, int upper, int maximumLength, int itemLength) {
            return lower + (upper - lower) * (itemLength / maximumLength);
        }

        protected void ParseMapNames(Phrase phrase) {
            var mapNames = Connection.ProtocolState.MapPool.Values.Select(map => new {
                map,
                Similarity = Math.Max(map.FriendlyName.DePluralStringSimularity(phrase.Text), map.Name.DePluralStringSimularity(phrase.Text))
            })
            .Where(@t => @t.Similarity >= 60)
            .Select(@t => new ThingObjectToken() {
                Reference = new MapThingReference() {
                    Maps = new List<MapModel>() {
                        @t.map
                    }
                },
                Text = phrase.Text,
                Similarity = @t.Similarity,
                MinimumWeightedSimilarity = 60
            });

            var names = new List<Token>();
            mapNames.ToList().ForEach(names.Add);

            phrase.AppendDistinctRange(names);
        }

        protected void ParsePlayerNames(Phrase phrase) {

            // We should cache this some where.
            var maximumNameLength = Connection.ProtocolState.Players.Count > 0 ? Connection.ProtocolState.Players.Values.Max(player => player.Name.Length) : 0;

            var playerNames = Connection.ProtocolState.Players.Values.Select(player => new {
                player,
                Similarity = Math.Max(player.NameStripped.DePluralStringSimularity(phrase.Text), player.Name.DePluralStringSimularity(phrase.Text))
            })
            .Where(@t => @t.Similarity >= MinimumSimilarity(55, 70, maximumNameLength, @t.player.Name.Length))
            .Select(@t => new ThingObjectToken() {
                Reference = new PlayerThingReference() {
                    Players = new List<PlayerModel>() {
                        @t.player
                    }
                },
                Text = phrase.Text,
                Similarity = @t.Similarity,
                MinimumWeightedSimilarity = 55
            });

            var names = new List<Token>();
            playerNames.ToList().ForEach(names.Add);

            phrase.AppendDistinctRange(names);
        }

        protected void ParseCountryNames(Phrase phrase) {
            var playerCountries = Connection.ProtocolState.Players.Values.Select(player => new {
                player,
                Similarity = player.Location.CountryName.StringSimularitySubsetBonusRatio(phrase.Text)
            })
            .Where(@t => @t.Similarity >= 60)
            .Select(@t => new ThingObjectToken() {
                Reference = new LocationThingReference() {
                    Locations = new List<Location>() {
                        @t.player.Location
                    }
                },
                Text = phrase.Text,
                Similarity = @t.Similarity,
                MinimumWeightedSimilarity = 60
            });

            var names = new List<Token>();
            playerCountries.ToList().ForEach(names.Add);

            phrase.AppendDistinctRange(names);
        }

        protected void ParseItemNames(Phrase phrase) {
            // Select all items that match our phrase. We don't deal with
            // items individually as many items share many tags, so you'll always need to deal
            // with them as sets.
            var items = Connection.ProtocolState.Items.Values.Select(item => new {
                item,
                Similarity = Math.Max(item.FriendlyName.StringSimularitySubsetBonusRatio(phrase.Text), item.Tags.Select(tag => tag.StringSimularitySubsetBonusRatio(phrase.Text)).Max())
            }).Where(@t => @t.Similarity >= 60)
            .ToList();

            // We have at least one matching item, add it as a token.
            if (items.Any()) {
                phrase.AppendDistinctRange(new List<Token>() {
                    new ThingObjectToken() {
                        Reference = new ItemThingReference() {
                            Items = items.Select(item => item.item).ToList()
                        },
                        Text = phrase.Text,
                        Similarity = items.Max(item => item.Similarity),
                        MinimumWeightedSimilarity = 60
                    }
                });
            }
        }

        public Phrase ParseThing(IFuzzyState state, Phrase phrase) {

            if (phrase.Any()) {
                var thing = phrase.First() as ThingObjectToken;

                if (thing != null) {
                    if (thing.Name == "Players") {
                        thing.Reference = new PlayerThingReference() {
                            Players = new List<PlayerModel>(Connection.ProtocolState.Players.Values)
                        };
                    }
                    else if (thing.Name == "Maps") {
                        thing.Reference = new MapThingReference() {
                            Maps = new List<MapModel>(Connection.ProtocolState.MapPool.Values)
                        };
                    }
                }
            }

            ParsePlayerNames(phrase);
            ParseMapNames(phrase);
            ParseCountryNames(phrase);
            ParseItemNames(phrase);

            return phrase;
        }

        private float MaximumLevenshtein(string argument, List<string> commands) {
            var max = 0.0F;

            commands.ForEach(x => max = Math.Max(max, x.StringSimularitySubsetBonusRatio(argument)));

            return max;
        }

        public Phrase ParseMethod(IFuzzyState state, Phrase phrase) {
            var methods = TextCommands.Select(textCommand => new {
                textCommand,
                similarity = MaximumLevenshtein(phrase.Text, textCommand.Commands)
            })
            .Where(@t => @t.similarity >= 60)
            .Select(@t => new MethodObjectToken() {
                MethodName = @t.textCommand.PluginCommand,
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 60
            });

            var names = new List<Token>();
            methods.ToList().ForEach(names.Add);
            phrase.AppendDistinctRange(names);

            return phrase;
        }

        public Phrase ParseProperty(IFuzzyState state, Phrase phrase) {
            // Edit each NumericPropertyObjectToken
            foreach (NumericPropertyObjectToken token in phrase.Where(token => token is NumericPropertyObjectToken)) {
                if (token.Name == "Ping") {
                    token.Reference = new PingPropertyReference();
                }
                else if (token.Name == "Score") {
                    token.Reference = new ScorePropertyReference();
                }
                else if (token.Name == "Kills") {
                    token.Reference = new KillsPropertyReference();
                }
                else if (token.Name == "Deaths") {
                    token.Reference = new DeathsPropertyReference();
                }
                else if (token.Name == "Kdr") {
                    token.Reference = new KdrPropertyReference();
                }
            }

            return phrase;
        }

        /// <summary>
        /// Finds the player object of the speaker to reference "me"
        /// </summary>
        /// <param name="state"></param>
        /// <param name="selfThing"></param>
        /// <returns></returns>
        public SelfReflectionThingObjectToken ParseSelfReflectionThing(IFuzzyState state, SelfReflectionThingObjectToken selfThing) {
            if (SpeakerPlayer != null) {
                selfThing.Reference = new PlayerThingReference() {
                    Players = new List<PlayerModel>() {
                        SpeakerPlayer
                    }
                };
            }

            return selfThing;
        }

        private List<TextCommandModel> ExtractCommandList(Sentence sentence) {

            // We need to know this method ahead of time so we can clear all other tokens in this phrase.
            var mainMethod = sentence.ExtractFirstOrDefault<MethodObjectToken>();
            var resultMethodList = new List<MethodObjectToken>();

            foreach (var phrase in sentence) {

                if (phrase.Count > 0 && phrase[0] == mainMethod) {
                    // Only bubble up very close matching arguments.
                    phrase.RemoveAll(token => token.Similarity < 80);
                }

                // Select them as good alternatives 
                resultMethodList.AddRange(phrase.OfType<MethodObjectToken>().ToList());

                // Then remove them for the remainder of the execution.
                phrase.RemoveAll(token => token is MethodObjectToken);
            }

            resultMethodList = resultMethodList.OrderByDescending(token => token.Similarity)
                                               .ThenByDescending(token => token.Text.Length)
                                               .ToList();

            var results = resultMethodList.Select(method => TextCommands.Find(command => command.PluginCommand == method.MethodName)).Where(command => command != null).ToList();

            return results.OrderByDescending(token => token.Priority).ToList();
        }

        /// <summary>
        /// Extracts a list of things from the sentence, combining sets and loose things.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public List<T> ExtractThings<T>(Sentence sentence) where T : IThingReference {
            var things = sentence.ScrapeStrictList<ThingObjectToken>().Where(token => token.Reference is T).Select(token => token.Reference).Cast<T>().ToList();

            things.AddRange(sentence.ScrapeStrictList<SelfReflectionThingObjectToken>().Where(token => token.Reference is T).Select(token => token.Reference).Cast<T>());
            
            return things;
        }

        protected TextCommandIntervalModel ExtractTextCommandInterval(Sentence sentence) {
            TextCommandIntervalModel interval = null;

            var pattern = sentence.ExtractList<TemporalToken>().Where(token => token.Pattern != null && token.Pattern.Modifier == TimeModifier.Interval)
                                           .Select(token => token.Pattern)
                                           .FirstOrDefault();

            if (pattern != null) {
                interval = new TextCommandIntervalModel() {
                    Day = pattern.Day,
                    DayOfWeek = pattern.DayOfWeek,
                    Hour = pattern.Hour,
                    IntervalType = (TextCommandIntervalType) Enum.Parse(typeof (TextCommandIntervalType), pattern.TemporalInterval.ToString()),
                    Minute = pattern.Minute,
                    Month = pattern.Month,
                    Second = pattern.Second,
                    Year = pattern.Year,
                };
            }

            return interval;
        }

        public override ICommandResult Parse(string prefix, string text) {
            var sentence = new Sentence().Parse(this, text).Reduce(this);

            ICommandResult result = null;

            var commands = ExtractCommandList(sentence);
            var priorityCommand = commands.FirstOrDefault();

            var quotes = sentence.Where(token => token.Count > 0 && token[0] is StringPrimitiveToken).Select(token => token[0].Text).ToList();

            var timeTokens = sentence.ExtractList<TemporalToken>();
            var delay = timeTokens.Where(token => token.Pattern != null && token.Pattern.Modifier == TimeModifier.Delay)
                                        .Select(token => token.Pattern.ToDateTime())
                                        .FirstOrDefault();

            var period = timeTokens.Where(token => token.Pattern != null && (token.Pattern.Modifier == TimeModifier.Period || token.Pattern.Modifier == TimeModifier.None))
                                         .Select(token => token.Pattern.ToTimeSpan())
                                         .FirstOrDefault();
            
            // Must have a method to execute on, the rest is optional.
            if (priorityCommand != null) {
                commands.Remove(priorityCommand);

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = new CommandData() {
                        Players = new List<PlayerModel>() {
                            SpeakerPlayer
                        },
                        TextCommands = new List<TextCommandModel>() {
                            priorityCommand
                        }
                        .Concat(commands)
                        .ToList(),
                        TextCommandMatches = new List<TextCommandMatchModel>() {
                            new TextCommandMatchModel() {
                                Prefix = prefix,
                                Players = ExtractThings<PlayerThingReference>(sentence).SelectMany(thing => thing.Players).ToList(),
                                Maps = ExtractThings<MapThingReference>(sentence).SelectMany(thing => thing.Maps).ToList(),
                                Numeric = sentence.ExtractList<FloatNumericPrimitiveToken>().Select(token => token.ToFloat()).ToList(),
                                Delay = delay,
                                Period = period,
                                Interval = ExtractTextCommandInterval(sentence),
                                Text = text,
                                Quotes = quotes
                            }
                        }
                    }
                };
            }

            return result;
        }
    }
}
