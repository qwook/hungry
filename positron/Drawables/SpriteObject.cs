using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace positron
{
	public class SpriteObject: SpriteBase, IWorldObject
	{
		protected int _WorldIndex;
		protected Body _SpriteBody;
		public int WorldIndex { get { return _WorldIndex; } }
		public Body Body {
			get { return _SpriteBody; }
			set { _SpriteBody = value; }
		}
		public Vector3d PositionWorld {
			get {
				return new Vector3d(
					_SpriteBody.Position.X,
					_SpriteBody.Position.Y,
					_Position.Z);
			}
			set {
				_SpriteBody.Position =
					new Microsoft.Xna.Framework.Vector2(
						(float)value.X,
						(float)value.Y);
			}
		}
		public double PositionWorldX {
			get { return (double)Body.Position.X; }
			set { Body.Position = new Microsoft.Xna.Framework.Vector2((float)value, Body.Position.Y); }
		}
		public double PositionWorldY {
			get { return (double)Body.Position.Y; }
			set { Body.Position = new Microsoft.Xna.Framework.Vector2(Body.Position.X, (float)value); }
		}
		public override Vector3d Position {
			get { return PositionWorld * Configuration.MeterInPixels; }
			set { PositionWorld = value / Configuration.MeterInPixels; }
		}
		public override double PositionX {
			get { return (double)Body.Position.X *  Configuration.MeterInPixels; }
			set { Body.Position = new Microsoft.Xna.Framework.Vector2((float)(value / Configuration.MeterInPixels), Body.Position.Y); }
		}
		public override double PositionY {
			get { return (double)Body.Position.Y *  Configuration.MeterInPixels; }
			set { Body.Position = new Microsoft.Xna.Framework.Vector2(Body.Position.X, (float)(value /  Configuration.MeterInPixels)); }
		}
		public Vector3d Corner {
			get { return PositionWorld * Configuration.MeterInPixels - new Vector3d(0.5 * SizeX, 0.5 * SizeY, 0.0); }
			set { PositionWorld = (value + new Vector3d(0.5 * SizeX, 0.5 * SizeY, 0.0)) / Configuration.MeterInPixels; }
		}
		public double CornerX {
			get { return (double)Body.Position.X * Configuration.MeterInPixels - 0.5 * SizeX; }
			set { Body.Position = new Microsoft.Xna.Framework.Vector2((float)((value + 0.5 * SizeX) / Configuration.MeterInPixels), Body.Position.Y); }
		}
		public double CornerY {
			get { return (double)Body.Position.Y * Configuration.MeterInPixels - 0.5 * SizeY; }
			set { Body.Position = new Microsoft.Xna.Framework.Vector2(Body.Position.X, (float)((value + 0.5 * SizeY) /  Configuration.MeterInPixels)); }
		}
		public Vector3d VelocityWorld {
			get {
				return new Vector3d(
					_SpriteBody.LinearVelocity.X,
					_SpriteBody.LinearVelocity.Y,
					_Velocity.Z);
			}
			set {
				_SpriteBody.LinearVelocity =
					new Microsoft.Xna.Framework.Vector2(
						(float)value.X,
						(float)value.Y);
			}
		}
		public double VelocityWorldX {
			get { return (double)Body.LinearVelocity.X; }
			set { Body.LinearVelocity = new Microsoft.Xna.Framework.Vector2((float)value, Body.LinearVelocity.Y); }
		}
		public double VelocityWorldY {
			get { return (double)Body.LinearVelocity.Y; }
			set { Body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(Body.LinearVelocity.X, (float)value); }
		}
		public override Vector3d Velocity {
			get { return VelocityWorld * Configuration.MeterInPixels; }
			set { VelocityWorld = value / Configuration.MeterInPixels; }
		}
		public override double VelocityX {
			get { return (double)Body.LinearVelocity.X *  Configuration.MeterInPixels; }
			set { Body.LinearVelocity = new Microsoft.Xna.Framework.Vector2((float)(value / Configuration.MeterInPixels), Body.LinearVelocity.Y); }
		}
		public override double VelocityY {
			get { return (double)Body.LinearVelocity.Y *  Configuration.MeterInPixels; }
			set { Body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(Body.LinearVelocity.X, (float)(value / Configuration.MeterInPixels)); }
		}

		public override double Theta {
			get { return Body.Rotation; }
			set { Body.Rotation = (float)value; }
		}

		#region Behavior
		public SpriteObject(RenderSet render_set):
			this(render_set, 0.0, 0.0, 1.0, 1.0, Texture.DefaultTexture)
		{
		}
		public SpriteObject(RenderSet render_set, Texture texture):
			this(render_set, 0.0, 0.0, 1.0, 1.0, texture)
		{
		}
		public SpriteObject (RenderSet render_set, double x, double y, Texture texture):
			this(render_set, x, y, 1.0, 1.0, texture)
		{		
		}
		// Main constructor:
		public SpriteObject (RenderSet render_set, double x, double y, double scalex, double scaley, Texture texture):
			base(render_set, x, y, scalex, scaley, texture)
		{
			_RenderSet = render_set;
			InitPhysics();
			Position = _Position; // Update body position from initial position
		}
		protected virtual void InitPhysics()
		{
            float w, h;
            if (Texture.Regions != null && Texture.Regions.Length > 0)
            {
                var size = Texture.Regions[0].Size;
                w = (float)(_Scale.X * size.X / Configuration.MeterInPixels);
                h = (float)(_Scale.Y * size.Y / Configuration.MeterInPixels);
            }
            else
            {
                w = (float)(_Scale.X * Texture.Width / Configuration.MeterInPixels);
                h = (float)(_Scale.Y * Texture.Height / Configuration.MeterInPixels);
            }
			var half_w_h = new Microsoft.Xna.Framework.Vector2(w * 0.5f, h * 0.5f);
			var msv2 = new Microsoft.Xna.Framework.Vector2(
				(float)(_Position.X / Configuration.MeterInPixels),
				(float)(_Position.Y / Configuration.MeterInPixels));
			_SpriteBody = BodyFactory.CreateBody(_RenderSet.Scene.World, msv2);
			FixtureFactory.AttachRectangle(w, h, 100.0f, Microsoft.Xna.Framework.Vector2.Zero, _SpriteBody);
			_SpriteBody.BodyType = BodyType.Static;
			_SpriteBody.FixedRotation = true;
			_SpriteBody.Friction = 0.5f;

			// HACK: Only enable bodies for which the object is in the current scene
			Body.Enabled = this.RenderSet.Scene == Program.MainGame.CurrentScene;

			ConnectBody();
		}
		public override void Render (double time)
		{
            GL.PushMatrix();
            {
                GL.Translate(
                    //Math.Round
					(_SpriteBody.Position.X * Configuration.MeterInPixels),
                    //Math.Round
					(_SpriteBody.Position.Y * Configuration.MeterInPixels), 0.0);
                // Don't even read this line:
                float r = (float)
                    //(45.0 * Math.Round(
                    (_Theta + (double)OpenTK.MathHelper.RadiansToDegrees(_SpriteBody.Rotation))
                    //	/ 45.0))
                        ;
                GL.Rotate(r, 0.0, 0.0, 1.0);
                GL.Scale(_Scale);
                Draw();
            }
            GL.PopMatrix();
		}
		public override void Update (double time)
		{
			// HACK: Unf*ckulate rotation
			base.Update(time);
		}
		public virtual void ConnectBody ()
		{
			Body.UserData = this;
			// Set up the blueprint from the vertices of he body here
			_Blueprints = new List<IRenderable>();
			for (int i = 0; i < Body.FixtureList.Count; i++) {
				Fixture fixture = Body.FixtureList[i];
				if(fixture.ShapeType == FarseerPhysics.Collision.Shapes.ShapeType.Polygon)
				{
					var poly_shape = (FarseerPhysics.Collision.Shapes.PolygonShape)fixture.Shape;
					Vector3d[] verts = new Vector3d[poly_shape.Vertices.Count];
					for(int j = 0; j < poly_shape.Vertices.Count; j++)
						verts[j] = new Vector3d(
							(poly_shape.Vertices[j].X) * Configuration.MeterInPixels,
							(poly_shape.Vertices[j].Y) * Configuration.MeterInPixels, 0.0);
					_Blueprints.Add (new BlueprintLineLoop(this, 0, verts));
				}
				else if(fixture.ShapeType == FarseerPhysics.Collision.Shapes.ShapeType.Edge)
				{
					var poly_shape = (FarseerPhysics.Collision.Shapes.EdgeShape)fixture.Shape;
					var p0 = new Vector3d(
						(poly_shape.Vertex1.X) * Configuration.MeterInPixels,
						(poly_shape.Vertex1.Y) * Configuration.MeterInPixels, 0.0);
					var p1 = new Vector3d(
						(poly_shape.Vertex2.X) * Configuration.MeterInPixels,
						(poly_shape.Vertex2.Y) * Configuration.MeterInPixels, 0.0);
					_Blueprints.Add (new BlueprintLineLoop(this, 0, p0, p1));
				}
			}
		}
		public virtual void Derez()
		{
            _SpriteBody.Dispose();
			_RenderSet.Remove(this);
		}
		#endregion
	}
}

