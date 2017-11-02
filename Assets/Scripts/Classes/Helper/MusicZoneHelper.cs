using System;


public class MusicZoneHelper
{
	private bool energyZoneReached;
	private bool calmZoneReached;

	public MusicZoneHelper() {
	}

	public MusicZoneHelper(bool energyZoneReached, bool calmZoneReached) {
		this.energyZoneReached = energyZoneReached;
		this.calmZoneReached = calmZoneReached;

	}

	public bool isEnergyZoneReached() {
		return energyZoneReached;
	}

	public bool isCalmZoneNotReached() {
		return !calmZoneReached;
	}

}

