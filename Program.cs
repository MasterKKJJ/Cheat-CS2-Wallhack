using ModMenuCS2;
using Swed64;
using System.Numerics;
Swed swed = new Swed("cs2");
//int time_ct = 3;
//int time_t = 2;
IntPtr client = swed.GetModuleBase("client.dll");
Renderer renderer = new Renderer();
Thread thread = new Thread(async () => await renderer.Start());
thread.Start();
Vector2 screenSize = renderer.screenSize2;
List<Entity> entities = new List<Entity>();
Entity Player = new Entity();
ViewMatrix pegarViewMatrix()
{
	ViewMatrix vmatrix = new ViewMatrix();
	float[] mtx = swed.ReadMatrix(client + Offsets.dwViewMatrix);
	if (mtx.Length >= 16) // Certifique-se de que a matriz foi lida corretamente
	{
		vmatrix.m11 = mtx[0]; vmatrix.m12 = mtx[1]; vmatrix.m13 = mtx[2]; vmatrix.m14 = mtx[3];
		vmatrix.m21 = mtx[4]; vmatrix.m22 = mtx[5]; vmatrix.m23 = mtx[6]; vmatrix.m24 = mtx[7];
		vmatrix.m31 = mtx[8]; vmatrix.m32 = mtx[9]; vmatrix.m33 = mtx[10]; vmatrix.m34 = mtx[11];
		vmatrix.m41 = mtx[12]; vmatrix.m42 = mtx[13]; vmatrix.m43 = mtx[14]; vmatrix.m44 = mtx[15];
	}
	return vmatrix;
}
while (true)
{
	ViewMatrix viewMatrix = pegarViewMatrix();
	entities.Clear(); // Clear the entities list at the beginning of each iteration
	IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);
	IntPtr listEntry = swed.ReadPointer(entityList, 0x10);
	IntPtr localPlayerSpawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
	Player.Team = swed.ReadInt(localPlayerSpawn, Offsets.m_iTeamNum);
	Player.Health = swed.ReadInt(localPlayerSpawn, Offsets.m_iHealth);
	Player.Position = swed.ReadVec(localPlayerSpawn, Offsets.m_vOldOrigin);
	Player.ViewOffset = swed.ReadVec(localPlayerSpawn, Offsets.m_vecViewOffset);
	Player.ViewAngles = swed.ReadVec(client, Offsets.dwViewAngles);
	
	// Console.WriteLine(Player.ViewAngles);
	Player.Address = localPlayerSpawn;
	Player.Position2D = Renderer.WorldToScreen(viewMatrix, Player.Position, screenSize);
	Player.viewPosition2D = Renderer.WorldToScreen(viewMatrix, Vector3.Add(Player.Position, Player.ViewOffset), screenSize);

	for (int i = 0; i < 64; i++)
	{
		IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
		if (currentController == IntPtr.Zero) { continue; }

		int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);

		if (pawnHandle == 0) { continue; }
		IntPtr listentry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

		if (listentry2 == IntPtr.Zero) { continue; }
		IntPtr currentSpawn = swed.ReadPointer(listentry2, 0x78 * (pawnHandle & 0x1FF));
		if (currentSpawn == IntPtr.Zero) { continue; }
		if (currentSpawn == localPlayerSpawn) { continue; }
		int lifestate = swed.ReadInt(currentSpawn, Offsets.m_lifeState);
		if (lifestate != 256) { continue; }

		Entity entity = new Entity();
		entity.ID = i;
		entity.Address = currentSpawn;
		entity.Team = swed.ReadInt(currentSpawn, Offsets.m_iTeamNum);
		entity.Position = swed.ReadVec(currentSpawn, Offsets.m_vOldOrigin);
		entity.Distance = Vector3.Distance(Player.Position, entity.Position);
		entity.Health = swed.ReadInt(currentSpawn, Offsets.m_iHealth);
		entity.ViewOffset = swed.ReadVec(currentSpawn, Offsets.m_vecViewOffset);
		entity.Position2D = Renderer.WorldToScreen(viewMatrix, entity.Position, screenSize);
		entity.viewPosition2D = Renderer.WorldToScreen(viewMatrix, Vector3.Add(entity.Position, entity.ViewOffset), screenSize);
		entities.Add(entity);
		// ...existing code...
	}
	// ...existing code...
	renderer.UpdateLocalPlayer(Player);
	renderer.UpdateEntities(entities);
	// ...existing code...
}