using System;
using System.Collections;
using System.Collections.Generic;
using Permutations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;


namespace PermutationsTests {

    [TestClass]
    public class PermutationClassTest {

        [TestMethod]
        public void TestInit() {
            var cycles = new List<Cycle>();
            var disjoint = new HashSet<Cycle>();

            cycles.Add(new Cycle(1, 2, 3));
            cycles.Add(new Cycle(4, 5, 2, 3));
            cycles.Add(new Cycle(8, 1, 2));
            cycles.Add(new Cycle(1, 6, 7, 2));

            disjoint.Add(new Cycle(1, 2, 8, 7, 6));
            disjoint.Add(new Cycle(3, 4, 5));

            var perm = new Permutation(cycles, -8501);

            Assert.IsFalse(perm.input_disjoint());
            Assert.IsFalse(perm.input_transpositions());
            Assert.IsFalse(perm.is_trivial());

            Assert.AreEqual(perm.power, -8501);
            Assert.AreEqual(perm.order, 15);
            Assert.AreEqual(perm.sign, 1);

            var dperm = new Permutation(disjoint.ToList());

            Assert.IsTrue(dperm.input_disjoint());
            Assert.IsFalse(dperm.input_transpositions());
            Assert.IsFalse(dperm.is_trivial());

            Assert.AreEqual(dperm.power, 1);
            Assert.AreEqual(dperm.order, 15);
            Assert.AreEqual(dperm.sign, 1);

            Assert.IsTrue(perm.disjoint_cycles.SetEquals(dperm.disjoint_cycles));
            Assert.AreEqual(perm, dperm);
            Assert.AreEqual(perm.format_input(), "[(1 2 3)(4 5 2 3)(8 1 2)(1 6 7 2)] ^ -8501");
            Assert.AreEqual(dperm.format_input(), "(1 2 8 7 6)(3 4 5)");
            Assert.AreEqual(perm.one_line_notation(), "2 8 4 5 3 1 6 7");
            Assert.AreEqual(dperm.one_line_notation(), "2 8 4 5 3 1 6 7");
        }

        [TestMethod]
        public void TestGenerate() {
            var perm = Permutation.generate();
            Assert.IsInstanceOfType(perm, typeof(Permutation));
        }
                
        [TestMethod]
        public void TestFromString() {
            var perm = Permutation.from_string("( 1  2 3   4 5 )   (6         7)(8 )");
            Assert.AreEqual(perm,
                new Permutation( new Cycle[] { new Cycle(1, 2, 3, 4, 5), new Cycle(6, 7), new Cycle(8) }.ToList()));
            // Permutation.from_string is *very* fault-tolerable, so these examples
            // are like super bad
            var bad = new string[] {"", "()", "(1 2 3", "(1, 2, 3)(4, 5)"};
            foreach (var s in bad) {
                try {
                    Permutation.from_string(s);
                    Assert.Fail();
                } catch (Exception e) {
                    if (e is AssertFailedException)
                         throw;
                }
            }
        }

        [TestMethod]
        public void TestPower() {
            var cycles = new List<Cycle>();
            cycles.Add(new Cycle(1, 2, 8, 7, 6));
            cycles.Add(new Cycle(3, 4, 5));

            var perm = new Permutation(cycles);

            Assert.AreEqual(
                Permutation.from_string("(3 5 4)(8)"),
                perm.pow(-85)
            );

            Assert.AreEqual(
                Permutation.from_string("(1 2 8 7 6)(3 4 5)"),
                perm.pow(16)
            );

            Assert.AreEqual(
                Permutation.from_string("(1 6 7 8 2)"),
                perm.pow(39)
            );
        }

        [TestMethod]
        public void TestNotations() {
            var perm = Permutation.from_string("(1 8 9 3)(5 2 3 1)(1 2 3 4)");
            Assert.AreEqual(perm.one_line_notation(), "1 8 4 5 2 6 7 9 3");
        }
    }
}
