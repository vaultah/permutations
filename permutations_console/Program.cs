using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permutations_console {

    class Program {
        static void Main(string[] args) {
            string command;
            Permutation permutation;
            while (true) {                
                Console.Write("> ");
                command = Console.ReadLine();
                // generate random permutation in case of empty input
                if (String.IsNullOrWhiteSpace(command)) {
                    permutation = Permutation.generate();
                } else {
                    try {
                        permutation = Permutation.from_string(command);
                    } catch (Exception e) {
                        Console.WriteLine(e);
                        continue;
                    }
                }
                
                PrintInfo(permutation);
            }
        }

        static void PrintInfo(Permutation perm) {
            Console.WriteLine("Input:\n  {0}", perm.format_input());
            Console.WriteLine("Disjoint cycles:\n  {0}", perm);
            Console.WriteLine("Permutation rules:\n  {0}", perm.permutation_rules());
            Console.WriteLine("Inverse:\n  {0}", perm.inverse());
            Console.WriteLine("One line notation:\n  {0}", perm.one_line_notation());
            Console.WriteLine("Signature:\n  {0}", perm.sign);
            Console.WriteLine("Parity:\n  {0}", perm.sign > 0 ? "even" : "odd");
            Console.WriteLine("Order:\n  {0}", perm.order);
            Console.WriteLine("Trivial:\n  {0}", perm.is_trivial());
        }
    }
}
