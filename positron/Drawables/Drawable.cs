using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace positron
{
	public abstract class Drawable : IRenderable, ISceneObject
	{
		#region State
		#region Member Variables
		protected RenderSet _RenderSet;
        #region OpenGL
        #endregion
        protected Vector3d _Position = new Vector3d();
		protected Vector3d _Scale = new Vector3d();
		protected Vector3d _Velocity = new Vector3d();
		protected double _Theta = 0.0;
		protected bool _Preserve = false;
		#endregion
		#region Accessors
		public virtual Vector3d Position {
			get { return _Position; }
			set { _Position = value; }
		}
		public virtual double PositionX {
			get { return _Position.X; }
			set { _Position.X = value; }
		}
		public virtual double PositionY {
			get { return _Position.Y; }
			set { _Position.Y = value; }
		}
		public virtual double PositionZ {
			get { return _Position.Z; }
			set { _Position.Z = value; }
		}
		public virtual Vector3d Velocity {
			get { return _Velocity; }
			set { _Velocity = value; }
		}
		public virtual double VelocityX {
			get { return _Velocity.X; }
			set { _Velocity.X = value; }
		}
		public virtual double VelocityY {
			get { return _Velocity.Y; }
			set { _Velocity.Y = value; }
		}
		public virtual Vector3d Scale {
			get { return _Scale; }
			set { _Scale = value; }
		}
		public virtual double ScaleX {
			get { return _Scale.X; }
			set { _Scale.X = value; }
		}
		public virtual double ScaleY {
			get { return _Scale.Y; }
			set { _Scale.Y = value; }
		}
		public virtual double Theta {
			get { return _Theta; }
			set { _Theta = value; }
		}
		/// <summary>
		/// Gets the render set associated with this drawable.
		/// Specifying a null render set in the constructor
		/// implies loose render set association and manual
		/// tracking of the object.
		/// </summary>
		public RenderSet RenderSet {
			get { return _RenderSet; }
			set { _RenderSet = value; }
		}
		public bool Preserve {
			get { return _Preserve; }
			set { _Preserve = value; }
		}
		#endregion
		#endregion
		public Drawable (RenderSet render_set)
		{
			_RenderSet = render_set;
            if(_RenderSet != null)
			    _RenderSet.Add(this);
		}
        /// <summary>
        /// Creates geometry information necessary for VBO
        /// This is either called in the constructor or
        /// called manually.
        /// </summary>
        public virtual void Build()
        {
            // Some inexpensive drawables are built each frame
            // Therefore it is not required to implement Build()
        }
		public abstract void Render(double time);
		public abstract double RenderSizeX();
		public abstract double RenderSizeY();
		public virtual void SetChange (object sender, SetChangeEventArgs e)
		{
			this._RenderSet = e.To;
		}
	}
}