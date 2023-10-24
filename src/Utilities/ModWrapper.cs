using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using System.ComponentModel;

namespace vibrio.src.Utilities {
    [TypeConverter(typeof(ModWrapperConverter))]
    public class ModWrapper {
        public readonly string Acronym;
        public readonly Mod? Mod;

        public ModWrapper(string acronym) {
            Acronym = acronym;
            Mod = new OsuRuleset().CreateModFromAcronym(acronym);
        }

        public ModWrapper(Mod mod) {
            Acronym = mod.Acronym;
            Mod = mod;
        }
    }
}
