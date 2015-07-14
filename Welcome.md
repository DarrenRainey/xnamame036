This is the XnaMame 036 project, a port of the MAME v0.36 to XNA/C#

# Introduction #
The aim of this project was to check if MAME could be ported to C#/XNA so that I could run Pacman. Silly, I know; but it seems to run ok.

Feeling that it was an success, I wanted to implement the drivers for the games that I remember playing as a kid.

Sound is an issue at the moment, but I'm working on it. Not all implemented games have proper joystick/gamepad controls mapped - yet.

# Details #

Since this started out as a project I wanted to try for myself, some things here is different than in the MAME version. Obviously, I've used more of the .NET API and C# language features to reach my goal. For instance, game drivers are loaded through reflection at need instead of being statically linked into the main program. For each driver then to be "self-contained" a drivers corresponding machine/vidhrdw entry is moved into the driver itself. Only drivers that are inherited have entries in these directories now.

There is one thing that will/may reduce the runtime memory footprint that I'd like to do real soon, and that is to create the CPU/SOUND drivers for a game when needed. Now they are contained in a static array just like in MAME.

## List of drivers/games that can be played (or sort of) ##

  * Pacman
  * Invaders
  * 1942
  * 1943
  * zaxxon
  * szaxxon
  * galaga
  * digdug
  * dkong
  * dkongjr
  * dkong3
  * mario
  * bombjack
  * amidar
  * phoenix
  * locomotn
  * tp84
  * phozon
  * mappy
  * gunfight
  * boothill
  * arkanoid
  * frogger
  * gunsmoke
  * qix
  * mpatrol
  * pbaction
  * citycon
  * jackal
  * speedbal
  * gyruss
  * rocnrope
  * galaxian
  * ladybug
  * ldrun
  * ldrun2
  * ldrun3
  * ldrun4
  * galaga3
  * gaplus
  * cobracom
  * ddragonb
  * ddragon2
  * retofinv
  * vigilant
  * pang
  * spang
  * gng
  * diamond


And even some vector games
  * zektor
  * tacscan
  * asteroid
  * llander
  * speedfrk