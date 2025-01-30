using Swed64;
using System.Numerics;

namespace ModMenuCS2
{
	public class AimBot
	{

		public static void AimBot2(Swed swed, IntPtr client, Entity entity, Entity Player)
		{
			if (entity != null && Player.Health > 1)
			{
				// Pega o inimigo mais próximo
				// Calcula os ângulos necessários para mirar
				Vector3 aimAngles = CalculateAngles(Player.Position, entity.Position);

				// var view = swed.ReadVec(compview, 0xC4);
				// Console.WriteLine(view);
				// Escreve os ângulos na memória do jogo
				swed.WriteVec(client, Offsets.dwViewAngles, aimAngles);
				//Console.WriteLine("Arma: "+swed.ReadFloat(Player.Address, 0x34));
				//Console.WriteLine("Arma: "+swed.ReadInt(Player.Address, Offset.Life));
				//swed.WriteInt(Player.Address, Offset.Life);
				// Console.WriteLine($"Mirando em inimigo a {entity.Distance} unidades.");
			}
		}

		public static Vector3 CalculateAngles(Vector3 playerPos, Vector3 enemyPos)
		{
			Vector3 delta = enemyPos - playerPos;

			// Calcula a distância horizontal (plano X-Y)
			float distanceXY = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

			// Calcula o ângulo de Pitch (vertical)
			float pitch = -(float)(Math.Atan2(delta.Z, distanceXY) * (180 / Math.PI));

			// Calcula o ângulo de Yaw (horizontal)
			float yaw = (float)(Math.Atan2(delta.Y, delta.X) * (180 / Math.PI));

			// Normaliza o Yaw para estar entre 0 e 360 graus
			if (yaw < 0) yaw += 360;

			return new Vector3(pitch, yaw, 0);
		}

	}
}