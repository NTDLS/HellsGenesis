The tick managers have two purposes, they act as factories for specific types of sprites and they contain the "world tick logic"
	for a specific type of object via a call to the ExecuteWorldClockTick() function.

For instance, the [ProjectileSpriteTickController] class is used to create, and delete projectiles and the ExecuteWorldClockTick(...) function moves
	the projectiles by the supplied vector.