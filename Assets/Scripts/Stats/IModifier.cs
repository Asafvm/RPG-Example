
using System.Collections.Generic;

namespace RPG.Stats {
    public interface IModifier 
    {
        IEnumerable<float> GetAdditiveModifiers(Stat stat);
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}
