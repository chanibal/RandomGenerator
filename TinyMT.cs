using System;

/// <summary>
/// Ported from TinyMT32 version 1.1.1 (2016.05.13) by Mutsuo Saitoand Makoto Matsumoto 
/// port to C# by Krzysztof Bociurko on 2016-06-28
/// original license: The 3-clause BSD License
/// </summary>
public class TinyMT
{

	const UInt32 TINYMT32_MASK = 0x7fffffff;
	//		const float TINYMT32_MUL = (1.0f / 16777216.0f);


	#region state
	/// <summary>
	/// tinymt32 internal state vector and parameters
	/// </summary>
	UInt32[] status = new UInt32[4];
	UInt32 mat1;
	UInt32 mat2;
	UInt32 tmat;
	#endregion


	/// <summary>
	/// This function certificate the period of 2^127-1.
	/// </summary>
	void period_certification ()
	{
		if ((status [0] & TINYMT32_MASK) == 0 &&
		    status [1] == 0 &&
		    status [2] == 0 &&
		    status [3] == 0) {
			status [0] = 'T';
			status [1] = 'I';
			status [2] = 'N';
			status [3] = 'Y';
		}
	}


	/// <summary>
	/// This function initializes the internal state array with a 32-bit unsigned integer seed.
	/// </summary>
	/// <param name="seed">a 32-bit unsigned integer used as a seed.</param>
	/// <param name="seed_mat1">mat1 from the state vector, will be used as additional seed</param>
	/// <param name="seed_mat2">mat2 from the state vector, will be used as additional seed</param>
	/// <param name="seed_tmat">tmat from the state vector, will be used as additional seed</param>
	public TinyMT (UInt32 seed, UInt32 seed_mat1 = 0, UInt32 seed_mat2 = 0, UInt32 seed_tmat = 0)
	{
		const int MIN_LOOP = 8;
		const int PRE_LOOP = 8;

		status [0] = seed;
		status [1] = mat1 = seed_mat1;
		status [2] = mat2 = seed_mat2;
		status [3] = tmat = seed_tmat;
		for (int i = 1; i < MIN_LOOP; i++) {
			status [i & 3] ^= (uint)(
				i + (1812433253)
				* (status [(i - 1) & 3]
				^ (status [(i - 1) & 3] >> 30))
			);
		}
		period_certification ();
		for (int i = 0; i < PRE_LOOP; i++) {
			Next ();
		}
	}


	/// <summary>
	/// This function changes internal state of tinymt32.
	/// Users should not call this function directly.
	/// </summary>
	void Next ()
	{
		const int TINYMT32_SH0 = 1;
		const int TINYMT32_SH1 = 10;


		UInt32 x;
		UInt32 y;

		y = status [3];
		x = (status [0] & TINYMT32_MASK)
		^ status [1]
		^ status [2];
		x ^= (x << TINYMT32_SH0);
		y ^= (y >> TINYMT32_SH0) ^ x;
		status [0] = status [1];
		status [1] = status [2];
		status [2] = x ^ (y << TINYMT32_SH1);
		status [3] = y;
		status [1] ^= (uint)(-((Int32)(y & 1)) & mat1);
		status [2] ^= (uint)(-((Int32)(y & 1)) & mat2);
	}


	/// <summary>
	/// This function outputs 32-bit unsigned integer from internal state.
	/// Users should not call this function directly.
	/// </summary>
	/// <returns>32-bit unsigned pseudorandom number</returns>
	UInt32 Temper ()
	{
		const int TINYMT32_SH8 = 8;

		UInt32 t0, t1;
		t0 = status [3];
		t1 = status [0]
		+ (status [2] >> TINYMT32_SH8);
		t0 ^= t1;
		t0 ^= (uint)(-((Int32)(t1 & 1)) & tmat);
		return t0;
	}


	/// <summary>
	/// This function outputs 32-bit unsigned integer from internal state.
	/// </summary>
	/// <returns>32-bit unsigned integer r (0 <= r < 2^32)</returns>
	public UInt32 GetUint32 ()
	{
		Next();
		return Temper ();
	}


}
