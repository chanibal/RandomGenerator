// Uncomment if you plan to use this outside of the Unity Game Engine:
// #define NO_UNITY

using System.Collections.Generic;
using System;

#if !NO_UNITY
using U = UnityEngine;
#endif

// This code is under the BSD 2-clause "Simplified" License:
//
// Copyright(c) 2016 Krzysztof Bociurko
// http://chanibal.pl, https://github.com/chanibal
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided 
// that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and 
// the following disclaimer.
//																																		
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and 
// the following disclaimer in the documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
// OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

namespace ChanibaL
{

	/// <summary>
	/// The RandomGenerator is a helper class designed to help with pseudo 
	/// randomness in dynamic media such as games or visualisations.
	/// 
	/// The generator is designed to be deterministic (have the same results) across platforms if needed.
	/// You can use it to make words the same from just one seed value or to send far
	/// fewer data through network.
	/// 
	/// Use: instantiate as many RandomGenerators as you want, a good idea is to keep one for each
	/// entity that requires randomness (world generator, AI, enemy generator etc).
	/// 
	/// You can save the state of the generator and replace it if needed.
	/// 
	/// Pseudo random generation is based on the 32 bit Tiny Mersenne Twister (c) by Mutsuo Saito and Makoto Matsumoto.
	/// 
	/// This code is released under the 3-Clause BSD license
	/// 
	/// For an up to date version, documentation or to report bugs, go to https://github.com/chanibal/RandomGenerator
	/// 
	/// Version 1.0, released 2016-06-29T23:13:05+2:00
	/// </summary>
	[Serializable]
	public class RandomGenerator
	{

		/// <summary>
		/// Global instance of RandomGenerator
		/// used for seed when none is provided and when no RandomGenerator is provided with helper functions
		/// Seed is provided from clock ticks on initialization.
		/// </summary>
		public static readonly RandomGenerator global = new RandomGenerator((uint)System.DateTime.Now.Ticks);


		#region TinyMT
		// This generator uses a port of TinyMT project by Mutsuo Saito and Makoto Matsumoto.
		// Original license follows:
		//
		// Copyright(c) 2011, 2013 Mutsuo Saito, Makoto Matsumoto,
		// Hiroshima University and The University of Tokyo.
		// All rights reserved.
		// Redistribution and use in source and binary forms, with or without
		// modification, are permitted provided that the following conditions are
		// met:
		//
		//  * Redistributions of source code must retain the above copyright
		//	  notice, this list of conditions and the following disclaimer.
		//	* Redistributions in binary form must reproduce the above
		//	  copyright notice, this list of conditions and the following
		//	  disclaimer in the documentation and/or other materials provided
		//	  with the distribution.
		//	* Neither the name of the Hiroshima University nor the names of
		//	  its contributors may be used to endorse or promote products
		//	  derived from this software without specific prior written
		//	  permission.
		//
		// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
		// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
		// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
		// A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT
		// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
		// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
		// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
		// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
		// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
		// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


		const UInt32 TINYMT32_MASK = 0x7fffffff;


		/// <summary>
		/// TinyMT32 internal state vector and parameters
		/// </summary>
		protected UInt32[] status = new UInt32[4];
		protected UInt32 mat1;
		protected UInt32 mat2;
		protected UInt32 tmat;


		/// <summary>
		/// Create a new RandomGenerator with known seed.
		/// This function initializes the internal state array of with a 32-bit unsigned integer seed.
		/// </summary>
		/// <param name="seed">a 32-bit unsigned integer used as a seed.</param>
		/// <param name="seed_mat1">mat1 from the state vector, will be used as additional seed</param>
		/// <param name="seed_mat2">mat2 from the state vector, will be used as additional seed</param>
		/// <param name="seed_tmat">tmat from the state vector, will be used as additional seed</param>
		public RandomGenerator(UInt32 seed, UInt32 seed_mat1 = 0, UInt32 seed_mat2 = 0, UInt32 seed_tmat = 0)
		{
			const int MIN_LOOP = 8;
			const int PRE_LOOP = 8;

			status[0] = seed;
			status[1] = mat1 = seed_mat1;
			status[2] = mat2 = seed_mat2;
			status[3] = tmat = seed_tmat;
			for (int i = 1; i < MIN_LOOP; i++)
			{
				status[i & 3] ^= (uint)(
					i + (1812433253)
					* (status[(i - 1) & 3]
						^ (status[(i - 1) & 3] >> 30))
				);
			}

			// Period certification: this function certificates the period of 2^127-1.
			if (
				(status[0] & TINYMT32_MASK) == 0 &&
				status[1] == 0 &&
				status[2] == 0 &&
				status[3] == 0
			)
			{
				status[0] = 'T';
				status[1] = 'I';
				status[2] = 'N';
				status[3] = 'Y';
			}

			for (int i = 0; i < PRE_LOOP; i++)
			{
				GetUInt32();
			}
		}


		/// <summary>
		/// Generates an unsigned 32 bit integer with uniform distribution
		/// </summary>
		/// <returns>0 <= value < 2**32</returns>
		public virtual UInt32 GetUInt32()
		{
			// next
			const int TINYMT32_SH0 = 1;
			const int TINYMT32_SH1 = 10;

			UInt32 x;
			UInt32 y;

			y = status[3];
			x = (status[0] & TINYMT32_MASK)
				^ status[1]
				^ status[2];
			x ^= (x << TINYMT32_SH0);
			y ^= (y >> TINYMT32_SH0) ^ x;
			status[0] = status[1];
			status[1] = status[2];
			status[2] = x ^ (y << TINYMT32_SH1);
			status[3] = y;
			status[1] ^= (uint)(-((Int32)(y & 1)) & mat1);
			status[2] ^= (uint)(-((Int32)(y & 1)) & mat2);


			// temper
			const int TINYMT32_SH8 = 8;

			UInt32 t0, t1;
			t0 = status[3];
			t1 = status[0]
				+ (status[2] >> TINYMT32_SH8);
			t0 ^= t1;
			t0 ^= (uint)(-((Int32)(t1 & 1)) & tmat);
			return t0;
		}
		#endregion


		/// <summary>
		/// Create a new RandomGenerator with random seed (taken from the global random generator).
		/// </summary>
		public RandomGenerator() : this(global.GetUInt32()) { }


		/// <summary>
		/// Copy constructor - makes a duplicate of the random generator .
		/// </summary>
		/// <param name="original">The original random generator.</param>
		protected RandomGenerator(RandomGenerator original) { CopyFrom(original); }


		/// <summary>
		/// Returns a copy of this instance.
		/// </summary>
		public RandomGenerator Clone() { return new RandomGenerator(this); }


		/// <summary>
		/// Copies state and seed from other RandomGenerator.
		/// </summary>
		public void CopyFrom(RandomGenerator original)
		{
			mat1 = original.mat1;
			mat2 = original.mat2;
			tmat = original.tmat;
			status[0] = original.status[0];
			status[1] = original.status[1];
			status[2] = original.status[2];
			status[3] = original.status[3];
		}




		#region scalars
		/// <summary>
		/// Generates a 16 bit unsigned integer with uniform distribution
		/// </summary>
		/// <returns>0 <= value < 2**16</returns>
		public UInt16 GetUInt16()
		{
			return (UInt16)(GetUInt32() & 0xFFFF);
		}


		/// <summary>
		/// Generates a single precision float value between 0f (inclusive) and 1f (exclusive) with uniform distribution
		/// </summary>
		/// <returns>
		/// 0 <= value < 1f
		/// </returns>
		public float GetFloat01()
		{
			const float TINYMT32_MUL = 1.0f / 16777216.0f;
			return (GetUInt32() >> 8) * TINYMT32_MUL;
		}


		/// <summary>
		/// Generates a single precision float value between min (inclusive) and max (exclusive) with uniform distribution
		/// </summary>
		/// <returns>
		/// min <= value < max
		/// </returns>
		public float GetFloatRange(float min, float max)
		{
			/// FIXME: this can have higher precision if not using normalized floats
			return min + GetFloat01() * (max - min);
		}


		/// <summary>
		/// Generates a 32 bit integer between min and max, inclusive with uniform distribution.
		/// </summary>
		/// <returns>
		/// min <= value <= max
		/// </returns>
		public int GetIntRange(int min, int max)
		{
			/// FIXME: very small numeric instability with very large numbers - lower values will tend to be generated a bit more often when distance is big
			return (int)(min + GetUInt32() % (max - min + 1));
		}


		/// <summary>
		/// Generates a boolean value with given chance (distribution) for true.
		/// </summary>
		/// <param name="chance">how often will this generate true? 0.9f means that it will 90% of the time</param>
		/// <returns>true or false</returns>
		public bool GetBool(float chance)
		{
			return GetUInt32() < UInt32.MaxValue * chance;
		}


		/// <summary>
		/// Generates a boolean value with 50% chance (distribution) for true or false
		/// </summary>
		/// <returns>true or false</returns>
		public bool GetBool()
		{
			return GetUInt32() < UInt32.MaxValue / 2;
		}
		#endregion
	



		#region time based propability

#if !NO_UNITY
		/// <summary>
		/// Given how often an event should happen, this method returns true on some calls depending on the value of Time.deltaTime.
		/// Should be called on each frame.
		/// </summary>
		/// <param name="happenWith50PercentChanceInSeconds">time interval in which you want to have 50% chance of event occuring</param>
		/// <returns>true when event should occur</returns>
		public bool HalfChanceInTime(float happenWith50PercentChanceInSeconds)
		{
			return HalfChanceInTime(happenWith50PercentChanceInSeconds, U.Time.deltaTime);
		}
#endif


		/// <summary>
		/// Given how often an event should happen, this method returns true on some calls depending on the value of deltaTime.
		/// Should be called on each frame.
		/// </summary>
		/// <param name="happenWith50PercentChanceInSeconds">time interval in which you want to have 50% chance of event occuring</param>
		/// <param name="deltaTime">time from last call to this method, in seconds</param>
		/// <returns>true when event should occur</returns>
		public bool HalfChanceInTime(float happenWith50PercentChanceInSeconds, float deltaTime)
		{
			// No time has elapsed, therefore nothing should happen
			if (deltaTime == 0)
				return false;

			// Time difference between events is 0, so events should happen infinitely often. 
			// this is the closest approximation.
			if (happenWith50PercentChanceInSeconds == 0)
				return true;

#if NO_UNITY
			return GetFloat01() > System.Math.Pow(0.5f, deltaTime/happenWith50PercentChanceInSeconds);
#else
			return GetFloat01() > U.Mathf.Pow(0.5f, deltaTime / happenWith50PercentChanceInSeconds);
#endif
		}


#if !NO_UNITY
		/// <summary>
		/// Checks for a time based event given the event average interval, depending on delta time from last check.
		/// </summary>
		/// <returns><c>true</c> if the event has happened, <c>false</c> otherwise.</returns>
		/// <param name="averageSecondsBetweenEvents">Average time between events in seconds.</param>
		/// <param name="deltaTime">Delta time in seconds</param>
		public bool ChanceForPeriod(float averageSecondsBetweenEvents) 
		{
			return ChanceForPeriod (averageSecondsBetweenEvents, U.Time.deltaTime);
		}
#endif


		/// <summary>
		/// Checks for a time based event given the event average interval, depending on delta time from last check.
		/// </summary>
		/// <returns><c>true</c> if the event has happened, <c>false</c> otherwise.</returns>
		/// <param name="averageSecondsBetweenEvents">Average time between events in seconds.</param>
		/// <param name="deltaTime">Delta time in seconds</param>
		public bool ChanceForPeriod(float averageSecondsBetweenEvents, float deltaTime)
		{
			if (averageSecondsBetweenEvents < 0)
				throw new ArgumentException ("seconds per event must be > 0", "secondsPerEvent");
			if (deltaTime <= 0)
				return false;
			if (averageSecondsBetweenEvents == 0)
				return true;
			
			// Poisson distribution:
			// P(k) = l**k * e**-l / k!
			// P(at least one event) = 1 - P(0) == 1 - l ** 0 * e ** -l / 0! = 1 - e ** -l
			// where k is number of events per time interval and l is the event rate

			float eventRate = deltaTime / averageSecondsBetweenEvents;	// l
			#if !NO_UNITY
			float propability = 1 - U.Mathf.Exp(-eventRate);
			#else
			float propability = 1 - System.Math.Exp(-eventRate);
			#endif

			return GetBool(propability);
		}
		#endregion




		#region distribution propabilities
		/// <summary>
		/// Generates an array of values incerementing from 0 to 1. The values are limits of intervals, dividing space in an uniform way.
		/// 
		/// An easy way to understand this is that each interval will have a length of 1 + (random between 0 and variance). 
		/// The sum of lengths will then be normalized.
		/// </summary>
		/// <param name="n">number of intervals</param>
		/// <param name="variation">how much should the intevals differ one from another? 1 is </param>
		/// <param name="addZero">Should a zero be added at the start of the array? This adds one to number of intervals</param>
		/// <returns>an array of incrementing floats from 0 to 1</returns>
		public float[] Partition(int n, float variation = 1f, bool addZero = false)
		{
			if (variation < 0)
				throw new ArgumentException("variation must be >= 0", "variation");

			if (addZero)
				n++;

			float[] dist = new float[n];
			if (addZero)
				dist[0] = 0f;

			float sum = 0f;
			for (int i = addZero ? 1 : 0; i < n; i++)
				sum += dist[i] = GetFloatRange(1f, variation);

			float revsum = 1f / sum;
			sum = 0f;
			for (int i = 0; i < n; i++)
			{
				dist[i] *= revsum;
				float td = dist[i];
				dist[i] = dist[i] + sum;
				sum += td;
			}

			return dist;
		}


		/// <summary>
		/// Generates an array of values incerementing from min to max. The values are limits of intervals, dividing space in an uniform way.
		/// 
		/// An easy way to understand this is that each interval will have a length of 1 + (random between 0 and variance). 
		/// The sum of lengths will then be normalized.
		/// </summary>
		/// <param name="n">number of intervals</param>
		/// <param name="variation">how much should the intevals differ one from another? 1 is </param>
		/// <param name="min">Start of first interval.</param>
		/// <param name="max">End of last interval.</param>
		/// <param name="addZero">Should a zero be added at the start of the array? This adds one to number of intervals</param>
		/// <returns>an array of incrementing floats from min to max</returns>
		public float[] Partition(int n, float variation, float min, float max, bool addZero = false)
		{
			float[] dist = Partition(n, variation);

			for (int i = 0; i < n; i++)
				dist[i] = dist[i] * (max - min) + min;

			return dist;
		}


		/// <summary>
		/// An helper for easy propability switching for use with switch language construct.
		/// 
		/// Usage example:
		///   switch(rng.Switch(4, 2, 1)) {
		/// 	case 0: break;	// happens 4/7 times (7=4+2+1)
		/// 	case 1: break;	// happens 2/7 times
		/// 	case 2: break;	// happens 1/7 times
		///   }
		/// </summary>
		/// <param name="propabilities">Propabilities.</param>
		public int Switch(params uint[] propabilities)
		{
			uint sum = 0;
			foreach (var p in propabilities)
				sum += p;
			var pr = GetIntRange(0, (int)sum - 1);

			sum = 0;
			for (int k = 0; k < propabilities.Length - 1; k++)
			{
				sum += propabilities[k];
				if (pr < sum)
					return k;
			}

			return propabilities.Length - 1;
		}
		#endregion




#if !NO_UNITY
		#region two dimensional geometries

		/// <summary>
		/// Generates a point on an edge of a 2D unit circle (radius = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 2D Vector whose both values are between -1 and 1 and it's magnitude is 1</returns>
		public U.Vector2 GetOnUnitCircle()
		{
			float a = GetFloat01() * U.Mathf.PI * 2;
			return new U.Vector2(U.Mathf.Sin(a), U.Mathf.Cos(a));
		}


		/// <summary>
		/// Generates a point in a 2D unit circle (radius = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 2D Vector whose both values are between -1 and 1 and it's magnitude is between 0 and 1</returns>
		public U.Vector2 GetInUnitCircle()
		{
			U.Vector2 v;
			do
			{
				v.x = GetFloatRange(-1, 1);
				v.y = GetFloatRange(-1, 1);
			} while (v.sqrMagnitude > 1);
			return v;
		}


		/// <summary>
		/// Generates a point in a 2D unit square (edge length = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 2D Vector whose both values are between 0 and 1.</returns>
		public U.Vector2 GetInUnitSquare() 
		{
			return new U.Vector2 (GetFloat01(), GetFloat01());
		}


		/// <summary>
		/// Generates a point on an edge of a 2D unit square (edge length = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 2D Vector whose one value is between 0 and 1 and other is 0 or 1</returns>
		public U.Vector2 GetOnUnitSquare() 
		{
			switch (Switch (1, 1, 1, 1)) {
				default:
				case 0: return new U.Vector2 (0f, GetFloat01());
				case 1: return new U.Vector2 (1f, GetFloat01());
				case 2: return new U.Vector2 (GetFloat01(), 0f);
				case 3: return new U.Vector2 (GetFloat01(), 1f);
			};
		}
		#endregion



		#region three dimensional geometries
		/// <summary>
		/// Generates a point in a unit sphere (radius = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 3D Vector whose all values are between -1 and 1 and it's magnitude is between 0 and 1</returns>
		public U.Vector3 GetInUnitSphere()
		{
			U.Vector3 v;
			do
			{
				v.x = GetFloatRange(-1, 1);
				v.y = GetFloatRange(-1, 1);
				v.z = GetFloatRange(-1, 1);
			} while (v.sqrMagnitude > 1);
			return v;
		}


		/// <summary>
		/// Generates a point in on the surface of a unit sphere (radius = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 3D Vector whose all values are between -1 and 1 and it's magnitude is 1</returns>
		public U.Vector3 GetOnUnitSphere()
		{
			return GetQuaternion() * U.Vector3.forward;
		}


		/// <summary>
		/// Generates a point in a unit cube (edge length = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 3D Vector whose all values are between 0 and 1</returns>
		public U.Vector3 GetInUnitCube()
		{
			return new U.Vector3(GetFloat01(), GetFloat01(), GetFloat01());
		}


		/// <summary>
		/// Generates a point in on the surface of a unit cube (edge length = 1). Uniform distribution.
		/// </summary>
		/// <returns>A 3D Vector whose all values are between -1 and 1 and it's magnitude is 1</returns>
		public U.Vector3 GetOnUnitCube()
		{
			switch (Switch(1, 1, 1, 1, 1, 1))
			{
				default:
				case 0: return new U.Vector3(0f, GetFloat01(), GetFloat01());
				case 1: return new U.Vector3(1f, GetFloat01(), GetFloat01());
				case 2: return new U.Vector3(GetFloat01(), 0f, GetFloat01());
				case 3: return new U.Vector3(GetFloat01(), 1f, GetFloat01());
				case 4: return new U.Vector3(GetFloat01(), GetFloat01(), 0f);
				case 5: return new U.Vector3(GetFloat01(), GetFloat01(), 1f);
			}
		}
		#endregion




		#region quaternions
		/// <summary>
		/// Generates a uniform quaternion.
		/// </summary>
		/// <returns>A quaternion that points in a random direction.</returns>
		public U.Quaternion GetQuaternion()
		{
			// Effective Sampling and Distance Metrics for 3D Rigid Body Path Planning by James J. Kuffner
			// https://www-preview.ri.cmu.edu/pub_files/pub4/kuffner_james_2004_1/kuffner_james_2004_1.pdf
			float s = GetFloat01();
			float sigma1 = U.Mathf.Sqrt(1f - s);
			float sigma2 = U.Mathf.Sqrt(s);
			float theta1 = U.Mathf.PI * 2 * GetFloat01();
			float theta2 = U.Mathf.PI * 2 * GetFloat01();
			return new U.Quaternion(
				U.Mathf.Cos(theta2) * sigma2,
				U.Mathf.Sin(theta1) * sigma1,
				U.Mathf.Cos(theta1) * sigma1,
				U.Mathf.Sin(theta2) * sigma2
			);
		}
		#endregion




		#region collection helpers
		/// <summary>
		/// Returns a random element from a collection with uniform distribution.
		/// </summary>
		/// <typeparam name="T">generic type of the collection</typeparam>
		/// <param name="collection">the collection</param>
		/// <returns>An element from provided collection</returns>
		public T GetRandomElement<T>(IList<T> collection)
		{
			int idx = GetIntRange(0, collection.Count - 1);
			return collection[idx];
		}


		/// <summary>
		/// Returns a random element from an array with uniform distribution.
		/// </summary>
		/// <typeparam name="T">generic type of the array</typeparam>
		/// <param name="array">the array</param>
		/// <returns>An element from provided array</returns>
		public T GetRandomElement<T>(T[] array)
		{
			int idx = GetIntRange(0, array.Length - 1);
			return array[idx];
		}


		/// <summary>
		/// Returns the collection after shuffling, does not modify the original.
		/// </summary>
		/// <returns>The shuffled collection.</returns>
		/// <param name="t">original collection</param>
		/// <typeparam name="T">generic type of the collection.</typeparam>
		public List<T> GetShuffled<T>(IList<T> t)
		{
			List<int> indices = new List<int> (t.Count - 1);

			for (int i = 0; i < t.Count; i++)
				indices.Add (i);

			List<T> ret = new List<T> ();
			while (indices.Count > 0) {
				int idx = GetIntRange (0, indices.Count - 1);
				ret.Add (t [indices [idx]]);
				indices.RemoveAt (idx);
			}

			return ret;
		}
		#endregion

#endif
	}




	/// <summary>
	/// Random generator extentions for collection handling.
	/// </summary>
	public static class RandomGeneratorExtentions
	{

		/// <summary>
		/// Returns a random element from a collection with uniform distribution.
		/// </summary>
		/// <typeparam name="T">generic type of the collection</typeparam>
		/// <param name="t">the collection</param>
		/// <param name="rng">a random generator instance to perform this, the global random generator will be used if not provided</param>
		/// <returns>An element from provided collection</returns>
		public static T GetRandomElement<T>(this IList<T> t, RandomGenerator rng = null)
		{
			return (rng ?? RandomGenerator.global).GetRandomElement<T>(t);
		}


		/// <summary>
		/// Returns a random element from a collection with uniform distribution.
		/// </summary>
		/// <typeparam name="T">generic type of the collection</typeparam>
		/// <param name="t">the array</param>
		/// <param name="rng">a random generator instance to perform this, the global random generator will be used if not provided</param>
		/// <returns>An element from provided array</returns>
		public static T GetRandomElement<T>(this T[] t, RandomGenerator rng = null)
		{
			return (rng ?? RandomGenerator.global).GetRandomElement<T>(t);
		}


		/// <summary>
		/// Returns the collection after shuffling, does not modify the original.
		/// </summary>
		/// <returns>The shuffled collection.</returns>
		/// <param name="t">original collection</param>
		/// <param name="rng">a random generator instance to perform this, the global random generator will be used if not provided</param>
		/// <typeparam name="T">generic type of the collection.</typeparam>
		public static List<T> GetShuffled<T>(this IList<T> t, RandomGenerator rng = null)
		{
			return (rng ?? RandomGenerator.global).GetShuffled (t);
		}


		/// <summary>
		/// Returns the array after shuffling, does not modify the original.
		/// </summary>
		/// <returns>The shuffled array.</returns>
		/// <param name="t">original array</param>
		/// <param name="rng">a random generator instance to perform this, the global random generator will be used if not provided</param>
		/// <typeparam name="T">generic type of the array.</typeparam>
		public static T[] GetShuffled<T>(this T[] t, RandomGenerator rng = null)
		{
			return new List<T>(t).GetShuffled(rng).ToArray();
		}

	}

}
