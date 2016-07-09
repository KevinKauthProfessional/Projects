namespace Poker.Utility.HandEvaluation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	/// <summary>
	/// This class represents a fuzzy set distribution and contains operations related to
	/// it's manipulation.
	/// </summary>
	public class StrengthWave
	{
		/// <summary>
		/// The number of "Buckets" to separate values into for the purpose of translating the 
		/// values used for creation into an approximation of a continuous function.
		/// </summary>
		public const int BucketCount = 10;

		/// <summary>
		/// The maximum number of values allowed before downsampling.
		/// </summary>
		private const int ValueCountMax = 1000;

		/// <summary>
		/// A delimiter character to convert to and from string.
		/// </summary>
		private const char Delimiter = '|';

		/// <summary>
		/// The raw list of values contained in the fuzzy set
		/// </summary>
		private List<float> valueList = new List<float>();

		/// <summary>
		/// An array of buckets used to approximate a continuous function.
		/// All the values of this array should sum to 1.0f.
		/// </summary>
		private float[] bucketPercentages = new float[BucketCount];

		/// <summary>
		/// Initializes a new instance of the FuzzySet class.
		/// </summary>
		/// <param name="singleValue">A single value to populate the set with</param>
		internal StrengthWave(float singleValue)
		{
			if (singleValue < 0.0f || singleValue > 1.0f)
			{
				throw new ArgumentException("Value out of range: " + singleValue.ToString());
			}

			this.valueList.Add(singleValue);
			this.FillBuckets();
		}

		/// <summary>
		/// Initializes a new instance of the FuzzySet class.
		/// </summary>
		/// <param name="setsToCombine">A list of FuzzySet objects to combine into a single list</param>
		internal StrengthWave(List<StrengthWave> setsToCombine)
		{
			// For each set coming in
			foreach (StrengthWave set in setsToCombine)
			{
				// Add the value list
				this.valueList.AddRange(set.valueList);
			}

			// Downsample our value list
			this.valueList = this.DownSampleValuesList(this.valueList);

			// Fill our function buckets
			this.FillBuckets();
		}

		/// <summary>
		/// Initializes a new instance of the FuzzySet class.
		/// </summary>
		/// <param name="fuzzySetString">A string representation of a FuzzySet</param>
		internal StrengthWave(string fuzzySetString)
		{
			// Split the string by newline character
			string[] valueSplit = fuzzySetString.Split(Delimiter);

			// For each line split
			for (int i = 0; i < valueSplit.Length; ++i)
			{
				// Convert the contents to a float and add it as a value
				float value = float.Parse(valueSplit[i]);
				this.valueList.Add(value);
			}

			// Fill our function buckets
			this.FillBuckets();
		}

		/// <summary>
		/// Gets the mean value of the set
		/// </summary>
		internal float Mean
		{
			get
			{
				// Add up all our values
				float sum = 0.0f;
				foreach (float value in this.valueList)
				{
					sum += value;
				}

				// Return average
				return sum / (float)this.valueList.Count;
			}
		}

		public float this[int i]
		{
			get
			{
				return this.bucketPercentages[i];
			}
		}

		/// <summary>
		/// An override of the ToString() function. Produces a string representation of 
		/// the approximation of the function represented by this set, listed by X axis step.
		/// Can be written to a .csv file and graphed.
		/// </summary>
		/// <returns>A string representation of this FuzzySet</returns>
		public override string ToString()
		{
			// Initialize a result
			string result = string.Empty;

			// For each fuction bucket
			for (int i = 0; i < BucketCount; ++i)
			{
				// Write bucket contents on their own line
				result += this.bucketPercentages[i].ToString() + Delimiter;
			}

			return result;
		}

		/// <summary>
		/// A helper method for the constructors to fill the buckets approximating the 
		/// continuous function derived from the passed values.
		/// </summary>
		private void FillBuckets()
		{
			float percentagePerValue = 1.0f /(float)this.valueList.Count;

			// For each value in our values list
			foreach (float value in this.valueList)
			{
				// Multiply by the number of buckets and truncate (floor) to get the bucket index
				int bucketIndex = (int)(value * BucketCount);

				// Prevent off by 1 error
				if (bucketIndex == BucketCount)
				{
					bucketIndex--;
				}

				// Increment the count of that bucket
				this.bucketPercentages[bucketIndex] += percentagePerValue;
			}
		}

		/// <summary>
		/// A method for reducing the number of values we have.  Simply averages pairs of values
		/// together, cutting the value list size in half until we are below the limit
		/// defined by the constant at the top of the class.
		/// </summary>
		private List<float> DownSampleValuesList(List<float> listIn)
		{
			// If we are already below the limit
			if (listIn.Count <= ValueCountMax)
			{
				// Do nothing
				return listIn;
			}

			// Initialize a results list
			List<float> resultList = new List<float>();

			// For every two values in the value list
			for (int i = 0; i < listIn.Count - 1; i += 2)
			{
				// Average together and add a value to the results list
				float average = (listIn[i] + listIn[i + 1]) / 2.0f;
				resultList.Add(average);
			}

			// Return the result list downsampled
			return this.DownSampleValuesList(resultList);
		}
	}
}
