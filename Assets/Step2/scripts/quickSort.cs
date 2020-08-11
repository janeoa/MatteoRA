//
// The Tripartite conditional enables Bentley-McIlroy 3-way Partitioning.
// This performs additional compares to isolate islands of keys equal to
// the pivot value.  Use unless key-equivalent classes are of small size.
//
#define Tripartite
 
namespace Sort {
  using System;
  using System.Diagnostics;
 
  class QuickSort<T> where T : IComparable {
    #region Constants
    private const Int32 INSERTION_LIMIT_DEFAULT = 12;
    #endregion
 
    #region Properties
    public Int32 InsertionLimit { get; set; }
    private Random Random { get; set; }
    private T Median { get; set; }
 
    private Int32 Left { get; set; }
    private Int32 Right { get; set; }
    private Int32 LeftMedian { get; set; }
    private Int32 RightMedian { get; set; }
    #endregion
 
    #region Constructors
    public QuickSort(Int32 insertionLimit, Random random) {
      this.InsertionLimit = insertionLimit;
      this.Random = random;
    }
 
    public QuickSort(Int32 insertionLimit)
      : this(insertionLimit, new Random()) {
    }
 
    public QuickSort()
      : this(INSERTION_LIMIT_DEFAULT) {
    }
    #endregion
 
    #region Sort Methods
    public void Sort(T[] entries) {
      Sort(entries, 0, entries.Length - 1);
    }
 
    public void Sort(T[] entries, Int32 first, Int32 last) {
      var length = last + 1 - first;
      while (length > 1) {
        if (length < InsertionLimit) {
          InsertionSort<T>.Sort(entries, first, last);
          return;
        }
 
        Left = first;
        Right = last;
        pivot(entries);
        partition(entries);
        //[Note]Right < Left
 
        var leftLength = Right + 1 - first;
        var rightLength = last + 1 - Left;
 
        //
        // First recurse over shorter partition, then loop
        // on the longer partition to elide tail recursion.
        //
        if (leftLength < rightLength) {
          Sort(entries, first, Right);
          first = Left;
          length = rightLength;
        }
        else {
          Sort(entries, Left, last);
          last = Right;
          length = leftLength;
        }
      }
    }
 
    private void pivot(T[] entries) {
      //
      // An odd sample size is chosen based on the log of the interval size.
      // The median of a randomly chosen set of samples is then returned as
      // an estimate of the true median.
      //
      var length = Right + 1 - Left;
      var logLen = (Int32)Math.Log10(length);
      var pivotSamples = 2 * logLen + 1;
      var sampleSize = Math.Min(pivotSamples, length);
      var last = Left + sampleSize - 1;
      // Sample without replacement
      for (var first = Left; first <= last; first++) {
        // Random sampling avoids pathological cases
        var random = Random.Next(first, Right + 1);
        Swap(entries, first, random);
      }
 
      InsertionSort<T>.Sort(entries, Left, last);
      Median = entries[Left + sampleSize / 2];
    }
 
    private void partition(T[] entries) {
      var first = Left;
      var last = Right;
#if Tripartite
      LeftMedian = first;
      RightMedian = last;
#endif
      while (true) {
        //[Assert]There exists some index >= Left where entries[index] >= Median
        //[Assert]There exists some index <= Right where entries[index] <= Median
        // So, there is no need for Left or Right bound checks
        while (Median.CompareTo(entries[Left]) > 0) Left++;
        while (Median.CompareTo(entries[Right]) < 0) Right--;
 
        //[Assert]entries[Right] <= Median <= entries[Left]
        if (Right <= Left) break;
 
        Swap(entries, Left, Right);
        swapOut(entries);
        Left++;
        Right--;
        //[Assert]entries[first:Left - 1] <= Median <= entries[Right + 1:last]
      }
 
      if (Left == Right) {
        Left++;
        Right--;
      }
      //[Assert]Right < Left
      swapIn(entries, first, last);
 
      //[Assert]entries[first:Right] <= Median <= entries[Left:last]
      //[Assert]entries[Right + 1:Left - 1] == Median when non-empty
    }
    #endregion
 
    #region Swap Methods
    [Conditional("Tripartite")]
    private void swapOut(T[] entries) {
      if (Median.CompareTo(entries[Left]) == 0) Swap(entries, LeftMedian++, Left);
      if (Median.CompareTo(entries[Right]) == 0) Swap(entries, Right, RightMedian--);
    }
 
    [Conditional("Tripartite")]
    private void swapIn(T[] entries, Int32 first, Int32 last) {
      // Restore Median entries
      while (first < LeftMedian) Swap(entries, first++, Right--);
      while (RightMedian < last) Swap(entries, Left++, last--);
    }
 
    public static void Swap(T[] entries, Int32 index1, Int32 index2) {
      if (index1 != index2) {
        var entry = entries[index1];
        entries[index1] = entries[index2];
        entries[index2] = entry;
      }
    }
    #endregion
  }
 
  #region Insertion Sort
  static class InsertionSort<T> where T : IComparable {
    public static void Sort(T[] entries, Int32 first, Int32 last) {
      for (var index = first + 1; index <= last; index++)
        insert(entries, first, index);
    }
 
    private static void insert(T[] entries, Int32 first, Int32 index) {
      var entry = entries[index];
      while (index > first && entries[index - 1].CompareTo(entry) > 0)
        entries[index] = entries[--index];
      entries[index] = entry;
    }
  }
  #endregion
}