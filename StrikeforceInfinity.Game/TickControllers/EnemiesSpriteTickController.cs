using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites.Enemies.BaseClasses;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;
using StrikeforceInfinity.Game.Utility;
using StrikeforceInfinity.Shared.Messages.Notify;
using System;

namespace StrikeforceInfinity.Game.Controller
{
    internal class EnemiesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _gameCore;
        private readonly SiSpriteVector _multiplaySpriteVector = new();

        public EnemiesSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
            _gameCore = gameCore;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            if (GameCore.Player.Sprite != null)
            {
                GameCore.Player.Sprite.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in Visible())
            {
                foreach (var weapon in enemy.Weapons)
                {
                    weapon.LockedOnObjects.Clear();
                }

                if (GameCore.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);

                    if (GameCore.Player.Sprite.SelectedSecondaryWeapon != null)
                    {
                        GameCore.Player.Sprite.SelectedSecondaryWeapon.ApplyWeaponsLock(displacementVector, enemy); //Player lock-on to enemy. :D
                    }
                }

                if (_gameCore.Multiplay.PlayMode != HgPlayMode.SinglePlayer)
                {
                    if ((DateTime.UtcNow - _multiplaySpriteVector.Timestamp).TotalMilliseconds >= _gameCore.Settings.Multiplayer.PlayerAbsoluteStateDelayMs)
                    {
                        _multiplaySpriteVector.MultiplayUID = enemy.MultiplayUID;
                        _multiplaySpriteVector.Timestamp = DateTime.UtcNow;
                        _multiplaySpriteVector.X = enemy.X;
                        _multiplaySpriteVector.Y = enemy.Y;
                        _multiplaySpriteVector.AngleDegrees = enemy.Velocity.Angle.Degrees;
                        _multiplaySpriteVector.BoostPercentage = enemy.Velocity.BoostPercentage;
                        _multiplaySpriteVector.ThrottlePercentage = enemy.Velocity.ThrottlePercentage;

                        _gameCore.Multiplay.Notify(_multiplaySpriteVector);
                    }
                }

                enemy.ApplyMotion(displacementVector);
                enemy.RenewableResources.RenewAllResources();
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            lock (SpriteManager.Collection)
            {
                object[] param = { GameCore };
                SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = GameCore.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Generator.Next(GameCore.Settings.MinEnemySpeed, GameCore.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Generator.Next(0, 360);

                obj.BeforeCreate();
                SpriteManager.Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }
    }
}
