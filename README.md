RandomGenerator
===============

A helper class designed to help with pseudo randomness in dynamic media such as games or visualisations. The generator is designed to be deterministic (have the same results) across platforms if needed. You can use it to make words the same from just one seed value or to send far fewer data through network.

The generator is written in C# and is made to work with the Unity Game Engine (Unity3d) but can also be used standalone without dependencies. I also had once a JavaScript 1.5 fork, but it is currently unmaintained and not released.

Use: instantiate as many RandomGenerators as you want, a good idea is to keep one for each entity that requires randomness (world generator, AI, enemy generator etc).

Pseudo random generation is based on the 32 bit Tiny Mersenne Twister (c) by Mutsuo Saito and Makoto Matsumoto.

The home page for the random generator is https://github.com/chanibal/RandomGenerator


Features
--------

* Easy to use random generator to be used without almost any higher maths knowledge. More advanced features are named by usage and not the name of long dead famous mathematicians.

* Generates uniform 1D, 2D, 3D and 4D values in useful combinations: float and int ranges; vectors in unit squares, cubes, circles and spheres and on their surfaces/edges; quaternions.

* Generates time based probabilistic events in a simple manner - just ask it every frame if the event with given probability per interval has happened this time.

* Generates helpers to create distribution intervals and ranges.

* Provides extensions to shuffle and retrieve random elements from generic collections.

* Is fairly fast and tested.


Installation
------------

Clone, download or add as a submodule this so it rests in the `Assets/ChanibaL/RandomGenerator` directory.



License
-------

License: BSD 2-clause "Simplified" License.

