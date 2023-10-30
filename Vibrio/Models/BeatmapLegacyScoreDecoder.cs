using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Scoring.Legacy;
using Vibrio.Exceptions;

namespace Vibrio.Models {
    public class BeatmapLegacyScoreDecoder : LegacyScoreDecoder {
        private readonly WorkingBeatmap beatmap;
        private readonly List<RulesetInfo> rulesets;

        public BeatmapLegacyScoreDecoder(WorkingBeatmap beatmap) {
            this.beatmap = beatmap;
            rulesets = new List<RulesetInfo> {
                new OsuRuleset().RulesetInfo
            };
        }

        protected override WorkingBeatmap GetBeatmap(string md5Hash) {
            if (beatmap.BeatmapInfo.Hash != md5Hash) {
                throw new BeatmapMismatchException("Provided beatmap does not match map used in replay");
            }

            return beatmap;
        }

        protected override Ruleset GetRuleset(int rulesetId) {
            var info = rulesets.FirstOrDefault(r => r.OnlineID == rulesetId)
                ?? throw new UnsupportedRulesetException($"No ruleset found for id {rulesetId}");
            return info.CreateInstance();
        }
    }
}
