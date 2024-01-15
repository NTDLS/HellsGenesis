using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.Shared;
using Si.Shared.Messages.Notify;
using Si.Shared.Messages.Query;
using Si.Shared.Payload;
using Si.Shared.Payload.SpriteActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.Core
{
    /// <summary>
    /// Handles the events from the Si.MultiplayClient.EngineMultiplayManager
    /// </summary>
    internal class MultiplayClientEventHandlers
    {
        private readonly GameEngineCore _gameEngine;

        public MultiplayClientEventHandlers(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;
        }

        /// <summary>
        /// The server is requesting that we as the lobby owner supply a list of sprites to populate other connections maps.
        /// </summary>
        /// <returns></returns>
        public List<SiSpriteLayout> OnNeedLevelLayout()
        {
            var spriteLayouts = new List<SiSpriteLayout>();

            //Maybe we only do this for newcomers to a game already in session and just rely on
            //  OnSpriteCreated() to add sprites to the clients that start the game with the host.

            /*
            //--------------------------------------------------------------------------------------
            //-- Send the enemy sprites:
            //--------------------------------------------------------------------------------------
            var enemies = _gameEngine.Sprites.Enemies.All();
            foreach (var enemy in enemies)
            {
                spriteLayouts.Add(new SiSpriteLayout(enemy.GetType().FullName, enemy.MultiplayUID)
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
            */

            return spriteLayouts;
        }

        /// <summary>
        /// The situation level has actually started.
        /// </summary>
        public void OnHostLevelStarted()
        {
            _gameEngine.Sprites.Use(o =>
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
            var playerDrone = SiReflection.CreateInstanceFromTypeName<SpritePlayerBase>($"{selectedPlayerClass}Drone", new[] { _gameEngine });
            playerDrone.MultiplayUID = playerMultiplayUID;
            playerDrone.Visable = true;
            playerDrone.X = 0;
            playerDrone.Y = 0;

            _gameEngine.Sprites.PlayerDrones.Add(playerDrone);
            //Debug.WriteLine($"Inserted Multiplay Sprite: '{selectedPlayerClass}'->'{playerMultiplayUID}'->{playerDrone.UID}");
        }

        public void OnSpriteCreated(SiSpriteLayout layout)
        {
            if (_gameEngine.Sprites.Use(o => o.Any(s => s.MultiplayUID == layout.MultiplayUID)))
            {
                return; //We already have a sprite with this UID.
            }

            SpriteBase sprite;

            //If the sprite has a dedicated "...Drone" type then create it instead.
            if (SiReflection.DoesTypeExist(layout.FullTypeName + "Drone"))
            {
                sprite = SiReflection.CreateInstanceFromTypeName<SpriteBase>(layout.FullTypeName + "Drone", new[] { _gameEngine });
            }
            else
            {
                sprite = SiReflection.CreateInstanceFromTypeName<SpriteBase>(layout.FullTypeName, new[] { _gameEngine });
            }

            sprite.IsDrone = true;
            sprite.MultiplayUID = layout.MultiplayUID;
            sprite.Visable = true;
            sprite.X = layout.Vector.X;
            sprite.Y = layout.Vector.Y;
            sprite.Velocity.MaxBoost = layout.Vector.MaxBoost;
            sprite.Velocity.MaxSpeed = layout.Vector.MaxSpeed;
            sprite.Velocity.BoostPercentage = layout.Vector.BoostPercentage;
            sprite.Velocity.ThrottlePercentage = layout.Vector.ThrottlePercentage;

            _gameEngine.Sprites.Add(sprite);
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

            _gameEngine.Sprites.Use(o =>
            {
                //Get all the sprites ahead of time. I "think" this is faster than searching in a loop.
                var sprites = o.Where(o => allMultiplayUIDs.Contains(o.MultiplayUID)).ToList();

                foreach (var action in actions.Collection)
                {
                    var sprite = sprites.SingleOrDefault(o => o.MultiplayUID == action.MultiplayUID);
                    if (sprite != null)
                    {
                        if (action is SiSpriteActionVector vector)
                        {
                            sprite.ApplyAbsoluteMultiplayVector(vector);
                        }
                        else if (action is SiSpriteActionHit hit)
                        {
                            sprite.Hit(hit.Damage);
                        }
                        else if (action is SiSpriteActionDelete)
                        {
                            sprite.QueueForDelete();
                        }
                        else if (action is SiSpriteActionExplode)
                        {
                            sprite.Explode();
                        }

                        if (sprite is ISpriteDrone drone)
                        {
                            if (action is SiSpriteActionFireWeapon weaponFire)
                            {
                                drone.FireDroneWeapon(weaponFire.WeaponTypeName);
                            }
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
            //Maybe we only do this for newcomers to a game already in session and just rely on
            //  OnSpriteCreated() to add sprites to the clients that start the game with the host.

            /*
            _gameEngine.Sprites.Enemies.DeleteAll();

            foreach (var spriteInfo in situationLayout.Sprites)
            {
                //Debug.WriteLine($"Adding Sprite: {spriteInfo.MultiplayUID}->'{spriteInfo.FullTypeName}'");

                var sprite = _gameEngine.Sprites.CreateByNameOfType(spriteInfo.FullTypeName);
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
                sprite.ControlledBy = _gameEngine.Multiplay.State.PlayMode switch
                {
                    SiPlayMode.MutiPlayerHost => SiControlledBy.LocalAI,
                    SiPlayMode.MutiPlayerClient => SiControlledBy.Server,
                    _ => throw new InvalidOperationException("Unhandled PlayMode")
                };
            }
            */
        }
    }
}
