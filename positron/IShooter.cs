using System;

namespace positron
{
	public class FireEventArgs : EventArgs
	{
		// Auto-fields are eeviill!
		protected object Info { get; set; }
	}
	public delegate void FireEventHandler(object sender, FireEventArgs e);
	public interface IShooter
	{
		event FireEventHandler Fire;
		void OnFire(object sender, FireEventArgs e);
	}
}

