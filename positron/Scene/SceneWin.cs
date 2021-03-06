using System;
using System.Diagnostics;

using OpenTK;

namespace positron
{
    public class SceneWin: Scene
    {
        Stopwatch DerpTimer = new Stopwatch();
        SpriteBase WinThing;
        public SceneWin (PositronGame game):
            base(game)
        {
            SceneEntry += (sender, e) => {
                if(_Game.Player1 != null)
                {
                    Follow(null);
                    _Game.Player1.Derez();	
                }
                WinThing.StartAnimation(WinThing.AnimationDefault);
                DerpTimer.Start();
            };
        }
        public override void InitializeScene()
        {
            WinThing = new SpriteBase(HUD, ViewWidth / 2.0, ViewHeight / 2.0, Texture.Get ("sprite_win")).CenterShift();
            //WinThing.AnimationDefault.Sound = Sound.Get("win");
            base.InitializeScene();
        }
        public override void Update(double time)
        {
            double fx = 2 - Math.Cos(MathHelper.TwoPi * DerpTimer.Elapsed.TotalSeconds);
            double gx = 5 * MathHelper.Pi * Math.Sin(MathHelper.Pi * DerpTimer.Elapsed.TotalSeconds);
            WinThing.Scale = new Vector3d(fx);
            WinThing.Theta = gx;
            base.Update(time);
			if (DerpTimer.Elapsed.TotalSeconds > 5.0) {
                _Game.AddUpdateEventHandler(this, (sender, e) => {
                    _Game.SetupScenes (typeof(ISceneGameplay));
                    DerpTimer.Reset();
                    return true;
                });
                _Game.CurrentScene = (Scene)_Game.Scenes["SceneFirstMenu"];
			}
        }
    }
}

