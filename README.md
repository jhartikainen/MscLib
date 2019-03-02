# MscLib

A collection of helpers to build My Summer Car mods.

## Installation

Put files into your mod project and hope it works.

## How to use

The classes in this library follow two patterns:

- Singletons: Objects such as the player character or vehicles. They cannot be duplicated without breaking the game. Some other game helpers such as the Spawner object are also represented as singletons for convenience.
- Everything else: Other objects, such as moose, dancers, can be duplicated. A new instance will duplicate the original, and in most cases a static accessor is provided to access the original "source" object

Most objects also provide the original GameObject as a property. If you need to adjust the position of something, active it, or do something else, just use the GameObject directly.

## Examples

```
//this library is namespaced as MscLib
using MscLib;

//player has some properties to mess with the stats...
Player.UrineLevel = 100;

//and some helpers to check things like the current vehicle
if(Player.IsDrivingOrPassenger) { /* player is currently driving or a passenger */ }

//CurrentVehicle can be null. Not all vehicles are supported yet
var vehicle = Player.CurrentVehicle;

//vehicles implement the DrivableVehicle interface...
ModConsole.Print(vehicle.FuelLevel);
vehicle.FuelLevel = vehicle.MaxFuelLevel;

//car parts have some things you can do with them, for example
//this would unattach all attached engine parts...
foreach(var part in PartsDatabase.Engine) {
  if(part.IsAttached) {
    part.Remove();
  }
}

//We can also spawn some items...
var item = Spawner.SpawnItem(Spawner.ItemType.BEER);
//position it in front of the player
item.transform.position = Player.Transform.position + (Player.Transform.forward * 2f);

//or we can spawn a moose
var moose = new Moose();
//for the moose to do anything interesting, it needs a route.
//this would place the moose 50 units in front of the player, and
//make it chase the player:
var mooseStart = Player.Transform.position + (Player.Transform.forward * 50f);
moose.SetRoute(mooseStart, Player.GameObject);
```
