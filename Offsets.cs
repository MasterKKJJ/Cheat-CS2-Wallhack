namespace ModMenuCS2
{
	public static class Offsets
	{

		// OFFSETS NORMAIS client.dll

		public static readonly int dwEntityList = 0x1A292F0;
		public static readonly int dwViewMatrix = 0x1A942F0;
		public static readonly int dwLocalPlayerPawn = 0x187CEF0;
		public static readonly int m_iHealth = 0x344;
		public static readonly int dwViewAngles = 0x1A9E400;
		// client.dll.cs
		public static readonly int m_vOldOrigin = 0x1324; // Vector
		public static readonly int m_iTeamNum = 0x3E3; // uint8
		public static readonly int m_lifeState = 0x348; // uint8
		public static readonly int m_hPlayerPawn = 0x80C; // CHandle<C_CSPlayerPawn>
		public static readonly int m_vecViewOffset = 0xCB0; // CNetworkViewOffsetVector
	}
}
