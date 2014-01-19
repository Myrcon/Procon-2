﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteGroupingList {

        private static FrostbitePlayerSubsetContext GetSubsetContext(String context) {
            FrostbitePlayerSubsetContext result = FrostbitePlayerSubsetContext.All;

            try {
                result = (FrostbitePlayerSubsetContext) Enum.Parse(typeof (FrostbitePlayerSubsetContext), context, true);
            }
            catch {
                // If any errors occur, default to all players.
                result = FrostbitePlayerSubsetContext.All;
            }

            return result;
        }

        public static List<GroupingModel> Parse(List<String> words) {
            List<GroupingModel> groupings = new List<GroupingModel>();

            if (words.Count >= 1) {

                FrostbitePlayerSubsetContext context = FrostbiteGroupingList.GetSubsetContext(words[0]);

                if (words.Count >= 2) {
                    int parsedTeamId = 0;

                    if (context == FrostbitePlayerSubsetContext.Player) {
                        groupings.Add(new GroupingModel() {
                            Type = GroupingModel.Player,
                            Uid = words[1]
                        });
                    }
                    else if (context == FrostbitePlayerSubsetContext.Team && int.TryParse(words[1], out parsedTeamId) == true) {
                        groupings.Add(new GroupingModel() {
                            Type = GroupingModel.Team,
                            Uid = parsedTeamId.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                    else if (words.Count >= 3) {
                        int parsedSquadId = 0;

                        if (context == FrostbitePlayerSubsetContext.Squad && int.TryParse(words[1], out parsedTeamId) == true && int.TryParse(words[2], out parsedSquadId) == true) {
                            groupings.Add(new GroupingModel() {
                                Type = GroupingModel.Team,
                                Uid = parsedTeamId.ToString(CultureInfo.InvariantCulture)
                            });
                            groupings.Add(new GroupingModel() {
                                Type = GroupingModel.Squad,
                                Uid = parsedSquadId.ToString(CultureInfo.InvariantCulture)
                            });
                        }
                    }
                }
            }

            return groupings;
        }

    }
}
