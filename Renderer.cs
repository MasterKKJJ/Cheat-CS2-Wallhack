using ClickableTransparentOverlay;
using ImGuiNET;
using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ModMenuCS2
{

	public class Renderer : Overlay
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);


		//public static Vector2 screenSize = new Vector2(1920, 1080);
		public static Vector2 screenSize = new Vector2(1920, 1080);
		public Vector2 screenSize2 = screenSize;
		//variaveis ImGUI
		public static bool AimBotVariavel = false;
		public static bool tpVariavel = false;
		public static int milisecounds = 0;
		public static bool enableESP = false;
		// entities copy
		public static List<Entity> entities = new List<Entity>();
		public static Entity Player = new Entity();
		private static readonly object lockObj = new object();
		static ImDrawListPtr drawList;
		public static Vector4 enemyColor = new Vector4(1, 0, 0, 1); // vermelho padrao
		public static Vector4 teamColor = new Vector4(0, 0, 1, 1); // vermelho padrao

		public static Swed swed = new Swed("cs2");
		//int time_ct = 3;
		//int time_t = 2;
		public static IntPtr client = swed.GetModuleBase("client.dll");





		public static bool EntityOnScreen(Entity entity)
		{

			return entity.Position2D.X > 0 && entity.Position2D.X < screenSize.X &&
				entity.Position2D.Y > 0 && entity.Position2D.Y < screenSize.Y;


		}

		public Entity getLocalPlayer() { return Player; }




		public static Vector2 WorldToScreen(ViewMatrix matrix, Vector3 pos, Vector2 windowSize)
		{
			float screenW = (matrix.m41 * pos.X) + (matrix.m42 * pos.Y) + (matrix.m43 * pos.Z) + matrix.m44;

			if (screenW > 0.001f)
			{
				float screenX = (matrix.m11 * pos.X) + (matrix.m12 * pos.Y) + (matrix.m13 * pos.Z) + matrix.m14;
				float screenY = (matrix.m21 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m23 * pos.Z) + matrix.m24;

				screenX /= screenW;
				screenY /= screenW;

				// Console.WriteLine($"WorldToScreen: {pos} => Screen: ({screenX}, {screenY})"); // Debug

				float X = (windowSize.X / 2) * (1 + screenX);
				float Y = (windowSize.Y / 2) * (1 - screenY);  // Inversão do eixo Y

				return new Vector2(X, Y);
			}

			return new Vector2(-1, -1); // Posição inválida
		}

		public static void DrawLine(Entity entity)
		{
			Vector4 lineColor = (Player.Team == entity.Team) ? teamColor : enemyColor;
			Vector2 screenCenter = new Vector2(screenSize.X / 2, screenSize.Y);
			//Console.WriteLine(lineColor);
			//Console.WriteLine(entity.Team);

			// Verifique se a posição da entidade é válida antes de desenhar a linha
			if (entity.Position2D.X >= 0 && entity.Position2D.Y >= 0)
			{
				uint color1 = ImGui.ColorConvertFloat4ToU32(lineColor);

				drawList.AddLine(screenCenter, entity.Position2D, color1);
			}
		}

		public static void DrawBox(Entity entity)
		{
			float entityHeight = entity.Position2D.Y - entity.viewPosition2D.Y;
			float boxWidth = entityHeight / 3;

			// Ajuste as posições para garantir que a caixa seja visível
			Vector2 boxTopLeft = new Vector2(Math.Max(entity.viewPosition2D.X - boxWidth, 0),
											 Math.Max(entity.viewPosition2D.Y, 0));
			Vector2 boxBottomRight = new Vector2(Math.Min(entity.viewPosition2D.X + boxWidth, screenSize.X),
												 Math.Min(entity.viewPosition2D.Y + entityHeight, screenSize.Y));

			// Verifique novamente se a caixa está dentro da tela
			if (boxTopLeft.X >= 0 && boxTopLeft.Y >= 0 && boxBottomRight.X <= screenSize.X && boxBottomRight.Y <= screenSize.Y)
			{
				Vector4 boxColor = (Player.Team == entity.Team) ? teamColor : enemyColor;
				uint color1 = ImGui.ColorConvertFloat4ToU32(boxColor);
				drawList.AddRect(boxTopLeft, boxBottomRight, color1);
			}
		}





		public void UpdateEntities(List<Entity> Newentities)
		{

			entities = new List<Entity>(Newentities);
			Newentities.Clear();
		}

		static void DrawOverlay(Vector2 screenSize)
		{

			ImGui.SetNextWindowSize(screenSize);
			ImGui.SetWindowPos(new Vector2(0, 0));
			ImGui.Begin("overlay",
			  ImGuiWindowFlags.NoDecoration
			| ImGuiWindowFlags.NoBackground
			| ImGuiWindowFlags.NoBringToFrontOnFocus
			| ImGuiWindowFlags.NoMove
			| ImGuiWindowFlags.NoCollapse
			| ImGuiWindowFlags.NoScrollbar
			| ImGuiWindowFlags.NoScrollWithMouse
			| ImGuiWindowFlags.NoInputs

);

		}

		public void UpdateLocalPlayer(Entity newEntity)
		{
			lock (lockObj)
			{
				Player = newEntity;
			}
		}



		private float GetDistanceFromCrosshair(Entity entity)
		{
			Vector2 screenCenter = new Vector2(screenSize.X, screenSize.Y);
			return Vector2.Distance(screenCenter, entity.Position2D);
		}

		protected override void Render()
		{

			// MaximizeOverlay();


			ImGui.Begin("Cheat CS2");
			ImGui.Checkbox("Aimbot", ref AimBotVariavel);
			ImGui.Checkbox("Ativar ESP", ref enableESP);
			if (ImGui.CollapsingHeader("Team Color"))
			{
				ImGui.ColorPicker4("##teamcolor", ref teamColor);
			}
			if (ImGui.CollapsingHeader("Enemy Color"))
			{
				ImGui.ColorPicker4("##enemycolor", ref enemyColor);
			}


			// draw overlay



			DrawOverlay(screenSize);
			drawList = ImGui.GetWindowDrawList();
			if (enableESP)
			{
				foreach (Entity entity in entities)
				{
					// checar se esta na tela
					// Console.WriteLine("EntityOnScreen Deu: " + EntityOnScreen(entity));
					if (EntityOnScreen(entity))
					{
						DrawBox(entity);
						DrawLine(entity);
					}

				}
			}

			if (AimBotVariavel)
			{

				// Ordena os inimigos pelo mais próximo da mira
				List<Entity> localEntities = entities;
				
				localEntities = localEntities.OrderBy(o => o.Distance).ToList();

				// Loop usando for para pegar apenas o primeiro inimigo válido


				if (EntityOnScreen(localEntities[0]))
				{
					AimBot.AimBot2(Renderer.swed, Renderer.client, localEntities[0], Player);

				}

			}


			ImGui.End();

		}
	}
}