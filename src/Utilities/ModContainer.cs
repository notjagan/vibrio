using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace vibrio.src.Utilities {
    [TypeConverter(typeof(ModContainerConverter))]
    public class ModContainer {
        public readonly string Acronym;
        public readonly Mod? Mod;

        public ModContainer(string acronym) {
            Acronym = acronym;
            Mod = new OsuRuleset().CreateModFromAcronym(acronym);
        }

        public ModContainer(Mod mod) {
            Acronym = mod.Acronym;
            Mod = mod;
        }
    }
}
