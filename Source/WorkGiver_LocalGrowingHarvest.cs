using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;

namespace LocalGrowingFirst
{
	public class WorkGiver_LocalGrowingHarvest : WorkGiver_GrowerHarvest 
	{
		public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
		{	//Adapted from latter half of WorkGiver_Grower PotentialWorkCellsGlobal
			Danger maxDanger = pawn.NormalMaxDanger();
			WorkGiver_Grower.wantedPlantDef = null;
			Zone_Growing growZone = pawn.Map.zoneManager.ZoneAt (pawn.Position) as Zone_Growing;

			if (growZone == null) {	//Try edge cells in pawn facing direction next
				growZone = GenAdj.CellsAdjacentAlongEdge (pawn.Position, pawn.Rotation, new IntVec2 (1, 1), Utilities.EdgeFacingRotation (pawn.Rotation)).
					Select (p => pawn.Map.zoneManager.ZoneAt (p)).OfType<Zone_Growing> ().FirstOrDefault ();

				if (growZone == default(Zone_Growing))
					yield break;
			}

			if (growZone.cells.Count == 0)
			{
				Log.ErrorOnce("Grow zone has 0 cells: " + growZone, -563487);
			}
			else if (this.ExtraRequirements(growZone, pawn))
			{
				if (!growZone.ContainsStaticFire)
				{
					if (pawn.CanReach(growZone.Cells[0], PathEndMode.OnCell, maxDanger, false, TraverseMode.ByPawn))
					{
						for (int k = 0; k < growZone.cells.Count; k++)
						{
							yield return growZone.cells[k];
						}
						WorkGiver_Grower.wantedPlantDef = null;
					}
				}
			}
			WorkGiver_Grower.wantedPlantDef = null;
		}
	}
}

