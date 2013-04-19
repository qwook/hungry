using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace positron
{
	public class SceneOne : SceneBasicBox
	{
		protected SceneOne ():
			base()
		{
			//UIGroup = new UIElementGroup();
			Scene next_scene = (Scene)Scene.Scenes["SceneTwo"];
			_DoorToNextScene = new Door(Rear, 512 - 68, 0, next_scene);
		}
		protected override void InitializeConnections ()
		{
			Scene next_scene = (Scene)Scene.Scenes["SceneTwo"];
			_DoorToNextScene.Destination = next_scene.DoorToPreviousScene;
			_DoorToNextScene.Destination.Position = _DoorToNextScene.Position;
		}
		protected override void InitializeScene ()
		{
			PerimeterOffsetX = -1;
			PerimeterOffsetY = -1;

			// Store width and height in local variables
			int w_i = (int)ViewWidth;
			int h_i = (int)ViewHeight;

			double xp = TileSize * PerimeterOffsetX;
			double yp = TileSize * PerimeterOffsetY;

			yp += TileSize;

			// Setup background tiles
			var BackgroundTiles = new TileMap (Background, 48, 24, Texture.Get ("sprite_tile_bg_atlas"));
			BackgroundTiles.PositionX = xp - 8 * TileSize;
			BackgroundTiles.PositionY = yp - 4 * TileSize;
			BackgroundTiles.PositionZ = 1.0;
			BackgroundTiles.RandomMap ();
			BackgroundTiles.Build ();

			double floor_sw_dy = -4.0;

			// Stage elements
			var ft0 = new FloorTile (Rear, xp + 0, yp);
			var ft1 = new FloorTile (Rear, xp + TileSize, yp - TileSize * 0.5);

			// Control key indicators (info graphics)
			var a_infogfx = new SpriteBase (Rear, ft1.PositionX, ft1.PositionY + TileSize, Texture.Get ("sprite_infogfx_key_a"));
			var d_infogfx = new SpriteBase (Rear, a_infogfx.PositionX + TileSize, a_infogfx.PositionY, Texture.Get ("sprite_infogfx_key_d")).CenterShift ();

			// Gateways
			var gw1 = new Gateway (Front, xp + TileSize * 4, yp, false);
			var gw2 = new Gateway (Front, xp + TileSize * 10, yp, false);

			var fs00 = new FloorSwitch (Front, xp + TileSize * 3, yp + floor_sw_dy, (sender, e) => {
				bool bstate = (FloorSwitch.SwitchState)e.Info != FloorSwitch.SwitchState.Open;
				gw1.OnAction (e.Self, new ActionEventArgs (bstate, gw1));
				//Console.WriteLine("{0} acted on {1}: {2}", sender, e.Self, e.Info);
			});
			var fs01 = new FloorSwitch (Front, xp + TileSize * 5, yp + floor_sw_dy, (sender, e) => {
				bool bstate = (FloorSwitch.SwitchState)e.Info != FloorSwitch.SwitchState.Open;
				gw1.OnAction (e.Self, new ActionEventArgs (bstate, gw1));
				//Console.WriteLine("{0} acted on {1}: {2}", sender, e.Self, e.Info);
			}, fs00);

			var fs10 = new FloorSwitch (Front, xp + TileSize * 7, yp + floor_sw_dy, (sender, e) => {
				bool bstate = (FloorSwitch.SwitchState)e.Info != FloorSwitch.SwitchState.Open;
				gw2.OnAction (e.Self, new ActionEventArgs (bstate, gw2));
				//Console.WriteLine("{0} acted on {1}: {2}", sender, e.Self, e.Info);
			}, 0.2);

			var fs11 = new FloorSwitch (Front, xp + TileSize * 9 + 14, yp + floor_sw_dy + TileSize * 3, (sender, e) => {
				bool bstate = (FloorSwitch.SwitchState)e.Info != FloorSwitch.SwitchState.Open;
				gw2.OnAction (e.Self, new ActionEventArgs (bstate, gw2));
				//Console.WriteLine("{0} acted on {1}: {2}", sender, e.Self, e.Info);
			}, fs10, 3.0);

			var space_infogfx = new SpriteBase (Rear, xp + fs11.PositionX - 128, yp + fs10.PositionY, Texture.Get ("sprite_infogfx_key_spacebar"));

			fs11.Theta = Math.PI * 0.5;
			var fs12 = new FloorSwitch (Front, xp + TileSize * 12, yp + floor_sw_dy, (sender, e) => {
				bool bstate = (FloorSwitch.SwitchState)e.Info != FloorSwitch.SwitchState.Open;
				gw2.OnAction (e.Self, new ActionEventArgs (bstate, gw2));
				//Console.WriteLine("{0} acted on {1}: {2}", sender, e.Self, e.Info);
			}, fs11);

			// Walls above gateways
			for (int i = 0; i < 5; i++) {
				new FloorTile (Rear, gw1.CornerX, gw1.CornerY + (2 + i) * TileSize + 8);
				new FloorTile (Rear, gw2.CornerX, gw2.CornerY + (2 + i) * TileSize + 8);
			}
			
			//var TestDialog = new Dialog(HUD, "Test", null);
			//TestDialog.RenderSet = HUD;
			//TestDialog.PauseTime = 1.0f;

			base.InitializeScene ();

		}
	}
}

