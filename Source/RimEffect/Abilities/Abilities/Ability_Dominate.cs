﻿namespace RimEffect
{
    using Verse;

    public class Ability_Dominate : Ability
    {
        public override bool ValidateTarget(LocalTargetInfo target) => 
            base.ValidateTarget(target) && target.Pawn.RaceProps.IsFlesh;

        public override void Cast(LocalTargetInfo target)
        {
            if(target.Pawn.mindState.mentalStateHandler.TryStartMentalState(RE_DefOf.RE_DominationBerserk, "RE.AbilityDominateBerserkReason".Translate(this.pawn.NameShortColored), forceWake: true))
                base.Cast(target);
        }

        public override int GetDurationForPawn() => RE_DefOf.RE_DominationBerserk.minTicksBeforeRecovery + RE_DefOf.RE_DominationBerserk.maxTicksBeforeRecovery - RE_DefOf.RE_DominationBerserk.minTicksBeforeRecovery;
    }
}