# Auto Roll Plugin

This unofficial TaleSpire mod that automatically spawns, rolls and cleans up any dice introduced into the dice tray
via the talespire://dice protocol. This includes, but is not limited to, the Dice Selection plugin and the Beyond The
Spire Chrome Extension (which allows you to make rolls from your D&D Beyond character sheets). 

## Change Log

```
1.0.3: Fixed multi dice set spawn when adding different dice groups.
1.0.2: Randomized the starting orientation of dice adding even more randomness.
1.0.1: Added more randomization to the roll.
1.0.1: Bug Fix: Manually added dice are ignored (work the same as core TS).
1.0.0: Initial release
```

## Install

Use R2ModMan or similar installer to install.

## Usage

Use any program or plugin (e.g. Dice Selection plugin, Dice Macros app, Beyond the Spire Chrome Extension, Webbrowser)
to send a talespire://dice request to the OS or TaleSpireURLRelay. Instead of just loading the dice tray with the
specified dice (as what happens in core TS), the dice will be placed on the board and rolled. After a specified time
the dice will be cleared (unless the setting is changed to keep them).

In R2ModMan there is a setting for this plugin which dictates how long after the roll is completed the dice remain
before being cleared. Normally the dice can be cleared immeidately because the dice being cleared has not effect on
the result message that indicates the roll result. However, in some cases players like to see the actual dice in which
case this setting allows some time for that before the dice are cleared.

A negative value for this setting means the dice are not cleared (i.e. the dice need to be cleared manually).
A positive value indicates the number of seconds that the dice exist after finishing the roll in before being cleared. 

Note: If the setting is negative then the dice will remain but will not be reused. The dice can be reused manually
      but a tilespire://dice request for the same roll will always generate a new die or dice.
	  
### Talespire Protocol

The following formats are supported by the AutoRoll plugin:

```talespire://dice/name:roll```

and

```talespire://dice/name:roll1/roll2/...```

```
Where "dice" is a mandatory keyword indicating that the request is a dice protocol request.
Where "name" is a mandatory single word (no spaces) indicating what the roll is for.
Where "roll" (and "roll1", "Roll2", ...) is a mandatory roll formula which can consist of one or more rolls
      specified in the #D# format (e.g. 3D6) with a possible penalty or bonus at the end (e.g. 3D6+2).
	  A single "roll" can contain multiple dice groups in which case the total is added to subtracted
	  (e.g. 2D10+3D8+2).
Where "roll2" is a optional secondary roll whose total will be totalled individually from the first roll.
Where "..." is a is any number of additional rolls (same as "roll2") separated by slashes. Each additional roll
      generates its own total.
```
