using osu.Framework.Bindables;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public abstract class Cherry : BosuHitObject, IHasComboInformation
    {
        public double TimePreempt { get; set; } = 400;

        public virtual bool NewCombo { get; set; }

        private HitObjectProperty<int> comboOffset;

        public Bindable<int> ComboOffsetBindable => comboOffset.Bindable;

        public int ComboOffset
        {
            get => comboOffset.Value;
            set => comboOffset.Value = value;
        }

        private HitObjectProperty<int> indexInCurrentCombo;

        public Bindable<int> IndexInCurrentComboBindable => indexInCurrentCombo.Bindable;

        public virtual int IndexInCurrentCombo
        {
            get => indexInCurrentCombo.Value;
            set => indexInCurrentCombo.Value = value;
        }

        private HitObjectProperty<int> comboIndex;

        public Bindable<int> ComboIndexBindable => comboIndex.Bindable;

        public virtual int ComboIndex
        {
            get => comboIndex.Value;
            set => comboIndex.Value = value;
        }

        private HitObjectProperty<int> comboIndexWithOffsets;

        public Bindable<int> ComboIndexWithOffsetsBindable => comboIndexWithOffsets.Bindable;

        public int ComboIndexWithOffsets
        {
            get => comboIndexWithOffsets.Value;
            set => comboIndexWithOffsets.Value = value;
        }

        private HitObjectProperty<bool> lastInCombo;

        public Bindable<bool> LastInComboBindable => lastInCombo.Bindable;

        public bool LastInCombo
        {
            get => lastInCombo.Value;
            set => lastInCombo.Value = value;
        }
    }
}
