﻿using System;
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
    public class SceneThree : Scene, ISceneGameplay
    {
		protected int PerimeterOffsetX = 12;
		protected int PerimeterOffsetY = 6;
		protected int PerimeterX = 18;
		protected int PerimeterY = 4;
		protected int Perimeter2X = 92;
		protected int Perimeter2Y = 8;

		protected double TileSize = 32;
		protected SceneThree ():
			base()
		{
			SceneEntry += (sender, e) => {
				var stanzas = new List<DialogStanza>();
				DialogSpeaker speaker = DialogSpeaker.Get("protagonist");
				stanzas.Add(new DialogStanza(speaker, "This room is eerily empty..."));
				var dialog = new Dialog(e.To.HUD, "Dialog", stanzas);
				dialog.Begin();
			};
		}
		protected override void InstantiateConnections()
		{
			_DoorToPreviousScene = new Door(Rear, 0, 0);
		}
        protected override void InitializeScene ()
		{
			// Basic perimeter:
			double x0 = PerimeterOffsetX * TileSize;
			double y0 = PerimeterOffsetY * TileSize;

			double recess_switch = -4.0;
			int chute_right = 3;
			
			Gateway gw_chute = null;
			FloorSwitch fs_chute = null;

			for (int i = 0; i < PerimeterX; i++) {
				if (i == PerimeterX - chute_right) {
					double x = x0 + TileSize * i;

					fs_chute = new FloorSwitch (Front, x - 2.0 * TileSize, y0 + TileSize + recess_switch, (sender, e) => {
						bool bstate = (FloorSwitch.SwitchState)e.Info != FloorSwitch.SwitchState.Open;
						gw_chute.OnAction (e.Self, new ActionEventArgs (bstate, gw_chute)); }, 2.0);

					gw_chute = new SmallGateway (Front, x, y0 + TileSize, false);
					gw_chute.CenterShift ();
					gw_chute.PositionX += 0.5 * TileSize;
					gw_chute.PositionY -= 0.5 * (TileSize - 0.5 * gw_chute.SizeY);
					gw_chute.Theta = -0.5 * Math.PI;

					continue;
				}
				BunkerFloor block = new BunkerFloor2 (this, x0 + TileSize * i, y0);
				block.PositionY -= block.SizeY;
				block = new BunkerFloor (this, x0 + TileSize * i, y0);
			}
			for (int i = 0; i <= PerimeterY; i++) {
				var block = new BunkerWall (this, x0 + TileSize * PerimeterX, y0 + TileSize * i);
				block.TileX = -1.0;
				block = new BunkerWall (this, x0 - 0.5 * TileSize, y0 + TileSize * (PerimeterY - i));
				
			}
			for (int i = 0; i < PerimeterX; i++) {
				var block = new FloorTile (Stage, x0 + TileSize * (PerimeterX - i - 1), y0 + TileSize * PerimeterY);
			}

			// Setup background tiles
			var BackgroundTiles = new TileMap (Background, Perimeter2X, 6, Texture.Get ("sprite_dark_rubble_atlas"));
			BackgroundTiles.CornerX = x0 - 2 * TileSize;
			BackgroundTiles.CornerY = y0;
			BackgroundTiles.PositionZ = 1.0;
			BackgroundTiles.RandomMap ();
			BackgroundTiles.Build ();
			// Store width and height in local variables for easy access
			int w_i = (int)ViewWidth;
			int h_i = (int)ViewHeight;
			
			// X and Y positioner variables
			double xp = TileSize * PerimeterOffsetX;
			double yp = TileSize * PerimeterOffsetY;
			
			yp += TileSize;
			xp += (PerimeterX / 2) * TileSize;
			
			// Set up doors:
			Scene prev_scene = (Scene)Program.MainGame.Scenes ["SceneTwo"];
			_DoorToPreviousScene.Destination = prev_scene.DoorToNextScene;
//			Scene next_scene = (Scene)Scene.Scenes["SceneThree"];
//			_DoorToNextScene.Destination = next_scene.DoorToPreviousScene;
//			_DoorToNextScene.Destination.Position = _DoorToNextScene.Position;

			var pipe_texture = Texture.Get ("sprite_bg_pipes");
			for (int i = 0; i < 5; i++) {
				var bg_pipes =
					new SpriteBase(Background,
					               x0 + pipe_texture.Width * i - TileSize,
					               y0 - pipe_texture.Height + TileSize - 6, pipe_texture);
				bg_pipes.PositionZ = 0.5;
			}

			double y1 = y0 - (Perimeter2Y + 1) * TileSize;

			for (int i = 0; i < Perimeter2X; i++) {
				BunkerFloor block = new BunkerFloor2 (this, x0 + TileSize * i, y1);
				block.PositionY -= block.SizeY;
				block = new BunkerFloor (this, x0 + TileSize * i, y1);
			}
			for (int i = 0; i <= Perimeter2Y; i++) {
				var block = new BunkerWall (this, x0 + TileSize * Perimeter2X, y1 + TileSize * i);
				block.TileX = -1.0;
				block = new BunkerWall (this, x0 - 0.5 * TileSize, y1 + TileSize * (Perimeter2Y - i));
				
			}


			// Lower area stuff
			for(int i = 0; i < 8; i++)
			{
				new BunkerFloor (this, x0 + TileSize * i, y1 + TileSize);
			}
			for(int i = 2; i < 6; i++)
			{
				new BunkerFloor (this, x0 + TileSize * i, y1 + 2 * TileSize);
			}
			for(int i = 20; i < 38; i++)
			{
				new BunkerFloor (this, x0 + TileSize * i, y1 + TileSize);
			}
			for(int i = 22; i < 34; i++)
			{
				new BunkerFloor (this, x0 + TileSize * i, y1 + 2 * TileSize);
			}
			new BunkerFloor (this, x0 + TileSize * 31, y1 + 3.5 * TileSize);
			for(int i = 28; i < 32; i++)
			{
				new BunkerFloor (this, x0 + TileSize * i, y1 + 3 * TileSize);
			}
			for(int i = 42; i < 50; i++)
			{
				for(int j = 2; j < Perimeter2Y; j++)
					new BunkerFloor (this, x0 + TileSize * i, y1 + (j + 0.5) * TileSize);
			}


//			for (int i = 0; i < Perimeter2X; i++) {
//				if (i ==  PerimeterX - chute_right) {
//					continue;
//				}
//				var block = new FloorTile (Stage, x0 + TileSize * i, y1 + TileSize * Perimeter2Y);
//			}

			// Call the base class initializer
			base.InitializeScene();
        }
    }
}

