using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Permutations;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace PermutationsTests {

    [TestClass]
    public class CycleClassTests
    {

       [TestMethod]
       public void TestInit() {

           // No duplicate elements allowed
           try {
               new Cycle(1, 1);
               Assert.Fail();
           } catch (Exception e) {
               if (e is AssertFailedException)
                   throw;
           }

           var cycle = new Cycle(5, 1, 2, 3);
           Assert.AreEqual(cycle.length, 4);
           Assert.IsTrue(cycle.ordered.SequenceEqual(new int[] {1, 2, 3, 5}));
           Assert.AreEqual(cycle, new Cycle(new int[] {1, 2, 3, 5}));
           Assert.AreEqual(cycle.format_input(), "(5 1 2 3)");
           Assert.AreEqual(cycle.ToString(), "(1 2 3 5)");
       }

       [TestMethod]
       public void TestGenerate() {
           var cycl = Cycle.generate();
           Assert.IsInstanceOfType(cycl, typeof(Cycle));
       }       
    
       [TestMethod]
       public void TestFromString() {
            var cycle = Cycle.from_string("    (2 6    3 1)    ");
            Assert.AreEqual(cycle, new Cycle(1, 2, 6, 3));

            var bad = new string[] {"", "1,2, 3", " ( )", "(1 2 1)", "(1 2 3"};
            foreach (var s in bad) {
                try {
                    Cycle.from_string(s);
                    Assert.Fail();
                } catch (Exception e) {
                    if (e is AssertFailedException)
                         throw;
                }
            }
       }

       [TestMethod]
       public void TestRotationInversion() {
           // rotate_left and rotate_right are just small utility functions
           var cycle = Cycle.from_string("(4 3 2 1)");

           Assert.IsTrue(cycle.rotated_left(2).SequenceEqual(new int[] {2, 1, 4, 3}.ToList()));
           Assert.IsTrue(cycle.rotated_left(0).SequenceEqual(new int[] {4, 3, 2, 1}.ToList()));
           Assert.IsTrue(cycle.rotated_left(4).SequenceEqual(new int[] {4, 3, 2, 1}.ToList()));

           Assert.IsTrue(cycle.rotated_right(2).SequenceEqual(new int[] {2, 1, 4, 3}.ToList()));
           Assert.IsTrue(cycle.rotated_right(0).SequenceEqual(new int[] {4, 3, 2, 1}.ToList()));
           Assert.IsTrue(cycle.rotated_right(4).SequenceEqual(new int[] {4, 3, 2, 1}.ToList()));

           Assert.AreEqual(cycle.reversed(), new Cycle(1, 2, 3, 4));
       }
    }
}
