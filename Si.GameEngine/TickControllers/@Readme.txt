The tick managers have two purposes, they act as factories for specific types of sprites and they contain the
	"world tick logic" for a specific type of object via a call to the ExecuteWorldClockTick() function.

For instance, the [MunitionSpriteTickController] class is used to create/delete munitions and the ExecuteWorldClockTick(...)
	function moves the munitions by the supplied vector as well as tests for munition collisions.

These are called by Si.GameEngine.Engine.EngineWorldClock.ExecuteWorldClockTick().