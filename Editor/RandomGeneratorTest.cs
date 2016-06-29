using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using ChanibaL;


public class RandomGeneratorTest
{
	const float largeEpsilon = 0.00001f;
	const float veryLargeEpsilon = 0.001f;

	/// <summary>
	/// Warning: do not use geometry functions with constant mocked values - they will hang
	/// </summary>
	class RandomGeneratorMock : RandomGenerator
	{
		public UInt32 mock;
		public override UInt32 GetUInt32() { return mock; }
		public RandomGeneratorMock(UInt32 mock) { this.mock = mock; }
	}


	[Test]
	public void TestPrecalculated()
	{
		// generated in reference implementation
		UInt32[] vals = new UInt32[]
		{
			0xa564a7ba, 0xdda6773c, 0x99452fe1, 0x0123936a, 0xe888e74e,
			0xc2290110, 0x7cb0a271, 0xc384ec27, 0x968bd6bb, 0x16caca33,
			0x7c651dea, 0xc8672f20, 0x16a5bd73, 0xf6a3ae2e, 0x3ac5531f,
			0xe2511090, 0x462fba19, 0x2737256e, 0xe1fc7209, 0x396ef346,
			0x1463328d, 0xe2ac8621, 0x0f3a2b0b, 0x3e7ee4a6, 0xc4a79f91,
			0xe664b78d, 0x639bf566, 0x610d0662, 0x4cb51b00, 0xe5b78b47,
			0x8c031703, 0x82607f49, 0x415a38f4, 0x12015244, 0x8d33956e,
			0x096d207a, 0xcf579bf3, 0x4404612b, 0x71d31686, 0x3f026479,
			0xf755a15c, 0xbc291ce7, 0x8fbb4cba, 0x9a2397f9, 0x71bed3bf,
			0x91667531, 0x228362b4, 0xa2ebac84, 0xe8a1dac5, 0x76e40102
		};
		RandomGenerator rng = new RandomGenerator(1, 3, 3, 7);
		for (int i = 0; i < vals.Length; i++)
			Assert.AreEqual(vals[i], rng.GetUInt32(), "wrong value " + i + " of " + vals.Length);
	}


	[Test]
	public void TestFloats()
	{
		Assert.AreEqual(0f, new RandomGeneratorMock(0).GetFloat01(), largeEpsilon, "float 0");
		Assert.AreEqual(0.25f, new RandomGeneratorMock(UInt32.MaxValue / 4).GetFloat01(), largeEpsilon, "float 1/4");
		Assert.AreEqual(0.50f, new RandomGeneratorMock(UInt32.MaxValue / 2).GetFloat01(), largeEpsilon, "float 1/2");
		Assert.AreEqual(1f, new RandomGeneratorMock(UInt32.MaxValue).GetFloat01(), largeEpsilon, "float 1");
	}


	[Test]
	public void TestPartition()
	{
		RandomGenerator rng = new RandomGenerator(1, 3, 3, 7);

		Assert.AreEqual(rng.Partition (5, 1, false).Length, 5, "partitions length without added zero");
		Assert.AreEqual(rng.Partition (5, 1, true).Length, 6, "partitions length with added zero");

		float[] d = rng.Partition(5, 1, true);
		for (int i = 1; i < 6; i++)
			Assert.AreEqual(1f/5, d[i] - d[i - 1], veryLargeEpsilon, "no variance test");

		for(int i = 1; i < 6; i++)
			Assert.GreaterOrEqual(d [i], d [i - 1], "each number is larger than its predecessor");
	}

}
