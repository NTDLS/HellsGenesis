using AI2D.Objects;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Game
    {
        private bool _shutdown = false;
        private System.Threading.Thread _graphicsThread;
        private UserInput _userInput;
        public UserInput Input { get; private set; }
        public Display Display { get; private set; }
        public ActorAssets Actors { get; private set; }

        public void Start()
        {
            for (int i = 0; i < 1; i++)
            {
                Actors.CreateBoulder();
            }

            _shutdown = false;
            _graphicsThread.Start();
            Actors.ShowNewPlayer();
        }

        public void Stop()
        {
            _shutdown = true;
        }

        #region ~/ctor.

        public Game(Control drawingSurface, Size visibleSize)
        {
            Display = new Display(drawingSurface, visibleSize);
            Actors = new ActorAssets(this);
            Input = new UserInput(this);

            _graphicsThread = new System.Threading.Thread(GraphicsThreadProc);
        }

        #endregion

        #region Engine Graphics.

        private void GraphicsThreadProc()
        {
            while (_shutdown == false)
            {
                AdvanceFrame();
                System.Threading.Thread.Sleep(10);
            }
        }

        void AdvanceFrame()
        {
            #region Boulders Frame Advancement.

            foreach (var boulder in Actors.Boulders)
            {
                boulder.AdvanceFrame();

                if (boulder.X < 0)
                {

                    //Program.CalculeAngle(new PointF(boulder.Velocity.Angle.X, boulder.Velocity.Angle.Y), 


                    //boulder.Velocity.Angle.X = (0 - boulder.Velocity.Angle.X);
                }
                else if (boulder.X >= (Display.VisibleSize.Width - boulder.Size.Width) - 15)
                {
                    //boulder.Velocity.Angle.X = (0 - boulder.Velocity.Angle.X);
                }

                if (boulder.Y < 0)
                {
                    //boulder.Velocity.Angle.Y = (0-boulder.Velocity.Angle.Y);
                }
                else if (boulder.Y >= (Display.VisibleSize.Height - boulder.Size.Height) - 35)
                {
                    //boulder.Velocity.Angle.Y = (0-boulder.Velocity.Angle.Y);
                }

                //boulder.X -= 1;

                /*boulder.Velocity.Speed*/

                boulder.X += (boulder.Velocity.Angle.X);
                boulder.Y += (boulder.Velocity.Angle.Y);

                if (boulder.Intersects(Actors.Player))
                {
                    Actors.Player.Hit();


                }

                foreach (var bullet in Actors.Bullets)
                {
                    if (bullet.Intersects(boulder))
                    {
                        bullet.ReadyForDeletion = true;
                        boulder.Explode();
                    }
                }
            }

            #endregion

            #region Player Frame Advancement.
            /*
                The inverse is A = atan2(V.y, V.x)
            */
            Actors.Player.AdvanceFrame();

            if (Input.IsKeyPressed(PlayerKey.Fire))
            {
                Actors.Player.FireGun();
            }

            if (Input.IsKeyPressed(PlayerKey.Forward))
            {
                Actors.Player.X += (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                Actors.Player.Y += (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                Actors.ShipEngineRoar.Play();
            }
            else if (Input.IsKeyPressed(PlayerKey.Reverse))
            {
                Actors.Player.X -= (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                Actors.Player.Y -= (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                Actors.ShipEngineRoar.Play();
            }
            else
            {
                Actors.ShipEngineRoar.Fade();
            }

            if (Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                Actors.Player.Rotate(-1.0f);
            }
            else if (Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                Actors.Player.Rotate(1.0f);
            }

            if (Input.IsKeyPressed(PlayerKey.Escape))
            {
                Actors.Player.Visable = false;
            }

            if (Actors.Player.ReadyForDeletion)
            {
                Actors.Player.Cleanup();
            }

            #endregion

            #region Bullet Frame Advancement.

            foreach (var bullet in Actors.Bullets)
            {
                bullet.AdvanceFrame();

                if (bullet.X < 0)
                {
                    bullet.ReadyForDeletion = true;
                }
                else if (bullet.X >= Display.VisibleSize.Width)
                {
                    bullet.ReadyForDeletion = true;
                }

                if (bullet.Y < 0)
                {
                    bullet.ReadyForDeletion = true;
                }
                else if (bullet.Y >= Display.VisibleSize.Height)
                {
                    bullet.ReadyForDeletion = true;
                }

                bullet.X += (bullet.Velocity.Angle.X * bullet.Velocity.Speed);
                bullet.Y += (bullet.Velocity.Angle.Y * bullet.Velocity.Speed);
            }

            #endregion

            #region Cleanup (cant be done in a foreach).

            for (int i = 0; i < Actors.Boulders.Count; i++)
            {
                if (Actors.Boulders[i].ReadyForDeletion)
                {
                    Actors.DeleteBoulder(Actors.Boulders[i]);
                }
            }

            for (int i = 0; i < Actors.Bullets.Count; i++)
            {
                if (Actors.Bullets[i].ReadyForDeletion)
                {
                    Actors.DeleteBullet(Actors.Bullets[i]);
                }
            }

            #endregion
        }

        public void RenderObjects(Graphics dc)
        {
            Actors.RenderObjects(dc);
        }

        #endregion
    }
}
