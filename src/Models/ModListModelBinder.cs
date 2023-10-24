﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;

namespace vibrio.src.Models {
    public class ModListModelBinder : IModelBinder {
        private readonly Ruleset ruleset;

        public ModListModelBinder(Ruleset ruleset) {
            this.ruleset = ruleset;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext) {
            var result = bindingContext.ValueProvider.GetValue("mods");
            var mods = result.AsEnumerable<string>().Select(acronym => ruleset.CreateModFromAcronym(acronym)).ToArray();
            bindingContext.Result = ModelBindingResult.Success(mods);
            return Task.CompletedTask;
        }
    }
}
