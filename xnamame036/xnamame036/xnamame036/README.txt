This is a (direct)port of Mame version 0.36 to C# and XNA, in an attempt both to see if it is doable, 
and to preserve the arcade classics in their original format on the XBOX 360. 
This is something I did for myself, and the port is merely a hack to get things working.
Although I've used some generics, reflection-stuff etc. to get it to work,it is a "true" port.
Which means no fancy fonts in the gui/userinterface etc.

Licensing follows MAME license (http://mamedev.org/legal.html)

TODO
----
Sound from samples are just static.
Load artwork (set useartwork=true)
Better tilemap gfxobj handling
More cpu's
frontend
better mapping of keyboard and gamepad
Implement a game selection pre-system, listing from implemented drivers and available roms.
Remove the static initialization of cpu/sound drivers to reduce runtime memory overhead.

If you make improvements and/or corrections, please give back so all can enjoy and benefit.

Kjetil Næss
kjetil.naess@live.no