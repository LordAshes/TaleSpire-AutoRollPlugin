# Auto Roll Plugin

This unofficial TaleSpire mod that automatically spawns, rolls and cleans up any dice introduced into the dice tray
via the talespire://dice protocol. This includes, but is not limited to, the Dice Selection plugin and the Beyond The
Spire Chrome Extension (which allows you to make rolls from your D&D Beyond character sheets). 

## Change Log

1.0.2: Randomized the starting orientation of dice adding even more randomness.

1.0.1: Added more randomization to the roll.

1.0.1: Bug Fix: Manually added dice are ignored (work the same as core TS).

1.0.0: Initial release

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
