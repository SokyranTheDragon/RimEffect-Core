﻿namespace RimEffect
{
    using UnityEngine;
    using Verse;

    public class Ability_SpawnBuilding : Ability
    {
        public override float Chance => 0f;

        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            AbilityExtension_Building extension = this.def.GetModExtension<AbilityExtension_Building>();

            if (extension.building != null)
            {
                Thing building = GenSpawn.Spawn(extension.building, target.Cell, this.pawn.Map);
                building.SetFaction(this.pawn.Faction);

                CompSpawnedBuilding comp = building.TryGetComp<CompSpawnedBuilding>();
                if (comp != null)
                {
                    comp.lastDamageTick = Find.TickManager.TicksGame;
                    comp.damagePerTick  = Mathf.RoundToInt(this.GetPowerForPawn());

                    int duration = this.GetDurationForPawn();
                    if (duration > 0)
                        comp.finalTick = comp.lastDamageTick + duration;
                }
            }
        }
    }

    public class AbilityExtension_Building : DefModExtension
    {
        public ThingDef building;
    }

    public class CompSpawnedBuilding : ThingComp
    {
        public int finalTick      = -1;
        public int damagePerTick  = 0;
        public int lastDamageTick = 0;


        public override void CompTick()
        {
            base.CompTick();
            this.TickCheck();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            this.TickCheck();
        }

        public override void CompTickLong()
        {
            base.CompTickLong();
            this.TickCheck();
        }

        private void TickCheck()
        {
            int  ticksGame = Find.TickManager.TicksGame;
            bool destroy   = false;


            if (this.damagePerTick > 0 && this.lastDamageTick < ticksGame)
            {
                this.parent.HitPoints -= this.damagePerTick * (ticksGame - this.lastDamageTick);
                this.lastDamageTick   =  ticksGame;

                if (this.parent.HitPoints <= 0)
                    destroy = true;
            }

            if (this.finalTick > 0)
                if (this.finalTick < ticksGame)
                    destroy = true;

            if (destroy)
                this.parent.Destroy();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref this.finalTick, nameof(this.finalTick));
            Scribe_Values.Look(ref this.damagePerTick, nameof(this.damagePerTick));
            Scribe_Values.Look(ref this.lastDamageTick, nameof(this.lastDamageTick));
        }
    }
}
