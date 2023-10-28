using Microsoft.AspNetCore.Mvc.ModelBinding;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;

namespace Vibrio.Models {
    public class ModListModelBinderProvider : IModelBinderProvider {
        private readonly Ruleset ruleset;

        public ModListModelBinderProvider(Ruleset ruleset) {
            this.ruleset = ruleset;
        }

        public IModelBinder? GetBinder(ModelBinderProviderContext context) {
            if (context.Metadata.ModelType == typeof(Mod[])) {
                return new ModListModelBinder(ruleset);
            }
            return null;
        }
    }
}
