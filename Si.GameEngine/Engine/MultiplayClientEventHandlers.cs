using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites.Player.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Messages.Notify;
using Si.Shared.Messages.Query;
using Si.Shared.Payload;
using Si.Shared.Payload.DroneActions;
using Si.Sprites.BasesAndInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Engine
{
    /// <summary>
    /// Handles the events from the Si.MultiplayClient.EngineMultiplayManager
    /// </summary>
    internal class MultiplayClientEventHandlers
    {
        private readonly EngineCore _gameCore;

        public MultiplayClientEventHandlers(EngineCore gameCore)
        {
            _gameCore = gameCore;
        }

        /// <summary>
        /// The server is requesting that we as the lobby owner supply a list of sprites to populate the other connections maps.
        /// </summary>
        /// <returns></returns>
        public List<SiSpriteLayout> OnNeedLevelLayout()
        {
            var spriteLayouts = new List<SiSpriteLayout>();

            //--------------------------------------------------------------------------------------
            //-- Send the enemy sprites:
            //--------------------------------------------------------------------------------------
            var enemies = _gameCore.Sprites.Enemies.All();
            foreach (var enemy in enemies)
            {
                spriteLayouts.Add(new SiSpriteLayout(enemy.GetType().FullName + "Drone", enemy.MultiplayUID)
                {
                    Vector = new SiSpriteVector()
                    {
                        X = enemy.LocalX,
                        Y = enemy.LocalY,
                        AngleDegrees = enemy.Velocity.Angle.Degrees,
                        MaxSpeed = enemy.Velocity.MaxSpeed,
                        MaxBoost = enemy.Velocity.MaxBoost,
                        ThrottlePercentage = enemy.Velocity.ThrottlePercentage,
                        BoostPercentage = enemy.Velocity.BoostPercentage,
                    }
                });
            }

            return spriteLayouts;
        }

        /// <summary>
        /// The situation level has actually started.
        /// </summary>
        public void OnHostLevelStarted()
        {
            _gameCore.Sprites.Use(o =>
            {
                var playerDrones = o.OfType<SpritePlayerBase>().Where(x => x.IsDrone).ToList();
                playerDrones.ForEach(x => x.Visable = true);
            });
        }

        /// <summary>
        /// The server is letting us know that a client created their player sprite.
        /// </summary>
        /// <param name="selectedPlayerClass"></param>
        /// <param name="playerMultiplayUID"></param>
        public void OnPlayerSpriteCreated(string selectedPlayerClass, Guid playerMultiplayUID)
        {
            var playerDrone = SiReflection.CreateInstanceFromTypeName<SpritePlayerBase>($"{selectedPlayerClass}Drone", new[] { _gameCore });
            playerDrone.MultiplayUID = playerMultiplayUID;
            playerDrone.Visable = true;
            playerDrone.LocalX = 0;
            playerDrone.LocalY = 0;

            _gameCore.Sprites.PlayerDrones.Add(playerDrone);
            //Debug.WriteLine($"Inserted Multiplay Sprite: '{selectedPlayerClass}'->'{playerMultiplayUID}'->{playerDrone.UID}");
        }

        public void OnSpriteCreated(SiSpriteLayout layout)
        {
            if (_gameCore.Sprites.Use(o => o.Any(s => s.MultiplayUID == layout.MultiplayUID)))
            {
                return; //We already have a sprite with this UID.
            }

            var sprite = SiReflection.CreateInstanceFromTypeName<SpriteBase>(layout.FullTypeName, new[] { _gameCore });
            sprite.IsDrone = true;
            sprite.MultiplayUID = layout.MultiplayUID;
            sprite.Visable = true;
            sprite.MultiplayX = layout.Vector.X;
            sprite.MultiplayY = layout.Vector.Y;
            sprite.Velocity.MaxBoost = layout.Vector.MaxBoost;
            sprite.Velocity.MaxSpeed = layout.Vector.MaxSpeed;
            sprite.Velocity.BoostPercentage = layout.Vector.BoostPercentage;
            sprite.Velocity.ThrottlePercentage = layout.Vector.ThrottlePercentage;

            _gameCore.Sprites.Add(sprite);
        }

        /// <summary>
        /// The server is letting us know that we have received one of more sprite actions from one or more clients.
        /// </summary>
        /// <param name="actions"></param>
        public void OnApplySpriteActions(SiSpriteActions actions)
        {
            var allMultiplayUIDs = actions.Collection.Select(o => o.MultiplayUID).ToHashSet();
            if (!allMultiplayUIDs.Any())
            {
                return;
            }

            _gameCore.Sprites.Use(o =>
            {
                //Get all the sprites ahead of time. I "think" this is faster than searching in a loop.
                var sprites = o.Where(o => allMultiplayUIDs.Contains(o.MultiplayUID)).ToList();

                foreach (var action in actions.Collection)
                {
                    var drone = sprites.Where(o => o.MultiplayUID == action.MultiplayUID).FirstOrDefault() as ISpriteDrone;
                    if (drone != null)
                    {
                        if (action is SiDroneActionVector vector)
                        {
                            drone.ApplyAbsoluteMultiplayVector(vector);
                        }
                        else if (action is SiDroneActionHit hit)
                        {
                            drone.Hit(hit.Damage);
                        }
                        else if (action is SiDroneActionExplode)
                        {
                            drone.Explode();
                        }
                        else if (action is SiDroneActionFireWeapon weaponFire)
                        {
                            drone.FireDroneWeapon(weaponFire.WeaponTypeName);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// The server has sent us a level layout.
        /// </summary>
        /// <param name="situationLayout"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void OnReceivedLevelLayout(SiSituationLayout situationLayout)
        {
            _gameCore.Sprites.Enemies.DeleteAll();

            foreach (var spriteInfo in situationLayout.Sprites)
            {
                //Debug.WriteLine($"Adding Sprite: {spriteInfo.MultiplayUID}->'{spriteInfo.FullTypeName}'");

                var sprite = _gameCore.Sprites.CreateByNameOfType(spriteInfo.FullTypeName);
                sprite.MultiplayUID = spriteInfo.MultiplayUID;
                sprite.MultiplayX = spriteInfo.Vector.X;
                sprite.MultiplayY = spriteInfo.Vector.Y;
                sprite.LocalX = 0;
                sprite.LocalY = 0;
                sprite.Velocity.MaxSpeed = spriteInfo.Vector.MaxSpeed;
                sprite.Velocity.MaxBoost = spriteInfo.Vector.MaxBoost;
                sprite.Velocity.Angle.Degrees = spriteInfo.Vector.AngleDegrees;
                sprite.Velocity.ThrottlePercentage = spriteInfo.Vector.ThrottlePercentage;
                sprite.Velocity.BoostPercentage = spriteInfo.Vector.BoostPercentage;
                sprite.ControlledBy = _gameCore.Multiplay.State.PlayMode switch
                {
                    SiPlayMode.MutiPlayerHost => SiControlledBy.LocalAI,
                    SiPlayMode.MutiPlayerClient => SiControlledBy.Server,
                    _ => throw new InvalidOperationException("Unhandled PlayMode")
                };
            }
        }
    }
}
